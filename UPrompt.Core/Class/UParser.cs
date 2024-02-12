using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace UPrompt.Core
{
    public static class UParser
    {
        internal static string CSSLink = "";
        public static string NewInputValue(string type, string name, string id, string value)
        {
            if (type != null && name != null && id != null && value != null)
            {
                string json = $"{{\"type\": \"{type}\", \"name\": \"{name}\", \"id\": \"{id}\", \"value\": \"{value}\"}}";
                string encryptedJson = Encrypt(json, $"{DateTime.Now.Day}__U+{DateTime.Now.DayOfWeek}+Prompt{DateTime.Now.Year}__{DateTime.Now.Month}");
                return encryptedJson;
            }
            else
            {
                UCommon.Error("You must provide 4 value for the function NewInputValue(type,name,id,value);", "Internal Error");
                return null;
            }
        }

        public enum ParseMode
        {
            Setting,
            System,
            User,
            Both,
            All
        }

        public static string ParseText(string Text, ParseMode Mode)
        {
            string ParsedText = Text;
            if (Mode == ParseMode.Setting || Mode == ParseMode.All)
            {
                ParsedText = Text
               .Replace("#TEXT_COLOR#", USettings.Text_Color)
               .Replace("#BACKGROUND_COLOR#", USettings.Background_Color)
               .Replace("#ACCENT_COLOR#", USettings.Accent_Color)
               .Replace("#ACCENT_TEXT_COLOR#", USettings.Text_Accent_Color)
               .Replace("#FADE_BACKGROUND_COLOR#", USettings.Fade_Background_Color)
               .Replace("#FADE_ACCENT_COLOR#", USettings.Fade_Accent_Color)
               .Replace("#ITEM_MARGIN#", USettings.Item_Margin)
               .Replace("#FONT#", USettings.Font_Name)
               .Replace("#WINDOWSOPENMODE#", USettings.WindowsOpenMode)
               .Replace("#WINDWOWSRESIZEMODE#", USettings.WindowsResizeMode)
               .Replace("#SHOWMINIMIZE#", USettings.ShowMinimize.ToString())
               .Replace("#SHOWMAXIMIZE#", USettings.ShowMaximize.ToString())
               .Replace("#SHOWCLOSE#", USettings.ShowClose.ToString())
               .Replace("#WIDTH#", USettings.Width.ToString())
               .Replace("#HEIGHT#", USettings.Height.ToString())
               .Replace("#APPLICATION_NAME#", USettings.Application_Name.ToString())
               .Replace("#APPLICATION_ICON#", USettings.Application_Icon.ToString())
               .Replace("#APPLICATION_ICONPATH#", USettings.Application_IconPath.ToString())
               .Replace("#PRODUCTION#", USettings.Production.ToString())
               ;
            }

            if (Mode == ParseMode.System || Mode == ParseMode.Both || Mode == ParseMode.All)
            {
                ParsedText = Text
                .Replace("{USER}", Environment.UserName)
                .Replace("{DEVICE}", Environment.MachineName)
                .Replace("{n}", "\n")
                .Replace("{e}", "=")
                .Replace("{q}", "'")
                .Replace("{AppPath}", UCommon.Application_Path)
                .Replace("{AppPathWindows}", UCommon.Application_Path_Windows)
                ;
            }

            if (Mode == ParseMode.User || Mode == ParseMode.Both || Mode == ParseMode.All)
            {
                foreach (string Key in UCommon.Variable.Keys)
                {
                    string Value = UCommon.GetVariable(Key);
                    ParsedText = ParsedText.Replace($"[{Key}]", Value);
                }
            }

            return ParsedText;
        }
        internal static string JsInputHandler(string ID)
        {
            string scriptContent = $"const Input_{ID} = document.getElementById(\"{ID}\");\n" +
                       $"Input_{ID}.addEventListener(\"change\" , function() {{\n" +
                       $"const myForm = document.getElementById(\"UForm\");\r\n" +
                       $"var submitEvent = new Event('submit');myForm.dispatchEvent(submitEvent);\r\n" +
                       "});";
            return $"<script>{scriptContent}</script>";
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
            string Html = null;
            XmlDocument TempXmlDocument = new XmlDocument();
            try { TempXmlDocument.LoadXml(XML); } catch { };
            XmlNode childNode = TempXmlDocument.DocumentElement;

            if (childNode != null)
            {
                //get all atribute that can exist in all type
                string Id = childNode.Attributes["Id"]?.Value ?? $"RandomId{new Random().Next()}";

                if (UCommon.GetVariable(Id) == null)
                {  UCommon.SetVariable(Id, childNode.InnerText); }

                string InnerValue = ParseText(childNode.InnerText, ParseMode.Both);

                string Type = childNode.Attributes["Type"]?.Value ?? "Default";
                string Action = childNode.Attributes["Action"]?.Value;
                string Argument = childNode.Attributes["Argument"]?.Value ?? "";

                string ExtraStyle = childNode.Attributes["Style"]?.Value ?? "";
                string Class = childNode.Attributes["Class"]?.Value ?? "";

                string ImageObject = childNode.Attributes["Image"]?.Value;
                string ImagePath = ""; string ImageSize = ""; string ImageAutoColor = "";

                if (UCommon.GetVariable(Id) != null && UCommon.GetVariable(Id).Length > 0)
                {
                    InnerValue = ParseText(UCommon.GetVariable(Id), ParseMode.All);
                }

                //Handle image argument
                if (ImageObject != null)
                {
                    try
                    {
                        ImagePath = ImageObject.Split(',')[0];
                        ImageSize = ImageObject.Split(',')[1] ?? "3";
                        ImageAutoColor = ImageObject.Split(',')[2] ?? "true";
                    }
                    catch { UCommon.Warning($"ImageObject property should be write like ImageObject=\"Path (string),Size (integer),AutoColor (boolean)\""); }

                    ExtraStyle += $"background-image: url('{UImage.GetCopyOfImage(ImagePath, bool.Parse(ImageAutoColor))}');background-size: {ImageSize}%;background-repeat: no-repeat;background-position: center;";
                }

                if (!USettings.ElementsParsingSkip.Contains(Id))
                {
                    // Check what kind of node is it 
                    switch (childNode.Name.ToLower() ?? childNode.Name)
                    {
                        case "viewinput":
                            //action that must be apply for all type of ViewInput
                            switch (Type.ToLower() ?? Type)
                            {
                                default:
                                case "textbox":
                                    Html += $"<input style=\"{ExtraStyle}\" class=\"TextBox {Class}\" type=\"text\" name=\"INPUT_{Id}\" Id=\"{Id}\" value=\"{InnerValue}\"/>";
                                    break;
                                case "linesbox":
                                    Html += $"<textarea style=\"{ExtraStyle}\" class=\"TextBox {Class}\" name=\"INPUT_{Id}\" Id=\"{Id}\">{InnerValue}</textarea>";
                                    break;
                                case "checkbox":
                                    string Checked = childNode.Attributes["Checked"]?.Value ?? "False";
                                    if (UCommon.GetVariable($"CheckBox_{Id}") != null)
                                    {
                                        Checked = UCommon.GetVariable($"CheckBox_{Id}");
                                    }
                                    string Checked_Text = "";
                                    if (Checked != null)
                                    {
                                        if (bool.Parse(Checked))
                                        { Checked_Text = "checked"; }
                                        UCommon.SetVariable($"CheckBox_{Id}", Checked.ToString());
                                    }
                                    else
                                    { UCommon.SetVariable($"CheckBox_{Id}", "false"); }

                                    Html += "\n" +
                                        $"<label style=\"{ExtraStyle}\" class=\"label-checkbox Item-Margin{Class}\">\r\n" +
                                        $"  <input type=\"checkbox\" id=\"{Id}\" name=\"INPUT_{Id}\" value=\"{InnerValue}\" {Checked_Text} onclick=\"saveCheckboxStatus(this)\"/>\r\n" +
                                        "  <span class=\"checkmark\"></span>\r\n" +
                                        $"{InnerValue}\r\n" +
                                        "</label>\r\n" +
                                        $"<input id=\"CheckBox_{Id}\" name=\"CheckBox_{Id}\" Value=\"{Checked}\" hidden/>";
                                    break;
                                case "dropdown":
                                    string DropList = childNode.Attributes["DropList"]?.Value;
                                    string url = "https://static.thenounproject.com/png/1590826-200.png";
                                    string dropstyle = $"background-image: url('{UImage.GetCopyOfImage(url, true)}');";


                                    Html += $"<div style=\"{ExtraStyle}\" class=\"dropdown {Class}\">\n";
                                    Html += $"<input style=\"{dropstyle}\" readonly type=\"text\" name=\"INPUT_{Id}\" Id=\"{Id}\" value=\"{InnerValue}\"/>\n";
                                    Html += $"<div class=\"dropdown-content\">\n";
                                    if (DropList == null)
                                    {
                                        Html += $"<a href=\"#\">{InnerValue}</a>\n";
                                    }
                                    else
                                    {
                                        foreach (string line in DropList.Split(','))
                                        {
                                            Html += $"<a href=\"#\">{line}</a>\n";
                                        }
                                    }
                                    Html += $"</div>\n";
                                    Html += $"</div>\n";
                                    break;
                            }
                            if (Action != null) { Html += GenerateHtmlFromXML($"<ViewAction Type=\"InputHandler\" Action=\"{Action}\" Argument=\"{Argument}\">{Id}</ViewAction>"); }
                            break;
                        case "viewitem":
                            switch (Type.ToLower() ?? Type)
                            {
                                case "spacer":
                                    Html += "<div class=\"Spacer\">.</div>";
                                    break;
                                case "title":
                                    Html += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Title {Class}\">{InnerValue}</div>\n";
                                    break;
                                case "subtitle":
                                    Html += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"SubTitle {Class}\">{InnerValue}</div>\n";
                                    break;
                                default:
                                case "label":
                                    Html += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Label {Class}\">{InnerValue}</div>\n";
                                    break;
                                case "labelbox":
                                    Html += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"LabelBox {Class}\">{InnerValue}</div>\n";
                                    break;
                                case "box":
                                    Html += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Box {Class}\">\n";
                                    

                                    foreach (XmlNode rowChildNode in childNode.ChildNodes)
                                    {
                                        Html += GenerateHtmlFromXML(rowChildNode.OuterXml);
                                    }

                                    Html += "</div>\n";
                                    break;
                                case "row":
                                    Html += $"<div style=\"{ExtraStyle}\" Id=\"{Id}\" class=\"Row {Class}\">\n";
                                    foreach (XmlNode rowChildNode in childNode.ChildNodes)
                                    {
                                        Html += GenerateHtmlFromXML(rowChildNode.OuterXml);
                                    }
                                    Html += "</div>\n";
                                    break;
                            }
                            break;
                        case "viewaction":
                            if (Action != null || Type.ToLower().Contains("button") || Type.ToLower().Contains("linker"))
                            {
                                switch (Type.ToLower() ?? Type)
                                {
                                    case "linker":
                                        string source = childNode.Attributes["Source"]?.Value ?? "default";
                                        string target = childNode.Attributes["Target"]?.Value ?? "default";

                                        if (source != null && target != null)
                                        {
                                            Html += $"<input hidden=\"hidden\" Id=\"{Id}\" name=\"Action_{Id}\" value=\"{NewInputValue("Linker", "", Id, $"{source},{target}")}\"/>\n";
                                        }
                                        else
                                        {
                                            UCommon.Warning($"Linker must include property Source and Target that is not empty)", "Linker Error");
                                        }
                                        break;
                                    default:
                                    case "button":
                                        if (Action != null)
                                        {
                                            Html += $"<button style=\"{ExtraStyle}\" class=\"Button {Class}\" type=\"submit\" Id=\"{Id}\" name=\"Action_{Id}\" value=\"{NewInputValue("ui", Action, Id, Argument)}\">{InnerValue}</button>\n";
                                        }
                                        else
                                        {
                                            Html += $"<button style=\"{ExtraStyle}\" class=\"Button {Class}\" type=\"submit\" Id=\"{Id}\" name=\"Action_{Id}\">{InnerValue}</button>\n";
                                        }
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
                                            Html += JsInputHandler(InnerValue);
                                        }
                                        catch (Exception ex) { UCommon.Error(ex.Message); }
                                        break;
                                    case "viewload":
                                        Html += $"<input hidden=\"hidden\" name=\"Action_{Id}\" value=\"{NewInputValue("ViewLoad", Action, Id, Argument)}\"/>\n";
                                        break;
                                    case "variablehandler":
                                        try
                                        {
                                            if (!UCommon.TrackedVariable.TryGetValue(InnerValue, out ActionStorage acs))
                                            {
                                                UCommon.TrackedVariable.Add(InnerValue, new ActionStorage(Action, Argument, UCommon.GetVariable(InnerValue), InnerValue));
                                            }
                                        }
                                        catch (Exception ex) { UCommon.Error(ex.Message); }
                                        break;
                                }
                            }
                            break;
                        // if html that will just write it down simply
                        default:
                                Html += ParseText(childNode.OuterXml, ParseMode.All);


                            break;
                    }
                }
            }

            return Html;
        }

    }
}