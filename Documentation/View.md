# [Back](https://github.com/TopDeveloper29/UPrompt/blob/Post/README.md) | View
In this section, you will learn about the different kinds and types of built-in elements you can use to create a user interface in the `View` node.
> Note in all element you can put `Id` property if you don't provide one it will create one each time the view load so some thing could not work as expect like checkbox won't keep it checkstate.
## Section in the page:
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

## ViewItem
These elements are used for displaying content and organizing the layout.

### Spacer
**Description:** Adds space between two elements in a `Row`.
```xml
<ViewItem Type="Spacer"/>
```

### Title
**Description:** Displays text as a title.

**Additional Properties:**

| Name | Description |
| ---- | ----------- |
| Id   | Defines the Id in the HTML element. |
| Style | You can add extra CSS style to the element. |
| Class | Defines the CSS class of the element. |

```xml
<ViewItem Type="Title">The title text that will be displayed in the UI</ViewItem>
```

### SubTitle
**Description:** Displays text as a sub title.

**Additional Properties:**

| Name | Description |
| ---- | ----------- |
| Id   | Defines the Id in the HTML element. |
| Style | You can add extra CSS style to the element. |
| Class | Defines the CSS class of the element. |

```xml
<ViewItem Type="SubTitle">The title text that will be displayed in the UI</ViewItem>
```

### Label
**Description:** Displays text.

**Additional Properties:**

| Name | Description |
| ---- | ----------- |
| Id   | Defines the Id in the HTML element. |
| Style | You can add extra CSS style to the element. |
| Class | Defines the CSS class of the element. |

```xml
<ViewItem Type="Label">The text that will be displayed in the UI</ViewItem>
```

### LabelBox
**Description:** Displays text with a border.

**Additional Properties:**

| Name | Description |
| ---- | ----------- |
| Id   | Defines the Id in the HTML element. |
| Style | You can add extra CSS style to the element. |
| Class | Defines the CSS class of the element. |

```xml
<ViewItem Type="LabelBox">The text that will be displayed in the UI</ViewItem>
```

### Box
**Description:** Holds multiple elements in the same line in the user interface but with a visible border line.

```xml
<ViewItem Type="Box">
	<ViewItem Type="Title">My Title</ViewItem>
    	<ViewItem Type="Label">My Text</ViewItem>
</VieItem>
```

### Row
**Description:** Holds multiple elements in the same line in the user interface.

```xml
<ViewItem Type="Row">
    <ViewItem Type="Title">My Title</ViewItem>
    <ViewItem Type="Label">My Text</ViewItem>
</ViewItem>
```
## [Top](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#section-in-the-page) | ViewInput
This kind of element includes all the elements that allow the user to enter or select a value. It comes with a `<ViewAction Type="InputHandler"/>` when you add the `Action` and `Argument` properties to track input changes and run an action. If you run action it will store the result in a variable startind with **Result_** folow by the Input **Id** so somthing like [Result_MyInputId]. It also create a variable name that is the **Id** so [MyInputId] that will hold the last value of the input.

### TextBox
**Description:** A simple textbox that allows users to enter text.

**Additional Properties:**

| Name | Description |
| ---- | ----------- |
| Id   | Defines the Id in the HTML element and sets the variable name of the action result. |
| Style | You can add extra CSS style to the element. |
| Class | Defines the CSS class of the element. |
| Action | Specifies the name of the action you want to run (refer to the [Action](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md) section). |
| Argument | Passes arguments to the action you are running (if you do not specify `Action`, it will not work). |

```xml
<ViewInput Type="TextBox">Default value in TextBox</ViewInput>
```

### LinesBox
**Description:** A textbox that allows users to enter text on multiple lines.

**Additional Properties:**

| Name | Description |
| ---- | ----------- |
| Id   | Defines the Id in the HTML element and sets the variable name of the action result. |
| Style | You can add extra CSS style to the element. |
| Class | Defines the CSS class of the element. |
| Action | Specifies the name of the action you want to run (refer to the [Action](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md) section). |
| Argument | Passes arguments to the action you are running (if you do not specify `Action`, it will not work). |

```xml
<ViewInput Type="LinesBox">Default value in LinesBox</ViewInput>
```

### CheckBox
**Description:** A simple checkbox that allows users to check or uncheck a box, returning a boolean value.
> Do not forget Id property else the checkbox won't keep it state when user check or uncheck it.


**Additional Properties:**

| Name | Description |
| ---- | ----------- |
| Checked | A boolean value that defines if the checkbox is checked by default or not. |
| Id   | Defines the Id in the HTML element and sets the variable name of the action result. |
| Style | You can add extra CSS style to the element. |
| Class | Defines the CSS class of the element. |
| Action | Specifies the name of the action you want to run (refer to the [Action](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md) section). |
| Argument | Passes arguments to the action you are running (if you do not specify `Action`, it will not work). |

```xml
<ViewInput Type="CheckBox" Id="CheckBox1">The text next to the checkbox</ViewInput>
```

### DropDown
**Description:** Allows users to select a value from a dropdown list.

**Additional Properties:**

| Name | Description |
| ---- | ----------- |
| Id   | Defines the Id in the HTML element and sets the variable name of the action result. |
| Style | You can add extra CSS style to the element. |
| Class | Defines the CSS class of the element. |
| Action | Specifies the name of the action you want to run (refer to the [Action](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md) section). |
| Argument | Passes arguments to the action you are running (if you do not specify `Action`, it will not work). |

```xml
<ViewInput Type="DropDown"/>
```
## [Top](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#section-in-the-page) | ViewAction
These are elements designed to run actions, most of which are hidden and designed to track values and run actions based on them. The action that will run will store the result in a variable startind with **Result_** folow by the Input **Id** so somthing like [Result_MyInputId].


### Button
**Description:** A simple button that you can click on to run an action.

**Additional Properties:**

| Name | Description |
| ---- | ----------- |
| Id   | Defines the Id in the HTML element and sets the variable name of the action result. |
| Style | You can add extra CSS style to the element. |
| Class | Defines the CSS class of the element. |
| Image | Allows you to add an image to the button. The property must be in this format: String path to image (local path or link), Integer size of image, Boolean auto-theme the image color between dark and light theme (depends on application color). |
| Action | Specifies the name of the action you want to run (refer to the [Action](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md) section). |
| Argument | Passes arguments to the action you are running (if you do not specify `Action`, it will not work). |

```xml
<ViewAction Type="Button" Action="RunExe" Argument="cmd.exe,/C echo test">Text that will be displayed in the button</ViewAction>
```

### Linker
**Description:** This element is used to copy the value of a variable to another variable each time the page loads.

**Additional Properties:**

| Name   | Description                                                                  |
| ------ | ---------------------------------------------------------------------------- |
| Source | Variable name that we want to copy to the target.                            |
| Target | Variable name that we want to copy to (if the variable does not exist, it will create a new one). |

```xml
<ViewAction Type="Linker" Source="Variable1" Target="Variable2"/>
```

### InputHandler
**Description:** This is used to track input changes. All `ViewInput` elements that you add `Action` and `Argument` to will automatically add this for the input. This is specifically used for HTML elements and not built-in elements.

**Additional Properties:**

| Name     | Description                                                                                       |
| -------- | ------------------------------------------------------------------------------------------------- |
| Action   | Specifies the name of the action you want to run (refer to the [Action](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md) section). |
| Argument | Passes arguments to the action you are running (if you do not specify `Action`, it will not work). |

```xml
<ViewAction Type="InputHandler" Action="RunExe" Argument="cmd.exe,/C echo test">Tracked Input Id</ViewAction>
```

### ViewLoad
**Description:** This is used to run an action every time the view loads/reloads.

**Additional Properties:**

| Name     | Description                                                                                       |
| -------- | ------------------------------------------------------------------------------------------------- |
| Id       | Defines the Id in the HTML element and sets the variable name of the action result.               |
| Action   | Specifies the name of the action you want to run (refer to the [Action](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md) section). |
| Argument | Passes arguments to the action you are running (if you do not specify `Action`, it will not work). |

```xml
<ViewAction Type="ViewLoad" Action="RunExe" Argument="cmd.exe,/C echo test"/>
```

### VariableHandler
**Description:** This will track variable changes and run an action if a change has been made.

**Additional Properties:**

| Name     | Description                                                                                       |
| -------- | ------------------------------------------------------------------------------------------------- |
| Id       | Defines the Id in the HTML element and sets the variable name of the action result.               |
| Action   | Specifies the name of the action you want to run (refer to the [Action](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Action.md) section). |
| Argument | Passes arguments to the action you are running (if you do not specify `Action`, it will not work). |

```xml
<ViewAction Type="VariableHandler" Action="RunExe" Argument="cmd.exe,/C echo test">Tracked Input Id</ViewAction>
```

## [Top](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/View.md#section-in-the-page) | Html
Creating a custom element requires a basic knowledge of HTML/CSS. To make it work with UPrompt, you will need more details about how built-in XML elements are converted to HTML elements and how UPrompt retrieves information from them. To start creating your own HTML element, refer to this page: [Learn how to create HTML elements for UPprompt](https://github.com/TopDeveloper29/UPrompt/blob/Post/Documentation/Html.md)
