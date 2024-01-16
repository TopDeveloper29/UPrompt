# [Back](https://github.com/TopDeveloper29/UPrompt/blob/Prod/README.md) | Setting

In this section, you can find a list of available settings that you can add to XML and how to use them. Settings are used to modify the behavior and appearance of the application.

## XML Structure
All setting elements have two properties: Name and Value. You can write them in the following ways:
```xml
<Setting Name="CSS" Value="C:\MyStyle.css"></Setting>
```
**OR**
```xml
<Setting Name="CSS" Value="C:\MyStyle.css"/>
```
> The `Setting` element also has an optional `Id` property that can be added in addition to `Name` and `Value` in some specific case.

## Theme
These settings are related to customizing the application's theme.

| Name | Accepted Value | Description |
| ---- | -------------- | ----------- |
| ShowSplash | boolean | This configure if at load of the page it should display a Splash screnn (it recoment to use only with heavy page that take some time to load else it will just flicker) |
| Font | Name of font | This sets the default font for all built-in elements. The font must exist in the CSS context. If you want to import a new font, you can do it from CSS. Refer to [Import font in CSS](https://www.w3schools.com/css/tryit.asp?filename=trycss3_font-face_rule) for more information.
| Application-Color | Hexadecimal color | This defines the main color of the application. The background will be of this color, and other elements will adapt their color accordingly.
| Text-Color | Hexadecimal color | This sets the default text color for built-in elements in the application.
| Accent-Color | Hexadecimal color | This defines a secondary color that will be used by some elements like buttons.
| Accent-Text-Color | Hexadecimal color | This defines a secondary text color that will be used by some elements like buttons.

## View Customization
These settings are used to customize the display of the view.

| Name | Accepted Value | Description |
| ---- | -------------- | ----------- |
| Item-Margin | Size in px or % | This sets the default margin between all items.
| CSS | Path to .css file | This is used to import an external CSS file into HTML. You can write your own CSS styles for custom classes or import fonts, etc.

## Application Customization
These settings control the look, properties, and behavior of the application.

| Name | Accepted Value | Description |
| ---- | -------------- | ----------- |
| Width | Integer | This sets the default width of the application.
| Height | Integer | This sets the default height of the application.
| WindowsResizeMode | All, Horizontal, Vertical, Diagonal, None | This sets how the user can resize the application or blocks resizing.
| WindowsOpenMode | Normal, Minimized, Maximized | This sets how the window should open at startup of Uprompt.
| Application-Icon | Path to an icon/image file | This defines the application icon that is displayed in the taskbar and elsewhere in the system.
| Application-Title | String | This defines the application title that is displayed in the taskbar and elsewhere in the system.
| ShowMinimize | Boolean | This determines whether the minimize button is displayed.
| ShowMaximize | Boolean | This determines whether the maximize button is displayed.
| ShowClose | Boolean | This determines whether the close button is displayed. If set to false, make sure to include an Exit button in your `View`.

## Extension Declaration
These settings load external files as extensions of the application. To learn how to create an extension, refer to the "Create Extension" section. These settings allow you to provide an additional property called Id, which must be a unique integer representing the extension in the environment. If no Id is provided, Uprompt will automatically assign one starting from 0 and incrementing by 1 for each new extension. Note that the code of the extension will not run at this time; you must use an `Action` to do so.

| Name | Accepted Value | Description |
| ---- | -------------- | ----------- |
| Extention | Path to .dll file, Namespace.Class | This loads a C# DLL file as an extension. The value must be set as: `Value="C:\MyExtension.dll,MyExtensionNamespace.MyMethod"`.
| Powershell | Path to .ps1 file | This loads a PowerShell file as an extension, allowing it to access variables and interact continuously with Uprompt by calling functions and reading output.

## Miscellaneous

| Name | Accepted Value | Description |
| ---- | -------------- | ----------- |
| Production | Boolean | This determines whether to hide warning messages.
| OnLoad | Action, Arguments | This runs an action when the page loads.
| SkipElementParsing | Element Id | This skips variable and system parsing for a given UI element.
| Variable | Name, Default Value | This initializes a variable with a default value (to avoid seeing `[Variable]` if displayed in UI).
