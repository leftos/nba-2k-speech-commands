NBA 2K Speech Recognition Tool
	by Lefteris "Leftos" Aslanoglou


	Overview
		This tool was built to provide Kinect-like Speech Recognition capabilities to the NBA 2K series, with a focus on NBA 2K12 and later. It uses Microsoft's Speech Recognition and the driver-level SendInput command to recognize user commands and emulate keyboard presses, resulting in in-game commands.


	First Time Setup
		The keyboard mapping will be adjustable in the future, but in this preview version you should remap the keyboard controls to the following:
		
		Pass - S
		Shoot - A
		Fake Pass - D
		Post Up - W
		Sprint - E
		Clutch - Q
		Positional Plays - Tab
		Icon Pass - R
		D-Pad Up - Up Arrow
		D-Pad Down - Down Arrow
		D-Pad Left - Left Arrow
		D-Pad Right - Right Arrow
		Call Timeout - Backspace


	Commands
		Visit the Add/Edit Commands window to see what you need to say for each command, and to also add and edit commands.


	Credits
		- Uses the InputManager library (http://www.codeproject.com/Articles/117657/InputManager-library-Track-user-input-and-simulate)


	Version History
		v0.3.1.1
			- Attempt to fix Speech Recognition Engine culture issue

		v0.3.1
			- Minor bug fixes

		v0.3
			- 'Call Timeout/Intentional Foul' (usually Back/Select) has been added to the buttons available for commands
			- 'Call Timeout' and 'Intentional Foul' commands added

		v0.2
			- Users can now add and edit commands

		v0.1
			- Initial Release