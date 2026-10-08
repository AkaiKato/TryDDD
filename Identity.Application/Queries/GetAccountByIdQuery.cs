using Identity.Domain.DTOs;
using Identity.Domain.Repositories;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Identity.Application.Queries
{
    public record GetAccountByIdQuery(Guid Id) : IRequest<AccountDto>;

    public class GetAccountByIdQueryHandler(
        IAccountRepository accountRepository) 
        : IRequestHandler<GetAccountByIdQuery, AccountDto>
    {
        public async Task<AccountDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            var account = await accountRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException($"Not found user with id {request.Id}");

            return new AccountDto
            {
                Id = account.Id,
                AccountStatus = account.AccountStatus,
                Email = account.Email
            };
        }
    }
}
