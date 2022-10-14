using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
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

        protected string OuterItemClassname(int index) => new CssBuilder($"mud-wheel-item mud-wheel-ani-{_animateGuid}")
            .AddClass("wheel-item-closest", Math.Abs(Items.IndexOf(Value) - index) == 1)
            .AddClass("my-1", Dense == false)
            .Build();

        protected string BorderClassname => new CssBuilder("mud-wheel-border mud-wheel-item mud-width-full")
            .AddClass("wheel-border-gradient", SmoothBorders)
            .AddClass("my-1", Dense == false)
            .Build();

        protected string EmptyItemClassname => new CssBuilder("mud-wheel-ani-{_animateGuid} mud-wheel-item wheel-item-empty")
            .AddClass("wheel-border-gradient", SmoothBorders)
            .AddClass("my-1", Dense == false)
            .AddClass("wheel-item-empty-dense", Dense == true)
            .Build();

        MudAnimate _animate;
        Guid _animateGuid = Guid.NewGuid();
        int _animateValue = 52;
        bool _mouseMiddle = true;

        [Parameter]
        public List<T> Items { get; set; }

        [Parameter]
        public int WheelLevel { get; set; } = 2;

        [Parameter]
        public bool Dense { get; set; }

        [Parameter]
        public bool SmoothBorders { get; set; }

        protected string GetStylename()
        {
            return new StyleBuilder()
            .AddStyle("height", $"{(WheelLevel * (Dense ? 24 : 40) * 2) + (Dense ? 32 : 46)}px")
            .AddStyle(Style)
            .Build();
        }

        protected async Task HandleOnWheel(WheelEventArgs args)
        {
            int index = GetIndex();
            if ((args.DeltaY < 0 && index == 0) || (0 < args.DeltaY && index == Items.Count - 1))
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
                T val = Items[index - 1];
                await SetValueAsync(val);
            }
            else if (0 < args.DeltaY && index != Items.Count - 1)
            {
                T val = Items[index + 1];
                await SetValueAsync(val);
            }
            await Task.Delay(300);
        }

        protected async Task HandleOnSwipe(SwipeDirection direction)
        {
            int index = GetIndex();
            if ((direction == SwipeDirection.TopToBottom && index == 0) || (direction == SwipeDirection.BottomToTop && index == Items.Count - 1))
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
                T val = Items[index - 1];
                await SetValueAsync(val);
            }
            else if (direction == SwipeDirection.BottomToTop)
            {
                T val = Items[index + 1];
                await SetValueAsync(val);
            }

        }

        protected async Task ChangeWheel(int changeCount)
        {
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
            T val = Items[index + changeCount];
            await SetValueAsync(val);
        }

        protected int GetIndex() => Items.IndexOf(Value) == -1 ? 0 : Items.IndexOf(Value);
        protected int GetAnimateValue() => Dense ? 24 : 46;
    }
}
