#NoEnv  ; Recommended for performance and compatibility with future AutoHotkey releases.
; #Warn  ; Enable warnings to assist with detecting common errors.
SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.


if (WinExist("ahk_class FFXiClass")) {
	if (WinActive("ahk_class FFXiClass") == 0) {
		WinActivate
		Sleep 500
		craft()
	}
	else {
		craft()
	}
}

craft() {
	SendEvent {Enter}
	Sleep 500
	SendEvent {Enter}
	Sleep 500
	SendEvent {Enter}
	Sleep 500
	SendEvent {Enter}
}