using AwesomeAssertions;
using Bunit;
using System.Reflection;

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

        [Test]
        public void MudCsvMapper_Should_Render_With_AllowCreateExpectedHeaders()
        {
            var cut = Context.Render<MudCsvMapper>(parameters => parameters
                .Add(p => p.AllowCreateExpectedHeaders, true)
                .Add(p => p.ExpectedHeaders, new List<MudExpectedHeader>())
            );

            cut.Markup.Should().Contain("Create Header");
        }

        [Test]
        public void CsvHeaders_Should_Match_ExpectedHeaders_Exactly()
        {
            // Arrange
            var expectedHeaders = new List<MudExpectedHeader>
            {
                new("Id", required: true),
                new("Name", required: false)
            };

            var cut = Context.Render<MudCsvMapper>(p => p
                .Add(x => x.ExpectedHeaders, expectedHeaders)
            );

            var csvContent = new List<IDictionary<string, object?>>
            {
                new Dictionary<string, object?>
                {
                    ["Id"] = 1,
                    ["Name"] = "Test"
                }
            };

            cut.Instance.GetType()
                .GetField("CsvContent", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(cut.Instance, csvContent);

            InvokePrivate(cut.Instance, "MatchSourceItemsWithExpectedHeaders");

            expectedHeaders[0].MatchedFieldCount.Should().Be(1);
            expectedHeaders[1].MatchedFieldCount.Should().Be(1);
        }

        [Test]
        public void CsvHeaders_Should_Match_ExpectedHeaders_Aliases()
        {
            // Arrange
            var expectedHeaders = new List<MudExpectedHeader>
            {
                new("Id", required: true, aliases: ["Identifier"]),
                new("Name", required: false, aliases: ["FullName"])
            };

            var cut = Context.Render<MudCsvMapper>(p => p
                .Add(x => x.ExpectedHeaders, expectedHeaders)
            );

            var csvContent = new List<IDictionary<string, object?>>
            {
                new Dictionary<string, object?>
                {
                    ["Identifier"] = 1,
                    ["FullName"] = "Test"
                }
            };

            cut.Instance.GetType()
                .GetField("CsvContent", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(cut.Instance, csvContent);

            InvokePrivate(cut.Instance, "MatchSourceItemsWithExpectedHeaders");

            expectedHeaders[0].MatchedFieldCount.Should().Be(1);
            expectedHeaders[1].MatchedFieldCount.Should().Be(1);
        }

        [Test]
        public void Normalize_Should_Lowercase_And_Remove_Spaces_When_Enabled()
        {
            var cut = Context.Render<MudCsvMapper>(p => p
                .Add(x => x.NormalizeHeaders, true)
            );

            var result = cut.Instance.GetType()
                .GetMethod("Normalize", BindingFlags.NonPublic | BindingFlags.Instance)!
                .Invoke(cut.Instance, new object[] { "My Header \"Name\"" });

            result.Should().Be("myheadername");
        }

        [Test]
        public void AddDefaultValues_Should_Add_Confirmed_Defaults()
        {
            var expectedHeaders = new List<MudExpectedHeader>
            {
                new("Age", required: true, allowDefaultValue: true)
            };

            var cut = Context.Render<MudCsvMapper>(p => p
                .Add(x => x.ExpectedHeaders, expectedHeaders)
            );

            var csvContent = new List<IDictionary<string, object?>>
            {
                new Dictionary<string, object?>()
            };

            SetPrivateMember(cut.Instance, "CsvContent", csvContent);

            var defaults = new Dictionary<string, ConfirmedDefaultValue>
            {
                ["Age"] = new ConfirmedDefaultValue
                {
                    Confirmed = true,
                    DefaultValue = "18"
                }
            };

            InvokePrivateWithArgs(cut.Instance, "AddDefaultValues",
                new[] { typeof(IReadOnlyDictionary<string, ConfirmedDefaultValue>) },
                new object[] { (IReadOnlyDictionary<string, ConfirmedDefaultValue>)defaults });

            csvContent[0].ContainsKey("Age").Should().BeTrue();
            csvContent[0]["Age"].Should().Be("18");
        }


        [Test]
        public void RemoveUnmappedData_Should_Remove_Unmapped_Fields()
        {
            var cut = Context.Render<MudCsvMapper>();

            var headers = new List<MudMapperItem>
            {
                new("A", "Source"),
                new("B", "Mapped")
            };

            SetPrivateMember(cut.Instance, "_sourceItems", headers);

            var csvContent = new List<IDictionary<string, object?>>
            {
                new Dictionary<string, object?>
                {
                    ["A"] = 1,
                    ["B"] = 2
                }
            };

            SetPrivateMember(cut.Instance, "CsvContent", csvContent);

            InvokePrivate(cut.Instance, "RemoveUnmappedData");

            csvContent[0].ContainsKey("A").Should().BeFalse();
            csvContent[0].ContainsKey("B").Should().BeTrue();
        }

        [Test]
        public void RemoveUnmappedData_Should_Not_Treat_Target_Header_Name_Source_As_Unmapped()
        {
            var cut = Context.Render<MudCsvMapper>();

            var headers = new List<MudMapperItem>
            {
                new("A", "Source"),
                new("B", MudMapper.SourcePoolZoneIdentifier)
            };

            SetPrivateMember(cut.Instance, "_sourceItems", headers);

            var csvContent = new List<IDictionary<string, object?>>
            {
                new Dictionary<string, object?>
                {
                    ["A"] = 1,
                    ["B"] = 2
                }
            };

            SetPrivateMember(cut.Instance, "CsvContent", csvContent);

            InvokePrivate(cut.Instance, "RemoveUnmappedData");

            csvContent[0].ContainsKey("A").Should().BeTrue();
            csvContent[0].ContainsKey("B").Should().BeFalse();
        }

        private static void InvokePrivate(object instance, string methodName)
        {
            var method = instance.GetType()
                .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            method.Should().NotBeNull();
            method!.Invoke(instance, null);
        }

        private static void InvokePrivateWithArgs(object instance, string methodName, Type[] paramTypes, object[] args)
        {
            var method = instance.GetType()
                .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, paramTypes, null);

            method.Should().NotBeNull($"method '{methodName}' not found");
            method!.Invoke(instance, args);
        }

        private static void SetPrivateMember(object instance, string name, object value)
        {
            var type = instance.GetType();

            var prop = type.GetProperty(name,
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (prop != null)
            {
                prop.SetValue(instance, value);
                return;
            }

            var field = type.GetField(name,
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(instance, value);
                return;
            }

            Assert.Fail($"Private member '{name}' not found on {type.Name}");
        }

    }
}
