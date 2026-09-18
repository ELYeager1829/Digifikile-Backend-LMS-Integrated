using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Moderators;
using DigiFikileLms.Application.Features.Moderators.Commands;
using DigiFikileLms.Application.Features.Moderators.Queries;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Enums;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Moderators;

/// <summary>
/// Creates a UserAccount with the Moderator role plus a Moderator profile row.
/// </summary>
public class CreateModeratorCommandHandler : IRequestHandler<CreateModeratorCommand, BaseResponse<ModeratorDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IModeratorRepository _moderatorRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateModeratorCommandHandler(
        IUserAccountRepository userRepository,
        IModeratorRepository moderatorRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _moderatorRepository = moderatorRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<BaseResponse<ModeratorDto>> Handle(CreateModeratorCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            return BaseResponse<ModeratorDto>.Failure("Email already registered");

        var user = UserAccount.Create(
            request.Name,
            request.Surname,
            request.Email,
            _passwordHasher.HashPassword(request.Password),
            UserRole.Moderator,
            request.Phone,
            request.Address);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var moderator = Moderator.Create(user, request.StaffNumber);
        await _moderatorRepository.AddAsync(moderator, cancellationToken);
        await _moderatorRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<ModeratorDto>.Success(new ModeratorDto(
            moderator.Id,
            user.Id,
            moderator.StaffNumber,
            user.Name,
            user.Surname,
            user.Email,
            moderator.CreatedAt));
    }
}

/// <summary>
/// Updates moderator profile details.
/// </summary>
public class UpdateModeratorCommandHandler : IRequestHandler<UpdateModeratorCommand, BaseResponse<ModeratorDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IModeratorRepository _moderatorRepository;

    public UpdateModeratorCommandHandler(IUserAccountRepository userRepository, IModeratorRepository moderatorRepository)
    {
        _userRepository = userRepository;
        _moderatorRepository = moderatorRepository;
    }

    public async Task<BaseResponse<ModeratorDto>> Handle(UpdateModeratorCommand request, CancellationToken cancellationToken)
    {
        var moderator = await _moderatorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (moderator == null)
            return BaseResponse<ModeratorDto>.Failure("Moderator not found");

        var user = await _userRepository.GetByIdAsync(moderator.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<ModeratorDto>.Failure("User not found");

        user.UpdateProfile(
            request.Name ?? user.Name,
            request.Surname ?? user.Surname,
            request.Phone ?? user.Phone,
            request.Address ?? user.Address);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<ModeratorDto>.Success(new ModeratorDto(
            moderator.Id,
            user.Id,
            moderator.StaffNumber,
            user.Name,
            user.Surname,
            user.Email,
            moderator.CreatedAt));
    }
}
/// <summary>
/// Deletes a moderator and its user account.
/// </summary>
public class DeleteModeratorCommandHandler : IRequestHandler<DeleteModeratorCommand, BaseResponse<bool>>
{
    private readonly IModeratorRepository _moderatorRepository;

    public DeleteModeratorCommandHandler(IModeratorRepository moderatorRepository)
    {
        _moderatorRepository = moderatorRepository;
    }

    public async Task<BaseResponse<bool>> Handle(DeleteModeratorCommand request, CancellationToken cancellationToken)
    {
        var moderator = await _moderatorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (moderator == null)
            return BaseResponse<bool>.Failure("Moderator not found");

        await _moderatorRepository.DeleteAsync(request.Id, cancellationToken);
        await _moderatorRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<bool>.Success(true);
    }
}

/// <summary>
/// Returns a moderator by profile id.
/// </summary>
public class GetModeratorByIdQueryHandler : IRequestHandler<GetModeratorByIdQuery, BaseResponse<ModeratorDto>>
{
    private readonly IModeratorRepository _moderatorRepository;
    private readonly IUserAccountRepository _userRepository;

    public GetModeratorByIdQueryHandler(IModeratorRepository moderatorRepository, IUserAccountRepository userRepository)
    {
        _moderatorRepository = moderatorRepository;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<ModeratorDto>> Handle(GetModeratorByIdQuery request, CancellationToken cancellationToken)
    {
        var moderator = await _moderatorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (moderator == null)
            return BaseResponse<ModeratorDto>.Failure("Moderator not found");

        var user = await _userRepository.GetByIdAsync(moderator.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<ModeratorDto>.Failure("User not found");

        return BaseResponse<ModeratorDto>.Success(new ModeratorDto(
            moderator.Id,
            user.Id,
            moderator.StaffNumber,
            user.Name,
            user.Surname,
            user.Email,
            moderator.CreatedAt));
    }
}
/// <summary>
/// Returns a moderator by linked user id.
/// </summary>
public class GetModeratorByUserIdQueryHandler : IRequestHandler<GetModeratorByUserIdQuery, BaseResponse<ModeratorDto>>
{
    private readonly IModeratorRepository _moderatorRepository;
    private readonly IUserAccountRepository _userRepository;

    public GetModeratorByUserIdQueryHandler(IModeratorRepository moderatorRepository, IUserAccountRepository userRepository)
    {
        _moderatorRepository = moderatorRepository;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<ModeratorDto>> Handle(GetModeratorByUserIdQuery request, CancellationToken cancellationToken)
    {
        var moderator = await _moderatorRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (moderator == null)
            return BaseResponse<ModeratorDto>.Failure("Moderator not found");

        var user = await _userRepository.GetByIdAsync(moderator.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<ModeratorDto>.Failure("User not found");

        return BaseResponse<ModeratorDto>.Success(new ModeratorDto(
            moderator.Id,
            user.Id,
            moderator.StaffNumber,
            user.Name,
            user.Surname,
            user.Email,
            moderator.CreatedAt));
    }
}

/// <summary>
/// Returns all moderators paged.
/// </summary>
public class GetAllModeratorsQueryHandler : IRequestHandler<GetAllModeratorsQuery, BaseResponse<PagedResponse<ModeratorDto>>>
{
    private readonly IModeratorRepository _moderatorRepository;
    private readonly IUserAccountRepository _userRepository;

    public GetAllModeratorsQueryHandler(IModeratorRepository moderatorRepository, IUserAccountRepository userRepository)
    {
        _moderatorRepository = moderatorRepository;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<PagedResponse<ModeratorDto>>> Handle(GetAllModeratorsQuery request, CancellationToken cancellationToken)
    {
        var moderators = await _moderatorRepository.GetAllAsync(cancellationToken);

        var dtos = new List<ModeratorDto>();
        foreach (var moderator in moderators)
        {
            var user = await _userRepository.GetByIdAsync(moderator.UserId, cancellationToken);
            if (user == null) continue;
            dtos.Add(new ModeratorDto(
                moderator.Id,
                user.Id,
                moderator.StaffNumber,
                user.Name,
                user.Surname,
                user.Email,
                moderator.CreatedAt));
        }

        var start = Math.Min((request.Page - 1) * request.PageSize, dtos.Count);
        var pageItems = dtos.Skip(start).Take(request.PageSize).ToList();

        return BaseResponse<PagedResponse<ModeratorDto>>.Success(new PagedResponse<ModeratorDto>
        {
            Items = pageItems,
            TotalCount = dtos.Count,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }
}