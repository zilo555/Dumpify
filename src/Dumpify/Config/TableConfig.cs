using System.Collections.Concurrent;

namespace Dumpify;

public class TableConfig : ConfigBase<TableConfig>
{
    private readonly ConcurrentQueue<TableLayoutRule> _layoutRules = new();

    /// <summary>
    /// Whether to show row indices for arrays and collections.
    /// When not explicitly set, the layout strategy determines the default
    /// (Horizontal defaults to true, Vertical defaults to false for collections).
    /// </summary>
    public TrackableProperty<bool> ShowRowIndices { get; set; } = new(true);

    /// <summary>
    /// Obsolete: Use ShowRowIndices instead.
    /// </summary>
    [Obsolete("Use ShowRowIndices instead. This property will be removed in a future version.")]
    public TrackableProperty<bool> ShowArrayIndices
    {
        get => ShowRowIndices;
        set => ShowRowIndices = value;
    }

    public TrackableProperty<bool> ShowTableHeaders { get; set; } = new(true);
    public TrackableProperty<bool> NoColumnWrapping { get; set; } = new(false);
    public TrackableProperty<bool> ExpandTables { get; set; } = new(false);
    public TrackableProperty<bool> ShowMemberTypes { get; set; } = new(false);
    public TrackableProperty<bool> ShowRowSeparators { get; set; } = new(false);

    /// <summary>
    /// The border style for tables. Default is Rounded.
    /// Use Ascii or Square for better terminal compatibility.
    /// </summary>
    public TrackableProperty<TableBorderStyle> BorderStyle { get; set; } = new(TableBorderStyle.Rounded);

    /// <summary>
    /// The table layout style. When set, applies to all tables unless a layout rule matches.
    /// Default is Vertical. Use Horizontal for LinqPad-style layout.
    /// </summary>
    public TrackableProperty<TableLayout> TableLayout { get; set; } = new(Dumpify.TableLayout.Vertical);

    /// <summary>
    /// Rules for determining table layout based on type.
    /// Rules are evaluated in order; the first matching rule wins.
    /// Use the extension methods (SetLayoutForType, SetLayoutWhen, etc.) to add rules.
    /// </summary>
    public IEnumerable<TableLayoutRule> LayoutRules => _layoutRules;

    /// <summary>
    /// Adds a layout rule. Internal use only - use extension methods.
    /// </summary>
    internal void AddLayoutRule(TableLayoutRule rule) => _layoutRules.Enqueue(rule);

    /// <summary>
    /// Clears all layout rules. Internal use only - use extension methods.
    /// </summary>
    internal void ClearLayoutRules()
    {
        while (_layoutRules.TryDequeue(out _)) { }
    }

    /// <inheritdoc />
    protected override TableConfig MergeOverride(TableConfig overrideConfig)
    {
        var merged = new TableConfig
        {
            ShowRowIndices = Merge(ShowRowIndices, overrideConfig.ShowRowIndices),
            ShowTableHeaders = Merge(ShowTableHeaders, overrideConfig.ShowTableHeaders),
            NoColumnWrapping = Merge(NoColumnWrapping, overrideConfig.NoColumnWrapping),
            ExpandTables = Merge(ExpandTables, overrideConfig.ExpandTables),
            ShowMemberTypes = Merge(ShowMemberTypes, overrideConfig.ShowMemberTypes),
            ShowRowSeparators = Merge(ShowRowSeparators, overrideConfig.ShowRowSeparators),
            BorderStyle = Merge(BorderStyle, overrideConfig.BorderStyle),
            TableLayout = Merge(TableLayout, overrideConfig.TableLayout),
        };

        // Merge layout rules: override rules take precedence (added first)
        foreach (var rule in overrideConfig.LayoutRules)
        {
            merged.AddLayoutRule(rule);
        }
        foreach (var rule in LayoutRules)
        {
            merged.AddLayoutRule(rule);
        }

        return merged;
    }
}