using System.Threading.Tasks;
using MudBlazor;
using MudBlazor.Services;

namespace CodeBeam.MudExtensions.UnitTests.Mocks
{
    public class MockBrowserWindowSizeProvider : IBrowserWindowSizeProvider
    {
        public ValueTask<BrowserWindowSize> GetBrowserWindowSize()
        {
            return new ValueTask<BrowserWindowSize>(new BrowserWindowSize());
        }
    }
}
