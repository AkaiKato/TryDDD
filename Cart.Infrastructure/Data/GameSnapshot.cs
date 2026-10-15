namespace Cart.Infrastructure.Data
{
    public class GameSnapshot
    {
        public Guid GameId { get; set; }
        public string Title { get; set; } = null!;
        public long Price { get; set; }
        public bool ForSale { get; set; } = true;
    }
}
