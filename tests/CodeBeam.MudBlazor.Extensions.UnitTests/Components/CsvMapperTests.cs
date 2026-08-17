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
                new("A", MudMapper.SourcePoolZoneIdentifier),
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

        [Test]
        public void ResetMapping_Should_Clear_Imported_State_And_Reset_Header_Counts()
        {
            var cut = Context.Render<MudCsvMapper>();
            var expectedHeaders = new List<MudExpectedHeader> { new("Age") };
            expectedHeaders[0].MatchedFieldCount = 2;

            cut.Instance.ExpectedHeaders = expectedHeaders;
            SetPrivateMember(cut.Instance, "_sourceItems", new List<MudMapperItem> { new("Name", "Name") });
            SetPrivateMember(cut.Instance, "CsvContent", new List<IDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["Name"] = "Test" }
            });
            cut.Instance.FileContentByte = new byte[] { 1, 2, 3 };
            cut.Instance.CsvMapping["Age"] = "Name";

            InvokePrivate(cut.Instance, "ResetMapping");

            cut.Instance.CsvMapping.Should().BeEmpty();
            cut.Instance.FileContentByte.Should().BeNull();
            expectedHeaders[0].MatchedFieldCount.Should().Be(0);

            var sourceItemsField = cut.Instance.GetType().GetField("_sourceItems", BindingFlags.NonPublic | BindingFlags.Instance)!;
            sourceItemsField.GetValue(cut.Instance).Should().BeOfType<List<MudMapperItem>>()
                .Which.Should().BeEmpty();

            var csvContentField = cut.Instance.GetType().GetField("CsvContent", BindingFlags.NonPublic | BindingFlags.Instance)!;
            csvContentField.GetValue(cut.Instance).Should().BeNull();
        }

        [Test]
        public void UpdateHeadersWithMappedFields_Should_Remap_And_Record_Mapping()
        {
            var cut = Context.Render<MudCsvMapper>();
            cut.Instance.ExpectedHeaders = new List<MudExpectedHeader> { new("Name") };

            var csvContent = new List<IDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["OriginalName"] = "Jane" }
            };

            SetPrivateMember(cut.Instance, "CsvContent", csvContent);
            SetPrivateMember(cut.Instance, "_sourceItems", new List<MudMapperItem>
            {
                new("OriginalName", "Name")
            });

            InvokePrivate(cut.Instance, "UpdateHeadersWithMappedFields");

            cut.Instance.CsvMapping["Name"].Should().Be("OriginalName");
            csvContent[0].ContainsKey("Name").Should().BeTrue();
            csvContent[0].ContainsKey("OriginalName").Should().BeFalse();
            csvContent[0]["Name"].Should().Be("Jane");
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

        private static async Task InvokePrivateAsync(object instance, string methodName)
        {
            var method = instance.GetType()
                .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            method.Should().NotBeNull();
            var result = method!.Invoke(instance, null);
            if (result is Task task)
            {
                await task;
            }
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
