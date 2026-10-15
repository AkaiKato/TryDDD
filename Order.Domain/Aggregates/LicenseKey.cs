using Order.Domain.Building_blocks;
using Order.Domain.DomainEvent;

namespace Order.Domain.Aggregates
{
    public class LicenseKey : Entity<Guid>
    {
        public Guid GameId { get; private set; }
        public string Code { get; private set; }
        public LicenseKeyStatus LicenseKeyStatus { get; private set; }
        public Guid? OrderId { get; private set; }

        private LicenseKey(Guid id, Guid gameId, string code, LicenseKeyStatus licenseKeyStatus, Guid? orderId) : base(id)
        {
            GameId = gameId;
            Code = code;
            LicenseKeyStatus = licenseKeyStatus;
            OrderId = orderId;
        }

        public static LicenseKey Create(Guid gameId, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code can't be empty");

            if (gameId == Guid.Empty)
                throw new ArgumentException("GameId can't be empty");

            var licenseKey = new LicenseKey(Guid.NewGuid(), gameId, code.Trim(), LicenseKeyStatus.Available, null);
            return licenseKey;
        }

        public static LicenseKey Restore(
            Guid id,
            Guid gameId,
            string code,
            LicenseKeyStatus licenseKeyStatus,
            Guid? orderId)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code can't be empty");

            if (gameId == Guid.Empty)
                throw new ArgumentException("GameId can't be empty");

            if (licenseKeyStatus == LicenseKeyStatus.Available && orderId != null)
                throw new InvalidOperationException("Available key must not habe an order");

            if ((licenseKeyStatus == LicenseKeyStatus.Reserved || licenseKeyStatus == LicenseKeyStatus.Assigned)
                && (orderId == null || orderId == Guid.Empty))
                throw new InvalidOperationException("Reserved or assigned key mus have an order");

            var licenseKey = new LicenseKey(id, gameId, code.Trim(), licenseKeyStatus, orderId);
            return licenseKey;
        }

        public void Reserve(Guid orderId)
        {
            if (LicenseKeyStatus != LicenseKeyStatus.Available)
                throw new InvalidOperationException("LicenseKey isn't in available status");

            if (orderId == Guid.Empty)
                throw new ArgumentException("OrderId can't be empty");

            OrderId = orderId;
            LicenseKeyStatus = LicenseKeyStatus.Reserved;
        }

        public void Release(Guid orderId)
        {
            if (LicenseKeyStatus != LicenseKeyStatus.Reserved)
                throw new InvalidOperationException("LicenseKey isn't in reserved status");

            if (OrderId != orderId)
                throw new InvalidOperationException("Another order can't release this reservation");

            OrderId = null;
            LicenseKeyStatus = LicenseKeyStatus.Available;
        }

        public void Assign(Guid orderId)
        {
            if (LicenseKeyStatus != LicenseKeyStatus.Reserved)
                throw new InvalidOperationException("LicenseKey isn't in reserved status");

            if (OrderId != orderId)
                throw new InvalidOperationException("Another order can't release this reservation");

            LicenseKeyStatus = LicenseKeyStatus.Assigned;
        }
    }

    public enum LicenseKeyStatus
    {
        Available,
        Reserved,
        Assigned
    }
}
