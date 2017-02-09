using Itenso.TimePeriod;
using Microsoft.Win32;
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
        public UnitateInfo currentUnitate;

        public bool DemoVersion = false;

        public MainForm(bool _demoVersion = false)
        {
            InitializeComponent();
            connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + databasePath + @"; Persist Security Info=False;";
            DemoVersion = _demoVersion;
            peopleDictionary = GetPeople();
            displayPeople = peopleDictionary.Values.ToList();
            UpdatePeopleInfo();
            RunOnStartup();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            CheckUnitateInfo();
            CheckTransaUpdates();
        }

        private void RunOnStartup()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            //if (rkApp.GetValue("Vechime") == null)
            //{
            //    rkApp.SetValue("Vechime", Application.ExecutablePath);
            //}

            //if (rkApp.GetValue("Vechime") != null)
            //{
            //    rkApp.DeleteValue("Vechime");
            //}
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        #region UpdateStuff

        private void UpdatePeople()
        {
            peopleDictionary = GetPeople();
            displayPeople = peopleDictionary.Values.ToList();
            UpdateList();
            GetAllPerioadaList();
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

        //verifica la pornirea aplicatiei daca exista unitate adaugata
        private void CheckUnitateInfo()
        {
            currentUnitate = GetCurrentUnitateInfo();

            if (currentUnitate == null)
            {
                MessageBox.Show("Nu exista informatii despre unitatea curenta! Completati informatiile in urmatoarele casute!", "Atentie!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                using (UnitateForm unitateForm = new UnitateForm(this, null))
                {
                    unitateForm.ShowDialog();
                }
            }

            CheckLucreazaUnitateaCurenta();
        }

        private void CheckLucreazaUnitateaCurenta()
        {
            foreach (Person person in peopleDictionary.Values)
            {
                foreach (Perioada perioada in person.Perioade)
                {
                    perioada.LucreazaUnitateaCurenta = (currentUnitate.SC.ToLower() == perioada.LocMunca.ToLower());
                }
            }
        }

        //verifica la pornirea aplicatiei, modificare/adaugarea/stergerea unei perioade
        private void CheckTransaUpdates()
        {
            foreach (Person person in peopleDictionary.Values)
            {
                if (person.Perioade.Where(x => x.LucreazaUnitateaCurenta == true).Count() > 0)
                {
                    Transa currentTransaInv = person.CurrentTransaInv;
                    Transa currentTransaMunca = person.CurrentTransaMunca;

                    if (currentTransaInv.TransaString != person.PreviousTransaInv || currentTransaMunca.TransaString != person.PreviousTransaMunca)
                    {
                        if (!string.IsNullOrWhiteSpace(person.PreviousTransaInv) && !string.IsNullOrWhiteSpace(person.PreviousTransaMunca))
                        {
                            MessageBox.Show(string.Format("{0} a trecut la o transa superioara!", person.NumeIntreg), "Atentie!");
                        }
                        person.PreviousTransaInv = currentTransaInv.TransaString;
                        person.PreviousTransaMunca = currentTransaMunca.TransaString;
                        ModifyPerson(person);
                    }
                }
            }
        }

        #endregion UpdateStuff

        #region SQL-Methods

        //unitate
        public UnitateInfo GetCurrentUnitateInfo()
        {
            UnitateInfo newUnitate = null;
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand("SELECT * FROM Unitate", connection))
                {
                    connection.Open();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                newUnitate = new UnitateInfo();
                                newUnitate.ID = Convert.ToInt32(reader[0]);
                                newUnitate.SC = Convert.ToString(reader[1]);
                                newUnitate.Strada = Convert.ToString(reader[2]);
                                newUnitate.Numar = Convert.ToString(reader[3]);
                                newUnitate.Localitate = Convert.ToString(reader[4]);
                                newUnitate.Judet = Convert.ToString(reader[5]);
                                newUnitate.Telefon = Convert.ToString(reader[6]);
                                newUnitate.Fax = Convert.ToString(reader[7]);
                                newUnitate.CUI = Convert.ToString(reader[8]);
                            }
                        }
                    }
                }
            }

            return newUnitate;
        }

        public void ModifyCurrentUnitate(UnitateInfo newUnitate)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand(@"UPDATE Unitate SET SC = @SC ,Strada = @Strada,Numar = @Numar,Localitate = @Localitate,Judet = @Judet,
                                                                Telefon=@Telefon,Fax=@Fax,CUI=@CUI
                                                                WHERE ID = @ID", connection))
                {
                    command.Parameters.Add("@SC", OleDbType.VarChar).Value = newUnitate.SC;
                    command.Parameters.Add("@Strada", OleDbType.VarChar).Value = newUnitate.Strada;
                    command.Parameters.Add("@Numar", OleDbType.VarChar).Value = newUnitate.Numar;
                    command.Parameters.Add("@Localitate", OleDbType.VarChar).Value = newUnitate.Localitate;
                    command.Parameters.Add("@Judet", OleDbType.VarChar).Value = newUnitate.Judet;
                    command.Parameters.Add("@Telefon", OleDbType.VarChar).Value = newUnitate.Telefon;
                    command.Parameters.Add("@Fax", OleDbType.VarChar).Value = newUnitate.Fax;
                    command.Parameters.Add("@CUI", OleDbType.VarChar).Value = newUnitate.CUI;
                    command.Parameters.Add("@ID", OleDbType.Integer).Value = newUnitate.ID;

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

            currentUnitate = newUnitate;
        }

        public void AddCurrentUnitate(UnitateInfo newUnitate)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand(@"INSERT INTO Unitate(SC,Strada,Numar,Localitate,Judet,
                                                                Telefon,Fax,CUI) VALUES (@SC,@Strada,@Numar,@Localitate,@Judet,
                                                                @Telefon,@Fax,@CUI)", connection))
                {
                    command.Parameters.Add("@SC", OleDbType.VarChar).Value = newUnitate.SC;
                    command.Parameters.Add("@Strada", OleDbType.VarChar).Value = newUnitate.Strada;
                    command.Parameters.Add("@Numar", OleDbType.VarChar).Value = newUnitate.Numar;
                    command.Parameters.Add("@Localitate", OleDbType.VarChar).Value = newUnitate.Localitate;
                    command.Parameters.Add("@Judet", OleDbType.VarChar).Value = newUnitate.Judet;
                    command.Parameters.Add("@Telefon", OleDbType.VarChar).Value = newUnitate.Telefon;
                    command.Parameters.Add("@Fax", OleDbType.VarChar).Value = newUnitate.Fax;
                    command.Parameters.Add("@CUI", OleDbType.VarChar).Value = newUnitate.CUI;

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

            currentUnitate = GetCurrentUnitateInfo();
        }

        //person
        public void AddPerson(Person newPerson)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand("INSERT INTO Persoane (Nume,Prenume,CNP,Serie,TransaInv,TransaMunca) VALUES (@nume,@prenume,@cnp,@serie,@TransaInv,@TransaMunca)", connection))
                {
                    command.Parameters.Add("@nume", OleDbType.VarChar, 50).Value = newPerson.Nume;
                    command.Parameters.Add("@prenume", OleDbType.VarChar, 50).Value = newPerson.Prenume;
                    command.Parameters.Add("@cnp", OleDbType.VarChar, 13).Value = newPerson.CNP;
                    command.Parameters.Add("@serie", OleDbType.VarChar, 6).Value = newPerson.Serie;
                    //Nu adauga transa pt o persoana noua pt ca nu are perioade!
                    command.Parameters.Add("@TransaInv", OleDbType.VarChar).Value = "";//newPerson.CurrentTransaInv.TransaString;
                    command.Parameters.Add("@TransaMunca", OleDbType.VarChar).Value = "";//newPerson.CurrentTransaMunca.TransaString;
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
                using (OleDbCommand command = new OleDbCommand("UPDATE Persoane SET Nume = @nume ,Prenume = @prenume,CNP = @cnp, Serie = @serie, TransaInv=@TransaInv, TransaMunca=@TransaMunca WHERE Id = @id", connection))
                {
                    command.Parameters.Add("@nume", OleDbType.VarChar, 50).Value = newPerson.Nume;
                    command.Parameters.Add("@prenume", OleDbType.VarChar, 50).Value = newPerson.Prenume;
                    command.Parameters.Add("@cnp", OleDbType.VarChar, 13).Value = newPerson.CNP;
                    command.Parameters.Add("@serie", OleDbType.VarChar, 6).Value = newPerson.Serie;
                    command.Parameters.Add("@TransaInv", OleDbType.VarChar).Value = newPerson.CurrentTransaInv.TransaString;
                    command.Parameters.Add("@TransaMunca", OleDbType.VarChar).Value = newPerson.CurrentTransaMunca.TransaString;
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
                                newPerson.PreviousTransaInv = Convert.ToString(reader[5]);
                                newPerson.PreviousTransaMunca = Convert.ToString(reader[6]);
                                peopleDict.Add(newPerson.ID, newPerson);
                            }
                        }
                    }
                }
            }

            return peopleDict;
        } //done

        //perioada
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
                CheckTransaUpdates();
            }
        } //done

        public void ModifyPerioada(Perioada currentPerioada, int personID)
        {
            int recordsChanged = 0;
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                string commandString = "UPDATE Perioade SET Id_Persoana=@Id_Persoana,Data_Inceput=@Data_Inceput,Data_Sfarsit=@Data_Sfarsit,CFS=@CFS," +
                                                             "TipCFS=@TipCFS,Norma=@Norma,Functie=@Functie,InvORMunca=@InvORMunca,Loc_Munca=@Loc_Munca,Lucreaza=@Lucreaza,Somaj=@Somaj WHERE Id=@Id";
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
                CheckTransaUpdates();
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

            if (peopleDictionary.ContainsKey(personID) && recordsChanged != 0)
            {
                peopleDictionary[personID].Perioade = GetPerioadaList(personID);
                CheckTransaUpdates();
            }
            peopleListBox.SelectedIndex = -1;
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

                                if (!reader.IsDBNull(reader.GetOrdinal("TipCFS")))
                                    newPerioada.TipCFS = Convert.ToString(reader[5]);
                                else
                                    newPerioada.TipCFS = "";

                                newPerioada.Norma = Convert.ToString(reader[6]);
                                newPerioada.Functie = Convert.ToString(reader[7]);
                                newPerioada.IOM = Convert.ToString(reader[8]);
                                newPerioada.LocMunca = Convert.ToString(reader[9]);
                                newPerioada.Lucreaza = Convert.ToBoolean(reader[10]);
                                newPerioada.Somaj = Convert.ToBoolean(reader[11]);

                                if (peopleDictionary.ContainsKey(ID_Person))
                                {
                                    peopleDictionary[ID_Person].Perioade.Add(newPerioada);
                                    peopleDictionary[ID_Person].Perioade[peopleDictionary[ID_Person].Perioade.Count - 1].ListNumber = peopleDictionary[ID_Person].Perioade.Count - 1;
                                }
                            }
                        }
                    }
                }
            }
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

                                if (!reader.IsDBNull(reader.GetOrdinal("TipCFS")))
                                    newPerioada.TipCFS = Convert.ToString(reader[5]);
                                else
                                    newPerioada.TipCFS = "";

                                newPerioada.Norma = Convert.ToString(reader[6]);
                                newPerioada.Functie = Convert.ToString(reader[7]);
                                newPerioada.IOM = Convert.ToString(reader[8]);
                                newPerioada.LocMunca = Convert.ToString(reader[9]);
                                newPerioada.Lucreaza = Convert.ToBoolean(reader[10]);
                                newPerioada.Somaj = Convert.ToBoolean(reader[11]);
                                perioadaList.Add(newPerioada);
                                perioadaList[perioadaList.Count - 1].ListNumber = perioadaList.Count - 1;
                            }
                        }
                    }
                }
            }

            return perioadaList;
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

                foreach (Perioada perioada in selectedPerson.Perioade.OrderBy(c => c.DTSfarsit))
                {

                    TimePeriod periodCalc = TimePeriod.CalculatePeriodTime(perioada);

                    DataGridViewRow newRow = new DataGridViewRow();
                    newRow.CreateCells(dataGridView1);
                    newRow.Cells[0].Value = perioada.ID;
                    newRow.Cells[1].Value = perioada.ListNumber;
                    newRow.Cells[2].Value = perioada.DTInceput.ToShortDateString();
                    newRow.Cells[3].Value = perioada.DTSfarsit.ToShortDateString();
                    newRow.Cells[4].Value = periodCalc.Years + "-" + periodCalc.Months + "-" + periodCalc.Days;
                    newRow.Cells[5].Value = (string.IsNullOrWhiteSpace(perioada.TipCFS)==true?"Nu":perioada.TipCFS.ToUpper());

                    // Am specificat daca se aplica norma de 1/1
                    DateTime changeNorma = new DateTime(2006, 09, 18);
                    string normaSpecification = "";
                    if (perioada.Norma.ToUpper() != "1/1" && perioada.DTSfarsit.CompareTo(changeNorma) > 0)
                    {
                        normaSpecification = " (1/1)";
                        newRow.Cells[6].ToolTipText = "De la data de  18/09/2006 se aplica norma 1/1.";
                    }
                    newRow.Cells[6].Value = perioada.Norma.ToUpper() + normaSpecification;

                    newRow.Cells[7].Value = perioada.Functie.ToUpper();
                    newRow.Cells[8].Value = perioada.IOM.ToUpper();
                    newRow.Cells[9].Value = perioada.LocMunca.ToUpper();
                    newRow.Cells[10].Value = perioada.Lucreaza;
                    dataGridView1.Rows.Add(newRow);
                }

                perioadaInvTB.Text = selectedPerson.PerioadaInv.Years.ToString() + " ani " + selectedPerson.PerioadaInv.Months.ToString() + " luni " + selectedPerson.PerioadaInv.Days.ToString() + " zile";
                perioadaTotalTB.Text = selectedPerson.PerioadaMunca.Years.ToString() + " ani " + selectedPerson.PerioadaMunca.Months.ToString() + " luni " + selectedPerson.PerioadaMunca.Days.ToString() + " zile";

                transaInvatamantTextBox.Text = selectedPerson.CurrentTransaInv.TransaString;
                transaMuncaTextBox.Text = selectedPerson.CurrentTransaMunca.TransaString;
            }
            else
            {
                dataGridView1.Rows.Clear();
                perioadaInvTB.Text = "";
                perioadaTotalTB.Text = "";
                transaInvatamantTextBox.Text = "";
                transaMuncaTextBox.Text = "";
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
            else
            {
                MessageBox.Show("Selectati o persoana!", "Atentie!");
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
                    CheckTransaUpdates();
                }
            }
            else
            {
                MessageBox.Show("Selectati o perioada!", "Atentie!");
            }
        }

        private void unitateaCurentaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (UnitateForm unitateForm = new UnitateForm(this, currentUnitate))
            {
                unitateForm.ShowDialog();
            }
        }

        #endregion DetailHandlers

        #region AddHandlers

        private void adaugaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(DemoVersion == true && peopleDictionary.Values.Count >= 3)
            {
                MessageBox.Show("Pentru a adauga mai multe persoane, va rugam sa cumparati versiunea full!","Atentie!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

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
                    CheckTransaUpdates();
                }
            }
            else
            {
                MessageBox.Show("Selectati o perioada!", "Atentie!");
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
            else
            {
                MessageBox.Show("Selectati o persoana!","Atentie!");
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
            else
            {
                MessageBox.Show("Selectati o perioada!", "Atentie!");
            }
        }

        #endregion DeleteHandlers

        #region UpdateHandlers

        private void modificareToolStripMenuItem_Click(object sender, EventArgs e)
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
            else
            {
                MessageBox.Show("Selectati o perioada!", "Atentie!");
            }
        }

        private void actualizareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Person selectedPerson = null;
            if (peopleListBox.SelectedIndex >= 0 && peopleDictionary.ContainsKey((peopleListBox.SelectedItem as Person).ID))
                selectedPerson = peopleDictionary[(peopleListBox.SelectedItem as Person).ID];
            if (selectedPerson != null)
            {
                if (MessageBox.Show("Sigur doriti sa actualizati perioadele?", "Atentie", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int index = peopleListBox.SelectedIndex;
                    for (int i = 0; i < selectedPerson.Perioade.Count; i++)
                    {
                        if (selectedPerson.Perioade[i].Lucreaza == true)
                        {
                            selectedPerson.Perioade[i].DTSfarsit = DateTime.Today;
                            ModifyPerioada(selectedPerson.Perioade[i], selectedPerson.ID);
                        }
                    }
                    peopleListBox.SelectedIndex = 0;
                    peopleListBox.SelectedIndex = index;
                }
            }
            else
            {
                MessageBox.Show("Selectati o persoana!", "Atentie!");
            }
        }

        #endregion UpdateHandlers

        #endregion Menu Strip Handlers

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

                gfx.DrawString("Nr.crt.  Data inceput  Data sfarsit  Norma  Invatamant/Munca  Ani  Luni  Zile  Locul de munca"
                              , fontTableHead, XBrushes.Black,
                              new XRect(25, 128, page.Width, page.Height),
                              XStringFormats.TopLeft);

                XFont fontList = new XFont("Consolas", 10);

                // A doua linie
                gfx.DrawLine(new XPen(XColor.FromName("black")), new System.Windows.Point(20, 148), new System.Windows.Point(wd - 20, 148));

                int count = 0;

                // Pastreaza inaltimea la care am ajuns in pagina
                int currentHeight = 148;

                foreach (Perioada perioada in selectedPerson.Perioade)
                {
                    count++;

                    currentHeight += 13;

                    if (currentHeight + 50 > page.Height)
                    {
                        page = document.AddPage();

                        gfx = XGraphics.FromPdfPage(page);

                        currentHeight = 30;
                    }

                    DateDiff diff = new DateDiff(perioada.DTInceput, perioada.DTSfarsit);

                    TimePeriod periodCalc = TimePeriod.CalculatePeriodTime(perioada);

                    string rowString = string.Empty.PadLeft(100);

                    rowString = rowString.Insert(3, count.ToString());

                    rowString = rowString.Insert(8, perioada.DTInceput.ToShortDateString());
                    rowString = rowString.Insert(21, perioada.DTSfarsit.ToShortDateString());
                    rowString = rowString.Insert(33, perioada.Norma.ToUpper());
                    rowString = rowString.Insert(41, perioada.IOM.ToUpper());
                    rowString = rowString.Insert(58, periodCalc.Years.ToString());
                    rowString = rowString.Insert(63, periodCalc.Months.ToString());
                    rowString = rowString.Insert(68, periodCalc.Days.ToString());

                    if (perioada.LocMunca.ToUpper() == "CONCEDIU")
                        rowString = rowString.Insert(76, perioada.LocMunca.ToUpper() + " " + perioada.TipCFS);
                    else
                        rowString = rowString.Insert(76, perioada.LocMunca.ToUpper());

                    gfx.DrawString(rowString, fontList, XBrushes.Black,
                                   new XRect(25, currentHeight, page.Width, page.Height),
                                   XStringFormats.TopLeft);
                }

                // Adaug spatiu dupa enumerarea perioadelor
                currentHeight += 40;

                if (currentHeight + 40 > page.Height)
                {
                    page = document.AddPage();

                    gfx = XGraphics.FromPdfPage(page);

                    currentHeight = 30;
                }

                // Clalculez timp

                TimePeriodSum periodsSum = TimePeriodSum.CalculateIndividualTime(selectedPerson.Perioade);

                // Adaug timp invatamant
                gfx.DrawString("Vechime in invatamant: " + periodsSum.YearsInv + " ani, " + periodsSum.MonthsInv + " luni, " + periodsSum.DaysInv + " zile.   Transa "+selectedPerson.CurrentTransaInv.TransaString
                                , fontList, XBrushes.Black,
                                new XRect(0, currentHeight, page.Width, page.Height),
                                XStringFormats.TopCenter);

                currentHeight += 15;

                // Adaug timp total
                gfx.DrawString("Vechime total: " + periodsSum.Years + " ani, " + periodsSum.Months + " luni, " + periodsSum.Days + " zile.  Transa "+selectedPerson.CurrentTransaMunca.TransaString
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

            int currentHeight = 100, count = 0;

            foreach (Person person in peopleDictionary.Values)
            {
                count++;
                currentHeight += 15;

                if (currentHeight + 50 > page.Height)
                {
                    page = document.AddPage();

                    gfx = XGraphics.FromPdfPage(page);
                    currentHeight = 30;
                }

                TimePeriodSum periodsSum = TimePeriodSum.CalculateIndividualTime(person.Perioade);

                gfx.DrawString(count.ToString(), fontList, XBrushes.Black,
                                new XRect(40, currentHeight, page.Width, page.Height),
                                XStringFormats.TopLeft);

                gfx.DrawString(person.NumeIntreg, fontList, XBrushes.Black,
                             new XRect(70, currentHeight, page.Width, page.Height),
                              XStringFormats.TopLeft);

                gfx.DrawString(person.CNP, fontList, XBrushes.Black,
                            new XRect(200, currentHeight, page.Width, page.Height),
                             XStringFormats.TopLeft);

                gfx.DrawString(periodsSum.YearsInv + " ani, " + periodsSum.MonthsInv + " luni, " + periodsSum.DaysInv + " zile. "
                                , fontList, XBrushes.Black,
                                new XRect(310, currentHeight, page.Width, page.Height),
                                XStringFormats.TopLeft);

                gfx.DrawString(periodsSum.Years + " ani, " + periodsSum.Months + " luni, " + periodsSum.Days + " zile. "
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
            paragraphSchoolInfo.AddText("S.C. " + currentUnitate.SC + "\n" +
                                        "Str. " + currentUnitate.Strada + " , nr " + currentUnitate.Numar + ", loc " + currentUnitate.Localitate + ", jud " + currentUnitate.Judet + "\n" +
                                        "Tel: " + currentUnitate.Telefon + ",  Fax: " + currentUnitate.Fax + ", \n" +
                                        "CUI: " + currentUnitate.CUI + ", \n\n" +
                                        "Nr.de inregistrare: "  + "\n Data " + DateTime.Now.ToString("dd/MM/yyyy"));

            //Add title

            Paragraph paragraphTitle = section.AddParagraph();

            // paragraphTitle.Format.LeftIndent = 30;

            paragraphTitle.Format.Font.Size = 15;
            paragraphTitle.Format.Alignment = ParagraphAlignment.Center;
            paragraphTitle.AddText("\nAdeverinta\n\n");

            if (peopleListBox.SelectedIndex < 0)
                return;

            Person selectedPerson = null;
            if (peopleDictionary.ContainsKey((peopleListBox.SelectedItem as Person).ID))
                selectedPerson = peopleDictionary[(peopleListBox.SelectedItem as Person).ID];
            else
            {
                MessageBox.Show("Person doesn't exist!", "Error!");
                return;
            }

            if (selectedPerson.Perioade.Where(x => x.LocMunca.ToUpper() == currentUnitate.SC.ToUpper()).Count() <= 0)
            {
                MessageBox.Show(string.Format("{0} nu lucreaza la unitatea curenta!", selectedPerson.NumeIntreg), "Atentie!");
                return;
            }


            string ultimaFunctie = "";
            

            foreach (Perioada perioada in selectedPerson.Perioade.OrderBy(c => c.DTSfarsit).ToList())
            {
                if (perioada.LocMunca.ToUpper() == currentUnitate.SC.ToUpper())
                {
                    
                    ultimaFunctie = perioada.Functie;
                }
            }

            Paragraph paragraphContent1 = section.AddParagraph();
            paragraphContent1.Format.Font.Name = "Verdana";
            paragraphContent1.Format.Font.Size = 9;
            paragraphContent1.Format.Font.Italic = true;
            paragraphContent1.AddFormattedText("       " + " Prin prezenta se atesta faptul ca dl./dna " + selectedPerson.NumeIntreg +
                                      ", posesor al BI/CI, seria " + selectedPerson.Serie.Substring(0, 2) + ", nr " + selectedPerson.Serie.Substring(2) + ", CNP " + selectedPerson.CNP +
                                      ", a fost angajat al unitatii               ." +
                                      ", in functia de " + ultimaFunctie + " \n" +
                                      "         " + " Pe durata executarii contractului individual de munca au intervenit urmatoarele mutatii " +
                                      "( incheierea, modificarea, suspendarea si incetarea contractului individual de munca ): \n\n");

            table = section.AddTable();

            // table.Borders.Color = BackColor;

            var form = section.AddTextFrame();

            Column column = table.AddColumn("1.5cm");
            column.Borders.Color = Colors.Black;
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("4cm");
            column.Borders.Color = Colors.Black;
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("3cm");
            column.Borders.Color = Colors.Black;
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("3cm");
            column.Borders.Color = Colors.Black;
            column.Format.Alignment = ParagraphAlignment.Right;

           
            Row row = table.AddRow();

            row.Cells[0].AddParagraph("Nr. crt");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
           
            row.Cells[1].AddParagraph("Mutatia / interventia");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph("Data");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            
            row.Cells[3].AddParagraph("Functia");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

            int count = 1;
            DateTime ultimaData = new DateTime();
            bool working = false;
            foreach (Perioada perioada in selectedPerson.Perioade.OrderBy(c => c.DTSfarsit).ToList())
            {
               
                if (perioada.LocMunca.ToUpper() == currentUnitate.SC.ToUpper() || (working && perioada.TipCFS !=""))
                {
                    working = true;

                    row = table.AddRow();

                    for (int i = 0; i < 4; i++)
                        row.Cells[i].Format.Font.Size = 9;

                    row.Cells[0].AddParagraph(count.ToString());
                    row.Cells[0].Format.Font.Bold = false;
                    row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                    // row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
                    count++;

                    if(perioada.TipCFS !="")
                        row.Cells[1].AddParagraph("Suspendare");
                    else
                    row.Cells[1].AddParagraph("Incadrat");


                    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                    row.Cells[2].AddParagraph(perioada.DTInceput.ToString("dd/MM/yyyy"));
                    row.Cells[2].Format.Alignment = ParagraphAlignment.Left;


                    row.Cells[3].AddParagraph(perioada.CFS == true ? perioada.Functie + " " + perioada.TipCFS : perioada.Functie);
                    row.Cells[3].Format.Alignment = ParagraphAlignment.Left;



                    row = table.AddRow();

                    for (int i = 0; i < 4; i++)
                        row.Cells[i].Format.Font.Size = 9;

                    row.Cells[0].AddParagraph(count.ToString());
                    row.Cells[0].Format.Font.Bold = false;
                    row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                    
                    count++;

                    if (perioada.TipCFS != "")
                        row.Cells[1].AddParagraph("Incetat suspendare");
                    else
                        row.Cells[1].AddParagraph("Incetat contract de munca");


                    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                    row.Cells[2].AddParagraph(perioada.DTSfarsit.ToString("dd/MM/yyyy"));
                    row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                    

                    row.Cells[3].AddParagraph(perioada.CFS==true?perioada.Functie + " " + perioada.TipCFS:perioada.Functie);
                    row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                    ultimaData = perioada.DTSfarsit;
                }
            }

            Paragraph paragraphContent2 = section.AddParagraph();
            paragraphContent2.Format.Font.Size = 9;
            paragraphContent2.Format.Font.Italic = true;
            paragraphContent2.AddFormattedText("" + "Incepind cu data de  " + ultimaData.ToString("dd/MM/yyyy") +
                ", contractul individual de munca al domnului (ei) a incetat.\n\n\n" +
          "                        " + "Reprezentant legal," + "                          " + "Intocmit,");

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false, PdfFontEmbedding.Always);

            pdfRenderer.Document = document;

            pdfRenderer.RenderDocument();

            string filename = "Adeverinta_" + RandomString(4) + ".pdf";

            string[] files = Directory.GetFiles(Application.StartupPath, "Adeverinta_*.pdf");


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
