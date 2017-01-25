using Itenso.TimePeriod;

using System;
using System.Windows.Forms;

namespace VechimeSoftware
{
    public partial class PerioadaForm : Form
    {
        private MainForm parent;
        private Perioada currentPerioada;
        private int currentPersonIndex;

        public PerioadaForm(MainForm _parent, Perioada _currentPerioada, int _currentPersonIndex)
        {
            InitializeComponent();
            parent = _parent;
            currentPerioada = _currentPerioada;
            currentPersonIndex = _currentPersonIndex;
            UpdateTitle();
            UpdateForm();
        }

        #region UI

        private void UpdateTitle()
        {
            if (currentPerioada != null)
            {
                this.Text = "Modifica";
                titleLabel.Text = "Detalii - [" + currentPerioada.DTInceput.ToShortDateString() + " - " + currentPerioada.DTSfarsit.ToShortDateString() + "]";
            }
            else
            {
                this.Text = "Adauga";
                titleLabel.Text = "Detalii perioada";
            }
        }

        private void UpdateForm()
        {
            if (currentPerioada != null)
            {
                inceputTimePicker.Value = currentPerioada.DTInceput;
                sfarsitTimePicker.Value = currentPerioada.DTSfarsit;

                cfsani_personalTB.Text = currentPerioada.CFSAni_Personal.ToString();
                cfsluni_personalTB.Text = currentPerioada.CFSLuni_Personal.ToString();
                cfszile_personalTB.Text = currentPerioada.CFSZile_Personal.ToString();

                cfsani_studiiTB.Text = currentPerioada.CFSAni_Studii.ToString();
                cfsluni_studiiTB.Text = currentPerioada.CFSLuni_Studii.ToString();
                cfszile_studiiTB.Text = currentPerioada.CFSZile_Studii.ToString();

                if (currentPerioada.Norma == "1/1")
                {
                    normaComboBox.SelectedIndex = 0;
                }
                else if (currentPerioada.Norma == "1/2")
                {
                    normaComboBox.SelectedIndex = 1;
                }
                else
                {
                    normaComboBox.SelectedIndex = 2;
                }

                if (currentPerioada.IOM.ToLower() == "invatamant")
                {
                    iomComboBox.SelectedIndex = 0;
                }
                else
                {
                    iomComboBox.SelectedIndex = 1;
                }
                functieTextBox.Text = currentPerioada.Functie.ToString().ToUpper();
                locMuncaTextBox.Text = currentPerioada.LocMunca.ToString().ToUpper();
                perioadaTextBox.Text = currentPerioada.CFSAni_Studii.ToString() + " ani " + currentPerioada.CFSLuni_Studii.ToString() + " luni " + currentPerioada.CFSZile_Studii.ToString() + " zile";
                lucreazaCheckBox.Checked = currentPerioada.Lucreaza;

                if(currentPerioada.Somaj)
                {
                    iomComboBox.Items.Add("SOMAJ");
                    iomComboBox.SelectedIndex = 2;
                }


                editButton.Enabled = true;
            }
            else
            {
                SetEnabled(this, true);

                editButton.Enabled = false;
                saveButton.Enabled = false;
                perioadaTextBox.Enabled = false;
            }
        }

        #endregion UI

        #region Handlers

        private void editButton_Click(object sender, EventArgs e)
        {
            if (currentPerioada == null)
                return;

            Button button = (Button)sender;
            if (MessageBox.Show("Sunteti sigur(a) ca vreti sa modificati aceasta informatie?", "Atentie!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SetEnabled(this, true);

                if (currentPerioada.Somaj == true)
                    iomComboBox.Enabled = false;

                perioadaTextBox.Enabled = false;
                editButton.Enabled = false;
                addButton.Enabled = false;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(locMuncaTextBox.Text) || string.IsNullOrWhiteSpace(perioadaTextBox.Text))
            {
                MessageBox.Show("Toate casutele trebuie completate!", "Atentie!");
                return;
            }
            currentPerioada = new Perioada();
            currentPerioada.DTInceput = inceputTimePicker.Value;
            currentPerioada.DTSfarsit = sfarsitTimePicker.Value;
            currentPerioada.CFSAni_Personal =  Convert.ToInt32(cfsani_personalTB.Text);
            currentPerioada.CFSLuni_Personal = Convert.ToInt32(cfsluni_personalTB.Text);
            currentPerioada.CFSZile_Personal = Convert.ToInt32(cfszile_personalTB.Text);
            currentPerioada.CFSAni_Studii =  Convert.ToInt32(cfsani_studiiTB.Text);
            currentPerioada.CFSLuni_Studii = Convert.ToInt32(cfsluni_studiiTB.Text);
            currentPerioada.CFSZile_Studii = Convert.ToInt32(cfszile_studiiTB.Text);
            currentPerioada.Norma = normaComboBox.SelectedItem.ToString();
            if (!somajCheckBox.Checked)
                currentPerioada.Functie = functieTextBox.Text.ToUpper();
            else
                currentPerioada.Functie = "SOMER";
            if (!somajCheckBox.Checked)
                currentPerioada.LocMunca = locMuncaTextBox.Text.ToUpper();
            else
                currentPerioada.LocMunca = "SOMER";

            currentPerioada.IOM = iomComboBox.SelectedItem.ToString();
            currentPerioada.Lucreaza = lucreazaCheckBox.Checked;
            currentPerioada.Somaj = somajCheckBox.Checked;

            parent.AddPerioada(currentPerioada, currentPersonIndex);

            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(locMuncaTextBox.Text) || string.IsNullOrWhiteSpace(perioadaTextBox.Text))
            {
                MessageBox.Show("Toate casutele trebuie completate!", "Atentie!");
                return;
            }

            currentPerioada.DTInceput = inceputTimePicker.Value;
            currentPerioada.DTSfarsit = sfarsitTimePicker.Value;
            currentPerioada.CFSAni_Personal = Convert.ToInt32(cfsani_personalTB.Text);
            currentPerioada.CFSLuni_Personal = Convert.ToInt32(cfsluni_personalTB.Text);
            currentPerioada.CFSZile_Personal = Convert.ToInt32(cfszile_personalTB.Text);
            currentPerioada.CFSAni_Studii = Convert.ToInt32(cfsani_studiiTB.Text);
            currentPerioada.CFSLuni_Studii = Convert.ToInt32(cfsluni_studiiTB.Text);
            currentPerioada.CFSZile_Studii = Convert.ToInt32(cfszile_studiiTB.Text);
            currentPerioada.Norma = normaComboBox.SelectedItem.ToString();
            if (!somajCheckBox.Checked)
                currentPerioada.Functie = functieTextBox.Text.ToUpper();
            else
                currentPerioada.Functie = "SOMER";
            if (!somajCheckBox.Checked)
                currentPerioada.LocMunca = locMuncaTextBox.Text.ToUpper();
            else
                currentPerioada.LocMunca = "SOMER";

            currentPerioada.IOM = iomComboBox.SelectedItem.ToString();
            currentPerioada.Lucreaza = lucreazaCheckBox.Checked;
            currentPerioada.Somaj = somajCheckBox.Checked;

            parent.ModifyPerioada(currentPerioada, currentPersonIndex);

            this.Close();
        }

        private void timePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime firstDate = inceputTimePicker.Value;
            DateTime secondDate = sfarsitTimePicker.Value;
            DateDiff span = new DateDiff(firstDate, secondDate);
            perioadaTextBox.Text = span.ElapsedYears + " ani " + span.ElapsedMonths + " luni " + span.ElapsedDays + " zile";
        }

        private void cfsTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void somajCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (somajCheckBox.Checked)
            {
                iomComboBox.Items.Add("SOMER");
                iomComboBox.SelectedIndex = 2;
                iomComboBox.Enabled = false;

                functieTextBox.Text = "SOMER";
                functieTextBox.Enabled = false;

                locMuncaTextBox.Text = "SOMER";
                locMuncaTextBox.Enabled = false;
                lucreazaLabel.Text = "Inca Somer:";
            }
            else
            {
                iomComboBox.Items.Remove("SOMER");
                iomComboBox.SelectedIndex = 0;
                iomComboBox.Enabled = true;

                functieTextBox.Text = currentPerioada != null?currentPerioada.Functie.ToUpper():"";
                functieTextBox.Enabled = true;

                locMuncaTextBox.Text = currentPerioada != null?currentPerioada.LocMunca.ToUpper():"";
                locMuncaTextBox.Enabled = true;

                lucreazaLabel.Text = "Inca Lucreaza:";
            }
        }

        #endregion Handlers

        #region Utils

        private void SetEnabled(Control control, bool enabled)
        {
            control.Enabled = enabled;
            foreach (Control child in control.Controls)
            {
                SetEnabled(child, enabled);
            }
        }
        #endregion
    }
}
