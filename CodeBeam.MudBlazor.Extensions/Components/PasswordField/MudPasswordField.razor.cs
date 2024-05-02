using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// Mud input component with enhanced password features.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class MudPasswordField<T> : MudDebouncedInput<T>
    {
        /// <summary>
        /// 
        /// </summary>
        protected string? Classname =>
           new CssBuilder("mud-input-input-control")
           .AddClass(Class)
           .Build();

        /// <summary>
        /// 
        /// </summary>
        public MudInputExtended<string?> InputReference { get; private set; } = new();
        InputType _passwordInput = InputType.Password;
        string? _passwordIcon = Icons.Material.Filled.VisibilityOff;
        bool _passwordMode = true;

        [CascadingParameter(Name = "Standalone")]
        internal bool StandaloneEx { get; set; } = true;

        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public InputType InputType { get; set; } = InputType.Text;

        private string GetCounterText() => Counter == null ? string.Empty : (Counter == 0 ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// If true disable paste to the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool DisablePaste { get; set; }

        /// <summary>
        /// If true, adornment button accepts tab stop. Default is false.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool AdornmentTabStop { get; set; }

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// RenderFragment for start adornment.
        /// </summary>
        [Parameter] public RenderFragment? AdornmentStart { get; set; }

        /// <summary>
        /// RenderFragment for end adornment.
        /// </summary>
        [Parameter] public RenderFragment? CustomAdornment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ValueTask FocusAsync()
        {
            return InputReference.FocusAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ValueTask BlurAsync()
        {
            return InputReference.BlurAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ValueTask SelectAsync()
        {
            return InputReference.SelectAsync();
        }

        /// <summary>
        /// 
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
        /// <returns></returns>
        protected override async Task ResetValueAsync()
        {
            await InputReference.ResetAsync();
            await base.ResetValueAsync();
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

        /// <summary>
        /// If true, masks text with password mode.
        /// </summary>
        [Parameter]
        public bool PasswordMode
        {
            get => _passwordMode;
            set
            {
                if (_passwordMode == value)
                {
                    return;
                }
                _passwordMode = value;
                if (_passwordMode)
                {
                    _passwordInput = InputType.Password;
                    _passwordIcon = Icons.Material.Filled.VisibilityOff;
                }
                else
                {
                    _passwordInput = InputType.Text;
                    _passwordIcon = Icons.Material.Filled.Visibility;
                }

                PasswordModeChanged.InvokeAsync(value).AndForget();
            }
        }

        /// <summary>
        /// Fires when password mode changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> PasswordModeChanged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task AdornmentClick()
        {
            PasswordMode = !PasswordMode;
            await OnAdornmentClick.InvokeAsync();
        }

    }
}
