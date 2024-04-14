using MudExtensions.Docs.Examples;
using FluentAssertions;

namespace MudExtensions.UnitTests.Components
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
