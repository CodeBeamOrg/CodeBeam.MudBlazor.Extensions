using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.State;
using MudBlazor.Utilities;
using System.Globalization;
using static MudBlazor.Colors;

namespace MudExtensions
{
    public abstract partial class MudBaseDatePickerX<T> : MudPicker<T>
    {
        internal readonly string _mudPickerCalendarContentElementId;
        private readonly ParameterState<string?> _formatState;
        protected readonly string _componentId = Identifier.Create();

        protected MudBaseDatePickerX()
        {
            _mudPickerCalendarContentElementId = Identifier.Create();
            Culture = CultureInfo.CurrentCulture;

            using var registerScope = CreateRegisterScope();
            _formatState = registerScope.RegisterParameter<string?>(nameof(Format))
                .WithParameter(() => Format)
                .WithChangeHandler(FormatChangedAsync);
        }

        // 🔥 GENERIC CONVERSION LAYER
        protected DateTime? ToDateTime(T? value)
        {
            if (value == null)
                return null;

            if (value is DateTime dt)
                return dt;

            if (value is DateTimeOffset dto)
                return dto.LocalDateTime;

            throw new NotSupportedException($"Type {typeof(T)} not supported");
        }

        protected T? FromDateTime(DateTime? date)
        {
            if (date == null)
                return default;

            var t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            if (t == typeof(DateTime))
                return (T)(object)date.Value;

            if (t == typeof(DateTimeOffset))
            {
                var offset = TimeZoneInfo.Local.GetUtcOffset(date.Value);
                return (T)(object)new DateTimeOffset(date.Value, offset);
            }

            throw new NotSupportedException($"Type {typeof(T)} not supported");
        }

        protected void ValidateType()
        {
            var t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            if (t != typeof(DateTime) && t != typeof(DateTimeOffset))
                throw new NotSupportedException($"Type {typeof(T)} not supported.");
        }

        protected override void OnInitialized()
        {
            ValidateType();
            base.OnInitialized();
        }

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

        [Inject] protected IScrollManager ScrollManager { get; set; } = null!;
        [Inject] private IJsApiService JsApiService { get; set; } = null!;
        [Inject] protected TimeProvider TimeProvider { get; set; } = null!;

        [Parameter] public DateTime? MaxDate { get; set; }
        [Parameter] public DateTime? MinDate { get; set; }
        [Parameter] public OpenTo OpenTo { get; set; } = OpenTo.Date;

        [Parameter, ParameterState]
        public string? Format { get; set; }

        protected virtual Task FormatChangedAsync(string? newFormat) => Task.CompletedTask;

        private Task FormatChangedAsync(ParameterChangedEventArgs<string?> args)
            => FormatChangedAsync(args.Value);

        [Parameter] public DayOfWeek? FirstDayOfWeek { get; set; }

        internal DateTime? _picker_month;

        /// <summary>
        /// Is set to true to scroll to the actual year after the next render
        /// </summary>
        protected bool _scrollToYearAfterRender = false;

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

        protected internal DateTime? HighlightedDate { get; set; }

        [Parameter] public EventCallback<DateTime?> PickerMonthChanged { get; set; }

        [Parameter] public int ClosingDelay { get; set; } = 100;
        [Parameter] public int DisplayMonths { get; set; } = 1;
        [Parameter] public int? MaxMonthColumns { get; set; }
        [Parameter] public DateTime? StartMonth { get; set; }
        [Parameter] public bool ShowWeekNumbers { get; set; }
        [Parameter] public string TitleDateFormat { get; set; } = "ddd, dd MMM";
        [Parameter] public bool AutoClose { get; set; }

        [Parameter]
        public Func<DateTime, bool> IsDateDisabledFunc { get; set; } = _ => false;

        [Parameter] public Func<DateTime, string>? AdditionalDateClassesFunc { get; set; }
        [Parameter] public string PreviousIcon { get; set; } = Icons.Material.Filled.ChevronLeft;
        [Parameter] public string NextIcon { get; set; } = Icons.Material.Filled.ChevronRight;

        [Parameter] public int? FixYear { get; set; }
        [Parameter] public int? FixMonth { get; set; }
        [Parameter] public int? FixDay { get; set; }

        protected OpenTo CurrentView;

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

        protected override string GetFormat()
        {
            if (!string.IsNullOrWhiteSpace(_formatState.Value))
                return _formatState.Value;

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
        public virtual async Task ScrollToYearAsync(DateTime? date = null)
        {
            
        }

        //private ValueTask HandleMouseoverOnPickerCalendarDayButton(int tempId)
        //{
        //    return JsApiService.UpdateStyleProperty(_mudPickerCalendarContentElementId, "--selected-day", tempId);
        //}
    }
}