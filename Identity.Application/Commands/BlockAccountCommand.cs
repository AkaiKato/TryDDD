using Identity.Domain.Repositories;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Identity.Application.Commands
{
    public record BlockAccountCommand(Guid Id) : IRequest;
    public class BlockAccountCommandHandler(
        IAccountRepository accountRepository
        ) : IRequestHandler<BlockAccountCommand>
    {
        public async Task Handle(BlockAccountCommand request, CancellationToken cancellationToken)
        {
            var existingAccount = await accountRepository.GetByIdAsync(request.Id, cancellationToken) 
                ?? throw new NotFoundException($"Not found user with id {request.Id}");
            
            existingAccount.Block();

            await accountRepository.UpdateAsync(existingAccount);
            await accountRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
