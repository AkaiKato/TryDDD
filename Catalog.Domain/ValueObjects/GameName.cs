namespace Catalog.Domain.ValueObjects
{
    public record GameName
    {
        public string Title { get; }

        private GameName(string title)
        {
            Title = title;
        }

        public static GameName Create(string title)
        {
            if (title is null)
            {
                throw new ArgumentException("Game name cannot be null, empty or exceed 120 characters.", nameof(title));
            }

            title = title.Trim();
            if (title.Length == 0 || title.Length > 120)
            {
                throw new ArgumentException("Game name cannot be null, empty or exceed 120 characters.", nameof(title));
            }
            return new GameName(title);
        }
    }
}
