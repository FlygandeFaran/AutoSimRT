using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace AutoSimRT
{
    public static class AriaInterface
    {
        private static SqlConnection connection = null;



        public static void Connect()
        {
            //Fill out with approriate information about your Varian DB and username + password
            //connection = new SqlConnection("data source = SLTVARDB3; initial catalog = VARIAN; persist security info = True; user id =  ; password = ; Connection Timeout=5; MultipleActiveResultSets = True");

            //Following rows can be erased when previous info is filled out
            string filename = @"\\ltvastmanland.se\ltv\shares\vradiofy\RADIOFYSIK NYSTART\Ö_Erik\02 Programmering\C# scripting\aria_account_information.txt";
            string connect = File.ReadAllText(filename);
            connection = new SqlConnection(connect);

            connection.Open();
        }

        public static void Disconnect()
        {
            connection.Close();
        }

        public static DataTable Query(string queryString)
        {
            DataTable dataTable = new DataTable();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(queryString, connection) { MissingMappingAction = MissingMappingAction.Passthrough, MissingSchemaAction = MissingSchemaAction.Add };
                adapter.Fill(dataTable);
                adapter.Dispose();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dataTable;
        }
        public static DataTable GetScheduledSimPatientsOnCT(string startDate, string endDate)
        {
            Connect();
            DataTable datatable = AriaInterface.Query(@"SELECT DISTINCT
	                                                        Patient.PatientId
                                                        FROM
                                                            ScheduledActivity WITH (NOLOCK),
                                                            Activity WITH (NOLOCK),
                                                            ActivityInstance WITH (NOLOCK),
                                                            Patient WITH (NOLOCK),
                                                            ResourceActivity WITH (NOLOCK),
                                                            Machine WITH (NOLOCK)
                                                        WHERE
	                                                        (Patient.PatientId LIKE '%19%' OR
	                                                        Patient.PatientId LIKE '%20%') AND
	                                                        ScheduledActivity.PatientSer = Patient.PatientSer AND
	                                                        ActivityInstance.ActivityInstanceSer = ScheduledActivity.ActivityInstanceSer AND
                                                            ScheduledActivity.ScheduledStartTime BETWEEN '" + startDate + @"' AND '" + endDate + @"' AND
	                                                        ScheduledActivity.ScheduledActivitySer = ResourceActivity.ScheduledActivitySer AND
	                                                        ResourceActivity.ResourceSer = Machine.ResourceSer AND
                                                            Machine.MachineId LIKE 'CT Canon' AND
	                                                        Activity.ActivitySer = ActivityInstance.ActivitySer AND
	                                                        (Activity.ActivityCode = 'DIB DT' OR
	                                                        Activity.ActivityCode = '4D DT')
                                                            ");
            Disconnect();
            return datatable;
        }
        public static DataTable GetPatientInfo(string patID)
        {
            Connect();
            DataTable datatable = AriaInterface.Query(@"SELECT DISTINCT
	                                                        Patient.PatientId,
	                                                        Patient.FirstName,
	                                                        Patient.LastName,
	                                                        Patient.Sex 
                                                        FROM
                                                            Patient WITH (NOLOCK)
                                                        WHERE
                                                            Patient.PatientId LIKE '" + patID + @"'
                                                            ");
            Disconnect();
            return datatable;
        }
        public static bool CheckPatientExist(string patID)
        {
            bool ok = false;
            Connect();
            DataTable datatable = AriaInterface.Query(@"SELECT 
	                                                        COUNT(Patient.PatientId)
                                                        FROM
                                                            Patient WITH (NOLOCK)
                                                        WHERE
	                                                        Patient.PatientId LIKE '" + patID + @"'
                                                            ");
            Disconnect();
            if (datatable.Rows[0].ItemArray[0].ToString() == "1")
            {
                ok = true;
            }
            return ok;
        }
        public static bool CheckAlreadyScannedPatient(string patID, string startDate)
        {
            bool ok = false;
            Connect();
            DataTable datatable = AriaInterface.Query(@"SELECT 
	                                                        COUNT(Patient.PatientId)
                                                        FROM
                                                            ScheduledActivity WITH (NOLOCK),
                                                            Activity WITH (NOLOCK),
                                                            ActivityInstance WITH (NOLOCK),
                                                            Patient WITH (NOLOCK),
                                                            ResourceActivity WITH (NOLOCK),
                                                            Machine WITH (NOLOCK)
                                                        WHERE
	                                                        Patient.PatientId LIKE '" + patID + @"' AND
	                                                        ScheduledActivity.PatientSer = Patient.PatientSer AND
	                                                        ActivityInstance.ActivityInstanceSer = ScheduledActivity.ActivityInstanceSer AND
                                                            ScheduledActivity.ScheduledStartTime BETWEEN '2024-01-01' AND '" + startDate + @"' AND
	                                                        ScheduledActivity.ScheduledActivitySer = ResourceActivity.ScheduledActivitySer AND
	                                                        ResourceActivity.ResourceSer = Machine.ResourceSer AND
                                                            Machine.MachineId LIKE 'CT Canon' AND
	                                                        Activity.ActivitySer = ActivityInstance.ActivitySer AND
	                                                        (Activity.ActivityCode = 'DIB DT' OR
	                                                        Activity.ActivityCode = '4D DT')
                                                            ");
            Disconnect();
            int count = int.Parse(datatable.Rows[0].ItemArray[0].ToString());
            if (count > 0)
            {
                ok = true;
            }
            return ok;
        }
    }
}