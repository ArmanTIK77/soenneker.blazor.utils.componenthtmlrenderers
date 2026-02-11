using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Blazor.Utils.ComponentHtmlRenderers.Abstract;

/// <summary>
/// Provides functionality for rendering Blazor components to HTML strings using an underlying
/// <see cref="HtmlRenderer"/>.
/// </summary>
/// <remarks>
/// Implementations are responsible for handling dispatcher access requirements internally.
/// This abstraction is intended for scenarios such as:
/// <list type="bullet">
/// <item>Build-time component rendering (e.g., Tailwind content generation)</item>
/// <item>Server-side HTML snapshot generation</item>
/// <item>Unit testing component markup</item>
/// <item>Static site generation</item>
/// </list>
/// No assumptions are made about how the underlying <see cref="IServiceProvider"/> is constructed.
/// </remarks>
public interface IComponentHtmlRenderer : IAsyncDisposable
{
    /// <summary>
    /// Renders the specified Blazor component type to an HTML string.
    /// </summary>
    /// <param name="componentType">
    /// The component <see cref="Type"/> to render. Must implement <see cref="IComponent"/>.
    /// </param>
    /// <param name="parameters">
    /// An optional dictionary of parameter names and values to supply to the component.
    /// If <see langword="null"/>, an empty parameter set is used.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that resolves to the rendered HTML string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="componentType"/> is <see langword="null"/>.
    /// </exception>
    [Pure]
    Task<string> RenderToHtml(Type componentType, IReadOnlyDictionary<string, object?>? parameters = null);

    /// <summary>
    /// Renders the specified Blazor component to an HTML string using a generic fast path.
    /// </summary>
    /// <typeparam name="TComponent">
    /// The component type to render.
    /// </typeparam>
    /// <param name="parameters">
    /// An optional dictionary of parameter names and values to supply to the component.
    /// If <see langword="null"/>, an empty parameter set is used.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that resolves to the rendered HTML string.
    /// </returns>
    /// <remarks>
    /// This overload avoids the need to pass a <see cref="Type"/> instance and may be
    /// preferable when the component type is known at compile time.
    /// </remarks>
    [Pure]
    Task<string> RenderToHtml<TComponent>(IReadOnlyDictionary<string, object?>? parameters = null)
        where TComponent : IComponent;

    /// <summary>
    /// Renders the specified Blazor component type to an HTML string using an inline parameter builder.
    /// </summary>
    /// <param name="componentType">
    /// The component <see cref="Type"/> to render. Must implement <see cref="IComponent"/>.
    /// </param>
    /// <param name="buildParameters">
    /// A delegate used to populate a parameter dictionary for the component.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that resolves to the rendered HTML string.
    /// </returns>
    /// <remarks>
    /// This overload can reduce call-site allocations by allowing parameters to be constructed inline
    /// without requiring a pre-built dictionary.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="componentType"/> or <paramref name="buildParameters"/> is <see langword="null"/>.
    /// </exception>
    [Pure]
    Task<string> RenderToHtml(Type componentType, Action<Dictionary<string, object?>> buildParameters);
}
