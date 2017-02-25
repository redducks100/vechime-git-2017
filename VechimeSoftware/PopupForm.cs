using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VechimeSoftware
{
    public partial class PopupForm : Form
    {
        public PopupForm(string text)
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.Manual;
            this.BringToFront();

            Rectangle res = Screen.PrimaryScreen.Bounds;

            this.Location = new Point(Screen.PrimaryScreen.Bounds.Right - this.Width, Screen.PrimaryScreen.Bounds.Bottom - (this.Height + 40));

            //this.Location = new Point(500, 500);

            labelText.Text = text;
        }

        private void buttonVerify_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void buttonIgnore_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
        }
    }
}
