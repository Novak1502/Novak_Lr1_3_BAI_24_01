using System;
using System.ComponentModel;

namespace Novak_Lr1_3_BAI_24_01.Model
{
    public class PersonDpo : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string roleName;
        public string RoleName
        {
            get { return roleName; }
            set { roleName = value; OnPropertyChanged("RoleName"); }
        }

        private string firstName;
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; OnPropertyChanged("FirstName"); }
        }

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; OnPropertyChanged("LastName"); }
        }

        private string birthday;
        public string Birthday
        {
            get { return birthday; }
            set { birthday = value; OnPropertyChanged("Birthday"); }
        }

        public PersonDpo() { }

        public PersonDpo(int id, string roleName, string firstName, string lastName, string birthday)
        {
            this.Id = id;
            this.RoleName = roleName;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Birthday = birthday;
        }

        public PersonDpo ShallowCopy()
        {
            return (PersonDpo)this.MemberwiseClone();
        }

        public static string GetStringBirthday(string birthday)
        {
            if (string.IsNullOrWhiteSpace(birthday))
                return DateTime.Now.ToString("dd.MM.yyyy");

            if (DateTime.TryParse(birthday, out DateTime dt))
                return dt.ToString("dd.MM.yyyy");

            return DateTime.Now.ToString("dd.MM.yyyy");
        }

        public PersonDpo CopyFrom(Person person)
        {
            this.Id = person.Id;
            this.FirstName = person.FirstName;
            this.LastName = person.LastName;
            this.Birthday = person.Birthday;
            return this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}