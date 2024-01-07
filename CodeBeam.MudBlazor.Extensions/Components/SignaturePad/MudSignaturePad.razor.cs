using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Utilities;

namespace MudExtensions;

public partial class MudSignaturePad : IAsyncDisposable
{
    public MudSignaturePad()
    {
        _dotnetObjectRef = DotNetObjectReference.Create<MudSignaturePad>(this);
    }

    protected string CanvasContainerClassname => new CssBuilder()
        //.AddClass($"border-solid border-2 mud-border-{Color.ToDescriptionString()}")
        .AddClass(CanvasContainerClass)
        .Build();

    protected string ToolbarClassname => new CssBuilder()
        .AddClass("pa-2 d-flex flex-wrap gap-2")
        .AddClass(ToolbarClass)
        .Build();

    private DotNetObjectReference<MudSignaturePad> _dotnetObjectRef;
    ElementReference _reference;
    bool _isErasing = true;
    int _lineWidth = 3;
    private byte[] _value = Array.Empty<byte>();
    readonly string _id = Guid.NewGuid().ToString();
    string DrawEraseChipText => _isErasing ? LocalizedStrings.Eraser : LocalizedStrings.Pen;
    string DrawEraseChipIcon => _isErasing ? Icons.Material.Filled.Edit : Icons.Material.Filled.EditOff;

    private object JsOptionsStruct => new
    {
        lineWidth = Options.LineWidth,
        lineCap = Options.LineCapStyle.ToString().ToLower(),
        lineJoin = Options.LineJoinStyle.ToString().ToLower(),
        strokeStyle = Options.StrokeStyle.Value
    };

    [Parameter]
    public byte[] Value
    {
        get => _value;
        set
        {
            if (value == _value) return;

            _value = value;
        }
    }

    [Parameter] public EventCallback<byte[]> ValueChanged { get; set; }
    [Parameter] public SignaturePadLocalizedStrings LocalizedStrings { get; set; } = new();
    [Parameter] public SignaturePadOptions Options { get; set; } = new SignaturePadOptions();

    [Parameter] public string ToolbarClass { get; set; }
    [Parameter] public string ToolbarStyle { get; set; }
    [Parameter] public string OuterClass { get; set; }
    [Parameter] public int Elevation { get; set; } = 4;
    [Parameter] public string CanvasContainerClass { get; set; }
    [Parameter] public string CanvasContainerStyle { get; set; } = "height: 100%;width: 100%; box-shadow: rgb(204, 219, 232) 3px 3px 6px 0px inset, rgba(255, 255, 255, 0.5) -3px -3px 6px 1px inset;";
    [Parameter] public bool ShowClear { get; set; } = true;
    [Parameter] public bool ShowLineWidth { get; set; } = true;
    [Parameter] public bool ShowStrokeStyle { get; set; } = true;
    [Parameter] public bool ShowDownload { get; set; } = true;
    [Parameter] public bool ShowLineJoinStyle { get; set; } = true;
    [Parameter] public bool ShowLineCapStyle { get; set; } = true;
    [Parameter] public bool Dense { get; set; }
    [Parameter] public Variant Variant { get; set; }
    [Parameter] public Color Color { get; set; }
    [Parameter] public RenderFragment ToolbarContent { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("mudSignaturePad.addPad", _dotnetObjectRef, _reference, JsOptionsStruct);
            if (Value.Length > 0)
            {
                await PushImageUpdateToJsRuntime();
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task IsEditToggled()
    {
        await JsRuntime.InvokeVoidAsync("mudSignaturePad.togglePadEraser", _reference);
        _isErasing = !_isErasing;
    }

    async Task ClearPad()
    {
        await ValueChanged.InvokeAsync(Array.Empty<byte>());
        await JsRuntime.InvokeVoidAsync("mudSignaturePad.clearPad", _reference);
    }

    async Task PushImageUpdateToJsRuntime()
    {
        await JsRuntime.InvokeVoidAsync("mudSignaturePad.updatePadImage", _reference, Convert.ToBase64String(Value));
    }

    async Task UpdateOptions()
    {
        await JsRuntime.InvokeVoidAsync("mudSignaturePad.updatePadOptions", _reference, JsOptionsStruct);
    }

    async Task Download()
    {
        await JsRuntime.InvokeVoidAsync("mudSignaturePad.downloadPadImage", _reference);
    }

    private async Task LineWidthUpdated(decimal obj)
    {
        Options.LineWidth = obj;
        await UpdateOptions();
    }

    private async Task StrokeStyleUpdated(MudColor obj)
    {
        Options.StrokeStyle = obj;
        await UpdateOptions();
    }

    private async Task LineJoinTypeUpdated(LineJoinTypes obj)
    {
        Options.LineJoinStyle = obj;
        await UpdateOptions();
    }

    private async Task LineCapTypeUpdated(LineCapTypes obj)
    {
        Options.LineCapStyle = obj;
        await UpdateOptions();
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("mudSignaturePad.disposePad", _reference);
        }
        catch
        {
            //ignore
        }
    }

    [JSInvokable]
    public async Task SignatureDataChangedAsync()
    {
        var base64Data = await JsRuntime.InvokeAsync<string>("mudSignaturePad.getBase64", _reference);
        try
        {
            Value = Convert.FromBase64String(base64Data.Replace("data:image/png;base64,", ""));
        }
        catch (Exception)
        {
            Value = Array.Empty<byte>();
        }

        await ValueChanged.InvokeAsync(Value);
    }
}
