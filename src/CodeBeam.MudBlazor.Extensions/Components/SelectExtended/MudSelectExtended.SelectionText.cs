namespace MudExtensions
{
    public partial class MudSelectExtended<T>
    {
        /// <inheritdoc />
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            // Reconcile collection-backed presentation after the complete parameter set has
            // been applied. ItemCollection and the selected values are authoritative, so display
            // text is derived from their current state rather than retained item components.
            //
            // This also covers mutable selected objects whose display representation changes
            // without replacing the collection or selected-value reference.
            if (ItemCollection is not null)
                await UpdateTextPropertyAsync(false);
        }
    }
}
