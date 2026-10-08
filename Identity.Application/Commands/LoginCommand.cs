using Identity.Domain.Heplers;
using Identity.Domain.Repositories;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Identity.Application.Commands
{
    public record LoginCommand(string Email, string Password) : IRequest<string>;

    public class LoginCommandHandler(
        IAccountRepository accountRepository,
        IPasswordHasher passwordHasher)
        : IRequestHandler<LoginCommand, string>
    {
        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var account = await accountRepository.GetByEmailAsync(request.Email, cancellationToken)
                ?? throw new NotFoundException($"Not found user with email {request.Email}");

            if (!account.CanSignIn())
                throw new ForbiddenException($"Account blocked");

            var passwordEquals = passwordHasher.VerifyHashedPassword(request.Password, account.PasswordHash);
            if (!passwordEquals)
                throw new BadRequestException($"Wrong password");


        }
    }
}
