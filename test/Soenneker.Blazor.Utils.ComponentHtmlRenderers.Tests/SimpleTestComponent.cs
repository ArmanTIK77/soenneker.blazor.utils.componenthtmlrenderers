using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Soenneker.Blazor.Utils.ComponentHtmlRenderers.Tests;

/// <summary>
/// Minimal IComponent used to verify rendering produces expected HTML.
/// </summary>
public sealed class SimpleTestComponent : IComponent
{
    private RenderHandle _renderHandle;

    public void Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

    public Task SetParametersAsync(ParameterView parameters)
    {
        _renderHandle.Render(Render);
        return Task.CompletedTask;
    }

    private void Render(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "test-component");
        builder.AddContent(2, "Hello from test component");
        builder.CloseElement();
    }
}
