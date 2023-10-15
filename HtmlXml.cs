using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace UPrompt
{
    internal class HtmlXml
    {
        internal static string HtmlFromXml = "";
        internal static string Text_Color = "#000000";
        internal static string Back_Color = "#ffffff";
        internal static string Main_Color = "#2d89e5";
        internal static string Text_Main_Color = "#ffffff";
        internal static string Fade_Back_Color = "#ffffff";
        internal static string Fade_Main_Color = "#ffffff";
        internal static int Current_Width = 0;
        internal static string Item_Margin = "10px";
        internal static string Font_Name = "Arial";
        internal static void GetNewFadeColor()
        {
            Fade_Back_Color = ColorTranslator.ToHtml(ControlPaint.Light(ColorTranslator.FromHtml(Back_Color)));
            Fade_Main_Color = ColorTranslator.ToHtml(ControlPaint.Light(ColorTranslator.FromHtml(Main_Color)));
        }

        public static string SettingsTextParse(string Text)
        {
            string ParsedText = Text
                .Replace("#TEXT_COLOR#", Text_Color)
                .Replace("#MAIN_COLOR#", Main_Color)
                .Replace("#BACKGROUND_COLOR#", Back_Color)
                .Replace("#FADE_BACKGROUND_COLOR#", Fade_Back_Color)
                .Replace("#FADE_MAIN_COLOR#", Fade_Main_Color)
                .Replace("#ITEM_MARGIN#", Item_Margin)
                .Replace("#MAIN_TEXT_COLOR#", Text_Main_Color)
                .Replace("#FONT_NAME#", Font_Name);

            return ParsedText;
        }
        public static string SpecialTextParse(string Text)
        {
            string ParsedText = Text
                .Replace("{USER}", Environment.UserName)
                .Replace("{DEVICE}", Environment.MachineName)
                ;
            //Internal Input Variable Replace
            foreach (string Key in InternalProcess.InternalVariable.Keys)
            {
                string Value = InternalProcess.InternalVariable[Key];
                ParsedText = ParsedText.Replace($"[{Key}]", Value);
            }
            return ParsedText;
        }
        public static void AddJsHandler(string ID)
        {
            string scriptContent = $"const Input_{ID} = document.getElementById(\"{ID}\");\n" +
                       $"Input_{ID}.addEventListener(\"change\", function() {{\n" +
                       $"const myForm = document.getElementById(\"UForm\");\r\nmyForm.submit();\r\n" +
                       "});";
            HtmlFromXml += $"<script>{scriptContent}</script>";
        }

        public static string GenrateHtmlFromXML(string XML)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XML);

            XmlNode childNode = doc.DocumentElement;
            // Vérifier le type de l'élément enfant
            switch (childNode.Name)
            {
                case "Spacer":
                    break;
                case "ViewInput":
                    // Récupérer les attributs Type et Id
                    string input_type = childNode.Attributes["Type"].Value;
                    string id = childNode.Attributes["Id"]?.Value;
                    string InputValue = SpecialTextParse(childNode.InnerText);
                    if (InternalProcess.InternalVariable.ContainsKey(id))
                    {
                        InputValue = SpecialTextParse(InternalProcess.InternalVariable[id]);
                    }
                    switch (input_type)
                    {
                        case "TextBox":
                            HtmlFromXml += $"<input class=\"TextBox\" type=\"text\" name=\"INPUT_{id}\" id=\"{id}\" value=\"{InputValue}\"/>";
                            AddJsHandler(id);
                            break;
                    }
                    break;
                case "ViewItem":
                    // Récupérer les attributs Type et Action
                    string type = childNode.Attributes["Type"].Value;
                    string InnerValue = SpecialTextParse(childNode.InnerText);

                    switch (type)
                    {
                        case "Title":
                            HtmlFromXml += $"<div class=\"Title\">{InnerValue}</div>\n";
                            break;

                        case "Label":
                            HtmlFromXml += $"<div class=\"Label\">{InnerValue}</div>\n";
                            break;

                        case "LabelBox":
                            HtmlFromXml += $"<div class=\"Box\">{InnerValue}</div>\n";
                            break;
                        case "Row":
                            HtmlFromXml += $"<div class=\"Row\">\n";
                            foreach (XmlNode rowChildNode in childNode.ChildNodes)
                            {
                                GenrateHtmlFromXML(rowChildNode.OuterXml);
                            }
                            HtmlFromXml += "</div>\n";
                            break;
                    }
                    break;
                case "ViewAction":
                    // Récupérer les attributs Type et Action
                    string action_type = childNode.Attributes["Type"].Value;
                    string action = childNode.Attributes["Action"]?.Value;
                    string action_id = childNode.Attributes["Id"]?.Value;
                    string action_name = "";
                    string action_value = "";
                    string ActionInnerValue = SpecialTextParse(childNode.InnerText);
                    if (action != null)
                    {
                        try
                        {
                            action_name = action.Split('=')[0];
                            action_value = action.Split('=')[1];
                        }
                        catch (Exception ex) { MessageBox.Show("An action must be include = so in this format ACTION=VALUE", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                    }


                    // Traiter l'élément en fonction de son type et de son action
                    switch (action_type)
                    {
                        case "Linker":
                            break;
                        case "Button":
                            HtmlFromXml += $"<button class=\"Button\" type=\"submit\" id=\"{action_id}\" name=\"{action_name}\" value=\"{action_value}\">{ActionInnerValue}</button>\n";
                            break;
                        case "InputHandler":
                            HtmlFromXml += $"<input hidden=\"hidden\" class=\"InputHandler\" id=\"{action_id}\" name=\"InputHandler_{ActionInnerValue}:Action:{action_name}\" value=\"{action_value}\"/>\n";
                            break;
                    }
                    break;

                default:
                    HtmlFromXml += $"{childNode.OuterXml}\n";
                    break;
            }

            return HtmlFromXml;
        }
    }
}
