#NoEnv  ; Recommended for performance and compatibility with future AutoHotkey releases.
; #Warn  ; Enable warnings to assist with detecting common errors.
SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.

; Call start fish function
fish()

; Function definitions
pressEsc() {
	if (WinExist("ahk_class FFXiClass")) {
		if (WinActive("ahk_class FFXiClass") == 0) {
			WinActivate
			SendEvent {ESC}
		}
		else {
			SendEvent {ESC}
		}
	}
}

pressF7() {
	if (WinExist("ahk_class FFXiClass")) {
		if (WinActive("ahk_class FFXiClass") == 0) {
			WinActivate
			SendEvent {F7}
		}
		else {
			SendEvent {F7}
		}
	}
}

pressDown() {
	if (WinExist("ahk_class FFXiClass")) {
		if (WinActive("ahk_class FFXiClass") == 0) {
			WinActivate
			SendEvent {down}
		}
		else {
			SendEvent {down}
		}
	}
}

pressEnter() {
	if (WinExist("ahk_class FFXiClass")) {
		if (WinActive("ahk_class FFXiClass") == 0) {
			WinActivate
			SendEvent {Enter}
		}
		else {
			SendEvent {Enter}
		}
	}
}

fish() {
	; pick fish in menu
	pressDown()
	Sleep 100
	pressDown()
	Sleep 100
	pressDown()
	Sleep 100
	pressDown()
	Sleep 100
	pressDown()
	Sleep 100
	pressDown()
	Sleep 100
	pressEnter()
}