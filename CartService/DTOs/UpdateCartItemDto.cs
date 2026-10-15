namespace CartService.DTOs
{
    public class UpdateCartItemDto
    {
        public Guid GameId { get; set; }
        public int Quantity { get; set; }
    }
}
