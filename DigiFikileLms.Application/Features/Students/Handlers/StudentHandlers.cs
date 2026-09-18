using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Students;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Enums;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Students;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, BaseResponse<StudentProfileDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateStudentCommandHandler(
        IUserAccountRepository userRepository,
        IStudentRepository studentRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<BaseResponse<StudentProfileDto>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            return BaseResponse<StudentProfileDto>.Failure("Email already registered");

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = UserAccount.Create(
            request.Name,
            request.Surname,
            request.Email,
            passwordHash,
            UserRole.Student,
            request.Phone,
            request.Address);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var student = Student.Create(user, request.StudentNumber);
        await _studentRepository.AddAsync(student, cancellationToken);
        await _studentRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<StudentProfileDto>.Success(new StudentProfileDto(
            student.Id,
            student.StudentNumber,
            user.Name,
            user.Surname,
            user.Email,
            user.Phone,
            user.Address,
            student.EnrolledAt,
            user.IsActive));
    }
}

public class GetStudentProfileQueryHandler : IRequestHandler<GetStudentProfileQuery, BaseResponse<StudentProfileDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserAccountRepository _userAccountRepository;

    public GetStudentProfileQueryHandler(
        IStudentRepository studentRepository,
        IUserAccountRepository userAccountRepository)
    {
        _studentRepository = studentRepository;
        _userAccountRepository = userAccountRepository;
    }

    public async Task<BaseResponse<StudentProfileDto>> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (student == null)
            return BaseResponse<StudentProfileDto>.Failure("Student not found");

        var user = await _userAccountRepository.GetByIdAsync(student.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<StudentProfileDto>.Failure("User not found");

        return BaseResponse<StudentProfileDto>.Success(new StudentProfileDto(
            student.Id,
            student.StudentNumber,
            user.Name,
            user.Surname,
            user.Email,
            user.Phone,
            user.Address,
            student.EnrolledAt,
            user.IsActive));
    }
}

public class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, BaseResponse<PagedResponse<StudentProfileDto>>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserAccountRepository _userAccountRepository;

    public GetAllStudentsQueryHandler(
        IStudentRepository studentRepository,
        IUserAccountRepository userAccountRepository)
    {
        _studentRepository = studentRepository;
        _userAccountRepository = userAccountRepository;
    }

    public async Task<BaseResponse<PagedResponse<StudentProfileDto>>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
    {
        var students = (await _studentRepository.GetAllAsync(cancellationToken)).ToList();
        var totalCount = students.Count;

        var page = students
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        var items = new List<StudentProfileDto>();
        foreach (var student in page)
        {
            var user = await _userAccountRepository.GetByIdAsync(student.UserId, cancellationToken);
            if (user == null)
                continue;

            items.Add(new StudentProfileDto(
                student.Id,
                student.StudentNumber,
                user.Name,
                user.Surname,
                user.Email,
                user.Phone,
                user.Address,
                student.EnrolledAt,
                user.IsActive));
        }

        return BaseResponse<PagedResponse<StudentProfileDto>>.Success(new PagedResponse<StudentProfileDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }
}

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, BaseResponse<StudentProfileDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserAccountRepository _userAccountRepository;

    public GetStudentByIdQueryHandler(
        IStudentRepository studentRepository,
        IUserAccountRepository userAccountRepository)
    {
        _studentRepository = studentRepository;
        _userAccountRepository = userAccountRepository;
    }

    public async Task<BaseResponse<StudentProfileDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (student == null)
            return BaseResponse<StudentProfileDto>.Failure("Student not found");

        var user = await _userAccountRepository.GetByIdAsync(student.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<StudentProfileDto>.Failure("User not found");

        return BaseResponse<StudentProfileDto>.Success(new StudentProfileDto(
            student.Id,
            student.StudentNumber,
            user.Name,
            user.Surname,
            user.Email,
            user.Phone,
            user.Address,
            student.EnrolledAt,
            user.IsActive));
    }
}

public class GetStudentEnrollmentsQueryHandler : IRequestHandler<GetStudentEnrollmentsQuery, BaseResponse<List<StudentEnrollmentDto>>>
{
    private readonly IStudentRepository _studentRepository;

    public GetStudentEnrollmentsQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<BaseResponse<List<StudentEnrollmentDto>>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (student == null)
            return BaseResponse<List<StudentEnrollmentDto>>.Failure("Student not found");

        var enrollments = student.Enrollments
            .Select(e => new StudentEnrollmentDto
            {
                EnrollmentId = e.Id,
                CourseId = e.CourseId,
                CourseName = e.Course.CourseName,
                CourseCode = e.Course.Code,
                EnrolledAt = e.EnrolledAt,
                NqfLevel = e.Course.NqfLevel
            })
            .ToList();

        return BaseResponse<List<StudentEnrollmentDto>>.Success(enrollments);
    }
}

public class GetStudentProgressQueryHandler : IRequestHandler<GetStudentProgressQuery, BaseResponse<List<StudentProgressDto>>>
{
    private readonly IStudentRepository _studentRepository;

    public GetStudentProgressQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<BaseResponse<List<StudentProgressDto>>> Handle(GetStudentProgressQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (student == null)
            return BaseResponse<List<StudentProgressDto>>.Failure("Student not found");

        var progress = student.ProgressRecords
            .Select(p => new StudentProgressDto(
                p.CourseId,
                p.Course.CourseName,
                p.Status.ToString(),
                p.Percentage,
                p.UpdatedAt))
            .ToList();

        return BaseResponse<List<StudentProgressDto>>.Success(progress);
    }
}

public class GetStudentCertificatesQueryHandler : IRequestHandler<GetStudentCertificatesQuery, BaseResponse<List<StudentCertificateDto>>>
{
    private readonly IStudentRepository _studentRepository;

    public GetStudentCertificatesQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<BaseResponse<List<StudentCertificateDto>>> Handle(GetStudentCertificatesQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (student == null)
            return BaseResponse<List<StudentCertificateDto>>.Failure("Student not found");

        var certificates = student.Results
            .SelectMany(r => r.Certificates)
            .Select(c => new StudentCertificateDto
            {
                CertificateId = c.Id,
                CertificateNumber = c.CertificateNumber,
                CourseCode = c.CourseCode ?? string.Empty,
                Description = c.Description ?? string.Empty,
                IssuedAt = c.IssuedAt
            })
            .ToList();

        return BaseResponse<List<StudentCertificateDto>>.Success(certificates);
    }
}
public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, BaseResponse<StudentProfileDto>>
{
    private readonly IStudentRepository _students;
    private readonly IUserAccountRepository _users;

    public UpdateStudentCommandHandler(IStudentRepository students, IUserAccountRepository users)
    {
        _students = students;
        _users = users;
    }

    public async Task<BaseResponse<StudentProfileDto>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _students.GetByIdAsync(request.Id, cancellationToken);
        if (student == null) return BaseResponse<StudentProfileDto>.Failure("Student not found");

        var user = await _users.GetByIdAsync(student.UserId, cancellationToken);
        if (user == null) return BaseResponse<StudentProfileDto>.Failure("User not found");

        var name = string.IsNullOrWhiteSpace(request.Name) ? user.Name : request.Name.Trim();
        var surname = string.IsNullOrWhiteSpace(request.Surname) ? user.Surname : request.Surname.Trim();
        var phone = request.Phone ?? user.Phone;
        var address = request.Address ?? user.Address;
        user.UpdateProfile(name, surname, phone, address);

        if (!string.IsNullOrWhiteSpace(request.Email) && !string.Equals(request.Email.Trim(), user.Email, StringComparison.OrdinalIgnoreCase))
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var existing = await _users.GetByEmailAsync(normalizedEmail, cancellationToken);
            if (existing != null && existing.Id != user.Id)
                return BaseResponse<StudentProfileDto>.Failure("Email already registered");
            user.UpdateEmail(normalizedEmail);
        }

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value) user.Activate();
            else user.Deactivate();
        }

        await _users.UpdateAsync(user, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);

        return BaseResponse<StudentProfileDto>.Success(new StudentProfileDto(
            student.Id,
            student.StudentNumber,
            user.Name,
            user.Surname,
            user.Email,
            user.Phone,
            user.Address,
            student.EnrolledAt,
            user.IsActive));
    }
}

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, BaseResponse<bool>>
{
    private readonly IStudentRepository _students;
    private readonly IUserAccountRepository _users;

    public DeleteStudentCommandHandler(IStudentRepository students, IUserAccountRepository users)
    {
        _students = students;
        _users = users;
    }

    public async Task<BaseResponse<bool>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        // Preserve learner history and related enrollment/audit records. "Delete" is therefore a
        // safe deactivation, matching the rest of DigiFikile's administrator account lifecycle.
        var student = await _students.GetByIdAsync(request.Id, cancellationToken);
        if (student == null) return BaseResponse<bool>.Failure("Student not found");
        var user = await _users.GetByIdAsync(student.UserId, cancellationToken);
        if (user == null) return BaseResponse<bool>.Failure("User not found");

        user.Deactivate();
        await _users.UpdateAsync(user, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);
        return BaseResponse<bool>.Success(true);
    }
}
