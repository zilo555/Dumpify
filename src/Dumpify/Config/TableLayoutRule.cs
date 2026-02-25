namespace Dumpify;

/// <summary>
/// A rule that determines the table layout for a type based on a predicate.
/// </summary>
/// <remarks>
/// Creates a rule with a custom resolver function.
/// </remarks>
/// <param name="resolver">Function that returns a TableLayoutResult for matching types, or null if not matching.</param>
public class TableLayoutRule(Func<TableLayoutContext, TableLayoutResult?> resolver)
{
    /// <summary>
    /// The resolver function that evaluates whether this rule applies to a type.
    /// Returns a TableLayoutResult if the rule matches, or null if it doesn't.
    /// </summary>
    public Func<TableLayoutContext, TableLayoutResult?> Resolver { get; } = resolver;

    /// <summary>
    /// Creates a rule that matches an exact type.
    /// </summary>
    /// <param name="type">The type to match.</param>
    /// <param name="result">The layout result to return when matched.</param>
    public TableLayoutRule(Type type, TableLayoutResult result)
        : this(ctx => ctx.Type == type ? result : null)
    {
    }

    /// <summary>
    /// Creates a rule that matches an exact type with just a layout.
    /// </summary>
    /// <param name="type">The type to match.</param>
    /// <param name="layout">The layout to use when matched.</param>
    public TableLayoutRule(Type type, TableLayout layout)
        : this(type, TableLayoutResult.From(layout))
    {
    }
}
