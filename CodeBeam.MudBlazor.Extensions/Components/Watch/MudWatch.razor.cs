using System.Diagnostics;
using MudExtensions.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MudWatch : MudComponentBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected string? Classname => new CssBuilder("d-flex mud-width-full align-end justify-center")
            .AddClass(Class)
            .Build();

        MudWheel<int> _wheelDay = new();
        MudWheel<int> _wheelHour = new();
        MudWheel<int> _wheelMinute = new();
        MudWheel<int> _wheelSecond = new();

        System.Timers.Timer _timer = new();
        Stopwatch _stopwatch = new();

        /// <summary>
        /// 
        /// </summary>
        protected override void OnInitialized()
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

            if (Mode == WatchMode.Watch)
            {
                SetWatchMode(Mode).AndForget();
                Start();
            }
            if (Mode == WatchMode.StopWatch)
            {
                _initialValue = Value;
            }
        }

        TimeSpan _initialValue = new();

        TimeSpan _value;
        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public TimeSpan Value 
        {
            get => _value; 
            set
            {
                if (_value == value)
                {
                    return;
                }
                _value = value;
                InvokeAsync(() => ValueChanged.InvokeAsync(_value)).AndForget();
                SetInternalValues();
            }
        }

        TimeSpan _interval = TimeSpan.FromSeconds(1);
        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public TimeSpan Interval 
        { 
            get => _interval; 
            set
            {
                if (_interval == value)
                {
                    return;
                }
                _interval = value;
                _timer.Interval = _interval.TotalMilliseconds;
            }
        }

        WatchMode _watchMode = WatchMode.Watch;
        /// <summary>
        /// 
        /// </summary>
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
                SetWatchMode(_watchMode).AndForget();
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
        /// If true, components shows days. Default is false.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool ShowDay { get; set; } = false;

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
        /// Fires when value changed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public EventCallback<TimeSpan> ValueChanged { get; set; }

        /// <summary>
        /// Fires when countdown reach to 0.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public EventCallback CountdownCompleted { get; set; }

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

        int _day = 0;
        internal List<int> Days { get; set; } = Enumerable.Range(0, 3600).ToList();

        int _hour = 0;
        internal List<int> Hours { get; set; } = Enumerable.Range(0, 24).ToList();

        int _minute = 0;
        internal List<int> Minutes { get; set; } = Enumerable.Range(0, 60).ToList();

        int _second = 0;
        internal List<int> Seconds { get; set; } = Enumerable.Range(0, 60).ToList();

        int _milliSecond = 0;
        internal List<int> MilliSeconds { get; set; } = Enumerable.Range(0, 999).ToList();


        /// <summary>
        /// The records that builds with Lap method or created manually.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public List<LapRecord> LapRecords { get; set; } = new();

        /// <summary>
        /// Fires when new lap record added.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public EventCallback<LapRecord> LapRecordsChanged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void Elapse(object sender, System.Timers.ElapsedEventArgs args)
        {
            int oldHour = ((int)Value.TotalHours);
            int oldMinute = ((int)Value.TotalMinutes);
            int oldSecond = ((int)Value.TotalSeconds);
            if (Mode == WatchMode.Watch)
            {
                Value = DateTime.Now.TimeOfDay;
            }
            else if (Mode == WatchMode.CountDown)
            {
                if (Value <= TimeSpan.Zero)
                {
                    await Stop();
                    Value = TimeSpan.Zero;
                    await InvokeAsync(StateHasChanged);
                    await InvokeAsync(CountdownCompleted.InvokeAsync);
                    return;
                }

                int oldSecondValue = ((int)Value.TotalSeconds);
                Value = CountdownTime - TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
            }
            else
            {
                int oldSecondValue = ((int)Value.TotalSeconds);
                Value = _initialValue + TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);

            }
#pragma warning disable CS4014
            if (Wheel)
            {
                if (oldHour != ((int)Value.TotalHours))
                {
                    // When you await this line, some animates will miss.
                    _wheelHour.RefreshAnimate();
                }

                if (oldMinute != ((int)Value.TotalMinutes))
                {
                    // When you await this line, some animates will miss.
                    _wheelSecond.RefreshAnimate();
                }

                if (oldSecond != ((int)Value.TotalSeconds))
                {
                    // When you await this line, some animates will miss.
                    _wheelSecond.RefreshAnimate();
                }
            }
#pragma warning restore CS4014
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task Stop()
        {
            _timer.Stop();
            _stopwatch.Stop();
            if (Mode == WatchMode.CountDown)
            {
                Value = CountdownTime - TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
                if (Value < TimeSpan.Zero)
                {
                    Value = TimeSpan.Zero;
                }
            }
            else if (Mode == WatchMode.StopWatch)
            {
                Value = _initialValue + TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
            }

            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task Reset()
        {
            if (Mode == WatchMode.Watch)
            {
                return;
            }
            else if (Mode == WatchMode.CountDown)
            {
                _stopwatch.Reset();
                Value = CountdownTime;
                LapRecords.Clear();
                await LapRecordsChanged.InvokeAsync();
                return;
            }
            else
            {
                _stopwatch.Reset();
                Value = _initialValue;
                LapRecords.Clear();
                await LapRecordsChanged.InvokeAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            _timer.Interval = Interval.TotalMilliseconds;
            _timer.Start();
            _stopwatch.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public async void Lap()
        {
            if (LapRecords == null)
            {
                LapRecords = new();
            }
            LapRecords.Add(new LapRecord()
            {
                TotalTime = TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds),
                Gap = LapRecords.Count == 0 ? TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds) : TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds) - LapRecords.LastOrDefault().TotalTime,
            });
            await LapRecordsChanged.InvokeAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        protected async Task SetWatchMode(WatchMode mode)
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
                await Reset();
                ShowHour = true;
                ShowMinute = true;
                ShowSecond = true;
                ShowMillisecond = true;
            }
            else if (mode == WatchMode.CountDown)
            {
                Interval = TimeSpan.FromMilliseconds(1);
                await Reset();
                ShowHour = true;
                ShowMinute = true;
                ShowSecond = true;
                ShowMillisecond = true;
            }
            StateHasChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SetInternalValues()
        {
            _day = Value.Days;
            _hour = Value.Hours;
            _minute = Value.Minutes;
            _second = Value.Seconds;
            _milliSecond = Value.Milliseconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetWatchText()
        {
            List<string> ls = new();
            if (ShowDay)
            {
                ls.Add(_day.ToString("D2"));
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected string? NumberToString(int val)
        {
            if (val < 10)
            {
                return $"0{val}";
            }
            return val.ToString();
        }

    }
}
