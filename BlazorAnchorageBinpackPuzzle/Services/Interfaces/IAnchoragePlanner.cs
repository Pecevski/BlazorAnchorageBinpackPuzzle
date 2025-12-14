namespace BlazorAnchorageBinpackPuzzle.Services.Interfaces;

using BlazorAnchorageBinpackPuzzle.Models;

/// <summary>
/// Defines the contract for anchorage bin-packing validation and placement logic.
/// </summary>
public interface IAnchoragePlanner
{
    /// <summary>
    /// Validates whether a vessel can be placed at the given position.
    /// </summary>
    /// <param name="x">The x-coordinate (column) of the placement.</param>
    /// <param name="y">The y-coordinate (row) of the placement.</param>
    /// <param name="dimensions">The dimensions of the vessel.</param>
    /// <param name="anchorageSize">The total anchorage dimensions.</param>
    /// <param name="placedVessels">The collection of already-placed vessels.</param>
    /// <returns>True if the vessel can be placed; false if it would collide or exceed boundaries.</returns>
    bool CanPlaceVessel(
        int x,
        int y,
        VesselDimensions dimensions,
        AnchorageSize anchorageSize,
        IEnumerable<PlacedVessel> placedVessels);

    /// <summary>
    /// Checks if all vessels of all types have been placed.
    /// </summary>
    /// <param name="vesselTypes">The vessel type definitions with remaining counts.</param>
    /// <param name="placedVessels">The collection of placed vessels (for future extensibility).</param>
    /// <returns>True if all vessels have been placed; false otherwise.</returns>
    bool AllVesselsPlaced(IEnumerable<VesselType> vesselTypes, IEnumerable<PlacedVessel> placedVessels);

    /// <summary>
    /// Calculates the total number of vessels that need to be placed.
    /// </summary>
    /// <param name="vesselTypes">The vessel type definitions.</param>
    /// <returns>The sum of ShipCount across all vessel types.</returns>
    int GetTotalVesselCount(IEnumerable<VesselType> vesselTypes);

    /// <summary>
    /// Gets the remaining vessel count across all types.
    /// </summary>
    /// <param name="vesselTypes">The vessel type definitions with remaining counts.</param>
    /// <returns>The sum of RemainingCount across all vessel types.</returns>
    int GetRemainingVesselCount(IEnumerable<VesselType> vesselTypes);
}