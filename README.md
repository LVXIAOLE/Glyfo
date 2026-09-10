# Glyfo

**Turns any text on your screen into text you can use — entirely on your machine, with no network
connections.**

[![License: MIT](https://img.shields.io/badge/License-MIT-informational.svg)](LICENSE)
[![Platform: Windows 10 19041+](https://img.shields.io/badge/Windows-10%2019041%2B%20·%20x64%20%7C%20ARM64-informational.svg)](#requirements)

[![Get it from Microsoft Store](https://get.microsoft.com/images/en-us%20dark.svg)](https://apps.microsoft.com/detail/9PMC03HX113W?cid=github)

![Glyfo reading text out of a screenshot](docs/store-screenshots/01-text-from-a-page.png)

Press <kbd>Alt</kbd>+<kbd>Z</kbd>, frame part of the screen, and the text is beside it — ready to
copy, hear read aloud, or clean up. Point it at a file instead and the same thing happens to an
image, a scanned form, a whole PDF, or a folder of them in one pass.

Free. No ads, no in-app purchases, no account. [Homepage](https://lvxiaole.github.io/glyfo-site/?cid=github)

## Glyfo makes no network connections

Not "your data is safe with us" — there is no server to send it to. Recognition runs on the Windows
OCR engine that is already on your machine. On a Copilot+ PC, Glyfo additionally uses the on-device
text-recognition model for difficult images and can translate the result, still without a network.

You don't have to take that on trust. In increasing order of effort: read the source in this
repository — there is no HTTP client in it; check the declared capabilities in the MSIX manifest
before you install; or block Glyfo outbound in Windows Firewall and watch every feature keep working.

## Getting an image in

| | |
|---|---|
| <kbd>Alt</kbd>+<kbd>Z</kbd> | Frame any part of the screen. <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>R</kbd> takes the whole screen. |
| <kbd>Ctrl</kbd>+<kbd>V</kbd> | Paste an image from the clipboard — or text, which works too. |
| <kbd>Ctrl</kbd>+<kbd>O</kbd> | Open an image or a PDF, or drag one into the window. |
| Drop a folder | Every image in it is recognized in one batch. |
| Share | Right-click an image in Explorer and open it with Glyfo, or send it from the Windows share sheet (Photos, Snipping Tool, your browser). |

A PDF can be taken a page at a time or all at once, and saved as `.txt` or `.md` — one file, or one
file per page.

## What you get back

- The recognized text beside the image, in the order it was laid out.
- One-click copy, or let a capture copy itself the moment it is ready.
- Read aloud with any voice installed on the machine.
- **Remove line breaks** puts paragraphs back together; **Remove spaces** strips the gaps that
  Chinese, Japanese and Korean text picks up during recognition.
- QR codes and barcodes read from the same image.
- Rotate and deskew a crooked scan before recognizing it.
- Translation on Copilot+ PCs, on-device.
- Searchable history that survives a restart, so a capture from last week is still there.
- Runs from the tray — close the window and the hotkey keeps working.

## Doesn't PowerToys already do this?

PowerToys Text Extractor grabs text from a screen region, and it is good at that. Glyfo also takes
files, PDFs and whole folders; reads QR codes and barcodes; speaks the result aloud; translates it;
keeps a history; unwraps and de-spaces the text; and its own interface is localized into 33
languages, picked up from your Windows language setting on first run.

Windows 11's Snipping Tool has text actions too. Windows 10 has nothing built in.

## What it doesn't do

- It gives you the **text**. It does not write a text layer back into a PDF, so you get the words,
  not a searchable version of the original. For that, use [OCRmyPDF](https://github.com/ocrmypdf/OCRmyPDF).
- Recognition quality is the Windows engine's. Good on clean print, ordinary on bad photos,
  unreliable on handwriting.
- Which recognition languages work depends on the OCR packs Windows has installed, under
  Settings → Time & language → Language & region. That is separate from the 33 interface languages.
- Tables come back as text in reading order, not as structured tables.
- The Microsoft Store build is the one that is signed and supported; there is no standalone
  installer. Build it yourself from here if you'd rather.

## Requirements

- Windows 10 version 2004 (build 19041) or later, or Windows 11.
- x64 or ARM64.
- Translation and the enhanced-accuracy recognition model require a **Copilot+ PC**. Everything else
  works on any supported machine.

## Building

Visual Studio 2022 or later with the **Windows application development** workload, and the .NET 8 SDK.
Open `Glyfo.slnx`, set `Glyfo (Package)` as the startup project, and run.

The listing copy for all 33 Store languages lives in [`docs/store-listing.md`](docs/store-listing.md),
and the scripts under [`tools/`](tools) turn it into a Partner Center listings CSV.

## Contributing

Bug reports and feature requests are welcome in [Issues](https://github.com/LVXIAOLE/Glyfo/issues).
For a pull request, open an issue first if it is more than a small fix — the app ships through the
Microsoft Store, so changes have to survive certification.

Recognition itself is `Windows.Media.Ocr`; the interesting code is the layout reconstruction that
turns its lines-and-bounding-boxes output into text you can actually paste.

## License

[MIT](LICENSE). © 2026 Le Lv.
