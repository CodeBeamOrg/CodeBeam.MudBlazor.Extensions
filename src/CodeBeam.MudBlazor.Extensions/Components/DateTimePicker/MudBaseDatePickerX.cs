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
    /// <summary>
    /// The unique identifier used to associate picker elements.
    /// </summary>
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
    /// Defaults to the current month.
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
    /// Defaults to <c>100</c>.
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
    /// Defaults to <c>null</c>.
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
    /// Defaults to <c>ddd, dd MMM</c>.
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
    /// Defaults to <c>null</c>.
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
    /// Determines whether a non-nullable picker value is its default value.
    /// </summary>
    /// <param name="value">The value to evaluate.</param>
    /// <returns><see langword="true"/> when <typeparamref name="T"/> is non-nullable and <paramref name="value"/> equals its default value; otherwise, <see langword="false"/>.</returns>
    protected internal bool IsDefaultValue(T? value)
    {
        return Nullable.GetUnderlyingType(typeof(T)) is null &&
               EqualityComparer<T?>.Default.Equals(value, default);
    }

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

    /// <summary>
    /// Validates that <typeparamref name="T"/> is a supported date type.
    /// </summary>
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

    /// <summary>
    /// Handles a change to the configured date format.
    /// </summary>
    /// <param name="newFormat">The newly configured date format.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <inheritdoc />
    protected override async Task OnPickerOpenedAsync()
    {
        await base.OnPickerOpenedAsync();

        var dateTime = ToDateTime(_value);

        if (IsDefaultValue(_value))
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            PickerMonth = new DateTime(
                calendar.GetYear(DateTime.Today),
                calendar.GetMonth(DateTime.Today),
                1,
                calendar);
        }
        else if (dateTime.HasValue)
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

    /// <summary>
    /// Gets the first day of the month at the specified offset from the displayed month.
    /// </summary>
    /// <param name="month">The zero-based offset from the displayed month.</param>
    /// <returns>The first day of the requested month.</returns>
    protected DateTime GetMonthStart(int month)
    {
        var culture = GetCulture();
        var calendar = culture.Calendar;
        var baseDate = _picker_month ?? DateTime.Today;

        return calendar.AddMonths(new DateTime(baseDate.Year, baseDate.Month, 1), month);
    }

    /// <summary>
    /// Gets the seven days displayed for a calendar week.
    /// </summary>
    /// <param name="month">The zero-based offset from the displayed month.</param>
    /// <param name="index">The zero-based week index.</param>
    /// <returns>The dates in the requested week.</returns>
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

    /// <summary>
    /// Determines whether a date is unavailable for selection.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns><see langword="true"/> when the date is disabled; otherwise, <see langword="false"/>.</returns>
    protected virtual bool IsDayDisabled(DateTime date)
    {
        return date < MinDate ||
               date > MaxDate ||
               IsDateDisabledFunc(date);
    }

    /// <summary>
    /// Gets the CSS classes for a calendar day.
    /// </summary>
    /// <param name="month">The zero-based offset from the displayed month.</param>
    /// <param name="day">The day to format.</param>
    /// <returns>The CSS classes for the day.</returns>
    protected abstract string GetDayClasses(int month, DateTime day);

    /// <summary>
    /// Handles selection of a calendar day.
    /// </summary>
    /// <param name="dateTime">The selected date and time.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task OnDayClickedAsync(DateTime dateTime);

    /// <summary>
    /// Formats a date for the picker title.
    /// </summary>
    /// <param name="date">The date to format.</param>
    /// <returns>The formatted title, or an empty string when <paramref name="date"/> is <see langword="null"/>.</returns>
    protected string FormatTitleDate(DateTime? date)
    {
        return date?.ToString(TitleDateFormat, GetCulture()) ?? "";
    }

    /// <summary>
    /// Gets abbreviated day names ordered according to the configured first day of the week.
    /// </summary>
    /// <returns>The ordered abbreviated day names.</returns>
    protected IEnumerable<string> GetAbbreviatedDayNames()
    {
        var culture = GetCulture();
        var names = culture.DateTimeFormat.AbbreviatedDayNames;

        var firstDay = (int)GetFirstDayOfWeek();

        return Enumerable.Range(0, 7).Select(i => names[(i + firstDay) % 7]);
    }

    /// <inheritdoc />
    protected override IConverter<T?, string?> GetDefaultConverter()
    {
        return new DefaultConverter<T?>
        {
            Culture = GetCulture,
            Format = GetFormat
        };
    }

    /// <inheritdoc />
    protected override string? ConvertSet(T? value)
    {
        var dt = ToDateTime(value);

        if (dt == null)
            return null;

        return dt.Value.ToString(GetFormat(), GetCulture());
    }

    /// <summary>
    /// Converts a picker value to its display text.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The formatted text, or <see langword="null"/> when the value has no date representation.</returns>
    protected internal string? ConvertSetInternal(T? value)
    {
        return ConvertSet(value);
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

    /// <summary>
    /// Gets the initial month shown by the calendar.
    /// </summary>
    /// <returns>The first day of the initial month.</returns>
    protected abstract DateTime GetCalendarStartOfMonth();

    /// <summary>
    /// Gets the culture-specific calendar year for a date.
    /// </summary>
    /// <param name="yearDate">The date whose calendar year is required.</param>
    /// <returns>The calendar year.</returns>
    protected abstract int GetCalendarYear(DateTime yearDate);

    /// <summary>
    /// Gets the first day of the calendar week.
    /// </summary>
    /// <returns>The configured first day of the week, or the culture default.</returns>
    protected DayOfWeek GetFirstDayOfWeek()
    {
        return FirstDayOfWeek ?? GetCulture().DateTimeFormat.FirstDayOfWeek;
    }

    /// <summary>
    /// Gets the last day of the month at the specified offset from the displayed month.
    /// </summary>
    /// <param name="month">The zero-based offset from the displayed month.</param>
    /// <returns>The last day of the requested month.</returns>
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
