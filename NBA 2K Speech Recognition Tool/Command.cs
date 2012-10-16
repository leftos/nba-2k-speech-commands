using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBA_2K_Speech_Recognition_Tool
{
    class Command
    {
        public string Name;
        public List<string> Buttons;
        public List<string> VoiceCommands;

        public Command()
        {
            Buttons = new List<string>();
            VoiceCommands = new List<string>();
        }
    }
}
