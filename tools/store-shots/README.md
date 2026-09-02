# Store screenshot harness

Produces the five 1920×1080 PNGs in `docs/store-screenshots/`. They are taken off the real,
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
| `make-sources.ps1` | Draws the three images the shots are taken of, into `src/`. Committed as well, so a reshoot on another machine frames the same content even if the fonts differ. |
| `shoot.ps1` | Installs nothing and mocks nothing: it activates the installed package, drives the real controls through UI Automation and real mouse clicks, waits for recognition to settle, and grabs the window off the screen onto a 1920×1080 canvas. `-Only 02-any-language` re-takes one shot while still doing all the state setup around it. |
| `trim-edges.ps1` | Repairs the one-pixel hairline of desktop wallpaper down the sides and across the top. The outermost ring of a Windows 11 window frame is semi-transparent, so a capture of the extended frame bounds composites whatever was behind it. |
| `check-edges.ps1` | Confirms the repair took. Should report `suspect edge pixels: 0` for all five. |

`shoot.ps1` moves the real cursor and clicks where UI Automation says the controls are, so run it
with nothing else on screen and keep your hands off the mouse until it finishes. It reads the
interface and recognition language at the start and puts them back at the end — except that the
interface language reads back as empty and is left in English, so set it again afterwards.

The captions that go with these images, in ten languages, are in `docs/store-listing.md`.

One thing the harness cannot fix: the read-aloud voice list shows whatever voices Windows has, and
`SpeechSynthesizer.AllVoices` only sees the ones registered under
`HKLM\SOFTWARE\Microsoft\Speech_OneCore\Voices\Tokens`. On a machine with no English voice there,
the English shots show a Chinese voice name. Add English (United States) with the speech option
under Settings, Time & language, Language & region before reshooting.
