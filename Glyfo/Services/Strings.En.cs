using System;
using System.Collections.Generic;

namespace Glyfo.Services;

/// <summary>
/// The interface strings, one table per language. English is the canonical set: every key exists
/// here, and <see cref="Loc.Get(string)"/> falls back to this table for anything a translation is
/// missing.
/// </summary>
internal static partial class Strings
{
    internal static readonly Dictionary<string, string> En = new(StringComparer.Ordinal)
    {
        // Engine status, shown next to the title
        ["Engine_NoLanguagePack"] = "Windows OCR · no language pack",
        ["Engine_OneLanguage"] = "Windows OCR · 1 language",
        ["Engine_ManyLanguages"] = "Windows OCR · {0} languages",
        ["Engine_OnDevice"] = "{0} · on-device",

        // Image pane
        ["Preview_Placeholder"] = "Drop an image here, or use File, Capture or Clipboard below",
        ["Btn_CopyImage"] = "Copy",
        ["Tip_CopyImage"] = "Copy the image to the clipboard",
        ["Btn_SaveImage"] = "Save",
        ["Tip_SaveImage"] = "Save the image as…",
        ["Btn_FitToWindow"] = "Fit",
        ["Tip_FitToWindow"] = "Zoom to fit the window",
        ["Tip_ActualSize"] = "Actual pixel size",
        ["Tip_PdfPrev"] = "Previous page",
        ["Tip_PdfNext"] = "Next page",

        // Smart actions. Verbs, because they are what the button does — the button's own label is
        // the address it found.
        ["Action_Open"] = "Open link",
        ["Action_Mail"] = "Write to",
        ["Action_Call"] = "Call",
        ["Action_Search"] = "Search the web for",

        // Text pane
        ["Voice_Placeholder"] = "Voice",
        ["Tip_Voice"] = "Voice used for reading aloud",
        ["Tip_Speak"] = "Read the text aloud",
        ["Tip_StopSpeak"] = "Stop reading",
        ["Tip_CopyText"] = "Copy the text (Ctrl+Shift+C)",
        ["Btn_RemoveLineBreaks"] = "Unwrap",
        ["Tip_RemoveLineBreaks"] = "Merge hard line breaks back into paragraphs",
        ["Btn_RemoveSpaces"] = "No spaces",
        ["Tip_RemoveSpaces"] = "Remove every space, for Chinese, Japanese and Korean",
        ["Result_Placeholder"] = "The recognized text appears here, and can be edited.",

        // Action bar
        ["Btn_OpenFile"] = "File",
        ["Tip_OpenFile"] = "Open an image file (Ctrl+O)",
        ["Btn_Capture"] = "Capture",
        ["Tip_Capture"] = "Select a screen region and recognize it ({0}). Press Ctrl+Shift+R for the whole screen.",
        ["Hotkey_Unavailable"] = "no shortcut — another app has taken both",
        ["Btn_Paste"] = "Clipboard",
        ["Tip_Paste"] = "Load an image or text from the clipboard (Ctrl+V)",
        ["Btn_History"] = "History",
        ["Tip_History"] = "Recent results",
        ["History_Empty"] = "Nothing recognized yet.",
        ["History_EmptyPreview"] = "(empty)",
        ["Tip_Settings"] = "Options",
        ["Settings_Header"] = "Options",
        ["Setting_UiLanguage"] = "App language",
        ["Lang_SystemDefault"] = "System default",
        ["Setting_RepairNumbers"] = "Fix l and I inside numbers",
        ["Setting_RepairNumbers_Desc"] = "Recognizers routinely read v1.6.5 as vl.6.5. When this is on, an l or I becomes a 1 only where a separator and a digit sit beside it, so html5 and IPv6 are left alone. When it is off, the engine's own reading is kept exactly as it came back.",
        ["Setting_WatchClipboard"] = "Read pictures I copy",
        ["Setting_WatchClipboard_Desc"] = "Take a screenshot with Win+Shift+S and Glyfo reads it straight away, then puts the text on the clipboard in place of the picture, so the next paste is the text. Only pictures are read, never copied text, and nothing is sent anywhere — the whole thing happens on this device.",
        ["Common_On"] = "On",
        ["Common_Off"] = "Off",
        ["Lang_Placeholder"] = "Language",
        ["Tip_OcrLanguage"] = "Recognition language, from the language packs Windows has installed",
        ["Btn_Translate"] = "Translate",
        ["Tip_Translate"] = "Translate on this device — nothing is sent over the network",
        ["Btn_Barcode"] = "Codes",
        ["Tip_Barcode"] = "Read QR codes and barcodes in the image",
        ["Btn_Recognize"] = "Recognize",
        ["Tip_Recognize"] = "Recognize the text in the image (F5)",

        // Recognition-language choices the app adds itself
        ["Option_AutoAi"] = "Automatic (Windows AI)",
        ["Option_AutoMulti"] = "Automatic (multi-language · slower)",

        // Status messages
        ["Status_Loaded"] = "Loaded {0}.",
        ["Status_OpenFailed"] = "Could not open the image: {0}",
        ["Status_ClipboardText"] = "Loaded text from the clipboard.",
        ["Status_ClipboardEmpty"] = "The clipboard holds no image and no text.",
        ["Status_ClipboardFailed"] = "Could not read the clipboard: {0}",
        ["Status_DropFailed"] = "Could not use what was dropped: {0}",
        ["Status_ImageCopied"] = "Image copied to the clipboard.",
        ["Status_ImageCopyFailed"] = "Could not copy the image: {0}",
        ["Status_ImageSaved"] = "Image saved as {0}.",
        ["Status_ImageSaveFailed"] = "Could not save the image: {0}",
        ["Status_Recognizing"] = "Recognizing…",
        ["Status_NoText"] = "{0} found no text in this image.",
        ["Status_Done"] = "Done ({0}).",
        ["Status_DoneConfidence"] = "Done ({0}) · average confidence {1}%.",
        ["Status_RecognizeFailed"] = "Recognition failed: {0}",
        ["Status_Scanning"] = "Looking for QR codes and barcodes…",
        ["Status_NoBarcode"] = "No QR code or barcode found in this image.",
        ["Status_BarcodeOne"] = "Found one {0} code.",
        ["Status_BarcodeMany"] = "Found {0} codes.",
        ["Status_ScanFailed"] = "The scan failed: {0}",
        ["Status_PdfFailed"] = "this PDF could not be read — it may be password-protected or damaged",
        ["Status_LaunchFailed"] = "Windows had nothing to open that with.",
        ["Status_NothingToTranslate"] = "There is no text to translate.",
        ["Status_Translating"] = "Translating into {0} on this device…",
        ["Status_Translated"] = "Translated into {0}, on this device, with no network access.",
        ["Status_TranslateFailed"] = "Translation failed: {0}",
        ["Status_NothingToCopy"] = "There is no text to copy.",
        ["Status_TextCopied"] = "Text copied to the clipboard.",
        ["Status_CopyFailed"] = "Could not copy the text: {0}",
        ["Status_NothingToSpeak"] = "There is no text to read aloud.",
        ["Status_SpeakFailed"] = "Could not read the text aloud: {0}",
        ["Status_HistoryLoaded"] = "Showing the result from {0}.",
        ["Status_NeedImage"] = "Load an image first.",
        ["Status_CaptureFailed"] = "The screen capture failed: {0}",
        ["Status_StartupBlockedByUser"] = "Windows has Glyfo switched off for startup. Turn it back on under Startup apps in Task Manager.",
        ["Status_StartupBlockedByPolicy"] = "Your organization does not allow apps to start with Windows.",
        ["Status_LanguagePackMissing"] = "Windows has no OCR language pack installed, so there is nothing to recognize with. Add the language you need under Time & language, then Language & region.",
        ["Btn_OpenLanguageSettings"] = "Open language settings",

        // Notification-area icon, its menu, and the toasts raised while the window is hidden
        ["Tray_Open"] = "Open Glyfo",
        ["Tray_CaptureRegion"] = "Capture a region",
        ["Tray_CaptureFullScreen"] = "Capture the whole screen",
        ["Tray_Exit"] = "Exit",
        ["Tray_Tooltip"] = "Glyfo — press {0} to capture a region",
        ["Toast_StartupTitle"] = "Glyfo is running",
        ["Toast_StartupBody"] = "Press {0} to select part of the screen and read the text in it. Ctrl+Shift+R takes the whole screen.",
        ["Toast_DontRemind"] = "Don't remind me",
        ["Toast_MinimizedTitle"] = "Glyfo is still running",
        ["Toast_MinimizedBody"] = "Press {0} any time to capture and recognize. To close it for good, right-click its icon in the notification area and choose Exit.",
        ["Toast_ExitApp"] = "Exit Glyfo",
        ["Toast_ResultTitle"] = "Text recognized",
        ["Toast_ResultCopied"] = "Copied to the clipboard.",
        ["Toast_OpenWindow"] = "Open window",
        ["Toast_NoTextTitle"] = "No text found",
        ["Toast_NoTextBody"] = "Nothing in that area could be read as text.",
        ["Toast_FailedTitle"] = "Recognition failed",
        ["Toast_HotkeyUnavailableTitle"] = "The capture shortcut is taken",
        ["Toast_HotkeyUnavailableBody"] = "Another app already holds it, so screen capture is only available from the window and from the notification area icon.",
        ["Setting_CloseToTray"] = "Keep running when the window is closed",
        ["Setting_CloseToTray_Desc"] = "When this is on, closing the window leaves Glyfo running so the capture shortcut keeps working, and its icon stays in the notification area — Exit there closes it for good. When it is off, closing the window ends the app.",
        ["Setting_Startup"] = "Start with Windows",
        ["Setting_Startup_Desc"] = "When this is on, Glyfo starts with Windows and goes straight to the notification area without opening a window, so the capture shortcut works from the moment you sign in.",

        // Where an image came from — used in the history list and in status text
        ["Source_Clipboard"] = "the clipboard image",
        ["Source_Dropped"] = "the dropped image",
        ["Source_Shared"] = "the shared image",
        ["Source_FullScreen"] = "Full screen",
        ["Source_Region"] = "Screen region",
        ["Source_Codes"] = "{0} (codes)",

        ["FileType_Image"] = "{0} image",

        // Region-capture overlay
        ["Region_Hint"] = "Drag to select an area　·　Esc or right-click to cancel",

        // Failures raised from the services
        ["Err_NoLanguagePack"] = "Windows has no OCR language pack installed for this language. Add one under Time & language, then Language & region.",
        ["Err_TextTooLong"] = "The text is too long for the on-device model. Translate a smaller piece of it.",
        ["Err_ModelDeclined"] = "The on-device model declined to translate this text.",
        ["Err_TranslationFailed"] = "On-device translation did not produce a result.",
        // About, the release notes, and the rating prompt
        ["Btn_About"] = "About Glyfo",
        ["About_Title"] = "About Glyfo",
        ["About_Version"] = "Version {0}",
        ["About_Tagline"] = "Reads the text in any image, on this device. Nothing is sent over the network.",
        ["About_Rate"] = "Rate Glyfo",
        ["About_WhatsNew"] = "What's new",
        ["About_Source"] = "Source code on GitHub",
        ["About_Privacy"] = "Privacy policy",
        ["Common_Close"] = "Close",
        ["News_Title"] = "What's new",
        ["News_120_1"] = "Take a screenshot with Win+Shift+S and Glyfo reads it for you, then leaves the text on the clipboard. Off until you turn it on, in Settings.",
        ["News_120_2"] = "PDFs open like images now, a page at a time.",
        ["News_120_3"] = "Links, email addresses and phone numbers found in the text get a button under it.",
        ["News_110_1"] ="An About page, with the version and links to the source code and the privacy policy.",
        ["News_110_2"] = "You can rate Glyfo without leaving the window.",
        ["News_110_3"] = "These notes, shown once for each new version.",
        ["News_110_4"] = "Capture now offers the whole screen as well, and hides the window first so Glyfo is never in the shot.",
        ["Rate_Title"] = "Is Glyfo earning its place?",
        ["Rate_Body"] = "A rating in the Store is how other people find it, and it only takes a moment.",
        ["Rate_Action"] = "Rate it",
        ["Rate_Later"] = "Not now",
    };
}
