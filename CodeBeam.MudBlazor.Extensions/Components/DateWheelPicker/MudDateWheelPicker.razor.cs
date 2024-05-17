using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MudDateWheelPicker : MudBaseInput<DateTime?>
    {
        /// <summary>
        /// 
        /// </summary>
        protected string? Classname =>
           new CssBuilder("mud-input-input-control")
           .AddClass(Class)
           .Build();
        DateTime dt;
        /// <summary>
        /// 
        /// </summary>
        protected override void OnInitialized()
        {
            Converter = new MudBlazor.Converter<DateTime?, string>()
            {
                SetFunc = x => x?.ToString(DateFormat),
                GetFunc = x => DateTime.TryParseExact(x, DateFormat, null, System.Globalization.DateTimeStyles.None, out dt) ? dt : null,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public MudInput<string?> InputReference { get; private set; } = new();

        [CascadingParameter(Name = "Standalone")]
        internal bool StandaloneEx { get; set; } = true;

        /// <summary>
        /// CSS classes for popover content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? PopoverClass { get; set; }

        /// <summary>
        /// CSS styles for popover content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? PopoverStyle { get; set; }

        /// <summary>
        /// The date format that determines the text and wheel order. Default is Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string DateFormat { get; set; } = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern;

        /// <summary>
        /// If false, users have to click the "done" button to submit value before close popover. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool SubmitOnClose { get; set; } = true;

        /// <summary>
        /// The minimum swipe delta to change wheel value on touch devices. Default is 30.
        /// </summary>
        [Parameter]
        public int Sensitivity { get; set; } = 30;

        /// <summary>
        /// If true, the year wheel is disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool FixYear { get; set; }

        /// <summary>
        /// If true, the month wheel is disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool FixMonth { get; set; }

        /// <summary>
        /// If true, the hour wheel is disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool FixHour { get; set; }

        /// <summary>
        /// If true, the minute wheel is disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool FixMinute { get; set; }

        /// <summary>
        /// If true, the second wheel is disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool FixSecond { get; set; }

        /// <summary>
        /// If true, the day wheel is disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool FixDay { get; set; }

        /// <summary>
        /// If true, only adornment click opens popover and users can directly write to the input.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Editable { get; set; }

        /// <summary>
        /// Determines the wheels are dense or not.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The color of various parts of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color Color { get; set; }

        /// <summary>
        /// The time wheels (hour, minute and second) color. If its inherit (by default) it takes the Color value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color ColorTime { get; set; } = Color.Inherit;

        /// <summary>
        /// Show wheel labels.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool ShowHeader { get; set; }

        /// <summary>
        /// Show toolbar that allows change between DateViews dynamically.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool ShowToolbar { get; set; }

        /// <summary>
        /// A class for provide all local strings at once.
        /// </summary>
        [Parameter]
        public DateWheelPickerLocalizedStrings LocalizedStrings { get; set; } = new();

        private string GetCounterText() => Counter == null ? string.Empty : (Counter == 0 ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// Determines the view of wheels.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public DateView DateView { get; set; } = DateView.Date;

        int _day = 1;
        /// <summary>
        /// Displayed number range as days.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public List<int> Days { get; set; } = Enumerable.Range(1, 31).ToList();

        int _month = 1;
        /// <summary>
        /// Displayed number range as months.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public List<int> Months { get; set; } = Enumerable.Range(1, 12).ToList();

        int _year = DateTime.Today.Year;
        /// <summary>
        /// Displayed number range as years.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public List<int> Years { get; set; } = Enumerable.Range(1, 9999).ToList();

        int _hour = 0;
        /// <summary>
        /// Displayed number range as hours.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public List<int> Hours { get; set; } = Enumerable.Range(0, 24).ToList();

        int _minute = 0;
        /// <summary>
        /// Displayed number range as minutes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public List<int> Minutes { get; set; } = Enumerable.Range(0, 60).ToList();

        int _second = 0;
        /// <summary>
        /// Displayed number range as seconds.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public List<int> Seconds { get; set; } = Enumerable.Range(0, 60).ToList();

        /// <summary>
        /// Max height of popover content. Default is 300.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public int MaxHeight { get; set; } = 300;

        internal bool _isOpen;
        internal string _currentIcon { get; set; } = Icons.Material.Filled.CalendarMonth;

        /// <summary>
        /// Sets the anchor origin point for the popover.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public Origin AnchorOrigin { get; set; } = Origin.TopCenter;

        /// <summary>
        /// Sets the transform origin point for the popover.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopCenter;

        /// <summary>
        /// If true, prevent scrolling while dropdown is open.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool LockScroll { get; set; } = false;

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateText"></param>
        /// <returns></returns>
        protected async Task UpdateValueAsync(bool updateText = true)
        {
            var _backUpValue = Value;
            try
            {
                await SetValueAsync(new DateTime(_year, _month, _day, _hour, _minute, _second), updateText);
            }
            catch
            {
                await SetValueAsync(_backUpValue, updateText);
            }
        }

        /// <summary>
        /// Toggles the menu.
        /// </summary>
        /// <returns></returns>
        public async Task ToggleMenu()
        {
            if (Disabled || ReadOnly)
                return;
            if (_isOpen)
                await CloseMenu(SubmitOnClose);
            else
                await OpenMenu();
        }

        /// <summary>
        /// Opens the menu.
        /// </summary>
        /// <returns></returns>
        public async Task OpenMenu()
        {
            if (Disabled || ReadOnly)
                return;
            SetWheelValues();
            _isOpen = true;
            if (Editable)
            {
                await InputReference.FocusAsync();
            }

            StateHasChanged();
        }

        /// <summary>
        /// Closes the menu.
        /// </summary>
        /// <param name="submit"></param>
        /// <returns></returns>
        public async Task CloseMenu(bool submit)
        {
            _isOpen = false;
            if (SubmitOnClose || submit)
            {
                await UpdateValueAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task HandleOnBlur()
        {
            await SetTextAsync(InputReference.Text, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task HandleAdornmentClick()
        {
            await OpenMenu();
            await OnAdornmentClick.InvokeAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateView"></param>
        protected void ExpandDateView(DateView dateView = DateView.Date)
        {
            if (DateView == DateView.Both)
            {
                DateView = dateView;
            }
            else
            {
                DateView = DateView.Both;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ToggleDateView()
        {
            if (DateView == DateView.Date)
            {
                DateView = DateView.Time;
            }
            else
            {
                DateView = DateView.Date;
            }
        }

        private void RefreshDays()
        {
            var month = _month;
            var year = _year;
            var day = _day;

            ++month;

            if (month == 13)
            {
                month = 1;
                ++year;
            }

            Days = Enumerable.Range(1, new DateTime(year, month, 1).AddDays(-1).Day).ToList();
            if (Days.Last() < _day)
            {
                _day = Days.Last();
            }
        }

        private void OnMonthChanged(int month)
        {
            _month = month;

            RefreshDays();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SetWheelValues()
        {
            if (Value == null)
            {
                return;
            }
            _day = Value.Value.Day;
            _month = Value.Value.Month;
            _year = Value.Value.Year;
            _hour = Value.Value.Hour;
            _minute = Value.Value.Minute;
            _second = Value.Value.Second;

            RefreshDays();
        }

        /// <summary>
        /// Focuses the component.
        /// </summary>
        /// <returns></returns>
        public override ValueTask FocusAsync()
        {
            return InputReference.FocusAsync();
        }

        /// <summary>
        /// Blur from the component.
        /// </summary>
        /// <returns></returns>
        public override ValueTask BlurAsync()
        {
            return InputReference.BlurAsync();
        }

        /// <summary>
        /// Focus and select all text.
        /// </summary>
        /// <returns></returns>
        public override ValueTask SelectAsync()
        {
            return InputReference.SelectAsync();
        }

        /// <summary>
        /// Focus and select partial text show with positions.
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return InputReference.SelectRangeAsync(pos1, pos2);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override async Task ResetValueAsync()
        {
            await InputReference.ResetAsync();
            await base.ResetValueAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool HasSeconds()
        {
            if (DateFormat.Contains("s"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected string NumberToString(int val)
        {
            return $"{val:00}";
        }

        /// <summary>
        /// Clear the text field, set Value to default(T) and Text to null
        /// </summary>
        /// <returns></returns>
        public async Task Clear()
        {
            await SetValueAsync(null);
            await InputReference.SetText(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task HandleClearButtonClick()
        {
            await Clear();
            await OnClearButtonClick.InvokeAsync();
        }

        /// <summary>
        /// Sets the input text from outside programmatically
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task SetText(string text)
        {
            if (InputReference != null)
                await InputReference.SetText(text);
        }

    }
}
