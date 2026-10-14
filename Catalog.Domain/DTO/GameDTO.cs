using Catalog.Domain.Aggregates;

namespace Catalog.Domain.DTO
{
    public class GameDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public long Price { get; set; }
        public GameStatus GameStatus { get; set; }
    }
}
