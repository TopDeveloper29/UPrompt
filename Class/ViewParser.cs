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
        internal static string CSSLink = "";
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
            HtmlFromXml = $"{CSSLink}\n{HtmlFromXml}";
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
                .Replace("{c}",",")
                .Replace("{Application_Path}",Common.Application_Path)
                .Replace("{AppPath}", Common.Application_Path)
                ;
            //Internal Input Variable Replace
            foreach (string Key in Common.Variable.Keys)
            {
                string Value = Common.Variable[Key];
                ParsedText = ParsedText.Replace($"[{Key}]", Value);
            }
            return ParsedText;
        }
        internal static void AddJsXmlEditor(string Id)
        {
            string scriptContent = "    // Define custom keywords\r\n" +
                "var myKeywords = [\"<Application>\", \"<Settings>\", \"<View>\", \"<ViewItem>\", \"<ViewInput>\", \"<ViewAction>\"];\r\n\r\n" +
                "// Initialize the CodeMirror editor with the hint addon\r\n" +
                $"    var editor = CodeMirror.fromTextArea(document.getElementById(\"{Id}\"), {{\r\n" +
                "      lineNumbers: true,\r\n" +
                "      mode: \"xml\",\r\n" +
                "      extraKeys: {\r\n" +
                "        Tab: \"autocomplete\" // Enable autocomplete on Tab key\r\n" +
                "      },\r\n" +
                "      hintOptions: {\r\n" +
                "        hint: function(editor) {\r\n" +
                "          // Get the current cursor position\r\n" +
                "          var cur = editor.getCursor();\r\n\r\n" +
                "          // Get the current line and text before the cursor\r\n" +
                "          var line = editor.getLine(cur.line);\r\n" +
                "          var start = cur.ch;\r\n" +
                "          while (start && /\\w/.test(line.charAt(start - 1))) --start;\r\n" +
                "          var prefix = line.slice(start, cur.ch);\r\n\r\n" +
                "          // Return a list of suggestions that start with the prefix\r\n" +
                "          return {\r\n            list: myKeywords.filter(function(keyword) {\r\n" +
                "              return keyword.startsWith(prefix);\r\n" +
                "            }),\r\n" +
                "            from: CodeMirror.Pos(cur.line, start),\r\n" +
                "            to: CodeMirror.Pos(cur.line, cur.ch)\r\n" +
                "          };\r\n" +
                "        }\r\n" +
                "      }\r\n" +
                "    });";
            HtmlFromXml += $"<script>{scriptContent}</script>";
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
            string Class = childNode.Attributes["Class"]?.Value;
            if (Class == null) { Class = ""; }
            string ImageObject = childNode.Attributes["Image"]?.Value;
            string ImagePath = "";
            string ImageSize = "";
            string ImageAutoColor = "";
            string RealImagePath = "";
            if (ImageObject != null)
            {
                if (!Directory.Exists($@"{Common.Application_Path}\Resources\Icon\")) { Directory.CreateDirectory($@"{Common.Application_Path}\Resources\Icon\"); }
                try
                {
                    ImagePath = ImageObject.Split(',')[0];
                    ImageSize = ImageObject.Split(',')[1];
                    ImageAutoColor = ImageObject.Split(',')[2];
                }
                catch { Common.Warning($"ImageObject property should be write like ImageObject=\"Path (string),Size (integer),AutoColor (boolean)\""); }
                if (ImageParser.IsUrl(ImagePath))
                {
                    RealImagePath = $@"{Common.Application_Path}\Resources\Icon\{ImageParser.GetImageNameFromUrl(ImagePath)}";
                    ImageParser.DownloadImage(ImagePath, RealImagePath);
                    
                }
                else
                {
                    RealImagePath = $@"{Common.Application_Path}\Resources\Icon\{ImageParser.GetImageNameFromLocalPath(ImagePath)}";
                    File.Move(ImagePath, RealImagePath);
                }

                if (ImageParser.IsDark(Common.Windows.TitleBar.BackColor) && ImageAutoColor.ToLower().Contains("true"))
                {
                    Image TempImage = Image.FromFile(RealImagePath);
                    
                    ImageParser.ReverseImageColors(TempImage, RealImagePath);
                }
                ExtraStyle += $"background-image: url('{Common.Application_Path}Resources/Icon/{ImageParser.GetImageNameFromLocalPath(RealImagePath)}');background-size: {ImageSize}%;background-repeat: no-repeat;background-position: center;";
            }

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
                            HtmlFromXml += $"<input style=\"{ExtraStyle}\" class=\"TextBox {Class}\" type=\"text\" name=\"INPUT_{Id}\" Id=\"{Id}\" value=\"{InnerValue}\"/>";
                            AddJsInputHandler(Id);
                            break;
                        case "LinesBox":
                            HtmlFromXml += $"<textarea style=\"{ExtraStyle}\" class=\"TextBox {Class}\" name=\"INPUT_{Id}\" Id=\"{Id}\">{InnerValue}</textarea>";
                            AddJsInputHandler(Id);
                            break;
                        case "Designer":
                            HtmlFromXml += $"<div style=\"margin:1%;width: 99%;{ExtraStyle}\" class=\"TextBox {Class}\"><textarea name=\"INPUT_{Id}\" id=\"{Id}\">{InnerValue}</textarea></div>";
                            AddJsXmlEditor(Id);
                            break;
                    }
                    break;
                case "ViewItem":
                    switch (Type)
                    {
                        case "Title":
                            HtmlFromXml += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Title {Class}\">{InnerValue}</div>\n";
                            break;

                        case "Label":
                            HtmlFromXml += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Label {Class}\">{InnerValue}</div>\n";
                            break;

                        case "LabelBox":
                            HtmlFromXml += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"LabelBox {Class}\">{InnerValue}</div>\n";
                            break;
                        default:
                        case "Row":
                            HtmlFromXml += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Row {Class}\">\n";
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
                                HtmlFromXml += $"<input hidden=\"hidden\" Id=\"{Id}\" name=\"Linker_{source}\" value=\"{target}\"/>\n";
                            }
                            else
                            {
                                Common.Warning($"Linker must include property Source and Target that is not empty (xml line: {Common.DebugXmlLineNumber})", "Linker Error");
                            }
                            break;
                        default:
                        case "Button":
                            HtmlFromXml += $"<button style=\"{ExtraStyle}\" class=\"Button {Class}\" type=\"submit\" Id=\"{Id}\" name=\"{Id}::ID::{Action}\" value=\"{Argument}\">{InnerValue}</button>\n";
                            break;
                        case "InputHandler":
                            HtmlFromXml += $"<input hidden=\"hidden\" Id=\"{Id}\" name=\"InputHandler_{InnerValue}::Action::{Id}::ID::{Action}\" value=\"{Argument}\"/>\n";
                            break;
                        case "VariableHandler":
                            try
                            {
                                if (!Common.TrackedVariable.TryGetValue(InnerValue,out ActionStorage acs))
                                {
                                    Common.TrackedVariable.Add(InnerValue, new ActionStorage(Action, Argument,Common.GetVariable(InnerValue)));
                                }
                            }
                            catch(Exception ex) { MessageBox.Show(ex.Message); }
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
