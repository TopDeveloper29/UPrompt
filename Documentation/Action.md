# [Back](https://github.com/TopDeveloper29/UPrompt/blob/Post/README.md) | Action

In this section, you will learn about all the built-in actions that you can directly run by writing them down in **XML** or **HTML** elements. Note that most of them are really simple, and UPrompt is designed more to run your own code using C# or PowerShell extensions.

Before reading this section, make sure you have read the UPrompt Basics, Settings, Variables, and have taken a look at the View documentation. Otherwise, you may be lost and not understand the examples.

## RunMethod

**Description:** It allows you to run a method of an extension (DLL) that is loaded from `Setting`.

**Arguments:**

| Name | Description | Mandatory |
| ---- | ----------- | --------- |
| Id | A number that represents the ID of a loaded C# extension (from setting Extension). | Yes |
| Method | The method to run in the extension, like `ExtensionMethod(MethodArgument, Argument)`. | Yes |

**Example:**

```xml
<Application>
	<Setting Name="Extension" Value="C:\TEMP\Extension.dll,DemoNameSpace.DemoClass" Id="123456789"/>
	<View>
		<ViewAction Type="Button" Action="RunMethod" Argument="123456789,RunDemo({USER},{DEVICE})">TEST</ViewAction>
	</View>
</Application>
```

## RunPowershell

**Description:** It allows you to run code or a function in a PowerShell extension loaded from `Setting`. Do not confuse it with `RunPs1` that runs a script directly. This action will not run the script file directly; instead, it will pass through **{AppPathWindows}\Resources\Code\PsHandler.ps1**, which will load itself and the targeted script extension at startup. It will then loop until the application stops. Each time it loops, it will reload **{AppPathWindows}\Resources\Code\Variables.ps1** to load UPrompt user variables in the environment and wait for input. When you run the action, UPrompt will send the input to the correct PsHandler instance, and the input will be executed in it. Here are two examples of what you can put in the Argument Command:

```bash
Write-Host "TEST";
```

In this case, it will directly run the code, and the result will be **TEST**.

```bash
MyFunction -Text "TEST";
```

In this case, you run a function that is written in your ps1 file, and the output will be whatever the function does. For example, if the function is:

```bash
function MyFunction([string]$Text)
{
	Write-Host "MY $Text"; 
}
```

the output will be **MY TEST**.

**Arguments:**

| Name | Description | Mandatory |
| ---- | ----------- | --------- |
| Id | A number that represents the ID of a loaded PowerShell file. | Yes |
| Command | Command that will be sent to the PowerShell environment, like `Write-Host "Test of [Variable]";` or `Function("[VariableAsArgument]");`. | Yes |

**Example:**

```xml
<Application>
	<Setting Name="Powershell" Value="C:\TEMP\Extension.ps1" Id="123456789"/>
	<View>
		<ViewAction Type="Button" Action="RunPowershell" Argument='123456789,Write-Host "TEST"'>TEST</ViewAction>
	</View>
</Application>
```

## RunExe

**Description:** It allows you to run an executable directly and pass arguments to it.

**Arguments:**

| Name | Description | Mandatory |
| ---- | ----------- | --------- |
| Path | Path to the executable file you want to run. | Yes |
| Argument | Argument passed to the executable, like `/Argument /S /Q`. | No |

**Example:**

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="RunExe" Argument='cmd.exe,/C exit'>TEST</ViewAction>
	</View>
</Application>
```

## RunPs1

**Description:** It allows you to run a PowerShell script file (.ps1) directly (not in the environment).

**Arguments:**

| Name | Description | Mandatory |
| ---- | ----------- | --------- |
| Path | Path to the ps1 file to run. You can also try `"C:\path\file.ps1" -ScriptArgument "true"` (but it's better to use PowerShell setting to load a script in the environment to do that). | Yes |

**Example:**

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="RunPs1" Argument='C:\TEMP\Script.ps1'>TEST</ViewAction>
	</View>
</Application>
```

## GetVariable

**Description:** It allows you to read the content of a user variable.

**Arguments:**

| Name | Description | Mandatory |
| ---- | ----------- | --------- |
| Name | The name of the variable to read its value. | Yes |

**Example:**

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="GetVariable" Argument='VariableName'>TEST</ViewAction>
	</View>
</Application>
```

## SetVariable

**Description:** It allows you to set the value of a variable (existing or new).

**Arguments:**

| Name  | Description | Mandatory |
|-------|-------------|-----------|
| Name  | A unique name for the variable. | Yes      |
| Value | The value that the variable will now contain. | Yes      |

**Example:**

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="SetVariable" Argument='VariableName,VariableValue'>TEST</ViewAction>
	</View>
</Application>
```

## ShowVariable

**Description:** It allows you to show all user variables or a specified one using a dialog or not.

**Arguments:**

| Name       | Description                                                                                      | Mandatory |
|------------|--------------------------------------------------------------------------------------------------|-----------|
| Id         | By default, it is set to `Show::All::Id`, which will show all variables. If it is an array like `"Var1,Var2"`, it will show all variables in the array. Otherwise, it will show the variable by name. | No     |
| ShowDialog | Boolean that determines if it will show the dialog or not. The default value is True. If the dialog is not shown, the result is outputted in a variable.       | No   |

**Example:**

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="ShowVariable" Argument='"VariableName1,VariableName2",True'>TEST</ViewAction>
		<ViewAction Type="Button" Action="ShowVariable" Argument='"VariableName1"'>TEST2</ViewAction>
		<ViewAction Type="Button" Action="ShowVariable">TEST3</ViewAction>
	</View>
</Application>
```
## YesNoDialog

**Description**: It shows a basic yes/no dialog that returns true/false depending on the user's answer.

**Arguments**:

| Name   | Description             | Mandatory |
|--------|-------------------------|-----------|
| String | The text to display in the dialog. | Yes      |

**Example**:

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="YesNoDialog" Argument='Do you like UPrompt?'>TEST</ViewAction>
	</View>
</Application>
```

---

## OkDialog

**Description**: It shows a basic OK dialog that returns true/false.

**Arguments**:

| Name   | Description             | Mandatory |
|--------|-------------------------|-----------|
| String | The text to display in the dialog. | Yes      |

**Example**:

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="OkDialog" Argument='You are {USER} on {DEVICE}!!!'>TEST</ViewAction>
	</View>
</Application>
```

---

## Browse

**Description**: Browse the file system (load or save files dialog) or select a folder dialog.

**Arguments**:

| Name   | Description                                                                                          | Mandatory |
|--------|------------------------------------------------------------------------------------------------------|-----------|
| Type   | Select the type of browsing. Can be `File` or `Folder`.                                               |  Yes      |
| Mode   | The valid values are `Save` and `Load`.                                  |  Only applies for Type `File`.  No    |
| Filter | By default, it is `All Files \| *.*`. You can apply a custom filter to the browser, like for XML files: `XML Files \| *.xml`. |  Only applies for Type `File`. No     |

**Example**:

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="Browse" Argument='File,Save'>TEST</ViewAction>
		<ViewAction Type="Button" Action="Browse" Argument='File,Save,XML Files|*.xml'>TESTA</ViewAction>
		<ViewAction Type="Button" Action="Browse" Argument='File,Load'>TEST2</ViewAction>
		<ViewAction Type="Button" Action="Browse" Argument='File,Load,XML Files|*.xml'>TEST2A</ViewAction>
		<ViewAction Type="Button" Action="Browse" Argument='Folder'>TEST3</ViewAction>
	</View>
</Application>
```

---

## LoadPage

**Description**: It allows you to load a new XML page by specifying its path.

**Arguments**:

| Name | Description                       | Mandatory |
|------|-----------------------------------|-----------|
| Path | Path of the XML page to load.     | true      |

**Example**:

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="LoadPage" Argument='C:\TEMP\NewPage.xml'>TEST</ViewAction>
	</View>
</Application>
```

---

## ReloadView

**Description**: Reload the current view from XML.

**Example**:

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="ReloadView">TEST</ViewAction>
	</View>
</Application>
```

---

## ReloadSettings

**Description**: Reload settings from XML.

**Example**:

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="ReloadSettings">TEST</ViewAction>
	</View>
</Application>
```

---

## LoadSetting

**Description**: It allows you to configure a setting during the execution of UPrompt (some settings may not load correctly).

**Arguments**:

| Name  | Description                                                | Mandatory |
|-------|------------------------------------------------------------|-----------|
| Name  | A string that represents the name of the setting.           | Yes      |
| Value | A string that represents the value of the setting.          | Yes      |
| Id    | If the setting allows having an ID defined (like Extension and PowerShell). | No     |

**Example**:

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="LoadSetting" Argument="Width,300">TEST</ViewAction>
		<ViewAction Type="Button" Action="LoadSetting" Argument="Powershell,C:\TEMP\Script.ps1,123456789">TEST2</ViewAction>
	</View>
</Application>
```

---

## GetClipboard

**Description**: It allows you to get the last line of the clipboard in the result variable.

**Example**:

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="GetClipboard">TEST</ViewAction>
	</View>
</Application>
```

---

## SetClipboard

**Description**: It allows you to copy a value to the clipboard.

**Arguments**:

| Name   | Description                   | Mandatory |
|--------|-------------------------------|-----------|
| String | Text you want to copy to the clipboard. | Yes      |

**Example**:

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="SetClipboard" Argument="So your name is {USER}">TEST</ViewAction>
	</View>
</Application>
```

---

## Exit

**Description**: It allows you to exit the application with or without an exit code.

**Arguments**:

| Name     | Description                                                             | Mandatory |
|----------|-------------------------------------------------------------------------|-----------|
| ExitCode | A number that represents the exit code of the application. By default, it throws 0. | No     |

**Example**:

```xml
<Application>
	<View>
		<ViewAction Type="Button" Action="Exit">TEST</ViewAction>
		<ViewAction Type="Button" Action="Exit" Argument="0">TEST2</ViewAction>
	</View>
</Application>
```
