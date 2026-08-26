using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;

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
    public void DateTimePicker_DateTime_Should_Render_Time()
    {
        var comp = RenderPicker(
            value: new DateTime(2026, 5, 3, 14, 30, 0),
            format: "dd.MM.yyyy HH:mm");

        comp.Instance.ConvertSetInternal(comp.Instance.Value).Should().Be("03.05.2026 14:30");
    }

    [Test]
    public void DateTimePicker_DateTimeNullable_Should_Render_Empty_When_Null()
    {
        var comp = RenderPicker<DateTime?>();

        comp.Find("input").GetAttribute("value").Should().BeNullOrEmpty();
    }

    [Test]
    public void DateTimePicker_Default_DateTime_Should_Render_Empty()
    {
        var comp = RenderPicker<DateTime>();

        comp.Find("input").GetAttribute("value").Should().BeNullOrEmpty();
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
    public void DateTimePicker_DateOnly_Should_Render_Date_Only()
    {
        var comp = RenderPicker(
            value: new DateOnly(2026, 5, 3),
            format: "dd.MM.yyyy");

        comp.Instance.ConvertSetInternal(comp.Instance.Value).Should().Be("03.05.2026");
    }

    [Test]
    public void DateTimePicker_Default_DateOnly_Should_Render_Empty()
    {
        var comp = RenderPicker<DateOnly>();

        comp.Find("input").GetAttribute("value").Should().BeNullOrEmpty();
    }

    [Test]
    public void DateTimePicker_DateOnlyNullable_Should_Render_Empty_When_Null()
    {
        var comp = RenderPicker<DateOnly?>();

        comp.Find("input").GetAttribute("value").Should().BeNullOrEmpty();
    }

    [Test]
    public void DateTimePicker_DateTimeOffset_Should_Convert_Correctly()
    {
        DateTimeOffset value = new DateTimeOffset(2026, 5, 3, 10, 0, 0, TimeSpan.Zero);

        var comp = Context.Render<MudDateTimePicker<DateTimeOffset>>(p => p
            .Add(x => x.Value, value)
            .Add(x => x.TimeZone, TimeZoneInfo.Utc)
        );

        var dt = comp.Instance.ToDateTime(value);

        dt.Should().Be(new DateTime(2026, 5, 3, 10, 0, 0));
    }

    [Test]
    public void DateTimePicker_DateTimeOffsetNullable_Should_Convert_Correctly()
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
    public void DateTimePicker_DateTimeOffsetNullable_Should_Render_Empty_When_Null()
    {
        var comp = RenderPicker<DateTimeOffset?>();

        comp.Find("input").GetAttribute("value").Should().BeNullOrEmpty();
    }

    [Test]
    public void DateTimePicker_Default_DateTimeOffset_Should_Render_Empty()
    {
        var comp = RenderPicker<DateTimeOffset>();

        comp.Find("input").GetAttribute("value").Should().BeNullOrEmpty();
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
}
