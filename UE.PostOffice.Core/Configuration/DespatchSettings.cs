namespace UE.PostOffice.Core.Configuration;

/// <summary>
/// Represents the configuration settings for handling despatch delays.
/// </summary>
/// <remarks>
/// This settings class is a bit overkill - from one perspective I know the number of days between Saturday and the end
/// of the weekend will always be 2 and likewise 1 for Sunday. However, I wanted to demonstrate moving hardcoded
/// values out of business logic and into configurable settings files.
/// </remarks>
public class DespatchSettings
{
    public int WeekendsSaturdayDelay { get; init; } = 2;
    public int WeekendsSundayDelay { get; init; } = 1;
}