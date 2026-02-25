namespace Dumpify;

/// <summary>
/// Extension methods for configuring table layouts in TableConfig.
/// </summary>
public static class TableConfigExtensions
{
    #region SetLayoutForType - Generic

    /// <summary>
    /// Sets the table layout for a specific type.
    /// </summary>
    /// <typeparam name="T">The type to match.</typeparam>
    /// <param name="config">The table config to modify.</param>
    /// <param name="layout">The layout to use for the type.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutForType<T>(this TableConfig config, TableLayout layout)
    {
        return config.SetLayoutForType<T>(TableLayoutResult.From(layout));
    }

    /// <summary>
    /// Sets the table layout for a specific type with config overrides.
    /// </summary>
    /// <typeparam name="T">The type to match.</typeparam>
    /// <param name="config">The table config to modify.</param>
    /// <param name="result">The layout result with optional config overrides.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutForType<T>(this TableConfig config, TableLayoutResult result)
    {
        config.AddLayoutRule(new TableLayoutRule(typeof(T), result));
        return config;
    }

    /// <summary>
    /// Sets the table layout for a specific type using a resolver function.
    /// </summary>
    /// <typeparam name="T">The type to match.</typeparam>
    /// <param name="config">The table config to modify.</param>
    /// <param name="resolver">A function that returns a TableLayoutResult based on the context, or null to skip.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutForType<T>(this TableConfig config, Func<TableLayoutContext, TableLayoutResult?> resolver)
    {
        config.AddLayoutRule(new TableLayoutRule(ctx => ctx.Type == typeof(T) ? resolver(ctx) : null));
        return config;
    }

    #endregion

    #region SetLayoutForType - Non-Generic

    /// <summary>
    /// Sets the table layout for a specific type.
    /// </summary>
    /// <param name="config">The table config to modify.</param>
    /// <param name="type">The type to match.</param>
    /// <param name="layout">The layout to use for the type.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutForType(this TableConfig config, Type type, TableLayout layout)
    {
        config.AddLayoutRule(new TableLayoutRule(type, TableLayoutResult.From(layout)));
        return config;
    }

    /// <summary>
    /// Sets the table layout for a specific type with config overrides.
    /// </summary>
    /// <param name="config">The table config to modify.</param>
    /// <param name="type">The type to match.</param>
    /// <param name="result">The layout result with optional config overrides.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutForType(this TableConfig config, Type type, TableLayoutResult result)
    {
        config.AddLayoutRule(new TableLayoutRule(type, result));
        return config;
    }

    /// <summary>
    /// Sets the table layout for a specific type using a resolver function.
    /// </summary>
    /// <param name="config">The table config to modify.</param>
    /// <param name="type">The type to match.</param>
    /// <param name="resolver">A function that returns a TableLayoutResult based on the context, or null to skip.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutForType(this TableConfig config, Type type, Func<TableLayoutContext, TableLayoutResult?> resolver)
    {
        config.AddLayoutRule(new TableLayoutRule(ctx => ctx.Type == type ? resolver(ctx) : null));
        return config;
    }

    #endregion

    #region SetLayoutForTypeAndDerived - Generic

    /// <summary>
    /// Sets the table layout for a type and all types derived from it.
    /// </summary>
    /// <typeparam name="T">The base type to match.</typeparam>
    /// <param name="config">The table config to modify.</param>
    /// <param name="layout">The layout to use for matching types.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutForTypeAndDerived<T>(this TableConfig config, TableLayout layout)
    {
        return config.SetLayoutForTypeAndDerived<T>(TableLayoutResult.From(layout));
    }

    /// <summary>
    /// Sets the table layout for a type and all types derived from it with config overrides.
    /// </summary>
    /// <typeparam name="T">The base type to match.</typeparam>
    /// <param name="config">The table config to modify.</param>
    /// <param name="result">The layout result with optional config overrides.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutForTypeAndDerived<T>(this TableConfig config, TableLayoutResult result)
    {
        config.AddLayoutRule(new TableLayoutRule(ctx => typeof(T).IsAssignableFrom(ctx.Type) ? result : null));
        return config;
    }

    /// <summary>
    /// Sets the table layout for a type and all types derived from it using a resolver function.
    /// </summary>
    /// <typeparam name="T">The base type to match.</typeparam>
    /// <param name="config">The table config to modify.</param>
    /// <param name="resolver">A function that returns a TableLayoutResult based on the context, or null to skip.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutForTypeAndDerived<T>(this TableConfig config, Func<TableLayoutContext, TableLayoutResult?> resolver)
    {
        config.AddLayoutRule(new TableLayoutRule(ctx => typeof(T).IsAssignableFrom(ctx.Type) ? resolver(ctx) : null));
        return config;
    }

    #endregion

    #region SetLayoutForTypeAndDerived - Non-Generic

    /// <summary>
    /// Sets the table layout for a type and all types derived from it.
    /// </summary>
    /// <param name="config">The table config to modify.</param>
    /// <param name="type">The base type to match.</param>
    /// <param name="layout">The layout to use for matching types.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutForTypeAndDerived(this TableConfig config, Type type, TableLayout layout)
    {
        return config.SetLayoutForTypeAndDerived(type, TableLayoutResult.From(layout));
    }

    /// <summary>
    /// Sets the table layout for a type and all types derived from it with config overrides.
    /// </summary>
    /// <param name="config">The table config to modify.</param>
    /// <param name="type">The base type to match.</param>
    /// <param name="result">The layout result with optional config overrides.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutForTypeAndDerived(this TableConfig config, Type type, TableLayoutResult result)
    {
        config.AddLayoutRule(new TableLayoutRule(ctx => type.IsAssignableFrom(ctx.Type) ? result : null));
        return config;
    }

    /// <summary>
    /// Sets the table layout for a type and all types derived from it using a resolver function.
    /// </summary>
    /// <param name="config">The table config to modify.</param>
    /// <param name="type">The base type to match.</param>
    /// <param name="resolver">A function that returns a TableLayoutResult based on the context, or null to skip.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutForTypeAndDerived(this TableConfig config, Type type, Func<TableLayoutContext, TableLayoutResult?> resolver)
    {
        config.AddLayoutRule(new TableLayoutRule(ctx => type.IsAssignableFrom(ctx.Type) ? resolver(ctx) : null));
        return config;
    }

    #endregion

    #region SetLayoutWhen

    /// <summary>
    /// Sets the table layout based on a predicate.
    /// </summary>
    /// <param name="config">The table config to modify.</param>
    /// <param name="predicate">A function that returns true if the context matches.</param>
    /// <param name="layout">The layout to use for matching types.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutWhen(this TableConfig config, Func<TableLayoutContext, bool> predicate, TableLayout layout)
    {
        return config.SetLayoutWhen(predicate, TableLayoutResult.From(layout));
    }

    /// <summary>
    /// Sets the table layout based on a predicate with config overrides.
    /// </summary>
    /// <param name="config">The table config to modify.</param>
    /// <param name="predicate">A function that returns true if the context matches.</param>
    /// <param name="result">The layout result with optional config overrides.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutWhen(this TableConfig config, Func<TableLayoutContext, bool> predicate, TableLayoutResult result)
    {
        config.AddLayoutRule(new TableLayoutRule(ctx => predicate(ctx) ? result : null));
        return config;
    }

    /// <summary>
    /// Sets the table layout based on a resolver function that can return different results per context.
    /// </summary>
    /// <param name="config">The table config to modify.</param>
    /// <param name="resolver">A function that returns a TableLayoutResult for matching contexts, or null if not matching.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig SetLayoutWhen(this TableConfig config, Func<TableLayoutContext, TableLayoutResult?> resolver)
    {
        config.AddLayoutRule(new TableLayoutRule(resolver));
        return config;
    }

    #endregion

    #region ClearLayoutRules

    /// <summary>
    /// Clears all layout rules.
    /// </summary>
    /// <param name="config">The table config to modify.</param>
    /// <returns>The modified config for chaining.</returns>
    public static TableConfig ClearLayoutRules(this TableConfig config)
    {
        config.ClearLayoutRules();
        return config;
    }

    #endregion
}
