namespace Catalog.Domain.ValueObjects
{
    public sealed record Money
    {
        public long Amount { get; }
        public Currency Currency { get; }
        private Money(long amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }
        public static Money Create(decimal amount, Currency currency)
        {          
            if (currency == Currency.Unknown)
            {
                throw new ArgumentException("Currency cannot be unknown.", nameof(currency));
            }

            var cents = amount * 100;

            if (cents != decimal.Truncate(cents) || cents <= 0 || cents > long.MaxValue)
            {
                throw new ArgumentException("Amount must be a positive whole number of kopecks.", nameof(amount));
            }

            return new Money((long)cents, currency);
        }
    }

    public enum Currency
    {
        Unknown,
        RUB,
    }
}
