namespace BlazorAnchorageBinpackPuzzle.Models;

public record VesselDimensions(int Width, int Height)
{
    public VesselDimensions Rotate() => new(Height, Width);
}
