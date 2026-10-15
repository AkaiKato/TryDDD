using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Exceptions.Exceptions;

namespace Order.Infrastructure.Data
{
    internal static class OrderDbSave
    {
        public static async Task SaveAsync(OrderDbContext context, CancellationToken cancellationToken)
        {
            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                context.ChangeTracker.Clear();
                throw new ConflictException("The data was changed by another operation.");
            }
            catch (DbUpdateException exception) when (IsUniqueViolation(exception))
            {
                context.ChangeTracker.Clear();
                throw new ConflictException("The data conflicts with an existing row.");
            }
        }

        private static bool IsUniqueViolation(DbUpdateException exception)
            => exception.InnerException is PostgresException postgres
               && postgres.SqlState == PostgresErrorCodes.UniqueViolation;
    }
}
