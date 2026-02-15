using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Soenneker.Blazor.Utils.ComponentHtmlRenderers.Tests;

/// <summary>
/// Component that renders a button with Tailwind/shadcn-style classes for testing
/// that malformed output (e.g. &amp; in class, corrupted selectors) is not generated.
/// </summary>
public sealed class ButtonTestComponent : IComponent
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
        // Correct Tailwind/shadcn button classes - uses & for arbitrary variant (must NOT become &amp; in output)
        builder.OpenElement(0, "button");
        builder.AddAttribute(1, "class",
            "[&_svg:not([class*='stroke-'])]:size-4 " +
            "aria-invalid:border-destructive aria-invalid:ring-[3px] aria-invalid:ring-destructive/20 " +
            "bg-clip-padding border border-transparent " +
            "dark:aria-invalid:border-destructive/50 dark:aria-invalid:ring-destructive/40 " +
            "focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50 " +
            "font-medium q-button rounded-lg text-sm");
        builder.CloseElement();
    }
}
