using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
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

        // Программа сама создаст эту папку и файл, если их нет
        private readonly string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataModels");
        private readonly string path;

        string _jsonRoles = String.Empty;
        public string Error { get; set; }
        public ObservableCollection<Role> ListRole { get; set; } = new ObservableCollection<Role>();

        private Role selectedRole;
        public Role SelectedRole
        {
            get { return selectedRole; }
            set { selectedRole = value; OnPropertyChanged("SelectedRole"); }
        }

        private RoleViewModel()
        {
            // Настраиваем путь
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            path = Path.Combine(folderPath, "RoleData.json");

            ListRole = LoadRole();
        }

        public RelayCommand AddRole
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    WindowNewRole wnRole = new WindowNewRole { Title = "Новая должность" };
                    int maxIdRole = MaxId() + 1;
                    Role role = new Role { Id = maxIdRole };
                    wnRole.DataContext = role;
                    if (wnRole.ShowDialog() == true)
                    {
                        ListRole.Add(role);
                        SaveChanges(ListRole);
                    }
                    SelectedRole = role;
                });
            }
        }

        public RelayCommand EditRole
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    WindowNewRole wnRole = new WindowNewRole { Title = "Редактирование должности" };
                    Role role = SelectedRole;
                    Role tempRole = role.ShallowCopy();
                    wnRole.DataContext = tempRole;
                    if (wnRole.ShowDialog() == true)
                    {
                        role.NameRole = tempRole.NameRole;
                        SaveChanges(ListRole);
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
                    MessageBoxResult result = MessageBox.Show("Удалить данные по должности: " + role.NameRole,
                        "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        ListRole.Remove(role);
                        SaveChanges(ListRole);
                    }
                }, (obj) => SelectedRole != null && ListRole.Count > 0);
            }
        }

        public ObservableCollection<Role> LoadRole()
        {
            try
            {
                if (File.Exists(path))
                {
                    _jsonRoles = File.ReadAllText(path, Encoding.UTF8);
                }

                // Если файл пустой, создаем базовые данные
                if (string.IsNullOrWhiteSpace(_jsonRoles))
                {
                    var defaultRoles = new ObservableCollection<Role>
                    {
                        new Role { Id = 0, NameRole = "Не задано" },
                        new Role { Id = 1, NameRole = "Директор" },
                        new Role { Id = 2, NameRole = "Бухгалтер" },
                        new Role { Id = 3, NameRole = "Менеджер" }
                    };
                    SaveChanges(defaultRoles);
                    return defaultRoles;
                }

                return JsonConvert.DeserializeObject<ObservableCollection<Role>>(_jsonRoles);
            }
            catch (Exception e)
            {
                Error = "Ошибка чтения json: " + e.Message;
                return new ObservableCollection<Role>();
            }
        }

        public int MaxId()
        {
            int max = 0;
            foreach (var r in this.ListRole) { if (max < r.Id) max = r.Id; }
            return max;
        }

        private void SaveChanges(ObservableCollection<Role> listRole)
        {
            var jsonRole = JsonConvert.SerializeObject(listRole, Formatting.Indented);
            try
            {
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                {
                    writer.Write(jsonRole);
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