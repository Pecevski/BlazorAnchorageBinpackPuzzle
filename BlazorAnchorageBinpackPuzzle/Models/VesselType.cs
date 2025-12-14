namespace BlazorAnchorageBinpackPuzzle.Models;

public record VesselType(
    VesselDimensions SingleShipDimensions,
    string ShipDesignation,
    int ShipCount)
{
    public int RemainingCount { get; set; } = ShipCount;
}
