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
        MudWheel<int> _wheelHour;
        MudWheel<int> _wheelMinute;
        MudWheel<int> _wheelSecond;

        System.Timers.Timer _timer = new();
        Stopwatch _stopwatch = new();

        public async void Elapse(object sender, System.Timers.ElapsedEventArgs args)
        {
            int oldHour = ((int)Value.Value.TotalHours);
            int oldMinute = ((int)Value.Value.TotalMinutes);
            int oldSecond = ((int)Value.Value.TotalSeconds);
            if (Mode == WatchMode.Watch)
            {
                Value = DateTime.Now.TimeOfDay;
                SetInternalValues();
            }
            else if (Mode == WatchMode.CountDown)
            {
                if (Value.Value <= TimeSpan.Zero)
                {
                    await Stop();
                    Value = TimeSpan.Zero;
                    SetInternalValues();
                    await InvokeAsync(StateHasChanged);
                    await InvokeAsync(CountdownCompleted.InvokeAsync);
                    return;
                }

                int oldSecondValue = ((int)Value.Value.TotalSeconds);
                Value = CountdownTime - TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
                SetInternalValues();
                
            }
            else
            {
                int oldSecondValue = ((int)Value.Value.TotalSeconds);
                Value = TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
                SetInternalValues();
            }
#pragma warning disable CS4014
            if (Wheel == true)
            {
                if (oldHour != ((int)Value.Value.TotalHours))
                {
                    // When you await this line, some animates will miss.
                    _wheelHour.RefreshAnimate();
                }

                if (oldMinute != ((int)Value.Value.TotalMinutes))
                {
                    // When you await this line, some animates will miss.
                    _wheelSecond.RefreshAnimate();
                }

                if (oldSecond != ((int)Value.Value.TotalSeconds))
                {
                    // When you await this line, some animates will miss.
                    _wheelSecond.RefreshAnimate();
                }
            }
#pragma warning restore CS4014
            await InvokeAsync(StateHasChanged);
        }

        public async Task Stop()
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
            await InvokeAsync(StateHasChanged);
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
                LapRecords.Clear();
                return;
            }
            _stopwatch.Reset();
            Value = TimeSpan.Zero;
            SetInternalValues();
            LapRecords.Clear();
        }

        public void Start()
        {
            _timer.Interval = Interval.TotalMilliseconds;
            _timer.Start();
            _stopwatch.Start();
        }

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

        protected string Classname =>
           new CssBuilder("mud-input-input-control")
           .AddClass(Class)
           .Build();
        TimeSpan dt;
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

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public EventCallback<LapRecord> LapRecordsChanged { get; set; }

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
