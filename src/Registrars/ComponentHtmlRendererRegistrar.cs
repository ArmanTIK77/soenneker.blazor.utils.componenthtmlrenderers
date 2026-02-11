using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Blazor.Utils.ComponentHtmlRenderers.Abstract;

namespace Soenneker.Blazor.Utils.ComponentHtmlRenderers.Registrars;

/// <summary>
/// A headless Blazor renderer
/// </summary>
public static class ComponentHtmlRendererRegistrar
{
    /// <summary>
    /// Adds <see cref="IComponentHtmlRenderer"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddComponentHtmlRendererAsSingleton(this IServiceCollection services)
    {
        services.TryAddSingleton<IComponentHtmlRenderer, ComponentHtmlRenderer>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="IComponentHtmlRenderer"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddComponentHtmlRendererAsScoped(this IServiceCollection services)
    {
        services.TryAddScoped<IComponentHtmlRenderer, ComponentHtmlRenderer>();

        return services;
    }
}
