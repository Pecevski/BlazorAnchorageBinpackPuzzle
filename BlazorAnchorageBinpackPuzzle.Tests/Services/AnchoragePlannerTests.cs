namespace BlazorAnchorageBinpackPuzzle.Tests.Services;

using BlazorAnchorageBinpackPuzzle.Models;
using BlazorAnchorageBinpackPuzzle.Services;
using Xunit;

/// <summary>
/// Unit tests for the AnchoragePlanner service.
/// Tests collision detection, boundary validation, and placement logic.
/// </summary>
public class AnchoragePlannerTests
{
    private readonly AnchoragePlanner _planner = new();

    #region CanPlaceVessel Tests

    [Fact]
    public void CanPlaceVessel_WithValidPlacement_ReturnsTrue()
    {
        // Arrange
        var anchorageSize = new AnchorageSize(10, 10);
        var dimensions = new VesselDimensions(3, 4);
        var placedVessels = new List<PlacedVessel>();

        // Act
        var result = _planner.CanPlaceVessel(0, 0, dimensions, anchorageSize, placedVessels);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanPlaceVessel_WithNegativeX_ReturnsFalse()
    {
        // Arrange
        var anchorageSize = new AnchorageSize(10, 10);
        var dimensions = new VesselDimensions(3, 4);
        var placedVessels = new List<PlacedVessel>();

        // Act
        var result = _planner.CanPlaceVessel(-1, 0, dimensions, anchorageSize, placedVessels);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanPlaceVessel_WithNegativeY_ReturnsFalse()
    {
        // Arrange
        var anchorageSize = new AnchorageSize(10, 10);
        var dimensions = new VesselDimensions(3, 4);
        var placedVessels = new List<PlacedVessel>();

        // Act
        var result = _planner.CanPlaceVessel(0, -1, dimensions, anchorageSize, placedVessels);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanPlaceVessel_ExceedsWidthBoundary_ReturnsFalse()
    {
        // Arrange
        var anchorageSize = new AnchorageSize(10, 10);
        var dimensions = new VesselDimensions(5, 4);
        var placedVessels = new List<PlacedVessel>();

        // Act - Vessel would extend beyond width (7 + 5 = 12 > 10)
        var result = _planner.CanPlaceVessel(7, 0, dimensions, anchorageSize, placedVessels);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanPlaceVessel_ExceedsHeightBoundary_ReturnsFalse()
    {
        // Arrange
        var anchorageSize = new AnchorageSize(10, 10);
        var dimensions = new VesselDimensions(3, 6);
        var placedVessels = new List<PlacedVessel>();

        // Act - Vessel would extend beyond height (6 + 6 = 12 > 10)
        var result = _planner.CanPlaceVessel(0, 6, dimensions, anchorageSize, placedVessels);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanPlaceVessel_FitsExactlyAtBoundary_ReturnsTrue()
    {
        // Arrange
        var anchorageSize = new AnchorageSize(10, 10);
        var dimensions = new VesselDimensions(3, 4);
        var placedVessels = new List<PlacedVessel>();

        // Act - Vessel fits exactly (7 + 3 = 10, 6 + 4 = 10)
        var result = _planner.CanPlaceVessel(7, 6, dimensions, anchorageSize, placedVessels);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Collision Detection Tests

    [Fact]
    public void CanPlaceVessel_CollidesWith_ExistingVessel_ReturnsFalse()
    {
        // Arrange
        var anchorageSize = new AnchorageSize(10, 10);
        var dimensions = new VesselDimensions(3, 4);
        var existingVessel = new PlacedVessel(
            Id: "vessel-1",
            ShipDesignation: "Test Ship 1",
            X: 2,
            Y: 2,
            Dimensions: new VesselDimensions(3, 4),
            IsRotated: false);
        var placedVessels = new List<PlacedVessel> { existingVessel };

        // Act - Try to place at same position (collision)
        var result = _planner.CanPlaceVessel(2, 2, dimensions, anchorageSize, placedVessels);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanPlaceVessel_PartialOverlap_ReturnsFalse()
    {
        // Arrange
        var anchorageSize = new AnchorageSize(10, 10);
        var dimensions = new VesselDimensions(3, 3);
        var existingVessel = new PlacedVessel(
            Id: "vessel-1",
            ShipDesignation: "Test Ship 1",
            X: 2,
            Y: 2,
            Dimensions: new VesselDimensions(4, 4),
            IsRotated: false);
        var placedVessels = new List<PlacedVessel> { existingVessel };

        // Act - Try to place with partial overlap (3,3 overlaps with 2-6,2-6)
        var result = _planner.CanPlaceVessel(3, 3, dimensions, anchorageSize, placedVessels);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanPlaceVessel_TouchingEdge_ReturnsTrue()
    {
        // Arrange
        var anchorageSize = new AnchorageSize(10, 10);
        var dimensions = new VesselDimensions(3, 3);
        var existingVessel = new PlacedVessel(
            Id: "vessel-1",
            ShipDesignation: "Test Ship 1",
            X: 0,
            Y: 0,
            Dimensions: new VesselDimensions(3, 3),
            IsRotated: false);
        var placedVessels = new List<PlacedVessel> { existingVessel };

        // Act - Place adjacent to existing vessel (touching edge at x=3)
        var result = _planner.CanPlaceVessel(3, 0, dimensions, anchorageSize, placedVessels);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanPlaceVessel_MultipleVessels_NoCollision_ReturnsTrue()
    {
        // Arrange
        var anchorageSize = new AnchorageSize(15, 10);
        var dimensions = new VesselDimensions(3, 3);
        var vessels = new List<PlacedVessel>
        {
            new("v1", "Ship 1", 0, 0, new VesselDimensions(3, 3), false),
            new("v2", "Ship 2", 5, 0, new VesselDimensions(3, 3), false),
            new("v3", "Ship 3", 10, 0, new VesselDimensions(3, 3), false)
        };

        // Act - Place in empty space between vessels
        var result = _planner.CanPlaceVessel(3, 5, dimensions, anchorageSize, vessels);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanPlaceVessel_MultipleVessels_WithCollision_ReturnsFalse()
    {
        // Arrange
        var anchorageSize = new AnchorageSize(15, 10);
        var dimensions = new VesselDimensions(2, 2);
        var vessels = new List<PlacedVessel>
        {
            new("v1", "Ship 1", 0, 0, new VesselDimensions(3, 3), false),
            new("v2", "Ship 2", 5, 0, new VesselDimensions(3, 3), false)
        };

        // Act - Place where it collides with Ship 1
        var result = _planner.CanPlaceVessel(2, 2, dimensions, anchorageSize, vessels);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanPlaceVessel_LargeVessel_WithSmallPlacedVessel_NoCollision_ReturnsTrue()
    {
        // Arrange
        var anchorageSize = new AnchorageSize(20, 20);
        var largeDimensions = new VesselDimensions(10, 10);
        var smallVessel = new PlacedVessel(
            "v1", "Small Ship", 15, 15, 
            new VesselDimensions(2, 2), false);
        var placedVessels = new List<PlacedVessel> { smallVessel };

        // Act - Place large vessel away from small one
        var result = _planner.CanPlaceVessel(0, 0, largeDimensions, anchorageSize, placedVessels);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region AllVesselsPlaced Tests

    [Fact]
    public void AllVesselsPlaced_WithZeroRemainingCount_ReturnsTrue()
    {
        // Arrange
        var vesselType1 = new VesselType(new VesselDimensions(3, 4), "Ship 1", 2) { RemainingCount = 0 };
        var vesselType2 = new VesselType(new VesselDimensions(4, 5), "Ship 2", 3) { RemainingCount = 0 };
        var vesselTypes = new[] { vesselType1, vesselType2 };
        var placedVessels = new List<PlacedVessel>();

        // Act
        var result = _planner.AllVesselsPlaced(vesselTypes, placedVessels);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AllVesselsPlaced_WithRemainingCount_ReturnsFalse()
    {
        // Arrange
        var vesselType1 = new VesselType(new VesselDimensions(3, 4), "Ship 1", 2) { RemainingCount = 0 };
        var vesselType2 = new VesselType(new VesselDimensions(4, 5), "Ship 2", 3) { RemainingCount = 1 };
        var vesselTypes = new[] { vesselType1, vesselType2 };
        var placedVessels = new List<PlacedVessel>();

        // Act
        var result = _planner.AllVesselsPlaced(vesselTypes, placedVessels);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AllVesselsPlaced_WithEmptyVesselTypes_ReturnsTrue()
    {
        // Arrange
        var vesselTypes = Array.Empty<VesselType>();
        var placedVessels = new List<PlacedVessel>();

        // Act
        var result = _planner.AllVesselsPlaced(vesselTypes, placedVessels);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region GetTotalVesselCount Tests

    [Fact]
    public void GetTotalVesselCount_ReturnsSumOfAllShipCounts()
    {
        // Arrange
        var vesselTypes = new[]
        {
            new VesselType(new VesselDimensions(3, 4), "Ship 1", 2),
            new VesselType(new VesselDimensions(4, 5), "Ship 2", 3),
            new VesselType(new VesselDimensions(5, 6), "Ship 3", 5)
        };

        // Act
        var result = _planner.GetTotalVesselCount(vesselTypes);

        // Assert
        Assert.Equal(10, result);
    }

    [Fact]
    public void GetTotalVesselCount_WithEmptyVesselTypes_ReturnsZero()
    {
        // Arrange
        var vesselTypes = Array.Empty<VesselType>();

        // Act
        var result = _planner.GetTotalVesselCount(vesselTypes);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void GetTotalVesselCount_WithSingleVessel_ReturnsCorrectCount()
    {
        // Arrange
        var vesselTypes = new[] { new VesselType(new VesselDimensions(3, 4), "Ship 1", 7) };

        // Act
        var result = _planner.GetTotalVesselCount(vesselTypes);

        // Assert
        Assert.Equal(7, result);
    }

    #endregion

    #region GetRemainingVesselCount Tests

    [Fact]
    public void GetRemainingVesselCount_ReturnsSumOfRemainingCounts()
    {
        // Arrange
        var vesselTypes = new[]
        {
            new VesselType(new VesselDimensions(3, 4), "Ship 1", 2) { RemainingCount = 1 },
            new VesselType(new VesselDimensions(4, 5), "Ship 2", 3) { RemainingCount = 2 },
            new VesselType(new VesselDimensions(5, 6), "Ship 3", 5) { RemainingCount = 3 }
        };

        // Act
        var result = _planner.GetRemainingVesselCount(vesselTypes);

        // Assert
        Assert.Equal(6, result);
    }

    [Fact]
    public void GetRemainingVesselCount_WithZeroRemaining_ReturnsZero()
    {
        // Arrange
        var vesselTypes = new[]
        {
            new VesselType(new VesselDimensions(3, 4), "Ship 1", 2) { RemainingCount = 0 },
            new VesselType(new VesselDimensions(4, 5), "Ship 2", 3) { RemainingCount = 0 }
        };

        // Act
        var result = _planner.GetRemainingVesselCount(vesselTypes);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void GetRemainingVesselCount_WithEmptyVesselTypes_ReturnsZero()
    {
        // Arrange
        var vesselTypes = Array.Empty<VesselType>();

        // Act
        var result = _planner.GetRemainingVesselCount(vesselTypes);

        // Assert
        Assert.Equal(0, result);
    }

    #endregion

    #region Edge Cases and Integration Tests

    [Fact]
    public void CanPlaceVessel_ComplexScenario_MultipleVesselsVariousSizes()
    {
        // Arrange - Create a complex grid with multiple vessels of different sizes
        var anchorageSize = new AnchorageSize(20, 20);
        var placedVessels = new List<PlacedVessel>
        {
            new("v1", "Large Ship", 0, 0, new VesselDimensions(8, 8), false),
            new("v2", "Medium Ship", 10, 0, new VesselDimensions(5, 6), false),
            new("v3", "Small Ship", 0, 10, new VesselDimensions(3, 3), false),
            new("v4", "Tiny Ship", 15, 15, new VesselDimensions(2, 2), false)
        };

        // Act & Assert - Try various placements
        Assert.True(_planner.CanPlaceVessel(8, 8, new VesselDimensions(4, 4), anchorageSize, placedVessels));
        Assert.False(_planner.CanPlaceVessel(7, 7, new VesselDimensions(4, 4), anchorageSize, placedVessels));
        Assert.True(_planner.CanPlaceVessel(10, 10, new VesselDimensions(3, 3), anchorageSize, placedVessels));
    }

    [Fact]
    public void CanPlaceVessel_SinglePixelPlacement_ValidatesCorrectly()
    {
        // Arrange
        var anchorageSize = new AnchorageSize(5, 5);
        var dimensions = new VesselDimensions(1, 1);
        var placedVessels = new List<PlacedVessel>();

        // Act & Assert
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                Assert.True(_planner.CanPlaceVessel(x, y, dimensions, anchorageSize, placedVessels));
            }
        }
    }

    [Fact]
    public void CanPlaceVessel_LargeAnchorage_PerformanceTest()
    {
        // Arrange - Test with large anchorage
        var anchorageSize = new AnchorageSize(100, 100);
        var dimensions = new VesselDimensions(10, 10);
        var placedVessels = new List<PlacedVessel>
        {
            new("v1", "Ship", 0, 0, new VesselDimensions(10, 10), false),
            new("v2", "Ship", 50, 50, new VesselDimensions(10, 10), false)
        };

        // Act
        var result = _planner.CanPlaceVessel(90, 90, dimensions, anchorageSize, placedVessels);

        // Assert
        Assert.True(result);
    }

    #endregion
}