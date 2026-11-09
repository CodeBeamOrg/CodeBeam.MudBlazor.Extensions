using System.ComponentModel;
using Microsoft.AspNetCore.Components;

namespace MudExtensions
{
    /// <summary>
    /// The text field rendered inside <see cref="MudChipField{T}"/>. It validates Required against the chips of
    /// the chip field instead of the typed text, so it agrees with the chip field's own validation.
    /// It is public only because Razor can't render internal components; it isn't meant to be used directly.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ChipFieldTextField<T> : MudTextFieldExtended<T>
    {
        /// <summary>
        /// Returns true when the parent chip field has at least one chip.
        /// </summary>
        [Parameter]
        public Func<bool>? HasChips { get; set; }

        /// <inheritdoc />
        protected override bool HasValue(T? value) => HasChips?.Invoke() ?? base.HasValue(value);
    }
}
