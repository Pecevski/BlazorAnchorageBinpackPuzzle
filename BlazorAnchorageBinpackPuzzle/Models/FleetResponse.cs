namespace BlazorAnchorageBinpackPuzzle.Models;

using System.Text.Json.Serialization;

public record FleetResponse(
    [property: JsonPropertyName("anchorageSize")] AnchorageSize AnchorageSize,
    [property: JsonPropertyName("fleets")] VesselType[] Fleets);
