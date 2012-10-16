using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace NBA_2K_Speech_Recognition_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private List<string> isoCommands;
        private List<string> postCommands;
        private List<string> wingCommands;
        private List<string> cutCommands;
        private List<string> bestCommands;
        private List<string> screen5Commands;
        private List<string> screenCommands; 
        private SpeechRecognitionEngine sre;
        private List<string> mainCommands;
        private List<string> easterEggCommands; 

        private bool enabled = true;
        private bool easterEggEnabled = true;

        private string title = "";
        public static string input = "";

        public static List<string> allVoiceCommands = new List<string>();

        public static string DocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\NBA 2K Speech Recognition Tool";
        public static string CommandsPath = DocsPath + @"\Commands";

        private List<Command> commandsList = new List<Command>(); 

        public MainWindow()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US"); 

            if (!Directory.Exists(DocsPath))
                Directory.CreateDirectory(DocsPath);

            if (!Directory.Exists(CommandsPath))
                Directory.CreateDirectory(CommandsPath);

            title = Title;
            Title += " - Enabled";

            if (File.Exists(DocsPath + @"\goodboy"))
                easterEggEnabled = false;

            //Create the speech recognition engine
            sre = new SpeechRecognitionEngine();
            
            //Set the audio device to the OS default
            sre.SetInputToDefaultAudioDevice();

            parseCommands();

            //Add an event handler
            sre.SpeechRecognized += sre_SpeechRecognized;
            sre.SpeechDetected += sre_SpeechDetected;
            sre.RecognizeCompleted += sre_RecognizeCompleted;

            Timer timer = new Timer(15000);
            timer.Elapsed += Callback;
            timer.Start();

            //Set the eventing model
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void parseCommands()
        {
            //Reset the Grammars
            sre.UnloadAllGrammars();

            foreach (string file in Directory.GetFiles(CommandsPath))
            {
                StreamReader sr = new StreamReader(file);
                string s = sr.ReadLine();
                Command c = new Command();
                c.Name = s;
                sr.ReadLine();
                s = sr.ReadLine();
                while (s != "END_BUTTONS")
                {
                    c.Buttons.Add(s);
                    s = sr.ReadLine();
                }
                sr.ReadLine();
                s = sr.ReadLine();
                while (s != "END_VOICE")
                {
                    c.VoiceCommands.Add(s);
                    s = sr.ReadLine();
                }
                sr.Close();
                commandsList.Add(c);
            }
            commandsList.ForEach(c => c.VoiceCommands.ForEach(vc => allVoiceCommands.Add(vc)));

            mainCommands = new List<string> { "start listening", "stop listening", "I promise to be good" };
            mainCommands.ForEach(s => allVoiceCommands.Add(s));

            easterEggCommands = new List<string> {"fuck", "shit", "bullshit", "this is bullshit", "fuck that", "fuck you", "what the fuck", 
                "fucker", "motherfucker", "mother fucker", "cunt", "bastard", "you bastard", "you cunt", "bitch", "fucking bitch", "you bitch"};
            easterEggCommands.ForEach(s => allVoiceCommands.Add(s));

            #region Old Commands
            /*
            isoCommands = new List<string>
                          {
                              "Iso", "Isolation", "Quick Iso", "Quick Isolation", "Give me space", "Man to man", "On your players", "Man tuman"
                          };
            postCommands = new List<string>
                           {
                               "Post up", "Go to post", "Go low", "Two three zone", "Two three"
                           };
            wingCommands = new List<string>
                           {
                               "Get open", "To the wing", "Three two zone", "Three two"
                           };

            cutCommands = new List<string>
                          {
                              "Cut to basket", "To the hoop", "One three one zone", "One three one"
                          };

            bestCommands = new List<string>
                           {
                               "Run best play", "You know what to do"
                           };

            screen5Commands = new List<string> {"five screen", "center screen", "center, set a screen", "five, set a screen", "five, set a pick", "center, set a pick"};

            screenCommands = new List<string> {"screen", "double team", "set a pick", "double", "set a screen"};

            //Create a set of choices
            isoCommands.ForEach(s => allVoiceCommands.Add(s));
            postCommands.ForEach(s => allVoiceCommands.Add(s));
            wingCommands.ForEach(s => allVoiceCommands.Add(s));
            cutCommands.ForEach(s => allVoiceCommands.Add(s));
            bestCommands.ForEach(s => allVoiceCommands.Add(s));
            screen5Commands.ForEach(s => allVoiceCommands.Add(s));
            screenCommands.ForEach(s => allVoiceCommands.Add(s));
            */
            #endregion


            Choices thisChoices = new Choices("Computer");
            allVoiceCommands.ForEach(s => thisChoices.Add(s));

            //Create a grammar based on these choices
            GrammarBuilder gb = new GrammarBuilder(thisChoices);
            gb.Culture = new CultureInfo("en-US");
            Grammar thisGrammar = new Grammar(gb);
            // Set the Grammar name
            thisGrammar.Name = "NBA 2K";

            //Load the grammar
            sre.LoadGrammar(thisGrammar);
        }

        void sre_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                sre.RecognizeAsync(RecognizeMode.Multiple);
                AddToList("SRE reset");
            }
        }

        private void Callback(object state, ElapsedEventArgs elapsedEventArgs)
        {
            sre.RecognizeAsyncStop();
        }

        void sre_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            AddToList("Speech Detected");
        }

        public void Say(string input)
        {
            var synth = new SpeechSynthesizer();
            var sayThis = new Prompt(input);
            synth.Speak(sayThis);
        }

        private void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string said = e.Result.Text;
            if (e.Result.Grammar.Name == "NBA 2K")
            {
                if (enabled)
                {
                    AddToList("Recognized \"" + said + "\"");
                    if (said == "Computer")
                    {
                        Say("I'm ready!");
                    }
                    #region Old Commands
                        /*
                    else if (isoCommands.Contains(said))
                    {
                        PressKey(Keys.Right);
                        PressKey(Keys.S);
                    }
                    else if (postCommands.Contains(said))
                    {
                        PressKey(Keys.Right);
                        PressKey(Keys.D);
                    }
                    else if (wingCommands.Contains(said))
                    {
                        PressKey(Keys.Right);
                        PressKey(Keys.A);
                    }
                    else if (cutCommands.Contains(said))
                    {
                        PressKey(Keys.Right);
                        PressKey(Keys.W);
                    }
                    else if (bestCommands.Contains(said))
                    {
                        PressKey(Keys.Right);
                        PressKey(Keys.Q);
                    }
                    else if (screen5Commands.Contains(said))
                    {
                        PressKey(Keys.Tab);
                        Thread.Sleep(50);
                        for (int i = 0; i < 30; i++)
                            PressKey(Keys.Q);
                    }
                    else if (screenCommands.Contains(said))
                    {
                        for (int i = 0; i < 30; i++)
                            PressKey(Keys.Tab);
                    }
                         */
                    #endregion
                    else if (mainCommands.Contains(said))
                    {
                        if (said == "stop listening")
                        {
                            enabled = false;
                            Say("Speech Recognition Disabled");
                            AddToList("Speech Recognition Disabled");
                            Title = title + " - Disabled";
                        }
                        else if (said == "I promise to be good")
                        {
                            if (!File.Exists(DocsPath + @"\goodboy"))
                            {
                                File.Create(DocsPath + @"\goodboy");
                            }
                            easterEggEnabled = false;
                        }
                    }
                    else if (easterEggCommands.Contains(said))
                    {
                        if (easterEggEnabled) PressKey(Keys.Back);
                    }
                    else
                    {
                        Command cmd = commandsList.Find(c => c.VoiceCommands.Contains(said));
                        AddToList("Running '" + cmd.Name + "'");
                        foreach (string buttonAction in cmd.Buttons)
                        {
                            string[] parts = buttonAction.Split(new[] {' '});
                            int endIndex = 1;
                            string btn = "";
                            for (int i = 1; i < parts.Length; i++)
                            {
                                btn += parts[i];
                                if (parts[i].EndsWith("'"))
                                {
                                    endIndex = i;
                                    break;
                                }
                                btn += " ";
                            }
                            btn = btn.Replace("'", "");
                            bool press = parts[0] == "Press";
                            int duration = Convert.ToInt32(parts[endIndex + 2]);
                            int repeats = 1;
                            if (!press) repeats = Convert.ToInt32(parts[endIndex + 4]);
                            Keys keyToPress = Keys.None;
                            // "Pass (A/Cross)", "Shoot (X/Square)", "Fake Pass (B/Circle)", "Post Up (Y/Triangle)", "Positional Plays (LB/L1)", "Icon Pass (RB/R1)",
                            //           "Clutch (LT/L2)", "Sprint (RT/R2)", "D-Pad Up", "D-Pad Down", "D-Pad Left", "D-Pad Right"
                            switch (btn)
                            {
                                case "Pass (A/Cross)":
                                    keyToPress = Keys.S;
                                    break;
                                case "Shoot (X/Square)":
                                    keyToPress = Keys.A;
                                    break;
                                case "Fake Pass (B/Circle)":
                                    keyToPress = Keys.D;
                                    break;
                                case "Post Up (Y/Triangle)":
                                    keyToPress = Keys.W;
                                    break;
                                case "Positional Plays (LB/L1)":
                                    keyToPress = Keys.Tab;
                                    break;
                                case "Icon Pass (RB/R1)":
                                    keyToPress = Keys.R;
                                    break;
                                case "Clutch (LT/L2)":
                                    keyToPress = Keys.Q;
                                    break;
                                case "Sprint (RT/R2)":
                                    keyToPress = Keys.E;
                                    break;
                                case "D-Pad Up":
                                    keyToPress = Keys.Up;
                                    break;
                                case "D-Pad Down":
                                    keyToPress = Keys.Down;
                                    break;
                                case "D-Pad Left":
                                    keyToPress = Keys.Left;
                                    break;
                                case "D-Pad Right":
                                    keyToPress = Keys.Right;
                                    break;
                                case "Call Timeout (Back/Select)":
                                    keyToPress = Keys.Back;
                                    break;
                            }
                            if (press)
                            {
                                if (keyToPress != Keys.None) PressKey(keyToPress, duration);
                            }
                            else
                            {
                                if (keyToPress!=Keys.None)
                                {
                                    for (int i = 0; i < repeats; i++)
                                    {
                                        PressKey(keyToPress, duration);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (mainCommands.Contains(said))
                    {
                        if (said == "start listening")
                        {
                            enabled = true;
                            Say("Speech Recognition Enabled");
                            AddToList("Speech Recognition Enabled");
                            Title = title + " - Enabled";
                        }
                    }
                }
            }
        }

        private void AddToList(string s)
        {
            lstMessages.Items.Insert(0, DateTime.Now.ToShortTimeString() + ": " + s);
        }

        private Keys CharToKeys(char c)
        {
            return (Keys) char.ToUpper(c);
        }

        private void PressChar(char c, int timeInMS = 50)
        {
            Keys k = CharToKeys(c);
            PressKey(k, timeInMS);
        }

        private static void PressKey(Keys k, int timeInMS = 50)
        {
            InputManager.Keyboard.KeyDown(k);
            Thread.Sleep(timeInMS);
            InputManager.Keyboard.KeyUp(k);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Process p;
            try
            {
                p = Process.GetProcessesByName("nba2k12")[0];
            }
            catch (Exception)
            {
                return;
            }

            IntPtr hwnd = p.MainWindowHandle;
            ShowWindowAsync(hwnd, 3);
            SetForegroundWindow(hwnd);
        }

        private void btnAddCommand_Click(object sender, RoutedEventArgs e)
        {
            CommandsWindow cw = new CommandsWindow();
            cw.ShowDialog();

            commandsList.ForEach(c => c.VoiceCommands.ForEach(cmd => allVoiceCommands.Remove(cmd)));
            parseCommands();
        }
    }
}
