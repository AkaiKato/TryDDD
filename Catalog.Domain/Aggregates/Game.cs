using Catalog.Domain.Building_blocks;
using Catalog.Domain.DomainEvent;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Aggregates
{
    public class Game : AggregateRoot<Guid>
    {
        public GameName Name { get; private set; }
        public Money Price { get; private set; }
        public GameStatus GameStatus { get; private set; }

        private Game(Guid id, GameName name, Money price, GameStatus gameStatus) : base(id)
        {
            Name = name;
            Price = price;
            GameStatus = gameStatus;
        }

        public static Game Create(string name, decimal price)
        {
            var game = new Game(Guid.NewGuid(), GameName.Create(name), Money.Create(price, Currency.RUB), GameStatus.Draft);
            game.RaiseDomainEvent(new GameCreated(game.Id, game.Name.Title, game.Price.Amount, DateTime.UtcNow));
            return game;
        }

        public static Game Restore(Guid id, string name, decimal price, GameStatus gameStatus)
            => new(id, GameName.Create(name), Money.Create(price/100, Currency.RUB), gameStatus);

        public void Publish()
        {
            if (GameStatus == GameStatus.Published || GameStatus == GameStatus.Archived)
            {
                return;
            }
            GameStatus = GameStatus.Published;
            RaiseDomainEvent(new GamePublished(Id, Name.Title, Price.Amount, DateTime.UtcNow));
        }

        public void Archive()
        {
            if (GameStatus == GameStatus.Archived)
            {
                return;
            }
            GameStatus = GameStatus.Archived;
            RaiseDomainEvent(new GameArchived(Id, DateTime.UtcNow));
        }

        public void UpdatePrice(decimal newPrice)
        {
            if(GameStatus == GameStatus.Archived)
            {
                throw new InvalidOperationException("Cannot update price of an archived game.");
            }
            Price = Money.Create(newPrice, Currency.RUB);
            RaiseDomainEvent(new PriceChanged(Id, Price.Amount, DateTime.UtcNow));
        }

        public void UpdateName(string newName)
        {
            if (GameStatus == GameStatus.Archived)
            {
                throw new InvalidOperationException("Cannot update name of an archived game.");
            }
            Name = GameName.Create(newName);
            RaiseDomainEvent(new GameRenamed(Id, Name.Title, DateTime.UtcNow));
        }
    }

    public enum GameStatus
    {
        Draft,
        Published,
        Archived,
    }
}
