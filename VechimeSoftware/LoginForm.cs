using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VechimeSoftware
{
    public partial class LoginForm : Form
    {
        private string server;
        private string serverPort;
        private string databaseName;
        private string databaseUser;
        private string databasePassword;

        private string connectionString = "";

        public MongoClient mongoDB;

      
        public LoginForm(bool startup)
        {
            InitializeComponent();

            server = "ds135089.mlab.com";
            serverPort = "35089";
            databaseName = "license_manager";
            databaseUser = "license_manager_admin";
            databasePassword = "cbyuvtigymohmfuhoe124ss";

            connectionString = $"mongodb://{databaseUser}:{databasePassword}@{server}:{serverPort}/{databaseName}";
            
            mongoDB = new MongoClient(connectionString);

            startUp = startup;

            CheckForUserData();
        }

        private bool startUp;
        public void SaveSettings(string username, string passhash)
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

            
            if (rememberCheckBox.Checked || settings.SaveUserData)
            {
                settings.Username = username;
                settings.Passhash = passhash;
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
        }

        public void CheckForUserData()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VechimeManager");
            if (Directory.Exists(path))
            {
                path = Path.Combine(path, "settings.json");
                if (File.Exists(path))
                {
                    string json = "";
                    using (StreamReader reader = new StreamReader(path))
                    {
                         json = reader.ReadToEnd();
                    }
                    Settings settings = JsonConvert.DeserializeObject<Settings>(json);
                    if (settings.Username != "" && settings.Passhash != "" && settings.SaveUserData)
                    {


                        Connect(settings.Username, settings.Passhash);
                    }
                    else if (startUp)
                        Close();

                    
                }
            }
        }

        public Permission CheckUser(string username, string password)
        {
            var db = mongoDB.GetDatabase(databaseName);
            var col = db.GetCollection<UserData>("UsersData");
            var result =  col.Find(x=>x.Username==username).Limit(1).ToListAsync().Result.FirstOrDefault();

            if (result == null)
            {
                MessageBox.Show("Nu exista cont pentru acest username!","Atentie!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return Permission.NONE;
            }


                if (Utils.SHA1(Utils.SHA1(password + result.HWID)) != result.Passhash)
                {
                    MessageBox.Show("Parola sau username-ul este gresita!", "Atentie!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Permission.NONE;
                }
         
            if (string.IsNullOrWhiteSpace(result.Serial))
            {
                return Permission.DEMO;
            }

            return Permission.FULL;
        }

        public bool CheckSerial(string serial)
        {
            var db = mongoDB.GetDatabase(databaseName);
            var col = db.GetCollection<SerialData>("SerialsData");
            var result = col.Find(x => x.Serial == serial).Limit(1).ToListAsync().Result.FirstOrDefault();

            if(result == null)
            {
                return false;
            }

            col.DeleteOne(x => x.Serial==serial);
            return true;
        }

        public bool AddUser(UserData data)
        {
            var db = mongoDB.GetDatabase(databaseName);
            var col = db.GetCollection<UserData>("UsersData");
            
            if(col.Find(x=>x.Username == data.Username).ToListAsync().Result.Count > 0)
            {
                MessageBox.Show("Username-ul este deja folosit!" , "Atentie!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return false;
            }

            if (col.Find(x => x.Email == data.Email).ToListAsync().Result.Count > 0)
            {
                MessageBox.Show("Email-ul este deja folosit!", "Atentie!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            col.InsertOneAsync(data);
            return true;
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(usernameTextBox.Text) || string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                return;
            }

            Connect(usernameTextBox.Text,passwordTextBox.Text);
        }

        private void Connect(string username,string pass)
        {
            Permission permission = CheckUser(username, pass);

            if (permission == Permission.NONE)
            {
                return;
            }

            SaveSettings(username, pass);

            if (permission == Permission.FULL)
            {
                using (MainForm main = new MainForm(false,startUp))
                {
                    main.Text += " - Full Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    this.Hide();
                    main.ShowDialog();
                }
            }
            else
            {
                using (MainForm main = new MainForm(true,startUp))
                {
                    main.Text += " - Demo Version";
                    this.Hide();
                    main.ShowDialog();
                }
            }
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            using (RegisterForm registerForm = new RegisterForm(this))
            {
                registerForm.ShowDialog();
            }
        }
    }

    public enum Permission
    {
        NONE,
        DEMO,
        FULL
    }

    public class Settings
    {
        public bool SaveUserData;
        public string Username;
        public string Passhash;
        public bool RunOnStartup;
    }
}
