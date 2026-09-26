namespace MudExtensions
{
    public partial class MudSelectExtended<T>
    {
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            // SelectedValues is a synchronous parameter setter. Its existing async text
            // updates cannot be awaited there, so reconcile the displayed text once
            // the incoming parameter set has been applied. Do not depend on the newly
            // selected shadow item having rendered yet.
            if (Virtualize && MultiSelection && ItemCollection is not null)
            {
                await UpdateTextPropertyAsync(false);
            }
        }
    }
}
