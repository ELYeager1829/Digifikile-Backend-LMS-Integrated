using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Auth;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, BaseResponse<UserDto>>
{
    private readonly IUserAccountRepository _userRepository;

    public GetCurrentUserQueryHandler(IUserAccountRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetWithDetailsAsync(request.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<UserDto>.Failure("User not found");

        // Get student number if user is a student
        string? studentNumber = null;
        if (user.Student != null)
        {
            studentNumber = user.Student.StudentNumber;
        }

        return BaseResponse<UserDto>.Success(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
            Role = user.UserRole.ToString(),
            RoleId = user.RoleId,
            IsActive = user.IsActive,
            Phone = user.Phone,
            Address = user.Address,
            CreatedAt = user.CreatedAt,
            StudentNumber = studentNumber,
            Permissions = user.Role?.Permissions?.Select(p => p.Code).ToList() ?? new List<string>()
        });
    }
}

public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, BaseResponse<List<string>>>
{
    private readonly IUserAccountRepository _userRepository;

    public GetUserPermissionsQueryHandler(IUserAccountRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<List<string>>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<List<string>>.Failure("User not found");

        var permissions = new List<string>();

        // Assign permissions based on role
        switch (user.UserRole.ToString())
        {
            case "SetaAdministrator":
                permissions.AddRange(new[]
                {
                    "manage_users",
                    "manage_courses",
                    "manage_assessments",
                    "manage_enrollments",
                    "view_reports",
                    "manage_certificates",
                    "manage_departments",
                    "manage_seta_programmes"
                });
                break;

            case "TrainingProvider":
                permissions.AddRange(new[]
                {
                    "view_reports",
                    "manage_departments",
                    "view_certificates",
                    "view_training_progress"
                });
                break;

            case "Facilitator":
                permissions.AddRange(new[]
                {
                    "create_courses",
                    "update_courses",
                    "manage_modules",
                    "create_assessments",
                    "view_course_enrollments",
                    "view_student_progress"
                });
                break;

            case "Moderator":
                permissions.AddRange(new[]
                {
                    "grade_assessments",
                    "view_results",
                    "provide_feedback",
                    "view_pending_gradings"
                });
                break;

            case "Student":
                permissions.AddRange(new[]
                {
                    "view_courses",
                    "enroll_courses",
                    "view_my_progress",
                    "submit_assessments",
                    "view_my_certificates",
                    "view_my_results"
                });
                break;

            default:
                permissions.Add("view_public_content");
                break;
        }

        return BaseResponse<List<string>>.Success(permissions);
    }
}