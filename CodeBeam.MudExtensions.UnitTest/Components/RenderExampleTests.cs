using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using CodeBeam.MudExtensions.UnitTests.Components;
using ComponentViewer.Docs.Pages.Components;
using ComponentViewer.Docs.Pages.Examples;
using FluentAssertions;
using MudBlazor;
using MudExtensions;

namespace CodeBeam.MudExtensions.UnitTest.Components
{
    [TestFixture]
    public class RenderExampleTests : BunitTest
    {
        [Test]
        public void AnimateExampleRenderTest()
        {
            var comp = Context.RenderComponent<AnimateExample1>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void AnimateExample2RenderTest()
        {
            var comp = Context.RenderComponent<AnimateExample2>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void DateWheelPickerExampleRenderTest()
        {
            var comp = Context.RenderComponent<DateWheelPickerExample1>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }
    }
}
