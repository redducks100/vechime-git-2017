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
    public partial class Interval : Form
    {
        public Interval()
        {
            InitializeComponent();
        }


        public DateTime dataInceput { get; set; }
        public DateTime dataSfarsit { get; set; }
        public bool algoritm { get; set; }

        private void buttonContinuati_Click(object sender, EventArgs e)
        {
            dataInceput = inceputTimePicker.Value;
            dataSfarsit = sfarsitTimePicker.Value;

            DialogResult = DialogResult.OK;

            Close();

        }
    }
}
