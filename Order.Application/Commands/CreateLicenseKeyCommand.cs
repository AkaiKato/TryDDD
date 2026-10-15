using MediatR;
using Order.Domain.Aggregates;
using Order.Domain.Repositories;
using Shared.Exceptions.Exceptions;

namespace Order.Application.Commands
{
    public record CreateLicenseKeyCommand(Guid GameId, string Code) : IRequest<Guid>;

    public class CreateLicenseKeyCommandHandler(ILicenseKeyRepository licenseKeys)
        : IRequestHandler<CreateLicenseKeyCommand, Guid>
    {
        public async Task<Guid> Handle(CreateLicenseKeyCommand request, CancellationToken cancellationToken)
        {
            if (request.GameId == Guid.Empty)
                throw new BadRequestException("GameId can't be empty");

            if (string.IsNullOrWhiteSpace(request.Code))
                throw new BadRequestException("Code can't be empty");

            var licenseKey = LicenseKey.Create(request.GameId, request.Code);
            await licenseKeys.AddAsync(licenseKey, cancellationToken);
            await licenseKeys.SaveChangesAsync(cancellationToken);
            return licenseKey.Id;
        }
    }
}
