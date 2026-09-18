using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Permission;
using DigiFikileLms.Application.Features.Permissions.Commands;
using DigiFikileLms.Application.Features.Permissions.Queries;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Permissions;

/// <summary>
/// Creates a permission via IPermissionRepository. Rejects duplicate codes.
/// </summary>
public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, BaseResponse<PermissionDto>>
{
    private readonly IPermissionRepository _permissionRepository;

    public CreatePermissionCommandHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<BaseResponse<PermissionDto>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        if (await _permissionRepository.ExistsByCodeAsync(request.Code, cancellationToken))
            return BaseResponse<PermissionDto>.Failure("Permission code already exists");

        var permission = Permission.Create(request.Code, request.Name, request.Description);
        await _permissionRepository.AddAsync(permission, cancellationToken);
        await _permissionRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<PermissionDto>.Success(new PermissionDto(
            permission.Id,
            permission.Code,
            permission.Name,
            permission.Description));
    }
}

/// <summary>
/// Updates the code/name/description of an existing permission.
/// </summary>
public class UpdatePermissionCommandHandler : IRequestHandler<UpdatePermissionCommand, BaseResponse<PermissionDto>>
{
    private readonly IPermissionRepository _permissionRepository;

    public UpdatePermissionCommandHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<BaseResponse<PermissionDto>> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (permission == null)
            return BaseResponse<PermissionDto>.Failure("Permission not found");

        permission.UpdateDetails(request.Code, request.Name, request.Description);
        await _permissionRepository.UpdateAsync(permission, cancellationToken);
        await _permissionRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<PermissionDto>.Success(new PermissionDto(
            permission.Id,
            permission.Code,
            permission.Name,
            permission.Description));
    }
}

/// <summary>
/// Deletes a permission by id.
/// </summary>
public class DeletePermissionCommandHandler : IRequestHandler<DeletePermissionCommand, BaseResponse<bool>>
{
    private readonly IPermissionRepository _permissionRepository;

    public DeletePermissionCommandHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<BaseResponse<bool>> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (permission == null)
            return BaseResponse<bool>.Failure("Permission not found");

        await _permissionRepository.DeleteAsync(request.Id, cancellationToken);
        await _permissionRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<bool>.Success(true);
    }
}

/// <summary>
/// Returns all permissions.
/// </summary>
public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, BaseResponse<IReadOnlyList<PermissionDto>>>
{
    private readonly IPermissionRepository _permissionRepository;

    public GetPermissionsQueryHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<BaseResponse<IReadOnlyList<PermissionDto>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);
        var dtos = permissions
            .Select(p => new PermissionDto(p.Id, p.Code, p.Name, p.Description))
            .ToList();

        return BaseResponse<IReadOnlyList<PermissionDto>>.Success(dtos);
    }
}

/// <summary>
/// Returns a single permission by id.
/// </summary>
public class GetPermissionByIdQueryHandler : IRequestHandler<GetPermissionByIdQuery, BaseResponse<PermissionDto>>
{
    private readonly IPermissionRepository _permissionRepository;

    public GetPermissionByIdQueryHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<BaseResponse<PermissionDto>> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (permission == null)
            return BaseResponse<PermissionDto>.Failure("Permission not found");

        return BaseResponse<PermissionDto>.Success(new PermissionDto(
            permission.Id,
            permission.Code,
            permission.Name,
            permission.Description));
    }
}