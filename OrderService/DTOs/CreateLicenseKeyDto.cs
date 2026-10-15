namespace OrderService.DTOs
{
    public record CreateLicenseKeyDto(Guid GameId, string Code);
}
