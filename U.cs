﻿using System;
using System.IO;
using System.Windows.Forms;
using UPrompt.Core;

namespace UPrompt
{
    public static class U
    {
        [STAThread]
        static void Main(string[]Args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Args.Length > 0)
            {
                bool Path = false;
                if (Args.Length == 1) { Path = true; }
                foreach (string arg in Args)
                {
                    switch (arg)
                    {
                        case "/Path":
                        case "/path":
                        case "/p":
                        case "/P":
                            Path = true;
                            break;
                        default:
                            if (arg.ToLower().Contains(".xml") && Path)
                            {
                                if (File.Exists(arg))
                                {
                                    UCommon.Xml_Path = arg;
                                }
                            }
                            Path = false;
                            break;
                    }
                }
            }
            Application.Run(new Prompt());
        }
    }
}
