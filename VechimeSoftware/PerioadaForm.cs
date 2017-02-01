using Itenso.TimePeriod;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace VechimeSoftware
{
    public partial class PerioadaForm : Form
    {
        private MainForm parent;
        private Perioada currentPerioada;
        private int currentPersonIndex;

        private List<Perioada> currentChangedPeriods;

        public PerioadaForm(MainForm _parent, Perioada _currentPerioada, int _currentPersonIndex)
        {
            InitializeComponent();
            parent = _parent;
            currentPerioada = _currentPerioada;
            currentPersonIndex = _currentPersonIndex;

            currentChangedPeriods = new List<Perioada>();

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
                perioadaTextBox.Text = currentPerioada.Difference.ElapsedYears.ToString() + " ani " + currentPerioada.Difference.ElapsedMonths.ToString() + " luni " + currentPerioada.Difference.ElapsedDays.ToString() + " zile";
                lucreazaCheckBox.Checked = currentPerioada.Lucreaza;

                concediuCheckBox.Checked = (currentPerioada.TipCFS == "" ? false : true);

                if(currentPerioada.TipCFS.ToLower() == "personal")
                {
                    tipConcediuComboBox.SelectedIndex = 0;
                }
                else
                {
                    tipConcediuComboBox.SelectedIndex = 1;
                }

                somerCheckBox.Checked = currentPerioada.Somaj;

                if (currentPerioada.Somaj)
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

            if (!VerificaData(inceputTimePicker.Value, sfarsitTimePicker.Value))
                return;

            currentPerioada = new Perioada();
            currentPerioada.DTInceput = inceputTimePicker.Value;
            currentPerioada.DTSfarsit = sfarsitTimePicker.Value;
            currentPerioada.CFS = concediuCheckBox.Checked;
            currentPerioada.TipCFS = (currentPerioada.CFS ? tipConcediuComboBox.SelectedItem.ToString() : "");
            currentPerioada.Norma = normaComboBox.SelectedItem.ToString();
            if (currentPerioada.CFS)
            {
                currentPerioada.Functie = "CONCEDIU";
                currentPerioada.LocMunca = "CONCEDIU";
                currentPerioada.IOM = "CONCEDIU";
                currentPerioada.Lucreaza = false;
                currentPerioada.LucreazaUnitateaCurenta = false;
            }
            else
            {
                if (somerCheckBox.Checked)
                {
                    currentPerioada.LocMunca = "SOMER";
                    currentPerioada.Functie = "SOMER";
                    currentPerioada.IOM = "SOMER";
                    currentPerioada.Lucreaza = false;
                    currentPerioada.LucreazaUnitateaCurenta = false;
                }
                else
                {
                    currentPerioada.LocMunca = locMuncaTextBox.Text.ToUpper();
                    currentPerioada.IOM = iomComboBox.SelectedItem.ToString();
                    currentPerioada.Lucreaza = lucreazaCheckBox.Checked;
                    currentPerioada.LucreazaUnitateaCurenta = lucreazaUCurentaCheckBox.Checked;
                    currentPerioada.Functie = functieTextBox.Text.ToUpper();
                }
            }
        
            currentPerioada.Somaj = somerCheckBox.Checked;

            parent.AddPerioada(currentPerioada, currentPersonIndex);

            AddToLocalList(currentPerioada);

            //TO-DO RESET FORM
            //this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(locMuncaTextBox.Text) || string.IsNullOrWhiteSpace(perioadaTextBox.Text))
            {
                MessageBox.Show("Toate casutele trebuie completate!", "Atentie!");
                return;
            }

            if (!VerificaData(inceputTimePicker.Value, sfarsitTimePicker.Value))
                return;

            currentPerioada.DTInceput = inceputTimePicker.Value;
            currentPerioada.DTSfarsit = sfarsitTimePicker.Value;
            currentPerioada.CFS = concediuCheckBox.Checked;
            currentPerioada.TipCFS = (currentPerioada.CFS ? tipConcediuComboBox.SelectedItem.ToString() : "");
            currentPerioada.Norma = normaComboBox.SelectedItem.ToString();
            if (currentPerioada.CFS)
            {
                currentPerioada.Functie = "CONCEDIU";
                currentPerioada.LocMunca = "CONCEDIU";
                currentPerioada.IOM = "CONCEDIU";
                currentPerioada.Lucreaza = false;
                currentPerioada.LucreazaUnitateaCurenta = false;
            }
            else
            {
                if (somerCheckBox.Checked)
                {
                    currentPerioada.LocMunca = "SOMER";
                    currentPerioada.Functie = "SOMER";
                    currentPerioada.IOM = "SOMER";
                    currentPerioada.Lucreaza = false;
                    currentPerioada.LucreazaUnitateaCurenta = false;
                }
                else
                {
                    currentPerioada.LocMunca = locMuncaTextBox.Text.ToUpper();
                    currentPerioada.IOM = iomComboBox.SelectedItem.ToString();
                    currentPerioada.Lucreaza = lucreazaCheckBox.Checked;
                    currentPerioada.LucreazaUnitateaCurenta = lucreazaUCurentaCheckBox.Checked;
                    currentPerioada.Functie = functieTextBox.Text.ToUpper();
                }
            }

            currentPerioada.Somaj = somerCheckBox.Checked;

            parent.ModifyPerioada(currentPerioada, currentPersonIndex);

            AddToLocalList(currentPerioada, true);

            //TO-DO RESET FORM
            //this.Close();
        }

        private bool VerificaData(DateTime inceput, DateTime sfarsit)
        {
            Person selectedPerson = null;
            if (parent.peopleDictionary.ContainsKey(currentPersonIndex))
                selectedPerson = parent.peopleDictionary[currentPersonIndex];

            foreach (Perioada perioada in selectedPerson.Perioade)
            {
                if (inceput.CompareTo(perioada.DTInceput) >= 0 && inceput.CompareTo(perioada.DTSfarsit) <= 0)
                {
                    MessageBox.Show("Aceasta perioada este cuprinsa in alta perioada deja inregistrata.");
                    return false;
                }

                if (sfarsit.CompareTo(perioada.DTInceput) >= 0 && sfarsit.CompareTo(perioada.DTSfarsit) <= 0)
                {
                    MessageBox.Show("Aceasta perioada este cuprinsa in alta perioada deja inregistrata.");
                    return false;
                }
            }
            return true;
        }

        private void timePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime firstDate = inceputTimePicker.Value;
            DateTime secondDate = sfarsitTimePicker.Value;
            DateDiff span = new DateDiff(firstDate.Subtract(new TimeSpan(1, 0, 0, 0, 0)), secondDate.Subtract(new TimeSpan(1, 0, 0, 0, 0)));
            perioadaTextBox.Text = span.ElapsedYears + " ani " + span.ElapsedMonths + " luni " + span.ElapsedDays + " zile";
        }

        private void somajCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (somerCheckBox.Checked)
            {
                iomComboBox.Items.Add("SOMER");
                iomComboBox.SelectedIndex = 2;
                iomComboBox.Enabled = false;

                functieTextBox.Text = "SOMER";
                functieTextBox.Enabled = false;

                locMuncaTextBox.Text = "SOMER";
                locMuncaTextBox.Enabled = false;

                lucreazaCheckBox.Enabled = false;
                lucreazaUCurentaCheckBox.Enabled = false;

                normaComboBox.Enabled = false;
            }
            else
            {
                iomComboBox.Items.Remove("SOMER");


                if (currentPerioada != null)
                {
                    if (currentPerioada.IOM.ToLower() == "invatamant")
                    {
                        iomComboBox.SelectedIndex = 0;
                    }
                    else
                    {
                        iomComboBox.SelectedIndex = 1;
                    }
                }
                else
                {
                    iomComboBox.SelectedIndex = 0;
                    iomComboBox.Enabled = true;
                }

                functieTextBox.Text = currentPerioada != null ? currentPerioada.Functie.ToUpper() : "";
                functieTextBox.Enabled = true;

                locMuncaTextBox.Text = currentPerioada != null ? currentPerioada.LocMunca.ToUpper() : "";
                locMuncaTextBox.Enabled = true;

                lucreazaCheckBox.Enabled = true;
                lucreazaUCurentaCheckBox.Enabled = true;

                normaComboBox.Enabled = true;
            }
        }

        private void concediuCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if(concediuCheckBox.Checked)
            {
                tipConcediuComboBox.Enabled = true;
                somerCheckBox.Enabled = false;
                lucreazaCheckBox.Enabled = false;
                functieTextBox.Text = "CONCEDIU";
                iomComboBox.Enabled = false;
                lucreazaUCurentaCheckBox.Enabled = false;
                lucreazaCheckBox.Enabled = false;
                locMuncaTextBox.Text = "CONCEDIU";
                normaComboBox.Enabled = false;
            }
            else
            {
                tipConcediuComboBox.Enabled = false;
                somerCheckBox.Enabled = true;
                lucreazaCheckBox.Enabled = true;
                iomComboBox.Enabled = true;
                lucreazaUCurentaCheckBox.Enabled = true;
                lucreazaCheckBox.Enabled = true;
                normaComboBox.Enabled = true;

                if (currentPerioada != null)
                {
                    locMuncaTextBox.Text = currentPerioada.LocMunca;
                    functieTextBox.Text = currentPerioada.Functie;
                }
                else
                {
                    locMuncaTextBox.Text = "";
                    functieTextBox.Text = "";
                }
            }
        }

        private void lucreazaUCurentaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if(lucreazaUCurentaCheckBox.Checked)
            {
                functieTextBox.Enabled = false;
                functieTextBox.Text = "GET SCHOOL INFO";
            }
            else
            {
                functieTextBox.Enabled = true;
                functieTextBox.Text = (currentPerioada == null ? "" : currentPerioada.Functie);
            }
        }

        #endregion Handlers

        private void AddToLocalList(Perioada perioada,bool modified = false)
        {
            //check for the same element in the list
            if(currentChangedPeriods.Where(x=> x==perioada).Count() > 0)
            {
                return;
            }

            perioada.Modified = modified;

            currentChangedPeriods.Add(perioada);

            UpdateDataGridView();
        }

        private void UpdateDataGridView()
        {
            dataGridView1.Rows.Clear();

            int count = 0;
            foreach (Perioada perioada in currentChangedPeriods.OrderBy(c => c.DTSfarsit))
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(dataGridView1);
                newRow.Cells[0].Value = perioada.ID;
                newRow.Cells[1].Value = count;
                newRow.Cells[2].Value = perioada.DTInceput.ToShortDateString();
                newRow.Cells[3].Value = perioada.DTSfarsit.ToShortDateString();
                newRow.Cells[4].Value = perioada.Difference.ElapsedYears + "-" + perioada.Difference.ElapsedMonths + "-" + perioada.Difference.ElapsedDays;
                newRow.Cells[5].Value = (perioada.Modified == true ? "MODIFICAT" : "ADAUGAT");
                dataGridView1.Rows.Add(newRow);
                count++;
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
    }
}
