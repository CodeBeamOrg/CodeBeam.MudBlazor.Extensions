namespace MudExtensions
{
    public sealed class MudBeforeInputEventArgs
    {
        /// <summary>
        /// Text that will be inserted. Null for delete actions.
        /// </summary>
        public string? Data { get; init; }

        /// <summary>
        /// Type of input action (insertText, deleteContentBackward, insertFromPaste, etc.)
        /// </summary>
        public string InputType { get; init; } = string.Empty;

        /// <summary>
        /// True when IME composition is active (Chinese, Japanese, etc.)
        /// </summary>
        public bool IsComposing { get; init; }

        /// <summary>
        /// Set true to prevent the input from happening.
        /// </summary>
        public bool PreventDefault { get; set; }

        public bool IsInsert => InputType.StartsWith("insert", StringComparison.Ordinal);

        public bool IsDeleteBackward => InputType == "deleteContentBackward";

        public bool IsDeleteForward => InputType == "deleteContentForward";

        public bool IsPaste => InputType == "insertFromPaste";

        public bool IsEnter => InputType == "insertLineBreak";
    }
}
