using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VechimeSoftware
{
    public partial class UnitateForm : Form
    {
        private MainForm parent;
        private UnitateInfo currentUnitate;

        public UnitateForm(MainForm _parent, UnitateInfo _currentUnitate)
        {
            InitializeComponent();
            currentUnitate = _currentUnitate;
            parent = _parent;
            UpdateForm();
        }

        private void UpdateForm()
        {
            if(currentUnitate != null)
            {
                scTextBox.Text =             currentUnitate.SC;
                stradaTextBox.Text =         currentUnitate.Strada;
                numarTextBox.Text =          currentUnitate.Numar;
                localitateTextBox.Text =     currentUnitate.Localitate;
                judetComboBox.SelectedItem = currentUnitate.Judet;
                telefonTextBox.Text =        currentUnitate.Telefon;
                faxTextBox.Text =            currentUnitate.Fax;
                cuiTextBox.Text =            currentUnitate.CUI;
                editButton.Enabled = true;
            }
            else
            {
                SetEnabled(this, true);
                editButton.Enabled = false;
                saveButton.Enabled = true;
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (currentUnitate == null)
                return;

            if (MessageBox.Show("Sunteti sigur(a) ca vreti sa modificati aceasta informatie?", "Atentie!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SetEnabled(this, true);

                editButton.Enabled = false;
                saveButton.Enabled = true;
            }
        }

        #region Utils

        private void SetEnabled(Control control, bool enabled)
        {
            control.Enabled = enabled;
            foreach (Control child in control.Controls)
            {
                SetEnabled(child, enabled);
            }
        }

        #endregion Utils

        private void saveButton_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(scTextBox.Text)||
               string.IsNullOrWhiteSpace(stradaTextBox.Text)||
               string.IsNullOrWhiteSpace(numarTextBox.Text)||
               string.IsNullOrWhiteSpace(localitateTextBox.Text)||
               string.IsNullOrWhiteSpace(telefonTextBox.Text)||
               string.IsNullOrWhiteSpace(faxTextBox.Text )||
               string.IsNullOrWhiteSpace(cuiTextBox.Text ))
            {
                MessageBox.Show("Toate casutele trebuie completate!","Atentie!");
                return;
            }

            UnitateInfo newUnitateInfo = new UnitateInfo();

            newUnitateInfo.SC = scTextBox.Text;
            newUnitateInfo.Strada = stradaTextBox.Text;
            newUnitateInfo.Numar = numarTextBox.Text;
            newUnitateInfo.Localitate = localitateTextBox.Text;
            newUnitateInfo.Judet = judetComboBox.SelectedItem.ToString();
            newUnitateInfo.Telefon = telefonTextBox.Text;
            newUnitateInfo.Fax = faxTextBox.Text;
            newUnitateInfo.CUI = cuiTextBox.Text;

            if (currentUnitate != null)
            {
                newUnitateInfo.ID = currentUnitate.ID;
                newUnitateInfo.NumarInregistrare = currentUnitate.NumarInregistrare;
                parent.ModifyCurrentUnitate(newUnitateInfo);
            }
            else
            {
                newUnitateInfo.NumarInregistrare = 0;
                parent.AddCurrentUnitate(newUnitateInfo);
            }

            this.Close();
        }

        private void UnitateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(parent.GetCurrentUnitateInfo() == null)
                e.Cancel = true;
        }
    }
}
