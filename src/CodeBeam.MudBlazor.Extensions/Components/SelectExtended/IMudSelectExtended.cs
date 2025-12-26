
namespace MudExtensions
{
    internal interface IMudSelectExtended
    {
        void CheckGenericTypeMatch(object select_item);
        bool MultiSelection { get; set; }
    }

    internal interface IMudShadowSelectExtended
    {
    }
}
