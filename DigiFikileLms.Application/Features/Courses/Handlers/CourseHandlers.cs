using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.Features.Courses.Commands;
using DigiFikileLms.Application.DTOs.Courses;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Courses;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, BaseResponse<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IFacilitatorRepository _facilitatorRepository;

    public CreateCourseCommandHandler(
        ICourseRepository courseRepository,
        IFacilitatorRepository facilitatorRepository)
    {
        _courseRepository = courseRepository;
        _facilitatorRepository = facilitatorRepository;
    }

    public async Task<BaseResponse<CourseDto>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var facilitator = await _facilitatorRepository.GetByIdAsync(request.FacilitatorId, cancellationToken);
        if (facilitator == null)
            return BaseResponse<CourseDto>.Failure("Facilitator not found");

        var course = Course.Create(
            request.CourseName,
            request.SaqaId,
            request.CurriculumCode,
            request.NqfLevel,
            facilitator);

        await _courseRepository.AddAsync(course, cancellationToken);
        await _courseRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<CourseDto>.Success(new CourseDto
        {
            Id = course.Id,
            CourseName = course.CourseName,
            Code = course.Code,
            SaqaId = course.SaqaId,
            CurriculumCode = course.CurriculumCode,
            NqfLevel = course.NqfLevel,
            FacilitatorId = course.FacilitatorId,
            CreatedAt = course.CreatedAt
        });
    }
}

/**
public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, BaseResponse<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;

    public GetCourseByIdQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<BaseResponse<CourseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (course == null)
            return BaseResponse<CourseDto>.Failure("Course not found");

        return BaseResponse<CourseDto>.Success(new CourseDto
        {
            Id = course.Id,
            CourseName = course.CourseName,
            Code = course.Code,
            SaqaId = course.SaqaId,
            CurriculumCode = course.CurriculumCode,
            NqfLevel = course.NqfLevel,
            FacilitatorId = course.FacilitatorId,
            CreatedAt = course.CreatedAt
        });
    }
}**/
public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, BaseResponse<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;

    public UpdateCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<BaseResponse<CourseDto>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (course == null)
            return BaseResponse<CourseDto>.Failure("Course not found");

        course.UpdateDetails(
            request.CourseName ?? course.CourseName,
            request.CurriculumCode,
            request.NqfLevel,
            request.SaqaId);

        await _courseRepository.UpdateAsync(course, cancellationToken);
        await _courseRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<CourseDto>.Success(new CourseDto
        {
            Id = course.Id,
            CourseName = course.CourseName,
            Code = course.Code,
            SaqaId = course.SaqaId,
            CurriculumCode = course.CurriculumCode,
            NqfLevel = course.NqfLevel,
            FacilitatorId = course.FacilitatorId,
            CreatedAt = course.CreatedAt
        });
    }
}

public class PublishCourseCommandHandler : IRequestHandler<PublishCourseCommand, BaseResponse<bool>>
{
    private readonly ICourseRepository _courseRepository;

    public PublishCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<BaseResponse<bool>> Handle(PublishCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (course == null)
            return BaseResponse<bool>.Failure("Course not found");

        try
        {
            course.Publish();
        }
        catch (InvalidOperationException ex)
        {
            return BaseResponse<bool>.Failure(ex.Message);
        }

        await _courseRepository.UpdateAsync(course, cancellationToken);
        await _courseRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<bool>.Success(true);
    }
}

public class ArchiveCourseCommandHandler : IRequestHandler<ArchiveCourseCommand, BaseResponse<bool>>
{
    private readonly ICourseRepository _courseRepository;

    public ArchiveCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<BaseResponse<bool>> Handle(ArchiveCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (course == null)
            return BaseResponse<bool>.Failure("Course not found");

        try
        {
            course.Archive();
        }
        catch (InvalidOperationException ex)
        {
            return BaseResponse<bool>.Failure(ex.Message);
        }

        await _courseRepository.UpdateAsync(course, cancellationToken);
        await _courseRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<bool>.Success(true);
    }
}

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, BaseResponse<bool>>
{
    private readonly ICourseRepository _courseRepository;

    public DeleteCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<BaseResponse<bool>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (course == null)
            return BaseResponse<bool>.Failure("Course not found");

        await _courseRepository.DeleteAsync(request.Id, cancellationToken);
        await _courseRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<bool>.Success(true);
    }
}