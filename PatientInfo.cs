using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSimRT
{
    public class PatientInfo
    {
		private string _patID;
        private string _sex;
        private string _lastName;
        private string _firstName;
        private string _birthDate;

        public string PatID
		{
			get { return _patID; }
			set { _patID = value; }
		}

		public string FirstName
		{
			get { return _firstName; }
			set { _firstName = value; }
		}

		public string LastName
		{
			get { return _lastName; }
			set { _lastName = value; }
		}

		public string Sex
		{
			get { return _sex; }
			set { _sex = value; }
		}

		public string BirthDate
		{
			get { return _birthDate; }
			set { _birthDate = value; }
		}

		public PatientInfo() { }

    }
}
