namespace Identity.Domain.ValueObjects
{
    public sealed record PasswordHashValueObject
    {
        public string Hash { get; }

        private PasswordHashValueObject(string hash)
        {
            Hash = hash;
        }

        public static PasswordHashValueObject Create(string hash)
        {
            if(string.IsNullOrEmpty(hash))
            {
                throw new ArgumentException("Password hash cannot be null or empty.", nameof(hash));
            }
            return new PasswordHashValueObject(hash);
        }
    }
}
