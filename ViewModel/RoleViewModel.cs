using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Novak_Lr1_3_BAI_24_01.Helper;
using Novak_Lr1_3_BAI_24_01.Model;
using Novak_Lr1_3_BAI_24_01.View;

namespace Novak_Lr1_3_BAI_24_01.ViewModel
{
    public class RoleViewModel : INotifyPropertyChanged
    {
        private static RoleViewModel instance;
        public static RoleViewModel Instance
        {
            get
            {
                if (instance == null) instance = new RoleViewModel();
                return instance;
            }
        }

        public ObservableCollection<Role> ListRole { get; set; } = new ObservableCollection<Role>();

        private Role selectedRole;
        public Role SelectedRole
        {
            get { return selectedRole; }
            set { selectedRole = value; OnPropertyChanged("SelectedRole"); }
        }

        private RoleViewModel()
        {
            ListRole = new ObservableCollection<Role>();
            ListRole = GetRoles();
        }

        // Загрузка из БД
        private ObservableCollection<Role> GetRoles()
        {
            using (var context = new CompanyEntities())
            {
                var query = from role in context.Roles
                            orderby role.NameRole
                            select role;
                if (query.Count() != 0)
                {
                    foreach (var c in query) ListRole.Add(c);
                }
            }
            return ListRole;
        }

        public RelayCommand AddRole
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Role newRole = new Role();
                    WindowNewRole wnRole = new WindowNewRole { Title = "Новая должность", DataContext = newRole };
                    wnRole.ShowDialog();
                    if (wnRole.DialogResult == true)
                    {
                        using (var context = new CompanyEntities())
                        {
                            try
                            {
                                context.Roles.Add(newRole);
                                context.SaveChanges();
                                ListRole.Clear();
                                ListRole = GetRoles();
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

        public RelayCommand EditRole
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Role editRole = SelectedRole;
                    WindowNewRole wnRole = new WindowNewRole { Title = "Редактирование должности", DataContext = editRole };
                    wnRole.ShowDialog();
                    if (wnRole.DialogResult == true)
                    {
                        using (var context = new CompanyEntities())
                        {
                            Role role = context.Roles.Find(editRole.Id);
                            if (role.NameRole != editRole.NameRole)
                                role.NameRole = editRole.NameRole.Trim();
                            try
                            {
                                context.SaveChanges();
                                ListRole.Clear();
                                ListRole = GetRoles();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("\nОшибка редактирования данных!\n" + ex.Message, "Предупреждение");
                            }
                        }
                    }
                }, (obj) => SelectedRole != null && ListRole.Count > 0);
            }
        }

        public RelayCommand DeleteRole
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Role role = SelectedRole;
                    using (var context = new CompanyEntities())
                    {
                        Role delRole = context.Roles.Find(role.Id);
                        if (delRole != null)
                        {
                            MessageBoxResult result = MessageBox.Show("Удалить данные по должности: " + delRole.NameRole,
                                "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Roles.Remove(delRole);
                                    context.SaveChanges();
                                    ListRole.Remove(role);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedRole != null && ListRole.Count > 0);
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