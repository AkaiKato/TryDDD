using Identity.Domain.Aggregates.IdentityAggregate;
using Identity.Domain.Heplers;
using Identity.Domain.Repositories;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Identity.Application.Commands
{
    public record RegisterAccountCommand(string Email, string Password) : IRequest<Guid>;

    public class RegisterAccountCommandHandler(
        IAccountRepository accountRepository,
        IOutbox outbox,
        IPasswordHasher passwordHasher)
        : IRequestHandler<RegisterAccountCommand, Guid>
    {
        public async Task<Guid> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
        {
            var existingAccount = await accountRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingAccount is not null)
            {
                throw new ConflictException($"An account with email {request.Email} already exists.");
            }

            var hashedPassword = passwordHasher.HashPassword(request.Password);
            var newAccount = Account.Register(request.Email, hashedPassword);

            await accountRepository.AddAsync(newAccount, cancellationToken);
            await outbox.EnqueueAsync(newAccount.DomainEvents, cancellationToken);
            newAccount.ClearDomainEvents();
            await accountRepository.SaveChangesAsync(cancellationToken);

            return newAccount.Id;
        }
    }
}
