using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions;

public partial class MudSignaturePad : IAsyncDisposable
{
    public MudSignaturePad()
    {
        _dotnetObjectRef = DotNetObjectReference.Create<MudSignaturePad>(this);
    }

    private DotNetObjectReference<MudSignaturePad> _dotnetObjectRef;
    ElementReference _reference;
    bool _isErasing = true;
    int _lineWidth = 3;
    private byte[] _value = Array.Empty<byte>();
    readonly string _id = Guid.NewGuid().ToString();
    string DrawEraseChipText => _isErasing ? "Eraser" : "Pen";
    string DrawEraseChipIcon => _isErasing ? @Icons.Material.Filled.DeleteSweep : @Icons.Material.Filled.Edit;

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

    [Parameter] public SignaturePadOptions Options { get; set; } = new SignaturePadOptions();

    [Parameter] public string ToolbarStyle { get; set; } = string.Empty;

    [Parameter] public string CanvasContainerClass { get; set; } = "border-solid border-2 mud-border-primary";
    [Parameter] public string CanvasContainerStyle { get; set; } = "height: 100%;width: 100%;";
    [Parameter] public bool ShowClear { get; set; } = true;
    [Parameter] public bool ShowLineWidth { get; set; } = true;
    [Parameter] public bool ShowStrokeStyle { get; set; } = true;
    [Parameter] public bool ShowDownload { get; set; } = true;
    [Parameter] public bool ShowLineJoinStyle { get; set; } = true;
    [Parameter] public bool ShowLineCapStyle { get; set; } = true;

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
        await JsRuntime.InvokeVoidAsync("mudSignaturePad.disposePad", _reference);
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