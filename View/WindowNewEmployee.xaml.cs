using System.Windows;
using Novak_Lr1_3_BAI_24_01.ViewModel;

namespace Novak_Lr1_3_BAI_24_01.View
{
    public partial class WindowNewEmployee : Window
    {
        public WindowNewEmployee()
        {
            InitializeComponent();

            // Берём ОДИН и тот же список должностей (Singleton)
            var roles = RoleViewModel.Instance.ListRole;
            CbRole.ItemsSource = roles;

            if (roles.Count > 0)
                CbRole.SelectedIndex = 0;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CbRole.SelectedItem == null)
            {
                MessageBox.Show("Выберите должность!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}