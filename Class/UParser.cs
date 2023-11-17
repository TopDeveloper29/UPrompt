using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shapes;
using System.Xml;
using UPrompt.Class;

namespace UPrompt.Class
{
    public static class UParser
    {
        internal static string CSSLink = "";
        private static string HtmlFromXml = "";
        private static int FallBackElementId = 0;

        public static void ClearHtml()
        {
            HtmlFromXml = null;
        }
        internal static void GenerateView(XmlNode viewNode)
        {
            HtmlFromXml = null;
            UCommon.DebugXmlLineNumber = (File.ReadAllLines(UCommon.Xml_Path).Length - USettings.Count) - 5;
            foreach (XmlNode childNode in viewNode.ChildNodes)
            {
                HtmlFromXml = GenerateHtmlFromXML(childNode.OuterXml);
            }
            if (HtmlFromXml.Length < 5) { HtmlFromXml = "=== THE VIEW IS EMPTY PLEASE FILL IT IN XML ==="; }
            HtmlFromXml = $"{CSSLink}\n{HtmlFromXml}";
            string Template = File.ReadAllText($@"{UCommon.Application_Path}\Resources\Code\Index.html");
            string html = Template.Replace("=== XML CODE WILL GENERATE THIS VIEW ===", HtmlFromXml);

            File.WriteAllText($@"{UCommon.Application_Path}\Resources\Code\View.html", UParser.ParseSettingsText(html));

        }
        public static string ParseSettingsText(string Text)
        {
            string ParsedText = Text
                .Replace("#TEXT_COLOR#", USettings.Text_Color)
                .Replace("#MAIN_COLOR#", USettings.Main_Color)
                .Replace("#BACKGROUND_COLOR#", USettings.Back_Color)
                .Replace("#FADE_BACKGROUND_COLOR#", USettings.Fade_Back_Color)
                .Replace("#FADE_MAIN_COLOR#", USettings.Fade_Main_Color)
                .Replace("#ITEM_MARGIN#", USettings.Item_Margin)
                .Replace("#MAIN_TEXT_COLOR#", USettings.Text_Main_Color)
                .Replace("#FONT_NAME#", USettings.Font_Name);

            return ParsedText;
        }
        public static string ParseSystemText(string Text)
        {
            if (USettings.SkipSystemParsing == false)
            {
                string ParsedText = Text
                    .Replace("{USER}", Environment.UserName)
                    .Replace("{DEVICE}", Environment.MachineName)
                    .Replace("{n}", "\n")
                    .Replace("{c}", ",")
                    .Replace("{Application_Path}", UCommon.Application_Path)
                    .Replace("{AppPath}", UCommon.Application_Path)
                    ;
                //Internal Input Variable Replace
                foreach (string Key in UCommon.Variable.Keys)
                {
                    string Value = UCommon.Variable[Key];
                    ParsedText = ParsedText.Replace($"[{Key}]", Value);
                }
                return ParsedText;
            }
            return Text;
        }
        internal static void AddJsInputHandler(string ID)
        {
            string scriptContent = $"const Input_{ID} = document.getElementById(\"{ID}\");\n" +
                       $"Input_{ID}.addEventListener(\"change\", function() {{\n" +
                       $"const myForm = document.getElementById(\"UForm\");\r\nmyForm.submit();\r\n" +
                       "});";
            HtmlFromXml += $"<script>{scriptContent}</script>";
        }

        public static string GenerateHtmlFromXML(string XML)
        {
            UCommon.DebugXmlLineNumber++;
            XmlDocument doc = new XmlDocument(); doc.LoadXml(XML);
            XmlNode childNode = doc.DocumentElement;

            //get all atribute that can exist in all type
            string Id = childNode.Attributes["Id"]?.Value; if (Id == null) { Id = FallBackElementId.ToString(); FallBackElementId++; }
            string Type = childNode.Attributes["Type"]?.Value;
            string InnerValue = ParseSystemText(childNode.InnerText);

            string Action = childNode.Attributes["Action"]?.Value;
            string DropList = childNode.Attributes["DropList"]?.Value;
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
                if (!Directory.Exists($@"{UCommon.Application_Path}\Resources\Icon\")) { Directory.CreateDirectory($@"{UCommon.Application_Path}\Resources\Icon\"); }
                try
                {
                    ImagePath = ImageObject.Split(',')[0];
                    ImageSize = ImageObject.Split(',')[1];
                    ImageAutoColor = ImageObject.Split(',')[2];
                }
                catch { UCommon.Warning($"ImageObject property should be write like ImageObject=\"Path (string),Size (integer),AutoColor (boolean)\""); }
                if (UImage.IsUrl(ImagePath))
                {
                    RealImagePath = $@"{UCommon.Application_Path}\Resources\Icon\{UImage.GetImageNameFromUrl(ImagePath)}";
                    UImage.DownloadImage(ImagePath, RealImagePath);
                    
                }
                else
                {
                    RealImagePath = $@"{UCommon.Application_Path}\Resources\Icon\{UImage.GetImageNameFromLocalPath(ImagePath)}";
                    File.Move(ImagePath, RealImagePath);
                }

                if (UImage.IsDark(UCommon.Windows.TitleBar.BackColor) && ImageAutoColor.ToLower().Contains("true"))
                {
                    Image TempImage = Image.FromFile(RealImagePath);
                    
                    UImage.ReverseImageColors(TempImage, RealImagePath);
                }
                ExtraStyle += $"background-image: url('{UCommon.Application_Path}Resources/Icon/{UImage.GetImageNameFromLocalPath(RealImagePath)}');background-size: {ImageSize}%;background-repeat: no-repeat;background-position: center;";
            }

            // Check what kind of node is it 
            switch (childNode.Name)
            {
                case "ViewSpacer":
                    HtmlFromXml += "<div class=\"Spacer\">.</div>";

                    break;
                case "ViewInput":
                    //action that must be apply for all type of ViewInput
                    if (UCommon.Variable.ContainsKey(Id)){ InnerValue = ParseSystemText(UCommon.Variable[Id]); }
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
                        case "DropDown":
                        case "Drop":
                        case "dropdown":
                            string url = "https://static.thenounproject.com/png/1590826-200.png";
                            string RealDropImagePath = $@"{UCommon.Application_Path}\Resources\Icon\{UImage.GetImageNameFromUrl(url)}";
                            UImage.DownloadImage(url, RealDropImagePath);
                            if (UImage.IsDark(UCommon.Windows.TitleBar.BackColor))
                            {
                                Image TempImage = Image.FromFile(RealDropImagePath);
                                UImage.ReverseImageColors(TempImage, RealDropImagePath);
                            }
                            string dropstyle = $"background-image: url('{UCommon.Application_Path}Resources/Icon/{UImage.GetImageNameFromLocalPath(RealDropImagePath)}');";

 
                            HtmlFromXml += $"<div style=\"{ExtraStyle}\" class=\"dropdown {Class}\">\n";
                            HtmlFromXml += $"<input style=\"{dropstyle}\" readonly type=\"text\" name=\"INPUT_{Id}\" Id=\"{Id}\" value=\"{InnerValue}\"/>\n";
                            HtmlFromXml += $"<div class=\"dropdown-content\">\n";
                            if (DropList == null)
                            {
                                HtmlFromXml += $"<a href=\"#\">{InnerValue}</a>\n";
                            }
                            else
                            {
                                foreach (string line in DropList.Split(','))
                                {
                                    HtmlFromXml += $"<a href=\"#\">{line}</a>\n";
                                }
                            }
                            HtmlFromXml += $"</div>\n";
                            HtmlFromXml += $"</div>\n";
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
                                UCommon.Warning($"Linker must include property Source and Target that is not empty (xml line: {UCommon.DebugXmlLineNumber})", "Linker Error");
                            }
                            break;
                        default:
                        case "Button":
                            HtmlFromXml += $"<button style=\"{ExtraStyle}\" class=\"Button {Class}\" type=\"submit\" Id=\"{Id}\" name=\"{Id}::ID::{Action}\" value=\"{Argument}\">{InnerValue}</button>\n";
                            break;
                        case "InputHandler":
                            HtmlFromXml += $"<input hidden=\"hidden\" Id=\"{Id}\" name=\"InputHandler_{InnerValue}::Action::{Id}::ID::{Action}\" value=\"{Argument}\"/>\n";
                            break;
                        case "ViewLoad":
                            HtmlFromXml += $"<input hidden=\"hidden\" name=\"OnLoad_{Action}\" value=\"{Argument}\"/>\n";
                            break;
                        case "VariableHandler":
                            try
                            {
                                if (!UCommon.TrackedVariable.TryGetValue(InnerValue,out ActionStorage acs))
                                {
                                    UCommon.TrackedVariable.Add(InnerValue, new ActionStorage(Action, Argument,UCommon.GetVariable(InnerValue)));
                                }
                            }
                            catch(Exception ex) { MessageBox.Show(ex.Message); }
                            break;
                    }
                    break;
                default:
                    HtmlFromXml += ParseSystemText($"{childNode.OuterXml}\n");

                    break;
            }
            return HtmlFromXml;
        }
    
    }
}
