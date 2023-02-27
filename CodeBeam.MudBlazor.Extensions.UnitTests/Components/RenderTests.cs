using Bunit;
using MudExtensions.UnitTests.Components;
using ComponentViewer.Docs.Pages.Components;
using FluentAssertions;
using MudBlazor;
using MudExtensions;

namespace MudExtensions.UnitTests.Components
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

        [Test]
        public void ListExtendedPageRenderTest()
        {
            var comp = Context.RenderComponent<ListExtendedPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void SelectExtendedPageRenderTest()
        {
            var comp = Context.RenderComponent<SelectExtendedPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void TextFieldExtendedPageRenderTest()
        {
            var comp = Context.RenderComponent<TextFieldExtendedPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void TransferListPageRenderTest()
        {
            var comp = Context.RenderComponent<TransferListPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }
    }
}
