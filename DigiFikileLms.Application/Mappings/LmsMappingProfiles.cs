using AutoMapper;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Application.DTOs.Certificates;
using DigiFikileLms.Application.DTOs.Courses;
using DigiFikileLms.Application.DTOs.Moderators;
using DigiFikileLms.Application.DTOs.Roles;
using DigiFikileLms.Application.DTOs.SetaAdministrators;
using DigiFikileLms.Application.DTOs.Students;
using DigiFikileLms.Application.DTOs.SystemAdmin;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Application.DTOs.Permission;

namespace DigiFikileLms.Application.Mappings;

public class LmsMappingProfiles : Profile
{
    public LmsMappingProfiles()
    {
        // ================================================================
        // USER MAPPINGS
        // ================================================================

        CreateMap<UserAccount, UserDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.UserRole.ToString()))
            .ForMember(dest => dest.StudentNumber, opt => opt.MapFrom(src =>
                src.Student != null ? src.Student.StudentNumber : null));

        // ================================================================
        // STUDENT MAPPINGS
        // ================================================================

        CreateMap<Student, StudentProfileDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StudentNumber, opt => opt.MapFrom(src => src.StudentNumber))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
            .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => src.EnrolledAt));

        CreateMap<Student, StudentListDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StudentNumber, opt => opt.MapFrom(src => src.StudentNumber))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => src.EnrolledAt))
            .ForMember(dest => dest.EnrollmentCount, opt => opt.MapFrom(src =>
                src.Enrollments != null ? src.Enrollments.Count : 0))
            .ForMember(dest => dest.AverageProgress, opt => opt.MapFrom(src =>
                src.ProgressRecords != null && src.ProgressRecords.Any()
                    ? src.ProgressRecords.Average(p => p.Percentage)
                    : 0));

        // ================================================================
        // COURSE MAPPINGS
        // ================================================================

        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseName))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.SaqaId, opt => opt.MapFrom(src => src.SaqaId))
            .ForMember(dest => dest.CurriculumCode, opt => opt.MapFrom(src => src.CurriculumCode))
            .ForMember(dest => dest.NqfLevel, opt => opt.MapFrom(src => src.NqfLevel))
            .ForMember(dest => dest.FacilitatorId, opt => opt.MapFrom(src => src.FacilitatorId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        // ================================================================
        // MODULE MAPPINGS
        // ================================================================

        CreateMap<Module, ModuleDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ModuleName))
            .ForMember(dest => dest.OrderIndex, opt => opt.MapFrom(src => 0));

        // ================================================================
        // ENROLLMENT MAPPINGS (FIXED)
        // ================================================================

        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Student.UserId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))  // ✅ Fixed
            .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => src.EnrolledAt));

        CreateMap<Enrollment, EnrollmentDetailDto>()
            .ForMember(dest => dest.EnrollmentId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src =>
                $"{src.Student.User.Name} {src.Student.User.Surname}"))
            .ForMember(dest => dest.StudentNumber, opt => opt.MapFrom(src =>
                src.Student.StudentNumber))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src =>
                src.Course.CourseName))
            .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => src.EnrolledAt));

        CreateMap<Enrollment, StudentEnrollmentDto>()
            .ForMember(dest => dest.EnrollmentId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.CourseName))
            .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.Course.Code))
            .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => src.EnrolledAt))
            .ForMember(dest => dest.NqfLevel, opt => opt.MapFrom(src => src.Course.NqfLevel));

        // ================================================================
        // ASSESSMENT MAPPINGS
        // ================================================================

        CreateMap<Assessment, DTOs.Assessment.AssessmentDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId))
            .ForMember(dest => dest.FacilitatorId, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.AssessmentType, opt => opt.MapFrom(src => src.AssessmentType.ToString()))
            .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => 100))
            .ForMember(dest => dest.PassingScore, opt => opt.MapFrom(src => 50))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<Assessment, AssessmentDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.Module.CourseId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.AssessmentType.ToString()))
            .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => 100))
            .ForMember(dest => dest.PassingScore, opt => opt.MapFrom(src => 50))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => (DateTime?)null));

        // ================================================================
        // SUBMISSION MAPPINGS
        // ================================================================

        CreateMap<Submission, SubmissionDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AssessmentId, opt => opt.MapFrom(src => src.AssessmentId))
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SubmissionDate))
            .ForMember(dest => dest.Attempts, opt => opt.MapFrom(src => src.Attempts));

        // ================================================================
        // RESULT MAPPINGS
        // ================================================================

        CreateMap<Result, ResultDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => src.Percentage))
            .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        // ================================================================
        // PROGRESS MAPPINGS
        // ================================================================

        CreateMap<Progress, StudentProgressDto>()
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.CourseName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => src.Percentage))
            .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.UpdatedAt ?? src.CreatedAt));

        // ================================================================
        // CERTIFICATE MAPPINGS
        // ================================================================

        CreateMap<Certificate, CertificateDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CertificateNumber, opt => opt.MapFrom(src => src.CertificateNumber))
            .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.CourseCode))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.IssuedAt, opt => opt.MapFrom(src => src.IssuedAt))
            .ForMember(dest => dest.VerificationToken, opt => opt.MapFrom(src => src.VerificationToken));

        CreateMap<Certificate, StudentCertificateDto>()
            .ForMember(dest => dest.CertificateId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CertificateNumber, opt => opt.MapFrom(src => src.CertificateNumber))
            .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.CourseCode))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.IssuedAt, opt => opt.MapFrom(src => src.IssuedAt));

        // ================================================================
        // NOTIFICATION MAPPINGS
        // ================================================================

        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
            .ForMember(dest => dest.DateSent, opt => opt.MapFrom(src => src.DateSent))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead));

        // ================================================================
        // ROLE & PERMISSION MAPPINGS
        // ================================================================

        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<Role, RoleDetailDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src =>
                src.Permissions != null
                    ? src.Permissions.Select(p => new PermissionDto(p.Id, p.Code, p.Name, p.Description)).ToList()
                    : new List<PermissionDto>()));

        CreateMap<Permission, PermissionDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        // ================================================================
        // MODERATOR & SETA ADMINISTRATOR MAPPINGS
        // ================================================================

        CreateMap<Moderator, ModeratorDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.StaffNumber, opt => opt.MapFrom(src => src.StaffNumber))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<SetaAdministrator, SetaAdministratorDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLogin))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}

