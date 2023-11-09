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
    internal static class HtmlXml
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
        private static int FallBackElementId = 0;
        
        internal static void GetNewFadeColor()
        {
            Fade_Back_Color = ColorTranslator.ToHtml(ControlPaint.Light(ColorTranslator.FromHtml(Back_Color),0.1f));
            Fade_Main_Color = ColorTranslator.ToHtml(ControlPaint.Light(ColorTranslator.FromHtml(Main_Color), 0.2f));
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
            foreach (string Key in Handler.InternalVariable.Keys)
            {
                string Value = Handler.InternalVariable[Key];
                ParsedText = ParsedText.Replace($"[{Key}]", Value);
            }
            return ParsedText;
        }
        public static void AddJsInputHandler(string ID)
        {
            string scriptContent = $"const Input_{ID} = document.getElementById(\"{ID}\");\n" +
                       $"Input_{ID}.addEventListener(\"change\", function() {{\n" +
                       $"const myForm = document.getElementById(\"UForm\");\r\nmyForm.submit();\r\n" +
                       "});";
            HtmlFromXml += $"<script>{scriptContent}</script>";
        }

        public static string GenrateHtmlFromXML(string XML)
        {
            XmlDocument doc = new XmlDocument(); doc.LoadXml(XML);
            XmlNode childNode = doc.DocumentElement;

            //global value that can be apply on all type
            string id = childNode.Attributes["Id"]?.Value; if (id == null) { id = FallBackElementId.ToString(); FallBackElementId++; }
            string ExtraStyle = childNode.Attributes["Style"]?.Value;
            // Vérifier le type de l'élément enfant
            switch (childNode.Name)
            {
                case "ViewSpacer":
                    HtmlFromXml += "<div class=\"Spacer\">.</div>";

                    break;
                case "ViewInput":
                    // Récupérer les attributs Type et Id
                    string input_type = childNode.Attributes["Type"].Value;
                    string InputValue = SpecialTextParse(childNode.InnerText);
                    if (Handler.InternalVariable.ContainsKey(id))
                    {
                        InputValue = SpecialTextParse(Handler.InternalVariable[id]);
                    }

                    string HandlerAction = childNode.Attributes["Action"]?.Value;
                    if (HandlerAction != null)
                    {
                        GenrateHtmlFromXML($"<ViewAction Type=\"InputHandler\" Action=\"{HandlerAction}\">{id}</ViewAction>");
                    }

                    switch (input_type)
                    {
                        default:
                        case "TextBox":
                            HtmlFromXml += $"<input style=\"{ExtraStyle}\" class=\"TextBox\" type=\"text\" name=\"INPUT_{id}\" id=\"{id}\" value=\"{InputValue}\"/>";
                            AddJsInputHandler(id);
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
                            HtmlFromXml += $"<div style=\"{ExtraStyle}\" id=\"{id}\" class=\"Title\">{InnerValue}</div>\n";
                            break;

                        case "Label":
                            HtmlFromXml += $"<div style=\"{ExtraStyle}\" id=\"{id}\" class=\"Label\">{InnerValue}</div>\n";
                            break;

                        case "LabelBox":
                            HtmlFromXml += $"<div style=\"{ExtraStyle}\" id=\"{id}\" class=\"Box\">{InnerValue}</div>\n";
                            break;
                        default:
                        case "Row":
                            HtmlFromXml += $"<div style=\"{ExtraStyle}\" id=\"{id}\" class=\"Row\">\n";
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
                        catch { MessageBox.Show("An action must be include = so in this format ACTION=VALUE", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                    }

                    // Traiter l'élément en fonction de son type et de son action
                    switch (action_type)
                    {
                        case "Linker":
                            HtmlFromXml += $"<input hidden=\"hidden\" class=\"InputHandler\" id=\"{id}\" name=\"Linker_{action_value}\" value=\"{action_name}\"/>\n";
                            break;
                        default: 
                        case "Button":
                            HtmlFromXml += $"<button style=\"{ExtraStyle}\" class=\"Button\" type=\"submit\" id=\"{id}\" name=\"{id}::ID::{action_name}\" value=\"{action_value}\">{ActionInnerValue}</button>\n";
                            break;
                        case "InputHandler":
                            HtmlFromXml += $"<input hidden=\"hidden\" class=\"InputHandler\" id=\"{id}\" name=\"InputHandler_{ActionInnerValue}::Action::{id}::ID::{action_name}\" value=\"{action_value}\"/>\n";
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
