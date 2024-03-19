### v 6.7.0
MudStepper's PreventStepChangeAsync func's parameters changed. (A new int target index added)
#### Before:
`public Func<StepChangeDirection, Task<bool>> PreventStepChangeAsync`
#### After:
`public Func<StepChangeDirection, int, Task<bool>> PreventStepChangeAsync`

### v 6.4.0
Change the css link to:\
`<link href="_content/CodeBeam.MudBlazor.Extensions/MudBlazor.Extensions.min.css" rel="stylesheet" />`\
Change the script to:\
`<script src="_content/CodeBeam.MudBlazor.Extensions/MudExtensions.min.js"></script>`

### v 6.3.0
Change the script to:\
`<script src="_content/CodeBeam.MudExtensions/MudExtensions.min.js"></script>`
