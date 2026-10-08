namespace Identity.Domain.Heplers
{
    public interface IPasswordHasher
    {
        public string HashPassword(string password);
        public bool VerifyHashedPassword(string providedPassword, string hashedPassword);
    }
}
