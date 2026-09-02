# Notes for certification

Ready-to-paste text for the Partner Center **提交选项 / Submission options** page, 认证说明 field.

Written for one reader: the person who installs the package, opens it, and has to decide within a
few minutes whether it does what the listing says. Glyfo makes that harder than most apps do,
because its main feature is behind a global shortcut and the window it opens is deliberately empty —
so the first thing the note does is tell them how to make the app show its work.

The claims below are checkable, and are meant to be: no network capability is declared, so the
reviewer can watch the process make no connections. Keep them true if the app changes.

---

Glyfo needs no account, no sign-in and no test credentials. It has no server, and it declares no
network capability at all, so nothing it does can leave the machine.

WHY THE WINDOW LOOKS EMPTY

Glyfo opens as two empty panes — the picture on the left, the text found in it on the right. That is
the resting state; there is nothing to show until it is given an image. Any one of these exercises
the whole app:

1. The Capture button in the bottom bar, or Alt+Z from anywhere in Windows, even with the window
   closed. The screen dims; drag a box around any part of it — a web page, a PDF, a video still —
   and let go. The text appears on the right. Ctrl+Shift+R takes the whole screen with no selection
   step. If another app already owns Alt+Z, Glyfo registers Ctrl+Shift+G instead and the button's
   tooltip names whichever one it got.
2. The File button (Ctrl+O): any .png, .jpg, .jpeg, .bmp, .gif, .tif, .tiff or .webp.
3. The Clipboard button (Ctrl+V): copy a picture anywhere — Snipping Tool, a browser, Word — and
   press it.
4. Drag an image onto the window; or right-click one in File Explorer and choose Open with, Glyfo;
   or send a picture to Glyfo from the Windows share sheet, for example from the Photos app.

Recognition starts by itself. The result is editable text: copy it, have it read aloud, unwrap the
line breaks back into paragraphs, or strip the spaces for Chinese, Japanese and Korean. The Codes
button reads QR codes and barcodes out of the same picture. History reopens anything recognized
earlier in the session.

INTENDED BEHAVIOUR THAT MIGHT READ AS A DEFECT

- Closing the window does not quit. Glyfo keeps running in the notification area so the capture
  shortcut still works; a notification says so the first time it happens. To quit: right-click the
  tray icon, then Exit. The behaviour can be switched off under Options.
- Start with Windows is off. The manifest ships the startup task disabled and the user turns it on
  in Options, which is where the system prompt comes from.
- Recognition needs a Windows OCR language pack. On a machine with none installed, Glyfo says so in
  the status bar and offers a button that opens the Windows language settings, rather than failing
  quietly. Add one under Settings, Time & language, Language & region.
- Read aloud is unavailable when Windows has no speech voice installed, for the same reason.
- Translate is only present on Copilot+ PCs. It uses the on-device Windows AI translation model; on
  any other machine the button is hidden. It never goes online.
- The interface is available in thirty-three languages and follows the Windows display language on
  first run, falling back to English. It can be changed under Options, App language, without a
  restart.

DECLARED CAPABILITIES

- runFullTrust. Glyfo is a packaged Win32 desktop app (WinUI 3, .NET 8). It needs full trust for the
  four things a pure UWP app cannot do: register a system-wide hotkey (RegisterHotKey), capture the
  desktop image (BitBlt), put an icon in the notification area (Shell_NotifyIcon), and subclass its
  own window to receive WM_HOTKEY.
- systemAIModels. For the Windows AI APIs on Copilot+ PCs: Microsoft.Windows.Vision.TextRecognizer,
  which recognizes text more accurately than the classic engine, and the on-device translation model
  behind the Translate button. Both run locally on the device. Where they are unavailable Glyfo falls
  back to Windows.Media.Ocr and hides the Translate button, so the app is fully functional on an
  ordinary PC.

No internet capability is declared. Privacy policy: https://lvxiaole.github.io/glyfo-site/privacy.html
