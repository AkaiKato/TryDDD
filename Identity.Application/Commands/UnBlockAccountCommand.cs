using Identity.Domain.Repositories;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Identity.Application.Commands
{
    public record UnBlockAccountCommand(Guid Id) : IRequest;
    public class UnBlockAccountCommandHandler(
        IAccountRepository accountRepository
        ) : IRequestHandler<UnBlockAccountCommand>
    {
        public async Task Handle(UnBlockAccountCommand request, CancellationToken cancellationToken)
        {
            var existingAccount = await accountRepository.GetByIdAsync(request.Id, cancellationToken) 
                ?? throw new NotFoundException($"Not found user with id {request.Id}");
            
            existingAccount.UnBlock();

            await accountRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
