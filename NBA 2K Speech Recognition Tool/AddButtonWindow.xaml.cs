using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NBA_2K_Speech_Recognition_Tool
{
    /// <summary>
    /// Interaction logic for AddButtonWindow.xaml
    /// </summary>
    public partial class AddButtonWindow : Window
    {
        public AddButtonWindow()
        {
            InitializeComponent();

            grdRepeats.Visibility = Visibility.Hidden;
            rbPress.IsChecked = true;
            txtDuration.Text = "50";
            txtRepeats.Text = "30";

            List<string> buttons = new List<string>
                                   {
                                       "Pass (A/Cross)", "Shoot (X/Square)", "Fake Pass (B/Circle)", "Post Up (Y/Triangle)", "Positional Plays (LB/L1)", "Icon Pass (RB/R1)",
                                       "Clutch (LT/L2)", "Sprint (RT/R2)", "D-Pad Up", "D-Pad Down", "D-Pad Left", "D-Pad Right", "Call Timeout (Back/Select)"
                                   };

            cmbButtons.ItemsSource = buttons;
        }

        public AddButtonWindow(string button, string type, int duration, int repeats): this()
        {
            cmbButtons.SelectedItem = button;
            if (type == "Press")
                rbPress.IsChecked = true;
            else
                rbHold.IsChecked = true;
            txtDuration.Text = duration.ToString();
            txtRepeats.Text = repeats.ToString();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (cmbButtons.SelectedIndex == -1)
                return;

            try
            {
                Convert.ToInt32(txtDuration.Text);
                Convert.ToInt32(txtRepeats.Text);
            }
            catch (Exception)
            {
                return;
            }

            DialogResult = true;
            MainWindow.input = "ADDBUTTONRESULT$;$" + cmbButtons.SelectedItem + "$;$" +
                               (rbPress.IsChecked.GetValueOrDefault() ? "Press" : "Hold") + "$;$" + txtDuration.Text + "$;$" +
                               txtRepeats.Text;
            Close();
        }

        private void rbHold_Checked(object sender, RoutedEventArgs e)
        {
            grdRepeats.Visibility = Visibility.Visible;
        }

        private void rbPress_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                grdRepeats.Visibility = Visibility.Hidden;
            }
            catch
            {
                
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
