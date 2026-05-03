using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.State;
using System.Globalization;

namespace MudExtensions;

/// <summary>
/// Represents a base class for designing date picker components.
/// </summary>
public abstract partial class MudBaseDatePickerX<T> : MudPicker<T>
{
    internal readonly string _mudPickerCalendarContentElementId;
    private readonly ParameterState<string?> _formatState;
    protected readonly string _componentId = Identifier.Create();

    internal DateTime? _picker_month;

    /// <summary>
    /// Is set to true to scroll to the actual year after the next render
    /// </summary>
    protected bool _scrollToYearAfterRender = false;

    /// <summary>
    /// Represents the currently selected date
    /// </summary>
    /// <remarks>
    /// This date is highlighted in the UI
    /// </remarks>
    protected internal DateTime? HighlightedDate { get; set; }

    /// <summary>
    /// Represents the current view state of the object.
    /// </summary>
    protected OpenTo CurrentView;

    /// <summary>
    /// Initializes a new instance of the MudBaseDatePickerX class.
    /// </summary>
    protected MudBaseDatePickerX()
    {
        _mudPickerCalendarContentElementId = Identifier.Create();
        Culture = CultureInfo.CurrentCulture;

        using var registerScope = CreateRegisterScope();
        _formatState = registerScope.RegisterParameter<string?>(nameof(DateFormat))
            .WithParameter(() => DateFormat)
            .WithChangeHandler(DateFormatChangedAsync);
    }

    /// <summary>
    /// Gets or sets the scroll manager used to control scrolling behavior within the component.
    /// </summary>
    [Inject] protected IScrollManager ScrollManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets the JavaScript interop service used to invoke JavaScript APIs from .NET code.
    /// </summary>
    /// <remarks>This property is typically provided by dependency injection in Blazor applications to enable
    /// communication between .NET and JavaScript. It should be set by the framework and not manually assigned in most
    /// scenarios.</remarks>
    [Inject] private IJsApiService JsApiService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the time provider used to obtain the current time within the component.
    /// </summary>
    /// <remarks>This property allows for abstraction of time-related operations, enabling easier testing and
    /// customization of time sources. When overriding the default time behavior, provide a suitable implementation of
    /// the TimeProvider.</remarks>
    [Inject] protected TimeProvider TimeProvider { get; set; } = null!;


    /// <summary>
    /// Time zone information used to convert the selected date and time to a specific time zone. If not set, the component will use the local time zone of the user's device. This property is particularly useful when you want to display or store the selected date and time in a different time zone than the user's local time zone.
    /// </summary>
    [Parameter]
    public TimeZoneInfo? TimeZone { get; set; }

    /// <summary>
    /// The maximum selectable date.
    /// </summary>
    [Parameter] public DateTime? MaxDate { get; set; }

    /// <summary>
    /// The minimum selectable date.
    /// </summary>
    [Parameter] public DateTime? MinDate { get; set; }

    /// <summary>
    /// The initial view to display.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="OpenTo.Date"/>.
    /// </remarks>
    [Parameter] public OpenTo OpenTo { get; set; } = OpenTo.Date;

    /// <summary>
    /// The format for selected dates.
    /// </summary>
    [Parameter, ParameterState]
    public string? DateFormat { get; set; }

    [Parameter] public DayOfWeek? FirstDayOfWeek { get; set; }

    /// <summary>
    /// The current month shown in the date picker.
    /// </summary>
    /// <remarks>
    /// Defaults to the current month.<br />
    /// When bound via <c>@bind-PickerMonth</c>, controls the initial month displayed.  This value is always the first day of a month.
    /// </remarks>
    [Parameter]
    public DateTime? PickerMonth
    {
        get => _picker_month;
        set
        {
            if (value == _picker_month)
                return;
            _picker_month = value;
            InvokeAsync(StateHasChanged);
            PickerMonthChanged.InvokeAsync(value);
        }
    }

    /// <summary>
    /// Occurs when <see cref="PickerMonth"/> has changed.
    /// </summary>
    [Parameter] public EventCallback<DateTime?> PickerMonthChanged { get; set; }

    /// <summary>
    /// The delay, in milliseconds, before closing the picker after a value is selected.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>100</c>.<br />
    /// This delay helps the user see that a date has been selected before the popover disappears.
    /// </remarks>
    [Parameter] public int ClosingDelay { get; set; } = 100;

    /// <summary>
    /// The number of months to display in the calendar.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>1</c>.
    /// </remarks>
    [Parameter] public int DisplayMonths { get; set; } = 1;

    /// <summary>
    /// The maximum number of months allowed in one row.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.<br />
    /// When <c>null</c>, the <see cref="DisplayMonths"/> is used.
    /// </remarks>
    [Parameter] public int? MaxMonthColumns { get; set; }

    /// <summary>
    /// The start month when opening the picker.
    /// </summary>
    [Parameter] public DateTime? StartMonth { get; set; }

    /// <summary>
    /// Shows week numbers at the start of each week.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter] public bool ShowWeekNumbers { get; set; }

    /// <summary>
    /// The format of the selected date in the title.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>ddd, dd MMM</c>.<br />
    /// Supported date formats can be found here: <see href="https://learn.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings"/>.
    /// </remarks>
    [Parameter] public string TitleDateFormat { get; set; } = "ddd, dd MMM";

    /// <summary>
    /// Closes this picker when a value is selected.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter] public bool AutoClose { get; set; }

    /// <summary>
    /// The function used to disable one or more dates.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.<br />
    /// When set, a date will be disabled if the function returns <c>true</c>.
    /// </remarks>
    [Parameter] public Func<DateTime, bool> IsDateDisabledFunc { get; set; } = _ => false;

    /// <summary>
    /// The function which returns CSS classes for a date.
    /// </summary>
    /// <remarks>
    /// Multiple classes must be separated by spaces.
    /// </remarks>
    [Parameter] public Func<DateTime, string>? AdditionalDateClassesFunc { get; set; }

    /// <summary>
    /// The icon for the button that navigates to the previous month or year.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.ChevronLeft"/>.
    /// </remarks>
    [Parameter] public string PreviousIcon { get; set; } = Icons.Material.Filled.ChevronLeft;

    /// <summary>
    /// The icon for the button which navigates to the next month or year.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.ChevronRight"/>.
    /// </remarks>
    [Parameter] public string NextIcon { get; set; } = Icons.Material.Filled.ChevronRight;

    /// <summary>
    /// The year to use, which cannot be changed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter] public int? FixYear { get; set; }

    /// <summary>
    /// The month to use, which cannot be changed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter] public int? FixMonth { get; set; }

    /// <summary>
    /// The day to use, which cannot be changed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter] public int? FixDay { get; set; }

    /// <summary>
    /// True if the generic type T is DateOnly or nullable, false otherwise.
    /// </summary>
    protected internal bool IsDateOnly => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) == typeof(DateOnly);

    /// <summary>
    /// Generic conversion method to convert the generic type T to DateTime. Supports DateTime and DateTimeOffset.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted DateTime value.</returns>
    /// <exception cref="NotSupportedException">Thrown when the type T is not supported.</exception>
    protected internal DateTime? ToDateTime(T? value)
    {
        if (value == null)
            return null;

        var tz = TimeZone ?? TimeZoneInfo.Local;

        if (value is DateTime dt)
            return dt;

        if (value is DateTimeOffset dto)
            return TimeZoneInfo.ConvertTime(dto, tz).DateTime;

        if (value is DateOnly d)
            return d.ToDateTime(TimeOnly.MinValue);

        throw new NotSupportedException($"Type {typeof(T)} not supported");
    }

    /// <summary>
    /// Converts a nullable <see cref="DateTime"/> value to an instance of type <typeparamref name="T"/>, if supported.
    /// </summary>
    /// <remarks>If <typeparamref name="T"/> is <see cref="DateTimeOffset"/>, the local time zone offset is
    /// applied to the converted value.</remarks>
    /// <param name="date">The nullable <see cref="DateTime"/> value to convert. If <see langword="null"/>, the method returns the default
    /// value for <typeparamref name="T"/>.</param>
    /// <returns>An instance of type <typeparamref name="T"/> representing the specified date, or the default value for
    /// <typeparamref name="T"/> if <paramref name="date"/> is <see langword="null"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown if <typeparamref name="T"/> is not <see cref="DateTime"/> or <see cref="DateTimeOffset"/>.</exception>
    protected T? FromDateTime(DateTime? date)
    {
        if (date == null)
            return default;

        var tz = TimeZone ?? TimeZoneInfo.Local;

        var t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (t == typeof(DateTime))
            return (T)(object)date.Value;

        if (t == typeof(DateTimeOffset))
        {
            var offset = tz.GetUtcOffset(date.Value);
            return (T)(object)new DateTimeOffset(date.Value, offset);
        }

        if (t == typeof(DateOnly))
            return (T)(object)DateOnly.FromDateTime(date.Value);

        throw new NotSupportedException($"Type {typeof(T)} not supported");
    }

    protected void ValidateType()
    {
        var t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (t != typeof(DateTime) && t != typeof(DateTimeOffset) && t != typeof(DateOnly))
            throw new NotSupportedException($"Type {typeof(T)} not supported.");
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        ValidateType();
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _picker_month ??= GetCalendarStartOfMonth();
        }

        if (firstRender && CurrentView == OpenTo.Year)
        {
            ScrollToYearAsync().CatchAndLog();
            return;
        }

        if (_scrollToYearAfterRender)
            ScrollToYearAsync().CatchAndLog();
    }

    protected virtual Task DateFormatChangedAsync(string? newFormat) => Task.CompletedTask;

    private Task DateFormatChangedAsync(ParameterChangedEventArgs<string?> args) => DateFormatChangedAsync(args.Value);

    /// <summary>
    /// True if the current view is either hours or minutes, false otherwise.
    /// </summary>
    public bool IsTimeView => CurrentView == OpenTo.Hours || CurrentView == OpenTo.Minutes;

    /// <summary>
    /// True if the current view is either date, month or year, false otherwise.
    /// </summary>
    public bool IsDateView =>
        CurrentView == OpenTo.Date
        || CurrentView == OpenTo.Month
        || CurrentView == OpenTo.Year;

    protected override async Task OnPickerOpenedAsync()
    {
        await base.OnPickerOpenedAsync();

        var dateTime = ToDateTime(_value);

        if (dateTime.HasValue)
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            PickerMonth = new DateTime(
                calendar.GetYear(dateTime.Value),
                calendar.GetMonth(dateTime.Value),
                1,
                calendar);
        }

        CurrentView = OpenTo;
    }

    protected DateTime GetMonthStart(int month)
    {
        var culture = GetCulture();
        var calendar = culture.Calendar;
        var baseDate = _picker_month ?? DateTime.Today;

        return calendar.AddMonths(new DateTime(baseDate.Year, baseDate.Month, 1), month);
    }

    protected IEnumerable<DateTime> GetWeek(int month, int index)
    {
        if (index is < 0 or > 5)
            throw new ArgumentException("Index must be between 0 and 5", nameof(index));

        var culture = GetCulture();
        var monthFirst = GetMonthStart(month);

        var weekFirst = monthFirst
            .AddDays(index * 7)
            .StartOfWeek(GetFirstDayOfWeek(), culture);

        for (var i = 0; i < 7; i++)
            yield return weekFirst.AddDays(i);
    }

    protected virtual bool IsDayDisabled(DateTime date)
    {
        return date < MinDate ||
               date > MaxDate ||
               IsDateDisabledFunc(date);
    }

    protected abstract string GetDayClasses(int month, DateTime day);
    protected abstract Task OnDayClickedAsync(DateTime dateTime);

    protected string FormatTitleDate(DateTime? date)
    {
        return date?.ToString(TitleDateFormat, GetCulture()) ?? "";
    }

    protected IEnumerable<string> GetAbbreviatedDayNames()
    {
        var culture = GetCulture();
        var names = culture.DateTimeFormat.AbbreviatedDayNames;

        var firstDay = (int)GetFirstDayOfWeek();

        return Enumerable.Range(0, 7).Select(i => names[(i + firstDay) % 7]);
    }

    protected override IConverter<T?, string?> GetDefaultConverter()
    {
        return new DefaultConverter<T?>
        {
            Culture = GetCulture,
            Format = GetFormat
        };
    }

    protected override string? ConvertSet(T? value)
    {
        var dt = ToDateTime(value);

        if (dt == null)
            return null;

        return dt.Value.ToString(GetFormat(), GetCulture());
    }

    /// <summary>
    /// Returns the date and time format string to use for formatting operations.
    /// </summary>
    /// <remarks>The returned format string is suitable for use with date and time formatting methods. If no
    /// custom format is specified, the method combines the current culture's short date pattern with a 24-hour time
    /// component.</remarks>
    /// <returns>A format string representing the date and time pattern. If a custom format is set, that value is returned;
    /// otherwise, a default pattern based on the current culture's short date pattern and a 24-hour time format is
    /// used.</returns>
    protected override string GetFormat()
    {
        if (!string.IsNullOrWhiteSpace(_formatState.Value))
            return _formatState.Value;

        if (IsDateOnly)
            return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

        return $"{CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern} HH:mm";
    }

    protected abstract DateTime GetCalendarStartOfMonth();
    protected abstract int GetCalendarYear(DateTime yearDate);

    protected DayOfWeek GetFirstDayOfWeek()
    {
        return FirstDayOfWeek ?? GetCulture().DateTimeFormat.FirstDayOfWeek;
    }

    protected DateTime GetMonthEnd(int month)
    {
        var culture = GetCulture();
        var calendar = culture.Calendar;
        var monthStartDate = PickerMonth ?? DateTime.Today.StartOfMonth(culture);

        return calendar
            .AddMonths(monthStartDate, month)
            .EndOfMonth(culture);
    }

    /// <summary>
    /// Scrolls to the current year.
    /// </summary>
    public virtual Task ScrollToYearAsync(DateTime? date = null)
    {
        return Task.CompletedTask;
    }

    //private ValueTask HandleMouseoverOnPickerCalendarDayButton(int tempId)
    //{
    //    return JsApiService.UpdateStyleProperty(_mudPickerCalendarContentElementId, "--selected-day", tempId);
    //}
}