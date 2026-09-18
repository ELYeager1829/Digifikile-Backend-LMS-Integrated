using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Group;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Groups;

// ================================================================
// CREATE GROUP HANDLER
// ================================================================

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, BaseResponse<GroupDto>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IAdministratorRepository _adminRepository;

    public CreateGroupCommandHandler(IGroupRepository groupRepository, IAdministratorRepository adminRepository)
    {
        _groupRepository = groupRepository;
        _adminRepository = adminRepository;
    }

    public async Task<BaseResponse<GroupDto>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        // Controller supplies the authenticated UserAccount id. Resolve the SETA profile id
        // here so callers can never choose another administrator by posting an arbitrary id.
        var admin = await _adminRepository.GetByUserIdAsync(request.setaAdministratorId, cancellationToken)
                    ?? await _adminRepository.GetByIdAsync(request.setaAdministratorId, cancellationToken);
        if (admin == null)
            return BaseResponse<GroupDto>.Failure("SETA Administrator not found");

        var group = Group.Create(
            admin.Id,
            request.setaProgrammeId,
            request.GroupName,
            request.Description);

        await _groupRepository.AddAsync(group, cancellationToken);
        await _groupRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<GroupDto>.Success(new GroupDto
        {
            Id = group.Id,
            setaAdministratorId = group.setaAdministratorId,
            setaAdministratorName = $"{admin.User.Name} {admin.User.Surname}",
            setaProgrammeId = group.setaProgrammeId,
            GroupName = group.GroupName,
            Description = group.Description,
            CreatedAt = group.CreatedAt
        });
    }
}

// ================================================================
// GET GROUPS BY ADMIN HANDLER
// ================================================================

public class GetGroupsByAdministratorQueryHandler : IRequestHandler<GetGroupsByAdministratorQuery, BaseResponse<List<GroupDto>>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IAdministratorRepository _adminRepository;

    public GetGroupsByAdministratorQueryHandler(IGroupRepository groupRepository, IAdministratorRepository adminRepository)
    {
        _groupRepository = groupRepository;
        _adminRepository = adminRepository;
    }

    public async Task<BaseResponse<List<GroupDto>>> Handle(GetGroupsByAdministratorQuery request, CancellationToken cancellationToken)
    {
        var admin = await _adminRepository.GetByUserIdAsync(request.AdministratorId, cancellationToken)
                    ?? await _adminRepository.GetByIdAsync(request.AdministratorId, cancellationToken);
        if (admin == null)
            return BaseResponse<List<GroupDto>>.Failure("SETA Administrator not found");

        var groups = await _groupRepository.GetByAdministratorAsync(admin.Id, cancellationToken);
        var groupList = groups.ToList();

        var items = groupList.Select(g => new GroupDto
        {
            Id = g.Id,
            setaAdministratorId = g.setaAdministratorId,
            setaProgrammeId = g.setaProgrammeId,
            GroupName = g.GroupName,
            Description = g.Description,
            MemberCount = g.GroupEnrollments.Count,
            CreatedAt = g.CreatedAt
        }).ToList();

        return BaseResponse<List<GroupDto>>.Success(items);
    }
}
// ================================================================
// SECURE GROUP CRUD / MEMBERSHIP HANDLERS
// The RequestingUserId is always derived from the JWT by GroupsController.
// ================================================================

internal static class GroupOwnership
{
    public static async Task<SetaAdministrator?> ResolveAdminAsync(
        IAdministratorRepository adminRepository,
        int requestingUserId,
        CancellationToken cancellationToken) =>
        await adminRepository.GetByUserIdAsync(requestingUserId, cancellationToken);

    public static GroupDto ToDto(Group group) => new()
    {
        Id = group.Id,
        setaAdministratorId = group.setaAdministratorId,
        setaAdministratorName = group.setaAdministrator?.User == null
            ? string.Empty
            : $"{group.setaAdministrator.User.Name} {group.setaAdministrator.User.Surname}".Trim(),
        setaProgrammeId = group.setaProgrammeId,
        setaProgrammeName = group.setaProgramme?.ProgrammeName,
        GroupName = group.GroupName,
        Description = group.Description,
        MemberCount = group.GroupEnrollments.Count,
        CreatedAt = group.CreatedAt
    };
}

public class GetAllGroupsQueryHandler : IRequestHandler<GetAllGroupsQuery, BaseResponse<PagedResponse<GroupDto>>>
{
    private readonly IGroupRepository _groups;
    private readonly IAdministratorRepository _admins;

    public GetAllGroupsQueryHandler(IGroupRepository groups, IAdministratorRepository admins)
    {
        _groups = groups;
        _admins = admins;
    }

    public async Task<BaseResponse<PagedResponse<GroupDto>>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        var admin = await GroupOwnership.ResolveAdminAsync(_admins, request.RequestingUserId, cancellationToken);
        if (admin == null) return BaseResponse<PagedResponse<GroupDto>>.Failure("SETA Administrator not found");

        var all = (await _groups.GetByAdministratorAsync(admin.Id, cancellationToken)).ToList();
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).Select(GroupOwnership.ToDto).ToList();

        return BaseResponse<PagedResponse<GroupDto>>.Success(new PagedResponse<GroupDto>
        {
            Items = items,
            TotalCount = all.Count,
            Page = page,
            PageSize = pageSize
        });
    }
}

public class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery, BaseResponse<GroupDto>>
{
    private readonly IGroupRepository _groups;
    private readonly IAdministratorRepository _admins;

    public GetGroupByIdQueryHandler(IGroupRepository groups, IAdministratorRepository admins)
    {
        _groups = groups;
        _admins = admins;
    }

    public async Task<BaseResponse<GroupDto>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var admin = await GroupOwnership.ResolveAdminAsync(_admins, request.RequestingUserId, cancellationToken);
        var group = await _groups.GetByIdAsync(request.Id, cancellationToken);
        if (admin == null || group == null || group.setaAdministratorId != admin.Id)
            return BaseResponse<GroupDto>.Failure("Group not found");

        return BaseResponse<GroupDto>.Success(GroupOwnership.ToDto(group));
    }
}

public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, BaseResponse<GroupDto>>
{
    private readonly IGroupRepository _groups;
    private readonly IAdministratorRepository _admins;

    public UpdateGroupCommandHandler(IGroupRepository groups, IAdministratorRepository admins)
    {
        _groups = groups;
        _admins = admins;
    }

    public async Task<BaseResponse<GroupDto>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.GroupName))
            return BaseResponse<GroupDto>.Failure("Group name is required");

        var admin = await GroupOwnership.ResolveAdminAsync(_admins, request.RequestingUserId, cancellationToken);
        var group = await _groups.GetByIdAsync(request.Id, cancellationToken);
        if (admin == null || group == null || group.setaAdministratorId != admin.Id)
            return BaseResponse<GroupDto>.Failure("Group not found");

        group.UpdateDetails(request.GroupName.Trim(), string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim());
        await _groups.UpdateAsync(group, cancellationToken);
        await _groups.SaveChangesAsync(cancellationToken);
        return BaseResponse<GroupDto>.Success(GroupOwnership.ToDto(group));
    }
}

public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, BaseResponse<bool>>
{
    private readonly IGroupRepository _groups;
    private readonly IAdministratorRepository _admins;

    public DeleteGroupCommandHandler(IGroupRepository groups, IAdministratorRepository admins)
    {
        _groups = groups;
        _admins = admins;
    }

    public async Task<BaseResponse<bool>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var admin = await GroupOwnership.ResolveAdminAsync(_admins, request.RequestingUserId, cancellationToken);
        var group = await _groups.GetByIdAsync(request.Id, cancellationToken);
        if (admin == null || group == null || group.setaAdministratorId != admin.Id)
            return BaseResponse<bool>.Failure("Group not found");

        await _groups.DeleteAsync(group.Id, cancellationToken);
        await _groups.SaveChangesAsync(cancellationToken);
        return BaseResponse<bool>.Success(true);
    }
}

public class AssignStudentsToGroupCommandHandler : IRequestHandler<AssignStudentsToGroupCommand, BaseResponse<bool>>
{
    private readonly IGroupRepository _groups;
    private readonly IAdministratorRepository _admins;
    private readonly IStudentRepository _students;

    public AssignStudentsToGroupCommandHandler(IGroupRepository groups, IAdministratorRepository admins, IStudentRepository students)
    {
        _groups = groups;
        _admins = admins;
        _students = students;
    }

    public async Task<BaseResponse<bool>> Handle(AssignStudentsToGroupCommand request, CancellationToken cancellationToken)
    {
        var admin = await GroupOwnership.ResolveAdminAsync(_admins, request.RequestingUserId, cancellationToken);
        var group = await _groups.GetByIdAsync(request.GroupId, cancellationToken);
        if (admin == null || group == null || group.setaAdministratorId != admin.Id)
            return BaseResponse<bool>.Failure("Group not found");

        var existing = group.GroupEnrollments.Select(e => e.StudentId).ToHashSet();
        foreach (var studentId in request.StudentIds.Distinct())
        {
            if (existing.Contains(studentId)) continue;
            var student = await _students.GetByIdAsync(studentId, cancellationToken);
            if (student == null) return BaseResponse<bool>.Failure($"Student {studentId} not found");
            group.AddEnrollment(GroupEnrollment.Create(group, student));
            existing.Add(studentId);
        }

        await _groups.UpdateAsync(group, cancellationToken);
        await _groups.SaveChangesAsync(cancellationToken);
        return BaseResponse<bool>.Success(true);
    }
}

public class RemoveStudentFromGroupCommandHandler : IRequestHandler<RemoveStudentFromGroupCommand, BaseResponse<bool>>
{
    private readonly IGroupRepository _groups;
    private readonly IAdministratorRepository _admins;

    public RemoveStudentFromGroupCommandHandler(IGroupRepository groups, IAdministratorRepository admins)
    {
        _groups = groups;
        _admins = admins;
    }

    public async Task<BaseResponse<bool>> Handle(RemoveStudentFromGroupCommand request, CancellationToken cancellationToken)
    {
        var admin = await GroupOwnership.ResolveAdminAsync(_admins, request.RequestingUserId, cancellationToken);
        var group = await _groups.GetByIdAsync(request.GroupId, cancellationToken);
        if (admin == null || group == null || group.setaAdministratorId != admin.Id)
            return BaseResponse<bool>.Failure("Group not found");

        var enrollment = group.GroupEnrollments.FirstOrDefault(e => e.StudentId == request.StudentId);
        if (enrollment != null)
        {
            group.RemoveEnrollment(enrollment);
            await _groups.UpdateAsync(group, cancellationToken);
            await _groups.SaveChangesAsync(cancellationToken);
        }
        return BaseResponse<bool>.Success(true);
    }
}

public class GetGroupStudentsQueryHandler : IRequestHandler<GetGroupStudentsQuery, BaseResponse<List<GroupEnrollmentDto>>>
{
    private readonly IGroupRepository _groups;
    private readonly IAdministratorRepository _admins;

    public GetGroupStudentsQueryHandler(IGroupRepository groups, IAdministratorRepository admins)
    {
        _groups = groups;
        _admins = admins;
    }

    public async Task<BaseResponse<List<GroupEnrollmentDto>>> Handle(GetGroupStudentsQuery request, CancellationToken cancellationToken)
    {
        var admin = await GroupOwnership.ResolveAdminAsync(_admins, request.RequestingUserId, cancellationToken);
        var group = await _groups.GetByIdAsync(request.GroupId, cancellationToken);
        if (admin == null || group == null || group.setaAdministratorId != admin.Id)
            return BaseResponse<List<GroupEnrollmentDto>>.Failure("Group not found");

        var items = group.GroupEnrollments.Select(e => new GroupEnrollmentDto
        {
            Id = e.Id,
            GroupId = group.Id,
            GroupName = group.GroupName,
            StudentId = e.StudentId,
            StudentName = e.Student?.User == null ? string.Empty : $"{e.Student.User.Name} {e.Student.User.Surname}".Trim(),
            StudentNumber = e.Student?.StudentNumber ?? string.Empty,
            AssignedAt = e.AssignedAt
        }).ToList();

        return BaseResponse<List<GroupEnrollmentDto>>.Success(items);
    }
}
