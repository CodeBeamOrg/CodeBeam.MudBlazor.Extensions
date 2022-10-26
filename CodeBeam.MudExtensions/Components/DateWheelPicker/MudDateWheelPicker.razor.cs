using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Utilities;
using static MudBlazor.CategoryTypes;

namespace MudExtensions
{
    public partial class MudDateWheelPicker : MudBaseInput<DateTime?>
    {
        protected string Classname =>
           new CssBuilder("mud-input-input-control")
           .AddClass(Class)
           .Build();

        public MudInput<string> InputReference { get; private set; }

        [CascadingParameter(Name = "Standalone")]
        internal bool StandaloneEx { get; set; } = true;

        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public InputType InputType { get; set; } = InputType.Text;

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool SubmitOnClose { get; set; } = true;

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Editable { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ShowHeader { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ShowToolbar { get; set; }

        private string GetCounterText() => Counter == null ? string.Empty : (Counter == 0 ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public DateView DateView { get; set; } = DateView.Date;

        int _day = 1;
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public List<int> Days { get; set; } = Enumerable.Range(1, 31).ToList();

        int _month = 1;
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public List<int> Months { get; set; } = Enumerable.Range(1, 12).ToList();

        int _year = 2000;
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public List<int> Years { get; set; } = Enumerable.Range(1, 9999).ToList();

        int _hour = 0;
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public List<int> Hours { get; set; } = Enumerable.Range(0, 24).ToList();

        int _minute = 0;
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public List<int> Minutes { get; set; } = Enumerable.Range(0, 60).ToList();

        int _second = 0;
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public List<int> Seconds { get; set; } = Enumerable.Range(0, 60).ToList();

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public int MaxHeight { get; set; } = 300;

        internal bool _isOpen;
        internal string _currentIcon { get; set; } = Icons.Filled.CalendarMonth;

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

        protected async Task UpdateValueAsync(bool updateText = true)
        {
            await SetValueAsync(new DateTime(_year, _month, _day, _hour, _minute, _second), updateText);
        }

        public async Task ToggleMenu()
        {
            if (Disabled || ReadOnly)
                return;
            if (_isOpen)
                await CloseMenu(SubmitOnClose);
            else
                await OpenMenu();
        }

        public async Task OpenMenu()
        {
            if (Disabled || ReadOnly)
                return;
            SetWheelValues();
            _isOpen = true;
            if (Editable == true)
            {
                await InputReference.FocusAsync();
            }
            
            StateHasChanged();

        }

        public async Task CloseMenu(bool submit)
        {
            _isOpen = false;
            if (SubmitOnClose || submit)
            {
                await UpdateValueAsync();
            }
        }

        protected void HandleOnBlur()
        {
            SetTextAsync(InputReference.Text, true);
            SetWheelValues();
        }

        protected async Task HandleAdornmentClick()
        {
            await OpenMenu();
            await OnAdornmentClick.InvokeAsync();
        }

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

        protected void SetWheelValues()
        {
            if (Value == null)
            {
                return;
            }
            _day = Value.Value.Day;
            _month = Value.Value.Month;
            _year = Value.Value.Year;
        }

        //public async Task Submit(bool close = true)
        //{
        //    await UpdateValueAsync();
        //    await CloseMenu();
        //}

        public override ValueTask FocusAsync()
        {
            return InputReference.FocusAsync();
        }

        public override ValueTask BlurAsync()
        {
            return InputReference.BlurAsync();
        }

        public override ValueTask SelectAsync()
        {
            return InputReference.SelectAsync();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return InputReference.SelectRangeAsync(pos1, pos2);
        }

        protected override void ResetValue()
        {
            InputReference.Reset();
            base.ResetValue();
        }

        protected string NumberToString(int val)
        {
            if (val < 10)
            {
                return $"0{val}";
            }
            return val.ToString();
        }

        /// <summary>
        /// Clear the text field, set Value to default(T) and Text to null
        /// </summary>
        /// <returns></returns>
        public Task Clear()
        {
            return InputReference.SetText(null);
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

        private async Task OnMaskedValueChanged(string s)
        {
            await SetTextAsync(s);
        }

    }
}
