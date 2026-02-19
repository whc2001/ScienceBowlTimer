# Science Bowl Timer

A quick vibe-coded desktop timer application for Science Bowl competitions.

## Usage

- This application expects dual-screen for optimal usage. With two screens attached in extended mode, one will display the public facing big-screen and another will display a control panel optimized for touch-screen operation.

- The big-screen will display the current half type (first/second) and remaining time, as well as the current question type and countdown.

- If the two screens are not on the correct display, click the "Swap" button on the bottom right of the control panel to swap the screens.

- The application can also be controlled via hotkeys, which is assignable by editing the `hotkeys.json` file in the application directory (also support combination keys e.g. "Ctrl+Alt+Shift+A"). By default, the hotkeys are set as follows:
  
	- F1: Start first half
	- F2: Start second half
	- F3: Stop half timer

	- F5: Start toss-up question
	- F6: Start bonus question
	- F7: Restart last question type
	- F8: Stop question timer

- If only one screen is detected, the application will run in single-screen mode. In this mode only the big-screen will be displayed, and the control panel will be hidden. The application can only be controlled via hotkeys in this mode.

- Press Alt+F4 or click the "Exit" button on the bottom right of the control panel to exit the application.

- If "Time.wav", "FiveSeconds.wav" and "HalfFinished.wav" are placed in the application directory (there are example voices inside the project directory and will be copied automatically when building), they will be played when for corresponding events. The OS volume will be automatically set to 100% each time an audio is played. Adjust the output volume through external equipments if necessary.

## AI Usage Appendix

The entire project is done by vibe-coding. Zero bytes of human output are in this repository.
