#NoEnv  ; Recommended for performance and compatibility with future AutoHotkey releases.
; #Warn  ; Enable warnings to assist with detecting common errors.
SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.

craft()

pressEsc() {
	if (WinExist("ahk_class FFXiClass")) {
		if (WinActive("ahk_class FFXiClass") == 0) {
			WinActivate
			Sleep 500
			SendEvent {ESC}
		}
		else {
			SendEvent {ESC}
		}
	}
}

pressEnter() {
	if (WinExist("ahk_class FFXiClass")) {
		if (WinActive("ahk_class FFXiClass") == 0) {
			WinActivate
			Sleep 500
			SendEvent {Enter}
		}
		else {
			SendEvent {Enter}
		}
	}
}

craft() {
	PressEsc()
	Sleep 500
	PressEnter()
	Sleep 500
	PressEnter()
	Sleep 500
	PressEnter()
	Sleep 500
	PressEnter()
}