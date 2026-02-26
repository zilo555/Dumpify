using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;

internal class ArrayTypeRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler) : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler = handler;

    public Type DescriptorType { get; } = typeof(MultiValueDescriptor);

    public (bool shouldHandle, object? handleContext) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type.IsArray && ((Array)obj).Rank < 3, null);

    public IRenderable Render(IDescriptor descriptor, object @object, RenderContext baseContext, object? handleContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;

        var mvd = (MultiValueDescriptor)descriptor;
        var obj = (Array)@object;

        return obj.Rank switch
        {
            1 => RenderSingleDimensionArray(obj, mvd, context),
            2 => RenderTwoDimensionalArray(obj, mvd, context),
            > 2 => RenderHighRankArrays(obj, mvd, context),
            _ => throw new NotImplementedException($"Rendering Array of rank {obj.Rank}")
        };
    }

    private IRenderable RenderSingleDimensionArray(Array obj, MultiValueDescriptor mvd, RenderContext<SpectreRendererState> context)
    {
        // Resolve the layout strategy for the element type
        // Pass isCollectionElement=true and the container type so rules can check if rendering inside a collection
        var (strategy, layoutResult) = TableLayoutResolver.Resolve(
            mvd.ElementsType,
            context,
            isCollectionElement: true,
            containerType: mvd.Type);

        var builder = new ObjectTableBuilder(context, mvd, obj);

        // Add row indices behavior if needed
        // Arrays default to showing indices (backward compatibility), unless explicitly disabled
        if (ShouldShowRowIndicesForArray(context, layoutResult))
        {
            builder.AddBehavior(new RowIndicesTableBuilderBehavior());
        }

        // Use centralized truncation
        var truncated = CollectionTruncator.TruncateArray(
            obj,
            context.Config.TruncationConfig.MaxCollectionCount,
            context.Config.TruncationConfig.Mode);

        // Delegate to the strategy
        strategy.ConfigureCollectionTable(
            builder,
            truncated,
            mvd,
            context,
            (item, itemDescriptor, ctx) => _handler.RenderDescriptor(item, itemDescriptor, ctx),
            marker => RenderTruncationMarker(marker, context));

        return builder.Build();
    }

    /// <summary>
    /// Determines if row indices should be shown for arrays.
    /// Arrays default to showing indices (backward compatibility).
    /// </summary>
    private static bool ShouldShowRowIndicesForArray(RenderContext<SpectreRendererState> context, TableLayoutResult? layoutResult)
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

        // 3. Arrays default to true (backward compatibility)
        return true;
    }

    private static IRenderable RenderTruncationMarker(TruncationMarker marker, RenderContext<SpectreRendererState> context)
    {
        var color = context.State.Colors.MetadataInfoColor;
        return new Markup(Markup.Escape(marker.GetDefaultMessage()), new Style(foreground: color));
    }

    private IRenderable RenderTwoDimensionalArray(Array obj, MultiValueDescriptor descriptor, RenderContext baseContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;

        if (obj.Rank != 2)
        {
            return RenderHighRankArrays(obj, descriptor, context);
        }

        RowIndicesTableBuilderBehavior rowIndicesBehavior = new();
        var builder = new ObjectTableBuilder(context, descriptor, obj)
            .HideTitle()
            .AddBehavior(rowIndicesBehavior);

        var rowsAll = obj.GetLength(0);
        var columnsAll = obj.GetLength(1);

        // Use centralized truncation for rows and columns
        var maxCount = context.Config.TruncationConfig.MaxCollectionCount.Value;
        var mode = context.Config.TruncationConfig.Mode.Value;

        // For 2D arrays, we apply truncation per dimension
        var rowTruncated = CollectionTruncator.Truncate(
            Enumerable.Range(0, rowsAll),
            maxCount,
            mode);

        var colTruncated = CollectionTruncator.Truncate(
            Enumerable.Range(0, columnsAll),
            maxCount,
            mode);

        var colorConfig = context.Config.ColorConfig;

        if (context.Config.TypeNamingConfig.ShowTypeNames)
        {
            var (typeName, rank) = context.Config.TypeNameProvider.GetJaggedArrayNameWithRank(descriptor.Type);
            builder.SetTitle($"{typeName}[{rowsAll},{columnsAll}]");
        }

        // Add column headers
        var columnStyle = new Style(foreground: colorConfig.ColumnNameColor.ToSpectreColor());

        // Start column marker
        if (colTruncated.StartMarker != null)
        {
            builder.AddColumnName(colTruncated.StartMarker.GetCompactMessage(), columnStyle);
        }

        for (int i = 0; i < colTruncated.Items.Count; i++)
        {
            // Middle marker for columns
            if (colTruncated.MiddleMarkerIndex == i && colTruncated.MiddleMarker != null)
            {
                builder.AddColumnName(colTruncated.MiddleMarker.GetCompactMessage(), columnStyle);
            }

            builder.AddColumnName(colTruncated.Items[i].ToString(), columnStyle);
        }

        // End column marker
        if (colTruncated.EndMarker != null)
        {
            builder.AddColumnName(colTruncated.EndMarker.GetCompactMessage(), columnStyle);
        }

        // Track current row for index behavior
        int currentRow = 0;

        // Start row marker
        if (rowTruncated.StartMarker != null)
        {
            rowIndicesBehavior.AddHideIndexForRow(currentRow, rowTruncated.StartMarker.GetCompactMessage());
            var cells = CreateEmptyCells(colTruncated);
            builder.AddMarkerRow(cells);
            currentRow++;
        }

        // Render rows
        for (int ri = 0; ri < rowTruncated.Items.Count; ri++)
        {
            // Middle marker for rows
            if (rowTruncated.MiddleMarkerIndex == ri && rowTruncated.MiddleMarker != null)
            {
                rowIndicesBehavior.AddHideIndexForRow(currentRow, rowTruncated.MiddleMarker.GetCompactMessage());
                var cells = CreateEmptyCells(colTruncated);
                builder.AddMarkerRow(cells);
                currentRow++;
            }

            var row = rowTruncated.Items[ri];
            var rowCells = new List<IRenderable>();

            // Start column marker cell
            if (colTruncated.StartMarker != null)
            {
                rowCells.Add(new Markup(""));
            }

            for (int ci = 0; ci < colTruncated.Items.Count; ci++)
            {
                // Middle marker for columns
                if (colTruncated.MiddleMarkerIndex == ci && colTruncated.MiddleMarker != null)
                {
                    rowCells.Add(new Markup(""));
                }

                var col = colTruncated.Items[ci];
                var item = obj.GetValue(row, col);

                var type = descriptor.ElementsType ?? item?.GetType();
                IDescriptor? itemsDescriptor = type is not null
                    ? DumpConfig.Default.Generator.Generate(type, null, context.Config.MemberProvider)
                    : null;

                var renderedItem = _handler.RenderDescriptor(item, itemsDescriptor, context);
                rowCells.Add(renderedItem);
            }

            // End column marker cell
            if (colTruncated.EndMarker != null)
            {
                rowCells.Add(new Markup(""));
            }

            builder.AddRow(null, null, rowCells);
            currentRow++;
        }

        // End row marker
        if (rowTruncated.EndMarker != null)
        {
            rowIndicesBehavior.AddHideIndexForRow(currentRow, rowTruncated.EndMarker.GetCompactMessage());
            var cells = CreateEmptyCells(colTruncated);
            builder.AddMarkerRow(cells);
        }

        return builder.Build();

        List<IRenderable> CreateEmptyCells(TruncatedEnumerable<int> colTruncation)
        {
            var count = colTruncation.Items.Count;
            if (colTruncation.StartMarker != null) count++;
            if (colTruncation.MiddleMarker != null) count++;
            if (colTruncation.EndMarker != null) count++;

            return Enumerable.Range(0, count).Select(_ => (IRenderable)new Markup("")).ToList();
        }
    }

    private IRenderable RenderHighRankArrays(Array arr, MultiValueDescriptor descriptor, RenderContext context)
        => _handler.RenderDescriptor(arr, descriptor, (RenderContext<SpectreRendererState>)context);
}