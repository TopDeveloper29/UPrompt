using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using UPrompt.Internal;

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
        public static void ReloadView()
        {
            ClearHtml();
            UCommon.XmlDocument.Load(UCommon.Xml_Path);
            GenerateView(UCommon.XmlDocument.SelectSingleNode("/Application/View"));
            if (UCommon.Windows.htmlhandler.Source != null)
            {
                UCommon.Windows.htmlhandler.Reload();
            }
        }
        public static string NewInputValue(string type, string name, string id, string value)
        {
            if (type != null && name != null && id != null && value != null)
            {
                string json = $"{{\"type\": \"{type}\", \"name\": \"{name}\", \"id\": \"{id}\", \"value\": \"{value}\"}}";
                string encryptedJson = Encrypt(json, "UPromptKey2023");
                return encryptedJson;
            }
            else
            {
                UCommon.Error("You must provide 4 value for the function NewInputValue(type,name,id,value);", "Internal Error");
                return null;
            }
        }
        internal static void GenerateView(XmlNode viewNode)
        {
            ClearHtml();
            UCommon.DebugXmlLineNumber = (File.ReadAllLines(UCommon.Xml_Path).Length - USettings.Count) - 5;
            foreach (XmlNode childNode in viewNode.ChildNodes)
            {
                // this generate html and include System parsing for inner value and other html element
                HtmlFromXml = GenerateHtmlFromXML(childNode.OuterXml); 
            }

            if (HtmlFromXml.Length < 5) { HtmlFromXml = "=== THE VIEW IS EMPTY PLEASE FILL IT IN XML ==="; }
            HtmlFromXml = $"{CSSLink}\n{HtmlFromXml}";
            string Template = File.ReadAllText($@"{UCommon.Application_Path}\Resources\Code\UTemplate.html");
            string html = Template.Replace("=== XML CODE WILL GENERATE THIS VIEW ===", HtmlFromXml);

            File.WriteAllText($@"{UCommon.Application_Path}\Resources\Code\UView.html", ParseSettingsText(html));

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
                .Replace("#FONT_NAME#", USettings.Font_Name)
                .Replace("#WINDOWSOPENMODE#", USettings.WindowsOpenMode)
                .Replace("#WINDWOWSRESIZEMODE#", USettings.WindowsResizeMode)
                .Replace("#SHOWMINIMIZE#", USettings.ShowMinimize.ToString())
                .Replace("#SHOWMAXIMIZE#", USettings.ShowMaximize.ToString())
                .Replace("#SHOWCLOSE#", USettings.ShowClose.ToString())
                .Replace("#WIDTH#", USettings.Width.ToString())
                .Replace("#HEIGHT#", USettings.Height.ToString())
                .Replace("#PRODUCTION#", USettings.Production.ToString())

                ;

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
                    .Replace("{e}","=")
                    .Replace("{q}", "'")
                    .Replace("{AppPath}", UCommon.Application_Path)
                    .Replace("{AppPathWindows}", UCommon.Application_Path_Windows)
                    ;
                //Internal Input Variable Replace
                foreach (string Key in UCommon.Variable.Keys)
                {
                    string Value = UCommon.GetVariable(Key);
                    ParsedText = ParsedText.Replace($"[{Key}]", Value);
                }
                return ParsedText;
            }
            return Text;
        }
        internal static void AddJsInputHandler(string ID)
        {
            string scriptContent = $"const Input_{ID} = document.getElementById(\"{ID}\");\n" +
                       $"Input_{ID}.addEventListener(\"change\" , function() {{\n" +
                       $"const myForm = document.getElementById(\"UForm\");\r\n" +
                       $"var submitEvent = new Event('submit');myForm.dispatchEvent(submitEvent);\r\n" +
                       "});";
            HtmlFromXml += $"<script>{scriptContent}</script>";
        }
        private static string Encrypt(string plainText, string encryptionKey)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Pad the input data to a multiple of the block size
            int blockSize = 16;
            int paddingSize = blockSize - (plainTextBytes.Length % blockSize);
            byte[] paddedBytes = new byte[plainTextBytes.Length + paddingSize];
            Array.Copy(plainTextBytes, paddedBytes, plainTextBytes.Length);

            byte[] keyBytes = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }, 1000).GetBytes(32);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(BitConverter.GetBytes(aes.IV.Length), 0, sizeof(int));
                    memoryStream.Write(aes.IV, 0, aes.IV.Length);
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(paddedBytes, 0, paddedBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        byte[] cipherTextBytes = memoryStream.ToArray();
                        return Convert.ToBase64String(cipherTextBytes);
                    }
                }
            }
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

            string Action = childNode.Attributes["Action"]?.Value ?? "null";
            string DropList = childNode.Attributes["DropList"]?.Value;
            string Argument = childNode.Attributes["Argument"]?.Value ?? "";

            string Checked = childNode.Attributes["Checked"]?.Value ?? "False";

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

            if (!USettings.ElementsParsingSkip.Contains(Id))
            {
                // Check what kind of node is it 
                switch (childNode.Name.ToLower())
                {
                    case "viewinput":
                        //action that must be apply for all type of ViewInput
                        if (UCommon.GetVariable(Id) != null && UCommon.GetVariable(Id).Length > 0)
                        {
                            InnerValue = ParseSystemText(UCommon.Variable[Id]);
                        }
                        if (Action != null) { GenerateHtmlFromXML($"<ViewAction Type=\"InputHandler\" Action=\"{Action}\" Argument=\"{Argument}\">{Id}</ViewAction>"); }

                        switch (Type.ToLower())
                        {
                            default:
                            case "textbox":
                                HtmlFromXml += $"<input style=\"{ExtraStyle}\" class=\"TextBox {Class}\" type=\"text\" name=\"INPUT_{Id}\" Id=\"{Id}\" value=\"{InnerValue}\"/>";
                                break;
                            case "linesbox":
                                HtmlFromXml += $"<textarea style=\"{ExtraStyle}\" class=\"TextBox {Class}\" name=\"INPUT_{Id}\" Id=\"{Id}\">{InnerValue}</textarea>";
                                break;
                            case "checkbox":
                                if (UCommon.GetVariable($"CheckBox_{Id}") != null)
                                {
                                    Checked = UCommon.GetVariable($"CheckBox_{Id}");
                                    Console.WriteLine($"CheckBox_{Id}" + " get value is " + Checked);
                                }
                                string Checked_Text = "";
                                if (Checked != null)
                                {
                                    if (bool.Parse(Checked))
                                    { Checked_Text = "checked"; }
                                }
                                

                                HtmlFromXml += "\n" +
                                    $"<label style=\"{ExtraStyle}\" class=\"label-checkbox {Class}\">\r\n" +
                                    $"  <input type=\"checkbox\" id=\"{Id}\" name=\"INPUT_{Id}\" value=\"{InnerValue}\" {Checked_Text} onclick=\"saveCheckboxStatus(this)\"/>\r\n" +
                                    "  <span class=\"checkmark\"></span>\r\n" +
                                    $"{InnerValue}\r\n" +
                                    "</label>\r\n" +
                                    $"<input id=\"CheckBox_{Id}\" name=\"CheckBox_{Id}\" Value=\"{Checked}\" hidden/>";
                                break;
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
                        AddJsInputHandler(Id);
                        break;
                    case "viewitem":
                        switch (Type.ToLower())
                        {
                            case "spacer":
                                HtmlFromXml += "<div class=\"Spacer\">.</div>";
                                break;
                            case "title":
                                HtmlFromXml += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Title {Class}\">{InnerValue}</div>\n";
                                break;
                            case "label":
                                HtmlFromXml += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Label {Class}\">{InnerValue}</div>\n";
                                break;
                            case "labelbox":
                                HtmlFromXml += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"LabelBox {Class}\">{InnerValue}</div>\n";
                                break;
                            case "box":
                                HtmlFromXml += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Box {Class}\">\n";
                                foreach (XmlNode rowChildNode in childNode.ChildNodes)
                                {
                                    GenerateHtmlFromXML(rowChildNode.OuterXml);
                                }
                                HtmlFromXml += "</div>\n";
                                break;
                            case "row":
                                HtmlFromXml += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Row {Class}\">\n";
                                foreach (XmlNode rowChildNode in childNode.ChildNodes)
                                {
                                    GenerateHtmlFromXML(rowChildNode.OuterXml);
                                }
                                HtmlFromXml += "</div>\n";
                                break;
                        }
                        break;
                    case "viewaction":
                        switch (Type.ToLower())
                        {
                            case "linker":
                                string source = childNode.Attributes["Source"]?.Value ?? "default";
                                string target = childNode.Attributes["Target"]?.Value ?? "default";
                                if (source != null && target != null)
                                {
                                    HtmlFromXml += $"<input hidden=\"hidden\" Id=\"{Id}\" name=\"Action_{Id}\" value=\"{NewInputValue("Linker", Action, Id, $"{source},{target}")}\"/>\n";
                                }
                                else
                                {
                                    UCommon.Warning($"Linker must include property Source and Target that is not empty (xml line: {UCommon.DebugXmlLineNumber})", "Linker Error");
                                }
                                break;
                            default:
                            case "button":
                                HtmlFromXml += $"<button style=\"{ExtraStyle}\" class=\"Button {Class}\" type=\"submit\" Id=\"{Id}\" name=\"Action_{Id}\" value=\"{NewInputValue("ui",Action,Id,Argument)}\">{InnerValue}</button>\n";
                                break;
                            case "inputhandler":
                                try
                                {
                                    if (!UCommon.TrackedVariable.TryGetValue(InnerValue, out ActionStorage acs))
                                    {
                                        if (acs == null)
                                        {
                                            if (UCommon.GetVariable(InnerValue) == null) { UCommon.SetVariable(InnerValue, ""); }
                                            UCommon.TrackedVariable.Add(InnerValue, new ActionStorage(Action, Argument, UCommon.GetVariable(InnerValue), InnerValue));

                                        }
                                    }
                                }
                                catch (Exception ex) { UCommon.Error(ex.Message); }
                                break;
                            case "viewload":
                                HtmlFromXml += $"<input hidden=\"hidden\" name=\"Action_{Id}\" value=\"{NewInputValue("ViewLoad", Action, Id, Argument)}\"/>\n";
                                break;
                            case "variablehandler":
                                try
                                {
                                    if (!UCommon.TrackedVariable.TryGetValue(InnerValue, out ActionStorage acs))
                                    {
                                        UCommon.TrackedVariable.Add(InnerValue, new ActionStorage(Action, Argument, UCommon.GetVariable(InnerValue),Id));
                                    }
                                }
                                catch (Exception ex) { UCommon.Error(ex.Message); }
                                break;
                        }
                        break;
                    default:
                        HtmlFromXml += ParseSystemText($"{childNode.OuterXml}\n");

                        break;
                }
            }
            return HtmlFromXml;
        }

    }
}
