using System.Windows.Forms;
using UPrompt.Class;

namespace DEMO
{
    public static class DemoPrompt
    {
        public static void ShowDemo(string value)
        {
            MessageBox.Show("Extension method called with value: " + value, USettings.Width.ToString());
        }
    }
}
