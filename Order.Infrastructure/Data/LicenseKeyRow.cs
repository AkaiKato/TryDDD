using Order.Domain.Aggregates;

namespace Order.Infrastructure.Data
{
    public class LicenseKeyRow
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public string Code { get; set; } = null!;
        public LicenseKeyStatus LicenseKeyStatus { get; set; }
        public Guid? OrderId { get; set; }
        public uint Version { get; set; }
    }
}
