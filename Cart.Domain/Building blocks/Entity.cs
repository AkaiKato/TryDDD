namespace Cart.Domain.Building_blocks
{
    public abstract class Entity<TId>(TId id) where TId : notnull
    {
        public TId Id { get; protected set; } = id;
    }
}
