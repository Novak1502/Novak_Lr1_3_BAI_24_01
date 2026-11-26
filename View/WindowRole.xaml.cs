using System.Windows;
using Novak_Lr1_3_BAI_24_01.ViewModel;

namespace Novak_Lr1_3_BAI_24_01.View
{
    public partial class WindowRole : Window
    {
        public WindowRole()
        {
            InitializeComponent();
            DataContext = RoleViewModel.Instance;  
        }
    }
}