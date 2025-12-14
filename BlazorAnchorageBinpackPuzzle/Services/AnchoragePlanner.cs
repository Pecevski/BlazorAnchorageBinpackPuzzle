namespace BlazorAnchorageBinpackPuzzle.Services;

using BlazorAnchorageBinpackPuzzle.Models;
using BlazorAnchorageBinpackPuzzle.Services.Interfaces;

/// <summary>
/// Pure domain logic for anchorage bin-packing validation and placement.
/// </summary>
public class AnchoragePlanner : IAnchoragePlanner
{
    /// <summary>
    /// Validates whether a vessel can be placed at the given position.
    /// </summary>
    public bool CanPlaceVessel(
        int x,
        int y,
        VesselDimensions dimensions,
        AnchorageSize anchorageSize,
        IEnumerable<PlacedVessel> placedVessels)
    {
        // Check boundary
        if (x < 0 || y < 0 || x + dimensions.Width > anchorageSize.Width ||
            y + dimensions.Height > anchorageSize.Height)
        {
            return false;
        }

        // Check collision with existing vessels
        return !placedVessels.Any(v => DoesCollide(x, y, dimensions, v));
    }

    /// <summary>
    /// Determines if two rectangles collide.
    /// </summary>
    private static bool DoesCollide(int x1, int y1, VesselDimensions dims1, PlacedVessel vessel2)
    {
        int x2 = vessel2.X;
        int y2 = vessel2.Y;
        int w2 = vessel2.Dimensions.Width;
        int h2 = vessel2.Dimensions.Height;

        return !(x1 + dims1.Width <= x2 || x2 + w2 <= x1 ||
                 y1 + dims1.Height <= y2 || y2 + h2 <= y1);
    }

    /// <summary>
    /// Checks if all vessels have been placed.
    /// </summary>
    public bool AllVesselsPlaced(IEnumerable<VesselType> vesselTypes, IEnumerable<PlacedVessel> placedVessels)
    {
        return vesselTypes.All(vt => vt.RemainingCount == 0);
    }

    /// <summary>
    /// Calculates total vessels that need to be placed.
    /// </summary>
    public int GetTotalVesselCount(IEnumerable<VesselType> vesselTypes)
    {
        return vesselTypes.Sum(vt => vt.ShipCount);
    }

    /// <summary>
    /// Gets the remaining vessel count.
    /// </summary>
    public int GetRemainingVesselCount(IEnumerable<VesselType> vesselTypes)
    {
        return vesselTypes.Sum(vt => vt.RemainingCount);
    }
}