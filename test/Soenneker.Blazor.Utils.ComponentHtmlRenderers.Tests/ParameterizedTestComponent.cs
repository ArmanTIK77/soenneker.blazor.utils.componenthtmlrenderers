using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Soenneker.Blazor.Utils.ComponentHtmlRenderers.Tests;

/// <summary>
/// Component that accepts a string parameter for testing parameter passing.
/// </summary>
public sealed class ParameterizedTestComponent : IComponent
{
    private RenderHandle _renderHandle;

    [Parameter]
    public string Message { get; set; } = string.Empty;

    public void Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        _renderHandle.Render(Render);
        return Task.CompletedTask;
    }

    private void Render(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "data-message", Message);
        builder.AddContent(2, Message);
        builder.CloseElement();
    }
}
