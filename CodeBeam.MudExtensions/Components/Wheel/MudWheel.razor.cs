using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudExtensions
{
    public partial class MudWheel<T> : MudBaseInput<T>
    {

        [Inject] public IScrollManager ScrollManager { get; set; }

        protected string Classname => new CssBuilder("mud-width-full")
            .AddClass(Class)
            .Build();

        protected string InnerClassname => new CssBuilder("mud-wheel d-flex flex-column align-center justify-center relative")
            .AddClass("mud-disabled", Disabled)
            .AddClass(InnerClass)
            .Build();

        protected string MiddleItemClassname => new CssBuilder("middle-item d-flex align-center justify-center mud-width-full")
            .AddClass("mud-disabled", Disabled)
            .Build();

        protected string OuterItemClassname(int index) => new CssBuilder($"mud-wheel-item mud-wheel-ani-{_animateGuid}")
            .AddClass("wheel-item-closest", Math.Abs(ItemCollection.IndexOf(Value) - index) == 1)
            .AddClass("my-1", Dense == false)
            .AddClass("mud-disabled", Disabled)
            .Build();

        protected string BorderClassname => new CssBuilder("mud-wheel-border mud-wheel-item mud-width-full")
            .AddClass($"mud-wheel-border-{Color.ToDescriptionString()}")
            .AddClass($"wheel-border-gradient-{Color.ToDescriptionString()}", SmoothBorders)
            .AddClass("my-1", Dense == false)
            .Build();

        protected string EmptyItemClassname => new CssBuilder("mud-wheel-ani-{_animateGuid} mud-wheel-item wheel-item-empty")
            .AddClass("my-1", Dense == false)
            .AddClass("wheel-item-empty-dense", Dense == true)
            .Build();

        MudAnimate _animate;
        Guid _animateGuid = Guid.NewGuid();
        int _animateValue = 52;

        [Parameter]
        public List<T> ItemCollection { get; set; }

        [Parameter]
        public int WheelLevel { get; set; } = 2;

        [Parameter]
        public string InnerClass { get; set; }

        [Parameter]
        public bool Dense { get; set; }

        [Parameter]
        public bool SmoothBorders { get; set; }

        [Parameter]
        public Color Color { get; set; }

        private Func<T, string> _toStringFunc = x => x?.ToString();
        /// <summary>
        /// Defines how values are displayed in the drop-down list
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T, string> ToStringFunc
        {
            get => _toStringFunc;
            set
            {
                if (_toStringFunc == value)
                    return;
                _toStringFunc = value;
                Converter = new Converter<T>
                {
                    SetFunc = _toStringFunc ?? (x => x?.ToString()),
                };
            }
        }

        protected string GetStylename()
        {
            return new StyleBuilder()
            .AddStyle("height", $"{(WheelLevel * (Dense ? 24 : 40) * 2) + (Dense ? 32 : 46)}px")
            .AddStyle(Style)
            .Build();
        }

        protected async Task HandleOnWheel(WheelEventArgs args)
        {
            if (Disabled || ReadOnly)
            {
                return;
            }
            int index = GetIndex();
            if ((args.DeltaY < 0 && index == 0) || (0 < args.DeltaY && index == ItemCollection.Count - 1))
            {
                return;
            }

            if (0 < args.DeltaY)
            {
                _animateValue = GetAnimateValue();
            }
            else
            {
                _animateValue = - GetAnimateValue();
            }
            await _animate.Refresh();
            if (args.DeltaY < 0 && index != 0)
            {
                T val = ItemCollection[index - 1];
                await SetValueAsync(val);
            }
            else if (0 < args.DeltaY && index != ItemCollection.Count - 1)
            {
                T val = ItemCollection[index + 1];
                await SetValueAsync(val);
            }
            await Task.Delay(300);
        }

        protected async Task HandleOnSwipe(SwipeDirection direction)
        {
            if (Disabled || ReadOnly)
            {
                return;
            }
            int index = GetIndex();
            if ((direction == SwipeDirection.TopToBottom && index == 0) || (direction == SwipeDirection.BottomToTop && index == ItemCollection.Count - 1))
            {
                return;
            }
            if (direction == SwipeDirection.BottomToTop)
            {
                _animateValue = GetAnimateValue();
            }
            else
            {
                _animateValue = - GetAnimateValue();
            }
            await _animate.Refresh();
            if (direction == SwipeDirection.TopToBottom)
            {
                T val = ItemCollection[index - 1];
                await SetValueAsync(val);
            }
            else if (direction == SwipeDirection.BottomToTop)
            {
                T val = ItemCollection[index + 1];
                await SetValueAsync(val);
            }

        }

        public async Task ChangeWheel(int changeCount)
        {
            if (Disabled || ReadOnly)
            {
                return;
            }
            int index = GetIndex();
            if (0 < changeCount)
            {
                _animateValue = GetAnimateValue();
            }
            else
            {
                _animateValue = - GetAnimateValue();
            }
            await _animate.Refresh();
            T val = ItemCollection[index + changeCount];
            await SetValueAsync(val);
        }

        public async Task RefreshAnimate()
        {
            await _animate.Refresh();
        }

        protected int GetIndex() => ItemCollection.IndexOf(Value) == -1 ? 0 : ItemCollection.IndexOf(Value);
        protected int GetAnimateValue() => Dense ? 24 : 42;
    }
}
