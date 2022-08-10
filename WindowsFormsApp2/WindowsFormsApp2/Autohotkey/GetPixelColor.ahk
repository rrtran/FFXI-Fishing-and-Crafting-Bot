#NoEnv  ; Recommended for performance and compatibility with future AutoHotkey releases.
; #Warn  ; Enable warnings to assist with detecting common errors.
SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.

getColor() {
	if (WinExist("ahk_class FFXiClass")) {
		if (WinActive("ahk_class FFXiClass") == 0) {
			WinActivate
			PixelGetColor, color, 396, 317
			if (ErrorLevel == 0) {
				MsgBox %color%
			}
			else {
				MsgBox could not get color
			}
		}
	}
}

F1::getColor()
F2::ExitApp