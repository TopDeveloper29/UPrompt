# [Back](https://github.com/TopDeveloper29/UPrompt/blob/Post/README.md) | Learn how to create HTML elements for UPprompt
Here you will learn how to properly name your custom elements in HTML to make them work with UPrompt. You will also learn about best practices for styling and adding custom behavior using JavaScript.
## View Item
For items that will only display data without interacting with UPrompt, you can simply write them as you would in a normal HTML page.

#### XML with HTML example:
```xml
<Application>
	<View>
		<ViewItem Type="Title">My Application Title</ViewItem>
		<br/><br/>
		<div style="color: red;">Some red text added using HTML/CSS in a div</div>
	</View>
</Application>
```

## Input Item
For an item that allows the user to enter or select a value, you must follow a pattern for the name and optionally add an InputHandler if you want to track the input change.

#### Rules to follow in property
| Property | Rule Description |
| -------- | ---------------- |
| Name     | The name must start with **INPUT_** followed by the **Id**. |
| Id       | The **Id** must be provided and match the one written in `Name`. |

If you do not follow the naming rule, the item will be displayed but cannot interact with UPrompt actions. If you add a handler, it will fail to track the input and may result in an error.

#### XML with HTML Input example:
```xml
<Application>
	<View>
		<ViewItem Type="Title">My Application Title</ViewItem>
		<input type="text" name="INPUT_MYID" id="MYID"/>
		<ViewAction Type="InputHandler" Action="YesNoDialog" Argument="Test of handler">MYID</ViewAction>
	</View>
</Application>
```

## Action Item
For an item that allows the user to run an action, such as a button, you need to follow some rules to make it work.

#### Rules to follow in property
| Property | Rule Description |
| -------- | ---------------- |
| Name     | The name must start with **Action_** followed by the **Id**, separated by an underscore, and ending with the **Action Name**. |
| Value    | The **Value** must be provided to pass arguments to the `Action` written in `Name`. |

If you do not follow the naming rule, the item will be displayed but cannot interact with UPrompt actions.

#### XML with HTML Button example:
```xml
<Application>
	<View>
		<ViewItem Type="Title">My Application Title</ViewItem>
		<button type="submit" id="MYID" name="Action_MYID_OkDialog" value="Argument passed to Action">Text displayed in button</button>
	</View>
</Application>
```

## Adding Style to Elements
If you want to add CSS styles for fine-tuning the position of UI elements or creating simple elements for use on a single page, you can create your own CSS file and link it using CSS `Setting`. If you want to create complex elements for use on multiple pages, you can either link your own CSS file using `Setting` or add your CSS class directly in **[Application_Directory]\Resources\Code\UTemplate.css**, making the class available on all pages of UPrompt without linking another file.

## Adding JavaScript to Elements
If you want to add JavaScript to an element, you can include it directly within the `View` by adding `<script></script>` HTML elements or by adding it directly in **[Application_Directory]\Resources\Code\UTemplate.html** or adding a link to a JavaScript file.

> Note: If you edit the template, it will be added/run on all pages.
