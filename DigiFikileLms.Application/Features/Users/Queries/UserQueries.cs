using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Users.Queries;

public record GetUserByIdQuery(int Id) : IRequest<BaseResponse<UserDto>>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, BaseResponse<UserDto>>
{
    private readonly IUserAccountRepository _users;
    public GetUserByIdQueryHandler(IUserAccountRepository users) => _users = users;

    public async Task<BaseResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _users.GetWithDetailsAsync(request.Id, cancellationToken);
        return user is null
            ? BaseResponse<UserDto>.Failure("User not found")
            : BaseResponse<UserDto>.Success(Map(user));
    }

    internal static UserDto Map(UserAccount user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name,
        Surname = user.Surname,
        Role = user.Role?.Name ?? user.UserRole.ToString(),
        RoleId = user.RoleId,
        IsActive = user.IsActive,
        Phone = user.Phone,
        Address = user.Address,
        CreatedAt = user.CreatedAt,
        StudentNumber = user.Student?.StudentNumber,
        Permissions = user.Role?.Permissions.Select(p => p.Code).ToList() ?? new List<string>()
    };
}

public record GetAllUsersQuery(int PageNumber = 1, int PageSize = 20) : IRequest<BaseResponse<PaginatedResponse<UserDto>>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, BaseResponse<PaginatedResponse<UserDto>>>
{
    private readonly IUserAccountRepository _users;
    public GetAllUsersQueryHandler(IUserAccountRepository users) => _users = users;

    public async Task<BaseResponse<PaginatedResponse<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var all = (await _users.GetAllAsync(cancellationToken)).OrderBy(u => u.Name).ThenBy(u => u.Surname).ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).Select(GetUserByIdQueryHandler.Map).ToList();
        return BaseResponse<PaginatedResponse<UserDto>>.Success(
            PaginatedResponse<UserDto>.Create(items, page, pageSize, all.Count));
    }
}
