using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.SystemLog;
using DigiFikileLms.Application.DTOs;
using MediatR;

namespace DigiFikileLms.Application.Features.SystemLogs;

// "As a System Administrator, I want to view all system logs"
public record GetAllSystemLogsQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<BaseResponse<PagedResponse<SystemLogDto>>>;

// "As a System Administrator, I want to search system logs"
public record SearchSystemLogsQuery(
    SystemLogFilterDto Filter
) : IRequest<BaseResponse<PagedResponse<SystemLogDto>>>;

// "As a System Administrator, I want to export system logs"
public record ExportSystemLogsQuery(
    SystemLogFilterDto Filter
) : IRequest<BaseResponse<FileDownloadDto>>;