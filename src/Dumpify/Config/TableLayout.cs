namespace Dumpify;

/// <summary>
/// Defines how tables are rendered for objects and collections.
/// </summary>
public enum TableLayout
{
    /// <summary>
    /// Default vertical layout:
    /// - Single object: Two columns (Name, Value), properties as rows
    /// - Collection: Single column with type name, items as rows
    /// </summary>
    Vertical,

    /// <summary>
    /// Horizontal layout with properties as columns:
    /// - Single object: Properties as columns, single value row
    /// - Collection: Properties as columns, items as rows (LinqPad style)
    /// </summary>
    Horizontal
}
