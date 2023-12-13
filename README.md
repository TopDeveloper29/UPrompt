# UPrompt

Uprompt is a software solution designed to simplify the process of creating user interfaces without the need for complex coding skills. It utilizes XML as the front-end language and HTML as both the back-end and front-end languages, allowing users to easily build customizable interfaces.

With Uprompt, users of all coding expertise levels can create visually appealing and interactive user interfaces. The program streamlines UI creation, making it accessible to individuals with different levels of experience. Additionally, Uprompt offers a range of built-in functions that facilitate seamless program interaction and code execution in external files and languages such as C# and PowerShell.

In summary, Uprompt enhances UI creation by providing a straightforward and effective tool suitable for users of all skill levels. Its use of XML and HTML simplifies the development of visually attractive and interactive interfaces, while its built-in functions enhance program functionality and flexibility.

## Getting Started
To get started with Uprompt, follow these steps:
1. Download the latest release of Uprompt.
> Actually there no release you must clone repository and build it using Visual Studio (Some feature still under developement)
3. Read the documentation provided above to learn how to use Uprompt.
4. Launch UPrompt.exe and start creating your UI.

## Table of Contents
- [UPrompt Basic](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Basic.md)
- [Setting](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Setting.md)
  - [XML Structure](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Setting.md#xml-structure)
  - [Theme](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Setting.md#theme)
  - [View Customization](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Setting.md#view-customization)
  - [Application Customization](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Setting.md#application-customization)
  - [Extension Declaration](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Setting.md#extension-declaration)
  - [Miscellaneous](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Setting.md#miscellaneous)
- [Variables](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Variable.md)
  - [Setting Variables](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Variable.md#setting-variables)
  - [System Variables](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Variable.md#system-variables)
  - [User Variables](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Variable.md#user-variables)
- [View](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md)
  - [ViewItem](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#viewitem)
    - [Spacer](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#spacer)
    - [Title](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#title)
    - [SubTitle](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#subtitle)
    - [Label](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#label)
    - [LabelBox](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#labelbox)
    - [Box](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#box)
    - [Row](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#row)
  - [ViewInput](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#top--viewinput)
    - [TextBox](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#textbox)
    - [LinesBox](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#linesbox)
    - [CheckBox](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#checkbox)
    - [DropDown](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#dropdown)
  - [ViewAction](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#top--viewaction)
    - [Linker](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#linker)
    - [Button](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#button)
    - [InputHandler](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#inputhandler)
    - [ViewLoad](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#viewload)
    - [VariableHandler](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#variablehandler)
  - [Html](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#top--html)
- [Action](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md)
  - [RunMethod](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#runmethod)
  - [RunPowershell](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#runpowershell)
  - [RunExe](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#runexe)
  - [RunPs1](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#runps1)
  - [GetVariable](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#getvariable)
  - [SetVariable](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#setvariable)
  - [ShowVariable](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#showvariable)
  - [YesNoDialog](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#yesnodialog)
  - [OkDialog](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#okdialog)
  - [Browse](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#browse)
  - [LoadPage](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#loadpage)
  - [ReloadView](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#reloadview)
  - [ReloadSettings](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#reloadsettings)
  - [LoadSetting](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#loadsetting)
  - [GetClipboard](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#getclipboard)
  - [SetClipboard](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#setclipboard)
  - [Exit](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md#exit)
- [Create Extension](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Extention.md)
  - [Powershell](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Extention.md)
  - [C#](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Extention.md)
  
