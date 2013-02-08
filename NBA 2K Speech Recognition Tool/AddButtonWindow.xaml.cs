#region Copyright Notice

//    Copyright 2011-2013 Eleftherios Aslanoglou
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

#endregion

#region Using Directives

using System;
using System.Collections.Generic;
using System.Windows;

#endregion

namespace NBA_2K_Speech_Recognition_Tool
{
    /// <summary>
    ///     Interaction logic for AddButtonWindow.xaml
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

            var buttons = new List<string>
                          {
                              "Pass (A/Cross)",
                              "Shoot (X/Square)",
                              "Fake Pass (B/Circle)",
                              "Post Up (Y/Triangle)",
                              "Positional Plays (LB/L1)",
                              "Icon Pass (RB/R1)",
                              "Clutch (LT/L2)",
                              "Sprint (RT/R2)",
                              "D-Pad Up",
                              "D-Pad Down",
                              "D-Pad Left",
                              "D-Pad Right",
                              "Call Timeout (Back/Select)"
                          };

            cmbButtons.ItemsSource = buttons;
        }

        public AddButtonWindow(string button, string type, int duration, int repeats) : this()
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