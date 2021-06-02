using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfMatchInterface
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class WpfMatchInterfaceClass : Window
    {
        public WpfMatchInterfaceClass()
        {
            InitializeComponent();

            
        }

        private void OnBlueBtnClick(object sender, RoutedEventArgs e)
        {
            OnTeamBtnClick(false);
        }

        private void OnYellowBtnClick(object sender, RoutedEventArgs e)
        {
            OnTeamBtnClick(true);
        }

        private void OnTeamBtnClick(bool Team)
        {
            if (!Team)
            {
                BlueTeamButton.Foreground = Brushes.White;
                BlueTeamButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#FF0554FF");
                BlueTeamButton.BorderThickness = new Thickness(20);

                YellowTeamButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#FFD7E011");
                YellowTeamButton.BorderThickness = new Thickness(1);
            }
            else
            {
                BlueTeamButton.Foreground = Brushes.Black;
                BlueTeamButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#FF005DFF");
                BlueTeamButton.BorderThickness = new Thickness(1);

                YellowTeamButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#FFECAD09");
                YellowTeamButton.BorderThickness = new Thickness(20);
            }
        }

        private void OnRbt1BtnClick(object sender, RoutedEventArgs e)
        {
            OnNumberBtnClick(1);
        }

        private void OnRbt2BtnClick(object sender, RoutedEventArgs e)
        {
            OnNumberBtnClick(2);
        }

        private void OnSoloBtnClick(object sender, RoutedEventArgs e)
        {
            OnNumberBtnClick(0);
        }

        private void OnNumberBtnClick(int number)
        {
            SoloButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFA3F926");
            Robot1Button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFB2A2A");
            Robot2Button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF00B9FF");

            SoloButton.BorderThickness = new Thickness(1);
            Robot1Button.BorderThickness = new Thickness(1);
            Robot2Button.BorderThickness = new Thickness(1);
            

            switch (number)
            {
                case 0:
                    SoloButton.Background = Brushes.DarkGreen;
                    SoloButton.BorderThickness = new Thickness(20);
                    break;
                case 1:
                    Robot1Button.Background = Brushes.DarkRed;
                    Robot1Button.BorderThickness = new Thickness(20);
                    break;
                case 2:
                    Robot2Button.Background = Brushes.DarkBlue;
                    Robot2Button.BorderThickness = new Thickness(20);
                    break;
            }
        }


        private void OnStrategyComboClick(object sender, MouseEventArgs e)
        {
            OnStrategyComboClick();
        }

        private void OnStrategyComboClick(object sender, MouseButtonEventArgs e)
        {
            OnStrategyComboClick();
        }

        private void OnStrategyComboClick()
        {
            
        }


        private void OnSubmitBtnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
