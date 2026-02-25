using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;

/// <summary>
/// Vertical table layout strategy - the default layout.
/// - Single object: Two columns (Name, Value), properties as rows
/// - Collection: Single column with type name, items as rows
/// </summary>
internal class VerticalTableLayoutStrategy : ITableLayoutStrategy
{
    /// <summary>
    /// Vertical layout does not show row indices by default for non-array collections.
    /// Arrays handle their own row indices via ArrayTypeRenderer.
    /// </summary>
    public bool DefaultShowRowIndices => false;

    public void ConfigureObjectTable(
        ObjectTableBuilder builder,
        object obj,
        ObjectDescriptor descriptor,
        RenderContext<SpectreRendererState> context,
        Func<object, IDescriptor, RenderContext<SpectreRendererState>, (bool success, object? value, IRenderable rendered)> getValueAndRender)
    {
        builder.AddDefaultColumnNames();

        foreach (var property in descriptor.Properties)
        {
            var (success, value, renderedValue) = getValueAndRender(obj, property, context with { CurrentDepth = context.CurrentDepth + 1 });
            builder.AddRowWithObjectName(property, value, renderedValue);
        }
    }

    public void ConfigureCollectionTable(
        ObjectTableBuilder builder,
        TruncatedEnumerable<object?> items,
        MultiValueDescriptor descriptor,
        RenderContext<SpectreRendererState> context,
        Func<object?, IDescriptor?, RenderContext<SpectreRendererState>, IRenderable> renderItem,
        Func<TruncationMarker, IRenderable> renderMarker)
    {
        builder.HideTitle();

        var typeName = GetTypeName(descriptor.Type, items.TotalCount, context);
        builder.AddColumnName(typeName, new Style(foreground: context.State.Colors.TypeNameColor));

        // Hide headers if configured
        if (!context.Config.TableConfig.ShowTableHeaders || !context.Config.TypeNamingConfig.ShowTypeNames)
        {
            builder.HideHeaders();
        }

        items.ForEachWithMarkers(
            onMarker: marker =>
            {
                var renderedMarker = renderMarker(marker);
                builder.AddMarkerRow(new[] { renderedMarker });
            },
            onItem: (item, _) =>
            {
                var type = descriptor.ElementsType ?? item?.GetType();

                IDescriptor? itemsDescriptor = type is not null
                    ? DumpConfig.Default.Generator.Generate(type, null, context.Config.MemberProvider)
                    : null;

                var renderedItem = renderItem(item, itemsDescriptor, context);
                builder.AddRow(itemsDescriptor, item, renderedItem);
            });
    }

    private static string GetTypeName(Type type, int? itemCount, RenderContext<SpectreRendererState> context)
    {
        if (type.IsArray)
        {
            var elementTypeName = type.GetElementType() is { } elementType
                ? context.Config.TypeNameProvider.GetTypeName(elementType)
                : "";

            // Only show count for single-dimension arrays
            // Multi-dimensional arrays (Rank > 1) should show the rank format like int[,,]
            if (type.GetArrayRank() == 1 && itemCount.HasValue)
            {
                return $"{elementTypeName}[{itemCount.Value}]";
            }

            var (name, rank) = context.Config.TypeNameProvider.GetJaggedArrayNameWithRank(type);
            return $"{name}[{new string(',', rank + 1)}]";
        }

        return context.Config.TypeNameProvider.GetTypeName(type);
    }
}
