using Catalog.Domain.Aggregates;

namespace Catalog.Infrastructure.Data
{
    public class GameRow
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public long Price { get; set; }
        public GameStatus GameStatus { get; set; }
    }
}
