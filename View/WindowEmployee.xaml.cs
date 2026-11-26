using System.Windows;
using Novak_Lr1_3_BAI_24_01.ViewModel;

namespace Novak_Lr1_3_BAI_24_01.View
{
    public partial class WindowEmployee : Window
    {
        public WindowEmployee()
        {
            InitializeComponent();
            DataContext = new PersonViewModel();
        }
    }
}