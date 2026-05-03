using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MudExtensions;

/// <summary>
/// Provides date and time selection in a single component. The date and time can be submitted together or separately. The time selection is done through an interactive clock interface where the user can select hours and minutes by clicking or dragging a pointer.
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class MudDateTimePicker<T> : MudBaseDatePickerX<T>
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

    [DynamicDependency(nameof(OnStickClick))]
    [DynamicDependency(nameof(SelectTimeFromStick))]
    public MudDateTimePicker()
    {
        _dotNetReferenceLazy = new Lazy<DotNetObjectReference<MudDateTimePicker<T>>>(CreateDotNetObjectReference);
    }

    private string? _clockElementReferenceId;
    private readonly Lazy<DotNetObjectReference<MudDateTimePicker<T>>> _dotNetReferenceLazy;

    private DotNetObjectReference<MudDateTimePicker<T>> CreateDotNetObjectReference() => DotNetObjectReference.Create(this);

    private DateTime? _workingValue;
    private readonly SetTime _timeSet = new();
    private string _timeHourFormat;

    private record SetTime
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
    }

    public bool PointerMoving { get; set; }

    protected ElementReference ClockElementReference { get; private set; }
    private bool _amPm = false;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _workingValue = ToDateTime(Value);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _workingValue = ToDateTime(Value);
        SyncTimeFromValue();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        // Initialize the pointer events for the clock every time it's created (ex: popover opening and closing).
        if (ClockElementReference.Id != _clockElementReferenceId)
        {
            _clockElementReferenceId = ClockElementReference.Id;

            await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudTimePicker.initPointerEvents", ClockElementReference, _dotNetReferenceLazy.Value);
        }
    }

    private void SyncTimeFromValue()
    {
        if (_workingValue == null)
        {
            _timeSet.Hour = 0;
            _timeSet.Minute = 0;
            return;
        }

        _timeSet.Hour = _workingValue.Value.Hour;
        _timeSet.Minute = _workingValue.Value.Minute;
    }

    protected PickerMode _mode = PickerMode.Date;

    /// <summary>
    /// The currently selected value.
    /// </summary>
    /// <remarks>
    /// When this value changes, <see cref="ValueChanged"/> occurs.
    /// </remarks>
    [Parameter]
    public T? Value
    {
        get => _value;
        set => SetDateAsync(ToDateTime(value), true).CatchAndLog();
    }

    /// <summary>
    /// Occurs when <see cref="Value"/> has changed.
    /// </summary>
    [Parameter]
    public EventCallback<T?> ValueChanged { get; set; }

    /// <summary>
    /// Shows a 12-hour selection clock.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.<br />
    /// When <c>true</c>, hours 1-12 are displayed with an AM or PM marker.<br />
    /// When <c>false</c>, hours 0-23 are displayed.<br />
    /// </remarks>
    [Parameter]
    public bool AmPm
    {
        get => _amPm;
        set
        {
            if (_amPm == value)
                return;

            _amPm = value;

            Touched = true;
            _ = SetTextAsync(ConvertSet(_value), false);
        }
    }

    /// <summary>
    /// The step interval when selecting minutes.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>1</c>. For example: a value of <c>15</c> would allow minutes <c>0</c>, <c>15</c>, 
    /// <c>30</c>, and <c>45</c> be selected.
    /// </remarks>
    [Parameter]
    public int MinuteSelectionStep { get; set; } = 1;

    /// <summary>
    /// Controls which values can be edited.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="TimeEditMode.Normal"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerBehavior)]
    public TimeEditMode TimeEditMode { get; set; } = TimeEditMode.Normal;

    private int RoundToStepInterval(int value)
    {
        if (MinuteSelectionStep > 1)
        {
            var interval = MinuteSelectionStep % 60;
            value = (value + (interval / 2)) / interval * interval;

            if (value == 60)
                value = 0;
        }

        return value;
    }

    /// <summary>
    /// Gets or sets the text displayed for the AM period in a time picker or similar control.
    /// </summary>
    [Parameter]
    public string AmText { get; set; } = "AM";

    /// <summary>
    /// Gets or sets the text displayed for the post-meridiem (PM) indicator.
    /// </summary>
    [Parameter]
    public string PmText { get; set; } = "PM";

    protected override async Task WriteTextAsync(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            _workingValue = null;
            _value = default;

            await ValueChanged.InvokeAsync(_value);
            return;
        }

        var culture = GetCulture();

        if (DateTime.TryParseExact(text, GetFormat(), culture, DateTimeStyles.None, out var parsed))
        {
            _workingValue = parsed;
            _value = FromDateTime(parsed);

            SyncTimeFromValue();

            PickerMonth = new DateTime(parsed.Year, parsed.Month, 1);

            await ValueChanged.InvokeAsync(_value);
            await BeginValidateAsync();
            FieldChanged(_value);
        }
        else
        {
            await SetTextAsync(ConvertSet(_value), false);
        }
    }

    private async Task OnAmClickedAsync()
    {
        _timeSet.Hour %= 12;
        await UpdateTimeAsync();
        await FocusAsync();
    }

    private async Task OnPmClickedAsync()
    {
        if (_timeSet.Hour <= 12)
        {
            _timeSet.Hour += 12;
        }

        _timeSet.Hour %= 24;
        await UpdateTimeAsync();
        await FocusAsync();
    }

    private DateTimeOffset _lastSetTime = DateTimeOffset.MinValue;
    private const int DebounceTimeoutMs = 100;

    protected async Task SetDateAsync(DateTime? date, bool updateValue)
    {
        var current = ToDateTime(_value);

        if (current != null && date != null && date.Value.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date.Value, current.Value.Kind);
        }

        var now = TimeProvider.GetUtcNow();

        if (current == date && (now - _lastSetTime).TotalMilliseconds < DebounceTimeoutMs)
            return;

        _lastSetTime = now;

        if (current != date || (date is null && Text != null))
        {
            Touched = true;

            HighlightedDate = date;

            if (date is not null && IsDateDisabledFunc(date.Value.Date))
            {
                await SetTextAsync(null, false);
                return;
            }

            if (date is not null)
            {
                var culture = GetCulture();
                PickerMonth = new DateTime(
                    culture.Calendar.GetYear(date.Value),
                    culture.Calendar.GetMonth(date.Value),
                    1,
                    culture.Calendar);
            }

            var converted = FromDateTime(date);
            _value = converted;

            if (updateValue)
            {
                ResetConverterErrors();
                await SetTextAsync(ConvertSet(_value), false);
            }

            await ValueChanged.InvokeAsync(_value);
            await BeginValidateAsync();
            FieldChanged(_value);
        }
    }

    private async Task UpdateTimeAsync()
    {
        if (_workingValue == null)
            _workingValue = TimeProvider.GetLocalNow().Date;

        _workingValue = new DateTime(
            _workingValue.Value.Year,
            _workingValue.Value.Month,
            _workingValue.Value.Day,
            _timeSet.Hour,
            _timeSet.Minute,
            0
        );
    }

    private void SetDatePart(DateTime date)
    {
        var current = _workingValue ?? TimeProvider.GetLocalNow().Date;

        _workingValue = new DateTime(
            date.Year,
            date.Month,
            date.Day,
            current.Hour,
            current.Minute,
            current.Second
        );
    }

    private void SetTimePart(int hour, int minute)
    {
        var current = _workingValue ?? TimeProvider.GetLocalNow().Date;

        _workingValue = new DateTime(
            current.Year,
            current.Month,
            current.Day,
            hour,
            minute,
            0
        );
    }

    protected override string GetDayClasses(int month, DateTime day)
    {
        var b = new CssBuilder("mud-day");

        b.AddClass(AdditionalDateClassesFunc?.Invoke(day) ?? string.Empty);

        if (day < GetMonthStart(month) || day > GetMonthEnd(month))
            return b.AddClass("mud-hidden").Build();

        var current = _workingValue ?? ToDateTime(Value);

        if (current?.Date == day.Date)
            return b.AddClass("mud-selected")
                .AddClass($"mud-theme-{Color.ToStringFast(true)}")
                .Build();

        if (day.Date == TimeProvider.GetLocalNow().Date)
            return b.AddClass("mud-current mud-button-outlined")
                .AddClass($"mud-button-outlined-{Color.ToStringFast(true)} mud-{Color.ToStringFast(true)}-text")
                .Build();

        return b.Build();
    }

    /// <summary>
    /// Handles the event when a day is clicked in the calendar view. This method updates the working value with the selected date, and if appropriate based on the component's configuration, submits the new value and closes the picker.
    /// </summary>
    /// <param name="dateTime">The date that was clicked.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task OnDayClickedAsync(DateTime dateTime)
    {
        await FocusAsync();

        SetDatePart(dateTime);

        if (PickerActions == null || AutoClose || PickerVariant == PickerVariant.Static)
        {
            await Task.Run(() => InvokeAsync(SubmitAsync));

            if (PickerVariant != PickerVariant.Static)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(ClosingDelay), TimeProvider);
                await CloseAsync(false);
            }
        }
    }

    /// <summary>
    /// Submits the current value asynchronously, triggering value change notifications and validation as appropriate.
    /// </summary>
    /// <remarks>The method does not perform any action if the control is in a read-only state or if the
    /// working value is null. Upon successful submission, the method updates the value, invokes change notifications,
    /// updates the displayed text, and initiates validation.</remarks>
    /// <returns>A task that represents the asynchronous submit operation.</returns>
    protected override async Task SubmitAsync()
    {
        if (GetReadOnlyState())
            return;

        if (_workingValue == null)
            return;

        var converted = FromDateTime(_workingValue);

        _value = converted;

        await ValueChanged.InvokeAsync(_value);
        await SetTextAsync(ConvertSet(_value), false);
        await BeginValidateAsync();
        FieldChanged(_value);
    }

    /// <summary>
    /// Clears the selected date and time, resetting the component to its initial state. If <see cref="AutoClose"/> is <c>true</c>, the picker will also close after clearing the value.
    /// </summary>
    /// <param name="close">Indicates whether the picker should close after clearing the value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task ClearAsync(bool close = true)
    {
        await SetDateAsync(null, true);

        if (AutoClose)
            await CloseAsync(false);
    }

    /// <summary>
    /// Gets the formatted date string for the title of the picker, based on the current working value, the component's value, or the current local date if neither is set. The date is formatted according to the culture settings and the specified format for the title.
    /// </summary>
    /// <returns>The formatted date string for the title of the picker.</returns>
    protected string GetTitleDateString()
    {
        var date = _workingValue
            ?? ToDateTime(Value)
            ?? TimeProvider.GetLocalNow().Date;

        return FormatTitleDate(date);
    }

    /// <summary>
    /// Calculates the first day of the month for the current calendar context.
    /// </summary>
    /// <remarks>The returned date is determined using the culture-specific calendar, which may affect the
    /// calculation of the month's start depending on the culture in use.</remarks>
    /// <returns>A <see cref="DateTime"/> representing the first day of the month, based on the current value, highlighted date,
    /// or the current local date if neither is set.</returns>
    protected override DateTime GetCalendarStartOfMonth()
    {
        var date = ToDateTime(Value) ?? HighlightedDate ?? TimeProvider.GetLocalNow().Date;
        return date.StartOfMonth(GetCulture());
    }

    /// <summary>
    /// Calculates the calendar year corresponding to the specified date, adjusted according to the current value and
    /// culture settings.
    /// </summary>
    /// <remarks>The result is determined using the calendar of the current culture. If the current value is
    /// not set, the calculation uses the current local date.</remarks>
    /// <param name="yearDate">The date for which to determine the calendar year. The calculation is based on the calendar associated with the
    /// current culture.</param>
    /// <returns>The calendar year as an integer, adjusted based on the current value and the specified date.</returns>
    protected override int GetCalendarYear(DateTime yearDate)
    {
        var date = ToDateTime(Value) ?? TimeProvider.GetLocalNow().Date;
        var diff = GetCulture().Calendar.GetYear(date) - GetCulture().Calendar.GetYear(yearDate);

        return GetCulture().Calendar.GetYear(date) - diff;
    }

    protected string GetMonthName(int month)
    {
        var date = GetMonthStart(month);
        return date.ToString("MMMM yyyy", GetCulture());
    }

    protected Task OnPreviousMonthClick()
    {
        PickerMonth = GetMonthStart(0).AddMonths(-1);
        return Task.CompletedTask;
    }

    protected Task OnNextMonthClick()
    {
        PickerMonth = GetMonthStart(0).AddMonths(1);
        return Task.CompletedTask;
    }

    private void GoToSelectedYear()
    {
        PickerMonth = HighlightedDate;
        OnYearClick();
    }

    private void OnYearClick()
    {
        if (!FixYear.HasValue)
        {
            _mode = PickerMode.Date;
            CurrentView = OpenTo.Year;
            StateHasChanged();
            _scrollToYearAfterRender = true;
        }
    }

    protected int GetMinYear()
    {
        return MinDate?.Year ?? 1900;
    }

    protected int GetMaxYear()
    {
        return MaxDate?.Year ?? 2100;
    }

    protected Task OnYearClickedAsync(int year)
    {
        var current = ToDateTime(Value) ?? TimeProvider.GetLocalNow().Date;
        PickerMonth = new DateTime(year, current.Month, 1);

        _workingValue = new DateTime(
            year,
            current.Month,
            current.Day,
            current.Hour,
            current.Minute,
            current.Second
        );

        CurrentView = OpenTo.Month;
        return Task.CompletedTask;
    }

    protected Typo GetYearTypo(int year)
    {
        var current = ToDateTime(Value);
        return current?.Year == year ? Typo.h5 : Typo.body1;
    }

    protected string GetYearClasses(int year)
    {
        var current = ToDateTime(Value);

        return new CssBuilder("mud-picker-year-text")
            .AddClass("mud-selected", current?.Year == year)
            .Build();
    }

    protected Task OnPreviousYearClick()
    {
        PickerMonth = (PickerMonth ?? DateTime.Today).AddYears(-1);
        return Task.CompletedTask;
    }

    protected Task OnNextYearClick()
    {
        PickerMonth = (PickerMonth ?? DateTime.Today).AddYears(1);
        return Task.CompletedTask;
    }

    protected IEnumerable<int> GetAllMonths()
    {
        return Enumerable.Range(1, 12);
    }

    protected Task OnMonthSelectedAsync(int month)
    {
        var current = _workingValue ?? ToDateTime(Value) ?? TimeProvider.GetLocalNow().Date;
        PickerMonth = new DateTime(current.Year, month, 1);

        _workingValue = new DateTime(
            current.Year,
            month,
            current.Day,
            current.Hour,
            current.Minute,
            current.Second
        );

        CurrentView = OpenTo.Date;
        return Task.CompletedTask;
    }

    protected bool IsMonthDisabled(int month)
    {
        if (!MinDate.HasValue && !MaxDate.HasValue)
            return false;

        var year = (PickerMonth ?? DateTime.Today).Year;

        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        return (MinDate.HasValue && end < MinDate.Value)
            || (MaxDate.HasValue && start > MaxDate.Value);
    }

    protected Typo GetMonthTypo(int month)
    {
        var current = ToDateTime(Value);
        return current?.Month == month ? Typo.h6 : Typo.body2;
    }

    protected string GetMonthClasses(int month)
    {
        var current = ToDateTime(Value);

        return new CssBuilder()
            .AddClass("mud-selected", current?.Month == month)
            .Build();
    }

    protected string GetAbbreviatedMonthName(int month)
    {
        return GetCulture().DateTimeFormat.AbbreviatedMonthNames[month - 1];
    }

    protected int GetWeekNumber(int month, int week)
    {
        var firstDay = GetWeek(month, week).First();

        return GetCulture().Calendar.GetWeekOfYear(
            firstDay,
            CalendarWeekRule.FirstFourDayWeek,
            GetFirstDayOfWeek());
    }

    protected string GetCalendarDayOfMonth(DateTime date)
    {
        return date.Day.ToString(GetCulture());
    }

    protected void OnFormattedDateClick()
    {
        CurrentView = OpenTo.Month;
    }

    protected void OnMonthClicked(int month)
    {
        CurrentView = OpenTo.Month;
    }

    private string GetCalendarHeaderClasses(int month)
    {
        return new CssBuilder("mud-picker-calendar-header")
            .AddClass($"mud-picker-calendar-header-{month + 1}")
            .AddClass($"mud-picker-calendar-header-last", month == DisplayMonths - 1)
            .Build();
    }

    private string HourDialClassname =>
    new CssBuilder("mud-time-picker-dial")
        .AddClass("mud-time-picker-hour")
        .AddClass("mud-time-picker-dial-out", CurrentView != OpenTo.Hours)
        .AddClass("mud-time-picker-dial-hidden", CurrentView != OpenTo.Hours)
        .Build();

    private string MinuteDialClassname =>
        new CssBuilder("mud-time-picker-dial")
            .AddClass("mud-time-picker-minute")
            .AddClass("mud-time-picker-dial-out", CurrentView != OpenTo.Minutes)
            .AddClass("mud-time-picker-dial-hidden", CurrentView != OpenTo.Minutes)
            .Build();

    private string GetPointerRotation()
    {
        return $"rotateZ({GetDeg()}deg);";
    }

    private double GetDeg()
    {
        double deg = 0;

        if (CurrentView == OpenTo.Hours)
        {
            deg = _timeSet.Hour * 30 % 360;
        }

        if (CurrentView == OpenTo.Minutes)
        {
            deg = _timeSet.Minute * 6 % 360;
        }

        return deg;
    }

    private string GetPointerHeight()
    {
        var height = 40;

        if (CurrentView == OpenTo.Minutes)
        {
            height = 40;
        }

        if (CurrentView == OpenTo.Hours)
        {
            if (!AmPm && _timeSet.Hour > 0 && _timeSet.Hour < 13)
            {
                height = 26;
            }
            else
            {
                height = 40;
            }
        }

        return $"{height}%;";
    }

    private string GetNumberColor(int value)
    {
        if (CurrentView == OpenTo.Hours)
        {
            var h = _timeSet.Hour;

            if (AmPm)
            {
                h = _timeSet.Hour % 12;
                if (_timeSet.Hour % 12 == 0)
                {
                    h = 12;
                }
            }

            if (h == value)
            {
                return $"mud-clock-number mud-theme-{Color.ToStringFast(true)}";
            }
        }
        else if (CurrentView == OpenTo.Minutes && _timeSet.Minute == value)
        {
            return $"mud-clock-number mud-theme-{Color.ToStringFast(true)}";
        }

        return "mud-clock-number";
    }

    private string GetClockPointerColor()
    {
        return PointerMoving
            ? $"mud-picker-time-clock-pointer mud-{Color.ToStringFast(true)}"
            : $"mud-picker-time-clock-pointer mud-picker-time-clock-pointer-animation mud-{Color.ToStringFast(true)}";
    }

    private string GetClockPinColor()
    {
        return $"mud-picker-time-clock-pin mud-{Color.ToStringFast(true)}";
    }

    private string GetClockPointerThumbColor()
    {
        var deg = GetDeg();
        return deg % 30 == 0
            ? $"mud-picker-time-clock-pointer-thumb mud-onclock-text mud-onclock-primary mud-{Color.ToStringFast(true)}"
            : $"mud-picker-time-clock-pointer-thumb mud-onclock-minute mud-{Color.ToStringFast(true)}-text";
    }

    private static string GetTransform(double angle, double radius, double offsetX, double offsetY)
    {
        angle = angle / 180 * Math.PI;
        var x = ((Math.Sin(angle) * radius) + offsetX).ToString("F3", CultureInfo.InvariantCulture);
        var y = (((Math.Cos(angle) + 1) * radius) + offsetY).ToString("F3", CultureInfo.InvariantCulture);
        return $"transform: translate({x}px, {y}px);";
    }

    [JSInvokable]
    public async Task SelectTimeFromStick(int value, bool pointerMoving)
    {
        PointerMoving = pointerMoving;

        if (CurrentView == OpenTo.Minutes)
            _timeSet.Minute = RoundToStepInterval(value);
        else
            _timeSet.Hour = value;

        await UpdateTimeAsync();

        StateHasChanged();
    }

    [JSInvokable]
    public async Task OnStickClick(int value)
    {
        // The pointer is up and not moving so animations can be enabled again.
        PointerMoving = false;

        // Clicking a stick will submit the time.
        if (CurrentView == OpenTo.Minutes)
        {
            await SubmitAndCloseAsync();
        }
        else if (CurrentView == OpenTo.Hours)
        {
            if (TimeEditMode == TimeEditMode.Normal)
            {
                CurrentView = OpenTo.Minutes;
            }
            else if (TimeEditMode == TimeEditMode.OnlyHours)
            {
                await SubmitAndCloseAsync();
            }
        }

        // Manually update because the event won't do it from JavaScript.
        StateHasChanged();
    }

    protected async Task SubmitAndCloseAsync()
    {
        if (PickerActions == null || AutoClose)
        {
            await SubmitAsync();

            if (PickerVariant != PickerVariant.Static)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(ClosingDelay), TimeProvider);
                await CloseAsync(false);
            }
        }
    }

    /// <summary>
    /// Gets the hour portion of the selected time.
    /// </summary>
    /// <returns>A two-character string depending on whether <see cref="AmPm"/> is set, or <c>--</c> if no value is set.</returns>
    private string GetHourString()
    {
        if (_workingValue?.Hour == null)
        {
            return "--";
        }

        return _workingValue.Value.Hour.ToString("D2");
    }

    /// <summary>
    /// Gets the minute portion of the selected time.
    /// </summary>
    /// <returns>A two-digit string for minutes, or <c>--</c> if no value is set.</returns>
    private string GetMinuteString()
    {
        if (_workingValue?.Minute == null)
        {
            return "--";
        }

        return _workingValue.Value.Minute.ToString("D2");
    }

    private async Task OnHourClickAsync()
    {
        CurrentView = OpenTo.Hours;
        await FocusAsync();
    }

    private async Task OnMinutesClick()
    {
        CurrentView = OpenTo.Minutes;
        await FocusAsync();
    }

    private async Task HourFormatChanged(string value)
    {
        if (value == "am")
        {
            await OnAmClickedAsync();
        }
        else if (value == "pm")
        {
            await OnPmClickedAsync();
        }
        StateHasChanged();
    }

    private void HandleModeChange(PickerMode mode)
    {
        if (mode == PickerMode.Date)
        {
            CurrentView = OpenTo.Date;
        }
        else if (mode == PickerMode.Time)
        {
            CurrentView = OpenTo.Hours;
        }
    }

    protected string GetFormattedYearString()
    {
        var date = _workingValue
            ?? ToDateTime(Value)
            ?? TimeProvider.GetLocalNow().Date;

        return date.Year.ToString();
    }

    /// <summary>
    /// Scrolls to the current year.
    /// </summary>
    public override async Task ScrollToYearAsync(DateTime? date = null)
    {
        var culture = GetCulture();
        var calendar = culture.Calendar;

        _scrollToYearAfterRender = false;

        var dateTime =
            date
            ?? _workingValue
            ?? ToDateTime(Value)
            ?? TimeProvider.GetLocalNow().Date;

        var id = $"{_componentId}{calendar.GetYear(dateTime)}";

        await ScrollManager.ScrollToYearAsync(id);

        StateHasChanged();
    }

    protected override async Task OnOpenedAsync()
    {
        _mode = PickerMode.Date;
        CurrentView = OpenTo.Hours;
        await base.OnOpenedAsync();
    }

    /// <summary>
    /// Sets the current view of the picker to the specified value.
    /// </summary>
    public void SetView(OpenTo view)
    {
        switch (view)
        {
            case OpenTo.Date:
                _mode = PickerMode.Date;
                CurrentView = OpenTo.Date;
                break;

            case OpenTo.Month:
                _mode = PickerMode.Date;
                CurrentView = OpenTo.Month;
                break;

            case OpenTo.Year:
                _mode = PickerMode.Date;
                CurrentView = OpenTo.Year;
                break;

            case OpenTo.Hours:
                _mode = PickerMode.Time;
                CurrentView = OpenTo.Hours;
                break;

            case OpenTo.Minutes:
                _mode = PickerMode.Time;
                CurrentView = OpenTo.Minutes;
                break;
        }

        StateHasChanged();
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
    /// </summary>
    protected override async ValueTask DisposeAsyncCore()
    {
        await base.DisposeAsyncCore();

        if (IsJSRuntimeAvailable)
        {
            await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudTimePicker.destroyPointerEvents", ClockElementReference);
        }

        if (_dotNetReferenceLazy.IsValueCreated)
        {
            _dotNetReferenceLazy.Value.Dispose();
        }
    }
}
