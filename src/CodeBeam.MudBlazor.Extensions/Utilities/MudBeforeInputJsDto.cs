namespace MudExtensions
{
    public sealed class MudBeforeInputJsDto
    {
        public MudBeforeInputJsDto() { }

        public string? Data { get; set; }
        public string? InputType { get; set; }
        public bool IsComposing { get; set; }
    }
}
