using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;

public class RowIndicesTableBuilderBehavior : ITableBuilderBehavior
{
    private readonly string _indexColumnName = Markup.Escape("#");
    private readonly Dictionary<int, string?> _rowIndexOverrides = new();
    private int _dataItemIndex = 0;

    public IEnumerable<IRenderable> GetAdditionalCells(object? obj, IDescriptor? currentDescriptor, RenderContext<SpectreRendererState> context)
    {
        yield break;
    }

    public IEnumerable<IRenderable> GetAdditionalColumns(RenderContext<SpectreRendererState> context)
    {
        yield return new Markup(_indexColumnName, new Style(foreground: context.State.Colors.ColumnNameColor));
    }

    public IEnumerable<IRenderable> GetAdditionalRowElements(BehaviorContext behaviorContext, RenderContext<SpectreRendererState> context)
    {
        var rowIndex = behaviorContext.AddedRows;

        // Check for explicit override
        if (_rowIndexOverrides.TryGetValue(rowIndex, out var overrideValue))
        {
            yield return new Markup(Markup.Escape(overrideValue ?? string.Empty), new Style(foreground: context.State.Colors.ColumnNameColor));
            yield break;
        }

        // Check if this is a marker row (truncation indicator) - uses IsMarkerRow from ObjectTableBuilder
        if (behaviorContext.IsMarkerRow)
        {
            yield return new Markup(string.Empty, new Style(foreground: context.State.Colors.ColumnNameColor));
            yield break;
        }

        // Regular data row - show the data item index
        yield return new Markup(Markup.Escape(_dataItemIndex.ToString()), new Style(foreground: context.State.Colors.ColumnNameColor));
        _dataItemIndex++;
    }

    public void AddHideIndexForRow(int row, string? value = null)
    {
        _rowIndexOverrides.Add(row, value);
    }
}