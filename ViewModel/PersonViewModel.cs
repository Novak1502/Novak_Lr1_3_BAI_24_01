using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using Novak_Lr1_3_BAI_24_01.Helper;
using Novak_Lr1_3_BAI_24_01.Model;
using Novak_Lr1_3_BAI_24_01.View;

namespace Novak_Lr1_3_BAI_24_01.ViewModel
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        // Программа сама создаст эту папку и файл, если их нет
        private readonly string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataModels");
        private readonly string path;

        string _jsonPersons = String.Empty;
        public string Error { get; set; }

        private PersonDpo selectedPersonDpo;
        public PersonDpo SelectedPersonDpo
        {
            get { return selectedPersonDpo; }
            set { selectedPersonDpo = value; OnPropertyChanged("SelectedPersonDpo"); }
        }

        public ObservableCollection<Person> ListPerson { get; set; }
        public ObservableCollection<PersonDpo> ListPersonDpo { get; set; }

        public PersonViewModel()
        {
            // Настраиваем путь
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            path = Path.Combine(folderPath, "PersonData.json");

            ListPerson = new ObservableCollection<Person>();
            ListPersonDpo = new ObservableCollection<PersonDpo>();
            ListPerson = LoadPerson();
            ListPersonDpo = GetListPersonDpo();
        }

        public RelayCommand AddPerson
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    WindowNewEmployee wnPerson = new WindowNewEmployee { Title = "Новый сотрудник" };
                    int maxIdPerson = MaxId() + 1;
                    PersonDpo per = new PersonDpo { Id = maxIdPerson, Birthday = DateTime.Now.ToString("dd.MM.yyyy") };
                    wnPerson.DataContext = per;

                    if (wnPerson.ShowDialog() == true)
                    {
                        var r = (Role)wnPerson.CbRole.SelectedItem;
                        if (r != null)
                        {
                            per.RoleName = r.NameRole;
                            per.Birthday = PersonDpo.GetStringBirthday(per.Birthday);
                            ListPersonDpo.Add(per);

                            Person p = new Person();
                            p = p.CopyFromPersonDpo(per);
                            p.RoleId = r.Id;
                            ListPerson.Add(p);
                            SaveChanges(ListPerson);
                        }
                    }
                });
            }
        }

        public RelayCommand EditPerson
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    WindowNewEmployee wnPerson = new WindowNewEmployee { Title = "Редактирование данных сотрудника" };
                    PersonDpo personDpo = SelectedPersonDpo;
                    var tempPerson = personDpo.ShallowCopy();
                    wnPerson.DataContext = tempPerson;

                    if (wnPerson.ShowDialog() == true)
                    {
                        var r = (Role)wnPerson.CbRole.SelectedItem;
                        if (r != null)
                        {
                            personDpo.RoleName = r.NameRole;
                            personDpo.FirstName = tempPerson.FirstName;
                            personDpo.LastName = tempPerson.LastName;
                            personDpo.Birthday = PersonDpo.GetStringBirthday(tempPerson.Birthday);

                            var per = ListPerson.FirstOrDefault(p => p.Id == personDpo.Id);
                            if (per != null)
                            {
                                per = per.CopyFromPersonDpo(personDpo);
                                per.RoleId = r.Id;
                            }
                            SaveChanges(ListPerson);
                        }
                    }
                }, (obj) => SelectedPersonDpo != null && ListPersonDpo.Count > 0);
            }
        }

        public RelayCommand DeletePerson
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    PersonDpo person = SelectedPersonDpo;
                    MessageBoxResult result = MessageBox.Show("Удалить данные по сотруднику: \n" + person.LastName + " " + person.FirstName,
                        "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        ListPersonDpo.Remove(person);
                        var per = ListPerson.FirstOrDefault(p => p.Id == person.Id);
                        if (per != null)
                        {
                            ListPerson.Remove(per);
                            SaveChanges(ListPerson);
                        }
                    }
                }, (obj) => SelectedPersonDpo != null && ListPersonDpo.Count > 0);
            }
        }

        public ObservableCollection<Person> LoadPerson()
        {
            try
            {
                if (File.Exists(path))
                {
                    _jsonPersons = File.ReadAllText(path, Encoding.UTF8);
                }

                // Если файл пустой, создаем базовые данные
                if (string.IsNullOrWhiteSpace(_jsonPersons))
                {
                    var defaultPersons = new ObservableCollection<Person>
                    {
                        new Person { Id = 1, RoleId = 1, FirstName = "Иван", LastName = "Иванов", Birthday = "28.02.1980" },
                        new Person { Id = 2, RoleId = 2, FirstName = "Петр", LastName = "Петров", Birthday = "20.03.1981" },
                        new Person { Id = 3, RoleId = 3, FirstName = "Виктор", LastName = "Викторов", Birthday = "16.04.1982" },
                        new Person { Id = 4, RoleId = 3, FirstName = "Сидор", LastName = "Сидоров", Birthday = "10.05.1983" }
                    };
                    SaveChanges(defaultPersons);
                    return defaultPersons;
                }

                return JsonConvert.DeserializeObject<ObservableCollection<Person>>(_jsonPersons);
            }
            catch (Exception e)
            {
                Error = "Ошибка чтения json: " + e.Message;
                return new ObservableCollection<Person>();
            }
        }

        public ObservableCollection<PersonDpo> GetListPersonDpo()
        {
            RoleViewModel vmRole = RoleViewModel.Instance;
            foreach (var person in ListPerson)
            {
                PersonDpo p = new PersonDpo();
                p = p.CopyFrom(person);

                string role = string.Empty;
                foreach (var r in vmRole.ListRole)
                {
                    if (r.Id == person.RoleId) { role = r.NameRole; break; }
                }
                p.RoleName = role;
                p.Birthday = PersonDpo.GetStringBirthday(person.Birthday);
                ListPersonDpo.Add(p);
            }
            return ListPersonDpo;
        }

        public int MaxId()
        {
            int max = 0;
            foreach (var r in this.ListPerson) { if (max < r.Id) max = r.Id; }
            return max;
        }

        private void SaveChanges(ObservableCollection<Person> listPersons)
        {
            var jsonPerson = JsonConvert.SerializeObject(listPersons, Formatting.Indented);
            try
            {
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                {
                    writer.Write(jsonPerson);
                }
            }
            catch (IOException e)
            {
                Error = "Ошибка записи json: " + e.Message;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}