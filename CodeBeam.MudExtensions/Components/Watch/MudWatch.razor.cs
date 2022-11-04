using System;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Timers;
using CodeBeam.MudExtensions.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Enums;
using static MudBlazor.CategoryTypes;

namespace MudExtensions
{
    public partial class MudWatch : MudBaseInput<TimeSpan?>
    {
        MudWheel<int> _wheelMinute;
        MudWheel<int> _wheelSecond;

        PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
        System.Timers.Timer _timer = new();
        Stopwatch _stopwatch = new();

        public void ExecuteAsync()
        {
            _timer.Interval = Interval.TotalMilliseconds;
            _timer.Start();
            _stopwatch.Start();
            


            //timer = new PeriodicTimer(Interval);
            
            //sw.Start();
            //try
            //{
            //    while (await timer.WaitForNextTickAsync())
            //    {
            //        Value = Value.Value.Add(Increment);
            //        SetWheelValues();
            //        //await _wheelSecond.RefreshAnimate();
            //        await InvokeAsync(StateHasChanged);
            //    }
            //    sw.Stop();
            //    Value = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
            //    SetWheelValues();
            //    await InvokeAsync(StateHasChanged);
            //    timer.Dispose();

            //}
            //catch
            //{

            //}

        }

        public async void Elapse(object sender, System.Timers.ElapsedEventArgs args)
        {
            if (Mode == WatchMode.Watch)
            {
                Value = DateTime.Now.TimeOfDay;
                SetInternalValues();
            }
            else if (Mode == WatchMode.CountDown)
            {
                int oldSecondValue = ((int)Value.Value.TotalSeconds);
                Value = CountdownTime - TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
                SetInternalValues();
                if (Wheel == true)
                {
                    if (oldSecondValue != ((int)Value.Value.TotalSeconds))
                    {
                        // When you await this line, some animates will miss.
                        _wheelSecond.RefreshAnimate();
                    }
                }
            }
            else
            {
                int oldSecondValue = ((int)Value.Value.TotalSeconds);
                Value = TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
                SetInternalValues();
                if (Wheel == true)
                {
                    if (oldSecondValue != ((int)Value.Value.TotalSeconds))
                    {
                        // When you await this line, some animates will miss.
                        _wheelSecond.RefreshAnimate();
                    }
                }
            }

            await InvokeAsync(StateHasChanged);
        }

        public void Stop()
        {
            _timer.Stop();
            _stopwatch.Stop();
            if (Mode == WatchMode.CountDown)
            {
                Value = CountdownTime - TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
            }
            else if (Mode == WatchMode.StopWatch)
            {
                Value = TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
            }

            SetInternalValues();
            StateHasChanged();
        }

        public void ResetWatch()
        {
            if (Mode == WatchMode.Watch)
            {
                return;
            }
            else if (Mode == WatchMode.CountDown)
            {
                _stopwatch.Reset();
                Value = CountdownTime;
                SetInternalValues();
                return;
            }
            _stopwatch.Reset();
            Value = TimeSpan.Zero;
            SetInternalValues();
        }

        public void Start()
        {
            ExecuteAsync();
        }

        protected string Classname =>
           new CssBuilder("mud-input-input-control")
           .AddClass(Class)
           .Build();
        TimeSpan dt;
        protected override async void OnInitialized()
        {
            _timer.Elapsed += Elapse;
            if (Mode == WatchMode.Watch)
            {
                Value = DateTime.Now.TimeOfDay;
            }
            else if (Mode == WatchMode.CountDown)
            {
                Value = CountdownTime;
            }
            else if (Mode == WatchMode.StopWatch)
            {
                Value = TimeSpan.FromSeconds(0);
            }
            //Converter = new MudBlazor.Converter<TimeSpan?, string>()
            //{
            //    SetFunc = x => x?.ToString(),
            //    GetFunc = x => TimeSpan.TryParseExact(x, DateFormat, null, System.Globalization.TimeSpanStyles.None, out dt) == true ? dt : null,
            //};

            if (Mode == WatchMode.Watch)
            {
                SetWatchMode(Mode);
                Start();
            }
        }

        public MudInput<string> InputReference { get; private set; }

        [CascadingParameter(Name = "Standalone")]
        internal bool StandaloneEx { get; set; } = true;

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public TimeSpan Interval { get; set; } = TimeSpan.FromMilliseconds(100);

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public TimeSpan Increment { get; set; } = TimeSpan.FromMilliseconds(100);

        WatchMode _watchMode = WatchMode.Watch;
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public WatchMode Mode 
        {
            get => _watchMode;
            set
            {
                if (_watchMode == value)
                {
                    return;
                }
                _watchMode = value;
                SetWatchMode(_watchMode);
            }
        }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public TimeSpan CountdownTime { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// The delimiter string that seperates hour, minute, second and millisecond values.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string Delimiter { get; set; } = ":";

        /// <summary>
        /// If true, components shows hours. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool ShowHour { get; set; } = true;

        /// <summary>
        /// If true, components shows minutes. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool ShowMinute { get; set; } = true;

        /// <summary>
        /// If true, components shows seconds. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool ShowSecond { get; set; } = true;

        /// <summary>
        /// If true, components shows milliseconds. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool ShowMillisecond { get; set; } = true;

        /// <summary>
        /// If true, components shows MudWheels.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Wheel { get; set; }

        /// <summary>
        /// If true, components shows MudWheels.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Typo Typo { get; set; } = Typo.h6;

        /// <summary>
        /// If true, components shows MudWheels.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Typo TypoMillisecond { get; set; } = Typo.h6;

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

        int _milliSecond = 0;
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public List<int> MilliSeconds { get; set; } = Enumerable.Range(0, 999).ToList();

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
            var _backUpValue = Value;
            try
            {
                await SetValueAsync(new TimeSpan(_hour, _minute, _second), updateText);
            }
            catch
            {
                await SetValueAsync(_backUpValue, updateText);
            }
        }  

        protected async Task HandleOnBlur()
        {
            await SetTextAsync(InputReference.Text, true);
        }

        protected void SetWatchMode(WatchMode mode)
        {
            if (mode == WatchMode.Watch)
            {
                Interval = TimeSpan.FromSeconds(1);
                Value = DateTime.Now.TimeOfDay;
                ShowHour = true;
                ShowMinute = true;
                ShowSecond = true;
                ShowMillisecond = false;
            }
            else if (mode == WatchMode.StopWatch)
            {
                Interval = TimeSpan.FromMilliseconds(1);
                Value = TimeSpan.Zero;
                ResetWatch();
                ShowHour = true;
                ShowMinute = true;
                ShowSecond = true;
                ShowMillisecond = true;
            }
            else if (mode == WatchMode.CountDown)
            {
                Interval = TimeSpan.FromMilliseconds(1);
                ResetWatch();
                ShowHour = true;
                ShowMinute = true;
                ShowSecond = true;
                ShowMillisecond = true;
            }
        }

        protected void SetInternalValues()
        {
            if (Value == null)
            {
                return;
            }
            _hour = Value.Value.Hours;
            _minute = Value.Value.Minutes;
            _second = Value.Value.Seconds;
            _milliSecond = Value.Value.Milliseconds;
        }

        protected override void ResetValue()
        {
            InputReference.Reset();
            base.ResetValue();
        }

        protected string GetWatchText()
        {
            List<string> ls = new();
            if (ShowHour)
            {
                ls.Add(_hour.ToString("D2"));
            }
            if (ShowMinute)
            {
                ls.Add(_minute.ToString("D2"));
            }
            if (ShowSecond)
            {
                ls.Add(_second.ToString("D2"));
            }
            if (ShowMillisecond)
            {
                ls.Add("");
            }
            return string.Join(" " + Delimiter + " ", ls);
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
        public async Task Clear()
        {
            await SetValueAsync(null);
            await InputReference.SetText(null);
        }

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
