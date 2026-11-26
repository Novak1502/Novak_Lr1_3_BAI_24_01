using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using Novak_Lr1_3_BAI_24_01.Helper;
using Novak_Lr1_3_BAI_24_01.Model;
using Novak_Lr1_3_BAI_24_01.View;

namespace Novak_Lr1_3_BAI_24_01.ViewModel
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        private Person selectedPerson;
        public Person SelectedPerson
        {
            get { return selectedPerson; }
            set { selectedPerson = value; OnPropertyChanged("SelectedPerson"); }
        }

        public ObservableCollection<Person> ListPerson { get; set; }

        public PersonViewModel()
        {
            ListPerson = new ObservableCollection<Person>();
            ListPerson = GetPersons();
        }

        // Загрузка из БД с подгрузкой должности (Include)
        private ObservableCollection<Person> GetPersons()
        {
            using (var context = new CompanyEntities())
            {
                var query = from per in context.Persons.Include("Role")
                            orderby per.LastName
                            select per;
                if (query.Count() != 0)
                {
                    foreach (var p in query) ListPerson.Add(p);
                }
            }
            return ListPerson;
        }

        public RelayCommand AddPerson
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Person newPerson = new Person { Birthday = DateTime.Now };
                    WindowNewEmployee wnPerson = new WindowNewEmployee { Title = "Новый сотрудник", DataContext = newPerson };
                    wnPerson.ShowDialog();
                    if (wnPerson.DialogResult == true)
                    {
                        using (var context = new CompanyEntities())
                        {
                            try
                            {
                                context.Persons.Add(newPerson);
                                context.SaveChanges();
                                ListPerson.Clear();
                                ListPerson = GetPersons();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("\nОшибка добавления данных!\n" + ex.Message, "Предупреждение");
                            }
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
                    Person editPerson = SelectedPerson;
                    WindowNewEmployee wnPerson = new WindowNewEmployee { Title = "Редактирование данных", DataContext = editPerson };
                    wnPerson.ShowDialog();
                    if (wnPerson.DialogResult == true)
                    {
                        using (var context = new CompanyEntities())
                        {
                            Person person = context.Persons.Find(editPerson.Id);
                            try
                            {
                                context.SaveChanges();
                                ListPerson.Clear();
                                ListPerson = GetPersons();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("\nОшибка редактирования данных!\n" + ex.Message, "Предупреждение");
                            }
                        }
                    }
                }, (obj) => SelectedPerson != null && ListPerson.Count > 0);
            }
        }

        public RelayCommand DeletePerson
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Person person = SelectedPerson;
                    using (var context = new CompanyEntities())
                    {
                        Person delPerson = context.Persons.Find(person.Id);
                        if (delPerson != null)
                        {
                            MessageBoxResult result = MessageBox.Show("Удалить данные по сотруднику: \n" + delPerson.LastName + " " + delPerson.FirstName,
                                "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Persons.Remove(delPerson);
                                    context.SaveChanges();
                                    ListPerson.Remove(person);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedPerson != null && ListPerson.Count > 0);
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