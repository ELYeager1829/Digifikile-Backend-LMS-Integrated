using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Courses;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Courses.Queries;

// ================================================================
// QUERIES
// ================================================================

public record GetCourseByIdQuery(
    int Id
) : IRequest<BaseResponse<CourseDto>>;

public record GetAllCoursesQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<BaseResponse<PagedResponse<CourseDto>>>;

public record GetCoursesByFacilitatorQuery(
    int FacilitatorId
) : IRequest<BaseResponse<List<CourseDto>>>;

public record GetPublishedCoursesQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<BaseResponse<PagedResponse<CourseDto>>>;

// ================================================================
// HANDLERS
// ================================================================

public class GetCourseByIdQueryHandler 
    : IRequestHandler<GetCourseByIdQuery, BaseResponse<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;

    public GetCourseByIdQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<BaseResponse<CourseDto>> Handle(
        GetCourseByIdQuery request, 
        CancellationToken cancellationToken)
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
}

public class GetAllCoursesQueryHandler 
    : IRequestHandler<GetAllCoursesQuery, BaseResponse<PagedResponse<CourseDto>>>
{
    private readonly ICourseRepository _courseRepository;

    public GetAllCoursesQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<BaseResponse<PagedResponse<CourseDto>>> Handle(
        GetAllCoursesQuery request, 
        CancellationToken cancellationToken)
    {
        var courses = await _courseRepository.GetAllAsync(cancellationToken);
        var dtos = courses.Select(c => new CourseDto
        {
            Id = c.Id,
            CourseName = c.CourseName,
            Code = c.Code,
            SaqaId = c.SaqaId,
            CurriculumCode = c.CurriculumCode,
            NqfLevel = c.NqfLevel,
            FacilitatorId = c.FacilitatorId,
            CreatedAt = c.CreatedAt
        }).ToList();

        var paged = new PagedResponse<CourseDto>
        {
            Items = dtos,
            TotalCount = dtos.Count,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return BaseResponse<PagedResponse<CourseDto>>.Success(paged);
    }
}

public class GetCoursesByFacilitatorQueryHandler 
    : IRequestHandler<GetCoursesByFacilitatorQuery, BaseResponse<List<CourseDto>>>
{
    private readonly ICourseRepository _courseRepository;

    public GetCoursesByFacilitatorQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<BaseResponse<List<CourseDto>>> Handle(
        GetCoursesByFacilitatorQuery request, 
        CancellationToken cancellationToken)
    {
        var courses = await _courseRepository.GetByFacilitatorAsync(
            request.FacilitatorId, cancellationToken);

        var dtos = courses.Select(c => new CourseDto
        {
            Id = c.Id,
            CourseName = c.CourseName,
            Code = c.Code,
            SaqaId = c.SaqaId,
            CurriculumCode = c.CurriculumCode,
            NqfLevel = c.NqfLevel,
            FacilitatorId = c.FacilitatorId,
            CreatedAt = c.CreatedAt
        }).ToList();

        return BaseResponse<List<CourseDto>>.Success(dtos);
    }
}

public class GetPublishedCoursesQueryHandler 
    : IRequestHandler<GetPublishedCoursesQuery, BaseResponse<PagedResponse<CourseDto>>>
{
    private readonly ICourseRepository _courseRepository;

    public GetPublishedCoursesQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<BaseResponse<PagedResponse<CourseDto>>> Handle(
        GetPublishedCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var allCourses = await _courseRepository.GetAllAsync(cancellationToken);
        var published = allCourses.Where(c => c.Status == "Published");

        var dtos = published.Select(c => new CourseDto
        {
            Id = c.Id,
            CourseName = c.CourseName,
            Code = c.Code,
            SaqaId = c.SaqaId,
            CurriculumCode = c.CurriculumCode,
            NqfLevel = c.NqfLevel,
            FacilitatorId = c.FacilitatorId,
            CreatedAt = c.CreatedAt
        }).ToList();

        var paged = new PagedResponse<CourseDto>
        {
            Items = dtos,
            TotalCount = dtos.Count,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return BaseResponse<PagedResponse<CourseDto>>.Success(paged);
    }
}

