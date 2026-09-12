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

    public static Voyage Create(Guid vesselId, string destination, DateTimeOffset departureAt)
    {
        if (vesselId == Guid.Empty)
        {
            throw new DomainException("Vessel id is required.", "vessel_id_required");
        }

        if (string.IsNullOrWhiteSpace(destination))
        {
            throw new DomainException("Destination is required.", "voyage_destination_required");
        }

        var trimmedDestination = destination.Trim();
        if (trimmedDestination.Length > DestinationMaxLength)
        {
            throw new DomainException(
                $"Destination must be at most {DestinationMaxLength} characters.",
                "voyage_destination_too_long");
        }

        if (departureAt == default)
        {
            throw new DomainException("Departure time is required.", "voyage_departure_required");
        }

        return new Voyage
        {
            VesselId = vesselId,
            Destination = trimmedDestination,
            DepartureAt = departureAt
        };
    }
}
