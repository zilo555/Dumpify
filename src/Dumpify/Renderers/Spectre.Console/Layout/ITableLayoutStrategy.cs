using Dumpify.Descriptors;
using Spectre.Console.Rendering;

namespace Dumpify;

/// <summary>
/// Strategy interface for configuring table layout for objects and collections.
/// </summary>
internal interface ITableLayoutStrategy
{
    /// <summary>
    /// The default value for ShowRowIndices when not explicitly configured.
    /// </summary>
    bool DefaultShowRowIndices { get; }

    /// <summary>
    /// Configures the table builder for rendering a single object.
    /// </summary>
    /// <param name="builder">The table builder to configure.</param>
    /// <param name="obj">The object being rendered.</param>
    /// <param name="descriptor">The object descriptor with property information.</param>
    /// <param name="context">The render context.</param>
    /// <param name="getValueAndRender">Function to get and render a property value.</param>
    void ConfigureObjectTable(
        ObjectTableBuilder builder,
        object obj,
        ObjectDescriptor descriptor,
        RenderContext<SpectreRendererState> context,
        Func<object, IDescriptor, RenderContext<SpectreRendererState>, (bool success, object? value, IRenderable rendered)> getValueAndRender);

    /// <summary>
    /// Configures the table builder for rendering a collection.
    /// </summary>
    /// <param name="builder">The table builder to configure.</param>
    /// <param name="items">The truncated collection items.</param>
    /// <param name="descriptor">The multi-value descriptor with element type information.</param>
    /// <param name="context">The render context.</param>
    /// <param name="renderItem">Function to render a single item.</param>
    /// <param name="renderMarker">Function to render a truncation marker.</param>
    void ConfigureCollectionTable(
        ObjectTableBuilder builder,
        TruncatedEnumerable<object?> items,
        MultiValueDescriptor descriptor,
        RenderContext<SpectreRendererState> context,
        Func<object?, IDescriptor?, RenderContext<SpectreRendererState>, IRenderable> renderItem,
        Func<TruncationMarker, IRenderable> renderMarker);
}
