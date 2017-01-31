using Itenso.TimePeriod;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace VechimeSoftware
{
    public partial class MainForm : Form
    {
        private string databasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Baza.mdb";
        private string connectionString = "";
        public Dictionary<int, Person> peopleDictionary = new Dictionary<int, Person>();
        private List<Person> displayPeople = new List<Person>();

        public MainForm()
        {
            InitializeComponent();
            connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + databasePath + @"; Persist Security Info=False;";
            peopleDictionary = GetPeople();
            displayPeople = peopleDictionary.Values.ToList();
            UpdatePeopleInfo();
        }

        #region UpdateStuff

        private void UpdatePeople()
        {
            peopleDictionary = GetPeople();
            displayPeople = peopleDictionary.Values.ToList();
            UpdateList();
        }

        private void UpdateList()
        {
            peopleListBox.DataSource = null;
            peopleListBox.DataSource = displayPeople;
            peopleListBox.DisplayMember = "NumeIntreg";
            peopleListBox.ValueMember = "ID";

            peopleListBox.SelectedIndex = -1;
        }

        private void UpdatePeopleInfo()
        {
            UpdateList();
            GetAllPerioadaList();
        }

        #endregion UpdateStuff

        #region SQL-Methods

        public Dictionary<int, Person> GetPeople()
        {
            Dictionary<int, Person> peopleDict = new Dictionary<int, Person>();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand("SELECT * FROM Persoane", connection))
                {
                    connection.Open();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Person newPerson = new Person();
                                newPerson.ID = Convert.ToInt32(reader[0]);
                                newPerson.Nume = Convert.ToString(reader[1]);
                                newPerson.Prenume = Convert.ToString(reader[2]);
                                newPerson.CNP = Convert.ToString(reader[3]);
                                newPerson.Serie = Convert.ToString(reader[4]);
                                newPerson.Perioade = new List<Perioada>();
                                peopleDict.Add(newPerson.ID, newPerson);
                            }
                        }
                    }
                }
            }

            return peopleDict;
        } //done

        public List<Perioada> GetPerioadaList(int ID)
        {
            List<Perioada> perioadaList = new List<Perioada>();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand("SELECT * FROM Perioade WHERE Id_Persoana=@id", connection))
                {
                    OleDbParameter idParameter = new OleDbParameter("@id", ID);
                    command.Parameters.Add(idParameter);

                    connection.Open();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Perioada newPerioada = new Perioada();
                                newPerioada.ID = Convert.ToInt32(reader[0]);
                                newPerioada.DTInceput = Convert.ToDateTime(reader[2]);
                                newPerioada.DTSfarsit = Convert.ToDateTime(reader[3]);
                                newPerioada.CFS = Convert.ToBoolean(reader[4]);
                                newPerioada.TipCFS = Convert.ToString(reader[5]);
                                newPerioada.Norma = Convert.ToString(reader[6]);
                                newPerioada.Functie = Convert.ToString(reader[7]);
                                newPerioada.IOM = Convert.ToString(reader[8]);
                                newPerioada.LocMunca = Convert.ToString(reader[9]);
                                newPerioada.Lucreaza = Convert.ToBoolean(reader[10]);
                                newPerioada.Somaj = Convert.ToBoolean(reader[11]);
                                perioadaList.Add(newPerioada);
                            }
                        }
                    }
                }
            }

            return perioadaList;
        } //done

        public void GetAllPerioadaList()
        {
            foreach (Person person in peopleDictionary.Values)
            {
                person.Perioade.Clear();
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand("SELECT * FROM Perioade", connection))
                {
                    connection.Open();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Perioada newPerioada = new Perioada();
                                newPerioada.ID = Convert.ToInt32(reader[0]);
                                int ID_Person = Convert.ToInt16(reader[1]);
                                newPerioada.DTInceput = Convert.ToDateTime(reader[2]);
                                newPerioada.DTSfarsit = Convert.ToDateTime(reader[3]);
                                newPerioada.CFS = Convert.ToBoolean(reader[4]);
                                newPerioada.TipCFS = Convert.ToString(reader[5]);
                                newPerioada.Norma = Convert.ToString(reader[6]);
                                newPerioada.Functie = Convert.ToString(reader[7]);
                                newPerioada.IOM = Convert.ToString(reader[8]);
                                newPerioada.LocMunca = Convert.ToString(reader[9]);
                                newPerioada.Lucreaza = Convert.ToBoolean(reader[10]);
                                newPerioada.Somaj = Convert.ToBoolean(reader[11]);

                                if (peopleDictionary.ContainsKey(ID_Person))
                                {
                                    peopleDictionary[ID_Person].Perioade.Add(newPerioada);
                                }
                            }
                        }
                    }
                }
            }
        } //done

        public void AddPerson(Person newPerson)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand("INSERT INTO Persoane (Nume,Prenume,CNP,Serie) VALUES (@nume,@prenume,@cnp,@serie)", connection))
                {
                    command.Parameters.Add("@nume", OleDbType.VarChar, 50).Value = newPerson.Nume;
                    command.Parameters.Add("@prenume", OleDbType.VarChar, 50).Value = newPerson.Prenume;
                    command.Parameters.Add("@cnp", OleDbType.VarChar, 13).Value = newPerson.CNP;
                    command.Parameters.Add("@serie", OleDbType.VarChar, 6).Value = newPerson.Serie;
                    connection.Open();

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }
            UpdatePeople();
        } //done

        public void ModifyPerson(Person newPerson)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand("UPDATE Persoane SET Nume = @nume ,Prenume = @prenume,CNP = @cnp, Serie = @serie WHERE Id = @id", connection))
                {
                    command.Parameters.Add("@nume", OleDbType.VarChar, 50).Value = newPerson.Nume;
                    command.Parameters.Add("@prenume", OleDbType.VarChar, 50).Value = newPerson.Prenume;
                    command.Parameters.Add("@cnp", OleDbType.VarChar, 13).Value = newPerson.CNP;
                    command.Parameters.Add("@serie", OleDbType.VarChar, 6).Value = newPerson.Serie;
                    command.Parameters.Add("@id", OleDbType.Integer).Value = newPerson.ID;
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }

            UpdatePeople();
        } //done

        public void DeletePerson(int personID)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand("DELETE FROM Persoane WHERE Id=@id", connection))
                {
                    command.Parameters.Add("@Id", OleDbType.Integer).Value = personID;
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }

            peopleDictionary = GetPeople();
            displayPeople = peopleDictionary.Values.ToList();
            UpdatePeopleInfo();
        } //done

        public void AddPerioada(Perioada currentPerioada, int personID)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                string commandString = @"INSERT INTO Perioade (Id_Persoana,Data_Inceput,Data_Sfarsit,CFS,TipCFS,Norma,Functie,InvORMunca,Loc_Munca,Lucreaza,Somaj)
                                                VALUES (@Id_Persoana,@Data_Inceput,@Data_Sfarsit,@CFS,@TipCFS,@Norma,@Functie,@InvORMunca,@Loc_Munca,@Lucreaza,@Somaj)";
                using (OleDbCommand command = new OleDbCommand(commandString, connection))
                {
                    command.Parameters.Add("@Id_Persoana", OleDbType.Integer).Value = personID;
                    command.Parameters.Add("@Data_Inceput", OleDbType.Date).Value = currentPerioada.DTInceput;
                    command.Parameters.Add("@Data_Sfarsit", OleDbType.Date).Value = currentPerioada.DTSfarsit;
                    command.Parameters.Add("@CFS", OleDbType.Boolean).Value = currentPerioada.CFS;
                    command.Parameters.Add("@TipCFS", OleDbType.VarChar).Value = currentPerioada.TipCFS;
                    command.Parameters.Add("@Norma", OleDbType.VarChar).Value = currentPerioada.Norma;
                    command.Parameters.Add("@Functie", OleDbType.VarChar).Value = currentPerioada.Functie;
                    command.Parameters.Add("@InvORMunca", OleDbType.VarChar).Value = currentPerioada.IOM;
                    command.Parameters.Add("@Loc_Munca", OleDbType.VarChar).Value = currentPerioada.LocMunca;
                    command.Parameters.Add("@Lucreaza", OleDbType.Boolean).Value = currentPerioada.Lucreaza;
                    command.Parameters.Add("@Somaj", OleDbType.Boolean).Value = currentPerioada.Somaj;
                    connection.Open();

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }

            if (peopleDictionary.ContainsKey(personID))
            {
                peopleDictionary[personID].Perioade = GetPerioadaList(personID);
                peopleListBox.SelectedIndex = -1;
            }
        } //done

        public void ModifyPerioada(Perioada currentPerioada, int personID)
        {
            int recordsChanged = 0;
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                string commandString = "UPDATE Perioade SET Id_Persoana=@Id_Persoana,Data_Inceput=@Data_Inceput,Data_Sfarsit=@Data_Sfarsit,CFS_Zile_Personal=@CFS_Zile_Personal," +
                                                             "CFS_Luni_Personal=@CFS_Luni_Personal,CFS_Ani_Personal=@CFS_Ani_Personal,CFS_Zile_Studii=@CFS_Zile_Studii,CFS_Luni_Studii=@CFS_Luni_Studii,"+
                                                             "CFS_Ani_Studii=@CFS_Ani_Studii,Norma=@Norma,Functie=@Functie,InvORMunca=@InvORMunca,Loc_Munca=@Loc_Munca,Lucreaza=@Lucreaza,Somaj=@Somaj WHERE Id=@Id";
                using (OleDbCommand command = new OleDbCommand(commandString, connection))
                {
                    command.Parameters.Add("@Id_Persoana", OleDbType.Integer).Value = personID;
                    command.Parameters.Add("@Data_Inceput", OleDbType.Date).Value = currentPerioada.DTInceput;
                    command.Parameters.Add("@Data_Sfarsit", OleDbType.Date).Value = currentPerioada.DTSfarsit;
                    command.Parameters.Add("@CFS", OleDbType.Boolean).Value = currentPerioada.CFS;
                    command.Parameters.Add("@TipCFS", OleDbType.VarChar).Value = currentPerioada.TipCFS;
                    command.Parameters.Add("@Norma", OleDbType.VarChar).Value = currentPerioada.Norma;
                    command.Parameters.Add("@Functie", OleDbType.VarChar).Value = currentPerioada.Functie;
                    command.Parameters.Add("@InvORMunca", OleDbType.VarChar).Value = currentPerioada.IOM;
                    command.Parameters.Add("@Loc_Munca", OleDbType.VarChar).Value = currentPerioada.LocMunca;
                    command.Parameters.Add("@Lucreaza", OleDbType.Boolean).Value = currentPerioada.Lucreaza;
                    command.Parameters.Add("@Somaj", OleDbType.Boolean).Value = currentPerioada.Somaj;
                    command.Parameters.Add("@Id", OleDbType.Integer).Value = currentPerioada.ID;
                    connection.Open();

                    try
                    {
                        recordsChanged = command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }

            if (peopleDictionary.ContainsKey(personID) && recordsChanged != 0)
            {
                peopleDictionary[personID].Perioade = GetPerioadaList(personID);
            }
            peopleListBox.SelectedIndex = -1;
        } //done

        public void DeletePerioada(int perioadaID, int personID)
        {
            int recordsChanged = 0;
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand("DELETE FROM Perioade WHERE Id=@Id", connection))
                {
                    command.Parameters.Add("@Id", OleDbType.Integer).Value = perioadaID;
                    connection.Open();
                    try
                    {
                        recordsChanged = command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }

            if (peopleDictionary.ContainsKey(personID) && recordsChanged!=0)
            {
                peopleDictionary[personID].Perioade = GetPerioadaList(personID);
            }
            peopleListBox.SelectedIndex = -1;
        } //done

        #endregion SQL-Methods

        #region PeopleList Handlers

        private void selectedIndexChanged(object sender, EventArgs e)
        {
            if (peopleListBox.SelectedIndex >= 0)
            {
                Person selectedPerson = null;
                if (peopleDictionary.ContainsKey((peopleListBox.SelectedItem as Person).ID))
                    selectedPerson = peopleDictionary[(peopleListBox.SelectedItem as Person).ID];
                else
                {
                    MessageBox.Show("Person doesn't exist!", "Error!");
                    return;
                }

                dataGridView1.Rows.Clear();

                int count = 0;
                foreach (Perioada perioada in selectedPerson.Perioade)
                {
                  /*  DataGridViewRow newRow = new DataGridViewRow();
                    newRow.CreateCells(dataGridView1);
                    newRow.Cells[0].Value = perioada.ID;
                    newRow.Cells[1].Value = count;
                    newRow.Cells[2].Value = perioada.DTInceput.ToShortDateString();
                    newRow.Cells[3].Value = perioada.DTSfarsit.ToShortDateString();
                    newRow.Cells[4].Value = perioada.CFSAni_Personal.ToString() + "-" + perioada.CFSLuni_Personal.ToString() + "-" + perioada.CFSZile_Personal.ToString();
                    newRow.Cells[5].Value = perioada.CFSAni_Studii.ToString() + "-" + perioada.CFSLuni_Studii.ToString() + "-" + perioada.CFSZile_Studii.ToString();
                    newRow.Cells[6].Value = perioada.Difference.ElapsedYears + "-" + perioada.Difference.ElapsedMonths + "-" + perioada.Difference.ElapsedDays;
                    newRow.Cells[7].Value = perioada.Norma.ToUpper();
                    newRow.Cells[8].Value = perioada.Functie.ToUpper();
                    newRow.Cells[9].Value = perioada.IOM.ToUpper();
                    newRow.Cells[10].Value = perioada.LocMunca.ToUpper();
                    newRow.Cells[11].Value = perioada.Lucreaza;
                    dataGridView1.Rows.Add(newRow);
                    count++;*/
                }

                perioadaInvTB.Text = selectedPerson.PerioadaInv.Years.ToString() + " ani " + selectedPerson.PerioadaInv.Months.ToString() + " luni " + selectedPerson.PerioadaInv.Days.ToString() + " zile";
                perioadaTotalTB.Text = selectedPerson.PerioadaMunca.Years.ToString() + " ani " + selectedPerson.PerioadaMunca.Months.ToString() + " luni " + selectedPerson.PerioadaMunca.Days.ToString() + " zile";

            }
            else
            {
                dataGridView1.Rows.Clear();
                perioadaInvTB.Text = "";
                perioadaTotalTB.Text = "";
            }
        } //done

        #endregion PeopleList Handlers

        #region Search

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                List<Person> newList;
                newList = peopleDictionary.Values.Where(x => x.Nume.ToLower().Contains(searchTextBox.Text.ToLower()) || x.Prenume.ToLower().Contains(searchTextBox.Text.ToLower())).ToList();
                if (newList != null)
                {
                    displayPeople = newList;
                    UpdateList();
                    ChangeResultLabel(newList.Count);
                }
                else
                {
                    displayPeople = new List<Person>();
                    UpdateList();
                    ChangeResultLabel(0);
                }
            }
            else
            {
                displayPeople = peopleDictionary.Values.ToList();
                UpdateList();
                ChangeResultLabel(-1);
            }
        } //done

        private void ChangeResultLabel(int number)
        {
            if (number < 0)
            {
                resultLabel.Text = "0 persoane";
            }
            else if (number == 0)
            {
                resultLabel.Text = "Nu s-a gasit nicio persoana.";
            }
            else if (number == 1)
            {
                resultLabel.Text = number.ToString() + " persoana";
            }
            else
            {
                resultLabel.Text = number.ToString() + " persoane";
            }
        }

        #endregion Search

        #region Menu Strip Handlers

        private void iesireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #region DetailHandlers

        private void veziDetaliiModificaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Person selectedPerson = null;
            if (peopleListBox.SelectedIndex >= 0 && peopleDictionary.ContainsKey((peopleListBox.SelectedItem as Person).ID))
                selectedPerson = peopleDictionary[(peopleListBox.SelectedItem as Person).ID];
            if (selectedPerson != null)
            {
                using (PersonForm personForm = new PersonForm(this, selectedPerson))
                {
                    personForm.ShowDialog();
                }
            }
        }

        private void veziDetaliiPerioadaModificaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Perioada selectedPerioada = null;
            int selectedIndex = -1;
            if (dataGridView1.SelectedRows.Count > 0 && peopleDictionary.ContainsKey((peopleListBox.SelectedItem as Person).ID))
            {
                selectedIndex = peopleDictionary[(peopleListBox.SelectedItem as Person).ID].ID;
                int selectedIndexPerioada = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells["placeList"].Value);
                selectedPerioada = peopleDictionary[(peopleListBox.SelectedItem as Person).ID].Perioade[selectedIndexPerioada];
            }

            if (selectedPerioada != null && selectedIndex != -1)
            {
                using (PerioadaForm perioadaForm = new PerioadaForm(this, selectedPerioada, selectedIndex))
                {
                    perioadaForm.ShowDialog();
                }
            }
        }

        #endregion DetailHandlers

        #region AddHandlers

        private void adaugaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PersonForm personForm = new PersonForm(this, null))
            {
                personForm.ShowDialog();
            }
        }

        private void adaugaToolPerioadaStripMenuItem_Click(object sender, EventArgs e)
        {
            int selectedIndex = -1;
            if (peopleListBox.SelectedIndex >= 0 && peopleDictionary.ContainsKey((peopleListBox.SelectedItem as Person).ID))
                selectedIndex = peopleDictionary[(peopleListBox.SelectedItem as Person).ID].ID;
            if (selectedIndex != -1)
            {
                using (PerioadaForm perioadaForm = new PerioadaForm(this, null, selectedIndex))
                {
                    perioadaForm.ShowDialog();
                }
            }
        }

        #endregion AddHandlers

        #region DeleteHandlers

        private void stergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Person selectedPerson = null;
            if (peopleListBox.SelectedIndex >= 0 && peopleDictionary.ContainsKey((peopleListBox.SelectedItem as Person).ID))
                selectedPerson = peopleDictionary[(peopleListBox.SelectedItem as Person).ID];
            if (selectedPerson != null)
            {
                if (MessageBox.Show("Sigur doriti sa stergeti aceasta persoana?", "Atentie", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DeletePerson(selectedPerson.ID);
                }
            }
        }

        private void stergeToolPerioadaStripMenuItem_Click(object sender, EventArgs e)
        {
            Perioada selectedPerioada = null;
            int selectedIndex = -1;
            if (dataGridView1.SelectedRows.Count > 0 && peopleDictionary.ContainsKey((peopleListBox.SelectedItem as Person).ID))
            {
                selectedIndex = peopleDictionary[(peopleListBox.SelectedItem as Person).ID].ID;
                int selectedIndexPerioada = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["placeList"].Value);
                selectedPerioada = peopleDictionary[(peopleListBox.SelectedItem as Person).ID].Perioade[selectedIndexPerioada];
            }

            if (selectedPerioada != null && selectedIndex != -1)
            {
                if (MessageBox.Show("Sigur doriti sa stergeti aceasta perioada?", "Atentie", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DeletePerioada(selectedPerioada.ID, selectedIndex);
                }
            }
        }

        #endregion DeleteHandlers

        #region UpdateHandlers

        private void actualizareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Person selectedPerson = null;
            if (peopleListBox.SelectedIndex >= 0 && peopleDictionary.ContainsKey((peopleListBox.SelectedItem as Person).ID))
                selectedPerson = peopleDictionary[(peopleListBox.SelectedItem as Person).ID];
            if (selectedPerson != null)
            {
                if (MessageBox.Show("Sigur doriti sa actualizati perioadele?", "Atentie", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    for (int i = 0; i < selectedPerson.Perioade.Count; i++)
                    {
                        if(selectedPerson.Perioade[i].Lucreaza == true)
                        {
                            selectedPerson.Perioade[i].DTSfarsit = DateTime.Today;
                            ModifyPerioada(selectedPerson.Perioade[i], selectedPerson.ID);
                        }
                    }
                }
            }

        }

        #endregion

        #endregion

        #region DrawHandlers

        private SolidBrush reportsForegroundBrushSelected = new SolidBrush(System.Drawing.Color.White);
        private SolidBrush reportsForegroundBrush = new SolidBrush(System.Drawing.Color.Black);
        private SolidBrush reportsBackgroundBrushSelected = new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.Highlight));
        private SolidBrush reportsBackgroundBrush1 = new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.ButtonFace));
        private SolidBrush reportsBackgroundBrush2 = new SolidBrush(System.Drawing.Color.LightGray);

        private void peopleListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);

            int index = e.Index;
            if (index >= 0 && index < peopleListBox.Items.Count)
            {
                string FullName = (peopleListBox.Items[index] as Person).NumeIntreg;
                Graphics g = e.Graphics;

                //background:
                SolidBrush backgroundBrush;
                if (selected)
                    backgroundBrush = reportsBackgroundBrushSelected;
                else if ((index % 2) == 0)
                    backgroundBrush = reportsBackgroundBrush1;
                else
                    backgroundBrush = reportsBackgroundBrush2;
                g.FillRectangle(backgroundBrush, e.Bounds);

                //text:
                SolidBrush foregroundBrush = (selected) ? reportsForegroundBrushSelected : reportsForegroundBrush;
                g.DrawString(FullName, e.Font, foregroundBrush, new PointF(e.Bounds.X, e.Bounds.Y));
            }

            e.DrawFocusRectangle();
        }

        #endregion DrawHandlers

        #region GeneratePdf

        // Generez pdf pentru persoana selectata
        private void persoanaSelectataGenerateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (peopleListBox.SelectedIndex >= 0)
            {
                Person selectedPerson = null;
                if (peopleDictionary.ContainsKey((peopleListBox.SelectedItem as Person).ID))
                    selectedPerson = peopleDictionary[(peopleListBox.SelectedItem as Person).ID];
                else
                {
                    MessageBox.Show("Person doesn't exist!", "Error!");
                    return;
                }

                PdfDocument document = new PdfDocument();

                PdfPage page = document.AddPage();

                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont fontTitle = new XFont("Verdana", 20);

                // Adaug titlu
                gfx.DrawString("Raport vechime individual"
                              , fontTitle, XBrushes.Black, new XRect(0, 20, page.Width, page.Height)
                              , XStringFormats.TopCenter);

                // Adaug informatii despre persoana
                XFont font = new XFont("Verdana", 11);


                gfx.DrawString("Persoana: " + selectedPerson.NumeIntreg
                              , font, XBrushes.Black,
                               new XRect(45, 70, page.Width, page.Height),
                               XStringFormats.TopLeft);

                gfx.DrawString("CNP: " + selectedPerson.CNP
                             , font, XBrushes.Black,
                               new XRect(45, 82, page.Width, page.Height),
                               XStringFormats.TopLeft);

                gfx.DrawString("SERIE: " + selectedPerson.Serie
                            , font, XBrushes.Black,
                              new XRect(45, 94, page.Width, page.Height),
                              XStringFormats.TopLeft);

                int wd = Convert.ToInt32(page.Width.Value);

                // Prima linie
                gfx.DrawLine(new XPen(XColor.FromName("black")), new System.Windows.Point(20, 118), new System.Windows.Point(wd - 20, 118));

                XFont fontTableHead = new XFont("Verdana", 10);

                gfx.DrawString("Nr.crt.  Data inceput  Data sfarsit  CFST(aa-ll-zz)  Norma  Invatamant/Munca  Ani  Luni  Zile  Locul de munca"
                              , fontTableHead, XBrushes.Black,
                              new XRect(25, 128, page.Width, page.Height),
                              XStringFormats.TopLeft);

                XFont fontList = new XFont("Consolas", 10);

                // A doua linie
                gfx.DrawLine(new XPen(XColor.FromName("black")), new System.Windows.Point(20, 148), new System.Windows.Point(wd - 20, 148));

                int count = 0;




                // Suma timpului
                int ani = 0, luni = 0, zile = 0;

                // Doar timpul in invatamant
                int aniInv = 0, luniInv = 0, zileInv = 0;

                // Pastreaza inaltimea la care am ajuns in pagina
                int currentHeight = 148;

                foreach (Perioada perioada in selectedPerson.Perioade)
                {
                    count++;

                    currentHeight += 13;
                    DateDiff diff = new DateDiff(perioada.DTInceput, perioada.DTSfarsit);

                    string rowString = string.Empty.PadLeft(100);

                    rowString = rowString.Insert(3, count.ToString());

                  /*  rowString = rowString.Insert(8, perioada.DTInceput.ToShortDateString());
                    rowString = rowString.Insert(21, perioada.DTSfarsit.ToShortDateString());
                    rowString = rowString.Insert(36, perioada.CFSAni_Personal.ToString() + "-" + perioada.CFSLuni_Personal.ToString() + "-" + perioada.CFSZile_Personal.ToString());
                    rowString = rowString.Insert(47, perioada.Norma.ToUpper());
                    rowString = rowString.Insert(55, perioada.IOM.ToUpper());
                    rowString = rowString.Insert(72, diff.ElapsedYears.ToString());
                    rowString = rowString.Insert(77, diff.ElapsedMonths.ToString());
                    rowString = rowString.Insert(82, diff.ElapsedDays.ToString());
                    rowString = rowString.Insert(87, perioada.LocMunca.ToUpper());*/

                    gfx.DrawString(rowString, fontList, XBrushes.Black,
                                   new XRect(25, currentHeight, page.Width, page.Height),
                                   XStringFormats.TopLeft);



                    //    if (perioada.Norma == "1/2")

                    //   else if (perioada.Norma == "1/4")

                    DateTime change = new DateTime(2002, 1, 1);

                    if (!perioada.Somaj || perioada.DTInceput.CompareTo(change) >= 0)
                    {

                        TimePeriod np = new TimePeriod();
                        np.Years = diff.ElapsedYears;
                        np.Months = diff.ElapsedMonths;
                        np.Days = diff.ElapsedDays;


                        // aplic norma,, folosesc inainte de 2002
                        if(perioada.DTInceput.CompareTo(change) <= 0)
                        if (perioada.Norma == "1/2")
                            np = TimePeriod.HalfTime(np);
                        else if (perioada.Norma == "1/4")
                            np = TimePeriod.QuarterTime(np);

                      /*  ani += perioada.CFSAni_Studii;
                        luni += perioada.CFSLuni_Studii;
                        zile += perioada.CFSZile_Studii;*/


                        ani += np.Years;
                        luni += np.Months;
                        zile += np.Days;

                        if (perioada.IOM.ToUpper() == "INVATAMANT")
                        {
                            aniInv += np.Years;
                            luniInv += np.Months;
                            zileInv += np.Days;
                        }
                    }
                }

                // Adaug spatiu dupa enumerarea perioadelor
                currentHeight += 40;

                // Clalculez timp in invatamant
                int zileInvCalc = Convert.ToInt32(zileInv % 30);
                int luniInvCalc = (luniInv + Convert.ToInt32(zileInv / 30)) % 12;
                int aniInvCalc = aniInv + (luniInv + Convert.ToInt32(zileInv / 30)) / 12;

                // Adaug timp invatamant
                gfx.DrawString("Vechime in invatamant: " + aniInvCalc + " ani, " + luniInvCalc + " luni, " + zileInvCalc + " zile. "
                                , fontList, XBrushes.Black,
                                new XRect(0, currentHeight, page.Width, page.Height),
                                XStringFormats.TopCenter);

                currentHeight += 15;

                // Calculez timp total
                int zileCalc = Convert.ToInt32(zile % 30);
                int luniCalc = (luni + Convert.ToInt32(zile / 30)) % 12;
                int aniCalc = ani + (luni + Convert.ToInt32(zile / 30)) / 12;

                // Adaug timp total
                gfx.DrawString("Vechime total: " + aniCalc + " ani, " + luniCalc + " luni, " + zileCalc + " zile. "
                                 , fontList, XBrushes.Black,
                                 new XRect(0, currentHeight, page.Width, page.Height),
                                 XStringFormats.TopCenter);

                // Adaug 4 litere random sa evit o problema cu PdfReader
                string filename = "Raport_" + RandomString(5) + ".pdf";

                string[] files = Directory.GetFiles(Application.StartupPath, "Raport_*.pdf");

                // Sterg fisierele pe care le-am creat anterior
                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                    }
                }

                document.Save(filename);

                var pdfProcess = Process.Start(filename);
            }
            else MessageBox.Show("Selectati mai intai o persoana!");
        }

        // Generez pdf pentru raport general
        private void toatePersoaneleGenerateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PdfDocument document = new PdfDocument();

            PdfPage page = document.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont fontTitle = new XFont("Verdana", 20);

            gfx.DrawString("Raport vechime general", fontTitle, XBrushes.Black,
                             new XRect(0, 20, page.Width, page.Height),
                             XStringFormats.TopCenter);

            int wd = Convert.ToInt32(page.Width.Value);

            gfx.DrawLine(new XPen(XColor.FromName("black")), new System.Windows.Point(20, 70), new System.Windows.Point(wd - 20, 70));

            XFont fontTableHead = new XFont("Verdana", 10);

            gfx.DrawString("Nr. crt.     Persoana                        CNP" +
                              "                   Vechime in invatamant            Vechime in munca"
                            , fontTableHead, XBrushes.Black,
                             new XRect(25, 80, page.Width, page.Height),
                             XStringFormats.TopLeft);

            gfx.DrawLine(new XPen(XColor.FromName("black")), new System.Windows.Point(20, 100), new System.Windows.Point(wd - 20, 100));

            XFont fontList = new XFont("Verdana", 8);

            int ani = 0, luni = 0, zile = 0, aniInv = 0, luniInv = 0, zileInv = 0;
            int currentHeight = 100, count = 0;

            int zileInvCalc = 0, luniInvCalc = 0, aniInvCalc = 0;
            int zileCalc = 0, luniCalc = 0, aniCalc = 0;

            foreach (Person person in peopleDictionary.Values)
            {
                count++;
                currentHeight += 15;

                foreach (Perioada period in person.Perioade)
                {
                    DateDiff diff = new DateDiff(period.DTInceput, period.DTSfarsit);
                    ani += diff.ElapsedYears;
                    luni += diff.ElapsedMonths;
                    zile += diff.ElapsedDays;

                    if (period.IOM.ToUpper() == "INVATAMANT")
                    {
                        aniInv += diff.ElapsedYears;
                        luniInv += diff.ElapsedMonths;
                        zileInv += diff.ElapsedDays;
                    }
                }

                zileInvCalc = Convert.ToInt32(zileInv % 30);
                luniInvCalc = (luniInv + Convert.ToInt32(zileInv / 30)) % 12;
                aniInvCalc = aniInv + (luniInv + Convert.ToInt32(zileInv / 30)) / 12;

                zileCalc = Convert.ToInt32(zile % 30);
                luniCalc = (luni + Convert.ToInt32(zile / 30)) % 12;
                aniCalc = ani + (luni + Convert.ToInt32(zile / 30)) / 12;

                ani = 0; luni = 0; zile = 0;
                aniInv = 0; luniInv = 0; zileInv = 0;

                gfx.DrawString(count.ToString(), fontList, XBrushes.Black,
                                new XRect(40, currentHeight, page.Width, page.Height),
                                XStringFormats.TopLeft);

                gfx.DrawString(person.NumeIntreg, fontList, XBrushes.Black,
                             new XRect(70, currentHeight, page.Width, page.Height),
                              XStringFormats.TopLeft);

                gfx.DrawString(person.CNP, fontList, XBrushes.Black,
                            new XRect(200, currentHeight, page.Width, page.Height),
                             XStringFormats.TopLeft);

                gfx.DrawString(aniInvCalc + " ani, " + luniInvCalc + " luni, " + zileInvCalc + " zile. "
                                , fontList, XBrushes.Black,
                                new XRect(310, currentHeight, page.Width, page.Height),
                                XStringFormats.TopLeft);

                gfx.DrawString(aniCalc + " ani, " + luniCalc + " luni, " + zileCalc + " zile. "
                                , fontList, XBrushes.Black,
                                new XRect(460, currentHeight, page.Width, page.Height),
                                XStringFormats.TopLeft);
            }

            string filename = "General_" + RandomString(5) + ".pdf";

            string[] files = Directory.GetFiles(Application.StartupPath, "General_*.pdf");

            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                }
            }

            document.Save(filename);

            var pdfProcess = Process.Start(filename);
        }

        // Generez pdf pentru adeverinta
        private void adeverintaVechimeGenerateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Document document = new Document();

            // Add school info

            Section section = document.AddSection();

            section.PageSetup.TopMargin = 20;

            Paragraph paragraphSchoolInfo = section.AddParagraph();

            paragraphSchoolInfo.Format.Font.Size = 9;
            paragraphSchoolInfo.Format.Alignment = ParagraphAlignment.Left;
            paragraphSchoolInfo.AddText("S.C.                    .\n" +
                                        "Str.                            ......................................nr    ..loc      ..jud      \n" +
                                        "Tel:.............................; Fax:           .\n" +
                                        "CUI               ..\n\n" +
                                        "Nr.de inregistrare      . / data          ");

            //Add title

            Paragraph paragraphTitle = section.AddParagraph();

            // paragraphTitle.Format.LeftIndent = 30;

            paragraphTitle.Format.Font.Size = 15;
            paragraphTitle.Format.Alignment = ParagraphAlignment.Center;
            paragraphTitle.AddText("\nAdeverinta\n");

            Paragraph paragraphContent1 = section.AddParagraph();
            paragraphContent1.Format.Font.Size = 9;
            paragraphContent1.Format.Font.Italic = true;
            paragraphContent1.AddFormattedText("          " + " Prin prezenta se atesta faptul ca dl./dna               ., domiciliat(a) in        .." +
                                      ",str.          .., nr.    ., bl.    ., sc   ., ap    , sect    , jud    ." +
                                      ", posesor al BI/CI   .., seria    ., nr.      .., CNP             ." +
                                      ", a fost angajatul (a) societatii               ., CUI          " +
                                      ", cu sediul social in         .." +
                                      ", in baza contractului individual de munca cu norma intreaga / cu timp partial de      . .ore / zi" +
                                      ", incheiat pe durata determinata / nedeterminata" +
                                      ", inregistrat la Inspectoratul Teritorial de Munca        cu nr.    ../    ." +
                                      ", in functia / meseria de               .. . \n" +
                                      "          " + " Pe durata executarii contractului individual de munca au intervenit urmatoarele mutatii " +
                                      "( incheierea, modificarea, suspendarea si incetarea contractului individual de munca ): \n\n");

            table = section.AddTable();

            // table.Borders.Color = BackColor;

            var form = section.AddTextFrame();

            Column column = table.AddColumn("1cm");
            column.Borders.Color = Colors.Black;
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn("3cm");
            column.Borders.Color = Colors.Black;
            column.Format.Alignment = ParagraphAlignment.Right;
            column = table.AddColumn("1.5cm");
            column.Borders.Color = Colors.Black;
            column.Format.Alignment = ParagraphAlignment.Right;
            column = table.AddColumn("3cm");
            column.Borders.Color = Colors.Black;
            column.Format.Alignment = ParagraphAlignment.Right;
            column = table.AddColumn("4cm");
            column.Borders.Color = Colors.Black;
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn("3cm");
            column.Borders.Color = Colors.Black;
            column.Format.Alignment = ParagraphAlignment.Right;

            Row row = table.AddRow();

            Row row2 = table.AddRow();
            row2.Cells[2].AddParagraph("Luna");
            row2 = table.AddRow();
            row2.Cells[2].AddParagraph("Ziua");

            row.Cells[0].AddParagraph("Nr. crt");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            // row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[0].MergeDown = 2;

            row.Cells[1].AddParagraph("Mutatia interventia");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].MergeDown = 2;

            row.Cells[2].AddParagraph("Anul");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[3].AddParagraph("Meseria/Functia");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[3].MergeDown = 2;

            row.Cells[4].AddParagraph("Salariul de baza, inclusiv sporurile care intra in calculul punctajului mediu anual");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[4].MergeDown = 2;

            row.Cells[5].AddParagraph("Nr.si data actului pe baza caruia se face inscrierea si temeiul legal");
            row.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[5].MergeDown = 2;

            for (int i = 0; i < 5; i++)
            {
                row = table.AddRow();
                row2 = table.AddRow();
                row2 = table.AddRow();

                for (int j = 0; j < 6; j++)
                {
                    row.Cells[j].MergeDown = 2;
                }
                row.Cells[2].MergeDown = 0;
            }

            Paragraph paragraphContent2 = section.AddParagraph();
            paragraphContent2.Format.Font.Size = 9;
            paragraphContent2.Format.Font.Italic = true;
            paragraphContent2.AddFormattedText("          " + "Incepind cu data de              .. " +
                ", contractul individual de munca al domnului (ei) a incetat la data de             in baza prevederilor art.    .." +
                ", alin.    , lit.     din Legea nr. 53/2003   Codul Muncii, modificata si completata.\n" +
               "          " + "In perioada lucrata a avut     . zile de absente nemotivate si      zile concediu fara plata.\n" +
               "          " + "In perioada de la      .pana la      .. a lucrat in grupa(I sau II de munca)" +
               ", pozitia nr.        din anexa la Ordinul nr.     ..din     . al ministrului       .." +
               ", in total      ani    . luni      zile(                    ). \n\n" +
                "                   " + "Reprezentant legal,                             Intocmit,");

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false, PdfFontEmbedding.Always);

            pdfRenderer.Document = document;

            pdfRenderer.RenderDocument();

            string filename = "Adeverinta"+RandomString(4)+".pdf";

            pdfRenderer.PdfDocument.Save(filename);

            Process.Start(filename);
        }

        // Functia care returneaza un sir random de caractere
        private static Random random = new Random();

        private Table table;

        public object BitmapFrame { get; private set; }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion GeneratePdf
    }
}
