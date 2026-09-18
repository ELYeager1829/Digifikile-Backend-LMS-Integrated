using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Application.DTOs.SystemLog;
using DigiFikileLms.Domain.Interfaces;
using MediatR;
using System.Text;

namespace DigiFikileLms.Application.Features.SystemLogs;

// ================================================================
// GET ALL SYSTEM LOGS HANDLER
// ================================================================

public class GetAllSystemLogsQueryHandler : IRequestHandler<GetAllSystemLogsQuery, BaseResponse<PagedResponse<SystemLogDto>>>
{
    private readonly ISystemLogRepository _logRepository;

    public GetAllSystemLogsQueryHandler(ISystemLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<BaseResponse<PagedResponse<SystemLogDto>>> Handle(GetAllSystemLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _logRepository.GetAllAsync(cancellationToken);
        var logList = logs.ToList();

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 500);
        var pageLogs = logList.OrderByDescending(l => l.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var items = pageLogs.Select(l => new SystemLogDto
        {
            Id = l.Id,
            SystemAdministratorId = l.SystemAdministratorId,
            SystemAdministratorName = $"{l.SystemAdministrator.User.Name} {l.SystemAdministrator.User.Surname}",
            Action = l.Action,
            ResourceType = l.ResourceType,
            ResourceId = l.ResourceId,
            Description = l.Description,
            IpAddress = l.IpAddress,
            UserAgent = l.UserAgent,
            CreatedAt = l.CreatedAt
        }).ToList();

        var response = new PagedResponse<SystemLogDto>
        {
            Items = items,
            TotalCount = logList.Count,
            Page = page,
            PageSize = pageSize
        };

        return BaseResponse<PagedResponse<SystemLogDto>>.Success(response);
    }
}

// ================================================================
// SEARCH SYSTEM LOGS HANDLER
// ================================================================

public class SearchSystemLogsQueryHandler : IRequestHandler<SearchSystemLogsQuery, BaseResponse<PagedResponse<SystemLogDto>>>
{
    private readonly ISystemLogRepository _logRepository;

    public SearchSystemLogsQueryHandler(ISystemLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<BaseResponse<PagedResponse<SystemLogDto>>> Handle(SearchSystemLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _logRepository.SearchAsync(
            request.Filter.Action,
            request.Filter.ResourceType,
            request.Filter.StartDate,
            request.Filter.EndDate,
            cancellationToken);

        var logList = logs.ToList();

        var page = Math.Max(1, request.Filter.Page);
        var pageSize = Math.Clamp(request.Filter.PageSize, 1, 500);
        var pageLogs = logList.OrderByDescending(l => l.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var items = pageLogs.Select(l => new SystemLogDto
        {
            Id = l.Id,
            SystemAdministratorId = l.SystemAdministratorId,
            SystemAdministratorName = $"{l.SystemAdministrator.User.Name} {l.SystemAdministrator.User.Surname}",
            Action = l.Action,
            ResourceType = l.ResourceType,
            ResourceId = l.ResourceId,
            Description = l.Description,
            IpAddress = l.IpAddress,
            UserAgent = l.UserAgent,
            CreatedAt = l.CreatedAt
        }).ToList();

        var response = new PagedResponse<SystemLogDto>
        {
            Items = items,
            TotalCount = logList.Count,
            Page = page,
            PageSize = pageSize
        };

        return BaseResponse<PagedResponse<SystemLogDto>>.Success(response);
    }
}

// ================================================================
// EXPORT SYSTEM LOGS HANDLER (CSV)
// ================================================================

public class ExportSystemLogsQueryHandler : IRequestHandler<ExportSystemLogsQuery, BaseResponse<FileDownloadDto>>
{
    private readonly ISystemLogRepository _logRepository;

    public ExportSystemLogsQueryHandler(ISystemLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<BaseResponse<FileDownloadDto>> Handle(ExportSystemLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _logRepository.SearchAsync(
            request.Filter.Action,
            request.Filter.ResourceType,
            request.Filter.StartDate,
            request.Filter.EndDate,
            cancellationToken);

        var logList = logs.ToList();
        var csv = new StringBuilder();

        // Header
        csv.AppendLine("Id,Administrator,Action,ResourceType,ResourceId,Description,IpAddress,CreatedAt");

        // Rows
        foreach (var log in logList)
        {
            csv.AppendLine($"{log.Id}," +
                $"{log.SystemAdministrator.User.Name} {log.SystemAdministrator.User.Surname}," +
                $"{log.Action}," +
                $"{log.ResourceType}," +
                $"{log.ResourceId}," +
                $"\"{log.Description}\"," +
                $"{log.IpAddress}," +
                $"{log.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        }

        var fileBytes = Encoding.UTF8.GetBytes(csv.ToString());

        return BaseResponse<FileDownloadDto>.Success(new FileDownloadDto
        {
            FileContent = fileBytes,
            FileName = $"SystemLogs_{DateTime.UtcNow:yyyyMMddHHmmss}.csv",
            ContentType = "text/csv"
        });
    }
}