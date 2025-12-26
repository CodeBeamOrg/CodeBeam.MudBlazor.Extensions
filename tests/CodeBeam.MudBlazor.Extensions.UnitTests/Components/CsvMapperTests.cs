
using AwesomeAssertions;
using Bunit;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class CsvMapperTests : BunitTest
    {
        [Test]
        public void MudCsvMapper_Should_Render_With_Minimal_Parameters()
        {
            var expectedHeaders = new List<MudExpectedHeader>
            {
                new("Id", required: true),
                new("Name"),
            };

            var cut = Context.Render<MudCsvMapper>(parameters => parameters.Add(p => p.ExpectedHeaders, expectedHeaders));

            cut.Markup.Should().NotBeNullOrWhiteSpace();
            cut.FindAll("input").Count.Should().BeGreaterThan(0);
        }
    }
}
