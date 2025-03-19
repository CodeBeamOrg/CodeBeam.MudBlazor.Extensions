using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MudExtensions;

#nullable enable

/// <summary>
/// Represents the child leaf of a JSON tree view.
/// </summary>
public partial class MudJsonTreeViewNode : ComponentBase
{
    /// <summary>
    /// Gets or sets the node to display (including children).
    /// </summary>
    [Parameter]
    [EditorRequired]
    public JsonNode? Node { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether items are sorted by key.
    /// </summary>
    [Parameter]
    public bool Sorted { get; set; }


    /// <summary>
    /// Gets or sets a children text.
    /// </summary>
    [Parameter]
    public string Text { get; set; } = string.Empty;

    public RenderFragment RenderJsonItem(KeyValuePair<string, JsonNode?> item) => builder =>
    {
        try
        {
            if (item.Value is JsonValue)
            {
                var value = item.Value.AsValue();
                var valueType = value.GetValue<JsonElement>().ValueKind;
                GenerateItemBasedOnType(valueType, item.Key, value, builder);
            }
            else if (item.Value is JsonArray)
                GenerateNestedComponent(builder, item, Icons.Material.Filled.DataArray, "(Array)");
            else if (item.Value is JsonObject)
                GenerateNestedComponent(builder, item, Icons.Material.Filled.DataObject, "(Object)");
            else if (item.Value is null)
                GenerateComponent(builder, item.Key, "null", Icons.Material.Filled.Block);
        }
        catch (Exception ex)
        {
            builder.OpenComponent<MudAlert>(0);
            builder.AddAttribute(1, "Severity", Severity.Warning);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(builder2 =>
            {
                builder2.AddContent(3, $"Error rendering JSON item: {ex.Message}");
            }));
            builder.CloseComponent();
        }
    };

    public RenderFragment RenderJsonItem(JsonNode? node) => builder =>
    {
        try
        {
            if (node is JsonValue)
            {
                var value = node.AsValue();
                var valueType = value.GetValue<JsonElement>().ValueKind;
                GenerateItemBasedOnType(valueType, Text, value, builder);
            }
            else if (node is JsonArray)
                GenerateNestedComponent(builder, node, Icons.Material.Filled.DataArray, "(Array)");
            else if (node is JsonObject)
                GenerateNestedComponent(builder, node, Icons.Material.Filled.DataObject, "(Object)");
            else if (node is null)
                GenerateComponent(builder, node.ToString(), "null", Icons.Material.Filled.Block);
        }
        catch (Exception ex)
        {
            builder.OpenComponent<MudAlert>(0);
            builder.AddAttribute(2, "Severity", Severity.Warning);
            builder.AddContent(1, $"Error rendering JSON item: {ex.Message}");
            builder.CloseComponent();
        }
    };

    void GenerateItemBasedOnType(JsonValueKind valueType, string text, JsonValue? value, RenderTreeBuilder builder)
    {
        switch (valueType)
        {
            case JsonValueKind.String:
                var str = value?.GetValue<string>();
                if (DateTime.TryParse(str, out DateTime date))
                    GenerateComponent(builder, text, date.ToString(), Icons.Material.Filled.DateRange);
                else if (Guid.TryParse(str, out Guid guid))
                    GenerateComponent(builder, text, str.ToUpperInvariant(), Icons.Material.Filled.Key);
                else
                    GenerateComponent(builder, text, str, Icons.Material.Filled.TextSnippet);
                break;

            case JsonValueKind.Number:
                string endText = string.Empty;
                if (value.TryGetValue<int>(out int intVal))
                {
                    endText = intVal.ToString();
                }
                else if (value.TryGetValue<double>(out double doubleVal))
                {
                    endText = doubleVal.ToString();
                }
                GenerateComponent(builder, text, endText, Icons.Material.Filled.Numbers);
                break;
            case JsonValueKind.True:
                GenerateComponent(builder, text, "true", Icons.Material.Filled.CheckBox);
                break;
            case JsonValueKind.False:
                GenerateComponent(builder, text, "false", Icons.Material.Filled.CheckBoxOutlineBlank);
                break;
        }
    }

    void GenerateComponent(RenderTreeBuilder builder, string text, string endText, string icon)
    {
        builder.OpenComponent<MudTreeViewItem<string>>(0);
        builder.AddAttribute(1, "Text", text);
        builder.AddAttribute(2, "Icon", icon);
        builder.AddAttribute(3, "EndText", endText);
        builder.CloseComponent();
    }

    void GenerateNestedComponent(RenderTreeBuilder builder, KeyValuePair<string, JsonNode?> item, string icon, string endText)
    {
        builder.OpenComponent<MudTreeViewItem<string>>(0);
        builder.AddAttribute(1, "Text", item.Key);
        builder.AddAttribute(2, "Icon", icon);
        builder.AddAttribute(3, "IconColor", Color.Primary);
        builder.AddAttribute(4, "EndText", endText);
        builder.AddAttribute(5, "EndTextClass", "mud-primary-text");
        builder.AddAttribute(6, "ChildContent", (RenderFragment)(childBuilder =>
        {
            if (item.Value.GetValueKind() is JsonValueKind.Array)
            {
                int count = 0;
                foreach (var childItem in item.Value.AsArray())
                {
                    count++;
                    childBuilder.OpenComponent<MudJsonTreeViewNode>(0);
                    childBuilder.AddAttribute(1, "Node", childItem);
                    childBuilder.AddAttribute(2, "Text", $"{count - 1}");
                    childBuilder.CloseComponent();
                }
            }
            else
            {
                childBuilder.OpenComponent<MudJsonTreeViewNode>(0);
                childBuilder.AddAttribute(1, "Node", item.Value);
                childBuilder.AddAttribute(2, "Text", $"{item.Key}");
                childBuilder.CloseComponent();
            }
        }));
        builder.CloseComponent();
    }

    void GenerateNestedComponent(RenderTreeBuilder builder, JsonNode? item, string icon, string endText)
    {
        builder.OpenComponent<MudTreeViewItem<string>>(0);
        builder.AddAttribute(1, "Text", item);
        builder.AddAttribute(2, "Icon", icon);
        builder.AddAttribute(3, "IconColor", Color.Primary);
        builder.AddAttribute(4, "EndText", endText);
        builder.AddAttribute(5, "EndTextClass", "mud-primary-text");
        builder.AddAttribute(6, "ChildContent", (RenderFragment)(childBuilder =>
        {
            if (item.GetValueKind() is JsonValueKind.Array)
            {
                int count = 0;
                foreach (var childItem in item.AsArray())
                {
                    count++;
                    childBuilder.OpenComponent<MudJsonTreeViewNode>(0);
                    childBuilder.AddAttribute(1, "Node", childItem);
                    childBuilder.AddAttribute(2, "Text", $"{count - 1}");
                    childBuilder.CloseComponent();
                }
            }
            else
            {
                childBuilder.OpenComponent<MudJsonTreeViewNode>(0);
                childBuilder.AddAttribute(1, "Node", item);
                childBuilder.AddAttribute(2, "Text", $"{item}");
                childBuilder.CloseComponent();
            }
        }));
        builder.CloseComponent();
    }
}
