# [Back](https://github.com/TopDeveloper29/UPrompt/blob/Prod/README.md) | Variables

## Setting Variables
Setting variables in UPrompt are denoted by starting and ending with **#**. These variables represent values that are configured in the application's settings and are commonly used in CSS to configure the theme of the application. Here are

| Name                  | Description                                                     |
| --------------------- | --------------------------------------------------------------- |
| #TEXT_COLOR#          | Returns the color of text in the user interface or application. |
| #MAIN_COLOR#          | Returns the main color used in the user interface or application. |
| #BACKGROUND_COLOR#    | Returns the background color used in the user interface or application. |
| #FADE_BACKGROUND_COLOR# | Returns the fade background color used in the user interface or application. |
| #FADE_MAIN_COLOR#     | Returns the fade main color used in the user interface or application. |
| #ITEM_MARGIN#         | Returns the margin used for items in the user interface or application. |
| #MAIN_TEXT_COLOR#     | Returns the main color used for text in the user interface or application. |
| #FONT_NAME#           | Returns the font used for text in the user interface or application. |
| #WINDOWSOPENMODE#    | Returns how windows are opened in the user interface or application. |
| #WINDOWRESIZEMODE#    | Returns how windows can be resized in the user interface or application. |
| #SHOWMINIMIZE#        | Returns whether the minimize button is shown in window frames. |
| #SHOWMAXIMIZE#        | Returns whether the maximize button is shown in window frames. |
| #SHOWCLOSE#           | Returns whether the close button is shown in window frames. |
| #WIDTH#               | Returns the width of the user interface or application. |
| #HEIGHT#              | Returns the height of the user interface or application. |
| #PRODUCTION#          | Returns whether the application is in production mode. |
| #APPLICATION_NAME#    | Returns the application name. |
| #APPLICATION_ICON#    | Returns the application icon object as a string. |
| #APPLICATION_ICONPATH# | Returns the application icon path. |

## System Variables
System variables in UPrompt are denoted by starting with **{** and ending with **}**. These variables represent both static and dynamic pre-defined values in the code, such as machine/user names or application paths. Here are some examples of system variables:

| Name        | Description                                                  |
| ----------- | ------------------------------------------------------------ |
| {USER}      | Returns the username of the user running the application.    |
| {DEVICE}    | Returns the name of the device.                              |
| {n}         | Represents a new line character (`\n` in C#).               |
| {AppPath}   | Returns the application path, which can be used in a web context (e.g., `file:///C:/MyAppPath/`). |
| {AppPathWindows} | Returns the application path in a Windows context (e.g., `C:\MyAppPath`). |

## User Variables

User variables in UPrompt are generated during the execution of the application and are denoted by starting with **[** and ending with **]**. These variables can be set using actions and store information from user interactions or action results.

For example, when running an action, a variable named `[Result_ElementId]` will be generated, where `ElementId` is the ID of the UI element that triggered the action. The value of this variable reflects the last result of the action. If an action returns a value, such as a PowerShell script returning a list of items, the variable will contain a string representation of all the returned items. If an action does not return a value, it will contain a boolean indicating the result of the action. For example, if you try to run an executable but the path is not found, the variable will contain `false`. Similarly, a Yes/No message will store `true` or `false` depending on what the user clicks.

For each input, a variable called `[ElementId]` will be generated, where `ElementId` is the ID of the UI element. The variable holds the last value written/selected in the input.

> To read these variables with a PowerShell extension, note that you can use `$VariableName`, where `VariableName` is the name of the variable in UPrompt.
