using System;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Blazor.Utils.ComponentHtmlRenderers.Tests;

public sealed class ComponentHtmlRendererConstructorTests
{
    [Fact]
    public void Constructor_with_null_serviceProvider_throws()
    {
        Action act = () => new ComponentHtmlRenderer((IServiceProvider)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_with_null_configureServices_throws()
    {
        Action act = () => new ComponentHtmlRenderer((Action<IServiceCollection>)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_with_configureServices_and_null_buildServiceProvider_throws()
    {
        Action act = () => new ComponentHtmlRenderer(_ => { }, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task Constructor_with_configureServices_renders_component()
    {
        await using var renderer = new ComponentHtmlRenderer(services =>
        {
            services.AddLogging();
        });

        string html = await renderer.RenderToHtml<SimpleTestComponent>();
        html.Should().Contain("Hello from test component");
    }
}
