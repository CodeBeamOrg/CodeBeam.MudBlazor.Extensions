using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using CodeBeam.MudExtensions.UnitTests.Components;
using FluentAssertions;
using MudBlazor;
using MudExtensions;

namespace CodeBeam.MudExtensions.UnitTest.Components
{
    [TestFixture]
    public class StepperTests : BunitTest
    {
        [Test]
        public void StepperRenderTest()
        {
            var comp = Context.RenderComponent<MudStepper>();
            comp.Instance.Steps.Count.Should().Be(0);
        }
    }
}
