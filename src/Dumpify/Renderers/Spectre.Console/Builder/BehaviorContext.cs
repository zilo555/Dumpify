namespace Dumpify;

public class BehaviorContext
{
    public required int TotalAvailableRows { get; init; }
    public required int AddedRows { get; init; }

    /// <summary>
    /// Whether the current row is a marker row (e.g., truncation indicator).
    /// Marker rows have null descriptor and null object.
    /// </summary>
    public bool IsMarkerRow { get; init; }
}