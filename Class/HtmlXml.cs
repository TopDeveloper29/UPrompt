using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using UPrompt.Class;

namespace UPrompt.Class
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
                .Replace("{n}", "\n")
                ;
            //Internal Input Variable Replace
            foreach (string Key in Common.InternalVariable.Keys)
            {
                string Value = Common.InternalVariable[Key];
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
            Common.DebugXmlLineNumber++;
            XmlDocument doc = new XmlDocument(); doc.LoadXml(XML);
            XmlNode childNode = doc.DocumentElement;

            //get all atribute that can exist in all type
            string Id = childNode.Attributes["Id"]?.Value; if (Id == null) { Id = FallBackElementId.ToString(); FallBackElementId++; }
            string Type = childNode.Attributes["Type"]?.Value;
            string InnerValue = SpecialTextParse(childNode.InnerText);
         
            string Action = childNode.Attributes["Action"]?.Value;
            string Argument = childNode.Attributes["Argument"]?.Value;

            string ExtraStyle = childNode.Attributes["Style"]?.Value;

            // Check what kind of node is it 
            switch (childNode.Name)
            {
                case "ViewSpacer":
                    HtmlFromXml += "<div class=\"Spacer\">.</div>";

                    break;
                case "ViewInput":
                    //action that must be apply for all type of ViewInput
                    if (Common.InternalVariable.ContainsKey(Id)){ InnerValue = SpecialTextParse(Common.InternalVariable[Id]); }
                    if (Action != null){ GenrateHtmlFromXML($"<ViewAction Type=\"InputHandler\" Action=\"{Action}\" Argument=\"{Argument}\">{Id}</ViewAction>"); }

                    switch (Type)
                    {
                        default:
                        case "TextBox":
                            HtmlFromXml += $"<input style=\"{ExtraStyle}\" class=\"TextBox\" type=\"text\" name=\"INPUT_{Id}\" Id=\"{Id}\" value=\"{InnerValue}\"/>";
                            AddJsInputHandler(Id);
                            break;
                    }
                    break;
                case "ViewItem":
                    switch (Type)
                    {
                        case "Title":
                            HtmlFromXml += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Title\">{InnerValue}</div>\n";
                            break;

                        case "Label":
                            HtmlFromXml += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Label\">{InnerValue}</div>\n";
                            break;

                        case "LabelBox":
                            HtmlFromXml += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"LabelBox\">{InnerValue}</div>\n";
                            break;
                        default:
                        case "Row":
                            HtmlFromXml += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Row\">\n";
                            foreach (XmlNode rowChildNode in childNode.ChildNodes)
                            {
                                GenrateHtmlFromXML(rowChildNode.OuterXml);
                            }
                            HtmlFromXml += "</div>\n";
                            break;
                    }
                    break;
                case "ViewAction":
                    switch (Type)
                    {
                        case "Linker":
                            string source = childNode.Attributes["Source"]?.Value;
                            string target = childNode.Attributes["Target"]?.Value;
                            if (source != null && target != null)
                            {
                                HtmlFromXml += $"<input hidden=\"hidden\" class=\"InputHandler\" Id=\"{Id}\" name=\"Linker_{source}\" value=\"{target}\"/>\n";
                            }
                            else
                            {
                                Common.Warning($"Linker must include property Source and Target that is not empty (xml line: {Common.DebugXmlLineNumber})","Linker Error");
                            }
                            break;
                        default: 
                        case "Button":
                            HtmlFromXml += $"<button style=\"{ExtraStyle}\" class=\"Button\" type=\"submit\" Id=\"{Id}\" name=\"{Id}::ID::{Action}\" value=\"{Argument}\">{InnerValue}</button>\n";
                            break;
                        case "InputHandler":
                            HtmlFromXml += $"<input hidden=\"hidden\" class=\"InputHandler\" Id=\"{Id}\" name=\"InputHandler_{InnerValue}::Action::{Id}::ID::{Action}\" value=\"{Argument}\"/>\n";
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
