# Anchorage Bin-Packing Puzzle

A production-quality Blazor WebAssembly application for solving interactive anchorage bin-packing puzzles using the Electronic Shipping Agent (ESA) API.

## Overview

This application retrieves random fleet configurations from the ESA API and presents users with an interactive puzzle: fit various vessel types into a constrained anchorage space while managing collisions, boundaries, and optional rotations.

### Key Features

- **Dynamic Fleet Loading**: Retrieves anchorage dimensions and vessel configurations from `https://esa.instech.no/api/fleets/random`
- **Interactive Drag & Drop**: Intuitive vessel placement with real-time validation
- **Collision Detection**: Prevents vessel overlaps and out-of-bounds placement
- **90° Rotation**: Double-click vessels to rotate (width/height swapped)
- **Live Status Tracking**: Displays remaining vessel counts and progress
- **Win Detection**: Congratulations screen when all vessels are placed
- **Clean Architecture**: Separation of concerns across services, components, and domain logic

## Architecture

### Project Structure

BlazorAnchorageBinpackPuzzle/
??? Models/
?   ??? AnchorageSize.cs            # Grid dimensions record
?   ??? VesselDimensions.cs         # Vessel size record with rotation
?   ??? VesselType.cs               # Fleet item definition
?   ??? PlacedVessel.cs             # Positioned vessel instance
?   ??? FleetResponse.cs            # API response model
??? Services/
?   ??? IFleetService.cs            # API contract
?   ??? FleetService.cs             # HTTP client implementation
?   ??? AnchoragePlanner.cs         # Pure domain logic
??? State/
?   ??? AnchorageState.cs           # Session state management
??? Components/
?   ??? AnchorageGrid.razor         # Grid rendering & drop zone
?   ??? VesselPalette.razor         # Available vessels list
?   ??? VesselItem.razor            # Draggable vessel component
??? Pages/
?   ??? AnchoragePage.razor         # Main puzzle page
??? wwwroot/
?   ??? css/
?       ??? app.css                 # Styling (Bootstrap + custom)
??? Program.cs                      # DI configuration


### Design Patterns

1. **Service Layer**: `IFleetService` provides typed access to the ESA API
2. **Domain Logic**: `AnchoragePlanner` contains pure, stateless validation functions
3. **State Management**: `AnchorageState` encapsulates session data
4. **Dependency Injection**: All services registered in `Program.cs`
5. **Component Composition**: Small, focused Razor components with single responsibilities

## Getting Started

### Prerequisites

- .NET 9 SDK
- Visual Studio 2026 or VS Code
- Modern web browser

### Configuration

Update the HttpClient base address in `Program.cs` if the ESA API URL changes:

### Running the Application

dotnet run

Navigate to `https://localhost:7000` (or your configured port).

## Usage

1. **Start**: Click "Start New Puzzle" to load a random fleet
2. **View**: Examine the anchorage grid and available vessels
3. **Drag**: Drag vessels from the palette into the grid
4. **Rotate**: Double-click a vessel to rotate it 90°
5. **Remove**: Click a placed vessel to unplace it
6. **Win**: Place all vessels to unlock the success state

## API Specification

### GET `/api/fleets/random`

**Response Example:**

{
  "anchorageSize": {
    "width": 12,
    "height": 15
  },
  "fleets": [
    {
      "singleShipDimensions": {
        "width": 6,
        "height": 5
      },
      "shipDesignation": "LNG Unit",
      "shipCount": 2
    },
    {
      "singleShipDimensions": {
        "width": 3,
        "height": 12
      },
      "shipDesignation": "Science & Engineering Ship",
      "shipCount": 5
    }
  ]
}


## Domain Logic

### Placement Validation

The `AnchoragePlanner` service validates:

1. **Boundary Check**: Vessel fits within anchorage dimensions
2. **Collision Detection**: No overlap with existing vessels (AABB algorithm)
3. **Rotation Support**: Dimensions swapped on 90° rotation
4. **Win Condition**: All vessels placed (remaining count = 0)

### Collision Detection

Uses Axis-Aligned Bounding Box (AABB) algorithm:
- Two rectangles collide if their projections on both axes overlap
- O(n) complexity per placement, negligible for typical fleet sizes

## Styling & UX

- **Bootstrap 5**: Responsive grid layout and components
- **Custom CSS**: Grid visualization with 40px cells
- **Drag-Drop Feedback**: Hover states and transitions
- **Color Coding**: Vessel types distinguished by gradient backgrounds
- **Status Panel**: Real-time progress tracking

## Error Handling

- Network errors: Displays user-friendly message with retry button
- API failures: Graceful error display with stack traces logged to console
- Validation errors: Silent failures prevent invalid placements

## Security & Assumptions

- CORS and authentication assumed to be handled by the ESA API infrastructure
- No persistent storage; state resets on page reload
- Client-side validation only; server-side validation should be implemented for production

## Future Enhancements

- Undo/redo stack
- Vessel rotation via UI button (alternative to double-click)
- Suggested placements or hints
- Leaderboard or completion statistics
- Puzzle difficulty levels
- Keyboard shortcuts (arrow keys to move, R to rotate)
- Accessibility improvements (screen reader support)

## Development

### Adding New Features

1. **New Vessel Logic**: Extend `AnchoragePlanner`
2. **New Components**: Add to `Components/` folder with clear parameter contracts
3. **State Changes**: Update `AnchorageState` class
4. **API Updates**: Modify `FleetService` and `FleetResponse` model

### Testing Strategy

- Unit tests for `AnchoragePlanner` (collision detection, boundary checks)
- Component tests for drag-drop behavior
- Integration tests for API calls
- Manual E2E testing for UI/UX flows

## License

[Insert License]

## Support

For issues, contact the development team or submit to the project repository.