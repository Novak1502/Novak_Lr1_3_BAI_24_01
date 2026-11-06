using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Novak_Lr1_3_BAI_24_01.Helper;
using Novak_Lr1_3_BAI_24_01.Model;
using Novak_Lr1_3_BAI_24_01.View;

namespace Novak_Lr1_3_BAI_24_01.ViewModel
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        private PersonDpo selectedPersonDpo;
        public PersonDpo SelectedPersonDpo
        {
            get { return selectedPersonDpo; }
            set
            {
                selectedPersonDpo = value;
                OnPropertyChanged("SelectedPersonDpo");
            }
        }

        public ObservableCollection<Person> ListPerson { get; set; } = new ObservableCollection<Person>();
        public ObservableCollection<PersonDpo> ListPersonDpo { get; set; } = new ObservableCollection<PersonDpo>();

        public PersonViewModel()
        {
            ListPerson.Add(new Person { Id = 1, RoleId = 1, FirstName = "Иван", LastName = "Иванов", Birthday = new DateTime(1980, 02, 28) });
            ListPerson.Add(new Person { Id = 2, RoleId = 2, FirstName = "Петр", LastName = "Петров", Birthday = new DateTime(1981, 03, 20) });
            ListPerson.Add(new Person { Id = 3, RoleId = 3, FirstName = "Виктор", LastName = "Викторов", Birthday = new DateTime(1982, 04, 15) });
            ListPerson.Add(new Person { Id = 4, RoleId = 3, FirstName = "Сидор", LastName = "Сидоров", Birthday = new DateTime(1983, 05, 10) });

            GetListPersonDpo();
        }

        public void GetListPersonDpo()
        {
            RoleViewModel vmRole = RoleViewModel.Instance;   
            foreach (var person in ListPerson)
            {
                PersonDpo p = new PersonDpo();
                string role = string.Empty;

                foreach (var r in vmRole.ListRole)
                {
                    if (r.Id == person.RoleId)
                    {
                        role = r.NameRole;
                        break;
                    }
                }

                p.Id = person.Id;
                p.RoleName = role;
                p.FirstName = person.FirstName;
                p.LastName = person.LastName;
                p.Birthday = person.Birthday;

                ListPersonDpo.Add(p);
            }
        }

        public int MaxId()
        {
            int max = 0;
            foreach (var r in ListPerson)
            {
                if (max < r.Id) max = r.Id;
            }
            return max;
        }

        private RelayCommand addPerson;
        public RelayCommand AddPerson
        {
            get
            {
                return addPerson ??
                (addPerson = new RelayCommand(obj =>
                {
                    WindowNewEmployee wnPerson = new WindowNewEmployee { Title = "Новый сотрудник" };
                    int maxIdPerson = MaxId() + 1;
                    PersonDpo per = new PersonDpo { Id = maxIdPerson, Birthday = DateTime.Now };
                    wnPerson.DataContext = per;

                    if (wnPerson.ShowDialog() == true)
                    {
                        Role r = wnPerson.CbRole.SelectedItem as Role;
                        if (r == null)
                        {
                            MessageBox.Show("Выберите должность!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        per.RoleName = r.NameRole;
                        ListPersonDpo.Add(per);

                        Person p = new Person
                        {
                            Id = per.Id,
                            RoleId = r.Id,
                            FirstName = per.FirstName,
                            LastName = per.LastName,
                            Birthday = per.Birthday
                        };
                        ListPerson.Add(p);
                    }
                },
                (obj) => true));
            }
        }

        private RelayCommand editPerson;
        public RelayCommand EditPerson
        {
            get
            {
                return editPerson ??
                (editPerson = new RelayCommand(obj =>
                {
                    WindowNewEmployee wnPerson = new WindowNewEmployee { Title = "Редактирование данных сотрудника" };
                    PersonDpo personDpo = SelectedPersonDpo;
                    PersonDpo tempPerson = personDpo.ShallowCopy();
                    wnPerson.DataContext = tempPerson;

                    if (wnPerson.ShowDialog() == true)
                    {
                        Role r = wnPerson.CbRole.SelectedItem as Role;
                        if (r == null)
                        {
                            MessageBox.Show("Выберите должность!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        personDpo.RoleName = r.NameRole;
                        personDpo.FirstName = tempPerson.FirstName;
                        personDpo.LastName = tempPerson.LastName;
                        personDpo.Birthday = tempPerson.Birthday;

                        FindPerson finder = new FindPerson(personDpo.Id);
                        List<Person> listPerson = ListPerson.ToList();
                        Person p = listPerson.Find(new Predicate<Person>(finder.PersonPredicate));
                        if (p != null)
                        {
                            p.RoleId = r.Id;
                            p.FirstName = personDpo.FirstName;
                            p.LastName = personDpo.LastName;
                            p.Birthday = personDpo.Birthday;
                        }
                    }
                },
                (obj) => SelectedPersonDpo != null && ListPersonDpo.Count > 0));
            }
        }

        private RelayCommand deletePerson;
        public RelayCommand DeletePerson
        {
            get
            {
                return deletePerson ??
                (deletePerson = new RelayCommand(obj =>
                {
                    PersonDpo person = SelectedPersonDpo;
                    MessageBoxResult result = MessageBox.Show(
                        "Удалить данные по сотруднику: \n" + person.LastName + " " + person.FirstName,
                        "Предупреждение",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.OK)
                    {
                        ListPersonDpo.Remove(person);

                        FindPerson finder = new FindPerson(person.Id);
                        List<Person> listPerson = ListPerson.ToList();
                        Person p = listPerson.Find(new Predicate<Person>(finder.PersonPredicate));
                        if (p != null)
                        {
                            ListPerson.Remove(p);
                        }
                    }
                },
                (obj) => SelectedPersonDpo != null && ListPersonDpo.Count > 0));
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