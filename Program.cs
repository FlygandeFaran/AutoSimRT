using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using static MouseClick.MouseOperations;

namespace AutoSimRT
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Main form1 = new Main();
            InfoOverlay infoOverlay = new InfoOverlay();
            DialogResult result = form1.ShowDialog();

            if (result == DialogResult.OK)
            {
                infoOverlay.Show();
                Run(form1.CheckedPatients);
                infoOverlay.Close();
            }
            else if (result == DialogResult.Yes)
            {
                string patientID = "199201011234";
                string birthDate = patientID.Substring(6, 2) + "/" + patientID.Substring(4, 2) + "/" + patientID.Substring(0, 4);
                PatientInfo testPatientInfo = new PatientInfo()
                {
                    PatID = patientID,
                    FirstName = "Peter",
                    LastName = "Parker",
                    Sex = "Male",
                    BirthDate = birthDate
                };
                infoOverlay.Show();
                SchedulePatient(testPatientInfo);
                infoOverlay.Close();
            }

        }
        private static void Run(List<string> CheckedPatients)
        {
            foreach (string patID in CheckedPatients)
            {
                var patInfo = AriaInterface.GetPatientInfo(patID);

                string patientID = patInfo.Rows[0].ItemArray[0].ToString();
                string birthDate = patientID.Substring(6, 2) + "/" + patientID.Substring(4, 2) + "/" + patientID.Substring(0, 4);

                PatientInfo patientInfo = new PatientInfo()
                {
                    PatID = patientID,
                    FirstName = patInfo.Rows[0].ItemArray[1].ToString(),
                    LastName = patInfo.Rows[0].ItemArray[2].ToString(),
                    Sex = patInfo.Rows[0].ItemArray[3].ToString().Trim(),
                    BirthDate = birthDate
                };
                SchedulePatient(patientInfo);
            }
        }
        private static void SchedulePatient(PatientInfo patientInfo)
        {
            //Add patient and write first name
            MousePoint mp = new MousePoint(1790, 980);
            SetCursorPosition(mp);
            Thread.Sleep(100);
            CheckIfMoved(mp);
            MouseClick();
            Thread.Sleep(100);
            SendKeys.SendWait(patientInfo.FirstName);

            //Last name
            mp = new MousePoint(880, 450);
            SetCursorPosition(mp);
            Thread.Sleep(100);
            CheckIfMoved(mp);
            MouseClick();
            Thread.Sleep(100);
            SendKeys.SendWait(patientInfo.LastName);

            //patient ID
            mp = new MousePoint(880, 520);
            SetCursorPosition(mp);
            Thread.Sleep(100);
            CheckIfMoved(mp);
            MouseClick();
            Thread.Sleep(100);
            SendKeys.SendWait(patientInfo.PatID);

            //Sex
            mp = new MousePoint(880, 580);
            SetCursorPosition(mp);
            Thread.Sleep(100);
            CheckIfMoved(mp);
            MouseClick();
            Thread.Sleep(100);

            if (patientInfo.Sex.Equals("Male"))
                mp = new MousePoint(880, 650);
            else if (patientInfo.Sex.Equals("Female"))
                mp = new MousePoint(880, 720);
            else
                mp = new MousePoint(880, 780);

            SetCursorPosition(mp);
            Thread.Sleep(100);
            CheckIfMoved(mp);
            MouseClick();

            //BirthDate
            mp = new MousePoint(880, 650);
            SetCursorPosition(mp);
            Thread.Sleep(100);
            CheckIfMoved(mp);
            MouseClick();
            Thread.Sleep(100);
            SendKeys.SendWait(patientInfo.BirthDate);

            //Save
            mp = new MousePoint(960, 730);
            SetCursorPosition(mp);
            Thread.Sleep(3000);
            CheckIfMoved(mp);
            MouseClick();
            Thread.Sleep(2000);
        }
        private static void MouseClick()
        {
            MouseEvent(MouseEventFlags.LeftDown);
            MouseEvent(MouseEventFlags.LeftUp);
        }
        private static void CheckIfMoved(MousePoint mp)
        {
            MousePoint currentMp = GetCursorPosition();
            if (currentMp.X != mp.X || currentMp.Y != mp.Y)
            {
                Interruption interruption = new Interruption();
                DialogResult result = interruption.ShowDialog();
                if (result == DialogResult.Yes)
                {
                    SetCursorPosition(mp);
                }
                else
                    Environment.Exit(0);
            }
        }
    }
}
