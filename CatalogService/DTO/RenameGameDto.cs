namespace CatalogService.DTO
{
    public class RenameGameDto
    {
        public Guid GameId { get; set; }

        public string NewTitle { get; set; } = string.Empty;
    }
}
