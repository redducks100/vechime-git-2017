
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VechimeSoftware
{
    public partial class OptiuniForm : Form
    {
        public OptiuniForm()
        {
            InitializeComponent();
        }

        private void OptiuniForm_Load(object sender, EventArgs e)
        {

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VechimeManager");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, "settings.json");

            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string json = reader.ReadToEnd();
                    Settings settings = JsonConvert.DeserializeObject<Settings>(json);

                    if (settings.RunOnStartup)
                        checkBoxStartUp.Checked = true;

                    if (settings.SaveUserData)
                        checkBoxAutoConnect.Checked = true;
                }
            }

        }

        #region Handler
        private void checkBoxAutoConnect_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoConnect.Checked)
            {
                checkBoxStartUp.Enabled = true;
            }
            else
            {
                checkBoxStartUp.Checked = false;
                checkBoxStartUp.Enabled = false;
            }

        }
        #endregion
        private void buttonSave_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VechimeManager");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, "settings.json");

            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string json = reader.ReadToEnd();
                    Settings oldSettings = JsonConvert.DeserializeObject<Settings>(json);
                    settings = oldSettings;
                }
                File.Delete(path);
            }
            File.Create(path).Close();


            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (checkBoxStartUp.Checked)
            {
                settings.RunOnStartup = true;

                if (rkApp.GetValue("Vechime /runminimized") == null)
                {
                    rkApp.SetValue("Vechime /runminimized", Application.ExecutablePath);
                }
            }
            else
            {
                settings.RunOnStartup = false;

                if (rkApp.GetValue("Vechime /runminimized") != null)
                {
                    rkApp.DeleteValue("Vechime /runminimized");
                }
            }
            
         

          
            if (checkBoxAutoConnect.Checked)
            {
                settings.SaveUserData = true;
            }
            else
            {
                settings.Username = "";
                settings.Passhash = "";
                settings.SaveUserData = false;
            }

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(JsonConvert.SerializeObject(settings));
            }

            Close();
        }

       
    }
}
