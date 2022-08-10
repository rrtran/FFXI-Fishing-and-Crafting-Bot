#NoEnv  ; Recommended for performance and compatibility with future AutoHotkey releases.
; #Warn  ; Enable warnings to assist with detecting common errors.
SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.

if (WinExist("ahk_class FFXiClass")) {
	if (WinActive("ahk_class FFXiClass") == 0) {
		WinActivate
		Sleep 500
		equipBait()
	}
	else {
		equipBait()
	}
}

equipBait() {
	SendEvent {CTRL down}
	sleep 5000
	SendEvent {1 down}
	sleep 250
	SendEvent {CTRL up}
	SendEvent {1 up}
}
