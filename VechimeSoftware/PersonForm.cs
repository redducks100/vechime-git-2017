using System;
using System.Linq;
using System.Windows.Forms;

namespace VechimeSoftware
{
    public partial class PersonForm : Form
    {
        private MainForm parent;
        private Person currentPerson;

        public PersonForm(MainForm _parent, Person _currentPerson)
        {
            InitializeComponent();
            parent = _parent;
            currentPerson = _currentPerson;
            UpdateTitle();
            UpdateForm();
        }

        private bool CheckCNP(string CNP)
        {
            if (parent.peopleDictionary.Values.ToList().Where(x => x.CNP == CNP).Count() > 0)
            {
                MessageBox.Show("Deja exista o persoana cu acest CNP: " + CNP + "!", "Eroare!");
                return false;
            }
            return true;
        }

        private bool ValidateCNP()
        {
            string CNP = cnpTextBox.Text;
            string validationString = "279146358279";
            if (CNP.Length == 13)
            {
                int lastNumber = CNP[12] - '0';
                int sum = 0;
                for (int i = 0; i < CNP.Length - 1; i++)
                {
                    sum += (CNP[i] - '0') * (validationString[i] - '0');
                }

                sum %= 11;

                if (sum == 10)
                    sum = 1;

                if (lastNumber == sum)
                    return true;
                return false;
            }
            return false;
        }

        #region UI

        private void UpdateTitle()
        {
            if (currentPerson != null)
            {
                this.Text = "Modifica";
                titleLabel.Text = "Detalii - " + currentPerson.Nume + " " + currentPerson.Prenume;
            }
            else
            {
                this.Text = "Adauga";
                titleLabel.Text = "Detalii persoana";
            }
        }

        private void UpdateForm()
        {
            if (currentPerson != null)
            {
                cnpTextBox.Text = currentPerson.CNP;
                serieTextBox.Text = currentPerson.Serie;
                nameTextBox.Text = currentPerson.Nume.ToUpper();
                prenumeTextBox.Text = currentPerson.Prenume.ToUpper();

                editButton.Enabled = true;
            }
            else
            {
                cnpTextBox.Enabled = true;
                nameTextBox.Enabled = true;
                serieTextBox.Enabled = true;
                prenumeTextBox.Enabled = true;
                addButton.Enabled = true;
            }
        }

        #endregion UI

        #region Handlers

        private void cnpTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if(!ValidateCNP())
            {
                MessageBox.Show("CNP-ul nu este valid!","Erroare!");
                return;
            }
            if (CheckCNP(cnpTextBox.Text) == false)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(nameTextBox.Text) || string.IsNullOrWhiteSpace(prenumeTextBox.Text))
            {
                MessageBox.Show("Toate casutele trebuie completate!");
                return;
            }
            currentPerson = new Person();
            currentPerson.CNP = cnpTextBox.Text;
            currentPerson.Nume = nameTextBox.Text.ToUpper();
            currentPerson.Prenume = prenumeTextBox.Text.ToUpper();
            currentPerson.Serie = serieTextBox.Text.ToUpper();

            parent.AddPerson(currentPerson);
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateCNP())
            {
                MessageBox.Show("CNP-ul nu este valid!", "Erroare!");
                return;
            }
            if (CheckCNP(cnpTextBox.Text) == false)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(nameTextBox.Text) || string.IsNullOrWhiteSpace(prenumeTextBox.Text) || string.IsNullOrWhiteSpace(serieTextBox.Text))
            {
                MessageBox.Show("Toate casutele trebuie completate!");
                return;
            }

            currentPerson.CNP = cnpTextBox.Text;
            currentPerson.Nume = nameTextBox.Text.ToUpper();
            currentPerson.Prenume = prenumeTextBox.Text.ToUpper();
            currentPerson.Serie = serieTextBox.Text.ToUpper();

            parent.ModifyPerson(currentPerson);
            this.Close();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (currentPerson == null)
                return;

            if (MessageBox.Show("Sunteti sigur(a) ca vreti sa modificati aceasta informatie?", "Atentie!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                cnpTextBox.Enabled = true;
                nameTextBox.Enabled = true;
                serieTextBox.Enabled = true;
                prenumeTextBox.Enabled = true;

                editButton.Enabled = false;
                saveButton.Enabled = true;
            }
        }

        #endregion Handlers
    }
}
