using Itenso.TimePeriod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace VechimeSoftware
{
    public partial class PerioadaForm : Form
    {
        private MainForm parent;
        private Perioada currentPerioada;
        private int currentPersonIndex;

        private bool modifyMode = false;

        private List<Perioada> currentChangedPeriods;

        public PerioadaForm(MainForm _parent, Perioada _currentPerioada, int _currentPersonIndex)
        {
            InitializeComponent();
            parent = _parent;
            currentPerioada = _currentPerioada;
            currentPersonIndex = _currentPersonIndex;

            modifyMode = currentPerioada == null;

            currentChangedPeriods = new List<Perioada>();

            //functieComboBox.SelectedIndex = 0;
            normaComboBox.SelectedIndex = 0;
            iomComboBox.SelectedIndex = 0;
            tipConcediuComboBox.SelectedIndex = 0;

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
                normaComboBox.SelectedItem = currentPerioada.Norma.ToUpper();
                iomComboBox.SelectedItem = currentPerioada.IOM.ToUpper();
                functieComboBox.Text = currentPerioada.Functie.ToUpper();
                locMuncaTextBox.Text = currentPerioada.LocMunca.ToUpper();

                Perioada temporaryPerioada = new Perioada();
                temporaryPerioada.DTInceput = inceputTimePicker.Value;
                temporaryPerioada.DTSfarsit = sfarsitTimePicker.Value;
                temporaryPerioada.Norma = normaComboBox.Text;
                TimePeriod periodCalc = TimePeriod.CalculatePeriodTime(temporaryPerioada);

                perioadaTextBox.Text = periodCalc.Years + " ani " + periodCalc.Months + " luni " + periodCalc.Days + " zile";
                perioadaTextBox.Enabled = false;
                lucreazaCheckBox.Checked = currentPerioada.Lucreaza;
                lucreazaUCurentaCheckBox.Checked = (currentPerioada.LocMunca.ToUpper() == parent.currentUnitate.SC.ToUpper());
                locMuncaTextBox.Enabled = false;
                concediuCheckBox.Checked = (string.IsNullOrWhiteSpace(currentPerioada.TipCFS) ? false : true);
                tipConcediuComboBox.SelectedItem = currentPerioada.TipCFS.ToUpper();
                tipConcediuComboBox.Enabled = false;
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
                tipConcediuComboBox.Enabled = false;
            }
        }

        private void ResetForm(bool modified = false)
        {
            if (!modified)
            {
                currentPerioada = null;
                inceputTimePicker.Value = DateTime.Now;
                sfarsitTimePicker.Value = DateTime.Now;
                normaComboBox.SelectedIndex = 0;
                iomComboBox.SelectedIndex = 0;
                functieComboBox.SelectedItem = "";
                locMuncaTextBox.Text = "";
                perioadaTextBox.Text = "0 ani 0 luni 0 zile";
                lucreazaCheckBox.Checked = false;
                concediuCheckBox.Checked = false;
                tipConcediuComboBox.Enabled = false;
                lucreazaUCurentaCheckBox.Checked = false;
                tipConcediuComboBox.SelectedIndex = 0;
                somerCheckBox.Checked = false;
                iomComboBox.Enabled = true;
                editButton.Enabled = false;
                saveButton.Enabled = false;
                addButton.Enabled = true;
            }
            else
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

                functieComboBox.Text = currentPerioada.Functie.ToString().ToUpper();
                locMuncaTextBox.Text = currentPerioada.LocMunca.ToString().ToUpper();

                Perioada temporaryPerioada = new Perioada();
                temporaryPerioada.DTInceput = inceputTimePicker.Value;
                temporaryPerioada.DTSfarsit = sfarsitTimePicker.Value;
                temporaryPerioada.Norma = normaComboBox.Text;

                TimePeriod periodCalc = TimePeriod.CalculatePeriodTime(temporaryPerioada);

                perioadaTextBox.Text = periodCalc.Years + " ani " + periodCalc.Months + " luni " + periodCalc.Days + " zile";

                lucreazaUCurentaCheckBox.Checked = (currentPerioada.LocMunca.ToUpper() == parent.currentUnitate.SC.ToUpper());
                locMuncaTextBox.Enabled = !lucreazaUCurentaCheckBox.Checked;

                lucreazaCheckBox.Checked = currentPerioada.Lucreaza;

                concediuCheckBox.Checked = (string.IsNullOrWhiteSpace(currentPerioada.TipCFS) ? false : true);

                tipConcediuComboBox.Enabled = concediuCheckBox.Checked;
                tipConcediuComboBox.SelectedItem = currentPerioada.TipCFS.ToUpper();

                somerCheckBox.Checked = currentPerioada.Somaj;

                if (currentPerioada.Somaj)
                {
                    iomComboBox.Items.Add("SOMAJ");
                    iomComboBox.SelectedIndex = 2;
                }

                if (concediuCheckBox.Checked)
                {
                    tipConcediuComboBox.Enabled = true;
                    somerCheckBox.Enabled = false;
                    lucreazaCheckBox.Enabled = false;
                    functieComboBox.SelectedItem = "CONCEDIU";
                    functieComboBox.Enabled = false;
                    iomComboBox.Enabled = false;
                    lucreazaUCurentaCheckBox.Enabled = false;
                    lucreazaCheckBox.Enabled = false;
                    locMuncaTextBox.Text = "CONCEDIU";
                    locMuncaTextBox.Enabled = false;
                    normaComboBox.Enabled = false;
                }

                editButton.Enabled = true;
                saveButton.Enabled = false;
                addButton.Enabled = false;
            }
        }

        #endregion UI

        #region Handlers

        private void editButton_Click(object sender, EventArgs e)
        {
            if (currentPerioada == null)
                return;

            if (MessageBox.Show("Sunteti sigur(a) ca vreti sa modificati aceasta informatie?", "Atentie!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SetEnabled(this, true);

                modifyMode = true;

                ResetForm(true);

                perioadaTextBox.Enabled = false;
                editButton.Enabled = false;
                saveButton.Enabled = true;
                addButton.Enabled = false;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(locMuncaTextBox.Text))
            {
                MessageBox.Show("Toate casutele trebuie completate!", "Atentie!");
                return;
            }

            if (!VerificaData(inceputTimePicker.Value, sfarsitTimePicker.Value, true))
                return;

            currentPerioada = new Perioada();
            currentPerioada.DTInceput = inceputTimePicker.Value;
            currentPerioada.DTSfarsit = sfarsitTimePicker.Value;
            currentPerioada.CFS = concediuCheckBox.Checked;
            currentPerioada.TipCFS = (currentPerioada.CFS ? tipConcediuComboBox.SelectedItem.ToString() : "");
            if (currentPerioada.CFS)
            {
                currentPerioada.Functie = "CONCEDIU";
                currentPerioada.LocMunca = "CONCEDIU";
                currentPerioada.IOM = "CONCEDIU";
                currentPerioada.Lucreaza = false;
                currentPerioada.LucreazaUnitateaCurenta = false;
                currentPerioada.Norma = "1/1";
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
                    currentPerioada.Norma = "1/1";
                }
                else
                {
                    currentPerioada.LocMunca = locMuncaTextBox.Text.ToUpper();
                    currentPerioada.IOM = iomComboBox.SelectedItem.ToString();
                    currentPerioada.Norma = normaComboBox.SelectedItem.ToString();
                    currentPerioada.Lucreaza = lucreazaCheckBox.Checked;
                    currentPerioada.LucreazaUnitateaCurenta = lucreazaUCurentaCheckBox.Checked;
                    currentPerioada.Functie = functieComboBox.Text.ToString().ToUpper();
                }
            }

            currentPerioada.Somaj = somerCheckBox.Checked;

            parent.AddPerioada(currentPerioada, currentPersonIndex);

            AddToLocalList(currentPerioada);

            //TO-DO RESET FORM
            ResetForm();

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
            if (currentPerioada.CFS)
            {
                currentPerioada.Functie = "CONCEDIU";
                currentPerioada.LocMunca = "CONCEDIU";
                currentPerioada.IOM = "CONCEDIU";
                currentPerioada.Lucreaza = false;
                currentPerioada.LucreazaUnitateaCurenta = false;
                currentPerioada.Norma = "1/1";
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
                    currentPerioada.Norma = "1/1";
                }
                else
                {
                    currentPerioada.LocMunca = locMuncaTextBox.Text.ToUpper();
                    currentPerioada.IOM = iomComboBox.SelectedItem.ToString();
                    currentPerioada.Norma = normaComboBox.SelectedItem.ToString();
                    currentPerioada.Lucreaza = lucreazaCheckBox.Checked;
                    currentPerioada.LucreazaUnitateaCurenta = lucreazaUCurentaCheckBox.Checked;
                    currentPerioada.Functie = functieComboBox.Text.ToUpper();
                }
            }
            currentPerioada.Somaj = somerCheckBox.Checked;

            parent.ModifyPerioada(currentPerioada, currentPersonIndex);

            AddToLocalList(currentPerioada, true);

            //TO-DO RESET FORM
            ResetForm(true);
            editButton.Enabled = false;
            saveButton.Enabled = true;
        }

        private bool VerificaData(DateTime inceput, DateTime sfarsit, bool adding = false)
        {
            Person selectedPerson = null;
            if (parent.peopleDictionary.ContainsKey(currentPersonIndex))
                selectedPerson = parent.peopleDictionary[currentPersonIndex];

            if (!adding)
            {
                foreach (Perioada perioada in selectedPerson.Perioade.Where(x => (x.DTInceput != inceput && x.DTSfarsit != sfarsit)))
                {
                    if (inceput.CompareTo(perioada.DTInceput) > 0 && inceput.CompareTo(perioada.DTSfarsit) < 0)
                    {
                        MessageBox.Show("Aceasta perioada este cuprinsa in alta perioada deja inregistrata.");
                        return false;
                    }
                    if (sfarsit.CompareTo(perioada.DTInceput) > 0 && sfarsit.CompareTo(perioada.DTSfarsit) < 0)
                    {
                        MessageBox.Show("Aceasta perioada este cuprinsa in alta perioada deja inregistrata.");
                        return false;
                    }
                }
            }
            else
            {
                foreach (Perioada perioada in selectedPerson.Perioade)
                {
                    if (inceput.CompareTo(perioada.DTInceput) > 0 && inceput.CompareTo(perioada.DTSfarsit) < 0)
                    {
                        MessageBox.Show("Aceasta perioada este cuprinsa in alta perioada deja inregistrata.");
                        return false;
                    }
                    if (sfarsit.CompareTo(perioada.DTInceput) > 0 && sfarsit.CompareTo(perioada.DTSfarsit) < 0)
                    {
                        MessageBox.Show("Aceasta perioada este cuprinsa in alta perioada deja inregistrata.");
                        return false;
                    }
                }
            }
            return true;
        }

        private void timePicker_ValueChanged(object sender, EventArgs e)
        {
            SendKeys.Send("{.}");

            DateTime firstDate = inceputTimePicker.Value;
            DateTime secondDate = sfarsitTimePicker.Value;
            // DateDiff span = new DateDiff(firstDate.Subtract(new TimeSpan(1, 0, 0, 0, 0)), secondDate.Subtract(new TimeSpan(1, 0, 0, 0, 0)));

            Perioada temporaryPerioada = new Perioada();
            temporaryPerioada.DTInceput = firstDate;
            temporaryPerioada.DTSfarsit = secondDate;
            temporaryPerioada.Norma = normaComboBox.Text;
            TimePeriod periodCalc = TimePeriod.CalculatePeriodTime(temporaryPerioada);

            perioadaTextBox.Text = periodCalc.Years + " ani " + periodCalc.Months + " luni " + periodCalc.Days + " zile";
        }

        private void PerioadaForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                SendKeys.Send("{Tab}");
        }

        private void normaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Perioada temporaryPerioada = new Perioada();
            temporaryPerioada.DTInceput = inceputTimePicker.Value;
            temporaryPerioada.DTSfarsit = sfarsitTimePicker.Value;
            temporaryPerioada.Norma = normaComboBox.Text;
            TimePeriod periodCalc = TimePeriod.CalculatePeriodTime(temporaryPerioada);

            perioadaTextBox.Text = periodCalc.Years + " ani " + periodCalc.Months + " luni " + periodCalc.Days + " zile";
        }

        private void somajCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (somerCheckBox.Checked)
            {
                iomComboBox.Items.Add("SOMER");
                iomComboBox.SelectedIndex = 2;
                iomComboBox.Enabled = false;

                functieComboBox.SelectedItem = "SOMER";
                functieComboBox.Enabled = false;

                locMuncaTextBox.Text = "SOMER";
                locMuncaTextBox.Enabled = false;

                lucreazaCheckBox.Enabled = false;
                lucreazaUCurentaCheckBox.Enabled = false;

                normaComboBox.SelectedItem = "1/1";
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

                functieComboBox.Text = currentPerioada != null ? currentPerioada.Functie.ToUpper() : "";
                functieComboBox.Enabled = true;

                locMuncaTextBox.Text = currentPerioada != null ? currentPerioada.LocMunca.ToUpper() : "";
                locMuncaTextBox.Enabled = true;

                lucreazaCheckBox.Enabled = true;
                lucreazaUCurentaCheckBox.Enabled = true;

                normaComboBox.Enabled = true;
            }
        }

        private void concediuCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (concediuCheckBox.Checked)
            {
                tipConcediuComboBox.Enabled = true;
                somerCheckBox.Enabled = false;
                lucreazaCheckBox.Enabled = false;
                functieComboBox.SelectedItem = "CONCEDIU";
                functieComboBox.Enabled = false;
                iomComboBox.Enabled = false;
                lucreazaUCurentaCheckBox.Enabled = false;
                lucreazaCheckBox.Enabled = false;
                locMuncaTextBox.Text = "CONCEDIU";
                locMuncaTextBox.Enabled = false;
                normaComboBox.Enabled = false;
            }
            else
            {
                tipConcediuComboBox.Enabled = false;
                somerCheckBox.Enabled = true;
                lucreazaCheckBox.Enabled = true;
                functieComboBox.Enabled = true;
                locMuncaTextBox.Enabled = true;
                iomComboBox.Enabled = true;
                lucreazaUCurentaCheckBox.Enabled = true;
                lucreazaCheckBox.Enabled = true;
                normaComboBox.Enabled = true;

                if (currentPerioada != null)
                {
                    locMuncaTextBox.Text = currentPerioada.LocMunca;
                    functieComboBox.Text = currentPerioada.Functie;
                }
                else
                {
                    locMuncaTextBox.Text = "";
                    functieComboBox.SelectedItem = "";
                }
            }
        }

        private void lucreazaUCurentaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (lucreazaUCurentaCheckBox.Checked)
            {
                locMuncaTextBox.Enabled = false;
                locMuncaTextBox.Text = parent.currentUnitate.SC.ToUpper();
            }
            else
            {
                locMuncaTextBox.Enabled = true;
                locMuncaTextBox.Text = (currentPerioada == null ? "" : currentPerioada.LocMunca);
            }
        }

        #endregion Handlers

        private void AddToLocalList(Perioada perioada, bool modified = false)
        {
            UpdateDataGridView();
            //check for the same element in the list
            if (currentChangedPeriods.Where(x => x == perioada).Count() > 0)
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
                TimePeriod periodCalc = TimePeriod.CalculatePeriodTime(perioada);

                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(dataGridView1);
                newRow.Cells[0].Value = perioada.ID;
                newRow.Cells[1].Value = count;
                newRow.Cells[2].Value = perioada.DTInceput.ToShortDateString();
                newRow.Cells[3].Value = perioada.DTSfarsit.ToShortDateString();
                newRow.Cells[4].Value = periodCalc.Years + "-" + periodCalc.Months + "-" + periodCalc.Days;
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
