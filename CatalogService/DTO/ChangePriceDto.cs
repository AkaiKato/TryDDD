namespace CatalogService.DTO
{
    public class ChangePriceDto
    {
        public Guid GameId { get; set; }

        public decimal NewPrice { get; set; }
    }
}
