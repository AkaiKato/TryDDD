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
        private readonly IAccountRepository _accountRepository = accountRepository;
        private readonly IOutbox _outbox = outbox;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<Guid> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
        {
            var existingAccount = await _accountRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingAccount is not null)
            {
                throw new ConflictException($"An account with email {request.Email} already exists.");
            }

            var hashedPassword = _passwordHasher.HashPassword(request.Password);
            var newAccount = Account.Register(request.Email, hashedPassword);

            await _accountRepository.AddAsync(newAccount, cancellationToken);
            await _outbox.EnqueueAsync(newAccount.DomainEvents, cancellationToken);
            newAccount.ClearDomainEvents();
            await _accountRepository.SaveChangesAsync(cancellationToken);

            return newAccount.Id;
        }
    }
}
