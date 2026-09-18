using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.SetaAdministrators;
using DigiFikileLms.Application.Features.SetaAdministrators.Commands;
using DigiFikileLms.Application.Features.SetaAdministrators.Queries;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Enums;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.SetaAdministrators;

/// <summary>
/// Creates a UserAccount with the SetaAdministrator role plus a SetaAdministrator profile row.
/// </summary>
public class CreateSetaAdministratorCommandHandler : IRequestHandler<CreateSetaAdministratorCommand, BaseResponse<SetaAdministratorDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly ISetaAdministratorRepository _setaAdministratorRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateSetaAdministratorCommandHandler(
        IUserAccountRepository userRepository,
        ISetaAdministratorRepository setaAdministratorRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _setaAdministratorRepository = setaAdministratorRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<BaseResponse<SetaAdministratorDto>> Handle(CreateSetaAdministratorCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            return BaseResponse<SetaAdministratorDto>.Failure("Email already registered");

        var user = UserAccount.Create(
            request.Name,
            request.Surname,
            request.Email,
            _passwordHasher.HashPassword(request.Password),
            UserRole.SetaAdministrator,
            request.Phone,
            request.Address);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var setaAdministrator = SetaAdministrator.Create(user);
        await _setaAdministratorRepository.AddAsync(setaAdministrator, cancellationToken);
        await _setaAdministratorRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<SetaAdministratorDto>.Success(new SetaAdministratorDto(
            setaAdministrator.Id,
            user.Id,
            user.Name,
            user.Surname,
            user.Email,
            setaAdministrator.IsActive,
            setaAdministrator.LastLogin,
            setaAdministrator.CreatedAt));
    }
}

/// <summary>
/// Updates a SETA Administrator profile (including activation toggle).
/// </summary>
public class UpdateSetaAdministratorCommandHandler : IRequestHandler<UpdateSetaAdministratorCommand, BaseResponse<SetaAdministratorDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly ISetaAdministratorRepository _setaAdministratorRepository;

    public UpdateSetaAdministratorCommandHandler(
        IUserAccountRepository userRepository,
        ISetaAdministratorRepository setaAdministratorRepository)
    {
        _userRepository = userRepository;
        _setaAdministratorRepository = setaAdministratorRepository;
    }

    public async Task<BaseResponse<SetaAdministratorDto>> Handle(UpdateSetaAdministratorCommand request, CancellationToken cancellationToken)
    {
        var setaAdministrator = await _setaAdministratorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (setaAdministrator == null)
            return BaseResponse<SetaAdministratorDto>.Failure("SETA Administrator not found");

        var user = await _userRepository.GetByIdAsync(setaAdministrator.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<SetaAdministratorDto>.Failure("User not found");

        user.UpdateProfile(
            request.Name ?? user.Name,
            request.Surname ?? user.Surname,
            request.Phone ?? user.Phone,
            request.Address ?? user.Address);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        if (request.IsActive is not null)
        {
            if (request.IsActive == true)
                setaAdministrator.Activate();
            else
                setaAdministrator.Deactivate();
            await _setaAdministratorRepository.UpdateAsync(setaAdministrator, cancellationToken);
            await _setaAdministratorRepository.SaveChangesAsync(cancellationToken);
        }

        return BaseResponse<SetaAdministratorDto>.Success(new SetaAdministratorDto(
            setaAdministrator.Id,
            user.Id,
            user.Name,
            user.Surname,
            user.Email,
            setaAdministrator.IsActive,
            setaAdministrator.LastLogin,
            setaAdministrator.CreatedAt));
    }
}
/// <summary>
/// Deletes a SETA Administrator profile.
/// </summary>
public class DeleteSetaAdministratorCommandHandler : IRequestHandler<DeleteSetaAdministratorCommand, BaseResponse<bool>>
{
    private readonly ISetaAdministratorRepository _setaAdministratorRepository;

    public DeleteSetaAdministratorCommandHandler(ISetaAdministratorRepository setaAdministratorRepository)
    {
        _setaAdministratorRepository = setaAdministratorRepository;
    }

    public async Task<BaseResponse<bool>> Handle(DeleteSetaAdministratorCommand request, CancellationToken cancellationToken)
    {
        var setaAdministrator = await _setaAdministratorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (setaAdministrator == null)
            return BaseResponse<bool>.Failure("SETA Administrator not found");

        await _setaAdministratorRepository.DeleteAsync(request.Id, cancellationToken);
        await _setaAdministratorRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<bool>.Success(true);
    }
}

/// <summary>
/// Returns a SETA Administrator by profile id.
/// </summary>
public class GetSetaAdministratorByIdQueryHandler : IRequestHandler<GetSetaAdministratorByIdQuery, BaseResponse<SetaAdministratorDto>>
{
    private readonly ISetaAdministratorRepository _setaAdministratorRepository;
    private readonly IUserAccountRepository _userRepository;

    public GetSetaAdministratorByIdQueryHandler(ISetaAdministratorRepository setaAdministratorRepository, IUserAccountRepository userRepository)
    {
        _setaAdministratorRepository = setaAdministratorRepository;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<SetaAdministratorDto>> Handle(GetSetaAdministratorByIdQuery request, CancellationToken cancellationToken)
    {
        var setaAdministrator = await _setaAdministratorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (setaAdministrator == null)
            return BaseResponse<SetaAdministratorDto>.Failure("SETA Administrator not found");

        var user = await _userRepository.GetByIdAsync(setaAdministrator.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<SetaAdministratorDto>.Failure("User not found");

        return BaseResponse<SetaAdministratorDto>.Success(new SetaAdministratorDto(
            setaAdministrator.Id,
            user.Id,
            user.Name,
            user.Surname,
            user.Email,
            setaAdministrator.IsActive,
            setaAdministrator.LastLogin,
            setaAdministrator.CreatedAt));
    }
}
/// <summary>
/// Returns a SETA Administrator by linked user id.
/// </summary>
public class GetSetaAdministratorByUserIdQueryHandler : IRequestHandler<GetSetaAdministratorByUserIdQuery, BaseResponse<SetaAdministratorDto>>
{
    private readonly ISetaAdministratorRepository _setaAdministratorRepository;
    private readonly IUserAccountRepository _userRepository;

    public GetSetaAdministratorByUserIdQueryHandler(ISetaAdministratorRepository setaAdministratorRepository, IUserAccountRepository userRepository)
    {
        _setaAdministratorRepository = setaAdministratorRepository;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<SetaAdministratorDto>> Handle(GetSetaAdministratorByUserIdQuery request, CancellationToken cancellationToken)
    {
        var setaAdministrator = await _setaAdministratorRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (setaAdministrator == null)
            return BaseResponse<SetaAdministratorDto>.Failure("SETA Administrator not found");

        var user = await _userRepository.GetByIdAsync(setaAdministrator.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<SetaAdministratorDto>.Failure("User not found");

        return BaseResponse<SetaAdministratorDto>.Success(new SetaAdministratorDto(
            setaAdministrator.Id,
            user.Id,
            user.Name,
            user.Surname,
            user.Email,
            setaAdministrator.IsActive,
            setaAdministrator.LastLogin,
            setaAdministrator.CreatedAt));
    }
}

/// <summary>
/// Returns all SETA Administrators paged.
/// </summary>
public class GetAllSetaAdministratorsQueryHandler : IRequestHandler<GetAllSetaAdministratorsQuery, BaseResponse<PagedResponse<SetaAdministratorDto>>>
{
    private readonly ISetaAdministratorRepository _setaAdministratorRepository;
    private readonly IUserAccountRepository _userRepository;

    public GetAllSetaAdministratorsQueryHandler(ISetaAdministratorRepository setaAdministratorRepository, IUserAccountRepository userRepository)
    {
        _setaAdministratorRepository = setaAdministratorRepository;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<PagedResponse<SetaAdministratorDto>>> Handle(GetAllSetaAdministratorsQuery request, CancellationToken cancellationToken)
    {
        var setaAdministrators = await _setaAdministratorRepository.GetAllAsync(cancellationToken);

        var dtos = new List<SetaAdministratorDto>();
        foreach (var setaAdministrator in setaAdministrators)
        {
            var user = await _userRepository.GetByIdAsync(setaAdministrator.UserId, cancellationToken);
            if (user == null) continue;
            dtos.Add(new SetaAdministratorDto(
                setaAdministrator.Id,
                user.Id,
                user.Name,
                user.Surname,
                user.Email,
                setaAdministrator.IsActive,
                setaAdministrator.LastLogin,
                setaAdministrator.CreatedAt));
        }

        var start = Math.Min((request.Page - 1) * request.PageSize, dtos.Count);
        var pageItems = dtos.Skip(start).Take(request.PageSize).ToList();

        return BaseResponse<PagedResponse<SetaAdministratorDto>>.Success(new PagedResponse<SetaAdministratorDto>
        {
            Items = pageItems,
            TotalCount = dtos.Count,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }
}