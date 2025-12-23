using AwesomeAssertions;
using MudExtensions.Docs.Pages;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class RenderTests : BunitTest
    {
        [Test]
        public void ApiPageRenderTest()
        {
            var comp = Context.Render<ApiPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void AnimatePageRenderTest()
        {
            var comp = Context.Render<AnimatePage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ComboBoxPageRenderTest()
        {
            var comp = Context.Render<ComboBoxPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void WheelDatePickerPageRenderTest()
        {
            var comp = Context.Render<DateWheelPickerPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void SpeedDialPageRenderTest()
        {
            var comp = Context.Render<SpeedDialPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void SplitterPageRenderTest()
        {
            var comp = Context.Render<SplitterPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void StepperPageRenderTest()
        {
            var comp = Context.Render<StepperExtendedPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ListExtendedPageRenderTest()
        {
            var comp = Context.Render<ListExtendedPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void SelectExtendedPageRenderTest()
        {
            var comp = Context.Render<SelectExtendedPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void TextFieldExtendedPageRenderTest()
        {
            var comp = Context.Render<TextFieldExtendedPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void TransferListPageRenderTest()
        {
            var comp = Context.Render<TransferListPage>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }
    }
}
