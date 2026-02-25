namespace Dumpify;

/// <summary>
/// Context information passed to table layout rule resolvers.
/// </summary>
public class TableLayoutContext
{
    /// <summary>
    /// The type being rendered.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// The current nesting depth (0 = root level).
    /// </summary>
    public int CurrentDepth { get; }

    /// <summary>
    /// Creates a new TableLayoutContext.
    /// </summary>
    /// <param name="type">The type being rendered.</param>
    /// <param name="currentDepth">The current nesting depth.</param>
    public TableLayoutContext(Type type, int currentDepth)
    {
        Type = type;
        CurrentDepth = currentDepth;
    }
}
