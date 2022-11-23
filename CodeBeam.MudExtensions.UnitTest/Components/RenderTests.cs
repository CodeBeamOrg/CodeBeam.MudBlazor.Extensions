using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using CodeBeam.MudExtensions.UnitTests.Components;
using ComponentViewer.Docs.Pages.Components;
using FluentAssertions;
using MudBlazor;
using MudExtensions;

namespace CodeBeam.MudExtensions.UnitTest.Components
{
    [TestFixture]
    public class RenderTests : BunitTest
    {
        [Test]
        public void ApiPageRenderTest()
        {
            var comp = Context.RenderComponent<ApiPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void AnimatePageRenderTest()
        {
            var comp = Context.RenderComponent<AnimatePage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void WheelDatePickerPageRenderTest()
        {
            var comp = Context.RenderComponent<DateWheelPickerPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void SpeedDialPageRenderTest()
        {
            var comp = Context.RenderComponent<SpeedDialPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void StepperPageRenderTest()
        {
            var comp = Context.RenderComponent<StepperPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }
    }
}
