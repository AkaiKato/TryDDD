using Identity.Domain.Aggregates.IdentityAggregate;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions.Exceptions;

namespace Identity.Infrastructure.Repositories
{
    public class AccountRepository(IdentityDbContext context) : IAccountRepository
    {
        public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var row = await context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return row is null ? null : ToAccount(row);
        }

        public async Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalized = EmailValueObject.Create(email).Email;
            var row = await context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == normalized, cancellationToken);

            return row is null ? null : ToAccount(row);
        }

        public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
        {
            context.Accounts.Add(ToRow(account));
        }

        public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
        {
            var row = await context.Accounts
                .FirstOrDefaultAsync(x => x.Id == account.Id, cancellationToken);

            if (row is null)
            {
                throw new NotFoundException($"Account {account.Id} was not found.");
            }

            row.Email = account.Email.Email;
            row.PasswordHash = account.PasswordHash;
            row.Status = account.AccountStatus;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        private static AccountRow ToRow(Account account) => new()
        {
            Id = account.Id,
            Email = account.Email.Email,
            PasswordHash = account.PasswordHash,
            Status = account.AccountStatus
        };

        private static Account ToAccount(AccountRow row) =>
            Account.Restore(row.Id, row.Email, row.PasswordHash, row.Status);
    }
}
