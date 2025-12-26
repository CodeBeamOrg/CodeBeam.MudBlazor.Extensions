// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Services;

namespace MudExtensions.UnitTests.Mocks
{
#pragma warning disable CS1998
#pragma warning disable CS0067
    public class MockJsEventFactory : IJsEventFactory
    {
        private readonly MockJsEvent? _jsEvent;

        public MockJsEventFactory(MockJsEvent jsEvent)
        {
            _jsEvent = jsEvent;
        }

        public MockJsEventFactory()
        {

        }

        public IJsEvent Create() => _jsEvent ?? new MockJsEvent();
    }

    public class MockJsEvent : IJsEvent
    {
#pragma warning disable CS0067
        public event Action<int>? CaretPositionChanged;
        public event Action<string>? Paste;
        public event Action<int, int>? Select;
#pragma warning restore CS0067

        public Task Connect(string element, JsEventOptions options) => Task.CompletedTask;

        public Task Disconnect() => Task.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
