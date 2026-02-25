namespace Dumpify;

/// <summary>
/// Resolves the table layout strategy and effective configuration for a given type.
/// </summary>
internal class TableLayoutResolver
{
    /// <summary>
    /// Resolves the layout strategy and any config overrides for the given type.
    /// </summary>
    /// <param name="type">The type to resolve layout for.</param>
    /// <param name="context">The render context.</param>
    /// <returns>A tuple of the strategy and optional layout result with config overrides.</returns>
    public static (ITableLayoutStrategy strategy, TableLayoutResult? layoutResult) Resolve(
        Type? type,
        RenderContext<SpectreRendererState> context)
    {
        TableLayoutResult? matchedResult = null;

        // Check layout rules in order
        if (type != null)
        {
            var layoutContext = new TableLayoutContext(type, context.CurrentDepth);
            foreach (var rule in context.Config.TableConfig.LayoutRules)
            {
                var result = rule.Resolver(layoutContext);
                if (result != null)
                {
                    matchedResult = result;
                    break;
                }
            }
        }

        // Get the layout (from matched rule or default)
        var layout = matchedResult?.Layout ?? context.Config.TableConfig.TableLayout.Value;

        // Create the appropriate strategy
        var strategy = CreateStrategy(layout);

        return (strategy, matchedResult);
    }

    /// <summary>
    /// Determines if row indices should be shown based on config and strategy defaults.
    /// Resolution order:
    /// 1. TableLayoutResult override (from matched rule)
    /// 2. TableConfig.ShowRowIndices if explicitly set
    /// 3. Strategy default (Horizontal=true, Vertical=false)
    /// </summary>
    public static bool ShouldShowRowIndices(
        RenderContext<SpectreRendererState> context,
        ITableLayoutStrategy strategy,
        TableLayoutResult? layoutResult)
    {
        // 1. Check layout result override
        if (layoutResult?.ShowRowIndices is bool ruleOverride)
        {
            return ruleOverride;
        }

        // 2. Check if config value was explicitly set
        if (context.Config.TableConfig.ShowRowIndices.IsSet)
        {
            return context.Config.TableConfig.ShowRowIndices.Value;
        }

        // 3. Use strategy default
        return strategy.DefaultShowRowIndices;
    }

    /// <summary>
    /// Determines if member types should be shown based on config and layout result.
    /// </summary>
    public static bool ShouldShowMemberTypes(
        RenderContext<SpectreRendererState> context,
        TableLayoutResult? layoutResult)
    {
        // Check layout result override
        if (layoutResult?.ShowMemberTypes is bool ruleOverride)
        {
            return ruleOverride;
        }

        // Use config value
        return context.Config.TableConfig.ShowMemberTypes.Value;
    }

    /// <summary>
    /// Determines if row separators should be shown based on config and layout result.
    /// </summary>
    public static bool ShouldShowRowSeparators(
        RenderContext<SpectreRendererState> context,
        TableLayoutResult? layoutResult)
    {
        // Check layout result override
        if (layoutResult?.ShowRowSeparators is bool ruleOverride)
        {
            return ruleOverride;
        }

        // Use config value
        return context.Config.TableConfig.ShowRowSeparators.Value;
    }

    /// <summary>
    /// Determines if table headers should be shown based on config and layout result.
    /// </summary>
    public static bool ShouldShowTableHeaders(
        RenderContext<SpectreRendererState> context,
        TableLayoutResult? layoutResult)
    {
        // Check layout result override
        if (layoutResult?.ShowTableHeaders is bool ruleOverride)
        {
            return ruleOverride;
        }

        // Use config value
        return context.Config.TableConfig.ShowTableHeaders.Value;
    }

    private static ITableLayoutStrategy CreateStrategy(TableLayout layout) => layout switch
    {
        TableLayout.Horizontal => new HorizontalTableLayoutStrategy(),
        TableLayout.Vertical => new VerticalTableLayoutStrategy(),
        _ => new VerticalTableLayoutStrategy()
    };
}
