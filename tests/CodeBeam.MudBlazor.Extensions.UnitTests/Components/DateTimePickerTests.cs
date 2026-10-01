using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;

using System.Reflection;
using System.Linq;

namespace MudExtensions.UnitTests.Components;

[TestFixture]
public class DateTimePickerTests : BunitTest
{
    private IRenderedComponent<MudDateTimePicker<T>> RenderPicker<T>(
        T? value = default,
        EventCallback<T?>? valueChanged = null,
        string? format = null)
    {
        return Context.Render<MudDateTimePicker<T>>(parameters =>
        {
            parameters.Add(p => p.Value, value);

            if (valueChanged.HasValue)
                parameters.Add(p => p.ValueChanged, valueChanged.Value);

            if (format is not null)
                parameters.Add(p => p.DateFormat, format);
        });
    }

    [Test]
    public void DateTimePicker_DefaultRender_Should_Render_Input()
    {
        var comp = Context.Render<MudDateTimePicker<DateTime>>();
        comp.Find("input").Should().NotBeNull();
    }

    [Test]
    public void DateTimePicker_Should_Format_Correctly()
    {
        var comp = RenderPicker(
            value: new DateTime(2026, 5, 3, 14, 30, 0),
            format: "dd.MM.yyyy HH:mm");

        var text = comp.Instance.ConvertSetInternal(comp.Instance.Value);

        text.Should().Be("03.05.2026 14:30");
    }

    [Test]
    public void DateTimePicker_DateOnly_Should_Not_Render_Time()
    {
        var comp = Context.Render<MudDateTimePicker<DateOnly>>(p => p
            .Add(x => x.Value, new DateOnly(2026, 5, 3))
        );

        comp.Markup.Should().NotContain("mud-picker-time");
    }

    [Test]
    public void DateTimePicker_DateTimeOffset_Should_Convert_Correctly()
    {
        DateTimeOffset? value = new DateTimeOffset(2026, 5, 3, 10, 0, 0, TimeSpan.Zero);

        var comp = Context.Render<MudDateTimePicker<DateTimeOffset?>>(p => p
            .Add(x => x.Value, value)
            .Add(x => x.TimeZone, TimeZoneInfo.Utc)
        );

        var dt = comp.Instance.ToDateTime(value);

        dt.Should().Be(new DateTime(2026, 5, 3, 10, 0, 0));
    }

    [Test]
    public async Task DateTimePicker_Input_Change_Should_Update_Value()
    {
        DateTime? value = null;

        var callback = EventCallback.Factory.Create<DateTime?>(
            this,
            v => value = v);

        var comp = RenderPicker<DateTime?>(
            value: default,
            valueChanged: callback,
            format: "dd.MM.yyyy HH:mm");

        var input = comp.Find("input");

        await input.ChangeAsync(new ChangeEventArgs
        {
            Value = "03.05.2026 14:30"
        });

        value.Should().Be(new DateTime(2026, 5, 3, 14, 30, 0));
    }

    [Test]
    public void DateTimePicker_Null_Value_Should_Render_Empty()
    {
        var comp = RenderPicker<DateTime?>();
        comp.Find("input").GetAttribute("value").Should().BeNullOrEmpty();
    }

    [Test]
    public async Task DateTimePicker_DefaultDateTime_Should_Open_Current_Month()
    {
        await AssertDefaultValueOpensCurrentMonthAsync<DateTime>();
    }

    [Test]
    public async Task DateTimePicker_DefaultDateOnly_Should_Open_Current_Month()
    {
        await AssertDefaultValueOpensCurrentMonthAsync<DateOnly>();
    }

    [Test]
    public async Task DateTimePicker_DefaultDateTimeOffset_WithIstanbulTimeZone_Should_Open_Current_Month()
    {
        var istanbulTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
        await AssertDefaultValueOpensCurrentMonthAsync<DateTimeOffset>(istanbulTimeZone);
    }

    private async Task AssertDefaultValueOpensCurrentMonthAsync<T>(TimeZoneInfo? timeZone = null)
    {
        var today = DateTime.Today;
        var expectedMonth = new DateTime(today.Year, today.Month, 1);
        var comp = Context.Render<MudDateTimePicker<T>>(parameters =>
        {
            parameters.Add(p => p.Value, default(T));

            if (timeZone is not null)
                parameters.Add(p => p.TimeZone, timeZone);
        });

        await InvokePickerOpenedAsync(comp.Instance);

        comp.Instance.PickerMonth.Should().Be(expectedMonth);
    }

    private static Task InvokePickerOpenedAsync<T>(MudDateTimePicker<T> picker)
    {
        var method = typeof(MudBaseDatePickerX<T>).GetMethod(
            "OnPickerOpenedAsync",
            BindingFlags.Instance | BindingFlags.NonPublic);

        return (Task)method!.Invoke(picker, null)!;
    }

    [Test]
    public void BoundDateOrNull_DefaultNonNullable_ReturnsNull()
    {
        var comp = Context.Render<MudDateTimePicker<DateTime>>(parameters => parameters.Add(p => p.Value, default(DateTime)));
        var method = comp.Instance.GetType().GetMethod("BoundDateOrNull", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var result = (DateTime?)method.Invoke(comp.Instance, null);
        result.Should().BeNull();
    }

    [Test]
    public void BoundDateOrNull_ReturnsValue()
    {
        var date = new DateTime(2026, 5, 3);
        var comp = Context.Render<MudDateTimePicker<DateTime>>(parameters => parameters.Add(p => p.Value, date));
        var method = comp.Instance.GetType().GetMethod("BoundDateOrNull", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var result = (DateTime?)method.Invoke(comp.Instance, null);
        result.Should().Be(date);
    }

    [Test]
    public void GetPickerHeaderDate_PrefersMinDate()
    {
        var min = new DateTime(2020, 1, 1);
        var comp = Context.Render<MudDateTimePicker<DateTime?>>(parameters => parameters.Add(p => p.Value, null).Add(p => p.MinDate, min));
        var method = comp.Instance.GetType().GetMethod("GetPickerHeaderDate", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var result = (DateTime)method.Invoke(comp.Instance, new object?[] { null })!;
        result.Should().Be(min.Date);
    }

    [Test]
    public void GetPickerHeaderDate_PrefersPickerMonth()
    {
        var pm = new DateTime(2021, 6, 1);
        var comp = Context.Render<MudDateTimePicker<DateTime?>>(parameters => parameters.Add(p => p.Value, null));
        comp.Instance.PickerMonth = pm;
        var method = comp.Instance.GetType().GetMethod("GetPickerHeaderDate", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var result = (DateTime)method.Invoke(comp.Instance, new object?[] { null })!;
        result.Should().Be(pm);
    }

    [Test]
    public void GetPickerHeaderDate_FallbackToToday()
    {
        var comp = Context.Render<MudDateTimePicker<DateTime?>>(parameters => parameters.Add(p => p.Value, null));
        var method = comp.Instance.GetType().GetMethod("GetPickerHeaderDate", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var result = (DateTime)method.Invoke(comp.Instance, new object?[] { null })!;
        result.Should().Be(DateTime.Today);
    }

    [Test]
    public void GetTitleDateString_StableDuringTimeEdits()
    {
        var value = new DateTime(2026, 5, 3, 10, 0, 0);
        var comp = Context.Render<MudDateTimePicker<DateTime?>>(parameters => parameters.Add(p => p.Value, value));
        var methodTitle = comp.Instance.GetType().GetMethod("GetTitleDateString", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var before = (string)methodTitle.Invoke(comp.Instance, null)!;

        var field = comp.Instance.GetType().GetField("_workingValue", BindingFlags.Instance | BindingFlags.NonPublic)!;
        field.SetValue(comp.Instance, new DateTime(2026, 5, 3, 22, 30, 0));

        var after = (string)methodTitle.Invoke(comp.Instance, null)!;
        after.Should().Be(before);
    }

    [Test]
    public async Task ClearDoesNotChangePickerMonth()
    {
        var initial = new DateTime(2022, 5, 1);
        var comp = Context.Render<MudDateTimePicker<DateTime?>>(parameters => parameters.Add(p => p.Value, (DateTime?)initial));
        comp.Instance.PickerMonth = new DateTime(2022, 5, 1);

        var method = comp.Instance.GetType().GetMethod("SetDateAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
        await comp.InvokeAsync(async () =>
        {
            var t = (Task)method.Invoke(comp.Instance, new object?[] { null, true })!;
            await t;
        });

        comp.Instance.PickerMonth.Should().Be(initial);
    }

    [Test]
    public async Task SyncTimeFromValue_Sets_Zero_When_WorkingValueNull()
    {
        var comp = Context.Render<MudDateTimePicker<DateTime?>>(parameters => parameters.Add(p => p.Value, null));
        var wfield = comp.Instance.GetType().GetField("_workingValue", BindingFlags.Instance | BindingFlags.NonPublic)!;
        wfield.SetValue(comp.Instance, null);
        var method = comp.Instance.GetType().GetMethod("SyncTimeFromValue", BindingFlags.Instance | BindingFlags.NonPublic)!;
        await comp.InvokeAsync(() => method.Invoke(comp.Instance, null));
        var ts = comp.Instance.GetType().GetField("_timeSet", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(comp.Instance)!;
        var hour = (int)ts.GetType().GetProperty("Hour")!.GetValue(ts)!;
        var minute = (int)ts.GetType().GetProperty("Minute")!.GetValue(ts)!;
        hour.Should().Be(0);
        minute.Should().Be(0);
    }

    [Test]
    public async Task UpdateTimeAsync_Applies_TimeSet_To_WorkingValue()
    {
        var comp = Context.Render<MudDateTimePicker<DateTime?>>(parameters => parameters.Add(p => p.Value, null));
        var tsField = comp.Instance.GetType().GetField("_timeSet", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var ts = tsField.GetValue(comp.Instance)!;
        ts.GetType().GetProperty("Hour")!.SetValue(ts, 13);
        ts.GetType().GetProperty("Minute")!.SetValue(ts, 45);
        var method = comp.Instance.GetType().GetMethod("UpdateTimeAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
        await comp.InvokeAsync(async () => await (Task)method.Invoke(comp.Instance, null)!);
        var w = (DateTime?)comp.Instance.GetType().GetField("_workingValue", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(comp.Instance);
        w.Should().NotBeNull();
        w.Value.Hour.Should().Be(13);
        w.Value.Minute.Should().Be(45);
    }

    [Test]
    public async Task SetTimePart_Sets_WorkingValue_Hour_Minute()
    {
        var comp = Context.Render<MudDateTimePicker<DateTime?>>(parameters => parameters.Add(p => p.Value, null));
        var method = comp.Instance.GetType().GetMethod("SetTimePart", BindingFlags.Instance | BindingFlags.NonPublic)!;
        await comp.InvokeAsync(async () => await (Task)method.Invoke(comp.Instance, new object?[] { 9, 15 })!);
        var w = (DateTime?)comp.Instance.GetType().GetField("_workingValue", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(comp.Instance);
        w.Should().NotBeNull();
        w.Value.Hour.Should().Be(9);
        w.Value.Minute.Should().Be(15);
    }

    [Test]
    public async Task OnAmClickedAsync_And_OnPmClickedAsync_Adjust_Hour()
    {
        var comp = Context.Render<MudDateTimePicker<DateTime?>>(parameters => parameters.Add(p => p.Value, null));
        var tsField = comp.Instance.GetType().GetField("_timeSet", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var ts = tsField.GetValue(comp.Instance)!;
        ts.GetType().GetProperty("Hour")!.SetValue(ts, 13);
        var am = comp.Instance.GetType().GetMethod("OnAmClickedAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
        await comp.InvokeAsync(async () => await (Task)am.Invoke(comp.Instance, null)!);
        var hourAfterAm = (int)ts.GetType().GetProperty("Hour")!.GetValue(ts)!;
        hourAfterAm.Should().Be(1);

        ts.GetType().GetProperty("Hour")!.SetValue(ts, 1);
        var pm = comp.Instance.GetType().GetMethod("OnPmClickedAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
        await comp.InvokeAsync(async () => await (Task)pm.Invoke(comp.Instance, null)!);
        var hourAfterPm = (int)ts.GetType().GetProperty("Hour")!.GetValue(ts)!;
        hourAfterPm.Should().Be(13);
    }

    [Test]
    public async Task SelectTimeFromStick_And_OnStickClick_Work()
    {
        var comp = Context.Render<MudDateTimePicker<DateTime?>>(parameters => parameters.Add(p => p.Value, null));
        // test minutes selection
        var currentViewField = comp.Instance.GetType().GetField("CurrentView", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;
        var enumType = currentViewField.FieldType;
        var hours = Enum.Parse(enumType, "Hours");
        var minutes = Enum.Parse(enumType, "Minutes");
        currentViewField.SetValue(comp.Instance, minutes);
        await comp.InvokeAsync(async () => await comp.Instance.SelectTimeFromStick(30, true));
        var ts = comp.Instance.GetType().GetField("_timeSet", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(comp.Instance)!;
        var minute = (int)ts.GetType().GetProperty("Minute")!.GetValue(ts)!;
        minute.Should().Be(30);
        comp.Instance.PointerMoving.Should().BeTrue();

        // test hours selection and stick click behaviour
        comp.Instance.GetType().GetField("CurrentView", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!.SetValue(comp.Instance, hours);
        await comp.InvokeAsync(async () => await comp.Instance.SelectTimeFromStick(5, false));
        var hour = (int)ts.GetType().GetProperty("Hour")!.GetValue(ts)!;
        hour.Should().Be(5);

        await comp.InvokeAsync(async () => await comp.Instance.OnStickClick(5));
        var currentView = comp.Instance.GetType().GetField("CurrentView", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(comp.Instance);
        currentView.Should().Be(minutes);
    }

    [Test]
    public async Task SubmitAsync_Updates_Value()
    {
        var comp = Context.Render<MudDateTimePicker<DateTime?>>(parameters => parameters.Add(p => p.Value, null));
        var wfield = comp.Instance.GetType().GetField("_workingValue", BindingFlags.Instance | BindingFlags.NonPublic)!;
        wfield.SetValue(comp.Instance, new DateTime(2026, 6, 2, 11, 11, 0));
        var method = comp.Instance.GetType().GetMethod("SubmitAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
        await comp.InvokeAsync(async () => await (Task)method.Invoke(comp.Instance, null)!);
        var value = comp.Instance.Value;
        value.Should().NotBeNull();
        var dt = (DateTime?)value;
        dt.Value.Year.Should().Be(2026);
        dt.Value.Hour.Should().Be(11);
    }
}
