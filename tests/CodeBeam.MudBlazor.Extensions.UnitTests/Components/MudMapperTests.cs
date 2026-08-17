using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudExtensions.Utilities;
using System.Reflection;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class MudMapperTests : BunitTest
    {
        // -------------------------------------------------------------------------
        // Rendering
        // -------------------------------------------------------------------------

        [Test]
        public void MudMapper_Should_Render_With_Minimal_Parameters()
        {
            var cut = Context.Render<MudMapper>();

            cut.Markup.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void MudMapper_Should_Render_TargetHeader_Names()
        {
            var headers = new List<MudExpectedHeader> { new("FirstName"), new("LastName") };

            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));

            cut.Markup.Should().Contain("FirstName");
            cut.Markup.Should().Contain("LastName");
        }

        [Test]
        public void MudMapper_Should_Show_SourceItems_Zone_When_SourceItems_Provided()
        {
            var sourceItems = new List<MudMapperItem> { new("ColA"), new("ColB") };

            var cut = Context.Render<MudMapper>(p => p.Add(x => x.SourceItems, sourceItems));

            cut.Markup.Should().Contain("Source Items");
        }

        [Test]
        public void MudMapper_Should_Not_Show_SourceItems_Zone_When_No_SourceItems()
        {
            var cut = Context.Render<MudMapper>();

            cut.Markup.Should().NotContain("Source Items");
        }

        [Test]
        public void MudMapper_Should_Show_CreateHeader_Button_When_AllowCreateTargetHeaders_True()
        {
            var cut = Context.Render<MudMapper>(p => p.Add(x => x.AllowCreateTargetHeaders, true));

            cut.Markup.Should().Contain("Create Header");
        }

        [Test]
        public void MudMapper_Should_Not_Show_CreateHeader_Button_By_Default()
        {
            var cut = Context.Render<MudMapper>();

            cut.Markup.Should().NotContain("Create Header");
        }

        [Test]
        public void MudMapper_Should_Show_IncludeUnmappedData_Switch_When_Enabled()
        {
            var cut = Context.Render<MudMapper>(p => p.Add(x => x.ShowIncludeUnmappedData, true));

            cut.Markup.Should().Contain("Include unmapped data");
        }

        [Test]
        public void MudMapper_Should_Not_Show_IncludeUnmappedData_Switch_By_Default()
        {
            var cut = Context.Render<MudMapper>();

            cut.Markup.Should().NotContain("Include unmapped data");
        }

        [Test]
        public void MudMapper_Should_Use_ConfirmLabel_Parameter()
        {
            var cut = Context.Render<MudMapper>(p => p.Add(x => x.ConfirmLabel, "Import CSV"));

            cut.Markup.Should().Contain("Import CSV");
        }

        [Test]
        public void MudMapper_Should_Fall_Back_To_LocalizedStrings_When_No_ConfirmLabel()
        {
            var strings = new MudMapperLocalizedStrings { Confirm = "Apply Mapping" };

            var cut = Context.Render<MudMapper>(p => p.Add(x => x.LocalizedStrings, strings));

            cut.Markup.Should().Contain("Apply Mapping");
        }

        [Test]
        public void MudMapper_Should_Mark_Required_Headers_With_Asterisk()
        {
            var headers = new List<MudExpectedHeader> { new("Id", required: true), new("Name") };

            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));

            cut.Markup.Should().Contain("*");
        }

        [Test]
        public void MudMapper_Should_Show_DefineHeaders_Text_When_No_TargetHeaders()
        {
            var strings = new MudMapperLocalizedStrings { DefineHeaders = "No headers defined yet" };

            var cut = Context.Render<MudMapper>(p => p.Add(x => x.LocalizedStrings, strings));

            cut.Markup.Should().Contain("No headers defined yet");
        }

        // -------------------------------------------------------------------------
        // Validity — CheckValid
        // -------------------------------------------------------------------------

        [Test]
        public void CheckValid_Should_Be_False_When_Required_Header_Not_Mapped()
        {
            var headers = new List<MudExpectedHeader> { new("Id", required: true) };
            var sourceItems = new List<MudMapperItem> { new("col1") }; // MappedZone = "Source"

            var cut = Context.Render<MudMapper>(p => p
                .Add(x => x.TargetHeaders, headers)
                .Add(x => x.SourceItems, sourceItems));

            InvokePrivate(cut.Instance, "CheckValid");

            cut.Instance.IsValid.Should().BeFalse();
        }

        [Test]
        public void CheckValid_Should_Be_True_When_Required_Header_Is_Mapped()
        {
            var headers = new List<MudExpectedHeader> { new("Id", required: true) };
            var sourceItems = new List<MudMapperItem> { new("col1", "Id") }; // already mapped
            headers[0].MatchedFieldCount = 1;

            var cut = Context.Render<MudMapper>(p => p
                .Add(x => x.TargetHeaders, headers)
                .Add(x => x.SourceItems, sourceItems));

            InvokePrivate(cut.Instance, "CheckValid");

            cut.Instance.IsValid.Should().BeTrue();
        }

        [Test]
        public void CheckValid_Should_Be_True_When_There_Are_No_Required_Headers()
        {
            var headers = new List<MudExpectedHeader> { new("Name") }; // optional only

            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));

            InvokePrivate(cut.Instance, "CheckValid");

            cut.Instance.IsValid.Should().BeTrue();
        }

        [Test]
        public void CheckValid_Should_Be_True_When_Required_Header_Has_Confirmed_Default_Value()
        {
            var headers = new List<MudExpectedHeader> { new("Age", required: true, allowDefaultValue: true) };

            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));

            var defaults = new Dictionary<string, ConfirmedDefaultValue>
            {
                ["Age"] = new() { DefaultValue = "18", Confirmed = true }
            };
            SetPrivateMember(cut.Instance, "_defaultValueHeaders", defaults);

            InvokePrivate(cut.Instance, "CheckValid");

            cut.Instance.IsValid.Should().BeTrue();
        }

        [Test]
        public void CheckValid_Should_Be_False_When_Required_Header_Has_Unconfirmed_Default_Value()
        {
            var headers = new List<MudExpectedHeader> { new("Age", required: true, allowDefaultValue: true) };

            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));

            var defaults = new Dictionary<string, ConfirmedDefaultValue>
            {
                ["Age"] = new() { DefaultValue = "18", Confirmed = false }
            };
            SetPrivateMember(cut.Instance, "_defaultValueHeaders", defaults);

            InvokePrivate(cut.Instance, "CheckValid");

            cut.Instance.IsValid.Should().BeFalse();
        }

        // -------------------------------------------------------------------------
        // OnDrop
        // -------------------------------------------------------------------------

        [Test]
        public void OnDrop_Should_Update_Item_MappedZone_To_Target()
        {
            var headers = new List<MudExpectedHeader> { new("Name") };
            var sourceItems = new List<MudMapperItem> { new("col1") };

            var cut = Context.Render<MudMapper>(p => p
                .Add(x => x.TargetHeaders, headers)
                .Add(x => x.SourceItems, sourceItems));

            SimulateDrop(cut.Instance, sourceItems[0], "Name");

            sourceItems[0].MappedZone.Should().Be("Name");
        }

        [Test]
        public void OnDrop_Should_Increment_MatchedFieldCount_On_Target_Header()
        {
            var headers = new List<MudExpectedHeader> { new("Name") };
            var sourceItems = new List<MudMapperItem> { new("col1") };

            var cut = Context.Render<MudMapper>(p => p
                .Add(x => x.TargetHeaders, headers)
                .Add(x => x.SourceItems, sourceItems));

            SimulateDrop(cut.Instance, sourceItems[0], "Name");

            headers[0].MatchedFieldCount.Should().Be(1);
        }

        [Test]
        public void OnDrop_Should_Decrement_Old_And_Increment_New_MatchedFieldCount()
        {
            var first = new MudExpectedHeader("First");
            var last = new MudExpectedHeader("Last");
            var headers = new List<MudExpectedHeader> { first, last };
            var sourceItems = new List<MudMapperItem> { new("col1", "First") }; // starts in "First"
            first.MatchedFieldCount = 1;

            var cut = Context.Render<MudMapper>(p => p
                .Add(x => x.TargetHeaders, headers)
                .Add(x => x.SourceItems, sourceItems));

            SimulateDrop(cut.Instance, sourceItems[0], "Last");

            first.MatchedFieldCount.Should().Be(0);
            last.MatchedFieldCount.Should().Be(1);
        }

        [Test]
        public void OnDrop_To_Source_Should_Decrement_MatchedFieldCount()
        {
            var headers = new List<MudExpectedHeader> { new("Name") };
            var sourceItems = new List<MudMapperItem> { new("col1", "Name") }; // currently mapped
            headers[0].MatchedFieldCount = 1;

            var cut = Context.Render<MudMapper>(p => p
                .Add(x => x.TargetHeaders, headers)
                .Add(x => x.SourceItems, sourceItems));

            SimulateDrop(cut.Instance, sourceItems[0], "Source");

            headers[0].MatchedFieldCount.Should().Be(0);
            sourceItems[0].MappedZone.Should().Be("Source");
        }

        [Test]
        public void OnDrop_Mapping_Required_Header_Should_Make_IsValid_True()
        {
            var headers = new List<MudExpectedHeader> { new("Id", required: true) };
            var sourceItems = new List<MudMapperItem> { new("col1") };

            var cut = Context.Render<MudMapper>(p => p
                .Add(x => x.TargetHeaders, headers)
                .Add(x => x.SourceItems, sourceItems));

            SimulateDrop(cut.Instance, sourceItems[0], "Id");

            cut.Instance.IsValid.Should().BeTrue();
        }

        [Test]
        public void OnDrop_Unmapping_Required_Header_Should_Make_IsValid_False()
        {
            var headers = new List<MudExpectedHeader> { new("Id", required: true) };
            var sourceItems = new List<MudMapperItem> { new("col1", "Id") };
            headers[0].MatchedFieldCount = 1;

            var cut = Context.Render<MudMapper>(p => p
                .Add(x => x.TargetHeaders, headers)
                .Add(x => x.SourceItems, sourceItems));

            SimulateDrop(cut.Instance, sourceItems[0], "Source");

            cut.Instance.IsValid.Should().BeFalse();
        }

        // -------------------------------------------------------------------------
        // Confirm / Reset
        // -------------------------------------------------------------------------

        [Test]
        public async Task HandleConfirm_Should_Set_IsConfirmed_True()
        {
            var cut = Context.Render<MudMapper>();
            SetPrivateMember(cut.Instance, "_valid", true);

            await cut.InvokeAsync(() => InvokePrivateAsync(cut.Instance, "HandleConfirm"));

            cut.Instance.IsConfirmed.Should().BeTrue();
        }

        [Test]
        public async Task HandleConfirm_Should_Fire_OnConfirmed_Callback()
        {
            bool fired = false;
            var cut = Context.Render<MudMapper>(p => p
                .Add(x => x.OnConfirmed, EventCallback.Factory.Create(this, () => fired = true)));
            SetPrivateMember(cut.Instance, "_valid", true);

            await cut.InvokeAsync(() => InvokePrivateAsync(cut.Instance, "HandleConfirm"));

            fired.Should().BeTrue();
        }

        [Test]
        public async Task HandleReset_Should_Clear_IsConfirmed()
        {
            var cut = Context.Render<MudMapper>();
            SetPrivateMember(cut.Instance, "_valid", true);
            await cut.InvokeAsync(() => InvokePrivateAsync(cut.Instance, "HandleConfirm"));
            cut.Instance.IsConfirmed.Should().BeTrue();

            await cut.InvokeAsync(() => InvokePrivateAsync(cut.Instance, "HandleReset"));

            cut.Instance.IsConfirmed.Should().BeFalse();
        }

        [Test]
        public async Task HandleReset_Should_Fire_OnReset_Callback()
        {
            bool fired = false;
            var cut = Context.Render<MudMapper>(p => p
                .Add(x => x.OnReset, EventCallback.Factory.Create(this, () => fired = true)));

            await cut.InvokeAsync(() => InvokePrivateAsync(cut.Instance, "HandleReset"));

            fired.Should().BeTrue();
        }

        // -------------------------------------------------------------------------
        // ResetMapping
        // -------------------------------------------------------------------------

        [Test]
        public void ResetMapping_Should_Clear_IsConfirmed()
        {
            var cut = Context.Render<MudMapper>();
            SetPrivateMember(cut.Instance, "_valid", true);

            cut.Instance.ResetMapping();

            cut.Instance.IsConfirmed.Should().BeFalse();
        }

        [Test]
        public void ResetMapping_Should_Clear_IsValid()
        {
            var headers = new List<MudExpectedHeader> { new("Id", required: true) };
            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));
            SetPrivateMember(cut.Instance, "_valid", true);

            cut.Instance.ResetMapping();

            cut.Instance.IsValid.Should().BeFalse();
        }

        [Test]
        public void ResetMapping_Should_Reset_MatchedFieldCounts_On_TargetHeaders()
        {
            var headers = new List<MudExpectedHeader> { new("Id"), new("Name") };
            headers[0].MatchedFieldCount = 2;
            headers[1].MatchedFieldCount = 1;

            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));

            cut.Instance.ResetMapping();

            headers[0].MatchedFieldCount.Should().Be(0);
            headers[1].MatchedFieldCount.Should().Be(0);
        }

        [Test]
        public void ResetMapping_Should_Clear_IncludeUnmappedData()
        {
            var cut = Context.Render<MudMapper>(p => p.Add(x => x.ShowIncludeUnmappedData, true));
            cut.Instance.ResetMapping();

            cut.Instance.IncludeUnmappedData.Should().BeFalse();
        }

        [Test]
        public void ResetMapping_Should_Reinitialize_DefaultValues()
        {
            var headers = new List<MudExpectedHeader>
            {
                new("Age", required: true, allowDefaultValue: true)
            };
            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));

            // Simulate a confirmed default
            var confirmedDefaults = new Dictionary<string, ConfirmedDefaultValue>
            {
                ["Age"] = new() { DefaultValue = "99", Confirmed = true }
            };
            SetPrivateMember(cut.Instance, "_defaultValueHeaders", confirmedDefaults);

            cut.Instance.ResetMapping();

            cut.Instance.DefaultValues.Should().NotBeNull();
            cut.Instance.DefaultValues!["Age"].Confirmed.Should().BeFalse();
            cut.Instance.DefaultValues!["Age"].DefaultValue.Should().Be("");
        }

        // -------------------------------------------------------------------------
        // Default values
        // -------------------------------------------------------------------------

        [Test]
        public void DefaultValues_Should_Be_Initialized_For_AllowDefaultValue_Headers()
        {
            var headers = new List<MudExpectedHeader>
            {
                new("Age", required: true, allowDefaultValue: true),
                new("Name") // no default value allowed
            };

            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));

            cut.Instance.DefaultValues.Should().NotBeNull();
            cut.Instance.DefaultValues!.ContainsKey("Age").Should().BeTrue();
            cut.Instance.DefaultValues!.ContainsKey("Name").Should().BeFalse();
        }

        [Test]
        public void DefaultValues_Should_Be_Empty_When_No_AllowDefaultValue_Headers()
        {
            var headers = new List<MudExpectedHeader> { new("Id"), new("Name") };

            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));

            cut.Instance.DefaultValues.Should().NotBeNull();
            cut.Instance.DefaultValues!.Should().BeEmpty();
        }

        [Test]
        public void SubmitDefaultValue_Should_Confirm_When_Value_Is_Provided()
        {
            var headers = new List<MudExpectedHeader> { new("Age", required: true, allowDefaultValue: true) };
            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));

            var defaults = new Dictionary<string, ConfirmedDefaultValue>
            {
                ["Age"] = new() { DefaultValue = "25", Confirmed = false }
            };
            SetPrivateMember(cut.Instance, "_defaultValueHeaders", defaults);

            InvokePrivateWithArgs(cut.Instance, "SubmitDefaultValue",
                new[] { typeof(string) },
                new object[] { "Age" });

            defaults["Age"].Confirmed.Should().BeTrue();
        }

        [Test]
        public void SubmitDefaultValue_Should_Not_Confirm_When_Value_Is_Empty()
        {
            var headers = new List<MudExpectedHeader> { new("Age", required: true, allowDefaultValue: true) };
            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));

            var defaults = new Dictionary<string, ConfirmedDefaultValue>
            {
                ["Age"] = new() { DefaultValue = "", Confirmed = false }
            };
            SetPrivateMember(cut.Instance, "_defaultValueHeaders", defaults);

            InvokePrivateWithArgs(cut.Instance, "SubmitDefaultValue",
                new[] { typeof(string) },
                new object[] { "Age" });

            defaults["Age"].Confirmed.Should().BeFalse();
        }

        [Test]
        public void SubmitDefaultValue_Should_Toggle_Off_When_Already_Confirmed()
        {
            var headers = new List<MudExpectedHeader> { new("Age", required: true, allowDefaultValue: true) };
            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));

            var defaults = new Dictionary<string, ConfirmedDefaultValue>
            {
                ["Age"] = new() { DefaultValue = "25", Confirmed = true }
            };
            SetPrivateMember(cut.Instance, "_defaultValueHeaders", defaults);

            InvokePrivateWithArgs(cut.Instance, "SubmitDefaultValue",
                new[] { typeof(string) },
                new object[] { "Age" });

            defaults["Age"].Confirmed.Should().BeFalse();
        }

        [Test]
        public void SubmitDefaultValue_For_Required_Header_Should_Make_IsValid_True()
        {
            var headers = new List<MudExpectedHeader> { new("Age", required: true, allowDefaultValue: true) };
            var cut = Context.Render<MudMapper>(p => p.Add(x => x.TargetHeaders, headers));

            var defaults = new Dictionary<string, ConfirmedDefaultValue>
            {
                ["Age"] = new() { DefaultValue = "18", Confirmed = false }
            };
            SetPrivateMember(cut.Instance, "_defaultValueHeaders", defaults);

            InvokePrivateWithArgs(cut.Instance, "SubmitDefaultValue",
                new[] { typeof(string) },
                new object[] { "Age" });

            cut.Instance.IsValid.Should().BeTrue();
        }

        [Test]
        public void Parameters_Should_Rebuild_MatchedFieldCounts_From_Current_SourceZones()
        {
            var headers = new List<MudExpectedHeader> { new("First"), new("Last") };
            var sourceItems = new List<MudMapperItem>
            {
                new("col1", "First"),
                new("col2", "Source"),
                new("col3", "DoesNotExist")
            };

            var cut = Context.Render<MudMapper>(p => p
                .Add(x => x.TargetHeaders, headers)
                .Add(x => x.SourceItems, sourceItems));

            headers[0].MatchedFieldCount.Should().Be(1);
            headers[1].MatchedFieldCount.Should().Be(0);
            sourceItems[2].MappedZone.Should().Be(MudMapper.SourcePoolZoneIdentifier);
        }

        [Test]
        public void OnDrop_Should_Recalculate_MatchedFieldCounts_For_New_Target()
        {
            var headers = new List<MudExpectedHeader> { new("First"), new("Last") };
            var sourceItems = new List<MudMapperItem> { new("col1", MudMapper.SourcePoolZoneIdentifier) };
            var cut = Context.Render<MudMapper>(p => p
                .Add(x => x.TargetHeaders, headers)
                .Add(x => x.SourceItems, sourceItems));

            SimulateDrop(cut.Instance, sourceItems[0], "First");
            headers[0].MatchedFieldCount.Should().Be(1);
            headers[1].MatchedFieldCount.Should().Be(0);

            SimulateDrop(cut.Instance, sourceItems[0], "Last");
            headers[0].MatchedFieldCount.Should().Be(0);
            headers[1].MatchedFieldCount.Should().Be(1);
        }

        [Test]
        public void SourcePool_Items_Should_Not_Be_Treated_As_Target_Header_Name_Source()
        {
            var headers = new List<MudExpectedHeader> { new("Source") };
            var sourceItems = new List<MudMapperItem> { new("col1", MudMapper.SourcePoolZoneIdentifier) };

            var cut = Context.Render<MudMapper>(p => p
                .Add(x => x.TargetHeaders, headers)
                .Add(x => x.SourceItems, sourceItems));

            headers[0].MatchedFieldCount.Should().Be(0);
            sourceItems[0].MappedZone.Should().Be(MudMapper.SourcePoolZoneIdentifier);
            cut.Instance.IsValid.Should().BeTrue();
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        private static void SimulateDrop(MudMapper instance, MudMapperItem item, string targetZone)
        {
            var dropInfo = new MudItemDropInfo<MudMapperItem>(item, targetZone, 0);
            InvokePrivateWithArgs(instance, "OnDrop",
                new[] { typeof(MudItemDropInfo<MudMapperItem>) },
                new object[] { dropInfo });
        }

        private static void InvokePrivate(object instance, string methodName)
        {
            var method = instance.GetType()
                .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            method.Should().NotBeNull($"private method '{methodName}' should exist");
            method!.Invoke(instance, null);
        }

        private static async Task InvokePrivateAsync(object instance, string methodName)
        {
            var method = instance.GetType()
                .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            method.Should().NotBeNull($"private async method '{methodName}' should exist");
            var task = (Task)method!.Invoke(instance, null)!;
            await task;
        }

        private static void InvokePrivateWithArgs(object instance, string methodName, Type[] paramTypes, object[] args)
        {
            var method = instance.GetType()
                .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, paramTypes, null);

            method.Should().NotBeNull($"method '{methodName}' with specified parameter types should exist");
            method!.Invoke(instance, args);
        }

        private static void SetPrivateMember(object instance, string name, object value)
        {
            var type = instance.GetType();

            var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(instance, value);
                return;
            }

            var prop = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null)
            {
                prop.SetValue(instance, value);
                return;
            }

            Assert.Fail($"Private member '{name}' not found on {type.Name}");
        }
    }
}
