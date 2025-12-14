namespace BlazorAnchorageBinpackPuzzle.Models;

public record PlacedVessel(
    string Id,
    string ShipDesignation,
    int X,
    int Y,
    VesselDimensions Dimensions,
    bool IsRotated)
{
    public override string ToString() => $"{ShipDesignation} at ({X}, {Y}) {Dimensions.Width}x{Dimensions.Height}";
}
