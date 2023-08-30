# CodeBeam.MudExtensions

#### Useful third party extension components for MudBlazor, from the community contributors.

##### `TheMudSecondary`

[![GitHub Repo stars](https://img.shields.io/github/stars/codebeamorg/codebeam.mudblazor.extensions?color=594ae2&style=flat-square&logo=github)](https://github.com/codebeamorg/codebeam.mudblazor.extensions/stargazers)
[![GitHub last commit](https://img.shields.io/github/last-commit/codebeamorg/codebeam.mudblazor.extensions?color=594ae2&style=flat-square&logo=github)](https://github.com/codebeamorg/codebeam.mudblazor.extensions)
[![Contributors](https://img.shields.io/github/contributors/codebeamorg/codebeam.mudblazor.extensions?color=594ae2&style=flat-square&logo=github)](https://github.com/codebeamorg/codebeam.mudblazor.extensions/graphs/contributors)
[![NuGet version](https://img.shields.io/nuget/v/CodeBeam.mudblazor.extensions?color=ff4081&label=nuget%20version&logo=nuget&style=flat-square)](https://www.nuget.org/packages/CodeBeam.MudBlazor.Extensions)
[![NuGet downloads](https://img.shields.io/nuget/dt/CodeBeam.mudblazor.extensions?color=ff4081&label=nuget%20downloads&logo=nuget&style=flat-square)](https://www.nuget.org/packages/CodeBeam.MudBlazor.Extensions)

## Utilities

- MudCssManager

## Components

- MudAnimate
- MudBarcode
- MudChipField
- MudCodeInput
- MudColorProvider
- MudComboBox
- MudCsvMapper
- MudDateWheelPicker
- MudFontPicker
- MudGallery
- MudInputStyler
- MudLoading
- MudLoadingButton
- MudPage
- MudPasswordField
- MudPopup
- MudRangeSlider
- MudScrollbar
- MudSection
- MudSpeedDial
- MudSplitter
- MudStepper
- MudSwitchM3
- MudTeleport
- MudTextM3
- MudToggle
- MudTransferList
- MudWatch
- MudWheel

## Extended Components

- MudListExtended
- MudSelectExtended
- MudTextFieldExtended

## Playground

- **Docs**
https://codebeam-mudextensions.pages.dev/
- **Try MudExtensions**
https://trymudextensions.pages.dev/

## Breaking Changes

Look at the [Breaking Changes](https://github.com/CodeBeamOrg/CodeBeam.MudBlazor.Extensions/blob/dev/BreakingChanges.md)

## API

https://codebeam-mudextensions.pages.dev/api

## Supported MudBlazor Versions

| MudExtensions  |    MudBlazor    |      .NET       |
| :------------- | :-------------: | :-------------: |
| 6.0.0 - 6.0.3  |     6.0.16      |     .NET 6      |
| 6.0.4 - 6.0.12 | 6.0.17 - 6.0.18 |     .NET 6      |
| 6.1.0 - 6.1.4  |  6.1.0 - 6.1.2  | .NET 6 & .NET 7 |
| 6.1.5 - 6.1.9  |  6.1.4 - 6.1.7  | .NET 6 & .NET 7 |
| 6.2.0 - 6.2.5  |      6.1.8      | .NET 6 & .NET 7 |
| 6.2.6 - 6.4.8  |  6.1.9 - 6.2.0  | .NET 6 & .NET 7 |
| 6.4.9 - 6.5.9  |  6.2.1 - 6.9.0  | .NET 6 & .NET 7 |

## Installation

This extension uses MudBlazor features (need to set up MudBlazor if you didn't before), so only need to add this line described below:<br /><br />
Add the following to your HTML **head** section, it's either `index.html` or `_Layout.cshtml`/`_Host.cshtml` depending on whether you're running Server-Side or WASM.

```html
<link href="_content/CodeBeam.MudBlazor.Extensions/MudExtensions.min.css" rel="stylesheet" />
```

Add the following to your HTML **body** section

```html
<script src="_content/CodeBeam.MudBlazor.Extensions/MudExtensions.min.js"></script>
```

Add the extension services into `Program.cs`

```cs
using MudExtensions.Services;
builder.Services.AddMudExtensions();
```

Adding namespace to `_Imports.razor` is not obligatory, but useful.

```razor
@using MudExtensions
```

## Examples

Each example is recorded in the playground website.

### MudAnimate

https://user-images.githubusercontent.com/78308169/194701300-e4ad517a-8fbb-4a5e-9407-a5a585157685.mp4

### MudCodeInput

https://user-images.githubusercontent.com/78308169/213863531-fa817188-3194-4858-8642-ace98a324b32.mp4

### MudDateWheelPicker

https://user-images.githubusercontent.com/78308169/198103805-56f9403b-dcdd-4e82-a02e-e53426d55ad5.mp4

### MudFontPicker

https://user-images.githubusercontent.com/78308169/196959281-76d6e05b-8368-43a2-a562-658dbd4ea3a9.mp4

### MudGallery

https://user-images.githubusercontent.com/78308169/197382396-6ff0a926-e52a-4554-b408-5736ebd539c9.mp4

### MudInputStyler

https://user-images.githubusercontent.com/78308169/198103898-41a8eb8a-ba0a-4f06-adfe-ced24c76b4e5.mp4

### MudPopup

https://user-images.githubusercontent.com/78308169/198975726-d2633c10-8059-4fc6-8e8a-ce9dd79ed018.mp4

### MudSpeedDial

https://user-images.githubusercontent.com/78308169/194750397-e0dfd08c-c297-426f-bcd6-55ce2e7c4a25.mp4

### MudStepper

https://user-images.githubusercontent.com/78308169/195593490-6fd8d5fe-0472-4b9b-8180-ef6b9d94bcbd.mp4

### MudWheel

https://user-images.githubusercontent.com/78308169/195979884-7cf1698c-ce0a-400f-83a7-2accf6fff8c5.mp4

## Contribution

Feel free to contribute.

## Updating css / Compiling from scss
- If you update or create an new scss file you will need to compile the scss to css using excubo webcompiler during a solution rebuild.
- Install Nuget Package [Excubo.WebCompiler](https://www.nuget.org/packages/Excubo.WebCompiler) for SCSS 
	- From command line run: **dotnet add package Excubo.WebCompiler**
- If add a new scss file you will need to add define it in the [MudExtensions.scs](https://github.com/CodeBeamOrg/CodeBeam.MudExtensions/blob/dev/CodeBeam.MudBlazor.Extensions/Styles/MudExtensions.css)
- [Uncomment this line](https://github.com/CodeBeamOrg/CodeBeam.MudBlazor.Extensions/blob/9d46ab49cc39bcbc5ed7f3c184db57201eca91cb/CodeBeam.MudBlazor.Extensions/CodeBeam.MudBlazor.Extensions.csproj#L42)
- The css will compile when you "rebuild" the solution
- Re-Comment the code you un commented in the previous step
- The css will compile on rebuild
- Re-Comment the code you un commented.
### If adding a scss you will need to add define it in the [MudExtensions.scs](https://github.com/enkodellc/CodeBeam.MudExtensions/blob/dev/CodeBeam.MudBlazor.Extensions/Styles/MudExtensions.css)

