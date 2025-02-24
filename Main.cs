using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoSimRT
{
    public partial class Main : Form
    {
        private List<string> _checkedPatients;

        public List<string> CheckedPatients
        {
            get { return _checkedPatients; }
            set { _checkedPatients = value; }
        }
        public Main()
        {
            InitializeComponent();
            InitializeGUI();
        }
        private void InitializeGUI()
        {
            clbCTpatients.Items.Clear();
            DateTime startDate = DateTime.Today;
            DateTime endDate = startDate.AddDays(1);
            var db = AriaInterface.GetScheduledSimPatientsOnCT(startDate.ToString(), endDate.ToString());
            foreach (DataRow dr in db.Rows)
            {
                string patId = dr.ItemArray[0].ToString();
                bool alreadyScanned = AriaInterface.CheckAlreadyScannedPatient(patId, startDate.ToString());
                clbCTpatients.Items.Add(patId);
                if (!alreadyScanned)
                    clbCTpatients.SetItemChecked(clbCTpatients.Items.Count - 1, true);
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            this._checkedPatients = clbCTpatients.CheckedItems.Cast<string>().ToList();
        }

        private void btnAddPatient_Click(object sender, EventArgs e)
        {
            bool ok = AriaInterface.CheckPatientExist(txtPatientID.Text);
            bool alreadyAdded = CheckIfIdExists(txtPatientID.Text);
            if (ok)
            {
                if (alreadyAdded)
                {
                    clbCTpatients.Items.Add(txtPatientID.Text);
                    clbCTpatients.SetItemChecked(clbCTpatients.Items.Count - 1, true);
                }
                else
                    MessageBox.Show(txtPatientID.Text + " finns redan i listan");
            }
            else
            {
                MessageBox.Show("Hittade ingen patient med ID: " + txtPatientID.Text);
            }
        }
        private bool CheckIfIdExists(string patId)
        {
            bool ok = !clbCTpatients.Items.Cast<string>().Any(item => item.Equals(patId));

            return ok;
        }
    }
}
