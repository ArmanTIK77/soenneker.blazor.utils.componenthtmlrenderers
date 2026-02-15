using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.Blazor.Utils.ComponentHtmlRenderers.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Blazor.Utils.ComponentHtmlRenderers.Tests;

[Collection("Collection")]
public sealed class ComponentHtmlRendererTests : FixturedUnitTest
{
    private readonly IComponentHtmlRenderer _util;

    public ComponentHtmlRendererTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IComponentHtmlRenderer>(true);
    }

    [Fact]
    public async Task RenderToHtml_generic_renders_simple_component()
    {
        string html = await _util.RenderToHtml<SimpleTestComponent>();

        html.Should().Contain("test-component").And.Contain("Hello from test component");
    }

    [Fact]
    public async Task RenderToHtml_Type_overload_renders_simple_component()
    {
        string html = await _util.RenderToHtml(typeof(SimpleTestComponent));

        html.Should().Contain("test-component").And.Contain("Hello from test component");
    }

    [Fact]
    public async Task RenderToHtml_with_null_parameters_uses_empty_params()
    {
        string html = await _util.RenderToHtml<SimpleTestComponent>(null);

        html.Should().Contain("Hello from test component");
    }

    [Fact]
    public async Task RenderToHtml_with_empty_dictionary_renders()
    {
        var parameters = new Dictionary<string, object?>();
        string html = await _util.RenderToHtml<SimpleTestComponent>(parameters);

        html.Should().Contain("Hello from test component");
    }

    [Fact]
    public async Task RenderToHtml_with_parameters_passes_to_component()
    {
        var parameters = new Dictionary<string, object?>
        {
            ["Message"] = "Custom message"
        };
        string html = await _util.RenderToHtml<ParameterizedTestComponent>(parameters);

        html.Should().Contain("Custom message").And.Contain("data-message");
    }

    [Fact]
    public async Task RenderToHtml_with_buildParameters_passes_to_component()
    {
        string html = await _util.RenderToHtml(typeof(ParameterizedTestComponent), dict =>
        {
            dict["Message"] = "Built by delegate";
        });

        html.Should().Contain("Built by delegate");
    }

    [Fact]
    public async Task RenderToHtml_Type_with_null_componentType_throws()
    {
        Func<Task> act = () => _util.RenderToHtml((Type)null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task RenderToHtml_with_buildParameters_null_componentType_throws()
    {
        Func<Task> act = () => _util.RenderToHtml((Type)null!, _ => { });
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task RenderToHtml_with_buildParameters_null_buildParameters_throws()
    {
        Func<Task> act = () => _util.RenderToHtml(typeof(SimpleTestComponent), (Action<Dictionary<string, object?>>)null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DisposeAsync_does_not_throw()
    {
        var renderer = Resolve<IComponentHtmlRenderer>(true);
        await renderer.DisposeAsync();
    }

    /// <summary>
    /// Ensures the renderer does not produce malformed button HTML with corrupted classes
    /// (e.g. &amp; instead of & in arbitrary selectors, jumbled/broken class attribute).
    /// </summary>
    [Fact]
    public async Task RenderToHtml_does_not_generate_malformed_button_with_corrupted_class()
    {
        const string malformedButton =
            "<button class=\"[&amp;_svg:not([class*= aria-invalid:border-destructive aria-invalid:ring-[3px] aria-invalid:ring-destructive/20 b-1 bg-clip-padding border border-transparent dark:aria-invalid:border-destructive/50 dark:aria-invalid:ring-destructive/40 focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50 font-medium q-button rounded-lg text-sm\"></button>";

        string html = await _util.RenderToHtml<ButtonTestComponent>();

        html.Should().NotContain(malformedButton);
        html.Should().NotContain("[&amp;_svg"); // Key corruption: & must not become &amp; in class
    }
}
