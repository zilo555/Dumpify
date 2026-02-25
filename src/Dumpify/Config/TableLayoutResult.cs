namespace Dumpify;

/// <summary>
/// Result of a table layout rule evaluation, containing the layout and optional config overrides.
/// </summary>
public class TableLayoutResult
{
    /// <summary>
    /// The table layout to use.
    /// </summary>
    public required TableLayout Layout { get; init; }

    /// <summary>
    /// Override for showing row indices. Null means use default resolution.
    /// </summary>
    public bool? ShowRowIndices { get; init; }

    /// <summary>
    /// Override for showing member types. Null means use default resolution.
    /// </summary>
    public bool? ShowMemberTypes { get; init; }

    /// <summary>
    /// Override for showing row separators. Null means use default resolution.
    /// </summary>
    public bool? ShowRowSeparators { get; init; }

    /// <summary>
    /// Override for showing table headers. Null means use default resolution.
    /// </summary>
    public bool? ShowTableHeaders { get; init; }

    /// <summary>
    /// Creates a TableLayoutResult with just the layout, no config overrides.
    /// </summary>
    public static TableLayoutResult From(TableLayout layout) => new() { Layout = layout };

    /// <summary>
    /// Represents no match. Use this in resolver functions to indicate the rule doesn't apply.
    /// </summary>
    public static TableLayoutResult? None => null;
}
