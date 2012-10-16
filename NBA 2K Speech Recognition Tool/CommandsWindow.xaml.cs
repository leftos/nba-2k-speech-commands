using System;
using System.Collections.Generic;
using System.IO;
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
using Path = System.IO.Path;

namespace NBA_2K_Speech_Recognition_Tool
{
    /// <summary>
    /// Interaction logic for CommandsWindow.xaml
    /// </summary>
    public partial class CommandsWindow : Window
    {
        bool deleting;
        
        public CommandsWindow()
        {
            InitializeComponent();

            foreach (string file in Directory.GetFiles(MainWindow.CommandsPath))
            {
                deleting = true;
                cmbCommands.Items.Add(Path.GetFileNameWithoutExtension(file));
                cmbCommands.SelectedIndex = 0;
                deleting = false;
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            InputBoxWindow ibw = new InputBoxWindow("Enter the name of the new command");
            if (ibw.ShowDialog() == true)
            {
                if (MainWindow.input.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    MessageBox.Show("'" + MainWindow.input + "' is not a valid name for a command.");
                    return;
                }
                cmbCommands.Items.Add(MainWindow.input);
                cmbCommands.SelectedIndex = cmbCommands.Items.Count - 1;
                lstButtons.Items.Clear();
                lstVoiceCommands.Items.Clear();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCommands.SelectedIndex == -1)
                return;

            if (cmbCommands.Items.Count == 0)
            {
                MessageBox.Show("You can't remove the last command.");
                return;
            }
            deleting = true;
            cmbCommands.SelectedIndex--;
            cmbCommands.Items.RemoveAt(cmbCommands.SelectedIndex + 1);
            deleting = false;
        }

        private void btnAddButton_Click(object sender, RoutedEventArgs e)
        {
            AddButtonWindow abw = new AddButtonWindow();
            if (abw.ShowDialog() == true)
            {
                var toShow = convertAddButtonResult();
                lstButtons.Items.Add(toShow);
            }
        }

        private static string convertAddButtonResult()
        {
            string[] parts = MainWindow.input.Split(new[] {"$;$"}, StringSplitOptions.None).Skip(1).ToArray();
            string toShow = "";
            if (parts[1] == "Press")
            {
                toShow = String.Format("{0} '{1}' for {2} ms", parts[1], parts[0], parts[2]);
            }
            else
            {
                toShow = String.Format("{0} '{1}' for {2} repeats, {3} ms each", parts[1], parts[0], parts[3], parts[2]);
            }
            return toShow;
        }

        private void btnEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (lstButtons.SelectedIndex == -1)
                return;

            string[] parts = lstButtons.SelectedItem.ToString().Split(new[] {' '});

            AddButtonWindow abw;
            int endIndex = 1;
            string cmd = "";
            for (int i = 1; i < parts.Count(); i++)
            {
                cmd += parts[i];
                if (parts[i].EndsWith("'"))
                {
                    endIndex = i;
                    break;
                }
                cmd += " ";
            }
            cmd = cmd.Replace("'", "");
            if (parts[0] == "Press")
            {
                abw = new AddButtonWindow(cmd, parts[0], Convert.ToInt32(parts[endIndex+2]), 30);
            }
            else
            {
                abw = new AddButtonWindow(cmd, parts[0], Convert.ToInt32(parts[endIndex+4]), Convert.ToInt32(parts[endIndex+2]));
            }
            if (abw.ShowDialog() == true)
            {
                var toShow = convertAddButtonResult();
                int index = lstButtons.SelectedIndex;
                lstButtons.Items.RemoveAt(index);
                lstButtons.Items.Insert(index, toShow);
            }
        }

        private void btnDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (lstButtons.SelectedIndex == -1)
                return;

            lstButtons.Items.RemoveAt(lstButtons.SelectedIndex);
        }

        private void btnAddVoiceCmd_Click(object sender, RoutedEventArgs e)
        {
            InputBoxWindow ibw = new InputBoxWindow("Enter the spoken phrase you want to tie to this action");
            if (ibw.ShowDialog() == true)
            {
                string cmd = MainWindow.input;
                if (MainWindow.allVoiceCommands.Contains(cmd))
                {
                    MessageBox.Show("This command is already in use.");
                    return;
                }

                lstVoiceCommands.Items.Add(cmd);
            }
        }

        private void btnEditVoiceCmd_Click(object sender, RoutedEventArgs e)
        {
            if (lstVoiceCommands.SelectedIndex == -1)
                return;

            InputBoxWindow ibw = new InputBoxWindow("Enter the spoken phrase you want to tie to this action", lstVoiceCommands.SelectedItem.ToString());
            if (ibw.ShowDialog() == true)
            {
                string cmd = MainWindow.input;
                if (MainWindow.allVoiceCommands.Contains(cmd))
                {
                    MessageBox.Show("This command is already in use.");
                    return;
                }

                int index = lstVoiceCommands.SelectedIndex;
                lstVoiceCommands.Items.RemoveAt(index);
                lstVoiceCommands.Items.Insert(index, cmd);
            }
        }

        private void btnDeleteVoiceCmd_Click(object sender, RoutedEventArgs e)
        {
            if (lstVoiceCommands.SelectedIndex == -1)
                return;

            lstVoiceCommands.Items.RemoveAt(lstVoiceCommands.SelectedIndex);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (lstButtons.SelectedIndex == -1)
                return;

            int index = lstButtons.SelectedIndex;
            string cmd = lstButtons.SelectedItem.ToString();
            lstButtons.Items.RemoveAt(index);
            lstButtons.Items.Insert(index - 1, cmd);
            lstButtons.SelectedIndex = index - 1;
        }

        private void btnMoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (lstButtons.SelectedIndex == -1)
                return;

            int index = lstButtons.SelectedIndex;
            string cmd = lstButtons.SelectedItem.ToString();
            lstButtons.Items.RemoveAt(index);
            lstButtons.Items.Insert(index + 1, cmd);
            lstButtons.SelectedIndex = index + 1;
        }

        private void cmbCommands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!deleting)
            {
                if (e.RemovedItems.Count == 1)
                    SaveCurrentCommand(e.RemovedItems[0].ToString());
            }
            LoadNewCommand(e.AddedItems[0].ToString());
        }

        private void LoadNewCommand(string cmdName)
        {
            string file = MainWindow.CommandsPath + "\\" + cmdName + ".src";
            lstButtons.Items.Clear();
            lstVoiceCommands.Items.Clear();
            if (File.Exists(file))
            {
                StreamReader sr = new StreamReader(file);
                sr.ReadLine();
                sr.ReadLine();
                string s = sr.ReadLine();
                while (s != "END_BUTTONS")
                {
                    lstButtons.Items.Add(s);
                    s = sr.ReadLine();
                }
                sr.ReadLine();
                s = sr.ReadLine();
                while (s != "END_VOICE")
                {
                    lstVoiceCommands.Items.Add(s);
                    s = sr.ReadLine();
                }
                sr.Close();
            }
        }

        private void SaveCurrentCommand(string cmdName)
        {
            StreamWriter sw = new StreamWriter(MainWindow.CommandsPath + "\\" + cmdName + ".src", false);
            sw.WriteLine(cmdName);
            sw.WriteLine("BEGIN_BUTTONS");
            foreach (string button in lstButtons.Items)
            {
                sw.WriteLine(button);
            }
            sw.WriteLine("END_BUTTONS");
            sw.WriteLine("BEGIN_VOICE");
            foreach (string voicecmd in lstVoiceCommands.Items)
            {
                sw.WriteLine(voicecmd);
            }
            sw.WriteLine("END_VOICE");
            sw.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (cmbCommands.SelectedIndex != -1) SaveCurrentCommand(cmbCommands.SelectedItem.ToString());
        }
    }
}
