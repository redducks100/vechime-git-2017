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
    public partial class RegisterForm : Form
    {
        private LoginForm parent;

        public RegisterForm(LoginForm _parent)
        {
            InitializeComponent();
            parent = _parent;
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(usernameTextBox.Text) || string.IsNullOrWhiteSpace(passwordTextBox.Text) || string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                MessageBox.Show("Toate casutele trebuie completate!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidEmail(emailTextBox.Text))
            {
                MessageBox.Show("Va rog sa introduceti un email valid!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserData newData = new UserData();
            newData.Username = usernameTextBox.Text;
            newData.Email = emailTextBox.Text;
            newData.HWID = Utils.GetMachineGuid();
            newData.Passhash = Utils.SHA1(Utils.SHA1(passwordTextBox.Text + newData.HWID));

            if (string.IsNullOrWhiteSpace(serialTextBox.Text))
            {
                newData.Serial = "";
            }
            else if (parent.CheckSerial(serialTextBox.Text) == false)
            {
                MessageBox.Show("Serial-ul nu este valid!", "Atentie!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            newData.Serial = serialTextBox.Text;
            if (!parent.AddUser(newData))
            {
                return;
            }
            else
            {
                MessageBox.Show("Utilizator inregistrat cu succes!", "Atentie");
            }
            this.Close();
        }
    }
}
