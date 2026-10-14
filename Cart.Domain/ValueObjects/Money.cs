namespace Cart.Domain.ValueObjects
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
        public static Money Create(long amount, Currency currency)
        {          
            if (currency == Currency.Unknown)
            {
                throw new ArgumentException("Currency cannot be unknown.", nameof(currency));
            }


            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be a positive whole number.", nameof(amount));
            }

            return new Money(amount, currency);
        }
    }

    public enum Currency
    {
        Unknown,
        RUB,
    }
}
