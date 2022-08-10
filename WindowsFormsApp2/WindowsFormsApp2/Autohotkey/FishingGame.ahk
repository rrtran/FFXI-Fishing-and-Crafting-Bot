#NoEnv  ; Recommended for performance and compatibility with future AutoHotkey releases.
; #Warn  ; Enable warnings to assist with detecting common errors.
SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.

badFeelingImages := Array()
badFeelingImages.Push(".\images\1920_1080-bad-feeling-1.png")
badFeelingImages.Push(".\images\1920_1080-bad-feeling-2.png")
badFeelingImages.Push(".\images\1920_1080-bad-feeling-3.png")
badFeelingImages.Push(".\images\1920_1080-bad-feeling-4.png")
badFeelingImages.Push(".\images\1920_1080-bad-feeling-5.png")
badFeelingImages.Push(".\images\1920_1080-bad-feeling-6.png")
badFeelingImages.Push(".\images\1920_1080-bad-feeling-7.png")
badFeelingImages.Push(".\images\1920_1080-bad-feeling-8.png")
badFeelingImages.Push(".\images\1600_900-bad-feeling-1.png")
badFeelingImages.Push(".\images\1600_900-bad-feeling-2.png")
badFeelingImages.Push(".\images\1600_900-bad-feeling-3.png")
badFeelingImages.Push(".\images\1600_900-bad-feeling-4.png")
badFeelingImages.Push(".\images\1600_900-bad-feeling-5.png")
badFeelingImages.Push(".\images\1600_900-bad-feeling-6.png")
badFeelingImages.Push(".\images\1600_900-bad-feeling-7.png")
badFeelingImages.Push(".\images\1600_900-bad-feeling-8.png")
badFeelingImages.Push(".\images\1280_720-bad-feeling-1.png")
badFeelingImages.Push(".\images\1280_720-bad-feeling-2.png")
badFeelingImages.Push(".\images\1280_720-bad-feeling-3.png")
badFeelingImages.Push(".\images\1280_720-bad-feeling-4.png")
badFeelingImages.Push(".\images\1280_720-bad-feeling-5.png")
badFeelingImages.Push(".\images\1280_720-bad-feeling-6.png")
badFeelingImages.Push(".\images\1280_720-bad-feeling-7.png")
badFeelingImages.Push(".\images\1280_720-bad-feeling-8.png")
badFeelingImages.Push(".\images\1366_768-bad-feeling-1.png")
badFeelingImages.Push(".\images\1366_768-bad-feeling-2.png")
badFeelingImages.Push(".\images\1366_768-bad-feeling-3.png")
badFeelingImages.Push(".\images\1366_768-bad-feeling-4.png")
badFeelingImages.Push(".\images\1366_768-bad-feeling-5.png")
badFeelingImages.Push(".\images\1366_768-bad-feeling-6.png")
badFeelingImages.Push(".\images\1366_768-bad-feeling-7.png")
badFeelingImages.Push(".\images\1366_768-bad-feeling-8.png")
badFeelingImages.Push(".\images\1680_1050-bad-feeling-1.png")
badFeelingImages.Push(".\images\1680_1050-bad-feeling-2.png")
badFeelingImages.Push(".\images\1680_1050-bad-feeling-3.png")
badFeelingImages.Push(".\images\1680_1050-bad-feeling-4.png")
badFeelingImages.Push(".\images\1680_1050-bad-feeling-5.png")
badFeelingImages.Push(".\images\1680_1050-bad-feeling-6.png")
badFeelingImages.Push(".\images\1680_1050-bad-feeling-7.png")
badFeelingImages.Push(".\images\1680_1050-bad-feeling-8.png")

if (WinExist("ahk_class FFXiClass")) {
	if (WinActive("ahk_class FFXiClass") == 0) {
		WinActivate ; Use the window found by WinExist.
		WinGetPos, X, Y, width, height, A
		Sleep 1000
		i := 1
		while (i <= badFeelingImages.Length()) {
			badFeelingImage := badFeelingImages[i]
			ImageSearch, FoundX, FoundY, 0, height * .95, width * .5, height, *30 %badFeelingImage%
			if (ErrorLevel == 0) {
				pressEsc()
				return
			}
			i++
		}
		doFishingGame()
		return
	}
	else {
		WinGetPos, X, Y, width, height, A
		Sleep 1000
		i := 1
		while (i <= badFeelingImages.Length()) {
			badFeelingImage := badFeelingImages[i]
			ImageSearch, FoundX, FoundY, 0, height * .95, width * .5, height, *30 %badFeelingImage%
			if (ErrorLevel == 0) {
				pressEsc()
				return
			}
			i++
		}
		doFishingGame()
		return
	}
}

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

press_a() {
	if (WinExist("ahk_class FFXiClass")) {
		if (WinActive("ahk_class FFXiClass") == 0) {
			WinActivate ; Use the window found by WinExist.
			SendEvent {a}
		}
		else {
			SendEvent {a}
		}
	}	
}

press_d() {
	if (WinExist("ahk_class FFXiClass")) {
		if (WinActive("ahk_class FFXiClass") == 0) {
			WinActivate ; Use the window found by WinExist.
			SendEvent {d}
		}
		else {
			SendEvent {d}
		}
	}	
}

doFishingGame() 
{
	WinGetPos, X, Y, width, height, A
	PixelSearch, Px, Py, width / 3, 0, width - (width / 3), height / 2, 0x9999FF, 3, Fast
	while (ErrorLevel == 0) {
		if (WinActive("ahk_class FFXiClass") == 0) {
			WinActivate
			PixelSearch, Px, Py, width * .21, height * .29, width * .37, height * .53, 0x5FC1F6, 3, Fast ; left gold arrow
			if (ErrorLevel == 0) {
				press_a()
			}
			PixelSearch, Px, Py, width * .6, height * .29, width * .8, height * .53, 0x5FC1F6, 3, Fast ; right gold arrow
			if (ErrorLevel == 0) {
				press_d()
			}
			PixelSearch, Px, Py, width * .21, height * .29, width * .37, height * .53, 0xDBB9A3, 3, Fast ; left silver arrow
			if (ErrorLevel == 0) {
				press_a()
			}
			PixelSearch, Px, Py, width * .6, height * .29, width * .8, height * .53, 0xDBB9A3, 3, Fast ; right silver arrow
			if (ErrorLevel == 0) {
				press_d()
			}
			PixelSearch, Px, Py, width / 3, 0, width - (width / 3), height / 2, 0x9999FF, 3, Fast
		}
		else {
			PixelSearch, Px, Py, width * .21, height * .29, width * .37, height * .53, 0x5FC1F6, 3, Fast ; left gold arrow
			if (ErrorLevel == 0) {
				press_a()
			}
			PixelSearch, Px, Py, width * .6, height * .29, width * .8, height * .53, 0x5FC1F6, 3, Fast ; right gold arrow
			if (ErrorLevel == 0) {
				press_d()
			}
			PixelSearch, Px, Py, width * .21, height * .29, width * .37, height * .53, 0xDBB9A3, 3, Fast ; left silver arrow
			if (ErrorLevel == 0) {
				press_a()
			}
			PixelSearch, Px, Py, width * .6, height * .29, width * .8, height * .53, 0xDBB9A3, 3, Fast ; right silver arrow
			if (ErrorLevel == 0) {
				press_d()
			}
			PixelSearch, Px, Py, width / 3, 0, width - (width / 3), height / 2, 0x9999FF, 3, Fast
		}
	}
	Send {enter}
}