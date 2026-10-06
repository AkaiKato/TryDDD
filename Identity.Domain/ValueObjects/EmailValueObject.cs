namespace Identity.Domain.ValueObjects
{
    public sealed record EmailValueObject
    {
        public string Email { get; }

        private EmailValueObject(string rawEmail)
        {
            Email = rawEmail;
        }

        public static EmailValueObject Create(string rawEmail)
        {
            rawEmail = rawEmail.Trim().ToLowerInvariant();
            if (!IsValidEmail(rawEmail))
            {
                throw new ArgumentException("Invalid email format.", nameof(rawEmail));
            }
            return new EmailValueObject(rawEmail);
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
            {
                return false;
            }
            return true;
        }
    }
}


