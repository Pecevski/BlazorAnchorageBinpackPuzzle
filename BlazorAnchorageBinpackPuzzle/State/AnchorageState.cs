namespace BlazorAnchorageBinpackPuzzle.State;

using BlazorAnchorageBinpackPuzzle.Models;

/// <summary>
/// Holds the current state of an anchorage puzzle session.
/// </summary>
public class AnchorageState
{
    private List<PlacedVessel> _placedVessels = new();
    private List<VesselType> _vesselTypes = new();

    public AnchorageSize? AnchorageSize { get; set; }
    public IReadOnlyList<PlacedVessel> PlacedVessels => _placedVessels.AsReadOnly();
    public IReadOnlyList<VesselType> VesselTypes => _vesselTypes.AsReadOnly();

    public void Initialize(FleetResponse fleetResponse)
    {
        AnchorageSize = fleetResponse.AnchorageSize;
        _vesselTypes = fleetResponse.Fleets.ToList();
        _placedVessels.Clear();
    }

    public void PlaceVessel(PlacedVessel vessel, VesselType vesselType)
    {
        _placedVessels.Add(vessel);
        vesselType.RemainingCount--;
    }

    public void RemoveVessel(PlacedVessel vessel, VesselType vesselType)
    {
        _placedVessels.Remove(vessel);
        vesselType.RemainingCount++;
    }

    public void Reset()
    {
        _placedVessels.Clear();
        _vesselTypes.Clear();
        AnchorageSize = null;
    }
}
