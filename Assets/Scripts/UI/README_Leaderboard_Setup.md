# Leaderboard Integration Setup Guide

This guide explains how to set up the leaderboard integration between the API response and UI components.

## Overview

The leaderboard system consists of several components:

1. **LeaderboardData.cs** - Data models for API response
2. **Leaderboard.cs** - API communication
3. **LeaderboardEntryUI.cs** - Individual entry UI component
4. **LeaderboardManager.cs** - Main manager for the leaderboard
5. **LeaderboardSetupHelper.cs** - Helper for UI setup

## Setup Instructions

### 1. Create UI Structure

Create a UI hierarchy for your leaderboard:

```
LeaderboardPanel (GameObject)
├── LeaderboardManager (Script)
├── Entry1 (GameObject)
│   ├── LeaderboardEntryUI (Script)
│   ├── UsernameText (TextMeshProUGUI)
│   └── ScoreText (TextMeshProUGUI)
├── Entry2 (GameObject)
│   ├── LeaderboardEntryUI (Script)
│   ├── UsernameText (TextMeshProUGUI)
│   └── ScoreText (TextMeshProUGUI)
└── ... (more entries as needed)
```

### 2. Configure LeaderboardManager

1. Add the `LeaderboardManager` script to your leaderboard panel
2. In the inspector, assign all `LeaderboardEntryUI` components to the `Leaderboard Entries` list
3. Configure settings:
   - **Auto Refresh On Enable**: Automatically fetch data when the panel is enabled
   - **Refresh Interval**: How often to refresh the data (in seconds)

### 3. Configure LeaderboardEntryUI

For each entry:

1. Add the `LeaderboardEntryUI` script to the entry GameObject
2. Assign the `UsernameText` and `ScoreText` TextMeshProUGUI components
3. Optionally customize the default values ("???")

### 4. Using the Setup Helper (Optional)

The `LeaderboardSetupHelper` can assist with setup:

1. Add the script to your leaderboard panel
2. Use the context menu "Auto Setup Leaderboard Entries" to create entry GameObjects
3. Manually assign the TextMeshProUGUI components in the inspector

## Features

### Automatic Data Handling

- Fetches data from the API endpoint
- Handles null/empty data by showing "???"
- Auto-refreshes at configurable intervals
- Error handling with fallback to default values

### API Integration

- Parses JSON response from `https://padj-hook-api.vercel.app/api/v2/leaderboard`
- Supports the expected response format:

```json
{
  "success": true,
  "message": "Success",
  "data": [
    {
      "username": "Player1",
      "score": 1000
    },
    {
      "username": "Player2",
      "score": 850
    }
  ]
}
```

### Public Methods

The `LeaderboardManager` provides these public methods:

- `RefreshLeaderboard()` - Manually refresh the data
- `ClearLeaderboard()` - Clear all entries to default values

## Error Handling

The system handles various error scenarios:

1. **Network errors** - Shows default values
2. **API errors** - Shows default values
3. **Null/empty data** - Shows "???" for missing entries
4. **JSON parsing errors** - Shows default values

## Customization

### Default Values

You can customize the default text shown when data is null:

- In `LeaderboardEntryUI`: Set `Default Username` and `Default Score`
- Default is "???" for both

### Refresh Behavior

- Disable `Auto Refresh On Enable` to prevent automatic fetching
- Adjust `Refresh Interval` to change how often data updates
- Use `RefreshLeaderboard()` for manual control

### UI Styling

- Modify the TextMeshProUGUI components for visual styling
- Add animations or effects as needed
- The system only handles text content, not visual appearance

## Troubleshooting

### Common Issues

1. **No data showing**: Check API endpoint and network connectivity
2. **"???" showing**: Data is null/empty - check API response
3. **UI not updating**: Ensure TextMeshProUGUI components are assigned
4. **Script errors**: Check that all required components are properly assigned

### Debug Information

The system logs detailed information to the console:

- Successful data fetches
- Error messages
- API response content
- Setup information

## Example Usage

```csharp
// Get reference to LeaderboardManager
LeaderboardManager manager = FindObjectOfType<LeaderboardManager>();

// Manually refresh
manager.RefreshLeaderboard();

// Clear all entries
manager.ClearLeaderboard();
```
