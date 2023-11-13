﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using UPrompt.Class;

namespace UPrompt.Class
{
    public static class ViewParser
    {
        private static string HtmlFromXml = "";
        private static int FallBackElementId = 0;

        internal static void GenerateView(XmlNode viewNode)
        {
            HtmlFromXml = null;
            Common.DebugXmlLineNumber = (File.ReadAllLines(Common.Xml_Path).Length - Settings.Count) - 5;
            foreach (XmlNode childNode in viewNode.ChildNodes)
            {
                HtmlFromXml = GenerateHtmlFromXML(childNode.OuterXml);
            }
            if (HtmlFromXml.Length < 5) { HtmlFromXml = "=== THE VIEW IS EMPTY PLEASE FILL IT IN XML ==="; }

            string Template = File.ReadAllText($@"{Common.Application_Path}\Resources\Code\Index.html");
            string html = Template.Replace("=== XML CODE WILL GENERATE THIS VIEW ===", HtmlFromXml);

            File.WriteAllText($@"{Common.Application_Path}\Resources\Code\View.html", ViewParser.ParseSettingsText(html));

        }
        public static string ParseSettingsText(string Text)
        {
            string ParsedText = Text
                .Replace("#TEXT_COLOR#", Settings.Text_Color)
                .Replace("#MAIN_COLOR#", Settings.Main_Color)
                .Replace("#BACKGROUND_COLOR#", Settings.Back_Color)
                .Replace("#FADE_BACKGROUND_COLOR#", Settings.Fade_Back_Color)
                .Replace("#FADE_MAIN_COLOR#", Settings.Fade_Main_Color)
                .Replace("#ITEM_MARGIN#", Settings.Item_Margin)
                .Replace("#MAIN_TEXT_COLOR#", Settings.Text_Main_Color)
                .Replace("#FONT_NAME#", Settings.Font_Name);

            return ParsedText;
        }
        public static string ParseSystemText(string Text)
        {
            string ParsedText = Text
                .Replace("{USER}", Environment.UserName)
                .Replace("{DEVICE}", Environment.MachineName)
                .Replace("{n}", "\n")
                ;
            //Internal Input Variable Replace
            foreach (string Key in Common.Variable.Keys)
            {
                string Value = Common.Variable[Key];
                ParsedText = ParsedText.Replace($"[{Key}]", Value);
            }
            return ParsedText;
        }
        internal static void AddJsInputHandler(string ID)
        {
            string scriptContent = $"const Input_{ID} = document.getElementById(\"{ID}\");\n" +
                       $"Input_{ID}.addEventListener(\"change\", function() {{\n" +
                       $"const myForm = document.getElementById(\"UForm\");\r\nmyForm.submit();\r\n" +
                       "});";
            HtmlFromXml += $"<script>{scriptContent}</script>";
        }
        internal static string GenerateHtmlFromXML(string XML)
        {
            Common.DebugXmlLineNumber++;
            XmlDocument doc = new XmlDocument(); doc.LoadXml(XML);
            XmlNode childNode = doc.DocumentElement;

            //get all atribute that can exist in all type
            string Id = childNode.Attributes["Id"]?.Value; if (Id == null) { Id = FallBackElementId.ToString(); FallBackElementId++; }
            string Type = childNode.Attributes["Type"]?.Value;
            string InnerValue = ParseSystemText(childNode.InnerText);
         
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
                    if (Common.Variable.ContainsKey(Id)){ InnerValue = ParseSystemText(Common.Variable[Id]); }
                    if (Action != null){ GenerateHtmlFromXML($"<ViewAction Type=\"InputHandler\" Action=\"{Action}\" Argument=\"{Argument}\">{Id}</ViewAction>"); }

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
                                GenerateHtmlFromXML(rowChildNode.OuterXml);
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