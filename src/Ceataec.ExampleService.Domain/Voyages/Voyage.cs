using ErrorOr;

namespace Ceataec.ExampleService.Domain.Voyages;

public sealed class Voyage : AggregateRoot
{
    public const int DestinationMaxLength = 200;

    private Voyage()
    {
    }

    public Guid VesselId { get; private set; }
    public string Destination { get; private set; } = string.Empty;
    public DateTimeOffset DepartureAt { get; private set; }

    public static ErrorOr<Voyage> Create(Guid vesselId, string destination, DateTimeOffset departureAt)
    {
        if (vesselId == Guid.Empty)
        {
            return Error.Validation("vessel_id_required", "Vessel id is required.");
        }

        if (string.IsNullOrWhiteSpace(destination))
        {
            return Error.Validation("voyage_destination_required", "Destination is required.");
        }

        var trimmedDestination = destination.Trim();
        if (trimmedDestination.Length > DestinationMaxLength)
        {
            return Error.Validation(
                "voyage_destination_too_long",
                $"Destination must be at most {DestinationMaxLength} characters.");
        }

        if (departureAt == default)
        {
            return Error.Validation("voyage_departure_required", "Departure time is required.");
        }

        return new Voyage
        {
            VesselId = vesselId,
            Destination = trimmedDestination,
            DepartureAt = departureAt
        };
    }
}
