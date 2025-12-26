namespace MudExtensions
{
    public sealed class BeforeInputEventArgs
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

        /// <summary>
        /// 
        /// </summary>
        public bool IsInsert => InputType.StartsWith("insert", StringComparison.Ordinal);

        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleteBackward => InputType == "deleteContentBackward";

        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleteForward => InputType == "deleteContentForward";

        /// <summary>
        /// 
        /// </summary>
        public bool IsPaste => InputType == "insertFromPaste";

        /// <summary>
        /// 
        /// </summary>
        public bool IsEnter => InputType == "insertLineBreak";
    }
}
