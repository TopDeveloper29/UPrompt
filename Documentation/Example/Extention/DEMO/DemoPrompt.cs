using System.Windows.Forms;
using UPrompt.Core;

namespace DEMO
{
    public static class DemoPrompt
    {
        public static void ShowDemo(string value)
        {
            MessageBox.Show("Extension method called with title: " + value, USettings.Application_Name.ToString());
        }
    }
}
