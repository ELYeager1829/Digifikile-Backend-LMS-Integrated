using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Enrollments;

public class EnrollStudentCommandHandler : IRequestHandler<EnrollStudentCommand, BaseResponse<StudentEnrollmentDto>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;

    public EnrollStudentCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<BaseResponse<StudentEnrollmentDto>> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student == null)
            return BaseResponse<StudentEnrollmentDto>.Failure("Student not found");

        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            return BaseResponse<StudentEnrollmentDto>.Failure("Course not found");

        // Check if already enrolled
        var existing = await _enrollmentRepository.GetByStudentAndCourseAsync(request.StudentId, request.CourseId, cancellationToken);
        if (existing != null)
            return BaseResponse<StudentEnrollmentDto>.Failure("Student already enrolled in this course");

        var enrollment = Enrollment.Create(student, course);
        await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
        await _enrollmentRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<StudentEnrollmentDto>.Success(new StudentEnrollmentDto
        {
            EnrollmentId = enrollment.Id,
            CourseId = course.Id,
            CourseName = course.CourseName,
            CourseCode = course.Code,
            EnrolledAt = enrollment.EnrolledAt,
            NqfLevel = course.NqfLevel
        });
    }
}