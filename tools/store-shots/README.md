# Store screenshot harness

Produces the seven 1920×1080 PNGs in `docs/store-screenshots/`. They are taken off the real,
packaged app — not mocked up — so a screenshot cannot claim something the build does not do.

```powershell
powershell -ExecutionPolicy Bypass -File tools\store-shots\make-sources.ps1
powershell -ExecutionPolicy Bypass -File tools\store-shots\shoot.ps1
powershell -ExecutionPolicy Bypass -File tools\store-shots\trim-edges.ps1
powershell -ExecutionPolicy Bypass -File tools\store-shots\check-edges.ps1
copy tools\store-shots\out\*.png docs\store-screenshots\
```

| Script | What it does |
|---|---|
| `make-sources.ps1` | Draws the three images and writes the three-page PDF the shots are taken of, into `src/`. Committed as well, so a reshoot on another machine frames the same content even if the fonts differ. The PDF is assembled by hand rather than with a library — the repo has no PDF writer and did not need one for three pages of text — and the script re-opens it through `Windows.Data.Pdf` at the end to prove the byte offsets in its xref table are right. |
| `shoot.ps1` | Installs nothing and mocks nothing: it activates the installed package, drives the real controls through UI Automation and real mouse clicks, waits for recognition to settle, and grabs the window off the screen onto a 1920×1080 canvas. `-Only 02-any-language` re-takes one shot while still doing all the state setup around it. |
| `trim-edges.ps1` | Repairs the one-pixel hairline of desktop wallpaper around all four sides. The outermost ring of a Windows 11 window frame is semi-transparent, so a capture of the extended frame bounds composites whatever was behind it. |
| `check-edges.ps1` | Confirms the repair took. Should report `suspect edge pixels: 0` for all seven. |

The seven shots: a page of English text, a Chinese page, a crate label with a QR code and a barcode,
the history flyout with a search term typed into it, the settings dialog, a PDF open at page 1 of 3,
and the batch dialog part-way through reading all three pages.

## Notes for whoever reshoots these

**File activation.** A packaged app cannot be handed a file by starting its executable, so the
harness opens each source through the file association: `ShellExecuteExW` with `SEE_MASK_CLASSNAME`
and the shell-generated progid found under `HKCU:\Software\Classes\<ext>\OpenWithProgids` by matching
`AppUserModelID` against `LVLE.Glyfo*`. The calling process has to stay alive for a few seconds
afterwards or the launched instance fails over RPC.

**The batch dialog is reached through the PDF**, not a file picker: `PdfAllButton` reads every page
of the open document, which puts three entries in the dialog without a modal picker in the way.
`Wait-Element` watches for `BatchSeparate` — the "save each one as its own file" checkbox, which is
collapsed until the run finishes and so is simply absent from the UI Automation tree until then.

**Dialogs are closed by invoking their close button, never with Esc.** Changing the interface
language rebuilds the settings dialog, and a key press afterwards goes wherever focus landed — which
once left the dialog up for the whole run, dimming four shots that were saved anyway because nothing
threw. `Save-Shot` now refuses to take a shot while a dialog is on screen unless it was asked for one
(`-WithDialog`), so that failure cannot repeat silently.

Detecting an open `ContentDialog` is less obvious than it looks: it puts *two* popups in the tree,
and the one holding the dialog content reports `IsOffscreen` with an empty rectangle while plainly
visible. The trustworthy signal is the other one — the layer that dims the window behind — which is
why `Test-DialogOpen` looks for a popup at least 80% of the window's width. Flyouts and combo
drop-downs are popups too, and none of them come close to filling the window.

`Get-DialogCloseButton` filters its matches rather than taking the first: `CloseButton` is the
`ContentDialog` template's name for its close button *and* the name the status bar's `InfoBar` gives
its dismiss "x", which is on screen for most of the run.

**Combo boxes** only realise their rows while the drop-down is expanded, so `Get-ComboItems` expands
before reading or writing and hands the pattern back for the caller to collapse. The interface
language is captured and restored *by index*, from inside the open settings dialog: it is not in the
tree while the dialog is closed, and the list is rewritten in the new language every time it changes,
so the name read at the start matches nothing by the end.

`shoot.ps1` moves the real cursor and clicks where UI Automation says the controls are, so run it
with nothing else on screen and keep your hands off the mouse until it finishes. It reads the
interface and recognition language at the start and puts them back at the end.

The captions that go with these images, in ten languages, are in `docs/store-listing.md`.

One thing the harness cannot fix: the read-aloud voice list shows whatever voices Windows has, and
`SpeechSynthesizer.AllVoices` only sees the ones registered under
`HKLM\SOFTWARE\Microsoft\Speech_OneCore\Voices\Tokens`. On a machine with no English voice there,
the English shots show a Chinese voice name. Add English (United States) with the speech option
under Settings, Time & language, Language & region before reshooting.
