using MudExtensions.Docs.Examples;
using AwesomeAssertions;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class RenderExampleTests : BunitTest
    {
        [Test]
        public void AnimateExampleRenderTest()
        {
            var comp = Context.Render<AnimateExample1>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void AnimateExample2RenderTest()
        {
            var comp = Context.Render<AnimateExample2>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void DateWheelPickerExampleRenderTest()
        {
            var comp = Context.Render<DateWheelPickerExample1>();
            comp.Markup.Should().NotBeNullOrEmpty();
        }
    }
}
