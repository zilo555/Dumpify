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
    /// Indicates whether this type is being rendered as an element of a collection or array.
    /// </summary>
    public bool IsCollectionElement { get; }

    /// <summary>
    /// The container type (e.g., Employee[] or List&lt;Employee&gt;) if this type is being rendered
    /// as part of a collection. Null if rendering a standalone object.
    /// </summary>
    public Type? ContainerType { get; }

    /// <summary>
    /// Creates a new TableLayoutContext.
    /// </summary>
    /// <param name="type">The type being rendered.</param>
    /// <param name="currentDepth">The current nesting depth.</param>
    /// <param name="isCollectionElement">Whether this type is being rendered as part of a collection.</param>
    /// <param name="containerType">The container type if rendering as part of a collection.</param>
    public TableLayoutContext(Type type, int currentDepth, bool isCollectionElement = false, Type? containerType = null)
    {
        Type = type;
        CurrentDepth = currentDepth;
        IsCollectionElement = isCollectionElement;
        ContainerType = containerType;
    }
}
