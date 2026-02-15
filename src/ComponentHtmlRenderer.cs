using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.HtmlRendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Soenneker.Blazor.Utils.ComponentHtmlRenderers.Abstract;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Soenneker.Extensions.String;

namespace Soenneker.Blazor.Utils.ComponentHtmlRenderers;

/// <summary>
/// Renders Blazor components to HTML strings using Blazor's <see cref="HtmlRenderer"/>.
/// No framework-specific registrations are performed; all initialization is external.
/// Reflection is used only during warm-up to compile delegates, then cached.
/// </summary>
public sealed class ComponentHtmlRenderer : IComponentHtmlRenderer
{
    private readonly HtmlRenderer _renderer;
    private readonly IServiceProvider _serviceProvider;
    private readonly bool _disposeServiceProvider;

    public ComponentHtmlRenderer(IServiceProvider serviceProvider, bool disposeServiceProvider = false)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _disposeServiceProvider = disposeServiceProvider;

        var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();

        if (loggerFactory == null)
            throw new Exception("ILoggerFactory on the service provider is required");

        _renderer = new HtmlRenderer(_serviceProvider, loggerFactory);
    }

    public ComponentHtmlRenderer(Action<IServiceCollection> configureServices, bool disposeServiceProvider = true)
        : this(configureServices, static sc => sc.BuildServiceProvider(), disposeServiceProvider)
    {
    }

    public ComponentHtmlRenderer(
        Action<IServiceCollection> configureServices,
        Func<IServiceCollection, IServiceProvider> buildServiceProvider,
        bool disposeServiceProvider = true)
        : this(Build(configureServices, buildServiceProvider), disposeServiceProvider)
    {
    }

    private static IServiceProvider Build(
        Action<IServiceCollection> configureServices,
        Func<IServiceCollection, IServiceProvider> buildServiceProvider)
    {
        if (configureServices is null) throw new ArgumentNullException(nameof(configureServices));
        if (buildServiceProvider is null) throw new ArgumentNullException(nameof(buildServiceProvider));

        var services = new ServiceCollection();
        configureServices(services);
        return buildServiceProvider(services);
    }

    public Task<string> RenderToHtml(Type componentType, IReadOnlyDictionary<string, object?>? parameters = null)
    {
        if (componentType is null) throw new ArgumentNullException(nameof(componentType));

        parameters ??= EmptyParams.Instance;

        if (_renderer.Dispatcher.CheckAccess())
            return RenderCore(componentType, parameters);

        return _renderer.Dispatcher.InvokeAsync(() => RenderCore(componentType, parameters));
    }

    public Task<string> RenderToHtml<TComponent>(IReadOnlyDictionary<string, object?>? parameters = null)
        where TComponent : IComponent
    {
        parameters ??= EmptyParams.Instance;

        if (_renderer.Dispatcher.CheckAccess())
            return RenderCore(typeof(TComponent), parameters);

        return _renderer.Dispatcher.InvokeAsync(() => RenderCore(typeof(TComponent), parameters));
    }

    public Task<string> RenderToHtml(Type componentType, Action<Dictionary<string, object?>> buildParameters)
    {
        if (componentType is null) throw new ArgumentNullException(nameof(componentType));
        if (buildParameters is null) throw new ArgumentNullException(nameof(buildParameters));

        var dict = new Dictionary<string, object?>(4);
        buildParameters(dict);

        if (_renderer.Dispatcher.CheckAccess())
            return RenderCore(componentType, dict);

        return _renderer.Dispatcher.InvokeAsync(() => RenderCore(componentType, dict));
    }

    private async Task<string> RenderCore(Type componentType, IReadOnlyDictionary<string, object?> parameters)
    {
        ParameterView pv = parameters.Count == 0
            ? ParameterView.Empty
            : ParameterView.FromDictionary(new Dictionary<string, object?>(parameters));

        HtmlRootComponent root = await _renderer.RenderComponentAsync(componentType, pv).NoSync();

        await using var sw = new StringWriter();
        root.WriteHtmlTo(sw);
        string html = sw.ToString();
        return await ToBrowserHtml(html);
    }

    private static async ValueTask<string> ToBrowserHtml(string html, CancellationToken cancellationToken = default)
    {
        if (html.IsNullOrEmpty())
            return html;

        IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        IDocument document = await context.OpenNewAsync(cancellation: cancellationToken);

        var parser = context.GetService<IHtmlParser>();
        INodeList fragment = parser.ParseFragment(html, document.Body);

        await using var writer = new StringWriter();

        foreach (INode node in fragment)
            await node.ToHtmlAsync(writer).NoSync();

        return writer.ToString();
    }

    public async ValueTask DisposeAsync()
    {
        await _renderer.DisposeAsync().NoSync();

        if (_disposeServiceProvider)
        {
            switch (_serviceProvider)
            {
                case IAsyncDisposable ad:
                    await ad.DisposeAsync().NoSync();
                    break;
                case IDisposable d:
                    d.Dispose();
                    break;
            }
        }
    }
}
