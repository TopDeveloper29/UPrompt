namespace UPrompt
{
    partial class Prompt
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.htmlhandler = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // htmlhandler
            // 
            this.htmlhandler.AllowWebBrowserDrop = false;
            this.htmlhandler.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlhandler.IsWebBrowserContextMenuEnabled = false;
            this.htmlhandler.Location = new System.Drawing.Point(3, 24);
            this.htmlhandler.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlhandler.Name = "htmlhandler";
            this.htmlhandler.ScriptErrorsSuppressed = true;
            this.htmlhandler.ScrollBarsEnabled = false;
            this.htmlhandler.Size = new System.Drawing.Size(794, 423);
            this.htmlhandler.TabIndex = 0;
            this.htmlhandler.Url = new System.Uri("", System.UriKind.Relative);
            this.htmlhandler.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.htmlhandler_Navigating);
            // 
            // Prompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.htmlhandler);
            this.FormStyle = MaterialSkin.Controls.MaterialForm.FormStyles.ActionBar_None;
            this.Name = "Prompt";
            this.Padding = new System.Windows.Forms.Padding(3, 24, 3, 3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UPrompt DEMO";
            this.Load += new System.EventHandler(this.Prompt_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser htmlhandler;
    }
}

