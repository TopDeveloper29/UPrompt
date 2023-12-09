# [Back](https://github.com/TopDeveloper29/UPrompt/blob/Post/README.md) | View
In this section, you will learn what are the diferent kind and type of built-it element you can use to create a user interphace in `View` node.

##ViewItem
###Spacer
Description: *This is for add space between 2 element in a `Row`* 
```xml
<ViewItem Type="Spacer"/>
```
###Title
**Description:** *This is a text element but of the size of title* 

**Aditional Properties:**

| Name | Description |
| ------------ | ------------ |
| Id  | This define Id in **HTML** element |
| Style  | You can add extra **CSS** style in the element  |
| Class  | Define the **CSS** class of the element  |

```xml
<ViewItem Type="Title">The title text that will be display in ui</ViewItem>
```
###Label
Description: *This is a text element* 

**Aditional Properties:**

| Name | Description |
| ------------ | ------------ |
| Id  | This define Id in **HTML** element |
| Style  | You can add extra **CSS** style in the element  |
| Class  | Define the **CSS** class of the element  |
```xml
<ViewItem Type="Label">The text that will be display in ui</ViewItem>
```
###LabelBox
Description: *This is a text element but with a border* 

**Aditional Properties:**

| Name | Description |
| ------------ | ------------ |
| Id  | This define Id in **HTML** element |
| Style  | You can add extra **CSS** style in the element  |
| Class  | Define the **CSS** class of the element  |
```xml
<ViewItem Type="Spacer"></ViewItem>
```
###Box
Description: *This is for add space between 2 element in a `Row`* 

**Aditional Properties:**

| Name | Description |
| ------------ | ------------ |
| Id  | This define Id in **HTML** element |
| Style  | You can add extra **CSS** style in the element  |
| Class  | Define the **CSS** class of the element  |
```xml
<ViewItem Type="Spacer"></ViewItem>
```
###Row
Description: *This is alow to hold multiple element in same line in user interphace* 
```xml
<ViewItem Type="Row">
	<ViewItem Type="Title">My Title</ViewItem>
	<ViewItem Type="Label">My Text</ViewItem>
</ViewItem>
```
##ViewInput
###TextBox
Description: 
```xml
<ViewItem Type="TextBox"/>
```
###LinesBox
Description:
```xml
<ViewItem Type="LinesBox"/>
```
###CheckBox
Description:
```xml
<ViewItem Type="CheckBox"/>
```
###DropDown
Description:
```xml
<ViewItem Type="DropDown"/>
```
##ViewAction
###Linker
Description:
```xml
<ViewItem Type="Linker"/>
```
###Button
Description:
```xml
<ViewItem Type="Button"/>
```
###InputHandler
Description:
```xml
<ViewItem Type="InputHandler"/>
```
###ViewLoad
Description:
```xml
<ViewItem Type="ViewLoad"/>
```
###VariableHandler
Description:
```xml
<ViewItem Type="VariableHandler"/>
```
