using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json.Nodes;

namespace MudExtensions;

#nullable enable

/// <summary>
/// Represents a tree view which displays a snippet of JSON.
/// </summary>
public partial class MudJsonTreeView : MudComponentBase
{
    private string? _json;
    private JsonNode? _root;

    /// <summary>
    /// The JSON to be displayed.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="Root"/> parameter instead if you have a <see cref="JsonNode"/> available.
    /// </remarks>
    [Parameter]
    public string? Json 
    {
        get => _json;
        set => SetJson(value);
    }

    /// <summary>
    /// The root node of the JSON to display.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="Json"/> parameter instead if you only have JSON available as a string.
    /// </remarks>
    [Parameter]
    public JsonNode? Root
    {
        get => _root;
        set => SetJson(value);
    }

    /// <summary>
    /// Sets the <see cref="Json"/> property and raises the <see cref="OnJsonChanged"/> event.
    /// </summary>
    /// <param name="json">The new JSON to use.</param>
    protected void SetJson(string? json)
    {
        _json = json;
        _root = string.IsNullOrEmpty(_json) ? null : JsonNode.Parse(_json);
        OnJsonChanged.InvokeAsync(Root);
        StateHasChanged();
    }

    /// <summary>
    /// Sets the <see cref="Json"/> property and raises the <see cref="OnJsonChanged"/> event.
    /// </summary>
    /// <param name="json">The new JSON to use.</param>
    protected void SetJson(JsonNode? json)
    {
        _json = json?.ToJsonString();
        _root = json;
        OnJsonChanged.InvokeAsync(Root);
        StateHasChanged();
    }

    /// <summary>
    /// Occurs when the JSON has changed.
    /// </summary>
    public EventCallback<JsonNode> OnJsonChanged { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tree contents are compacted.
    /// </summary>
    [Parameter]
    public bool Dense { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current row is highlighted.
    /// </summary>
    [Parameter]
    public bool Hover { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether items are sorted by key.
    /// </summary>
    [Parameter]
    public bool Sorted { get; set; }
}
