namespace CartService.DTOs
{
    public class AddToCartDto
    {
        public Guid GameId { get; set; }
        public int Quantity { get; set; } 
    }
}
