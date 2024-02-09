using System;
using System.Windows.Forms;

namespace UPrompt
{
    public static class U
    {
        [STAThread]
        static void Main(string[] Args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Prompt(Args));
        }
    }
}