using MudExtensions.Utilities;

namespace MudExtensions.Docs.Services
{
    public class MudExtensionsDocsService
    {
        List<MudExtensionComponentInfo> _components = new()
        {
            new MudExtensionComponentInfo() {Title = "MudAnimate", Component = typeof(MudAnimate), Usage = ComponentUsage.Utility, IsUnique = true, Description = "A revolutionary next generation animate component."},
            new MudExtensionComponentInfo() {Title = "MudBarcode", Component = typeof(MudBarcode), Usage = ComponentUsage.Display, IsUnique = true, Description = "A QR and barcode viewer with defined value."},
            new MudExtensionComponentInfo() {Title = "MudChipField", Component = typeof(MudChipField<>), Usage = ComponentUsage.Input, IsUnique = true, Description = "A textfield which transform a text to a chip when pressed pre-defined key."},
            new MudExtensionComponentInfo() {Title = "MudCodeInput", Component = typeof(MudCodeInput<>), Usage = ComponentUsage.Input, IsUnique = true, Description = "A group of textfields that each one contains a char."},
            new MudExtensionComponentInfo() {Title = "MudColorProvider", Component = typeof(MudColorProvider), Usage = ComponentUsage.Utility, IsUnique = true, Description = "An extension for primary, secondary and tertiary colors to support Material 3.", IsMaterial3 = true},
            new MudExtensionComponentInfo() {Title = "MudComboBox", Component = typeof(MudComboBox<>),  RelatedComponents = new List<Type>() {typeof(MudComboBoxItem<string>)}, Usage = ComponentUsage.Input, IsUnique = true, Description = "Unites MudSelect and MudAutocomplete features."},
            new MudExtensionComponentInfo() {Title = "MudCssManager", Component = typeof(MudCssManager), Usage = ComponentUsage.Utility, IsUnique = true, IsUtility = true, Description = "Directly and dynamically get or set component's css property."},
            new MudExtensionComponentInfo() {Title = "MudCsvMapper", Component = typeof(MudCsvMapper), Usage = ComponentUsage.Display, IsUnique = true, Description = "A .csv file uploader that matches the .csv file headers to supplied / expected headers."},
            new MudExtensionComponentInfo() {Title = "MudDateWheelPicker", Component = typeof(MudDateWheelPicker), Usage = ComponentUsage.Input, IsUnique = true, Description = "A date time picker with MudWheels."},
            new MudExtensionComponentInfo() {Title = "MudGallery", Component = typeof(MudGallery), Usage = ComponentUsage.Display, IsUnique = true, Description = "Mobile friendly image gallery component."},
            new MudExtensionComponentInfo() {Title = "MudInputStyler", Component = typeof(MudInputStyler), Usage = ComponentUsage.Utility, IsUnique = true, Description = "Applies colors or other CSS styles easily for mud inputs like MudTextField and MudSelect."},
            new MudExtensionComponentInfo() {Title = "MudJsonTreeView", Component = typeof(MudJsonTreeView), Usage = ComponentUsage.Display, RelatedComponents = new List<Type>() {typeof(MudJsonTreeViewNode)}, IsUnique = true, Description = "A treeview for display json data."},
            new MudExtensionComponentInfo() {Title = "MudListExtended", Component = typeof(MudListExtended<>), Usage = ComponentUsage.Input, RelatedComponents = new List<Type>() {typeof(MudListItemExtended<string>)}, IsUnique = false, Description = "The extended MudList component with richer features."},
            new MudExtensionComponentInfo() {Title = "MudLoading", Component = typeof(MudLoading), Usage = ComponentUsage.Display, IsUnique = true, Description = "Loading container for a whole page or a specific section."},
            new MudExtensionComponentInfo() {Title = "MudLoadingButton", Component = typeof(MudLoadingButton), Usage = ComponentUsage.Button, IsUnique = true, Description = "A classic MudButton with loading improvements."},
            new MudExtensionComponentInfo() {Title = "MudPage", Component = typeof(MudPage), Usage = ComponentUsage.Layout, RelatedComponents = new List<Type>() {typeof(MudSection)}, IsUnique = true, Description = "A CSS grid layout component that builds columns and rows, supports ColSpan & RowSpan."},
            new MudExtensionComponentInfo() {Title = "MudPasswordField", Component = typeof(MudPasswordField<>), Usage = ComponentUsage.Input, IsUnique = true, Description = "A specialized textfield that designed for working easily with passwords."},
            new MudExtensionComponentInfo() {Title = "MudPopup", Component = typeof(MudPopup), Usage = ComponentUsage.Display, IsUnique = true, Description = "A mobile friendly multi-functional popup content."},
            new MudExtensionComponentInfo() {Title = "MudRangeSlider", Component = typeof(MudRangeSlider<int>), Usage = ComponentUsage.Input, IsUnique = true, Description = "A slider with range capabilities, set upper and lower values."},
            new MudExtensionComponentInfo() {Title = "MudScrollbar", Component = typeof(MudScrollbar), Usage = ComponentUsage.Utility, IsUnique = true, Description = "Customize all or defined scrollbars."},
            new MudExtensionComponentInfo() {Title = "MudSelectExtended", Component = typeof(MudSelectExtended<>), Usage = ComponentUsage.Input, RelatedComponents = new List<Type>() {typeof(MudSelectItemExtended<string>)}, IsUnique = false, Description = "The extended MudSelect component with richer features."},
            new MudExtensionComponentInfo() {Title = "MudSignaturePad", Component = typeof(MudSignaturePad), Usage = ComponentUsage.Display, IsUnique = true, Description = "Draw and export a signature on a canvas."},
            new MudExtensionComponentInfo() {Title = "MudSpeedDial", Component = typeof(MudSpeedDial), Usage = ComponentUsage.Button, IsUnique = true, Description = "One button that stack other buttons in a popover."},
            new MudExtensionComponentInfo() {Title = "MudSplitter", Component = typeof(MudSplitter), Usage = ComponentUsage.Layout, IsUnique = true, Description = "A resizeable content splitter."},
            new MudExtensionComponentInfo() {Title = "MudStepperExtended", Component = typeof(MudStepperExtended), Usage = ComponentUsage.Display, RelatedComponents = new List<Type>() {typeof(MudStepExtended)}, IsUnique = false, Description = "A wizard-like steps to control the flow with rich options."},
            new MudExtensionComponentInfo() {Title = "MudSwitchM3", Component = typeof(MudSwitchM3<bool>), Usage = ComponentUsage.Input, IsUnique = true, IsMaterial3 = true, Description = "Material 3 switch component that has all MudSwitch features."},
            new MudExtensionComponentInfo() {Title = "MudTeleport", Component = typeof(MudTeleport), Usage = ComponentUsage.Layout, IsUnique = true, Description = "Teleport the content to the specified parent and redesign the DOM hierarchy."},
            new MudExtensionComponentInfo() {Title = "MudTextFieldExtended", Component = typeof(MudTextFieldExtended<>), Usage = ComponentUsage.Input, IsUnique = false, Description = "The extended MudTextField component with richer features."},
            new MudExtensionComponentInfo() {Title = "MudTextM3", Component = typeof(MudTextM3), Usage = ComponentUsage.Display, IsUnique = true, IsMaterial3 = true, Description = "Material 3 typography."},
            new MudExtensionComponentInfo() {Title = "MudTransferList", Component = typeof(MudTransferList<>), Usage = ComponentUsage.Input, IsUnique = true, Description = "A component that has 2 lists that transfer items to another."},
            new MudExtensionComponentInfo() {Title = "MudWatch", Component = typeof(MudWatch), Usage = ComponentUsage.Display, IsUnique = true, Description = "A performance optimized watch to show current time or show stopwatch or countdown."},
            new MudExtensionComponentInfo() {Title = "MudWheel", Component = typeof(MudWheel<>), Usage = ComponentUsage.Input, IsUnique = true, Description = "Smoothly changes values in a wheel within defined ItemCollection."},
        };

        public List<MudExtensionComponentInfo> GetAllComponentInfo()
        {
            return _components;
        }
    }
}
