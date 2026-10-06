using Identity.Domain.Building_blocks;
using Identity.Domain.DomainEvent;
using Identity.Domain.ValueObjects; 

namespace Identity.Domain.Aggregates.IdentityAggregate
{
    public class Account : AggregateRoot<Guid>
    {

        public string Email { get; private set; }

        public string PasswordHash { get; private set; }

        public AccountStatus AccountStatus { get; private set; }

        private Account(Guid id, string email, string passwordHash, AccountStatus status) : base(id)
        {
            Email = EmailValueObject.Create(email).Email;
            PasswordHash = PasswordHashValueObject.Create(passwordHash).Hash;
            AccountStatus = status;
        }

        public static Account Register(string email, string passwordHash)
        {
            var account = new Account(Guid.NewGuid(), email, passwordHash, AccountStatus.Active);

            account.RaiseDomainEvent(new AccountRegistered(account.Id, DateTime.UtcNow));

            return account;
        }

        public static Account Restore(Guid id, string email, string passwordHash, AccountStatus status)
            => new(id, email, passwordHash, status);

        public bool CanSignIn()
        {
            if (AccountStatus == AccountStatus.Blocked)
            {
                return false;
            }
            return true;
        }

        public void Block()
        {
            if(AccountStatus == AccountStatus.Blocked)
            {
                return;
            }
            AccountStatus = AccountStatus.Blocked;
            RaiseDomainEvent(new AccountBlocked(Id, DateTime.UtcNow));
        }

        public void UnBlock()
        {
            if (AccountStatus == AccountStatus.Active)
            {
                return;
            }
            AccountStatus = AccountStatus.Active;
            RaiseDomainEvent(new AccountUnblocked(Id, DateTime.UtcNow));
        }
    }

    public enum AccountStatus
    {
        Active,
        Blocked,
    }
}
