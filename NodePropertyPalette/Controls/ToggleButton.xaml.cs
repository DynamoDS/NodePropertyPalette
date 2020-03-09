using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NodePropertyPalette.Controls
{
    /// <summary>
    /// Interaction logic for ToggleButton.xaml
    /// </summary>
    public partial class ToggleButton : UserControl
    {
        Thickness LeftSide = new Thickness(4, 4, 44, 4);
        Thickness RightSide = new Thickness(44, 4, 4, 4);
        SolidColorBrush Off = new SolidColorBrush(Color.FromRgb(160,160,160));
        SolidColorBrush On = new SolidColorBrush(Color.FromRgb(130, 190, 125));
        internal bool IsToggled = false;

        public ToggleButton()
        {
            InitializeComponent();
            Back.Fill = On;
            IsToggled = true;
            Dot.Margin = RightSide;
        }

        private void Dot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(!IsToggled)
            {
                // Should turn on
                Back.Fill = On;
                IsToggled = true;
                Dot.Margin = RightSide;
            }
            else
            {
                // Should turn off
                Back.Fill = Off;
                IsToggled = false;
                Dot.Margin = LeftSide;
            }
        }

        private void Back_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsToggled)
            {
                // Should turn on
                Back.Fill = On;
                IsToggled = true;
                Dot.Margin = RightSide;
            }
            else
            {
                // Should turn off
                Back.Fill = Off;
                IsToggled = false;
                Dot.Margin = LeftSide;
            }
        }
    }
}
