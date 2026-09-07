# Store listing copy

Ready-to-paste text for the Partner Center **Store listings** page, one section per language.

The package declares thirty-three languages and all thirty-three have their own listing here. None
of them falls back to English: a customer browsing the Store in Thai or Hebrew reads the app
described in their own language, which is the whole point of shipping the UI in thirty-three.

Filling thirty-three listings by hand in the dashboard is not worth it, so the way in is the CSV:
export **Store listings** from Partner Center, run `tools/Fill-ListingCsv.ps1` over the export, and
import the result. That script reads *this file* — the sections below are the source, and the CSV
is generated from them, never the other way round. It leaves the three listings already written by
hand (en-us, zh-hant, zh-hans) alone and fills the other thirty from their own sections.

Each section has exactly five `###` subsections and the script keys them **by position**, not by
heading text — short description, description, ten `- ` features, five numbered captions, seven
backtick-quoted search terms. Translate the headings freely; do not reorder them, do not wrap a
feature across two lines, and keep the counts exact or the script throws.

Arabic, Hebrew and Persian carry no directional control characters. Every paragraph and bullet is
instead written to *begin* with a word in its own script, so first-strong bidi detection resolves
the whole run right-to-left on its own. That survives a CSV round trip; an invisible U+200F may not,
and cannot be proofread by eye.

| Partner Center field | Limit | Section below |
|---|---|---|
| 产品名称 / Product name | picked from reserved names | `Glyfo — OCR & Screen Capture` for every language |
| 说明 / Description | 10,000 characters | **说明** |
| 简短说明 / Short description | 1,000 characters | **简短说明** |
| 产品功能 / Product features | 20 entries, 200 characters each | **产品功能** |
| 屏幕截图 / Screenshots | up to 10 images, caption 200 characters each | **屏幕截图标题** |
| 搜索词 / Search terms | 7 terms, 30 characters each | **搜索词** |

The seven search terms are one intent each, in the same order in every language, so a missing or
duplicated intent is visible by reading down the column rather than by knowing the language:

1. picture → text, 2. screenshot → text, 3. **PDF → text**, 4. text recognition,
5. QR code, 6. barcode, 7. `OCR`.

Each is written in the phrasing that language's own users would type, not translated from the
English row — `文字识别` and `Texterkennung` are the same slot but neither is a rendering of the
other. Two rules follow from there being only seven slots. A term that repeats an intent another
term already covers is waste, which is why "copy text from image" is gone from English and
`免费OCR` / `無料OCR` / `무료 OCR` from the CJK rows: the price is already on the listing. And a
term has to be a query someone actually types with the intent of finding *this* app, which is why
"read aloud" is gone from all thirty-three — it is a real feature, but people searching it want a
screen reader, and the slot buys far more as PDF. `OCR` is kept in all thirty-three even though it
is also in the product name; whether Partner Center indexes title words is not documented either
way, and it is the single highest-volume query in the category, so dropping it is a gamble with no
upside. Every term must describe something the app really does — Store Policy 10.1 is checked at
certification.

The same seven PNGs go into every language listing — the Store does not share images between them,
and none of the seven has any text burned into it. Captions are per language. The images are in
`docs/store-screenshots/`; `tools/store-shots/` is what produced them, off the real packaged app.

The numbering below is the **shot** order — `01-…` through `07-…`, the file names — which is not
necessarily the slot order any listing is in. Slots can be dragged around in the dashboard, and have
been. `Fill-ListingCsv.ps1` moves the image cells into shot order before it writes these captions,
so the numbering here is what the CSV means and the dashboard's arrangement is not something this
file has to track.

What that reordering needs, and cannot get from the export, is en-us's own slot order. A screenshot
cell is a dashboard URL ending in an asset id; there is no file name anywhere in it, and the type
column says only "relative path (or URL to a Partner Center file)". So the export can say that two
listings show *the same* image, and that is what places the other thirty-two — they carry en-us's
asset ids — but nothing in it says *which* image that is. As long as en-us's assets are untouched
the answer can be recovered from its captions, since those were typed next to them by hand; the
moment en-us's PNGs are re-uploaded, every asset id is new, the captions stay behind in their old
slots, and the pairing is gone from the file entirely. That is why `Fill-ListingCsv.ps1` takes
`-EnSlotOrder`: it is one fact, it lives in the dashboard, and guessing it puts the QR caption under
the settings dialog in thirty-three markets at once.

Two things to keep true if this text is edited: the app makes no network connections, and
translation exists only on Copilot+ PCs. Both are claims a certification reviewer can check, and
the privacy policy at <https://lvxiaole.github.io/glyfo-site/privacy.html> repeats the first one.

Shortcuts named below are the app's **defaults**: region capture is Alt+Z, falling back to
Ctrl+Shift+G when another app already holds Alt+Z, and whole screen is Ctrl+Shift+R. Since 1.3.0
both are user-configurable, so the copy names Alt+Z as a starting point rather than as the shortcut.

The **What's new in this version** field is not part of the listing export and has no section here.
Its copy, covering 1.2.0 and 1.3.0 together, is in `docs/store-release-notes.md`.

Adding an eighth screenshot works the way the sixth and seventh did. The export carries
`DesktopScreenshot1..30` and `DesktopScreenshotCaption1..30`, but a slot with nothing uploaded is
empty in every language, and the image cells hold dashboard URLs rather than file names — there is
nothing to write into slot 8 until the asset exists. So upload the new PNG to en-us in the dashboard
first, re-export, then raise the caption count here and the `Shots` count in `Fill-ListingCsv.ps1`,
and pass the dashboard's slot order as `-EnSlotOrder`.

---

## English (default)

### 简短说明 — Short description

Glyfo pulls text out of anything you can see: a screenshot, a photo of a page, a scanned form, a
slide, a video still, a PDF. Press Alt+Z and drag a box around part of the screen, or open a file,
paste from the clipboard, or send a picture over from another app. The text comes back beside the
image, ready to copy, save, search or read aloud. It handles a whole PDF or a stack of pictures in
one go, and keeps everything it has read in a searchable history. It reads QR codes and barcodes
from the same picture, and on a Copilot+ PC it can translate the result. Everything happens on your
PC — Glyfo makes no internet connections at all. Free, with no ads and nothing to buy.

### 说明 — Description

Glyfo turns pictures of text into text you can use.

Recognition runs on the OCR that ships with Windows, so there is no account to create, no upload
and no waiting on a server. On a Copilot+ PC, Glyfo also uses the on-device text recognition model
for harder images and can translate what it found — still without going online.

**Five ways to get an image in**
- Press Alt+Z and drag a box around any part of the screen. Ctrl+Shift+R takes the whole screen.
  Both shortcuts are yours to change.
- Ctrl+V pastes an image, or text, straight from the clipboard.
- Ctrl+O opens a picture or a PDF, and dragging one onto the window works too.
- Right-click a picture in File Explorer and open it with Glyfo, or send it through the Windows
  share sheet from Photos, Snipping Tool or a browser.
- Switch on clipboard watching and every snip you take with Win+Shift+S is read on its own, the
  text left on the clipboard for you to paste. Off until you ask for it.

**What you get back**
- The recognized text next to the image, in the order it was laid out.
- Copy with one click, or let a capture copy itself the moment it finishes.
- Save it to a .txt or .md file with Ctrl+S.
- Find in the text with Ctrl+F, with a word and character count beside it.
- Read aloud, using any voice installed on your PC.
- Unwrap merges hard line breaks back into paragraphs. No spaces strips every space, which is what
  Chinese, Japanese and Korean text needs after recognition.
- Buttons for the links, e-mail addresses and phone numbers found in the text.
- QR codes and barcodes decoded from the same image.
- A searchable history that survives a restart, so a capture you took last week is still one click
  away. It stores text only, and you can empty it or switch it off.

**More than one page at a time**
- Open a PDF and read it a page at a time, moving through it in the window.
- Or read the whole document at once, and save the result as one file or one file per page.
- Drop a stack of pictures on the window and recognize all of them in a single run.

**Choosing how it reads**
- Pick the recognition language from the packs Windows has installed, or let Glyfo choose.
- An option repairs the classic OCR mistake of reading v1.6.5 as vl.6.5 — an l or an I becomes a 1
  only where a separator and a digit sit beside it, so html5 and IPv6 are left alone.
- Rotate a picture, or straighten a photo taken at an angle, before reading it.
- Zoom to fit, or view at actual pixel size, and recognize the whole image or just a selection.

**It stays out of the way**
- Closing the window leaves Glyfo in the notification area, so the capture shortcut keeps working.
  Exit from there closes it for good, and the whole behaviour is a setting you can turn off.
- It can start with Windows and go straight to the notification area without opening a window, so
  the shortcut works from the moment you sign in. Off by default; you turn it on.
- When the window is hidden, a notification shows the first line of what was just recognized.
- The window comes back at the size and place you left it, light or dark as you prefer.

**Languages**
The interface is available in 33 languages and follows your Windows language setting. Text
recognition uses the OCR language packs installed on your PC — add more under Settings › Time &
language › Language & region.

**Privacy**
Glyfo makes no network connections. Images, recognized text and translations never leave your PC.
No account, no telemetry, no advertising.

### 产品功能 — Product features

- Capture any part of the screen with a shortcut of your own choosing and recognize it immediately, without leaving the app you were reading
- Runs on the OCR built into Windows, plus the on-device recognition model on Copilot+ PCs
- Open a picture or a PDF, paste from the clipboard, drag and drop, or receive a file through the Windows share sheet
- Reads a whole PDF or a stack of images in one batch, saving the result as one file or one file per page
- Keeps a searchable history that survives a restart, and saves any result to a .txt or .md file
- Reads QR codes and barcodes out of the same image
- Translates on Copilot+ PCs, on the device, with no network access
- Reads the result aloud with any voice on your PC, and unwraps line breaks or removes the spaces that Chinese, Japanese and Korean text needs
- Lives in the notification area so the capture shortcut keeps working after you close the window; interface in 33 languages, light or dark
- Makes no internet connections: nothing you recognize ever leaves your PC

### 屏幕截图标题 — Screenshot captions

1. `01-text-from-a-page.png` — Every recognized line appears beside the picture in reading order, ready to copy, read aloud, or unwrap back into paragraphs.
2. `02-any-language.png` — Pick the recognition language from the packs Windows has installed — Chinese, Japanese, Korean and Arabic among them — or let Glyfo choose it.
3. `03-qr-and-barcodes.png` — QR codes and barcodes are read out of the same picture, so no separate scanner app is needed.
4. `04-history.png` — Recent results stay in the history, so something you captured a few minutes ago is still one click away.
5. `05-settings.png` — Interface language, starting with Windows, staying in the notification area, and the fix that keeps v1.6.5 from becoming vl.6.5.
6. `06-pdf-pages.png` — A PDF opens page by page, and the page you are on is recognized straight away; the arrows walk through the rest of the document.
7. `07-batch.png` — A whole document or a stack of pictures goes through in one run, saved as a single file or as one file per page.

### 搜索词 — Search terms

`image to text`, `screenshot to text`, `pdf to text`, `extract text from picture`, `qr code reader`, `barcode scanner`, `OCR`

---

## 中文(简体) — Chinese (Simplified)

### 简短说明

Glyfo 把屏幕上看得见的字变成能用的字：截图、拍下来的书页、扫描件、幻灯片、视频画面、PDF 都行。按
Alt+Z 框选屏幕上的任意一块，或者打开文件、粘贴剪贴板、从别的应用分享一张图过来，识别结果就出现在图片
旁边，可以复制、保存、查找、朗读。整本 PDF 或者一叠图片可以一次读完，读过的内容都留在可搜索的历史
记录里。同一张图里的二维码和条形码也会一并读出来；在 Copilot+ PC 上还能就地翻译。全部在本机完成——
Glyfo 不进行任何联网。免费，无广告，无内购。

### 说明

Glyfo 把图片里的文字，变成可以直接用的文字。

识别用的是 Windows 自带的 OCR，不需要注册账号，不上传，也不用等服务器。在 Copilot+ PC 上，Glyfo 还会
调用设备端的文字识别模型来处理更难的图，并且可以翻译识别结果——同样不联网。

**五种方式把图放进来**
- 按 Alt+Z 框选屏幕上的任意一块；Ctrl+Shift+R 直接截全屏。两个快捷键都可以自己改。
- Ctrl+V 从剪贴板粘贴图片，粘文字也行。
- Ctrl+O 打开图片或 PDF，把文件拖进窗口同样可以。
- 在文件资源管理器里右键图片，用 Glyfo 打开；或者从"照片""截图工具"、浏览器的分享面板送过来。

- 打开剪贴板监听后，每次用 Win+Shift+S 截的图都会自动识别，文字留在剪贴板里等你粘贴。默认关闭，
  想要才开。

**能拿到什么**
- 识别出的文字就在图片旁边，按原来的版面顺序排列。
- 一键复制；也可以让截图识别完自动复制。
- Ctrl+S 直接存成 .txt 或 .md 文件。
- Ctrl+F 在结果里查找，旁边还有字数和字符数。
- 用本机安装的任意语音朗读。
- "去换行"把硬换行拼回段落，"去空格"清掉所有空格——中日韩文本识别完通常都需要这一步。
- 文本里认出的网址、邮箱、电话，各自会多出一个按钮。
- 同一张图里的二维码和条形码一并解出来。
- 历史记录能活过重启，还能搜索，上周截的那一张仍然一点就回来。只存文字，随时可以清空或关闭。

**一次不止一页**
- 打开 PDF 逐页阅读，在窗口里直接翻页。
- 也可以整本一次识别，结果存成一个文件，或者每页一个。
- 把一叠图片拖进窗口，一次全部识别完。

**识别方式可以调**
- 识别语言从 Windows 已装的语言包里选，也可以交给 Glyfo 自己判断。
- 有一个开关专治 OCR 把 v1.6.5 读成 vl.6.5 的老毛病：只有当 l 或 I 紧挨着分隔符和数字时才改成 1，
  html5 和 IPv6 不受影响。
- 识别之前可以把图片转个方向，拍歪的照片也能一键摆正。
- 适应窗口缩放，或按实际像素查看；可以识别整张图，也可以只识别框选的部分。

**不碍事**
- 关掉窗口后 Glyfo 留在通知区域，截图快捷键照常可用；在那里选"退出"才是真正关掉。这一整套行为本身
  也是个开关，不想要可以关掉。
- 可以开机自启并直接进通知区域、不弹窗口，登录后立刻就能用快捷键。默认关闭，由你打开。
- 窗口收起时，识别完会用通知显示第一行内容。
- 窗口按上次关闭时的大小和位置打开，界面浅色深色随你挑。

**语言**
界面有 33 种语言，跟随 Windows 的语言设置。文字识别用的是本机已安装的 OCR 语言包——可以在
"设置 › 时间和语言 › 语言和区域"里添加。

**隐私**
Glyfo 不进行任何网络连接。图片、识别出的文字、翻译结果都不会离开这台电脑。没有账号，没有遥测，
没有广告。

### 产品功能

- 用自己设定的快捷键框选屏幕任意区域立即识别，不用离开正在看的那个应用
- 基于 Windows 自带 OCR；在 Copilot+ PC 上叠加设备端识别模型
- 打开图片或 PDF、粘贴剪贴板、拖放，或者从 Windows 分享面板把文件送进来
- 整本 PDF 或一叠图片可以一次批量识别，结果合并成一个文件，或者每页一个
- 历史记录能活过重启并且可以搜索，任何一次结果都能存成 .txt 或 .md 文件
- 顺带读出同一张图里的二维码和条形码
- 在 Copilot+ PC 上就地翻译，全程不联网
- 用本机任意语音朗读识别结果，并能去换行、去空格——中日韩文本识别之后正需要的清理
- 常驻通知区域，关掉窗口之后截图快捷键依然可用；界面 33 种语言，浅色深色可选
- 不进行任何联网：识别的内容不会离开这台电脑

### 屏幕截图标题

1. `01-text-from-a-page.png` — 识别结果按原文顺序排在图片旁边，可以直接复制、朗读，或去掉换行还原成段落。
2. `02-any-language.png` — 识别语言从 Windows 已安装的语言包里选——中日韩都在其中——也可以交给 Glyfo 自动判断。
3. `03-qr-and-barcodes.png` — 二维码和条码从同一张图里一并读出，不用再装扫码工具。
4. `04-history.png` — 最近的识别结果留在历史里，几分钟前截的那一张仍然一键可达。
5. `05-settings.png` — 界面语言、开机自启、关闭后留在通知区域，以及那条让 v1.6.5 不被读成 vl.6.5 的修正。
6. `06-pdf-pages.png` — PDF 按页打开，当前这一页立刻识别，翻页箭头把整份文档走完。
7. `07-batch.png` — 整份文档或一叠图片一次跑完，可以合成一个文件，也可以一页存一个。

### 搜索词

`图片转文字`, `截图取字`, `PDF转文字`, `文字识别`, `二维码识别`, `条形码扫描`, `OCR`

---

## 中文(繁體) — Chinese (Traditional)

### 簡短說明

Glyfo 把螢幕上看得到的字變成能用的字：擷圖、拍下來的書頁、掃描件、投影片、影片畫面、PDF 都可以。按
Alt+Z 框選螢幕上任一塊，或是開啟檔案、貼上剪貼簿、從別的應用程式分享一張圖過來，辨識結果就出現在圖片
旁邊，可以複製、儲存、尋找、朗讀。整份 PDF 或一疊圖片可以一次讀完，讀過的內容都留在可搜尋的歷程記錄
裡。同一張圖裡的 QR Code 和條碼也會一併讀出來；在 Copilot+ PC 上還能就地翻譯。全部在本機完成——
Glyfo 不進行任何連網。免費，沒有廣告，沒有內購。

### 說明

Glyfo 把圖片裡的文字，變成可以直接使用的文字。

辨識用的是 Windows 內建的 OCR，不必註冊帳號、不上傳，也不用等伺服器。在 Copilot+ PC 上，Glyfo 還會
呼叫裝置端的文字辨識模型來處理比較難的圖，並且可以翻譯辨識結果——同樣不連網。

**五種方式把圖放進來**
- 按 Alt+Z 框選螢幕上任一塊；Ctrl+Shift+R 直接擷取全螢幕。兩個快速鍵都可以自己改。
- Ctrl+V 從剪貼簿貼上圖片，貼文字也可以。
- Ctrl+O 開啟圖片或 PDF，把檔案拖進視窗同樣可以。
- 在檔案總管裡對圖片按右鍵，用 Glyfo 開啟；或從「相片」「剪取工具」、瀏覽器的分享面板送過來。

- 開啟剪貼簿監看後，每次用 Win+Shift+S 擷取的畫面都會自動辨識，文字留在剪貼簿裡等你貼上。預設
  關閉，想要才開。

**能拿到什麼**
- 辨識出的文字就在圖片旁邊，照原本的版面順序排列。
- 一鍵複製；也可以讓擷圖辨識完自動複製。
- Ctrl+S 直接存成 .txt 或 .md 檔案。
- Ctrl+F 在結果裡尋找，旁邊還有字數和字元數。
- 用本機安裝的任一語音朗讀。
- 「去換行」把硬換行接回段落，「去空格」清掉所有空格——中日韓文本辨識完通常都需要這一步。
- 文字裡認出的網址、電子郵件、電話，各自會多出一個按鈕。
- 同一張圖裡的 QR Code 和條碼一併解出來。
- 歷程記錄能活過重新啟動，還能搜尋，上週擷取的那一張依然一點就回來。只存文字，隨時可以清空或關閉。

**一次不只一頁**
- 開啟 PDF 逐頁閱讀，在視窗裡直接翻頁。
- 也可以整份一次辨識，結果存成一個檔案，或者每頁一個。
- 把一疊圖片拖進視窗，一次全部辨識完。

**辨識方式可以調**
- 辨識語言從 Windows 已安裝的語言套件裡選，也可以交給 Glyfo 自行判斷。
- 有一個開關專門處理 OCR 把 v1.6.5 讀成 vl.6.5 的老問題：只有當 l 或 I 緊鄰分隔符號與數字時才改成
  1，html5 和 IPv6 不受影響。
- 辨識之前可以把圖片轉個方向，拍歪的照片也能一鍵擺正。
- 縮放至符合視窗，或依實際像素檢視；可以辨識整張圖，也可以只辨識框選的部分。

**不礙事**
- 關掉視窗後 Glyfo 留在通知區域，擷圖快速鍵照常可用；在那裡選「結束」才是真正關掉。這一整套行為
  本身也是個開關，不想要可以關掉。
- 可以開機自動啟動並直接進通知區域、不開視窗，登入後立刻就能用快速鍵。預設關閉，由你開啟。
- 視窗收起時，辨識完會用通知顯示第一行內容。
- 視窗照上次關閉時的大小和位置開啟，介面淺色深色隨你挑。

**語言**
介面有 33 種語言，跟隨 Windows 的語言設定。文字辨識使用本機已安裝的 OCR 語言套件——可以在
「設定 › 時間與語言 › 語言與地區」中新增。

**隱私**
Glyfo 不進行任何網路連線。圖片、辨識出的文字、翻譯結果都不會離開這台電腦。沒有帳號、沒有遙測、
沒有廣告。

### 產品功能

- 用自己設定的快速鍵框選螢幕任一區域立即辨識，不必離開正在看的那個應用程式
- 以 Windows 內建 OCR 為基礎；在 Copilot+ PC 上再加上裝置端辨識模型
- 開啟圖片或 PDF、貼上剪貼簿、拖放，或從 Windows 分享面板把檔案送進來
- 整份 PDF 或一疊圖片可以一次批次辨識，結果合併成一個檔案，或者每頁一個
- 歷程記錄能活過重新啟動而且可以搜尋，任何一次結果都能存成 .txt 或 .md 檔案
- 順帶讀出同一張圖裡的 QR Code 與條碼
- 在 Copilot+ PC 上就地翻譯，全程不連網
- 用本機任一語音朗讀辨識結果，並能去換行、去空格——中日韓文本辨識之後正需要的整理
- 常駐通知區域，關掉視窗之後擷圖快速鍵依然可用；介面 33 種語言，淺色深色可選
- 不進行任何連網：辨識的內容不會離開這台電腦

### 螢幕擷取畫面標題

1. `01-text-from-a-page.png` — 辨識結果依原文順序排在圖片旁邊，可以直接複製、朗讀，或去掉換行還原成段落。
2. `02-any-language.png` — 辨識語言從 Windows 已安裝的語言套件裡挑——中日韓都在其中——也可以交給 Glyfo 自動判斷。
3. `03-qr-and-barcodes.png` — QR 碼和條碼從同一張圖裡一併讀出，不必再裝掃碼工具。
4. `04-history.png` — 最近的辨識結果留在歷程記錄裡，幾分鐘前擷取的那一張仍然一鍵可達。
5. `05-settings.png` — 介面語言、開機自動啟動、關閉後留在通知區域，以及那條讓 v1.6.5 不被讀成 vl.6.5 的修正。
6. `06-pdf-pages.png` — PDF 按頁開啟，目前這一頁立刻辨識，翻頁箭頭把整份文件走完。
7. `07-batch.png` — 整份文件或一疊圖片一次跑完，可以合成一個檔案，也可以一頁存一個。

### 搜尋詞

`圖片轉文字`, `螢幕擷取文字`, `PDF 轉文字`, `文字辨識`, `QR Code 掃描`, `條碼掃描`, `OCR`

---

## 日本語 — Japanese

### 簡単な説明

Glyfo は、画面に映っているものを使えるテキストに変えます。スクリーンショット、紙面の写真、スキャン、
スライド、動画のコマ、PDF。Alt+Z を押して画面の一部を四角く囲むだけ。ファイルを開く、クリップボード
から貼り付ける、ほかのアプリから共有で送る、どれでも構いません。認識したテキストは画像の隣に出て、
コピー、保存、検索、読み上げがすぐできます。PDF 一冊でも画像の束でも一度に読み取り、読んだものは
検索できる履歴に残ります。同じ画像から QR コードとバーコードも読み取り、Copilot+ PC では翻訳もその場
で。すべて PC 内で完結し、Glyfo はインターネットに一切接続しません。無料、広告なし、課金なし。

### 説明

Glyfo は、文字が写った画像を、そのまま使えるテキストに変えます。

認識には Windows に組み込まれた OCR を使います。アカウント登録も、アップロードも、サーバー待ちも
ありません。Copilot+ PC では、読み取りの難しい画像にデバイス上の文字認識モデルを併用し、結果を翻訳
することもできます——こちらもオフラインのままです。

**画像を渡す 5 つの方法**
- Alt+Z で画面の好きな範囲を囲む。Ctrl+Shift+R なら画面全体。どちらのショートカットも変更できます。
- Ctrl+V でクリップボードから画像を貼り付け。テキストも貼れます。
- Ctrl+O で画像や PDF を開く。ウィンドウにドラッグしても同じです。
- エクスプローラーで画像を右クリックして Glyfo で開く。フォト、Snipping Tool、ブラウザーの共有
  メニューからでも送れます。

- クリップボード監視をオンにすると、Win+Shift+S で切り取るたびに自動で読み取り、テキストはクリップ
  ボードに残ります。既定はオフで、必要なときに自分でオンにします。

**戻ってくるもの**
- 認識されたテキストが画像の隣に、元のレイアウトの順序で並びます。
- ワンクリックでコピー。取り込みが終わった時点で自動コピーさせることもできます。
- Ctrl+S で .txt や .md ファイルにそのまま保存。
- Ctrl+F で本文を検索。隣に単語数と文字数が出ます。
- PC にインストールされている任意の音声で読み上げ。
- 「改行を除去」は強制改行を段落に戻し、「スペースを除去」はすべての空白を削除します。日本語・
  中国語・韓国語の認識結果には、たいていこの一手間が要ります。
- 本文で見つかったリンク、メールアドレス、電話番号には、それぞれのボタンが付きます。
- 同じ画像の QR コードとバーコードもデコード。
- 履歴は再起動後も残り、検索できます。先週の取り込みもワンクリックで戻せます。保存するのは文字だけ
  で、いつでも消去も無効化もできます。

**一度に 1 ページとは限りません**
- PDF を開いて、ウィンドウの中でページを送りながら読み取れます。
- 一冊まるごと読み取って、結果を 1 つのファイルにも、ページごとのファイルにも保存できます。
- 画像の束をウィンドウに落とせば、まとめて一度に認識します。

**読み取り方を選べます**
- 認識言語は Windows にインストール済みの言語パックから選択。自動に任せることもできます。
- OCR が v1.6.5 を vl.6.5 と読む昔からの誤りに対処するオプションがあります。区切り文字と数字が隣に
  ある場所でだけ l や I を 1 に直すので、html5 や IPv6 はそのままです。
- 読み取る前に画像を回転できます。斜めに撮った写真もワンクリックでまっすぐに。
- ウィンドウに合わせる表示と等倍表示。画像全体でも、選択した部分だけでも認識できます。

**邪魔になりません**
- ウィンドウを閉じても Glyfo は通知領域に残り、取り込みのショートカットは効いたままです。そこから
  「終了」を選ぶと完全に終了します。この動作自体もオン/オフできます。
- Windows と一緒に起動し、ウィンドウを開かずそのまま通知領域に入ることもできます。サインインした
  瞬間からショートカットが使えます。既定はオフで、必要なときに自分でオンにします。
- ウィンドウが隠れているときは、認識が終わると通知に先頭の 1 行が表示されます。
- ウィンドウは前回閉じたときの大きさと位置で開きます。外観は明るくも暗くもできます。

**言語**
インターフェイスは 33 言語。Windows の言語設定に従います。文字認識は PC にインストールされた OCR
言語パックを使います——[設定 › 時刻と言語 › 言語と地域] から追加できます。

**プライバシー**
Glyfo はネットワークに接続しません。画像、認識したテキスト、翻訳結果が PC の外に出ることはありま
せん。アカウントなし、テレメトリなし、広告なし。

### 製品の機能

- 自分で決めたショートカットで画面の任意の範囲を囲んですぐ認識。読んでいたアプリから離れる必要がありません
- Windows 内蔵の OCR を使用。Copilot+ PC ではデバイス上の認識モデルも併用します
- 画像や PDF を開く、クリップボードから貼り付け、ドラッグ＆ドロップ、Windows の共有メニューから受け取る
- PDF 一冊でも画像の束でも一括で認識し、結果を 1 つのファイルにもページごとのファイルにも保存できます
- 再起動後も残る検索できる履歴。どの結果も .txt や .md ファイルに保存できます
- 同じ画像から QR コードとバーコードも読み取ります
- Copilot+ PC ではデバイス上で翻訳。ネットワークは使いません
- PC の任意の音声で読み上げ。改行の除去とスペースの除去は、日本語・中国語・韓国語の認識結果に必要な整形です
- 通知領域に常駐するので、ウィンドウを閉じても取り込みのショートカットは使えます。33 言語、明暗の外観も選べます
- インターネットに一切接続しません。認識した内容が PC の外に出ることはありません

### スクリーンショットのキャプション

1. `01-text-from-a-page.png` — 認識した文字は読み取った順に画像の横へ。そのままコピー、読み上げ、改行をほどいて段落に戻すこともできます。
2. `02-any-language.png` — 認識する言語は Windows に入っている言語パックから選べます。日本語・中国語・韓国語も含まれ、自動選択も可能です。
3. `03-qr-and-barcodes.png` — QR コードとバーコードは同じ画像からまとめて読み取ります。別途スキャナーアプリは要りません。
4. `04-history.png` — 直近の結果は履歴に残るので、数分前に取り込んだものにもワンクリックで戻れます。
5. `05-settings.png` — 表示言語、Windows と同時に起動、閉じても通知領域に常駐、そして v1.6.5 が vl.6.5 にならないための補正。
6. `06-pdf-pages.png` — PDF はページ単位で開き、いま見ているページはすぐ認識されます。矢印で文書全体をたどれます。
7. `07-batch.png` — 文書ひとまとめ、あるいは画像の束を一度に処理し、1 つのファイルにも、ページごとのファイルにも保存できます。

### 検索キーワード

`画像から文字`, `スクショ 文字起こし`, `PDF 文字起こし`, `文字認識`, `QRコード 読み取り`, `バーコード読み取り`, `OCR`

---

## 한국어 — Korean

### 간단한 설명

Glyfo는 화면에 보이는 것을 쓸 수 있는 텍스트로 바꿉니다. 스크린샷, 책장을 찍은 사진, 스캔 문서,
슬라이드, 동영상 한 장면, PDF까지. Alt+Z를 누르고 화면의 원하는 부분을 사각형으로 감싸면 됩니다.
파일을 열거나, 클립보드에서 붙여넣거나, 다른 앱에서 공유로 보내도 됩니다. 인식된 텍스트는 이미지
옆에 나타나 바로 복사하고, 저장하고, 찾고, 읽어줄 수 있습니다. PDF 한 권이든 사진 여러 장이든 한
번에 읽어내고, 읽은 것은 검색되는 기록에 남습니다. 같은 이미지의 QR 코드와 바코드도 함께 읽고,
Copilot+ PC에서는 그 자리에서 번역합니다. 모두 PC 안에서 처리되며 Glyfo는 인터넷에 전혀 연결하지
않습니다. 무료이고 광고나 결제도 없습니다.

### 설명

Glyfo는 글자가 담긴 이미지를 바로 쓸 수 있는 텍스트로 바꿉니다.

인식에는 Windows에 내장된 OCR을 사용합니다. 계정을 만들 필요도, 업로드할 일도, 서버를 기다릴 일도
없습니다. Copilot+ PC에서는 어려운 이미지에 온디바이스 문자 인식 모델을 함께 사용하고, 결과를 번역할
수도 있습니다 — 이때도 네트워크는 쓰지 않습니다.

**이미지를 넣는 다섯 가지 방법**
- Alt+Z로 화면의 원하는 부분을 감싸세요. Ctrl+Shift+R은 전체 화면입니다. 두 단축키 모두 직접 바꿀
  수 있습니다.
- Ctrl+V로 클립보드의 이미지를 붙여넣습니다. 텍스트도 됩니다.
- Ctrl+O로 그림이나 PDF를 열고, 창으로 끌어다 놓아도 됩니다.
- 파일 탐색기에서 이미지를 마우스 오른쪽 버튼으로 눌러 Glyfo로 열거나, 사진·캡처 도구·브라우저의
  공유 메뉴에서 보내세요.

- 클립보드 감시를 켜 두면 Win+Shift+S로 잘라낼 때마다 알아서 읽고, 그 글자를 클립보드에 남겨
  둡니다. 기본값은 꺼짐이며 직접 켜는 방식입니다.

**결과로 얻는 것**
- 인식된 텍스트가 이미지 옆에, 원래 배치 순서대로 놓입니다.
- 한 번 클릭으로 복사하거나, 캡처가 끝나는 즉시 자동으로 복사되게 할 수 있습니다.
- Ctrl+S로 .txt나 .md 파일에 바로 저장합니다.
- Ctrl+F로 본문을 찾고, 옆에 낱말 수와 글자 수가 보입니다.
- PC에 설치된 아무 음성으로나 소리 내어 읽어줍니다.
- '줄바꿈 제거'는 강제 줄바꿈을 문단으로 되돌리고, '공백 제거'는 모든 공백을 없앱니다. 한국어·중국어·
  일본어 인식 결과에는 대개 이 과정이 필요합니다.
- 본문에서 찾아낸 링크, 메일 주소, 전화번호마다 전용 단추가 생깁니다.
- 같은 이미지에서 QR 코드와 바코드도 해독합니다.
- 기록은 다시 시작한 뒤에도 남고 검색됩니다. 지난주 캡처도 클릭 한 번이면 돌아옵니다. 글자만
  저장하며 언제든 비우거나 끌 수 있습니다.

**한 번에 한 쪽만은 아닙니다**
- PDF를 열어 창 안에서 쪽을 넘겨 가며 읽습니다.
- 한 권을 통째로 읽어 결과를 파일 하나로, 또는 쪽마다 하나씩 저장할 수도 있습니다.
- 사진 여러 장을 창에 끌어다 놓으면 한 번에 모두 인식합니다.

**읽는 방식을 고를 수 있습니다**
- 인식 언어는 Windows에 설치된 언어 팩에서 고르거나 자동에 맡깁니다.
- OCR이 v1.6.5를 vl.6.5로 읽는 오래된 문제를 잡는 옵션이 있습니다. 구분 기호와 숫자가 바로 옆에 있을
  때만 l이나 I를 1로 바꾸므로 html5나 IPv6는 그대로 둡니다.
- 읽기 전에 그림을 돌릴 수 있고, 비스듬히 찍은 사진도 한 번 눌러 바로 세웁니다.
- 창에 맞추기와 실제 크기 보기. 이미지 전체를 인식할 수도, 선택한 부분만 인식할 수도 있습니다.

**방해하지 않습니다**
- 창을 닫아도 Glyfo는 알림 영역에 남아 캡처 단축키가 계속 동작합니다. 거기서 '끝내기'를 선택해야 완전히
  종료됩니다. 이 동작 자체도 끌 수 있는 설정입니다.
- Windows와 함께 시작해 창을 열지 않고 바로 알림 영역으로 들어가게 할 수 있습니다. 로그인하는 순간부터
  단축키가 동작합니다. 기본값은 꺼짐이며 직접 켜는 방식입니다.
- 창이 숨겨져 있을 때는 인식이 끝나면 알림에 첫 줄이 표시됩니다.
- 창은 지난번에 두었던 크기와 자리에서 열리고, 밝거나 어두운 모습으로 고를 수 있습니다.

**언어**
인터페이스는 33개 언어이며 Windows 언어 설정을 따릅니다. 문자 인식은 PC에 설치된 OCR 언어 팩을
사용합니다 — 설정 › 시간 및 언어 › 언어 및 지역에서 추가할 수 있습니다.

**개인 정보**
Glyfo는 네트워크에 연결하지 않습니다. 이미지, 인식된 텍스트, 번역 결과가 PC를 벗어나지 않습니다.
계정도, 원격 분석도, 광고도 없습니다.

### 제품 기능

- 직접 고른 단축키로 화면의 어느 부분이든 감싸 즉시 인식합니다. 보고 있던 앱을 떠날 필요가 없습니다
- Windows에 내장된 OCR을 사용하고, Copilot+ PC에서는 온디바이스 인식 모델을 더합니다
- 그림이나 PDF 열기, 클립보드 붙여넣기, 끌어다 놓기, Windows 공유 메뉴로 받기
- PDF 한 권이나 사진 여러 장을 한 번에 인식하고, 결과를 파일 하나로 또는 쪽마다 하나씩 저장합니다
- 다시 시작해도 남고 검색되는 기록, 그리고 어떤 결과든 .txt나 .md 파일로 저장
- 같은 이미지에서 QR 코드와 바코드도 읽습니다
- Copilot+ PC에서는 기기 안에서 번역하며 네트워크를 쓰지 않습니다
- PC의 아무 음성으로나 결과를 읽어주고, 줄바꿈 제거와 공백 제거로 한국어·중국어·일본어 결과를 정리합니다
- 알림 영역에 머물러 창을 닫은 뒤에도 캡처 단축키가 동작합니다. 33개 언어, 밝거나 어두운 모습
- 인터넷에 전혀 연결하지 않습니다. 인식한 내용은 PC를 벗어나지 않습니다

### 스크린샷 캡션

1. `01-text-from-a-page.png` — 인식한 글자는 읽은 순서대로 이미지 옆에 나타납니다. 그대로 복사하거나 소리 내어 읽거나 줄바꿈을 풀어 문단으로 되돌릴 수 있습니다.
2. `02-any-language.png` — 인식 언어는 Windows에 설치된 언어 팩에서 고릅니다. 한국어·중국어·일본어도 포함되며 자동 선택도 가능합니다.
3. `03-qr-and-barcodes.png` — QR 코드와 바코드를 같은 이미지에서 함께 읽어냅니다. 별도의 스캐너 앱이 필요 없습니다.
4. `04-history.png` — 최근 결과는 기록에 남아 있어 몇 분 전에 캡처한 것도 클릭 한 번이면 다시 꺼낼 수 있습니다.
5. `05-settings.png` — 인터페이스 언어, Windows 시작 시 실행, 닫아도 알림 영역에 유지, 그리고 v1.6.5가 vl.6.5로 읽히지 않게 하는 보정.
6. `06-pdf-pages.png` — PDF는 쪽 단위로 열리고 지금 보고 있는 쪽은 곧바로 인식됩니다. 화살표로 문서 전체를 넘길 수 있습니다.
7. `07-batch.png` — 문서 한 편이나 이미지 여러 장을 한 번에 처리해 하나의 파일로, 또는 쪽마다 따로 저장합니다.

### 검색어

`이미지 텍스트 추출`, `화면 캡처 문자인식`, `PDF 텍스트 추출`, `문자 인식`, `QR 코드 스캔`, `바코드 스캔`, `OCR`

---

## Deutsch — German

### Kurzbeschreibung

Glyfo holt Text aus allem heraus, was Sie sehen können: aus einem Screenshot, dem Foto einer
Buchseite, einem Scan, einer Folie, einem Videostandbild, einem PDF. Alt+Z drücken und einen Rahmen
um einen Teil des Bildschirms ziehen — oder eine Datei öffnen, aus der Zwischenablage einfügen, ein
Bild aus einer anderen App herüberschicken. Der erkannte Text steht neben dem Bild, bereit zum
Kopieren, Speichern, Durchsuchen und Vorlesen. Ein ganzes PDF oder ein Stapel Bilder geht in einem
Durchgang, und alles Gelesene bleibt in einem durchsuchbaren Verlauf. QR-Codes und Barcodes liest
Glyfo aus demselben Bild mit, und auf einem Copilot+ PC übersetzt es das Ergebnis. Alles geschieht
auf Ihrem PC — Glyfo baut überhaupt keine Internetverbindung auf. Kostenlos, ohne Werbung, ohne
Käufe.

### Beschreibung

Glyfo macht aus Bildern von Text wieder Text, mit dem Sie arbeiten können.

Die Erkennung läuft auf der OCR, die Windows mitbringt: kein Konto, kein Upload, kein Warten auf
einen Server. Auf einem Copilot+ PC nutzt Glyfo zusätzlich das Texterkennungsmodell auf dem Gerät
für schwierige Bilder und kann das Ergebnis übersetzen — ebenfalls ohne Netzverbindung.

**Fünf Wege, ein Bild hineinzubekommen**
- Alt+Z drücken und einen Rahmen um einen beliebigen Teil des Bildschirms ziehen. Ctrl+Shift+R
  nimmt den ganzen Bildschirm. Beide Tastenkürzel können Sie selbst festlegen.
- Ctrl+V fügt ein Bild aus der Zwischenablage ein, Text ebenso.
- Ctrl+O öffnet ein Bild oder ein PDF; ins Fenster ziehen geht genauso.
- Im Explorer mit der rechten Maustaste auf ein Bild und mit Glyfo öffnen, oder über die
  Windows-Teilen-Funktion aus Fotos, dem Snipping Tool oder dem Browser schicken.

- Mit eingeschalteter Zwischenablage-Überwachung wird jeder Ausschnitt, den Sie mit Win+Shift+S
  machen, von selbst gelesen und der Text bleibt in der Zwischenablage. Aus, bis Sie es einschalten.

**Was zurückkommt**
- Der erkannte Text neben dem Bild, in der Reihenfolge des ursprünglichen Layouts.
- Kopieren mit einem Klick — oder eine Aufnahme kopiert sich selbst, sobald sie fertig ist.
- Mit Ctrl+S direkt in eine .txt- oder .md-Datei speichern.
- Mit Ctrl+F im Text suchen, daneben die Wort- und Zeichenzahl.
- Vorlesen mit jeder Stimme, die auf Ihrem PC installiert ist.
- „Umbrüche entfernen" fügt harte Zeilenumbrüche wieder zu Absätzen zusammen, „Leerzeichen
  entfernen" streicht jedes Leerzeichen — was chinesischer, japanischer und koreanischer Text nach
  der Erkennung meist braucht.
- Eigene Schaltflächen für Links, E-Mail-Adressen und Telefonnummern, die im Text stehen.
- QR-Codes und Barcodes aus demselben Bild.
- Ein durchsuchbarer Verlauf, der einen Neustart übersteht: die Aufnahme von letzter Woche ist
  einen Klick entfernt. Er speichert nur Text, und Sie können ihn leeren oder abschalten.

**Mehr als eine Seite auf einmal**
- Ein PDF öffnen und Seite für Seite darin lesen.
- Oder das ganze Dokument auf einmal, gespeichert als eine Datei oder als eine Datei je Seite.
- Einen Stapel Bilder ins Fenster ziehen und alle in einem Durchgang erkennen.

**Sie bestimmen, wie gelesen wird**
- Erkennungssprache aus den in Windows installierten Sprachpaketen wählen oder Glyfo entscheiden
  lassen.
- Eine Option behebt den klassischen OCR-Fehler, v1.6.5 als vl.6.5 zu lesen: Ein l oder I wird nur
  dort zur 1, wo ein Trennzeichen und eine Ziffer daneben stehen — html5 und IPv6 bleiben unberührt.
- Ein Bild drehen oder ein schief aufgenommenes Foto gerade richten, bevor es gelesen wird.
- Ans Fenster anpassen oder in Originalgröße ansehen; das ganze Bild erkennen oder nur die Auswahl.

**Es steht nicht im Weg**
- Wird das Fenster geschlossen, bleibt Glyfo im Infobereich, und das Tastenkürzel funktioniert
  weiter. „Beenden" dort schließt es endgültig. Dieses Verhalten selbst ist abschaltbar.
- Glyfo kann mit Windows starten und ohne Fenster direkt in den Infobereich gehen, sodass das
  Kürzel ab der Anmeldung bereitsteht. Standardmäßig aus; Sie schalten es ein.
- Bei verstecktem Fenster zeigt eine Benachrichtigung die erste Zeile des Erkannten.
- Das Fenster kommt in der Größe und an der Stelle zurück, wo Sie es verlassen haben — hell oder
  dunkel, ganz wie Sie möchten.

**Sprachen**
Die Oberfläche gibt es in 33 Sprachen und richtet sich nach Ihrer Windows-Spracheinstellung. Die
Texterkennung nutzt die auf dem PC installierten OCR-Sprachpakete — weitere fügen Sie unter
Einstellungen › Zeit und Sprache › Sprache und Region hinzu.

**Datenschutz**
Glyfo stellt keine Netzwerkverbindungen her. Bilder, erkannter Text und Übersetzungen verlassen
Ihren PC nicht. Kein Konto, keine Telemetrie, keine Werbung.

### Produktfunktionen

- Mit einem selbst gewählten Tastenkürzel einen beliebigen Bildschirmausschnitt aufnehmen und sofort erkennen, ohne die App zu verlassen
- Nutzt die in Windows eingebaute OCR, auf Copilot+ PCs zusätzlich das Erkennungsmodell auf dem Gerät
- Bild oder PDF öffnen, aus der Zwischenablage einfügen, per Drag-and-drop oder über die Windows-Teilen-Funktion
- Liest ein ganzes PDF oder einen Stapel Bilder im Stapelbetrieb, gespeichert als eine Datei oder eine Datei je Seite
- Ein durchsuchbarer Verlauf, der einen Neustart übersteht, und jedes Ergebnis als .txt- oder .md-Datei
- Liest QR-Codes und Barcodes aus demselben Bild
- Übersetzt auf Copilot+ PCs direkt auf dem Gerät, ohne Netzzugriff
- Liest das Ergebnis mit jeder Stimme auf dem PC vor und entfernt Zeilenumbrüche oder Leerzeichen, wie CJK-Text es braucht
- Bleibt im Infobereich, damit das Tastenkürzel auch nach dem Schließen des Fensters funktioniert; 33 Sprachen, hell oder dunkel
- Baut keine Internetverbindung auf: Erkanntes verlässt Ihren PC nie

### Screenshot-Beschriftungen

1. `01-text-from-a-page.png` — Der erkannte Text steht neben dem Bild, in der Reihenfolge, in der er gelesen wurde – zum Kopieren, Vorlesen oder Zurückführen in Absätze.
2. `02-any-language.png` — Die Erkennungssprache wählen Sie aus den in Windows installierten Sprachpaketen – oder Glyfo entscheidet selbst.
3. `03-qr-and-barcodes.png` — QR-Codes und Barcodes werden aus demselben Bild gelesen; eine separate Scanner-App ist nicht nötig.
4. `04-history.png` — Die letzten Ergebnisse bleiben im Verlauf, sodass eine Aufnahme von vor ein paar Minuten weiterhin einen Klick entfernt ist.
5. `05-settings.png` — Oberflächensprache, Start mit Windows, Verbleib im Infobereich und die Korrektur, die aus v1.6.5 kein vl.6.5 macht.
6. `06-pdf-pages.png` — Ein PDF wird Seite für Seite geöffnet; die aktuelle Seite wird sofort erkannt, mit den Pfeilen geht es durch das restliche Dokument.
7. `07-batch.png` — Ein ganzes Dokument oder ein Stapel Bilder läuft in einem Durchgang durch – gespeichert als eine Datei oder als eine Datei pro Seite.

### Suchbegriffe

`Text aus Bild`, `Screenshot zu Text`, `PDF in Text umwandeln`, `Texterkennung`, `QR-Code lesen`, `Barcode scannen`, `OCR`

---

## Français — French

### Brève description

Glyfo extrait le texte de tout ce que vous voyez : une capture d'écran, la photo d'une page, un
document scanné, une diapositive, une image de vidéo, un PDF. Appuyez sur Alt+Z et encadrez une
partie de l'écran — ou ouvrez un fichier, collez depuis le presse-papiers, envoyez une image depuis
une autre application. Le texte reconnu apparaît à côté de l'image, prêt à être copié, enregistré,
cherché ou lu à voix haute. Glyfo traite aussi un PDF entier ou une pile d'images en une seule fois,
et conserve tout ce qu'il a lu dans un historique consultable. Il lit les QR codes et les
codes-barres de la même image, et sur un PC Copilot+ il traduit le résultat. Tout se passe sur votre
PC : Glyfo n'établit aucune connexion Internet. Gratuit, sans publicité et sans achat.

### Description

Glyfo transforme les images de texte en texte utilisable.

La reconnaissance s'appuie sur l'OCR intégré à Windows : aucun compte à créer, aucun envoi, aucune
attente côté serveur. Sur un PC Copilot+, Glyfo utilise en plus le modèle de reconnaissance de texte
embarqué pour les images difficiles et peut traduire le résultat — toujours sans passer par le
réseau.

**Cinq façons d'amener une image**
- Alt+Z pour encadrer n'importe quelle partie de l'écran. Ctrl+Shift+R prend l'écran entier. Les deux
  raccourcis sont modifiables.
- Ctrl+V colle une image depuis le presse-papiers, du texte également.
- Ctrl+O ouvre une image ou un PDF ; le glisser dans la fenêtre fonctionne aussi.
- Clic droit sur une image dans l'Explorateur pour l'ouvrir avec Glyfo, ou envoi depuis le volet de
  partage de Windows (Photos, Outil Capture d'écran, navigateur).
- Activez la surveillance du presse-papiers et chaque capture faite avec Win+Maj+S est lue toute
  seule, le texte restant dans le presse-papiers. Désactivée tant que vous ne la demandez pas.

**Ce que vous récupérez**
- Le texte reconnu à côté de l'image, dans l'ordre de la mise en page d'origine.
- Copie en un clic, ou copie automatique dès qu'une capture se termine.
- Enregistrement dans un fichier .txt ou .md avec Ctrl+S.
- Recherche dans le texte avec Ctrl+F, avec le nombre de mots et de caractères à côté.
- Lecture à voix haute avec n'importe quelle voix installée sur votre PC.
- « Supprimer les retours » recolle les retours à la ligne forcés en paragraphes ; « Supprimer les
  espaces » retire tous les espaces, ce dont le texte chinois, japonais et coréen a besoin après
  reconnaissance.
- Des boutons pour les liens, adresses e-mail et numéros de téléphone trouvés dans le texte.
- Les QR codes et codes-barres de la même image.
- Un historique consultable qui survit à un redémarrage : la capture de la semaine dernière reste à
  un clic. Il ne conserve que du texte, et vous pouvez le vider ou le désactiver.

**Plus d'une page à la fois**
- Ouvrez un PDF et lisez-le page par page, en le parcourant dans la fenêtre.
- Ou lisez tout le document d'un coup, et enregistrez le résultat en un fichier ou un fichier par
  page.
- Déposez une pile d'images sur la fenêtre et reconnaissez-les toutes en une seule passe.

**Vous choisissez comment il lit**
- Langue de reconnaissance parmi les modules linguistiques installés dans Windows, ou choix
  automatique.
- Une option corrige l'erreur classique consistant à lire v1.6.5 comme vl.6.5 : un l ou un I ne
  devient un 1 que là où un séparateur et un chiffre se trouvent à côté, si bien que html5 et IPv6
  restent intacts.
- Faire pivoter une image, ou redresser une photo prise de travers, avant de la lire.
- Ajuster à la fenêtre ou afficher à la taille réelle ; reconnaître toute l'image ou seulement la
  sélection.

**Il ne gêne pas**
- Fermer la fenêtre laisse Glyfo dans la zone de notification, et le raccourci de capture continue
  de fonctionner. « Quitter » l'arrête pour de bon. Ce comportement est lui-même une option.
- Glyfo peut démarrer avec Windows et rejoindre directement la zone de notification sans ouvrir de
  fenêtre : le raccourci est actif dès l'ouverture de session. Désactivé par défaut, c'est vous qui
  l'activez.
- Fenêtre masquée, une notification affiche la première ligne de ce qui vient d'être reconnu.
- La fenêtre revient à la taille et à l'endroit où vous l'avez laissée, en clair ou en sombre selon
  votre préférence.

**Langues**
L'interface existe en 33 langues et suit le réglage de langue de Windows. La reconnaissance utilise
les modules OCR installés sur le PC — vous en ajoutez dans Paramètres › Heure et langue › Langue et
région.

**Confidentialité**
Glyfo n'établit aucune connexion réseau. Les images, le texte reconnu et les traductions ne quittent
jamais votre PC. Pas de compte, pas de télémétrie, pas de publicité.

### Fonctionnalités du produit

- Capturez n'importe quelle partie de l'écran avec le raccourci de votre choix et reconnaissez-la aussitôt, sans quitter l'application que vous lisiez
- Repose sur l'OCR intégré à Windows, complété par le modèle embarqué sur les PC Copilot+
- Ouvrir une image ou un PDF, coller depuis le presse-papiers, glisser-déposer ou recevoir via le partage Windows
- Lit un PDF entier ou une pile d'images en un seul lot, résultat enregistré en un fichier ou un fichier par page
- Conserve un historique consultable qui survit à un redémarrage, et enregistre tout résultat en fichier .txt ou .md
- Lit les QR codes et les codes-barres de la même image
- Traduit sur les PC Copilot+, sur l'appareil, sans accès réseau
- Lit le résultat à voix haute avec n'importe quelle voix du PC, et supprime les retours à la ligne ou les espaces dont le texte CJC a besoin
- Reste dans la zone de notification pour que le raccourci fonctionne après la fermeture de la fenêtre ; interface en 33 langues, en clair ou en sombre
- N'établit aucune connexion Internet : ce que vous reconnaissez ne quitte pas votre PC

### Légendes des captures d'écran

1. `01-text-from-a-page.png` — Le texte reconnu s'affiche à côté de l'image, dans l'ordre où il a été lu : à copier, à faire lire à voix haute ou à remettre en paragraphes.
2. `02-any-language.png` — La langue de reconnaissance se choisit parmi les modules linguistiques installés dans Windows — ou Glyfo la détermine seul.
3. `03-qr-and-barcodes.png` — Les QR codes et les codes-barres sont lus dans la même image : aucune application de scan supplémentaire n'est nécessaire.
4. `04-history.png` — Les résultats récents restent dans l'historique ; une capture faite il y a quelques minutes reste à un clic.
5. `05-settings.png` — Langue de l'interface, démarrage avec Windows, maintien dans la zone de notification, et la correction qui évite que v1.6.5 devienne vl.6.5.
6. `06-pdf-pages.png` — Un PDF s'ouvre page par page : celle que vous regardez est reconnue aussitôt, et les flèches parcourent le reste du document.
7. `07-batch.png` — Un document entier ou une pile d'images passe en une seule fois, enregistré en un seul fichier ou en un fichier par page.

### Termes de recherche

`texte depuis image`, `capture écran texte`, `PDF en texte`, `reconnaissance texte`, `lire QR code`, `scanner code-barres`, `OCR`

---

## Español — Spanish

### Descripción breve

Glyfo extrae el texto de todo lo que puedas ver: una captura de pantalla, la foto de una página, un
documento escaneado, una diapositiva, un fotograma, un PDF. Pulsa Alt+Z y encuadra una parte de la
pantalla, o abre un archivo, pega desde el portapapeles o envía una imagen desde otra aplicación. El
texto reconocido aparece junto a la imagen, listo para copiar, guardar, buscar o escuchar. También
procesa un PDF entero o un montón de imágenes de una sola vez, y guarda todo lo que ha leído en un
historial con búsqueda. Lee códigos QR y de barras de esa misma imagen, y en un PC Copilot+ traduce
el resultado. Todo ocurre en tu PC: Glyfo no establece ninguna conexión a Internet. Gratis, sin
anuncios y sin compras.

### Descripción

Glyfo convierte las imágenes con texto en texto que puedes usar.

El reconocimiento se apoya en el OCR que trae Windows: sin cuenta, sin subir nada y sin esperar a un
servidor. En un PC Copilot+, Glyfo suma el modelo de reconocimiento de texto del propio dispositivo
para las imágenes difíciles y puede traducir el resultado, también sin conexión.

**Cinco formas de aportar una imagen**
- Alt+Z para encuadrar cualquier zona de la pantalla. Ctrl+Shift+R toma la pantalla completa. Los dos
  atajos puedes cambiarlos.
- Ctrl+V pega una imagen desde el portapapeles, y también texto.
- Ctrl+O abre una imagen o un PDF; arrastrarlo a la ventana funciona igual.
- Clic derecho sobre una imagen en el Explorador para abrirla con Glyfo, o envíala desde el panel de
  uso compartido de Windows (Fotos, Recorte, el navegador).
- Activa la vigilancia del portapapeles y cada recorte que hagas con Win+Mayús+S se lee solo, con el
  texto listo en el portapapeles. Desactivada hasta que la pidas.

**Lo que obtienes**
- El texto reconocido junto a la imagen, en el orden en que estaba dispuesto.
- Copiar con un clic, o dejar que una captura se copie sola en cuanto termina.
- Guardarlo en un archivo .txt o .md con Ctrl+S.
- Buscar dentro del texto con Ctrl+F, con el recuento de palabras y caracteres al lado.
- Lectura en voz alta con cualquier voz instalada en el PC.
- «Quitar saltos» une los saltos de línea forzados en párrafos y «Quitar espacios» elimina todos los
  espacios, que es lo que suele necesitar el texto chino, japonés y coreano tras el reconocimiento.
- Botones para los enlaces, las direcciones de correo y los teléfonos que aparezcan en el texto.
- Códigos QR y de barras de la misma imagen.
- Un historial con búsqueda que sobrevive a un reinicio: la captura de la semana pasada sigue a un
  clic. Guarda solo texto, y puedes vaciarlo o desactivarlo.

**Más de una página a la vez**
- Abre un PDF y léelo página a página, recorriéndolo dentro de la ventana.
- O lee el documento entero de una vez, y guarda el resultado en un archivo o en uno por página.
- Suelta un montón de imágenes en la ventana y reconócelas todas en una sola pasada.

**Tú decides cómo lee**
- Elige el idioma de reconocimiento entre los paquetes instalados en Windows, o deja que Glyfo lo
  decida.
- Una opción corrige el error clásico de leer v1.6.5 como vl.6.5: una l o una I pasa a ser 1 solo
  donde hay un separador y un dígito al lado, así que html5 e IPv6 quedan intactos.
- Girar una imagen, o enderezar una foto tomada torcida, antes de leerla.
- Ajustar a la ventana o ver a tamaño real; reconocer la imagen entera o solo la selección.

**No estorba**
- Al cerrar la ventana, Glyfo se queda en el área de notificación y el atajo de captura sigue
  funcionando. «Salir» lo cierra del todo. Ese comportamiento es, a su vez, una opción.
- Puede iniciarse con Windows y pasar directamente al área de notificación sin abrir ventana, de
  modo que el atajo funciona desde que inicias sesión. Desactivado de fábrica; lo activas tú.
- Con la ventana oculta, una notificación muestra la primera línea de lo reconocido.
- La ventana vuelve al tamaño y al sitio donde la dejaste, en claro o en oscuro, como prefieras.

**Idiomas**
La interfaz está en 33 idiomas y sigue la configuración de idioma de Windows. El reconocimiento usa
los paquetes de OCR instalados en el PC: se añaden en Configuración › Hora e idioma › Idioma y
región.

**Privacidad**
Glyfo no establece conexiones de red. Las imágenes, el texto reconocido y las traducciones nunca
salen de tu PC. Sin cuenta, sin telemetría y sin publicidad.

### Características del producto

- Captura cualquier parte de la pantalla con el atajo que tú elijas y reconócela al instante, sin salir de la aplicación que estabas leyendo
- Funciona sobre el OCR integrado en Windows y, en PC Copilot+, sobre el modelo del propio dispositivo
- Abrir una imagen o un PDF, pegar del portapapeles, arrastrar y soltar o recibir por el panel de uso compartido
- Lee un PDF entero o un montón de imágenes en un solo lote, guardando el resultado en un archivo o en uno por página
- Mantiene un historial con búsqueda que sobrevive a un reinicio, y guarda cualquier resultado en un archivo .txt o .md
- Lee códigos QR y de barras de la misma imagen
- Traduce en los PC Copilot+, en el dispositivo y sin acceso a la red
- Lee el resultado en voz alta con cualquier voz del PC, y quita los saltos de línea o los espacios que el texto CJK necesita
- Se queda en el área de notificación para que el atajo siga activo tras cerrar la ventana; interfaz en 33 idiomas, en claro u oscuro
- No se conecta a Internet: lo que reconoces no sale de tu PC

### Leyendas de las capturas de pantalla

1. `01-text-from-a-page.png` — El texto reconocido aparece junto a la imagen, en el orden en que se leyó: listo para copiar, escuchar en voz alta o volver a unir en párrafos.
2. `02-any-language.png` — El idioma de reconocimiento se elige entre los paquetes que Windows tenga instalados, o lo decide Glyfo por su cuenta.
3. `03-qr-and-barcodes.png` — Los códigos QR y de barras se leen de la misma imagen; no hace falta otra aplicación para escanear.
4. `04-history.png` — Los resultados recientes quedan en el historial, así que una captura de hace unos minutos sigue a un clic de distancia.
5. `05-settings.png` — Idioma de la interfaz, inicio con Windows, permanencia en el área de notificación y la corrección que evita que v1.6.5 se lea vl.6.5.
6. `06-pdf-pages.png` — Un PDF se abre página a página: la que está a la vista se reconoce enseguida y las flechas recorren el resto del documento.
7. `07-batch.png` — Un documento entero o un montón de imágenes pasa de una sola vez, guardado en un único archivo o en un archivo por página.

### Términos de búsqueda

`texto desde imagen`, `captura a texto`, `PDF a texto`, `reconocimiento de texto`, `leer código QR`, `escanear código de barras`, `OCR`

---

## Português (Brasil) — Portuguese (Brazil)

### Descrição breve

O Glyfo extrai texto de tudo o que você consegue ver: uma captura de tela, a foto de uma página, um
documento digitalizado, um slide, um quadro de vídeo, um PDF. Pressione Alt+Z e enquadre um pedaço da
tela, ou abra um arquivo, cole da área de transferência, envie uma imagem de outro aplicativo. O
texto reconhecido aparece ao lado da imagem, pronto para copiar, salvar, pesquisar ou ouvir. Ele dá
conta de um PDF inteiro ou de uma pilha de imagens de uma vez só, e guarda tudo o que leu em um
histórico pesquisável. Lê QR codes e códigos de barras da mesma imagem e, em um PC Copilot+, traduz o
resultado. Tudo acontece no seu PC: o Glyfo não faz nenhuma conexão com a Internet. Gratuito, sem
anúncios e sem compras.

### Descrição

O Glyfo transforma imagens de texto em texto que dá para usar.

O reconhecimento roda sobre o OCR que já vem no Windows: sem conta, sem upload e sem esperar
servidor. Em um PC Copilot+, o Glyfo ainda usa o modelo de reconhecimento de texto do próprio
dispositivo nas imagens mais difíceis e pode traduzir o resultado — também sem rede.

**Cinco jeitos de trazer uma imagem**
- Alt+Z para enquadrar qualquer parte da tela. Ctrl+Shift+R pega a tela inteira. Os dois atalhos são
  seus para trocar.
- Ctrl+V cola uma imagem da área de transferência, e texto também.
- Ctrl+O abre uma imagem ou um PDF; arrastar para a janela funciona igual.
- Clique com o botão direito em uma imagem no Explorador e abra com o Glyfo, ou mande pelo painel de
  compartilhamento do Windows (Fotos, Ferramenta de Captura, navegador).
- Ligue o monitoramento da área de transferência e cada recorte feito com Win+Shift+S é lido
  sozinho, com o texto pronto na área de transferência. Vem desligado até você pedir.

**O que volta**
- O texto reconhecido ao lado da imagem, na ordem em que estava disposto.
- Copiar com um clique, ou deixar que a captura se copie sozinha assim que terminar.
- Salvar em um arquivo .txt ou .md com Ctrl+S.
- Pesquisar dentro do texto com Ctrl+F, com a contagem de palavras e caracteres ao lado.
- Leitura em voz alta com qualquer voz instalada no PC.
- "Remover quebras" junta as quebras de linha forçadas de volta em parágrafos e "Remover espaços"
  tira todos os espaços — o que o texto chinês, japonês e coreano costuma precisar depois do
  reconhecimento.
- Botões para os links, endereços de e-mail e telefones encontrados no texto.
- QR codes e códigos de barras da mesma imagem.
- Um histórico pesquisável que sobrevive a uma reinicialização: a captura da semana passada continua
  a um clique. Guarda só texto, e você pode esvaziar ou desligar.

**Mais de uma página por vez**
- Abra um PDF e leia página por página, navegando dentro da janela.
- Ou leia o documento inteiro de uma vez, salvando o resultado em um arquivo ou um por página.
- Solte uma pilha de imagens na janela e reconheça todas em uma única passada.

**Você decide como ele lê**
- Escolha o idioma de reconhecimento entre os pacotes instalados no Windows, ou deixe automático.
- Uma opção corrige o erro clássico de ler v1.6.5 como vl.6.5: um l ou I vira 1 apenas onde há um
  separador e um dígito ao lado, então html5 e IPv6 ficam intactos.
- Girar uma imagem, ou endireitar uma foto tirada torta, antes de ler.
- Ajustar à janela ou ver em tamanho real; reconhecer a imagem inteira ou só a seleção.

**Ele não atrapalha**
- Fechar a janela deixa o Glyfo na área de notificação, e o atalho de captura continua funcionando.
  "Sair" ali encerra de vez. Esse comportamento é, ele próprio, uma opção.
- Pode iniciar com o Windows e ir direto para a área de notificação sem abrir janela, de modo que o
  atalho funciona desde o login. Vem desligado; você que liga.
- Com a janela oculta, uma notificação mostra a primeira linha do que acabou de ser reconhecido.
- A janela volta no tamanho e no lugar onde você deixou, clara ou escura, como preferir.

**Idiomas**
A interface está em 33 idiomas e segue a configuração de idioma do Windows. O reconhecimento usa os
pacotes de OCR instalados no PC — dá para adicionar em Configurações › Hora e idioma › Idioma e
região.

**Privacidade**
O Glyfo não faz conexões de rede. Imagens, texto reconhecido e traduções nunca saem do seu PC. Sem
conta, sem telemetria e sem publicidade.

### Recursos do produto

- Capture qualquer parte da tela com um atalho da sua escolha e reconheça na hora, sem sair do aplicativo que você estava lendo
- Roda sobre o OCR embutido no Windows e, em PCs Copilot+, sobre o modelo do próprio dispositivo
- Abrir uma imagem ou um PDF, colar da área de transferência, arrastar e soltar ou receber pelo compartilhamento do Windows
- Lê um PDF inteiro ou uma pilha de imagens em um único lote, salvando em um arquivo ou um por página
- Mantém um histórico pesquisável que sobrevive a uma reinicialização, e salva qualquer resultado em .txt ou .md
- Lê QR codes e códigos de barras da mesma imagem
- Traduz em PCs Copilot+, no dispositivo, sem acesso à rede
- Lê o resultado em voz alta com qualquer voz do PC, e remove quebras de linha ou os espaços de que o texto CJK precisa
- Fica na área de notificação para o atalho continuar valendo depois que a janela é fechada; interface em 33 idiomas, clara ou escura
- Não se conecta à Internet: o que você reconhece não sai do seu PC

### Legendas das capturas de tela

1. `01-text-from-a-page.png` — O texto reconhecido aparece ao lado da imagem, na ordem em que foi lido: pronto para copiar, ouvir em voz alta ou juntar de novo em parágrafos.
2. `02-any-language.png` — O idioma de reconhecimento vem dos pacotes que o Windows tem instalados — ou o Glyfo escolhe sozinho.
3. `03-qr-and-barcodes.png` — Códigos QR e de barras são lidos da mesma imagem; não é preciso outro aplicativo para escanear.
4. `04-history.png` — Os resultados recentes ficam no histórico, então uma captura de alguns minutos atrás continua a um clique.
5. `05-settings.png` — Idioma da interface, iniciar com o Windows, continuar na área de notificação e a correção que impede v1.6.5 de virar vl.6.5.
6. `06-pdf-pages.png` — Um PDF abre página a página: a que está à vista é reconhecida na hora e as setas percorrem o resto do documento.
7. `07-batch.png` — Um documento inteiro ou uma pilha de imagens passa de uma vez só, salvo em um único arquivo ou em um arquivo por página.

### Termos de pesquisa

`texto de imagem`, `captura para texto`, `PDF para texto`, `reconhecimento de texto`, `ler QR code`, `escanear código de barras`, `OCR`

---

## Русский — Russian

### Краткое описание

Glyfo достаёт текст из всего, что видно на экране: из снимка экрана, фотографии страницы,
отсканированного документа, слайда, кадра видео, PDF. Нажмите Alt+Z и обведите часть экрана — либо
откройте файл, вставьте из буфера обмена, отправьте картинку из другого приложения. Распознанный
текст появляется рядом с изображением: копируйте, сохраняйте, ищите, слушайте. Glyfo справляется и с
целым PDF, и с пачкой картинок за один раз, а всё прочитанное сохраняет в журнале с поиском. QR-коды
и штрихкоды он читает из того же изображения, а на ПК Copilot+ ещё и переводит результат. Всё
происходит на вашем ПК — Glyfo не устанавливает ни одного сетевого соединения. Бесплатно, без
рекламы и без покупок.

### Описание

Glyfo превращает изображения с текстом в текст, с которым можно работать.

Распознавание работает на OCR, встроенном в Windows: не нужны учётная запись, загрузка на сервер и
ожидание ответа. На ПК Copilot+ Glyfo дополнительно использует модель распознавания текста на самом
устройстве для сложных изображений и может перевести результат — тоже без выхода в сеть.

**Пять способов передать изображение**
- Alt+Z — обвести любую область экрана. Ctrl+Shift+R снимает экран целиком. Оба сочетания можно
  переназначить.
- Ctrl+V вставляет изображение из буфера обмена, текст тоже.
- Ctrl+O открывает картинку или PDF; перетаскивание в окно работает так же.
- Правый щелчок по картинке в проводнике — открыть с помощью Glyfo, или отправить через панель
  «Поделиться» из «Фотографий», «Ножниц» или браузера.
- Включите наблюдение за буфером обмена — и каждый снимок, сделанный через Win+Shift+S, читается сам
  собой, а текст остаётся в буфере. По умолчанию выключено, пока вы не попросите.

**Что вы получаете**
- Распознанный текст рядом с изображением, в порядке исходной вёрстки.
- Копирование одним щелчком — или снимок копируется сам, как только распознавание закончилось.
- Сохранение в файл .txt или .md по Ctrl+S.
- Поиск по тексту через Ctrl+F, рядом — счётчик слов и знаков.
- Чтение вслух любым голосом, установленным на вашем ПК.
- «Убрать переносы» собирает жёсткие переносы обратно в абзацы, «Убрать пробелы» удаляет все пробелы
  — именно это обычно требуется китайскому, японскому и корейскому тексту после распознавания.
- Кнопки для ссылок, адресов электронной почты и телефонов, найденных в тексте.
- QR-коды и штрихкоды из того же изображения.
- Журнал с поиском, переживающий перезапуск: снимок недельной давности по-прежнему в одном щелчке.
  Хранится только текст, журнал можно очистить или отключить.

**Больше одной страницы за раз**
- Откройте PDF и читайте его постранично, листая прямо в окне.
- Или прочитайте весь документ сразу и сохраните результат одним файлом либо по файлу на страницу.
- Бросьте пачку картинок в окно и распознайте их все за один проход.

**Вы задаёте, как читать**
- Язык распознавания выбирается из языковых пакетов, установленных в Windows, либо определяется
  автоматически.
- Отдельная настройка исправляет классическую ошибку, когда v1.6.5 читается как vl.6.5: l или I
  становится единицей только там, где рядом стоят разделитель и цифра, так что html5 и IPv6
  остаются нетронутыми.
- Повернуть изображение или выпрямить снятую под углом фотографию перед распознаванием.
- Вписать в окно или показать в натуральную величину; распознать всё изображение или только
  выделенное.

**Не мешает работать**
- Закрытие окна оставляет Glyfo в области уведомлений, и сочетание клавиш продолжает работать.
  «Выход» оттуда завершает программу окончательно. Само это поведение — тоже переключатель.
- Glyfo может запускаться вместе с Windows и сразу уходить в область уведомлений, не открывая окна,
  так что сочетание клавиш доступно сразу после входа в систему. По умолчанию выключено — включаете
  вы.
- Когда окно скрыто, уведомление показывает первую строку только что распознанного текста.
- Окно возвращается того же размера и на то же место, где вы его оставили, — светлое или тёмное, как
  вам удобнее.

**Языки**
Интерфейс доступен на 33 языках и следует языковым настройкам Windows. Распознавание использует
языковые пакеты OCR, установленные на ПК, — добавить их можно в разделе «Параметры › Время и язык ›
Язык и регион».

**Конфиденциальность**
Glyfo не устанавливает сетевых соединений. Изображения, распознанный текст и переводы никогда не
покидают ваш ПК. Ни учётной записи, ни телеметрии, ни рекламы.

### Возможности продукта

- Снимите любую часть экрана выбранным вами сочетанием клавиш и сразу распознайте её, не выходя из приложения, которое читали
- Работает на встроенном в Windows OCR, а на ПК Copilot+ — ещё и на модели распознавания в самом устройстве
- Открыть картинку или PDF, вставить из буфера обмена, перетащить или получить через панель «Поделиться»
- Читает целый PDF или пачку изображений одним пакетом, сохраняя результат одним файлом или по файлу на страницу
- Ведёт журнал с поиском, переживающий перезапуск, и сохраняет любой результат в файл .txt или .md
- Читает QR-коды и штрихкоды из того же изображения
- Переводит на ПК Copilot+ прямо на устройстве, без доступа к сети
- Читает результат вслух любым голосом на ПК, убирает переносы или пробелы — правку, которая нужна тексту CJK
- Остаётся в области уведомлений, поэтому сочетание клавиш работает и после закрытия окна; интерфейс на 33 языках, светлый или тёмный
- Не выходит в интернет: распознанное не покидает ваш ПК

### Подписи к снимкам экрана

1. `01-text-from-a-page.png` — Распознанный текст стоит рядом с изображением в том порядке, в каком он был прочитан: копируйте, слушайте вслух или собирайте обратно в абзацы.
2. `02-any-language.png` — Язык распознавания выбирается из языковых пакетов, установленных в Windows, — или Glyfo определяет его сам.
3. `03-qr-and-barcodes.png` — QR-коды и штрихкоды считываются с того же изображения; отдельное приложение-сканер не нужно.
4. `04-history.png` — Недавние результаты остаются в журнале, поэтому снимок, сделанный несколько минут назад, по-прежнему в одном клике.
5. `05-settings.png` — Язык интерфейса, запуск вместе с Windows, работа в области уведомлений и исправление, из-за которого v1.6.5 не превращается в vl.6.5.
6. `06-pdf-pages.png` — PDF открывается постранично: текущая страница распознаётся сразу, а стрелки проводят по всему остальному документу.
7. `07-batch.png` — Целый документ или стопка изображений проходит за один заход и сохраняется одним файлом либо по файлу на страницу.

### Поисковые запросы

`текст с картинки`, `скриншот в текст`, `PDF в текст`, `распознавание текста`, `сканер QR-кода`, `сканер штрихкодов`, `OCR`

---

## Italiano — Italian

### Descrizione breve

Glyfo estrae il testo da tutto ciò che vedi: uno screenshot, la foto di una pagina, un documento
scansionato, una diapositiva, un fotogramma di un video, un PDF. Premi Alt+Z e traccia un riquadro su
una parte dello schermo, oppure apri un file, incolla dagli appunti, invia un'immagine da un'altra
app. Il testo riconosciuto compare accanto all'immagine, pronto da copiare, salvare, cercare o
ascoltare. Se la cava anche con un PDF intero o una pila di immagini in una sola volta, e conserva
tutto ciò che ha letto in una cronologia consultabile. Legge i codici QR e i codici a barre dalla
stessa immagine e, su un PC Copilot+, ne traduce il risultato. Tutto avviene sul tuo PC: Glyfo non
stabilisce alcuna connessione a Internet. Gratis, senza pubblicità e senza acquisti.

### Descrizione

Glyfo trasforma le immagini di testo in testo che puoi usare.

Il riconoscimento si appoggia all'OCR incluso in Windows: nessun account da creare, nessun
caricamento, nessuna attesa di un server. Su un PC Copilot+, Glyfo usa in più il modello di
riconoscimento del testo sul dispositivo per le immagini difficili e può tradurre il risultato —
anche in questo caso senza rete.

**Cinque modi per far entrare un'immagine**
- Alt+Z traccia un riquadro su una parte qualsiasi dello schermo. Ctrl+Shift+R prende tutto lo
  schermo. Entrambe le scorciatoie sono tue da cambiare.
- Ctrl+V incolla un'immagine dagli appunti, e anche del testo.
- Ctrl+O apre un'immagine o un PDF; trascinarlo nella finestra funziona allo stesso modo.
- Clic destro su un'immagine in Esplora file per aprirla con Glyfo, oppure inviala dal riquadro di
  condivisione di Windows (Foto, Strumento di cattura, browser).
- Attiva il monitoraggio degli appunti e ogni ritaglio fatto con Win+Maiusc+S viene letto da solo,
  con il testo pronto negli appunti. Disattivato finché non lo chiedi.

**Cosa ottieni**
- Il testo riconosciuto accanto all'immagine, nell'ordine in cui era disposto.
- Copia con un clic, oppure lascia che una cattura si copi da sola appena finisce.
- Salvataggio in un file .txt o .md con Ctrl+S.
- Ricerca nel testo con Ctrl+F, con il conteggio di parole e caratteri accanto.
- Lettura ad alta voce con qualsiasi voce installata sul PC.
- «Rimuovi a capo» ricompone le interruzioni di riga forzate in paragrafi, «Rimuovi spazi» elimina
  tutti gli spazi: è ciò di cui il testo cinese, giapponese e coreano ha bisogno dopo il
  riconoscimento.
- Pulsanti per i link, gli indirizzi e-mail e i numeri di telefono trovati nel testo.
- Codici QR e codici a barre dalla stessa immagine.
- Una cronologia consultabile che sopravvive a un riavvio: la cattura della settimana scorsa è ancora
  a un clic. Conserva solo testo, e puoi svuotarla o disattivarla.

**Più di una pagina alla volta**
- Apri un PDF e leggilo una pagina alla volta, scorrendolo nella finestra.
- Oppure leggi tutto il documento in una volta e salva il risultato in un file solo o uno per pagina.
- Trascina una pila di immagini sulla finestra e riconoscile tutte in una sola passata.

**Decidi tu come legge**
- Scegli la lingua di riconoscimento tra i pacchetti installati in Windows, oppure lascia decidere a
  Glyfo.
- Un'opzione corregge il classico errore dell'OCR che legge v1.6.5 come vl.6.5: una l o una I
  diventa 1 solo dove accanto ci sono un separatore e una cifra, così html5 e IPv6 restano intatti.
- Ruota un'immagine, o raddrizza una foto scattata storta, prima di leggerla.
- Adatta alla finestra o visualizza a dimensione reale; riconosci l'immagine intera o solo la
  selezione.

**Non sta tra i piedi**
- Chiudendo la finestra Glyfo resta nell'area di notifica e la scorciatoia di cattura continua a
  funzionare. «Esci» da lì lo chiude davvero. Questo comportamento è a sua volta un'impostazione.
- Può avviarsi con Windows e andare dritto nell'area di notifica senza aprire finestre, così la
  scorciatoia è pronta dal momento dell'accesso. Disattivato per impostazione predefinita: sei tu
  ad attivarlo.
- A finestra nascosta, una notifica mostra la prima riga di ciò che è stato appena riconosciuto.
- La finestra torna della dimensione e nel punto in cui l'hai lasciata, chiara o scura come preferisci.

**Lingue**
L'interfaccia è disponibile in 33 lingue e segue l'impostazione della lingua di Windows. Il
riconoscimento usa i pacchetti OCR installati sul PC: se ne aggiungono da Impostazioni › Data/ora e
lingua › Lingua e area geografica.

**Privacy**
Glyfo non stabilisce connessioni di rete. Immagini, testo riconosciuto e traduzioni non lasciano mai
il tuo PC. Nessun account, nessuna telemetria, nessuna pubblicità.

### Funzionalità del prodotto

- Cattura una parte qualsiasi dello schermo con una scorciatoia scelta da te e riconoscila subito, senza uscire dall'app che stavi leggendo
- Si basa sull'OCR integrato in Windows e, sui PC Copilot+, sul modello di riconoscimento sul dispositivo
- Apri un'immagine o un PDF, incolla dagli appunti, trascina e rilascia o ricevi dal riquadro di condivisione di Windows
- Legge un PDF intero o una pila di immagini in un solo lotto, salvando il risultato in un file o uno per pagina
- Tiene una cronologia consultabile che sopravvive a un riavvio, e salva qualsiasi risultato in un file .txt o .md
- Legge codici QR e codici a barre dalla stessa immagine
- Traduce sui PC Copilot+, sul dispositivo, senza accesso alla rete
- Legge il risultato ad alta voce con qualsiasi voce sul PC, e rimuove gli a capo o gli spazi di cui il testo CJK ha bisogno
- Resta nell'area di notifica, così la scorciatoia funziona anche dopo aver chiuso la finestra; interfaccia in 33 lingue, chiara o scura
- Non si collega a Internet: ciò che riconosci non lascia il tuo PC

### Didascalie degli screenshot

1. `01-text-from-a-page.png` — Il testo riconosciuto compare accanto all'immagine, nell'ordine in cui è stato letto: pronto da copiare, ascoltare o ricomporre in paragrafi.
2. `02-any-language.png` — La lingua di riconoscimento si sceglie tra i pacchetti installati in Windows — oppure decide Glyfo da solo.
3. `03-qr-and-barcodes.png` — Codici QR e codici a barre vengono letti dalla stessa immagine: non serve un'altra app per la scansione.
4. `04-history.png` — I risultati recenti restano nella cronologia, così una cattura di qualche minuto fa è ancora a un clic.
5. `05-settings.png` — Lingua dell'interfaccia, avvio con Windows, permanenza nell'area di notifica e la correzione che evita che v1.6.5 diventi vl.6.5.
6. `06-pdf-pages.png` — Un PDF si apre pagina per pagina: quella che hai davanti viene riconosciuta subito e le frecce percorrono il resto del documento.
7. `07-batch.png` — Un intero documento o una pila di immagini passa in una sola volta, salvato in un unico file o in un file per pagina.

### Termini di ricerca

`testo da immagine`, `screenshot in testo`, `PDF in testo`, `riconoscimento testo`, `leggere codice QR`, `scanner codice a barre`, `OCR`

---

## Polski — Polish

### Krótki opis

Glyfo wyciąga tekst ze wszystkiego, co widzisz: ze zrzutu ekranu, zdjęcia strony, skanu, slajdu,
klatki filmu, pliku PDF. Naciśnij Alt+Z i zaznacz ramką fragment ekranu albo otwórz plik, wklej ze
schowka, prześlij obraz z innej aplikacji. Rozpoznany tekst pojawia się obok obrazu — gotowy do
skopiowania, zapisania, przeszukania lub odczytania na głos. Glyfo poradzi sobie też z całym plikiem
PDF albo ze stosem obrazów za jednym razem, a wszystko, co przeczytał, trzyma w przeszukiwalnej
historii. Odczytuje kody QR i kody kreskowe z tego samego obrazu, a na komputerze Copilot+ tłumaczy
wynik. Wszystko dzieje się na Twoim komputerze — Glyfo nie nawiązuje żadnych połączeń internetowych.
Bezpłatnie, bez reklam i bez zakupów.

### Opis

Glyfo zamienia obrazy z tekstem w tekst, którego można używać.

Rozpoznawanie działa na OCR wbudowanym w Windows: bez zakładania konta, bez wysyłania czegokolwiek i
bez czekania na serwer. Na komputerze Copilot+ Glyfo dodatkowo korzysta z modelu rozpoznawania
tekstu działającego na urządzeniu przy trudniejszych obrazach i potrafi przetłumaczyć wynik —
również bez sieci.

**Pięć sposobów na wczytanie obrazu**
- Alt+Z zaznacza ramką dowolny fragment ekranu. Ctrl+Shift+R robi zrzut całego ekranu. Oba skróty
  możesz zmienić.
- Ctrl+V wkleja obraz ze schowka, tekst również.
- Ctrl+O otwiera obraz lub plik PDF; przeciągnięcie go do okna działa tak samo.
- Kliknij obraz prawym przyciskiem w Eksploratorze plików i otwórz go w Glyfo albo prześlij przez
  panel udostępniania Windows (Zdjęcia, Narzędzie Wycinanie, przeglądarka).
- Włącz nasłuchiwanie schowka, a każdy wycinek zrobiony przez Win+Shift+S zostanie odczytany sam, z
  tekstem gotowym w schowku. Domyślnie wyłączone, dopóki o to nie poprosisz.

**Co dostajesz**
- Rozpoznany tekst obok obrazu, w kolejności pierwotnego układu.
- Kopiowanie jednym kliknięciem albo automatyczne kopiowanie zaraz po zakończeniu zrzutu.
- Zapis do pliku .txt lub .md skrótem Ctrl+S.
- Szukanie w tekście przez Ctrl+F, z liczbą słów i znaków obok.
- Czytanie na głos dowolnym głosem zainstalowanym na komputerze.
- „Usuń podziały wierszy” skleja twarde złamania z powrotem w akapity, a „Usuń spacje” kasuje
  wszystkie spacje — tego zwykle wymaga tekst chiński, japoński i koreański po rozpoznaniu.
- Przyciski do linków, adresów e-mail i numerów telefonu znalezionych w tekście.
- Kody QR i kody kreskowe z tego samego obrazu.
- Przeszukiwalna historia, która przetrwa ponowne uruchomienie: zrzut sprzed tygodnia wciąż jest o
  jedno kliknięcie. Trzyma wyłącznie tekst, a Ty możesz ją opróżnić albo wyłączyć.

**Więcej niż jedna strona naraz**
- Otwórz PDF i czytaj go strona po stronie, przewijając w oknie.
- Albo przeczytaj cały dokument za jednym razem i zapisz wynik jako jeden plik lub plik na stronę.
- Upuść stos obrazów na okno i rozpoznaj je wszystkie w jednym przebiegu.

**To Ty decydujesz, jak czyta**
- Język rozpoznawania wybierasz spośród pakietów zainstalowanych w Windows albo zostawiasz decyzję
  Glyfo.
- Osobna opcja naprawia klasyczny błąd OCR, przez który v1.6.5 czytane jest jako vl.6.5: l lub I
  zamienia się w 1 tylko tam, gdzie obok stoi separator i cyfra, więc html5 i IPv6 pozostają
  nietknięte.
- Obróć obraz albo wyprostuj zdjęcie zrobione pod kątem, zanim je odczytasz.
- Dopasowanie do okna albo podgląd w rzeczywistym rozmiarze; rozpoznawanie całego obrazu lub tylko
  zaznaczenia.

**Nie wchodzi w drogę**
- Po zamknięciu okna Glyfo zostaje w obszarze powiadomień, a skrót do zrzutu nadal działa. Dopiero
  „Zakończ” zamyka program na dobre. Samo to zachowanie też jest przełącznikiem.
- Glyfo może uruchamiać się razem z Windows i od razu przechodzić do obszaru powiadomień, bez
  otwierania okna — skrót działa od momentu zalogowania. Domyślnie wyłączone; włączasz je sam.
- Gdy okno jest ukryte, powiadomienie pokazuje pierwszy wiersz właśnie rozpoznanego tekstu.
- Okno wraca w rozmiarze i w miejscu, w którym je zostawiłeś — jasne albo ciemne, jak wolisz.

**Języki**
Interfejs jest dostępny w 33 językach i podąża za ustawieniem języka Windows. Rozpoznawanie korzysta
z pakietów OCR zainstalowanych na komputerze — kolejne dodasz w Ustawienia › Czas i język › Język i
region.

**Prywatność**
Glyfo nie nawiązuje połączeń sieciowych. Obrazy, rozpoznany tekst i tłumaczenia nigdy nie opuszczają
Twojego komputera. Bez konta, bez telemetrii, bez reklam.

### Funkcje produktu

- Zaznacz dowolny fragment ekranu wybranym przez siebie skrótem i rozpoznaj go od razu, nie wychodząc z aplikacji, którą właśnie czytasz
- Działa na OCR wbudowanym w Windows, a na komputerach Copilot+ dodatkowo na modelu rozpoznawania na urządzeniu
- Otwórz obraz lub PDF, wklej ze schowka, przeciągnij i upuść albo odbierz przez panel udostępniania Windows
- Czyta cały PDF albo stos obrazów w jednej partii, zapisując wynik jako jeden plik lub plik na stronę
- Prowadzi przeszukiwalną historię, która przetrwa ponowne uruchomienie, i zapisuje każdy wynik do pliku .txt lub .md
- Odczytuje kody QR i kody kreskowe z tego samego obrazu
- Tłumaczy na komputerach Copilot+, na urządzeniu, bez dostępu do sieci
- Czyta wynik na głos dowolnym głosem na komputerze i usuwa podziały wierszy albo spacje, których wymaga tekst CJK
- Zostaje w obszarze powiadomień, więc skrót działa także po zamknięciu okna; interfejs w 33 językach, jasny albo ciemny
- Nie łączy się z internetem: to, co rozpoznajesz, nie opuszcza Twojego komputera

### Podpisy zrzutów ekranu

1. `01-text-from-a-page.png` — Rozpoznany tekst stoi obok obrazu w kolejności, w jakiej został odczytany: do skopiowania, odczytania na głos albo złożenia z powrotem w akapity.
2. `02-any-language.png` — Język rozpoznawania wybierasz spośród pakietów zainstalowanych w Windows — albo Glyfo ustala go sam.
3. `03-qr-and-barcodes.png` — Kody QR i kreskowe są odczytywane z tego samego obrazu; osobna aplikacja do skanowania nie jest potrzebna.
4. `04-history.png` — Ostatnie wyniki zostają w historii, więc zrzut sprzed kilku minut wciąż jest o jedno kliknięcie.
5. `05-settings.png` — Język interfejsu, uruchamianie z Windows, pozostawanie w obszarze powiadomień i poprawka, dzięki której v1.6.5 nie staje się vl.6.5.
6. `06-pdf-pages.png` — PDF otwiera się strona po stronie: ta na wierzchu zostaje odczytana od razu, a strzałki prowadzą przez resztę dokumentu.
7. `07-batch.png` — Cały dokument albo stos obrazów przechodzi za jednym razem — zapisany jako jeden plik lub po pliku na stronę.

### Wyszukiwane hasła

`tekst ze zdjęcia`, `zrzut ekranu na tekst`, `PDF na tekst`, `rozpoznawanie tekstu`, `czytnik kodów QR`, `skaner kodów kreskowych`, `OCR`

---

## Nederlands — Dutch

### Korte beschrijving

Glyfo haalt tekst uit alles wat je kunt zien: een schermafbeelding, een foto van een bladzijde, een
scan, een dia, een videobeeld, een PDF. Druk op Alt+Z en trek een kader om een deel van het scherm,
of open een bestand, plak vanaf het klembord, stuur een afbeelding vanuit een andere app. De herkende
tekst verschijnt naast de afbeelding, klaar om te kopiëren, op te slaan, te doorzoeken of voor te
laten lezen. Het neemt ook een hele PDF of een stapel afbeeldingen in één keer, en bewaart alles wat
het gelezen heeft in een doorzoekbare geschiedenis. Het leest QR-codes en streepjescodes uit dezelfde
afbeelding, en op een Copilot+ pc vertaalt het het resultaat. Alles gebeurt op je eigen pc — Glyfo
maakt geen enkele internetverbinding. Gratis, zonder advertenties en zonder aankopen.

### Beschrijving

Glyfo maakt van afbeeldingen met tekst weer tekst waarmee je kunt werken.

De herkenning draait op de OCR die in Windows zit: geen account, geen upload, geen wachten op een
server. Op een Copilot+ pc gebruikt Glyfo daarnaast het tekstherkenningsmodel op het apparaat voor
lastigere afbeeldingen en kan het het resultaat vertalen — ook dat zonder netwerk.

**Vijf manieren om een afbeelding binnen te krijgen**
- Alt+Z trekt een kader om een willekeurig deel van het scherm. Ctrl+Shift+R neemt het hele scherm.
  Beide sneltoetsen mag je zelf veranderen.
- Ctrl+V plakt een afbeelding vanaf het klembord, tekst ook.
- Ctrl+O opent een afbeelding of een PDF; het in het venster slepen werkt net zo goed.
- Klik met de rechtermuisknop op een afbeelding in Verkenner en open die met Glyfo, of stuur hem via
  het deelvenster van Windows (Foto's, Knipprogramma, browser).
- Zet klembordbewaking aan en elke knip die je met Win+Shift+S maakt, wordt vanzelf gelezen, met de
  tekst klaar op het klembord. Uit totdat je erom vraagt.

**Wat je terugkrijgt**
- De herkende tekst naast de afbeelding, in de volgorde van de oorspronkelijke opmaak.
- Kopiëren met één klik, of een opname zichzelf laten kopiëren zodra hij klaar is.
- Opslaan in een .txt- of .md-bestand met Ctrl+S.
- Zoeken in de tekst met Ctrl+F, met het aantal woorden en tekens ernaast.
- Voorlezen met elke stem die op je pc is geïnstalleerd.
- ‘Regeleinden verwijderen’ voegt harde regelovergangen weer samen tot alinea's en ‘Spaties
  verwijderen’ haalt elke spatie weg — precies wat Chinese, Japanse en Koreaanse tekst na herkenning
  nodig heeft.
- Knoppen voor de links, e-mailadressen en telefoonnummers die in de tekst gevonden zijn.
- QR-codes en streepjescodes uit dezelfde afbeelding.
- Een doorzoekbare geschiedenis die een herstart overleeft: de opname van vorige week is nog één klik
  weg. Er wordt alleen tekst bewaard, en je kunt hem legen of uitzetten.

**Meer dan één bladzijde tegelijk**
- Open een PDF en lees hem bladzijde voor bladzijde, bladerend in het venster.
- Of lees het hele document in één keer en sla het resultaat op als één bestand of één per bladzijde.
- Zet een stapel afbeeldingen op het venster en herken ze allemaal in één ronde.

**Jij bepaalt hoe het leest**
- Kies de herkenningstaal uit de taalpakketten die in Windows zijn geïnstalleerd, of laat Glyfo
  kiezen.
- Een optie herstelt de klassieke OCR-fout waarbij v1.6.5 als vl.6.5 wordt gelezen: een l of I wordt
  alleen een 1 waar er een scheidingsteken en een cijfer naast staan, dus html5 en IPv6 blijven
  ongemoeid.
- Een afbeelding draaien, of een scheef genomen foto rechtzetten, voordat je hem laat lezen.
- Passend maken aan het venster of op ware grootte bekijken; de hele afbeelding herkennen of alleen
  de selectie.

**Het zit niet in de weg**
- Het venster sluiten laat Glyfo in het systeemvak achter, zodat de sneltoets blijft werken.
  ‘Afsluiten’ daar stopt het echt. Dat gedrag is zelf ook een instelling.
- Glyfo kan met Windows meestarten en meteen naar het systeemvak gaan zonder venster, zodat de
  sneltoets werkt vanaf het moment dat je je aanmeldt. Standaard uit; jij zet het aan.
- Is het venster verborgen, dan toont een melding de eerste regel van wat er zojuist is herkend.
- Het venster komt terug op de grootte en de plek waar je het achterliet, licht of donker zoals je
  wilt.

**Talen**
De interface is er in 33 talen en volgt je taalinstelling in Windows. De tekstherkenning gebruikt de
OCR-taalpakketten die op je pc staan — meer voeg je toe via Instellingen › Tijd en taal › Taal en
regio.

**Privacy**
Glyfo maakt geen netwerkverbindingen. Afbeeldingen, herkende tekst en vertalingen verlaten je pc
nooit. Geen account, geen telemetrie, geen advertenties.

### Productfuncties

- Leg met een zelfgekozen sneltoets een willekeurig deel van het scherm vast en herken het meteen, zonder de app te verlaten waarin je aan het lezen was
- Draait op de OCR die in Windows is ingebouwd, op Copilot+ pc's aangevuld met het herkenningsmodel op het apparaat
- Een afbeelding of PDF openen, plakken vanaf het klembord, slepen en neerzetten of ontvangen via het deelvenster van Windows
- Leest een hele PDF of een stapel afbeeldingen in één ronde, met het resultaat als één bestand of één per bladzijde
- Houdt een doorzoekbare geschiedenis bij die een herstart overleeft, en slaat elk resultaat op als .txt- of .md-bestand
- Leest QR-codes en streepjescodes uit dezelfde afbeelding
- Vertaalt op Copilot+ pc's, op het apparaat zelf, zonder netwerktoegang
- Leest het resultaat voor met elke stem op de pc, en verwijdert regeleinden of de spaties die CJK-tekst nodig heeft
- Blijft in het systeemvak, zodat de sneltoets ook na het sluiten van het venster werkt; interface in 33 talen, licht of donker
- Maakt geen internetverbinding: wat je herkent verlaat je pc niet

### Bijschriften bij schermafbeeldingen

1. `01-text-from-a-page.png` — De herkende tekst staat naast de afbeelding, in de volgorde waarin hij gelezen is: klaar om te kopiëren, voor te laten lezen of terug te voegen tot alinea's.
2. `02-any-language.png` — De herkenningstaal kies je uit de taalpakketten die in Windows zijn geïnstalleerd — of Glyfo bepaalt hem zelf.
3. `03-qr-and-barcodes.png` — QR-codes en streepjescodes worden uit dezelfde afbeelding gelezen; een aparte scan-app is niet nodig.
4. `04-history.png` — Recente resultaten blijven in de geschiedenis staan, dus een opname van een paar minuten geleden is nog één klik weg.
5. `05-settings.png` — Interfacetaal, meestarten met Windows, in het systeemvak blijven, en de correctie die voorkomt dat v1.6.5 vl.6.5 wordt.
6. `06-pdf-pages.png` — Een pdf gaat pagina voor pagina open: de pagina die je ziet wordt meteen gelezen en met de pijlen loop je door de rest.
7. `07-batch.png` — Een heel document of een stapel afbeeldingen gaat er in één keer doorheen, opgeslagen als één bestand of als één bestand per pagina.

### Zoektermen

`tekst uit afbeelding`, `schermafbeelding naar tekst`, `PDF naar tekst`, `tekstherkenning`, `QR-code lezen`, `streepjescode scannen`, `OCR`

---

## Čeština — Czech

### Stručný popis

Glyfo vytáhne text ze všeho, co vidíte: ze snímku obrazovky, z fotky stránky, ze skenu, ze snímku
prezentace, z videa i z PDF. Stiskněte Alt+Z a orámujte část obrazovky, nebo otevřete soubor, vložte
ze schránky, pošlete obrázek z jiné aplikace. Rozpoznaný text se objeví vedle obrázku — připravený ke
zkopírování, uložení, prohledání nebo přečtení nahlas. Zvládne i celé PDF nebo hromadu obrázků
najednou a všechno přečtené si nechá v prohledávatelné historii. Ze stejného obrázku přečte i QR kódy
a čárové kódy a na počítači Copilot+ výsledek přeloží. Všechno probíhá ve vašem počítači — Glyfo se
vůbec nepřipojuje k internetu. Zdarma, bez reklam a bez nákupů.

### Popis

Glyfo mění obrázky s textem v text, se kterým se dá pracovat.

Rozpoznávání běží na OCR, které je součástí Windows: žádný účet, žádné nahrávání, žádné čekání na
server. Na počítači Copilot+ Glyfo u složitějších obrázků navíc využije model rozpoznávání textu
přímo v zařízení a dokáže výsledek přeložit — také bez sítě.

**Pět způsobů, jak dostat obrázek dovnitř**
- Alt+Z orámuje libovolnou část obrazovky. Ctrl+Shift+R sejme celou obrazovku. Obě zkratky si můžete
  změnit.
- Ctrl+V vloží obrázek ze schránky, text také.
- Ctrl+O otevře obrázek nebo PDF; přetažení do okna funguje stejně.
- Klepněte na obrázek v Průzkumníku pravým tlačítkem a otevřete ho v Glyfo, nebo ho pošlete přes
  panel sdílení Windows (Fotky, Nástroj pro vystřižení, prohlížeč).
- Zapněte sledování schránky a každý výstřižek pořízený přes Win+Shift+S se přečte sám, text zůstane
  ve schránce. Ve výchozím stavu vypnuto, dokud si o to neřeknete.

**Co dostanete zpět**
- Rozpoznaný text vedle obrázku, v pořadí původního rozvržení.
- Kopírování jedním klepnutím, nebo ať se snímek zkopíruje sám, jakmile je hotový.
- Uložení do souboru .txt nebo .md klávesami Ctrl+S.
- Hledání v textu přes Ctrl+F, vedle toho počet slov a znaků.
- Čtení nahlas libovolným hlasem nainstalovaným v počítači.
- „Odstranit zalomení“ spojí tvrdé konce řádků zpět do odstavců, „Odstranit mezery“ smaže všechny
  mezery — právě to čínský, japonský a korejský text po rozpoznání obvykle potřebuje.
- Tlačítka pro odkazy, e-mailové adresy a telefonní čísla nalezená v textu.
- QR kódy a čárové kódy ze stejného obrázku.
- Prohledávatelná historie, která přežije restart: snímek z minulého týdne je pořád jedno klepnutí
  daleko. Ukládá jen text a můžete ji vysypat nebo vypnout.

**Víc než jedna stránka najednou**
- Otevřete PDF a čtěte ho stránku po stránce, s listováním přímo v okně.
- Nebo přečtěte celý dokument naráz a výsledek uložte do jednoho souboru či po jednom na stránku.
- Pusťte na okno hromadu obrázků a rozpoznejte je všechny v jediném průchodu.

**Vy určujete, jak čte**
- Jazyk rozpoznávání vyberete z jazykových sad nainstalovaných ve Windows, nebo ho nechte na Glyfo.
- Volba opravuje klasickou chybu OCR, kdy se v1.6.5 čte jako vl.6.5: l nebo I se změní na 1 jen tam,
  kde vedle stojí oddělovač a číslice, takže html5 a IPv6 zůstanou nedotčené.
- Otočit obrázek nebo srovnat nakřivo vyfocenou stránku, ještě než se přečte.
- Přizpůsobit oknu nebo zobrazit ve skutečné velikosti; rozpoznat celý obrázek, nebo jen výběr.

**Nepřekáží**
- Zavřením okna Glyfo zůstane v oznamovací oblasti a klávesová zkratka dál funguje. Teprve
  „Ukončit“ ho zavře nadobro. Celé toto chování je samo o sobě přepínač.
- Glyfo se může spouštět s Windows a jít rovnou do oznamovací oblasti bez okna, takže zkratka
  funguje od chvíle přihlášení. Ve výchozím stavu vypnuto; zapnete si ho sami.
- Když je okno skryté, oznámení ukáže první řádek právě rozpoznaného textu.
- Okno se vrátí ve velikosti a na místě, kde jste ho nechali — světlé, nebo tmavé, jak chcete.

**Jazyky**
Rozhraní je k dispozici ve 33 jazycích a řídí se nastavením jazyka ve Windows. Rozpoznávání používá
jazykové sady OCR nainstalované v počítači — další přidáte v Nastavení › Čas a jazyk › Jazyk a
oblast.

**Soukromí**
Glyfo nenavazuje žádná síťová spojení. Obrázky, rozpoznaný text ani překlady nikdy neopustí váš
počítač. Žádný účet, žádná telemetrie, žádná reklama.

### Funkce produktu

- Sejměte vlastní zvolenou zkratkou libovolnou část obrazovky a hned ji rozpoznejte, aniž byste opustili aplikaci, kterou jste právě četli
- Staví na OCR vestavěném ve Windows, na počítačích Copilot+ navíc na modelu rozpoznávání v zařízení
- Otevřít obrázek nebo PDF, vložit ze schránky, přetáhnout nebo přijmout přes panel sdílení Windows
- Přečte celé PDF nebo hromadu obrázků v jedné dávce a uloží výsledek do jednoho souboru či po jednom na stránku
- Vede prohledávatelnou historii, která přežije restart, a uloží každý výsledek do souboru .txt nebo .md
- Přečte ze stejného obrázku QR kódy i čárové kódy
- Na počítačích Copilot+ překládá přímo v zařízení, bez přístupu k síti
- Přečte výsledek nahlas libovolným hlasem v počítači a odstraní zalomení řádků či mezery, které text CJK potřebuje
- Zůstává v oznamovací oblasti, takže zkratka funguje i po zavření okna; rozhraní ve 33 jazycích, světlé nebo tmavé
- Nepřipojuje se k internetu: co rozpoznáte, neopustí váš počítač

### Popisky snímků obrazovky

1. `01-text-from-a-page.png` — Rozpoznaný text stojí vedle obrázku v pořadí, ve kterém byl přečten: ke zkopírování, přečtení nahlas nebo spojení zpět do odstavců.
2. `02-any-language.png` — Jazyk rozpoznávání vyberete z jazykových sad nainstalovaných ve Windows — nebo si ho Glyfo určí sám.
3. `03-qr-and-barcodes.png` — QR kódy a čárové kódy se čtou ze stejného obrázku; samostatná aplikace na skenování není potřeba.
4. `04-history.png` — Poslední výsledky zůstávají v historii, takže snímek z doby před pár minutami je pořád jedno klepnutí daleko.
5. `05-settings.png` — Jazyk rozhraní, spouštění s Windows, setrvání v oznamovací oblasti a oprava, díky které se z v1.6.5 nestane vl.6.5.
6. `06-pdf-pages.png` — PDF se otevírá po stránkách: ta, kterou máte před sebou, se přečte hned a šipkami projdete zbytek dokumentu.
7. `07-batch.png` — Celý dokument nebo hromádka obrázků projde na jeden zátah, uložená jako jeden soubor nebo jeden soubor na stránku.

### Hledané výrazy

`text z obrázku`, `snímek obrazovky na text`, `PDF do textu`, `rozpoznávání textu`, `čtečka QR kódů`, `skener čárových kódů`, `OCR`

---

## Türkçe — Turkish

### Kısa açıklama

Glyfo gördüğünüz her şeyden metni çıkarır: ekran görüntüsü, bir sayfanın fotoğrafı, taranmış belge,
sunu slaydı, video karesi, PDF. Alt+Z tuşuna basıp ekranın bir bölümünü çerçeveleyin; ya da dosya
açın, panodan yapıştırın, başka bir uygulamadan görsel gönderin. Tanınan metin görselin yanında
belirir; kopyalamaya, kaydetmeye, aramaya veya sesli okutmaya hazırdır. Bütün bir PDF'i ya da bir
yığın görseli tek seferde işler ve okuduğu her şeyi aranabilir bir geçmişte tutar. Aynı görseldeki QR
kodlarını ve barkodları da okur, Copilot+ bilgisayarlarda sonucu çevirir. Her şey kendi
bilgisayarınızda olur — Glyfo hiçbir internet bağlantısı kurmaz. Ücretsiz, reklamsız ve satın alma
içermez.

### Açıklama

Glyfo metin içeren görselleri kullanabileceğiniz metne dönüştürür.

Tanıma, Windows ile birlikte gelen OCR üzerinde çalışır: hesap açmak, dosya yüklemek ya da sunucu
beklemek yok. Copilot+ bilgisayarlarda Glyfo, zor görseller için cihaz üzerindeki metin tanıma
modelini de kullanır ve sonucu çevirebilir — bu da yine çevrimdışı gerçekleşir.

**Görseli içeri almanın beş yolu**
- Alt+Z ile ekranın herhangi bir bölümünü çerçeveleyin. Ctrl+Shift+R ekranın tamamını alır. İki
  kısayolu da siz değiştirebilirsiniz.
- Ctrl+V panodaki görseli yapıştırır, metni de.
- Ctrl+O bir görsel ya da PDF açar; pencereye sürüklemek de aynı işi görür.
- Dosya Gezgini'nde bir görsele sağ tıklayıp Glyfo ile açın ya da Windows paylaşım panelinden
  gönderin (Fotoğraflar, Ekran Alıntısı Aracı, tarayıcı).
- Pano izlemeyi açın; Win+Shift+S ile aldığınız her alıntı kendiliğinden okunur, metin panoda
  yapıştırmaya hazır kalır. Siz istemedikçe kapalıdır.

**Elinize ne geçer**
- Tanınan metin görselin yanında, özgün yerleşim sırasıyla.
- Tek tıkla kopyalama ya da alıntı biter bitmez kendiliğinden kopyalanması.
- Ctrl+S ile .txt veya .md dosyasına kaydetme.
- Ctrl+F ile metin içinde arama, yanında sözcük ve karakter sayısı.
- Bilgisayarınızda yüklü herhangi bir sesle sesli okuma.
- “Satır sonlarını kaldır” zorunlu satır sonlarını paragraflara geri birleştirir, “Boşlukları
  kaldır” tüm boşlukları siler — Çince, Japonca ve Korece metnin tanıma sonrasında genelde ihtiyaç
  duyduğu şey budur.
- Metinde bulunan bağlantılar, e-posta adresleri ve telefon numaraları için düğmeler.
- Aynı görselden QR kodları ve barkodlar.
- Yeniden başlatmadan sonra da duran, aranabilir bir geçmiş: geçen haftaki alıntı hâlâ bir tık
  uzağınızda. Yalnızca metin tutar; boşaltabilir ya da kapatabilirsiniz.

**Aynı anda birden çok sayfa**
- Bir PDF açın ve pencerede sayfa sayfa gezinerek okuyun.
- Ya da belgenin tamamını tek seferde okuyup sonucu tek dosya veya sayfa başına bir dosya olarak
  kaydedin.
- Pencereye bir yığın görsel bırakın ve hepsini tek geçişte tanıyın.

**Nasıl okuyacağına siz karar verirsiniz**
- Tanıma dilini Windows'ta yüklü dil paketleri arasından seçin ya da kararı Glyfo'ya bırakın.
- Bir seçenek, OCR'nin v1.6.5'i vl.6.5 diye okuduğu klasik hatayı düzeltir: l ya da I yalnızca
  yanında bir ayırıcı ve bir rakam varken 1'e dönüşür, böylece html5 ve IPv6 olduğu gibi kalır.
- Okutmadan önce görseli döndürün ya da eğri çekilmiş bir fotoğrafı düzeltin.
- Pencereye sığdırın veya gerçek boyutta görün; görselin tamamını ya da yalnızca seçili bölümü
  tanıyın.

**Ayak altında dolaşmaz**
- Pencereyi kapattığınızda Glyfo bildirim alanında kalır ve alıntı kısayolu çalışmaya devam eder.
  Oradan “Çıkış” demek onu tamamen kapatır. Bu davranışın kendisi de bir ayardır.
- Glyfo Windows ile birlikte başlayıp pencere açmadan doğrudan bildirim alanına geçebilir; böylece
  kısayol oturum açtığınız andan itibaren hazırdır. Varsayılan olarak kapalıdır; açan siz olursunuz.
- Pencere gizliyken bir bildirim, yeni tanınan metnin ilk satırını gösterir.
- Pencere, bıraktığınız boyut ve konumda geri gelir; açık ya da koyu, tercihiniz nasılsa.

**Diller**
Arayüz 33 dilde sunulur ve Windows dil ayarınızı izler. Metin tanıma, bilgisayarınızda yüklü OCR dil
paketlerini kullanır — yenilerini Ayarlar › Saat ve dil › Dil ve bölge altından eklersiniz.

**Gizlilik**
Glyfo ağ bağlantısı kurmaz. Görseller, tanınan metin ve çeviriler bilgisayarınızdan asla çıkmaz.
Hesap yok, telemetri yok, reklam yok.

### Ürün özellikleri

- Kendi seçtiğiniz bir kısayolla ekranın herhangi bir bölümünü alın ve okumakta olduğunuz uygulamadan çıkmadan anında tanıyın
- Windows'un yerleşik OCR'si üzerinde çalışır, Copilot+ bilgisayarlarda cihaz üzerindeki tanıma modeliyle desteklenir
- Görsel ya da PDF açma, panodan yapıştırma, sürükle bırak ya da Windows paylaşım panelinden alma
- Bütün bir PDF'i veya bir yığın görseli tek toplu işte okur, sonucu tek dosya ya da sayfa başına bir dosya olarak kaydeder
- Yeniden başlatmadan sonra da duran aranabilir bir geçmiş tutar ve her sonucu .txt veya .md dosyasına kaydeder
- Aynı görselden QR kodlarını ve barkodları okur
- Copilot+ bilgisayarlarda cihaz üzerinde, ağ erişimi olmadan çeviri yapar
- Sonucu bilgisayardaki herhangi bir sesle sesli okur, satır sonlarını ya da CJK metninin gerektirdiği boşlukları kaldırır
- Bildirim alanında kalır, böylece pencere kapandıktan sonra da kısayol çalışır; 33 dilde arayüz, açık ya da koyu
- İnternete hiç bağlanmaz: tanıdığınız hiçbir şey bilgisayarınızdan çıkmaz

### Ekran görüntüsü açıklamaları

1. `01-text-from-a-page.png` — Tanınan metin, okunduğu sırayla görselin yanında durur: kopyalamaya, sesli dinlemeye ya da paragraflara geri birleştirmeye hazır.
2. `02-any-language.png` — Tanıma dili Windows'ta yüklü dil paketleri arasından seçilir — ya da Glyfo kendisi belirler.
3. `03-qr-and-barcodes.png` — QR kodları ve barkodlar aynı görselden okunur; ayrı bir tarayıcı uygulamasına gerek yoktur.
4. `04-history.png` — Son sonuçlar geçmişte kalır; birkaç dakika önce aldığınız bir alıntı hâlâ bir tık uzağınızdadır.
5. `05-settings.png` — Arayüz dili, Windows ile başlatma, kapatınca bildirim alanında kalma ve v1.6.5'in vl.6.5 olmasını engelleyen düzeltme.
6. `06-pdf-pages.png` — PDF sayfa sayfa açılır: önünüzdeki sayfa hemen okunur, oklarla belgenin geri kalanını gezersiniz.
7. `07-batch.png` — Bütün bir belge ya da bir yığın görsel tek seferde geçer; tek dosya olarak da, sayfa başına bir dosya olarak da kaydedilir.

### Arama terimleri

`görselden metin`, `ekran görüntüsü metne`, `PDF metne çevirme`, `metin tanıma`, `QR kod okuyucu`, `barkod tarayıcı`, `OCR`

---

## Svenska — Swedish

### Kort beskrivning

Glyfo plockar ut texten ur allt du kan se: en skärmbild, ett foto av en sida, en inskannad handling,
en presentationsbild, en filmruta, en PDF. Tryck Alt+Z och rama in en del av skärmen, eller öppna en
fil, klistra in från Urklipp, skicka en bild från en annan app. Den avlästa texten hamnar bredvid
bilden, klar att kopiera, spara, söka i eller lyssna på. Den klarar också en hel PDF eller en hög med
bilder på en gång, och behåller allt den läst i en sökbar historik. Den läser QR-koder och
streckkoder ur samma bild, och på en Copilot+-dator översätter den resultatet. Allt sker på din egen
dator — Glyfo upprättar inga internetanslutningar. Gratis, utan annonser och utan köp.

### Beskrivning

Glyfo gör om bilder med text till text du kan använda.

Avläsningen bygger på den OCR som redan finns i Windows: inget konto, ingen uppladdning, ingen
väntan på en server. På en Copilot+-dator använder Glyfo dessutom textigenkänningsmodellen på
enheten för svårare bilder och kan översätta resultatet — även det utan nätverk.

**Fem sätt att få in en bild**
- Alt+Z ramar in vilken del av skärmen som helst. Ctrl+Shift+R tar hela skärmen. Båda
  kortkommandona får du ändra själv.
- Ctrl+V klistrar in en bild från Urklipp, text också.
- Ctrl+O öppnar en bild eller en PDF; att dra in den i fönstret fungerar lika bra.
- Högerklicka en bild i Utforskaren och öppna den med Glyfo, eller skicka den via delningsfönstret i
  Windows (Foton, Skärmklipp, webbläsaren).
- Slå på urklippsbevakning så läses varje klipp du gör med Win+Skift+S av på egen hand, med texten
  kvar i Urklipp. Avstängt tills du ber om det.

**Vad du får tillbaka**
- Den avlästa texten bredvid bilden, i den ordning den låg i.
- Kopiera med ett klick, eller låt en skärmbild kopiera sig själv så fort den är klar.
- Spara till en .txt- eller .md-fil med Ctrl+S.
- Sök i texten med Ctrl+F, med antalet ord och tecken bredvid.
- Uppläsning med vilken röst som helst som är installerad på datorn.
- ”Ta bort radbrytningar” fogar ihop hårda radbrytningar till stycken igen och ”Ta bort blanksteg”
  tar bort varje mellanslag — precis vad kinesisk, japansk och koreansk text behöver efter
  avläsning.
- Knappar för de länkar, e-postadresser och telefonnummer som hittas i texten.
- QR-koder och streckkoder ur samma bild.
- En sökbar historik som överlever en omstart: skärmbilden från förra veckan är fortfarande ett klick
  bort. Den sparar bara text, och du kan tömma den eller stänga av den.

**Mer än en sida i taget**
- Öppna en PDF och läs den en sida i taget, med bläddring i fönstret.
- Eller läs hela dokumentet på en gång och spara resultatet som en fil eller en fil per sida.
- Släpp en hög med bilder på fönstret och läs av dem alla i en enda omgång.

**Du bestämmer hur den läser**
- Välj avläsningsspråk bland de språkpaket som är installerade i Windows, eller låt Glyfo välja.
- En inställning rättar det klassiska OCR-felet där v1.6.5 läses som vl.6.5: ett l eller I blir en
  1:a bara där det står en avgränsare och en siffra intill, så html5 och IPv6 lämnas i fred.
- Vrid en bild, eller räta upp ett foto taget på snedden, innan den läses av.
- Anpassa till fönstret eller visa i verklig storlek; läs av hela bilden eller bara markeringen.

**Den är inte i vägen**
- Stänger du fönstret ligger Glyfo kvar i meddelandefältet och kortkommandot fortsätter fungera.
  ”Avsluta” därifrån stänger den på riktigt. Beteendet är i sin tur en inställning.
- Glyfo kan starta med Windows och gå rakt ned i meddelandefältet utan att öppna något fönster, så
  kortkommandot fungerar från det att du loggar in. Avstängt som standard; du slår på det själv.
- När fönstret är dolt visar en avisering första raden av det som just lästes av.
- Fönstret kommer tillbaka i den storlek och på den plats du lämnade det, ljust eller mörkt som du
  vill ha det.

**Språk**
Gränssnittet finns på 33 språk och följer språkinställningen i Windows. Avläsningen använder de
OCR-språkpaket som finns på datorn — fler lägger du till under Inställningar › Tid och språk › Språk
och region.

**Integritet**
Glyfo upprättar inga nätverksanslutningar. Bilder, avläst text och översättningar lämnar aldrig din
dator. Inget konto, ingen telemetri, inga annonser.

### Produktfunktioner

- Fånga vilken del av skärmen som helst med ett kortkommando du själv väljer och läs av den direkt, utan att lämna appen du läste i
- Bygger på den OCR som finns i Windows, och på Copilot+-datorer även på igenkänningsmodellen på enheten
- Öppna en bild eller en PDF, klistra in från Urklipp, dra och släpp eller ta emot via delningsfönstret i Windows
- Läser en hel PDF eller en hög med bilder i en enda omgång och sparar resultatet som en fil eller en fil per sida
- Håller en sökbar historik som överlever en omstart, och sparar vilket resultat som helst till en .txt- eller .md-fil
- Läser QR-koder och streckkoder ur samma bild
- Översätter på Copilot+-datorer, på enheten, utan nätverksåtkomst
- Läser upp resultatet med vilken röst som helst på datorn, och tar bort radbrytningar eller de blanksteg som CJK-text behöver
- Ligger kvar i meddelandefältet, så kortkommandot fungerar även när fönstret är stängt; gränssnitt på 33 språk, ljust eller mörkt
- Ansluter aldrig till internet: det du läser av lämnar inte din dator

### Bildtexter till skärmbilder

1. `01-text-from-a-page.png` — Den avlästa texten står bredvid bilden i den ordning den lästes: klar att kopiera, lyssna på eller foga ihop till stycken igen.
2. `02-any-language.png` — Avläsningsspråket väljer du bland språkpaketen som är installerade i Windows — eller så avgör Glyfo det själv.
3. `03-qr-and-barcodes.png` — QR-koder och streckkoder läses ur samma bild; någon separat skanningsapp behövs inte.
4. `04-history.png` — De senaste resultaten ligger kvar i historiken, så en skärmbild från några minuter sedan är fortfarande ett klick bort.
5. `05-settings.png` — Gränssnittsspråk, start med Windows, kvar i meddelandefältet, och rättelsen som hindrar v1.6.5 från att bli vl.6.5.
6. `06-pdf-pages.png` — En pdf öppnas sida för sida: den du har framme läses av direkt, och pilarna tar dig genom resten av dokumentet.
7. `07-batch.png` — Ett helt dokument eller en hög med bilder går igenom på en gång, sparat som en fil eller som en fil per sida.

### Söktermer

`text från bild`, `skärmbild till text`, `PDF till text`, `textigenkänning`, `läsa QR-kod`, `streckkodsläsare`, `OCR`

---

## Dansk — Danish

### Kort beskrivelse

Glyfo henter teksten ud af alt, hvad du kan se: et skærmbillede, et foto af en side, et scannet
dokument, et dias, et videobillede, en PDF. Tryk Alt+Z og træk en ramme om en del af skærmen, eller
åbn en fil, indsæt fra udklipsholderen, send et billede fra en anden app. Den genkendte tekst står
ved siden af billedet, klar til at kopiere, gemme, søge i eller lytte til. Den klarer også en hel PDF
eller en stak billeder på én gang og beholder alt, den har læst, i en søgbar historik. Den læser
QR-koder og stregkoder i det samme billede, og på en Copilot+-pc oversætter den resultatet. Det hele
sker på din egen pc — Glyfo opretter ingen internetforbindelser. Gratis, uden reklamer og uden køb.

### Beskrivelse

Glyfo laver billeder med tekst om til tekst, du kan bruge.

Genkendelsen kører på den OCR, der allerede findes i Windows: ingen konto, ingen upload, ingen
ventetid på en server. På en Copilot+-pc bruger Glyfo desuden tekstgenkendelsesmodellen på enheden
til de svære billeder og kan oversætte resultatet — også det uden netværk.

**Fem måder at få et billede ind på**
- Alt+Z trækker en ramme om en hvilken som helst del af skærmen. Ctrl+Shift+R tager hele skærmen.
  Begge genveje må du selv ændre.
- Ctrl+V indsætter et billede fra udklipsholderen, tekst også.
- Ctrl+O åbner et billede eller en PDF; at trække den ind i vinduet virker lige så godt.
- Højreklik et billede i Stifinder og åbn det med Glyfo, eller send det via delingspanelet i Windows
  (Billeder, Klippeværktøj, browseren).
- Slå overvågning af udklipsholderen til, så bliver hvert udklip, du tager med Win+Skift+S, læst af
  sig selv, med teksten liggende klar i udklipsholderen. Slået fra, indtil du beder om det.

**Hvad du får igen**
- Den genkendte tekst ved siden af billedet, i den rækkefølge den lå i.
- Kopiér med ét klik, eller lad et udklip kopiere sig selv, så snart det er færdigt.
- Gem i en .txt- eller .md-fil med Ctrl+S.
- Søg i teksten med Ctrl+F, med antal ord og tegn ved siden af.
- Oplæsning med enhver stemme, der er installeret på pc'en.
- ”Fjern linjeskift” samler hårde linjeskift til afsnit igen, og ”Fjern mellemrum” fjerner hvert
  eneste mellemrum — netop det, kinesisk, japansk og koreansk tekst har brug for efter genkendelse.
- Knapper til de links, mailadresser og telefonnumre, der findes i teksten.
- QR-koder og stregkoder fra det samme billede.
- En søgbar historik, der overlever en genstart: udklippet fra sidste uge er stadig ét klik væk. Den
  gemmer kun tekst, og du kan tømme den eller slå den fra.

**Mere end én side ad gangen**
- Åbn en PDF og læs den side for side, mens du bladrer i vinduet.
- Eller læs hele dokumentet på én gang og gem resultatet som én fil eller én fil pr. side.
- Slip en stak billeder på vinduet og genkend dem alle i én omgang.

**Du bestemmer, hvordan den læser**
- Vælg genkendelsessprog blandt de sprogpakker, der er installeret i Windows, eller lad Glyfo vælge.
- En indstilling retter den klassiske OCR-fejl, hvor v1.6.5 læses som vl.6.5: et l eller I bliver
  kun til et 1-tal, hvor der står et skilletegn og et ciffer ved siden af, så html5 og IPv6 får lov
  at være.
- Drej et billede, eller ret et skævt taget foto op, inden det læses.
- Tilpas til vinduet eller vis i faktisk størrelse; genkend hele billedet eller kun markeringen.

**Den er ikke i vejen**
- Lukker du vinduet, bliver Glyfo liggende i meddelelsesområdet, og genvejen virker stadig. ”Afslut”
  derfra lukker den for alvor. Den adfærd er selv en indstilling.
- Glyfo kan starte med Windows og gå direkte i meddelelsesområdet uden at åbne et vindue, så
  genvejen virker fra det øjeblik, du logger på. Slået fra som standard; du slår den til selv.
- Når vinduet er skjult, viser en meddelelse den første linje af det, der lige er genkendt.
- Vinduet kommer tilbage i den størrelse og på det sted, du forlod det, lyst eller mørkt som du vil.

**Sprog**
Brugerfladen findes på 33 sprog og følger sprogindstillingen i Windows. Genkendelsen bruger de
OCR-sprogpakker, der ligger på pc'en — flere tilføjer du under Indstillinger › Klokkeslæt og sprog ›
Sprog og område.

**Beskyttelse af personlige oplysninger**
Glyfo opretter ingen netværksforbindelser. Billeder, genkendt tekst og oversættelser forlader aldrig
din pc. Ingen konto, ingen telemetri, ingen reklamer.

### Produktfunktioner

- Tag et udklip af en hvilken som helst del af skærmen med en genvej, du selv vælger, og genkend det straks, uden at forlade den app, du læste i
- Bygger på den OCR, der er indbygget i Windows, og på Copilot+-pc'er også på genkendelsesmodellen på enheden
- Åbn et billede eller en PDF, indsæt fra udklipsholderen, træk og slip eller modtag via delingspanelet i Windows
- Læser en hel PDF eller en stak billeder i én omgang og gemmer resultatet som én fil eller én fil pr. side
- Fører en søgbar historik, der overlever en genstart, og gemmer ethvert resultat i en .txt- eller .md-fil
- Læser QR-koder og stregkoder fra det samme billede
- Oversætter på Copilot+-pc'er, på enheden, uden netværksadgang
- Læser resultatet op med enhver stemme på pc'en og fjerner linjeskift eller de mellemrum, CJK-tekst har brug for
- Bliver liggende i meddelelsesområdet, så genvejen virker, også når vinduet er lukket; brugerflade på 33 sprog, lys eller mørk
- Opretter aldrig forbindelse til internettet: det, du genkender, forlader ikke din pc

### Billedtekster til skærmbilleder

1. `01-text-from-a-page.png` — Den genkendte tekst står ved siden af billedet i den rækkefølge, den blev læst: klar til at kopiere, lytte til eller samle til afsnit igen.
2. `02-any-language.png` — Genkendelsessproget vælger du blandt de sprogpakker, der er installeret i Windows — eller Glyfo afgør det selv.
3. `03-qr-and-barcodes.png` — QR-koder og stregkoder læses fra det samme billede; en separat scannerapp er ikke nødvendig.
4. `04-history.png` — De seneste resultater bliver i historikken, så et udklip fra få minutter siden er stadig ét klik væk.
5. `05-settings.png` — Sprog i brugerfladen, start med Windows, bliv i meddelelsesområdet, og rettelsen der forhindrer v1.6.5 i at blive til vl.6.5.
6. `06-pdf-pages.png` — En pdf åbnes side for side: den, du har foran dig, læses med det samme, og pilene fører dig gennem resten af dokumentet.
7. `07-batch.png` — Et helt dokument eller en stak billeder kører igennem på én gang, gemt som én fil eller som én fil pr. side.

### Søgetermer

`tekst fra billede`, `skærmbillede til tekst`, `PDF til tekst`, `tekstgenkendelse`, `læs QR-kode`, `stregkodescanner`, `OCR`

---

## Norsk bokmål — Norwegian

### Kort beskrivelse

Glyfo henter teksten ut av alt du kan se: et skjermbilde, et foto av en side, et skannet dokument,
et lysbilde, en videorute, en PDF. Trykk Alt+Z og ramme inn en del av skjermen, eller åpne en fil,
lim inn fra utklippstavlen, send et bilde fra en annen app. Den gjenkjente teksten står ved siden av
bildet, klar til å kopieres, lagres, søkes i eller lyttes til. Den tar også en hel PDF eller en bunke
bilder på én gang, og beholder alt den har lest i en søkbar historikk. Den leser QR-koder og
strekkoder fra det samme bildet, og på en Copilot+-PC oversetter den resultatet. Alt skjer på din
egen PC — Glyfo oppretter ingen internettforbindelser. Gratis, uten annonser og uten kjøp.

### Beskrivelse

Glyfo gjør bilder med tekst om til tekst du kan bruke.

Gjenkjenningen bygger på OCR-en som allerede finnes i Windows: ingen konto, ingen opplasting, ingen
venting på en server. På en Copilot+-PC bruker Glyfo i tillegg tekstgjenkjenningsmodellen på enheten
til de vanskelige bildene og kan oversette resultatet — også det uten nettverk.

**Fem måter å få inn et bilde på**
- Alt+Z rammer inn hvilken som helst del av skjermen. Ctrl+Shift+R tar hele skjermen. Begge
  hurtigtastene kan du endre selv.
- Ctrl+V limer inn et bilde fra utklippstavlen, tekst også.
- Ctrl+O åpner et bilde eller en PDF; å dra den inn i vinduet fungerer like godt.
- Høyreklikk et bilde i Filutforsker og åpne det med Glyfo, eller send det via delingspanelet i
  Windows (Bilder, Utklippsverktøy, nettleseren).
- Slå på overvåking av utklippstavlen, så blir hvert utklipp du tar med Win+Skift+S lest på egen
  hånd, med teksten liggende klar på utklippstavlen. Av til du ber om det.

**Hva du får tilbake**
- Den gjenkjente teksten ved siden av bildet, i den rekkefølgen den lå i.
- Kopier med ett klikk, eller la et utklipp kopiere seg selv så snart det er ferdig.
- Lagre i en .txt- eller .md-fil med Ctrl+S.
- Søk i teksten med Ctrl+F, med antall ord og tegn ved siden av.
- Opplesing med hvilken som helst stemme som er installert på PC-en.
- ”Fjern linjeskift” setter harde linjeskift sammen til avsnitt igjen, og ”Fjern mellomrom” fjerner
  hvert mellomrom — akkurat det kinesisk, japansk og koreansk tekst trenger etter gjenkjenning.
- Knapper for lenkene, e-postadressene og telefonnumrene som finnes i teksten.
- QR-koder og strekkoder fra det samme bildet.
- En søkbar historikk som overlever en omstart: utklippet fra forrige uke er fortsatt ett klikk unna.
  Den lagrer bare tekst, og du kan tømme den eller slå den av.

**Mer enn én side om gangen**
- Åpne en PDF og les den side for side, mens du blar i vinduet.
- Eller les hele dokumentet på én gang og lagre resultatet som én fil eller én fil per side.
- Slipp en bunke bilder på vinduet og gjenkjenn dem alle i én omgang.

**Du bestemmer hvordan den leser**
- Velg gjenkjenningsspråk blant språkpakkene som er installert i Windows, eller la Glyfo velge.
- En innstilling retter den klassiske OCR-feilen der v1.6.5 leses som vl.6.5: en l eller I blir et
  1-tall bare der det står et skilletegn og et siffer ved siden av, så html5 og IPv6 blir stående.
- Roter et bilde, eller rett opp et skjevt tatt foto, før det leses.
- Tilpass til vinduet eller vis i faktisk størrelse; gjenkjenn hele bildet eller bare utvalget.

**Den er ikke i veien**
- Lukker du vinduet, blir Glyfo liggende i systemstatusfeltet, og hurtigtasten virker fortsatt.
  ”Avslutt” derfra lukker den for godt. Denne oppførselen er i seg selv en innstilling.
- Glyfo kan starte med Windows og gå rett til systemstatusfeltet uten å åpne et vindu, slik at
  hurtigtasten virker fra det øyeblikket du logger på. Av som standard; du slår den på selv.
- Når vinduet er skjult, viser et varsel den første linjen av det som nettopp ble gjenkjent.
- Vinduet kommer tilbake i den størrelsen og på det stedet du forlot det, lyst eller mørkt som du vil.

**Språk**
Grensesnittet finnes på 33 språk og følger språkinnstillingen i Windows. Gjenkjenningen bruker
OCR-språkpakkene som ligger på PC-en — flere legger du til under Innstillinger › Tid og språk ›
Språk og område.

**Personvern**
Glyfo oppretter ingen nettverksforbindelser. Bilder, gjenkjent tekst og oversettelser forlater aldri
PC-en din. Ingen konto, ingen telemetri, ingen annonser.

### Produktfunksjoner

- Ta et utklipp av hvilken som helst del av skjermen med en hurtigtast du velger selv, og gjenkjenn det med én gang, uten å forlate appen du leste i
- Bygger på OCR-en som er innebygd i Windows, og på Copilot+-PC-er også på gjenkjenningsmodellen på enheten
- Åpne et bilde eller en PDF, lim inn fra utklippstavlen, dra og slipp eller motta via delingspanelet i Windows
- Leser en hel PDF eller en bunke bilder i én omgang og lagrer resultatet som én fil eller én fil per side
- Fører en søkbar historikk som overlever en omstart, og lagrer ethvert resultat i en .txt- eller .md-fil
- Leser QR-koder og strekkoder fra det samme bildet
- Oversetter på Copilot+-PC-er, på enheten, uten nettverkstilgang
- Leser opp resultatet med hvilken som helst stemme på PC-en, og fjerner linjeskift eller mellomrommene CJK-tekst trenger
- Blir liggende i systemstatusfeltet, så hurtigtasten virker også når vinduet er lukket; grensesnitt på 33 språk, lyst eller mørkt
- Kobler seg aldri til internett: det du gjenkjenner, forlater ikke PC-en din

### Bildetekster til skjermbilder

1. `01-text-from-a-page.png` — Den gjenkjente teksten står ved siden av bildet i den rekkefølgen den ble lest: klar til å kopieres, lyttes til eller settes sammen til avsnitt igjen.
2. `02-any-language.png` — Gjenkjenningsspråket velger du blant språkpakkene som er installert i Windows — eller Glyfo bestemmer det selv.
3. `03-qr-and-barcodes.png` — QR-koder og strekkoder leses fra det samme bildet; en egen skanne-app er ikke nødvendig.
4. `04-history.png` — De siste resultatene blir liggende i historikken, så et utklipp fra noen minutter siden er fortsatt ett klikk unna.
5. `05-settings.png` — Språk i grensesnittet, start med Windows, bli i systemstatusfeltet, og rettelsen som hindrer v1.6.5 i å bli vl.6.5.
6. `06-pdf-pages.png` — En pdf åpnes side for side: den du har foran deg leses med én gang, og pilene tar deg gjennom resten av dokumentet.
7. `07-batch.png` — Et helt dokument eller en bunke bilder går gjennom på én gang, lagret som én fil eller som én fil per side.

### Søkeord

`tekst fra bilde`, `skjermbilde til tekst`, `PDF til tekst`, `tekstgjenkjenning`, `lese QR-kode`, `strekkodeleser`, `OCR`

---

## Suomi — Finnish

### Lyhyt kuvaus

Glyfo poimii tekstin kaikesta, mitä näet: kuvakaappauksesta, sivun valokuvasta, skannatusta
asiakirjasta, PDF:stä, diasta, videoruudusta. Paina Alt+Z ja rajaa osa näytöstä, tai avaa tiedosto,
liitä leikepöydältä, lähetä kuva toisesta sovelluksesta. Tunnistettu teksti ilmestyy kuvan viereen
valmiina kopioitavaksi, tallennettavaksi, haettavaksi tai ääneen luettavaksi. Glyfo lukee kokonaisen
PDF:n tai kuvanipun yhdellä kertaa ja pitää tuloksista haettavaa historiaa, joka säilyy uudelleen
käynnistyksen yli. Se lukee samasta kuvasta myös QR-koodit ja viivakoodit, ja Copilot+-tietokoneessa
se kääntää tuloksen. Kaikki tapahtuu omalla koneellasi — Glyfo ei muodosta lainkaan
internet-yhteyksiä. Ilmainen, ei mainoksia eikä ostoksia.

### Kuvaus

Glyfo muuttaa tekstiä sisältävät kuvat tekstiksi, jota voi käyttää.

Tunnistus toimii Windowsin omalla OCR:llä: ei tiliä, ei latausta palvelimelle, ei odottelua.
Copilot+-tietokoneessa Glyfo käyttää vaikeisiin kuviin lisäksi laitteessa toimivaa
tekstintunnistusmallia ja osaa kääntää tuloksen — myös se tapahtuu ilman verkkoa.

**Viisi tapaa tuoda kuva sisään**
- Alt+Z rajaa minkä tahansa osan näytöstä. Ctrl+Shift+R ottaa koko näytön. Molemmat pikanäppäimet
  saa vaihtaa omiin.
- Ctrl+V liittää kuvan leikepöydältä, tekstin myös.
- Ctrl+O avaa kuvan tai PDF:n; ikkunaan raahaaminen toimii yhtä hyvin.
- Napsauta kuvaa Resurssienhallinnassa hiiren kakkospainikkeella ja avaa se Glyfossa, tai lähetä se
  Windowsin jakopaneelista (Kuvat, Kuvakaappaustyökalu, selain).
- Ota leikepöydän seuranta käyttöön, niin jokainen Win+Shift+S:llä otettu kaappaus tunnistetaan
  itsestään ja teksti jää leikepöydälle liitettäväksi. Pois päältä, kunnes sen itse pyydät.

**Mitä saat takaisin**
- Tunnistetun tekstin kuvan vieressä, alkuperäisen asettelun järjestyksessä.
- Kopiointi yhdellä napsautuksella, tai anna kaappauksen kopioida itsensä heti kun se on valmis.
- Tallennus .txt- tai .md-tiedostoon Ctrl+S:llä.
- Haku tekstistä Ctrl+F:llä, vieressä sana- ja merkkimäärä.
- Ääneen lukeminen millä tahansa koneelle asennetulla äänellä.
- ”Poista rivinvaihdot” liittää kovat rivinvaihdot takaisin kappaleiksi ja ”Poista välilyönnit”
  poistaa jokaisen välilyönnin — juuri sitä kiinan-, japanin- ja koreankielinen teksti tarvitsee
  tunnistuksen jälkeen.
- Painikkeet tekstistä löytyville linkeille, sähköpostiosoitteille ja puhelinnumeroille.
- QR-koodit ja viivakoodit samasta kuvasta.
- Haettava historia, joka säilyy uudelleenkäynnistyksen yli: viime viikolla otettu kaappaus on yhä
  yhden napsautuksen päässä. Siihen tallentuu vain teksti, ja sen voi tyhjentää tai kytkeä pois.

**Useampi sivu kerralla**
- Avaa PDF ja lue sitä sivu kerrallaan, liikkuen siinä suoraan ikkunassa.
- Tai lue koko asiakirja yhdellä kertaa ja tallenna tulos yhtenä tiedostona tai tiedostona per sivu.
- Pudota nippu kuvia ikkunaan ja tunnista ne kaikki yhdellä ajolla.

**Sinä päätät, miten se lukee**
- Valitse tunnistuskieli Windowsiin asennetuista kielipaketeista tai anna Glyfon valita.
- Yksi asetus korjaa klassisen OCR-virheen, jossa v1.6.5 luetaan muodossa vl.6.5: l tai I muuttuu
  ykköseksi vain silloin, kun vieressä on erotin ja numero, joten html5 ja IPv6 jäävät ennalleen.
- Sovita ikkunaan tai näytä todellisessa koossa; tunnista koko kuva tai vain valinta.
- Käännä kuvaa tai suorista vinossa otettu valokuva ennen lukemista.

**Se ei ole tiellä**
- Kun suljet ikkunan, Glyfo jää ilmoitusalueelle ja kaappauspikanäppäin toimii edelleen. Sieltä
  valittu ”Lopeta” sulkee sen kokonaan. Tämäkin toiminta on oma asetuksensa.
- Glyfo voi käynnistyä Windowsin mukana ja mennä suoraan ilmoitusalueelle avaamatta ikkunaa, jolloin
  pikanäppäin toimii heti kirjautumisesta alkaen. Oletuksena pois päältä; sinä otat sen käyttöön.
- Kun ikkuna on piilossa, ilmoitus näyttää juuri tunnistetun tekstin ensimmäisen rivin.
- Ikkuna palaa siihen kokoon ja paikkaan, johon sen jätit, vaaleana tai tummana oman valintasi
  mukaan.

**Kielet**
Käyttöliittymä on saatavilla 33 kielellä ja noudattaa Windowsin kieliasetusta. Tunnistus käyttää
koneelle asennettuja OCR-kielipaketteja — lisää niitä kohdasta Asetukset › Aika ja kieli › Kieli ja
alue.

**Tietosuoja**
Glyfo ei muodosta verkkoyhteyksiä. Kuvat, tunnistettu teksti ja käännökset eivät poistu koneeltasi
koskaan. Ei tiliä, ei telemetriaa, ei mainoksia.

### Tuotteen ominaisuudet

- Kaappaa mikä tahansa osa näytöstä itse valitsemallasi pikanäppäimellä ja tunnista se heti poistumatta sovelluksesta, jota olit lukemassa
- Perustuu Windowsin sisäänrakennettuun OCR:ään, ja Copilot+-koneissa myös laitteessa toimivaan tunnistusmalliin
- Avaa kuva tai PDF, liitä leikepöydältä, raahaa ja pudota tai vastaanota Windowsin jakopaneelista
- Lukee kokonaisen PDF:n tai kuvanipun yhtenä eränä ja tallentaa tuloksen yhtenä tiedostona tai tiedostona per sivu
- Pitää haettavaa historiaa, joka säilyy uudelleenkäynnistyksen yli, ja tallentaa minkä tahansa tuloksen .txt- tai .md-tiedostoon
- Lukee samasta kuvasta QR-koodit ja viivakoodit
- Kääntää Copilot+-koneissa laitteessa, ilman verkkoyhteyttä
- Lukee tuloksen ääneen millä tahansa koneen äänellä ja poistaa rivinvaihdot tai välilyönnit, kuten CJK-teksti tarvitsee
- Jää ilmoitusalueelle, joten pikanäppäin toimii myös ikkunan sulkemisen jälkeen; käyttöliittymä 33 kielellä, vaalea tai tumma
- Ei yhdistä internetiin: se, minkä tunnistat, ei poistu koneeltasi

### Kuvakaappausten tekstit

1. `01-text-from-a-page.png` — Tunnistettu teksti on kuvan vieressä siinä järjestyksessä, jossa se luettiin: valmiina kopioitavaksi, kuunneltavaksi tai koottavaksi takaisin kappaleiksi.
2. `02-any-language.png` — Tunnistuskielen valitset Windowsiin asennetuista kielipaketeista — tai Glyfo päättää sen itse.
3. `03-qr-and-barcodes.png` — QR-koodit ja viivakoodit luetaan samasta kuvasta; erillistä skannaussovellusta ei tarvita.
4. `04-history.png` — Viimeisimmät tulokset jäävät historiaan, joten muutaman minuutin takainen kaappaus on yhä yhden napsautuksen päässä.
5. `05-settings.png` — Käyttöliittymän kieli, käynnistys Windowsin mukana, ilmoitusalueelle jääminen ja korjaus, joka estää v1.6.5:tä muuttumasta muotoon vl.6.5.
6. `06-pdf-pages.png` — PDF avautuu sivu kerrallaan: edessä oleva sivu luetaan heti, ja nuolilla kuljet läpi loput asiakirjasta.
7. `07-batch.png` — Kokonainen asiakirja tai kasa kuvia menee läpi yhdellä kertaa, tallennettuna yhteen tiedostoon tai sivu kerrallaan omiinsa.

### Hakutermit

`teksti kuvasta`, `kuvakaappaus tekstiksi`, `PDF tekstiksi`, `tekstintunnistus`, `QR-koodin lukija`, `viivakoodinlukija`, `OCR`

---

## Ελληνικά — Greek

### Σύντομη περιγραφή

Το Glyfo βγάζει το κείμενο από οτιδήποτε βλέπετε: ένα στιγμιότυπο οθόνης, τη φωτογραφία μιας
σελίδας, ένα σαρωμένο έγγραφο, ένα PDF, μια διαφάνεια, ένα καρέ βίντεο. Πατήστε Alt+Z και πλαισιώστε
ένα τμήμα της οθόνης, ή ανοίξτε ένα αρχείο, επικολλήστε από το πρόχειρο, στείλτε μια εικόνα από άλλη
εφαρμογή. Το κείμενο που αναγνωρίστηκε εμφανίζεται δίπλα στην εικόνα, έτοιμο για αντιγραφή,
αποθήκευση, αναζήτηση ή ανάγνωση φωναχτά. Το Glyfo διαβάζει ολόκληρο PDF ή μια στοίβα εικόνες με τη
μία και κρατά ένα ιστορικό με δυνατότητα αναζήτησης που επιβιώνει μιας επανεκκίνησης. Διαβάζει
επίσης κωδικούς QR και barcode από την ίδια εικόνα, και σε έναν υπολογιστή Copilot+ μεταφράζει το
αποτέλεσμα. Όλα γίνονται στον υπολογιστή σας — το Glyfo δεν κάνει καμία σύνδεση στο διαδίκτυο.
Δωρεάν, χωρίς διαφημίσεις και χωρίς αγορές.

### Περιγραφή

Το Glyfo μετατρέπει τις εικόνες με κείμενο σε κείμενο που μπορείτε να χρησιμοποιήσετε.

Η αναγνώριση στηρίζεται στο OCR που είναι ήδη μέσα στα Windows: χωρίς λογαριασμό, χωρίς αποστολή
αρχείων, χωρίς αναμονή για κάποιον διακομιστή. Σε υπολογιστή Copilot+, το Glyfo αξιοποιεί επιπλέον
το μοντέλο αναγνώρισης κειμένου που τρέχει στη συσκευή για τις δύσκολες εικόνες και μπορεί να
μεταφράσει το αποτέλεσμα — και αυτό χωρίς δίκτυο.

**Πέντε τρόποι να μπει μια εικόνα**
- Alt+Z για να πλαισιώσετε οποιοδήποτε τμήμα της οθόνης. Ctrl+Shift+R παίρνει ολόκληρη την οθόνη.
  Και τις δύο συντομεύσεις μπορείτε να τις αλλάξετε.
- Ctrl+V επικολλά εικόνα από το πρόχειρο, και κείμενο επίσης.
- Ctrl+O ανοίγει μια εικόνα ή ένα PDF· η μεταφορά του μέσα στο παράθυρο δουλεύει το ίδιο καλά.
- Δεξί κλικ σε μια εικόνα στην Εξερεύνηση αρχείων και άνοιγμα με το Glyfo, ή αποστολή από το
  παράθυρο κοινής χρήσης των Windows (Φωτογραφίες, Εργαλείο αποκομμάτων, πρόγραμμα περιήγησης).
- Ανοίξτε την παρακολούθηση του πρόχειρου και κάθε απόκομμα που παίρνετε με Win+Shift+S
  αναγνωρίζεται μόνο του, με το κείμενο να μένει στο πρόχειρο για επικόλληση. Κλειστό μέχρι να το
  ζητήσετε.

**Τι παίρνετε πίσω**
- Το αναγνωρισμένο κείμενο δίπλα στην εικόνα, με τη σειρά που είχε στη διάταξη.
- Αντιγραφή με ένα κλικ, ή αφήστε το απόκομμα να αντιγραφεί μόνο του μόλις ολοκληρωθεί.
- Αποθήκευση σε αρχείο .txt ή .md με Ctrl+S.
- Αναζήτηση μέσα στο κείμενο με Ctrl+F, με τον αριθμό λέξεων και χαρακτήρων δίπλα.
- Ανάγνωση φωναχτά με όποια φωνή είναι εγκατεστημένη στον υπολογιστή.
- Η «Αφαίρεση αλλαγών γραμμής» ενώνει ξανά τις σκληρές αλλαγές γραμμής σε παραγράφους και η
  «Αφαίρεση κενών» σβήνει κάθε κενό — ακριβώς αυτό που χρειάζεται το κινεζικό, ιαπωνικό και κορεατικό
  κείμενο μετά την αναγνώριση.
- Κουμπιά για τους συνδέσμους, τις διευθύνσεις ηλεκτρονικού ταχυδρομείου και τους αριθμούς τηλεφώνου
  που βρέθηκαν στο κείμενο.
- Κωδικοί QR και barcode από την ίδια εικόνα.
- Ένα ιστορικό με δυνατότητα αναζήτησης που επιβιώνει μιας επανεκκίνησης, ώστε ένα απόκομμα της
  περασμένης εβδομάδας να είναι ακόμη ένα κλικ μακριά. Κρατά μόνο κείμενο, και μπορείτε να το
  αδειάσετε ή να το κλείσετε.

**Πάνω από μία σελίδα τη φορά**
- Ανοίξτε ένα PDF και διαβάστε το σελίδα σελίδα, κινούμενοι μέσα του στο ίδιο το παράθυρο.
- Ή διαβάστε ολόκληρο το έγγραφο με τη μία και αποθηκεύστε το αποτέλεσμα ως ένα αρχείο ή ένα αρχείο
  ανά σελίδα.
- Ρίξτε μια στοίβα εικόνες στο παράθυρο και αναγνωρίστε τις όλες σε ένα πέρασμα.

**Εσείς αποφασίζετε πώς διαβάζει**
- Διαλέξτε γλώσσα αναγνώρισης ανάμεσα στα πακέτα που είναι εγκατεστημένα στα Windows, ή αφήστε το
  Glyfo να αποφασίσει.
- Μια επιλογή διορθώνει το κλασικό λάθος του OCR που διαβάζει το v1.6.5 ως vl.6.5: το l ή το I
  γίνεται 1 μόνο εκεί που δίπλα του υπάρχει διαχωριστικό και ψηφίο, οπότε το html5 και το IPv6 μένουν
  ανέπαφα.
- Προσαρμογή στο παράθυρο ή προβολή σε πραγματικό μέγεθος· αναγνώριση ολόκληρης της εικόνας ή μόνο
  της επιλογής.
- Περιστρέψτε μια εικόνα, ή ισιώστε μια φωτογραφία που τραβήχτηκε στραβά, πριν τη διαβάσετε.

**Δεν σας μπαίνει εμπόδιο**
- Κλείνοντας το παράθυρο, το Glyfo παραμένει στην περιοχή ειδοποιήσεων και η συντόμευση συνεχίζει να
  δουλεύει. Η «Έξοδος» από εκεί το κλείνει στ' αλήθεια. Η ίδια αυτή συμπεριφορά είναι κι αυτή
  ρύθμιση.
- Το Glyfo μπορεί να ξεκινά μαζί με τα Windows και να πηγαίνει κατευθείαν στην περιοχή ειδοποιήσεων
  χωρίς να ανοίγει παράθυρο, ώστε η συντόμευση να δουλεύει από τη στιγμή που συνδέεστε.
  Απενεργοποιημένο εξ ορισμού· εσείς το ανοίγετε.
- Με το παράθυρο κρυμμένο, μια ειδοποίηση δείχνει την πρώτη γραμμή αυτού που μόλις αναγνωρίστηκε.
- Το παράθυρο επιστρέφει στο μέγεθος και στη θέση που το αφήσατε, φωτεινό ή σκοτεινό όπως
  προτιμάτε.

**Γλώσσες**
Το περιβάλλον είναι διαθέσιμο σε 33 γλώσσες και ακολουθεί τη ρύθμιση γλώσσας των Windows. Η
αναγνώριση χρησιμοποιεί τα πακέτα γλώσσας OCR που υπάρχουν στον υπολογιστή — προσθέτετε κι άλλα από
τις Ρυθμίσεις › Ώρα και γλώσσα › Γλώσσα και περιοχή.

**Απόρρητο**
Το Glyfo δεν κάνει συνδέσεις δικτύου. Οι εικόνες, το αναγνωρισμένο κείμενο και οι μεταφράσεις δεν
φεύγουν ποτέ από τον υπολογιστή σας. Χωρίς λογαριασμό, χωρίς τηλεμετρία, χωρίς διαφημίσεις.

### Δυνατότητες προϊόντος

- Αποτυπώστε οποιοδήποτε τμήμα της οθόνης με μια συντόμευση της επιλογής σας και αναγνωρίστε το αμέσως, χωρίς να φύγετε από την εφαρμογή που διαβάζατε
- Στηρίζεται στο ενσωματωμένο OCR των Windows και, σε υπολογιστές Copilot+, στο μοντέλο αναγνώρισης της συσκευής
- Άνοιγμα εικόνας ή PDF, επικόλληση από το πρόχειρο, μεταφορά και απόθεση ή λήψη από το παράθυρο κοινής χρήσης
- Διαβάζει ολόκληρο PDF ή μια στοίβα εικόνες σε μία παρτίδα και αποθηκεύει το αποτέλεσμα ως ένα αρχείο ή ένα αρχείο ανά σελίδα
- Κρατά ιστορικό με δυνατότητα αναζήτησης που επιβιώνει μιας επανεκκίνησης, και αποθηκεύει κάθε αποτέλεσμα σε αρχείο .txt ή .md
- Διαβάζει κωδικούς QR και barcode από την ίδια εικόνα
- Μεταφράζει σε υπολογιστές Copilot+, στη συσκευή, χωρίς πρόσβαση στο δίκτυο
- Διαβάζει το αποτέλεσμα φωναχτά με όποια φωνή του υπολογιστή και αφαιρεί τις αλλαγές γραμμής ή τα κενά που χρειάζεται το κείμενο CJK
- Παραμένει στην περιοχή ειδοποιήσεων, ώστε η συντόμευση να δουλεύει και με κλειστό παράθυρο· περιβάλλον σε 33 γλώσσες, φωτεινό ή σκοτεινό
- Δεν συνδέεται στο διαδίκτυο: ό,τι αναγνωρίζετε δεν φεύγει από τον υπολογιστή σας

### Λεζάντες στιγμιότυπων

1. `01-text-from-a-page.png` — Το αναγνωρισμένο κείμενο στέκεται δίπλα στην εικόνα με τη σειρά που διαβάστηκε: έτοιμο για αντιγραφή, ακρόαση ή ένωση ξανά σε παραγράφους.
2. `02-any-language.png` — Τη γλώσσα αναγνώρισης τη διαλέγετε ανάμεσα στα πακέτα που είναι εγκατεστημένα στα Windows — ή την ορίζει μόνο του το Glyfo.
3. `03-qr-and-barcodes.png` — Κωδικοί QR και barcode διαβάζονται από την ίδια εικόνα· δεν χρειάζεται ξεχωριστή εφαρμογή σάρωσης.
4. `04-history.png` — Τα πρόσφατα αποτελέσματα μένουν στο ιστορικό, οπότε ένα απόκομμα λίγων λεπτών πριν είναι ακόμη ένα κλικ μακριά.
5. `05-settings.png` — Γλώσσα περιβάλλοντος, εκκίνηση με τα Windows, παραμονή στην περιοχή ειδοποιήσεων και η διόρθωση που δεν αφήνει το v1.6.5 να γίνει vl.6.5.
6. `06-pdf-pages.png` — Το PDF ανοίγει σελίδα-σελίδα: αυτή που βλέπετε αναγνωρίζεται αμέσως και τα βελάκια σάς πάνε στο υπόλοιπο έγγραφο.
7. `07-batch.png` — Ένα ολόκληρο έγγραφο ή μια στοίβα εικόνες περνά με τη μία, αποθηκευμένο σε ένα αρχείο ή σε ένα αρχείο ανά σελίδα.

### Όροι αναζήτησης

`κείμενο από εικόνα`, `στιγμιότυπο σε κείμενο`, `PDF σε κείμενο`, `αναγνώριση κειμένου`, `ανάγνωση QR κωδικού`, `σαρωτής barcode`, `OCR`

---

## Magyar — Hungarian

### Rövid leírás

A Glyfo mindenből kiszedi a szöveget, amit látsz: képernyőképből, egy oldalról készült fotóból,
beszkennelt iratból, PDF-ből, diából, videokockából. Nyomd meg az Alt+Z billentyűt, és keretezz be egy
részt a képernyőből, vagy nyiss meg egy fájlt, illessz be a vágólapról, küldj át egy képet egy másik
alkalmazásból. A felismert szöveg a kép mellett jelenik meg, készen a másolásra, mentésre, keresésre
vagy felolvasásra. A Glyfo egy egész PDF-et vagy egy köteg képet egyszerre olvas be, és kereshető
előzményt vezet, amely túléli az újraindítást. Ugyanabból a képből a QR-kódokat és vonalkódokat is
kiolvassa, Copilot+ gépen pedig le is fordítja az eredményt. Minden a saját gépeden történik — a
Glyfo egyáltalán nem kapcsolódik az internethez. Ingyenes, hirdetések és vásárlások nélkül.

### Leírás

A Glyfo a szöveget tartalmazó képekből használható szöveget csinál.

A felismerés a Windowsba épített OCR-re támaszkodik: nincs fiók, nincs feltöltés, nincs várakozás egy
kiszolgálóra. Copilot+ gépen a Glyfo a nehezebb képekhez az eszközön futó szövegfelismerő modellt is
igénybe veszi, és le tudja fordítani az eredményt — ez is hálózat nélkül.

**Ötféleképpen kerülhet be egy kép**
- Az Alt+Z bekeretezi a képernyő tetszőleges részét. A Ctrl+Shift+R az egész képernyőt viszi.
  Mindkét gyorsbillentyűt átírhatod a sajátodra.
- A Ctrl+V képet illeszt be a vágólapról, szöveget is.
- A Ctrl+O képet vagy PDF-et nyit meg; az ablakba húzás ugyanúgy működik.
- Kattints jobb gombbal egy képre a Fájlkezelőben, és nyisd meg a Glyfóval, vagy küldd át a Windows
  megosztási paneljéről (Fényképek, Képmetsző, böngésző).
- Kapcsold be a vágólapfigyelést, és minden Win+Shift+S billentyűvel készített felvételt magától
  felismer, a szöveget pedig a vágólapon hagyja, hogy beilleszthesd. Amíg nem kéred, ki van kapcsolva.

**Mit kapsz vissza**
- A felismert szöveget a kép mellett, az eredeti elrendezés sorrendjében.
- Másolás egy kattintással, vagy hagyd, hogy a felvétel magától a vágólapra kerüljön, amint elkészül.
- Mentés .txt vagy .md fájlba a Ctrl+S billentyűvel.
- Keresés a szövegben a Ctrl+F billentyűvel, mellette a szavak és karakterek száma.
- Felolvasás bármelyik, a gépre telepített hanggal.
- A „Sortörések eltávolítása” a kemény sortöréseket visszafűzi bekezdésekké, a „Szóközök
  eltávolítása” pedig minden szóközt kitöröl — pontosan erre van szüksége a kínai, japán és koreai
  szövegnek a felismerés után.
- Gombok a szövegben talált hivatkozásokhoz, e-mail-címekhez és telefonszámokhoz.
- QR-kódok és vonalkódok ugyanabból a képből.
- Kereshető előzmény, amely túléli az újraindítást, így a múlt heti felvétel is egy kattintásnyira
  van. Csak szöveget tárol, és bármikor kiüríthető vagy kikapcsolható.

**Egyszerre több oldal**
- Nyiss meg egy PDF-et, és olvasd oldalanként, az ablakban lapozva benne.
- Vagy olvasd be az egész dokumentumot egyszerre, és mentsd az eredményt egyetlen fájlba vagy
  oldalanként egy-egy fájlba.
- Ejts egy köteg képet az ablakra, és ismerd fel mindet egyetlen menetben.

**Te döntöd el, hogyan olvas**
- A felismerés nyelvét a Windowsban telepített nyelvi csomagok közül választhatod ki, vagy a Glyfóra
  bízhatod.
- Egy beállítás javítja azt a klasszikus OCR-hibát, amelytől a v1.6.5 vl.6.5 lesz: az l vagy az I
  csak ott válik 1-essé, ahol elválasztó és számjegy áll mellette, így a html5 és az IPv6 érintetlen
  marad.
- Igazítás az ablakhoz vagy valódi méret; az egész kép vagy csak a kijelölés felismerése.
- Forgasd el a képet, vagy egyenesítsd ki a ferdén készült fotót, mielőtt beolvasnád.

**Nincs útban**
- Az ablak bezárásakor a Glyfo az értesítési területen marad, és a gyorsbillentyű továbbra is
  működik. Az ottani „Kilépés” zárja be igazán. Maga ez a viselkedés is beállítás.
- A Glyfo elindulhat a Windowsszal, és ablak nyitása nélkül egyenesen az értesítési területre mehet,
  így a gyorsbillentyű a bejelentkezés pillanatától működik. Alapértelmezés szerint kikapcsolva; te
  kapcsolod be.
- Rejtett ablaknál egy értesítés mutatja az imént felismert szöveg első sorát.
- Az ablak akkora méretben és ott jön vissza, ahogy elhagytad, világos vagy sötét megjelenéssel,
  ahogy szeretnéd.

**Nyelvek**
A felület 33 nyelven érhető el, és a Windows nyelvi beállítását követi. A felismerés a gépre
telepített OCR-nyelvi csomagokat használja — továbbiakat a Beállítások › Idő és nyelv › Nyelv és
régió alatt adhatsz hozzá.

**Adatvédelem**
A Glyfo nem létesít hálózati kapcsolatot. A képek, a felismert szöveg és a fordítások soha nem
hagyják el a gépedet. Nincs fiók, nincs telemetria, nincs hirdetés.

### Termékjellemzők

- Vágd ki a képernyő bármelyik részét egy általad választott gyorsbillentyűvel, és ismerd fel azonnal, anélkül hogy kilépnél abból az alkalmazásból, amelyet éppen olvastál
- A Windowsba épített OCR-re épül, Copilot+ gépeken pedig az eszközön futó felismerő modellre is
- Kép vagy PDF megnyitása, beillesztés a vágólapról, húzd és ejtsd, vagy fogadás a Windows megosztási paneljéről
- Egy egész PDF-et vagy egy köteg képet egyetlen menetben olvas be, az eredményt egyetlen fájlba vagy oldalanként egy-egy fájlba mentve
- Kereshető előzményt vezet, amely túléli az újraindítást, és bármelyik eredményt .txt vagy .md fájlba menti
- Kiolvassa ugyanabból a képből a QR-kódokat és a vonalkódokat
- Copilot+ gépeken az eszközön fordít, hálózati hozzáférés nélkül
- Felolvassa az eredményt a gép bármelyik hangjával, és eltávolítja a sortöréseket vagy a szóközöket, ahogy a CJK-szövegnek kell
- Az értesítési területen marad, így a gyorsbillentyű az ablak bezárása után is működik; 33 nyelvű felület, világos vagy sötét
- Nem kapcsolódik az internethez: amit felismersz, nem hagyja el a gépedet

### Képernyőképek feliratai

1. `01-text-from-a-page.png` — A felismert szöveg a kép mellett áll, abban a sorrendben, ahogy beolvasásra került: másolható, felolvastatható vagy visszafűzhető bekezdésekké.
2. `02-any-language.png` — A felismerés nyelvét a Windowsban telepített nyelvi csomagok közül választod ki — vagy a Glyfo dönti el magától.
3. `03-qr-and-barcodes.png` — A QR-kódok és a vonalkódok ugyanabból a képből olvashatók ki; külön szkennelő alkalmazásra nincs szükség.
4. `04-history.png` — A legutóbbi eredmények az előzményekben maradnak, így egy néhány perccel ezelőtti felvétel még mindig egy kattintásnyira van.
5. `05-settings.png` — A felület nyelve, indulás a Windowsszal, az értesítési területen maradás, és a javítás, amitől a v1.6.5 nem lesz vl.6.5.
6. `06-pdf-pages.png` — A PDF oldalanként nyílik meg: az éppen látható oldalt rögtön felismeri, a nyilakkal pedig végigmész a dokumentumon.
7. `07-batch.png` — Egy egész dokumentum vagy egy halom kép egy menetben fut le, egyetlen fájlba mentve vagy oldalanként külön fájlba.

### Keresési kifejezések

`szöveg képből`, `képernyőkép szöveggé`, `PDF szöveggé`, `szövegfelismerés`, `QR-kód olvasó`, `vonalkód olvasó`, `OCR`

---

## Română — Romanian

### Descriere scurtă

Glyfo extrage textul din tot ce vezi: o captură de ecran, fotografia unei pagini, un document
scanat, un PDF, un diapozitiv, un cadru dintr-un clip. Apasă Alt+Z și încadrează o parte a ecranului,
ori deschide un fișier, lipește din clipboard, trimite o imagine din altă aplicație. Textul
recunoscut apare lângă imagine, gata de copiat, de salvat, de căutat sau de ascultat. Glyfo citește
un PDF întreg sau un teanc de imagini dintr-o singură trecere și ține un istoric în care poți căuta,
care rezistă la o repornire. Citește din aceeași imagine și codurile QR și codurile de bare, iar pe
un PC Copilot+ traduce rezultatul. Totul se întâmplă pe calculatorul tău — Glyfo nu deschide nicio
conexiune la internet. Gratuit, fără reclame și fără achiziții.

### Descriere

Glyfo transformă imaginile cu text în text pe care îl poți folosi.

Recunoașterea se sprijină pe OCR-ul din Windows: fără cont, fără încărcare, fără așteptare după un
server. Pe un PC Copilot+, Glyfo folosește în plus modelul de recunoaștere a textului de pe
dispozitiv pentru imaginile dificile și poate traduce rezultatul — tot fără rețea.

**Cinci feluri de a aduce o imagine înăuntru**
- Alt+Z încadrează orice parte a ecranului. Ctrl+Shift+R ia tot ecranul. Ambele scurtături pot fi
  schimbate cum vrei.
- Ctrl+V lipește o imagine din clipboard, și text la fel.
- Ctrl+O deschide o imagine sau un PDF; tragerea lui în fereastră funcționează la fel de bine.
- Clic dreapta pe o imagine în Explorer și deschide-o cu Glyfo, sau trimite-o din panoul de
  partajare Windows (Fotografii, Instrument de decupare, browser).
- Pornește urmărirea clipboardului și fiecare decupaj făcut cu Win+Shift+S e citit de la sine, iar
  textul rămâne în clipboard, gata de lipit. Oprită până când o ceri.

**Ce primești înapoi**
- Textul recunoscut lângă imagine, în ordinea în care era așezat.
- Copiere cu un clic, sau lasă o captură să se copieze singură imediat ce e gata.
- Salvare într-un fișier .txt sau .md cu Ctrl+S.
- Căutare în text cu Ctrl+F, cu numărul de cuvinte și de caractere alături.
- Citire cu voce tare cu orice voce instalată pe calculator.
- „Elimină întreruperile de rând” lipește la loc rândurile rupte în paragrafe, iar „Elimină
  spațiile” șterge fiecare spațiu — exact ce îi trebuie textului chinezesc, japonez și coreean după
  recunoaștere.
- Butoane pentru linkurile, adresele de e-mail și numerele de telefon găsite în text.
- Coduri QR și coduri de bare din aceeași imagine.
- Un istoric în care poți căuta și care rezistă la o repornire, așa că o captură de săptămâna trecută
  e tot la un clic distanță. Păstrează doar text și îl poți goli sau opri.

**Mai mult de o pagină deodată**
- Deschide un PDF și citește-l pagină cu pagină, umblând prin el chiar în fereastră.
- Sau citește tot documentul dintr-o dată și salvează rezultatul într-un singur fișier ori câte un
  fișier pe pagină.
- Lasă un teanc de imagini pe fereastră și recunoaște-le pe toate într-o singură trecere.

**Tu hotărăști cum citește**
- Alege limba de recunoaștere dintre pachetele instalate în Windows, sau las-o pe Glyfo să aleagă.
- O opțiune repară eroarea clasică de OCR prin care v1.6.5 e citit vl.6.5: un l sau un I devine 1
  doar acolo unde alături stau un separator și o cifră, așa că html5 și IPv6 rămân neatinse.
- Potrivire la fereastră sau vizualizare la dimensiunea reală; recunoaște toată imaginea sau doar
  selecția.
- Rotește o imagine, sau îndreaptă o fotografie făcută strâmb, înainte de a o citi.

**Nu îți stă în cale**
- Dacă închizi fereastra, Glyfo rămâne în zona de notificare, iar scurtătura funcționează în
  continuare. „Ieșire” de acolo îl închide cu adevărat. Chiar și comportamentul acesta e o setare.
- Glyfo poate porni odată cu Windows și poate merge direct în zona de notificare fără să deschidă o
  fereastră, așa încât scurtătura funcționează din clipa în care te conectezi. Dezactivat implicit;
  tu îl activezi.
- Cu fereastra ascunsă, o notificare arată primul rând din ce tocmai a fost recunoscut.
- Fereastra revine la dimensiunea și în locul unde ai lăsat-o, deschisă la culoare sau întunecată,
  cum îți place.

**Limbi**
Interfața există în 33 de limbi și urmează setarea de limbă din Windows. Recunoașterea folosește
pachetele de limbă OCR instalate pe calculator — mai adaugi din Setări › Oră și limbă › Limbă și
regiune.

**Confidențialitate**
Glyfo nu deschide conexiuni de rețea. Imaginile, textul recunoscut și traducerile nu îți părăsesc
niciodată calculatorul. Fără cont, fără telemetrie, fără reclame.

### Caracteristicile produsului

- Capturează orice parte a ecranului cu o scurtătură aleasă de tine și recunoaște-o pe loc, fără să ieși din aplicația în care citeai
- Se bazează pe OCR-ul integrat în Windows, iar pe PC-urile Copilot+ și pe modelul de recunoaștere de pe dispozitiv
- Deschide o imagine sau un PDF, lipește din clipboard, trage și plasează sau primește din panoul de partajare Windows
- Citește un PDF întreg sau un teanc de imagini într-o singură trecere, salvând rezultatul într-un fișier ori câte un fișier pe pagină
- Ține un istoric în care poți căuta și care rezistă la o repornire, și salvează orice rezultat într-un fișier .txt sau .md
- Citește coduri QR și coduri de bare din aceeași imagine
- Traduce pe PC-urile Copilot+, pe dispozitiv, fără acces la rețea
- Citește rezultatul cu voce tare cu orice voce de pe calculator și elimină întreruperile de rând sau spațiile de care are nevoie textul CJK
- Rămâne în zona de notificare, așa că scurtătura merge și după închiderea ferestrei; interfață în 33 de limbi, deschisă la culoare sau întunecată
- Nu se conectează la internet: ce recunoști nu îți părăsește calculatorul

### Subtitrări pentru capturi de ecran

1. `01-text-from-a-page.png` — Textul recunoscut stă lângă imagine în ordinea în care a fost citit: gata de copiat, de ascultat sau de lipit la loc în paragrafe.
2. `02-any-language.png` — Limba de recunoaștere se alege dintre pachetele instalate în Windows — sau o stabilește Glyfo singur.
3. `03-qr-and-barcodes.png` — Codurile QR și codurile de bare se citesc din aceeași imagine; nu îți trebuie o aplicație separată de scanare.
4. `04-history.png` — Rezultatele recente rămân în istoric, așa că o captură de acum câteva minute e tot la un clic distanță.
5. `05-settings.png` — Limba interfeței, pornirea odată cu Windows, rămânerea în zona de notificare și corecția care nu lasă v1.6.5 să devină vl.6.5.
6. `06-pdf-pages.png` — Un PDF se deschide pagină cu pagină: cea din față e citită pe loc, iar săgețile te poartă prin restul documentului.
7. `07-batch.png` — Un document întreg sau un teanc de imagini trece dintr-o singură rulare, salvat într-un singur fișier sau câte unul pe pagină.

### Termeni de căutare

`text din imagine`, `captură de ecran`, `PDF în text`, `recunoaștere text`, `citire cod QR`, `coduri de bare`, `OCR`

---

## Українська — Ukrainian

### Короткий опис

Glyfo дістає текст з усього, що ви бачите: зі знімка екрана, з фотографії сторінки, зі сканованого
документа, з PDF, зі слайда, з кадру відео. Натисніть Alt+Z і обведіть частину екрана, або відкрийте
файл, вставте з буфера обміну, надішліть зображення з іншої програми. Розпізнаний текст з'являється
поруч із зображенням — готовий скопіювати, зберегти, знайти в ньому потрібне або прослухати. Glyfo
читає цілий PDF чи стос зображень за один раз і веде історію з пошуком, яка переживає
перезапуск. Він зчитує з того самого зображення й QR-коди та штрихкоди, а на комп'ютері Copilot+ ще
й перекладає результат. Усе відбувається на вашому комп'ютері — Glyfo не встановлює жодних
інтернет-з'єднань. Безкоштовно, без реклами та без покупок.

### Опис

Glyfo перетворює зображення з текстом на текст, яким можна користуватися.

Розпізнавання працює на OCR, вбудованому у Windows: без облікового запису, без завантаження на
сервер, без очікування. На комп'ютері Copilot+ Glyfo додатково задіює модель розпізнавання тексту
просто на пристрої для складних зображень і вміє перекласти результат — теж без мережі.

**П'ять способів завести зображення**
- Alt+Z обводить будь-яку частину екрана. Ctrl+Shift+R знімає весь екран. Обидві комбінації можна
  змінити на свої.
- Ctrl+V вставляє зображення з буфера обміну, а також текст.
- Ctrl+O відкриває зображення або PDF; перетягування у вікно працює так само.
- Клацніть зображення правою кнопкою у Провіднику й відкрийте його в Glyfo або надішліть із панелі
  спільного доступу Windows (Фотографії, Ножиці, браузер).
- Увімкніть стеження за буфером обміну — і кожен фрагмент, знятий через Win+Shift+S, розпізнається
  сам, а текст лишається в буфері, готовий до вставлення. Вимкнено, доки ви самі не попросите.

**Що ви отримуєте**
- Розпізнаний текст поруч із зображенням, у порядку початкового розташування.
- Копіювання одним клацанням або автоматичне копіювання знімка одразу після завершення.
- Збереження у файл .txt або .md через Ctrl+S.
- Пошук у тексті через Ctrl+F, поруч — кількість слів і символів.
- Читання вголос будь-яким голосом, встановленим на комп'ютері.
- «Прибрати переноси» зшиває жорсткі розриви рядків назад в абзаци, а «Прибрати пробіли» видаляє
  кожен пробіл — саме це потрібно китайському, японському та корейському тексту після
  розпізнавання.
- Кнопки для посилань, адрес електронної пошти та номерів телефону, знайдених у тексті.
- QR-коди та штрихкоди з того самого зображення.
- Історія з пошуком, яка переживає перезапуск, тож знімок, зроблений минулого тижня, усе ще за одне
  клацання. У ній зберігається лише текст, і її можна очистити або вимкнути.

**Більше ніж одна сторінка за раз**
- Відкрийте PDF і читайте його сторінка за сторінкою, гортаючи прямо у вікні.
- Або прочитайте весь документ за один раз і збережіть результат одним файлом чи окремим файлом на
  кожну сторінку.
- Киньте стос зображень у вікно й розпізнайте їх усі за один прохід.

**Ви вирішуєте, як воно читає**
- Мову розпізнавання оберіть із мовних пакетів, встановлених у Windows, або довіртеся Glyfo.
- Окремий параметр виправляє класичну помилку OCR, через яку v1.6.5 читається як vl.6.5: l або I
  стає одиницею лише там, де поруч стоять роздільник і цифра, тож html5 та IPv6 лишаються цілими.
- Вписати у вікно або показати в справжньому розмірі; розпізнати все зображення чи тільки виділене.
- Повернути зображення або вирівняти знятий під кутом знімок, перш ніж його читати.

**Воно не заважає**
- Після закриття вікна Glyfo лишається в області сповіщень, і комбінація клавіш працює далі.
  «Вийти» звідти закриває його по-справжньому. Сама ця поведінка теж є перемикачем.
- Glyfo може запускатися разом із Windows і одразу йти в область сповіщень, не відкриваючи вікна,
  тож комбінація працює з моменту входу в систему. Типово вимкнено; вмикаєте ви самі.
- Коли вікно приховане, сповіщення показує перший рядок щойно розпізнаного тексту.
- Вікно повертається того ж розміру й на те саме місце, де ви його лишили, у світлій або темній
  темі — як вам більше до вподоби.

**Мови**
Інтерфейс доступний 33 мовами й слідує за мовним налаштуванням Windows. Розпізнавання використовує
мовні пакети OCR, встановлені на комп'ютері — нові додаються в Параметри › Час і мова › Мова та
регіон.

**Конфіденційність**
Glyfo не встановлює мережевих з'єднань. Зображення, розпізнаний текст і переклади ніколи не
залишають ваш комп'ютер. Без облікового запису, без телеметрії, без реклами.

### Функції продукту

- Зніміть будь-яку частину екрана комбінацією клавіш на власний вибір і розпізнайте її одразу, не виходячи з програми, яку читали
- Спирається на вбудований в Windows OCR, а на комп'ютерах Copilot+ ще й на модель розпізнавання на пристрої
- Відкрити зображення або PDF, вставити з буфера обміну, перетягнути або прийняти з панелі спільного доступу Windows
- Читає цілий PDF чи стос зображень за один прохід, зберігаючи результат одним файлом або окремим файлом на сторінку
- Веде історію з пошуком, яка переживає перезапуск, і зберігає будь-який результат у файл .txt чи .md
- Зчитує QR-коди та штрихкоди з того самого зображення
- Перекладає на комп'ютерах Copilot+, просто на пристрої, без доступу до мережі
- Читає результат уголос будь-яким голосом на комп'ютері та прибирає переноси чи пробіли, як того потребує текст CJK
- Лишається в області сповіщень, тож комбінація клавіш працює й після закриття вікна; інтерфейс 33 мовами, світлий або темний
- Не під'єднується до інтернету: те, що ви розпізнаєте, не залишає ваш комп'ютер

### Підписи до знімків екрана

1. `01-text-from-a-page.png` — Розпізнаний текст стоїть поруч із зображенням у порядку, в якому його прочитали: копіюйте, слухайте або зшивайте назад в абзаци.
2. `02-any-language.png` — Мову розпізнавання ви обираєте з мовних пакетів, встановлених у Windows — або Glyfo визначає її сам.
3. `03-qr-and-barcodes.png` — QR-коди та штрихкоди зчитуються з того самого зображення; окрема програма-сканер не потрібна.
4. `04-history.png` — Останні результати лишаються в історії, тож знімок кількахвилинної давнини все ще за одне клацання.
5. `05-settings.png` — Мова інтерфейсу, запуск разом із Windows, перебування в області сповіщень і виправлення, яке не дає v1.6.5 стати vl.6.5.
6. `06-pdf-pages.png` — PDF відкривається посторінково: та сторінка, що перед вами, розпізнається одразу, а стрілки проводять рештою документа.
7. `07-batch.png` — Цілий документ або стос зображень проходить за один раз і зберігається одним файлом чи по файлу на сторінку.

### Пошукові терміни

`текст із зображення`, `скриншот у текст`, `PDF у текст`, `розпізнавання тексту`, `сканер QR-коду`, `сканер штрихкодів`, `OCR`

---

## Tiếng Việt — Vietnamese

### Mô tả ngắn

Glyfo lấy chữ ra khỏi mọi thứ bạn nhìn thấy: một ảnh chụp màn hình, ảnh chụp một trang sách, một
bản quét, một tệp PDF, một trang chiếu, một khung hình video. Nhấn Alt+Z rồi khoanh một vùng màn
hình, hoặc mở tệp, dán từ bảng tạm, gửi ảnh sang từ ứng dụng khác. Văn bản nhận dạng được hiện ngay
bên cạnh ảnh, sẵn sàng để sao chép, lưu lại, tìm kiếm hoặc nghe đọc. Glyfo đọc trọn một tệp PDF hay
cả một xấp ảnh trong một lượt, và giữ một lịch sử tìm kiếm được, còn nguyên sau khi khởi động lại.
Nó cũng đọc mã QR và mã vạch từ chính tấm ảnh đó, và trên máy Copilot+ thì dịch luôn kết quả. Mọi
thứ diễn ra trên máy của bạn — Glyfo không hề tạo kết nối internet nào. Miễn phí, không quảng cáo và
không mua thêm.

### Mô tả

Glyfo biến những tấm ảnh có chữ thành văn bản bạn dùng được.

Việc nhận dạng chạy trên OCR có sẵn trong Windows: không cần tài khoản, không tải lên, không chờ máy
chủ. Trên máy Copilot+, Glyfo dùng thêm mô hình nhận dạng văn bản ngay trên thiết bị cho những tấm
ảnh khó và có thể dịch kết quả — cũng không cần mạng.

**Năm cách đưa ảnh vào**
- Alt+Z khoanh bất kỳ vùng nào trên màn hình. Ctrl+Shift+R lấy toàn màn hình. Cả hai phím tắt đều
  đổi được theo ý bạn.
- Ctrl+V dán ảnh từ bảng tạm, dán chữ cũng được.
- Ctrl+O mở một tấm ảnh hay một tệp PDF; kéo tệp vào cửa sổ cũng cho kết quả như vậy.
- Bấm chuột phải vào một tấm ảnh trong File Explorer rồi mở bằng Glyfo, hoặc gửi từ bảng chia sẻ của
  Windows (Photos, Snipping Tool, trình duyệt).
- Bật theo dõi bảng tạm, và mỗi lần bạn cắt màn hình bằng Win+Shift+S thì ảnh đó tự được nhận dạng,
  chữ nằm sẵn trong bảng tạm để dán. Mặc định tắt cho tới khi bạn yêu cầu.

**Bạn nhận lại được gì**
- Văn bản nhận dạng nằm cạnh ảnh, theo đúng thứ tự bố cục ban đầu.
- Sao chép bằng một cú bấm, hoặc để bản chụp tự sao chép ngay khi xong.
- Lưu ra tệp .txt hoặc .md bằng Ctrl+S.
- Tìm trong văn bản bằng Ctrl+F, bên cạnh là số từ và số ký tự.
- Đọc to bằng bất kỳ giọng nào đã cài trên máy.
- “Bỏ ngắt dòng” nối những chỗ xuống dòng cứng trở lại thành đoạn văn, còn “Bỏ khoảng trắng” xoá
  từng khoảng trắng — đúng thứ mà văn bản tiếng Trung, tiếng Nhật và tiếng Hàn cần sau khi nhận
  dạng.
- Các nút cho đường liên kết, địa chỉ e-mail và số điện thoại tìm thấy trong văn bản.
- Mã QR và mã vạch từ chính tấm ảnh đó.
- Một lịch sử tìm kiếm được và còn nguyên sau khi khởi động lại, nên bản chụp từ tuần trước vẫn chỉ
  cách một cú bấm. Nó chỉ lưu chữ, và bạn có thể xoá sạch hoặc tắt hẳn.

**Nhiều trang cùng một lúc**
- Mở một tệp PDF và đọc từng trang, lật qua lại ngay trong cửa sổ.
- Hoặc đọc trọn tài liệu trong một lượt và lưu kết quả thành một tệp duy nhất hay mỗi trang một tệp.
- Thả cả một xấp ảnh vào cửa sổ và nhận dạng tất cả trong một lần chạy.

**Bạn quyết định cách nó đọc**
- Chọn ngôn ngữ nhận dạng trong số các gói đã cài trong Windows, hoặc để Glyfo tự chọn.
- Một tuỳ chọn sửa lỗi OCR kinh điển khiến v1.6.5 bị đọc thành vl.6.5: chữ l hay I chỉ biến thành số
  1 ở chỗ có dấu phân cách và chữ số đứng cạnh, nên html5 và IPv6 vẫn nguyên vẹn.
- Vừa khung cửa sổ hoặc xem đúng kích thước thật; nhận dạng cả tấm ảnh hoặc chỉ phần đã chọn.
- Xoay ảnh, hoặc nắn thẳng một tấm ảnh chụp bị nghiêng, trước khi đọc.

**Nó không vướng chân bạn**
- Đóng cửa sổ thì Glyfo vẫn nằm ở khay thông báo và phím tắt chụp vẫn chạy. Chọn “Thoát” ở đó mới
  thực sự đóng hẳn. Bản thân cách hoạt động này cũng là một tuỳ chọn.
- Glyfo có thể khởi động cùng Windows và đi thẳng xuống khay thông báo mà không mở cửa sổ nào, nên
  phím tắt dùng được ngay từ lúc bạn đăng nhập. Mặc định tắt; bạn tự bật.
- Khi cửa sổ đang ẩn, một thông báo hiện dòng đầu tiên của phần vừa nhận dạng xong.
- Cửa sổ mở lại đúng kích thước và đúng chỗ bạn để nó, sáng hay tối tuỳ bạn thích.

**Ngôn ngữ**
Giao diện có 33 ngôn ngữ và đi theo thiết lập ngôn ngữ của Windows. Việc nhận dạng dùng các gói ngôn
ngữ OCR đã cài trên máy — thêm gói mới ở Settings › Time & language › Language & region.

**Quyền riêng tư**
Glyfo không tạo kết nối mạng. Ảnh, văn bản nhận dạng và bản dịch không bao giờ rời khỏi máy bạn.
Không tài khoản, không thu thập dữ liệu, không quảng cáo.

### Tính năng sản phẩm

- Chụp bất kỳ vùng nào trên màn hình bằng phím tắt do bạn tự chọn và nhận dạng ngay, không phải rời khỏi ứng dụng bạn đang đọc
- Chạy trên OCR có sẵn của Windows, và trên máy Copilot+ còn dùng thêm mô hình nhận dạng trên thiết bị
- Mở ảnh hoặc PDF, dán từ bảng tạm, kéo thả hoặc nhận từ bảng chia sẻ của Windows
- Đọc trọn một tệp PDF hay cả xấp ảnh trong một lượt, lưu kết quả thành một tệp duy nhất hay mỗi trang một tệp
- Giữ một lịch sử tìm kiếm được và còn nguyên sau khi khởi động lại, và lưu bất kỳ kết quả nào ra tệp .txt hoặc .md
- Đọc mã QR và mã vạch từ chính tấm ảnh đó
- Dịch trên máy Copilot+, ngay trên thiết bị, không cần truy cập mạng
- Đọc to kết quả bằng bất kỳ giọng nào trên máy, và bỏ ngắt dòng hay khoảng trắng như văn bản CJK cần
- Nằm lại ở khay thông báo, nên phím tắt vẫn chạy sau khi bạn đóng cửa sổ; giao diện 33 ngôn ngữ, sáng hoặc tối
- Không kết nối internet: thứ bạn nhận dạng không rời khỏi máy bạn

### Chú thích ảnh chụp màn hình

1. `01-text-from-a-page.png` — Văn bản nhận dạng nằm cạnh tấm ảnh theo đúng thứ tự đã đọc: sẵn sàng để sao chép, nghe đọc hoặc nối lại thành đoạn văn.
2. `02-any-language.png` — Ngôn ngữ nhận dạng chọn trong số các gói đã cài trong Windows — hoặc để Glyfo tự quyết định.
3. `03-qr-and-barcodes.png` — Mã QR và mã vạch được đọc từ chính tấm ảnh đó; không cần thêm ứng dụng quét riêng.
4. `04-history.png` — Các kết quả gần đây nằm lại trong lịch sử, nên bản chụp vài phút trước vẫn chỉ cách một cú bấm.
5. `05-settings.png` — Ngôn ngữ giao diện, khởi động cùng Windows, nằm lại ở khay thông báo, và phần sửa lỗi giữ cho v1.6.5 không thành vl.6.5.
6. `06-pdf-pages.png` — PDF mở theo từng trang: trang đang xem được nhận dạng ngay, còn các mũi tên đưa bạn đi hết tài liệu.
7. `07-batch.png` — Cả một tài liệu hay một xấp ảnh chạy xong trong một lượt, lưu thành một tệp duy nhất hoặc mỗi trang một tệp.

### Từ khoá tìm kiếm

`ảnh sang chữ`, `chụp màn hình`, `PDF sang văn bản`, `nhận dạng văn bản`, `mã QR`, `mã vạch`, `OCR`

---

## ไทย — Thai

### คำอธิบายแบบสั้น

Glyfo ดึงข้อความออกมาจากทุกอย่างที่คุณเห็น ไม่ว่าจะเป็นภาพหน้าจอ ภาพถ่ายของหน้าหนังสือ เอกสารที่สแกนมา
ไฟล์ PDF สไลด์นำเสนอ หรือเฟรมจากวิดีโอ กด Alt+Z แล้วลากกรอบครอบส่วนใดก็ได้ของหน้าจอ หรือจะเปิดไฟล์
วางจากคลิปบอร์ด ส่งภาพมาจากแอปอื่นก็ได้ ข้อความที่อ่านได้จะปรากฏข้างภาพ พร้อมให้คัดลอก บันทึกเก็บไว้ ค้นหา
หรือฟังเสียงอ่าน Glyfo อ่าน PDF ทั้งเล่มหรือภาพทั้งกองได้ในครั้งเดียว และเก็บประวัติที่ค้นหาได้
ซึ่งยังอยู่ครบแม้เปิดโปรแกรมใหม่ ทั้งยังอ่านคิวอาร์โค้ดและบาร์โค้ดจากภาพเดียวกันนี้ด้วย และบนเครื่อง Copilot+
ยังแปลผลลัพธ์ให้อีก ทุกอย่างเกิดขึ้นบนเครื่องของคุณเอง Glyfo ไม่เชื่อมต่ออินเทอร์เน็ตเลยแม้แต่ครั้งเดียว
ใช้ฟรี ไม่มีโฆษณา และไม่มีการซื้อเพิ่ม

### คำอธิบาย

Glyfo เปลี่ยนภาพที่มีตัวหนังสือให้กลายเป็นข้อความที่คุณเอาไปใช้งานต่อได้

การอ่านข้อความทำงานบน OCR ที่มีอยู่แล้วในตัว Windows ไม่ต้องสมัครบัญชี ไม่ต้องอัปโหลด
ไม่ต้องรอเซิร์ฟเวอร์ บนเครื่อง Copilot+ นั้น Glyfo จะเรียกใช้โมเดลรู้จำข้อความที่ทำงานบนเครื่องเพิ่มอีกชั้น
สำหรับภาพที่อ่านยาก และแปลผลลัพธ์ให้ได้ด้วย ซึ่งก็ไม่ต้องใช้เครือข่ายเช่นกัน

**ห้าวิธีในการนำภาพเข้ามา**
- Alt+Z ลากกรอบครอบส่วนใดก็ได้ของหน้าจอ ส่วน Ctrl+Shift+R จับภาพทั้งหน้าจอ ปุ่มลัดทั้งสองชุดเปลี่ยนเองได้
- Ctrl+V วางภาพจากคลิปบอร์ด วางข้อความก็ได้เช่นกัน
- Ctrl+O เปิดภาพหรือไฟล์ PDF และการลากไฟล์เข้ามาในหน้าต่างก็ได้ผลเหมือนกัน
- คลิกขวาที่ภาพใน File Explorer แล้วเปิดด้วย Glyfo หรือส่งมาจากแผงแชร์ของ Windows (Photos,
  Snipping Tool, เว็บเบราว์เซอร์)
- เปิดการเฝ้าดูคลิปบอร์ดไว้ ทุกภาพที่คุณตัดด้วย Win+Shift+S จะถูกอ่านให้เองโดยอัตโนมัติ
  แล้ววางข้อความทิ้งไว้ในคลิปบอร์ดให้คุณวางต่อ ค่าเริ่มต้นคือปิดไว้จนกว่าคุณจะสั่งเปิด

**สิ่งที่คุณจะได้กลับมา**
- ข้อความที่อ่านได้วางอยู่ข้างภาพ เรียงตามลำดับเดิมของเนื้อหา
- คัดลอกด้วยคลิกเดียว หรือจะให้ภาพที่เพิ่งจับคัดลอกตัวเองทันทีที่อ่านเสร็จก็ได้
- บันทึกเป็นไฟล์ .txt หรือ .md ด้วย Ctrl+S
- ค้นหาในข้อความด้วย Ctrl+F พร้อมจำนวนคำและจำนวนอักขระอยู่ข้าง ๆ
- อ่านออกเสียงด้วยเสียงใดก็ได้ที่ติดตั้งอยู่บนเครื่อง
- “ลบการขึ้นบรรทัด” จะเชื่อมบรรทัดที่ถูกตัดแข็ง ๆ กลับเป็นย่อหน้า ส่วน “ลบช่องว่าง” จะลบช่องว่างทุกตัว
  ซึ่งเป็นสิ่งที่ข้อความภาษาจีน ญี่ปุ่น และเกาหลีต้องการหลังการอ่าน
- ปุ่มลัดสำหรับลิงก์ อีเมล และหมายเลขโทรศัพท์ที่พบในข้อความ
- คิวอาร์โค้ดและบาร์โค้ดจากภาพเดียวกัน
- ประวัติที่ค้นหาได้และยังอยู่ครบแม้เปิดโปรแกรมใหม่ ภาพที่จับไว้เมื่อสัปดาห์ก่อนจึงยังห่างแค่คลิกเดียว
  ประวัตินี้เก็บเฉพาะข้อความ และคุณจะล้างทิ้งหรือปิดไปเลยก็ได้

**อ่านทีละหลายหน้า**
- เปิดไฟล์ PDF แล้วอ่านทีละหน้า พลิกไปมาได้ในหน้าต่างเดียวกัน
- หรือจะอ่านทั้งเอกสารรวดเดียว แล้วบันทึกผลเป็นไฟล์เดียวหรือแยกไฟล์ละหน้าก็ได้
- ลากภาพทั้งกองมาวางบนหน้าต่าง แล้วอ่านทั้งหมดในรอบเดียว

**คุณเป็นคนกำหนดว่าจะให้อ่านอย่างไร**
- เลือกภาษาที่จะใช้อ่านจากชุดภาษาที่ติดตั้งไว้ใน Windows หรือปล่อยให้ Glyfo เลือกเอง
- มีตัวเลือกหนึ่งที่แก้ข้อผิดพลาดคลาสสิกของ OCR ที่อ่าน v1.6.5 เป็น vl.6.5 โดยตัว l หรือ I
  จะกลายเป็นเลข 1 เฉพาะตรงที่มีตัวคั่นและตัวเลขอยู่ข้าง ๆ เท่านั้น html5 และ IPv6 จึงไม่ถูกแตะต้อง
- ย่อให้พอดีหน้าต่างหรือดูขนาดจริง จะอ่านทั้งภาพหรืออ่านเฉพาะส่วนที่เลือกไว้ก็ได้
- หมุนภาพ หรือดัดภาพถ่ายที่ถ่ายมาเอียงให้ตรงก่อนอ่านก็ได้

**มันไม่เกะกะ**
- ปิดหน้าต่างแล้ว Glyfo จะยังอยู่ในพื้นที่แจ้งเตือน และปุ่มลัดสำหรับจับภาพก็ยังใช้ได้ ต้องเลือก “ออก”
  จากตรงนั้นจึงจะปิดจริง ๆ พฤติกรรมนี้เองก็เป็นตัวเลือกที่เปิดปิดได้
- Glyfo เริ่มทำงานพร้อม Windows แล้วลงไปอยู่ในพื้นที่แจ้งเตือนโดยไม่เปิดหน้าต่างเลยก็ได้
  ปุ่มลัดจึงพร้อมใช้ตั้งแต่วินาทีที่คุณลงชื่อเข้าใช้ ค่าเริ่มต้นคือปิดไว้ คุณเป็นคนเปิดเอง
- เมื่อหน้าต่างถูกซ่อนอยู่ การแจ้งเตือนจะแสดงบรรทัดแรกของข้อความที่เพิ่งอ่านได้
- หน้าต่างกลับมาที่ขนาดและตำแหน่งเดิมที่คุณทิ้งไว้ จะให้เป็นธีมสว่างหรือมืดก็เลือกได้ตามใจ

**ภาษา**
หน้าตาโปรแกรมมีให้เลือก 33 ภาษา และจะเดินตามการตั้งค่าภาษาของ Windows ส่วนการอ่านข้อความจะใช้ชุดภาษา OCR
ที่ติดตั้งอยู่บนเครื่อง เพิ่มชุดใหม่ได้ที่ การตั้งค่า › เวลาและภาษา › ภาษาและภูมิภาค

**ความเป็นส่วนตัว**
Glyfo ไม่เปิดการเชื่อมต่อเครือข่ายใด ๆ ภาพ ข้อความที่อ่านได้ และคำแปล จะไม่ออกจากเครื่องของคุณเลย
ไม่มีบัญชีผู้ใช้ ไม่มีการเก็บข้อมูลการใช้งาน ไม่มีโฆษณา

### คุณสมบัติของผลิตภัณฑ์

- จับภาพส่วนใดก็ได้ของหน้าจอด้วยปุ่มลัดที่คุณตั้งเอง แล้วอ่านข้อความทันที โดยไม่ต้องออกจากแอปที่คุณกำลังอ่านอยู่
- ทำงานบน OCR ที่มีมาในตัว Windows และบนเครื่อง Copilot+ ยังเสริมด้วยโมเดลรู้จำข้อความบนเครื่อง
- เปิดภาพหรือไฟล์ PDF วางจากคลิปบอร์ด ลากมาวาง หรือรับมาจากแผงแชร์ของ Windows
- อ่าน PDF ทั้งเล่มหรือภาพทั้งกองในรอบเดียว แล้วบันทึกผลเป็นไฟล์เดียวหรือแยกไฟล์ละหน้า
- เก็บประวัติที่ค้นหาได้และยังอยู่ครบแม้เปิดโปรแกรมใหม่ พร้อมบันทึกผลลัพธ์ใดก็ได้เป็นไฟล์ .txt หรือ .md
- อ่านคิวอาร์โค้ดและบาร์โค้ดจากภาพเดียวกัน
- แปลบนเครื่อง Copilot+ โดยประมวลผลบนเครื่อง ไม่ต้องต่อเครือข่าย
- อ่านผลลัพธ์ออกเสียงด้วยเสียงใดก็ได้บนเครื่อง และลบการขึ้นบรรทัดหรือช่องว่างอย่างที่ข้อความ CJK ต้องการ
- อยู่ต่อในพื้นที่แจ้งเตือน ปุ่มลัดจึงยังใช้ได้แม้ปิดหน้าต่างไปแล้ว หน้าตาโปรแกรม 33 ภาษา เลือกธีมสว่างหรือมืดได้
- ไม่ต่ออินเทอร์เน็ต สิ่งที่คุณอ่านจะไม่ออกไปจากเครื่องของคุณ

### คำบรรยายภาพหน้าจอ

1. `01-text-from-a-page.png` — ข้อความที่อ่านได้วางอยู่ข้างภาพตามลำดับที่อ่านมา พร้อมให้คัดลอก ฟังเสียงอ่าน หรือเชื่อมกลับเป็นย่อหน้า
2. `02-any-language.png` — เลือกภาษาที่จะใช้อ่านได้จากชุดภาษาที่ติดตั้งไว้ใน Windows หรือจะให้ Glyfo ตัดสินใจเองก็ได้
3. `03-qr-and-barcodes.png` — คิวอาร์โค้ดและบาร์โค้ดถูกอ่านจากภาพเดียวกันนี้ ไม่ต้องหาแอปสแกนอีกตัว
4. `04-history.png` — ผลลัพธ์ล่าสุดยังอยู่ในประวัติ ภาพที่จับไว้เมื่อไม่กี่นาทีก่อนจึงยังห่างแค่คลิกเดียว
5. `05-settings.png` — ภาษาของหน้าตาโปรแกรม การเริ่มพร้อม Windows การอยู่ต่อในพื้นที่แจ้งเตือน และตัวแก้ที่กัน v1.6.5 ไม่ให้กลายเป็น vl.6.5
6. `06-pdf-pages.png` — PDF เปิดทีละหน้า หน้าที่อยู่ตรงหน้าถูกอ่านทันที ส่วนลูกศรพาไล่ไปจนจบเอกสาร
7. `07-batch.png` — ทั้งเอกสารหรือภาพทั้งกองผ่านในรอบเดียว จะรวมเป็นไฟล์เดียวหรือแยกไฟล์ต่อหน้าก็ได้

### คำค้นหา

`ข้อความจากรูปภาพ`, `ภาพหน้าจอเป็นข้อความ`, `PDF เป็นข้อความ`, `รู้จำข้อความ`, `อ่านคิวอาร์โค้ด`, `สแกนบาร์โค้ด`, `OCR`

---

## Bahasa Indonesia — Indonesian

### Deskripsi singkat

Glyfo menarik teks dari apa pun yang Anda lihat: tangkapan layar, foto sebuah halaman, dokumen hasil
pindai, berkas PDF, salindia presentasi, satu bingkai video. Tekan Alt+Z lalu tarik kotak pada bagian
layar mana pun, atau buka berkas, tempel dari papan klip, kirim gambar dari aplikasi lain. Teks yang
terbaca muncul di samping gambar, siap disalin, disimpan, dicari, atau didengarkan. Glyfo membaca
satu PDF utuh atau setumpuk gambar dalam sekali jalan, dan menyimpan riwayat yang bisa dicari serta
tetap ada setelah aplikasi ditutup. Ia juga membaca kode QR dan kode batang dari gambar yang sama,
dan pada PC Copilot+ ia menerjemahkan hasilnya. Semuanya terjadi di PC Anda sendiri — Glyfo tidak
membuka koneksi internet sama sekali. Gratis, tanpa iklan dan tanpa pembelian.

### Deskripsi

Glyfo mengubah gambar berisi tulisan menjadi teks yang bisa Anda pakai.

Pembacaannya berjalan di atas OCR bawaan Windows: tanpa akun, tanpa unggahan, tanpa menunggu server.
Pada PC Copilot+, Glyfo juga memakai model pengenalan teks yang berjalan di perangkat untuk gambar
yang sulit dan dapat menerjemahkan hasilnya — ini pun tanpa jaringan.

**Lima cara memasukkan gambar**
- Alt+Z menarik kotak pada bagian layar mana pun. Ctrl+Shift+R mengambil seluruh layar. Kedua
  pintasan itu bebas Anda ganti.
- Ctrl+V menempelkan gambar dari papan klip, teks juga bisa.
- Ctrl+O membuka gambar atau berkas PDF; menyeretnya ke jendela sama saja hasilnya.
- Klik kanan sebuah gambar di File Explorer lalu buka dengan Glyfo, atau kirim lewat panel berbagi
  Windows (Photos, Snipping Tool, peramban).
- Nyalakan pemantauan papan klip, dan setiap potongan layar yang Anda ambil dengan Win+Shift+S
  terbaca dengan sendirinya, teksnya ditinggalkan di papan klip untuk Anda tempel. Mati sampai Anda
  memintanya.

**Apa yang Anda dapatkan**
- Teks yang terbaca di samping gambar, dalam urutan tata letak aslinya.
- Salin dengan satu klik, atau biarkan hasil tangkapan menyalin dirinya sendiri begitu selesai.
- Simpan ke berkas .txt atau .md dengan Ctrl+S.
- Cari di dalam teks dengan Ctrl+F, lengkap dengan jumlah kata dan karakter di sebelahnya.
- Pembacaan nyaring dengan suara mana pun yang terpasang di PC.
- “Hapus pemenggalan baris” menyambung kembali baris yang terpotong menjadi paragraf, dan “Hapus
  spasi” menghapus setiap spasi — persis yang dibutuhkan teks Tionghoa, Jepang, dan Korea setelah
  dibaca.
- Tombol untuk tautan, alamat surel, dan nomor telepon yang ditemukan di dalam teks.
- Kode QR dan kode batang dari gambar yang sama.
- Riwayat yang bisa dicari dan tetap ada setelah aplikasi ditutup, jadi tangkapan minggu lalu pun
  masih berjarak satu klik. Yang disimpan hanya teks, dan Anda bisa mengosongkan atau mematikannya.

**Lebih dari satu halaman sekaligus**
- Buka sebuah PDF dan baca halaman demi halaman, berpindah-pindah di dalam jendela.
- Atau baca seluruh dokumen sekaligus, lalu simpan hasilnya sebagai satu berkas atau satu berkas per
  halaman.
- Jatuhkan setumpuk gambar ke jendela dan baca semuanya dalam sekali jalan.

**Anda yang menentukan cara membacanya**
- Pilih bahasa pembacaan dari paket bahasa yang terpasang di Windows, atau serahkan pada Glyfo.
- Sebuah opsi memperbaiki kesalahan klasik OCR yang membaca v1.6.5 sebagai vl.6.5: huruf l atau I
  berubah menjadi angka 1 hanya di tempat yang bersebelahan dengan pemisah dan angka, jadi html5 dan
  IPv6 tetap utuh.
- Sesuaikan dengan jendela atau tampilkan pada ukuran asli; baca seluruh gambar atau hanya bagian
  yang dipilih.
- Putar gambar, atau luruskan foto yang terambil miring, sebelum dibaca.

**Ia tidak menghalangi**
- Menutup jendela membuat Glyfo tetap tinggal di area pemberitahuan, dan pintasan tangkapan tetap
  bekerja. “Keluar” dari sana barulah benar-benar menutupnya. Perilaku ini sendiri pun sebuah
  pengaturan.
- Glyfo bisa ikut menyala bersama Windows dan langsung masuk ke area pemberitahuan tanpa membuka
  jendela, sehingga pintasan siap sejak Anda masuk. Mati secara bawaan; Anda sendiri yang
  menyalakannya.
- Saat jendela tersembunyi, sebuah pemberitahuan menampilkan baris pertama dari yang baru terbaca.
- Jendela kembali pada ukuran dan tempat yang Anda tinggalkan, terang atau gelap sesuai selera Anda.

**Bahasa**
Antarmukanya tersedia dalam 33 bahasa dan mengikuti pengaturan bahasa Windows. Pembacaan memakai
paket bahasa OCR yang terpasang di PC — tambahkan lainnya lewat Settings › Time & language ›
Language & region.

**Privasi**
Glyfo tidak membuka koneksi jaringan. Gambar, teks yang terbaca, dan terjemahan tidak pernah keluar
dari PC Anda. Tanpa akun, tanpa telemetri, tanpa iklan.

### Fitur produk

- Tangkap bagian layar mana pun dengan pintasan pilihan Anda sendiri dan baca saat itu juga, tanpa keluar dari aplikasi yang sedang Anda baca
- Berjalan di atas OCR bawaan Windows, dan pada PC Copilot+ ditambah model pengenalan di perangkat
- Buka gambar atau PDF, tempel dari papan klip, seret dan lepas, atau terima lewat panel berbagi Windows
- Membaca satu PDF utuh atau setumpuk gambar dalam sekali jalan, menyimpan hasilnya sebagai satu berkas atau satu berkas per halaman
- Menyimpan riwayat yang bisa dicari dan tetap ada setelah aplikasi ditutup, serta menyimpan hasil apa pun ke berkas .txt atau .md
- Membaca kode QR dan kode batang dari gambar yang sama
- Menerjemahkan pada PC Copilot+, di perangkat, tanpa akses jaringan
- Membacakan hasilnya dengan suara mana pun di PC, dan menghapus pemenggalan baris atau spasi seperti yang dibutuhkan teks CJK
- Tetap tinggal di area pemberitahuan, jadi pintasan bekerja walau jendela sudah ditutup; antarmuka dalam 33 bahasa, terang atau gelap
- Tidak menyambung ke internet: apa yang Anda baca tidak keluar dari PC Anda

### Keterangan tangkapan layar

1. `01-text-from-a-page.png` — Teks yang terbaca berdiri di samping gambar dalam urutan saat dibaca: siap disalin, didengarkan, atau disambung kembali menjadi paragraf.
2. `02-any-language.png` — Bahasa pembacaan dipilih dari paket bahasa yang terpasang di Windows — atau Glyfo yang menentukannya sendiri.
3. `03-qr-and-barcodes.png` — Kode QR dan kode batang dibaca dari gambar yang sama; aplikasi pemindai terpisah tidak diperlukan.
4. `04-history.png` — Hasil terbaru tersimpan di riwayat, jadi tangkapan beberapa menit lalu masih berjarak satu klik.
5. `05-settings.png` — Bahasa antarmuka, menyala bersama Windows, tinggal di area pemberitahuan, dan koreksi yang menjaga v1.6.5 tidak menjadi vl.6.5.
6. `06-pdf-pages.png` — PDF terbuka per halaman: halaman yang sedang tampil langsung dibaca, dan panahnya membawa Anda menyusuri sisa dokumen.
7. `07-batch.png` — Satu dokumen utuh atau setumpuk gambar lewat dalam sekali jalan, disimpan sebagai satu berkas atau satu berkas per halaman.

### Istilah pencarian

`teks dari gambar`, `tangkapan layar ke teks`, `PDF ke teks`, `pengenalan teks`, `baca kode QR`, `pemindai barcode`, `OCR`

---

## Bahasa Melayu — Malay

### Penerangan ringkas

Glyfo mengeluarkan teks daripada apa sahaja yang anda lihat: tangkapan skrin, foto sesebuah halaman,
dokumen yang diimbas, fail PDF, slaid pembentangan, satu bingkai video. Tekan Alt+Z lalu bingkaikan
mana-mana bahagian skrin, atau buka fail, tampal daripada papan keratan, hantar imej dari aplikasi
lain. Teks yang dikenali muncul di sebelah imej, sedia untuk disalin, disimpan, dicari atau didengar.
Glyfo membaca satu PDF penuh atau setimbun imej dalam satu kali jalan, dan menyimpan sejarah yang
boleh dicari serta kekal selepas aplikasi ditutup. Ia turut membaca kod QR dan kod bar daripada imej
yang sama, dan pada PC Copilot+ ia menterjemah hasilnya. Semuanya berlaku pada PC anda sendiri —
Glyfo tidak membuka sebarang sambungan internet. Percuma, tanpa iklan dan tanpa pembelian.

### Penerangan

Glyfo menukar imej bertulisan menjadi teks yang boleh anda guna.

Pengecaman berjalan di atas OCR yang sedia ada dalam Windows: tiada akaun, tiada muat naik, tiada
menunggu pelayan. Pada PC Copilot+, Glyfo turut menggunakan model pengecaman teks yang berjalan pada
peranti untuk imej yang sukar dan boleh menterjemah hasilnya — itu pun tanpa rangkaian.

**Lima cara memasukkan imej**
- Alt+Z membingkaikan mana-mana bahagian skrin. Ctrl+Shift+R mengambil keseluruhan skrin. Kedua-dua
  pintasan itu boleh anda tukar sendiri.
- Ctrl+V menampal imej daripada papan keratan, teks juga boleh.
- Ctrl+O membuka imej atau fail PDF; menyeretnya ke dalam tetingkap memberi hasil yang sama.
- Klik kanan sesebuah imej dalam File Explorer lalu buka dengan Glyfo, atau hantar melalui panel
  perkongsian Windows (Photos, Snipping Tool, pelayar web).
- Hidupkan pemantauan papan keratan, dan setiap keratan yang anda ambil dengan Win+Shift+S dikenali
  dengan sendirinya, teksnya ditinggalkan pada papan keratan untuk anda tampal. Dimatikan sehingga
  anda memintanya.

**Apa yang anda dapat**
- Teks yang dikenali di sebelah imej, mengikut susunan asalnya.
- Salin dengan satu klik, atau biarkan tangkapan menyalin dirinya sendiri sebaik sahaja siap.
- Simpan ke fail .txt atau .md dengan Ctrl+S.
- Cari dalam teks dengan Ctrl+F, berserta bilangan perkataan dan aksara di sebelahnya.
- Bacaan kuat dengan mana-mana suara yang dipasang pada PC.
- “Buang pemisah baris” menyambung semula baris yang terputus menjadi perenggan, dan “Buang ruang”
  membuang setiap ruang — itulah yang diperlukan teks Cina, Jepun dan Korea selepas dikenali.
- Butang untuk pautan, alamat e-mel dan nombor telefon yang ditemui dalam teks.
- Kod QR dan kod bar daripada imej yang sama.
- Sejarah yang boleh dicari dan kekal selepas aplikasi ditutup, jadi tangkapan minggu lepas pun masih
  sejauh satu klik. Ia menyimpan teks sahaja, dan anda boleh mengosongkan atau mematikannya.

**Lebih daripada satu halaman serentak**
- Buka sebuah PDF dan bacanya halaman demi halaman, berpindah-pindah di dalam tetingkap.
- Atau baca keseluruhan dokumen sekali gus, lalu simpan hasilnya sebagai satu fail atau satu fail
  bagi setiap halaman.
- Lepaskan setimbun imej pada tetingkap dan kenali kesemuanya dalam satu kali jalan.

**Anda yang menentukan cara ia membaca**
- Pilih bahasa pengecaman daripada pakej bahasa yang dipasang dalam Windows, atau serahkan kepada
  Glyfo.
- Satu pilihan membetulkan ralat OCR klasik yang membaca v1.6.5 sebagai vl.6.5: huruf l atau I
  bertukar menjadi angka 1 hanya di tempat yang bersebelahan dengan pemisah dan digit, jadi html5
  dan IPv6 kekal seperti asal.
- Muatkan pada tetingkap atau lihat pada saiz sebenar; kenali keseluruhan imej atau bahagian yang
  dipilih sahaja.
- Putarkan imej, atau luruskan foto yang terambil senget, sebelum ia dibaca.

**Ia tidak menghalang**
- Menutup tetingkap membuatkan Glyfo kekal di kawasan pemberitahuan, dan pintasan tangkapan terus
  berfungsi. “Keluar” dari situ barulah menutupnya betul-betul. Kelakuan ini sendiri pun satu
  tetapan.
- Glyfo boleh dimulakan bersama Windows dan terus masuk ke kawasan pemberitahuan tanpa membuka
  tetingkap, jadi pintasan berfungsi sebaik sahaja anda log masuk. Dimatikan secara lalai; anda yang
  menghidupkannya.
- Ketika tetingkap tersembunyi, satu pemberitahuan menunjukkan baris pertama yang baru dikenali.
- Tetingkap kembali pada saiz dan tempat yang anda tinggalkan, cerah atau gelap mengikut pilihan
  anda.

**Bahasa**
Antara mukanya tersedia dalam 33 bahasa dan mengikut tetapan bahasa Windows. Pengecaman menggunakan
pakej bahasa OCR yang dipasang pada PC — tambah lagi melalui Settings › Time & language › Language &
region.

**Privasi**
Glyfo tidak membuka sambungan rangkaian. Imej, teks yang dikenali dan terjemahan tidak pernah keluar
daripada PC anda. Tiada akaun, tiada telemetri, tiada iklan.

### Ciri produk

- Tangkap mana-mana bahagian skrin dengan pintasan pilihan anda sendiri dan kenali serta-merta, tanpa keluar daripada aplikasi yang sedang anda baca
- Berjalan di atas OCR terbina dalam Windows, dan pada PC Copilot+ ditambah model pengecaman pada peranti
- Buka imej atau PDF, tampal daripada papan keratan, seret dan lepas, atau terima melalui panel perkongsian Windows
- Membaca satu PDF penuh atau setimbun imej dalam satu kali jalan, menyimpan hasilnya sebagai satu fail atau satu fail bagi setiap halaman
- Menyimpan sejarah yang boleh dicari dan kekal selepas aplikasi ditutup, serta menyimpan mana-mana hasil ke fail .txt atau .md
- Membaca kod QR dan kod bar daripada imej yang sama
- Menterjemah pada PC Copilot+, pada peranti, tanpa akses rangkaian
- Membacakan hasilnya dengan mana-mana suara pada PC, dan membuang pemisah baris atau ruang seperti yang diperlukan teks CJK
- Kekal di kawasan pemberitahuan, jadi pintasan berfungsi walaupun tetingkap sudah ditutup; antara muka dalam 33 bahasa, cerah atau gelap
- Tidak menyambung ke internet: apa yang anda kenali tidak keluar daripada PC anda

### Kapsyen tangkapan skrin

1. `01-text-from-a-page.png` — Teks yang dikenali berdiri di sebelah imej mengikut susunan ia dibaca: sedia untuk disalin, didengar atau disambung semula menjadi perenggan.
2. `02-any-language.png` — Bahasa pengecaman dipilih daripada pakej bahasa yang dipasang dalam Windows — atau Glyfo menentukannya sendiri.
3. `03-qr-and-barcodes.png` — Kod QR dan kod bar dibaca daripada imej yang sama; aplikasi pengimbas berasingan tidak diperlukan.
4. `04-history.png` — Hasil terkini kekal dalam sejarah, jadi tangkapan beberapa minit lalu masih sejauh satu klik.
5. `05-settings.png` — Bahasa antara muka, mula bersama Windows, kekal di kawasan pemberitahuan, dan pembetulan yang menghalang v1.6.5 daripada menjadi vl.6.5.
6. `06-pdf-pages.png` — PDF dibuka halaman demi halaman: halaman yang di depan terus dibaca, dan anak panah membawa anda menyusuri baki dokumen.
7. `07-batch.png` — Satu dokumen penuh atau setimbun imej lalu dalam sekali jalan, disimpan sebagai satu fail atau satu fail bagi setiap halaman.

### Istilah carian

`teks daripada imej`, `tangkapan skrin ke teks`, `PDF ke teks`, `pengecaman teks`, `baca kod QR`, `pengimbas kod bar`, `OCR`

---

## Filipino

### Maikling paglalarawan

Kinukuha ng Glyfo ang teksto mula sa kahit anong nakikita mo: isang screenshot, litrato ng isang
pahina, na-scan na dokumento, PDF, slide sa presentasyon, isang frame ng video. Pindutin ang Alt+Z at
kahunan ang alinmang bahagi ng screen, o magbukas ng file, mag-paste mula sa clipboard, magpadala ng
larawan mula sa ibang app. Lumalabas ang nabasang teksto sa tabi ng larawan, handa nang kopyahin,
i-save, hanapan o pakinggan. Nababasa ng Glyfo ang buong PDF o isang tumpok ng larawan sa isang
takbo, at nag-iingat ito ng kasaysayang mahahanapan na nananatili kahit isara mo ang app. Binabasa
rin nito ang mga QR code at barcode mula sa parehong larawan, at sa isang Copilot+ PC ay isinasalin
pa ang resulta. Lahat ay nangyayari sa sarili mong PC — hindi kumakabit ang Glyfo sa internet kahit
kailan. Libre, walang ad at walang bibilhin.

### Paglalarawan

Ginagawang magamit na teksto ng Glyfo ang mga larawang may nakasulat.

Umaandar ang pagbasa sa OCR na nasa Windows na: walang account, walang ia-upload, walang hinihintay
na server. Sa isang Copilot+ PC, ginagamit din ng Glyfo ang modelo ng pagkilala ng teksto na tumatakbo
sa mismong device para sa mahihirap na larawan, at kayang isalin ang resulta — wala ring network
dito.

**Limang paraan para makapasok ang larawan**
- Kinakahunan ng Alt+Z ang alinmang bahagi ng screen. Kinukuha ng Ctrl+Shift+R ang buong screen.
  Mapapalitan mo ang dalawang shortcut na iyon.
- Nagpe-paste ang Ctrl+V ng larawan mula sa clipboard, pati teksto.
- Nagbubukas ng larawan o PDF ang Ctrl+O; ganoon din kung kakaladkarin mo ito papasok sa window.
- I-right-click ang isang larawan sa File Explorer at buksan sa Glyfo, o ipadala mula sa share panel
  ng Windows (Photos, Snipping Tool, browser).
- Buksan ang pagbabantay sa clipboard at kusa nang nababasa ang bawat snip na kukunin mo sa
  Win+Shift+S, naiiwan sa clipboard ang teksto para i-paste mo. Naka-off hanggang hilingin mo.

**Ano ang makukuha mo**
- Ang nabasang teksto sa tabi ng larawan, sa pagkakasunod-sunod ng orihinal na ayos nito.
- Kopyahin sa isang click, o hayaang kusang kumopya ang isang capture pagkatapos nito.
- I-save sa isang .txt o .md na file gamit ang Ctrl+S.
- Maghanap sa loob ng teksto gamit ang Ctrl+F, may bilang ng salita at karakter sa tabi.
- Pagbabasa nang malakas gamit ang alinmang boses na naka-install sa PC.
- Pinagdurugtong muli ng “Alisin ang mga line break” ang matitigas na putol ng linya pabalik sa mga
  talata, at inaalis ng “Alisin ang mga espasyo” ang bawat espasyo — iyon mismo ang kailangan ng
  tekstong Tsino, Hapon at Koreano pagkatapos basahin.
- Mga button para sa mga link, e-mail address at numero ng telepono na nakita sa teksto.
- Mga QR code at barcode mula sa parehong larawan.
- Kasaysayang mahahanapan at nananatili kahit isara mo ang app, kaya isang click pa rin ang layo ng
  capture noong isang linggo. Teksto lang ang iniimbak nito, at puwede mong laktawan o patayin.

**Higit sa isang pahina nang sabay**
- Magbukas ng PDF at basahin ito pahina-pahina, gumagalaw dito sa loob mismo ng window.
- O basahin ang buong dokumento nang sabay-sabay, at i-save ang resulta bilang isang file o isang
  file kada pahina.
- Ihulog ang isang tumpok ng larawan sa window at basahin lahat sa isang takbo.

**Ikaw ang nagpapasya kung paano ito magbasa**
- Piliin ang wikang gagamitin sa pagbasa mula sa mga language pack na naka-install sa Windows, o
  ipaubaya sa Glyfo.
- May opsyong nag-aayos sa klasikong mali ng OCR kung saan nababasa ang v1.6.5 bilang vl.6.5: nagiging
  1 lamang ang l o I kung may separator at digit sa tabi nito, kaya nananatiling buo ang html5 at
  IPv6.
- Ikasya sa window o tingnan sa totoong laki; basahin ang buong larawan o ang napiling bahagi lang.
- Iikot ang larawan, o ituwid ang litratong pahilig ang pagkakakuha, bago ito basahin.

**Hindi ito nakakaabala**
- Kapag isinara mo ang window, nananatili ang Glyfo sa notification area at gumagana pa rin ang
  shortcut sa pagkuha. Ang “Lumabas” doon ang tunay na nagsasara nito. Isa ring setting ang ugaling
  ito.
- Puwedeng bumukas ang Glyfo kasabay ng Windows at dumiretso sa notification area nang hindi
  nagbubukas ng window, kaya gumagana ang shortcut mula sa sandaling mag-sign in ka. Naka-off bilang
  default; ikaw ang magbubukas nito.
- Kapag nakatago ang window, ipinapakita ng isang notification ang unang linya ng katatapos basahin.
- Bumabalik ang window sa laki at lugar kung saan mo ito iniwan, maliwanag o madilim ayon sa gusto
  mo.

**Mga wika**
Makukuha ang interface sa 33 wika at sumusunod ito sa setting ng wika ng Windows. Ginagamit ng
pagbasa ang mga OCR language pack na nasa PC — magdagdag pa sa Settings › Time & language › Language
& region.

**Privacy**
Walang binubuksang koneksyon sa network ang Glyfo. Hindi kailanman umaalis sa PC mo ang mga larawan,
ang nabasang teksto at ang mga salin. Walang account, walang telemetry, walang ad.

### Mga tampok ng produkto

- Kunan ang alinmang bahagi ng screen gamit ang shortcut na ikaw ang pumili at basahin agad, nang hindi umaalis sa app na binabasa mo
- Umaandar sa OCR na nakapaloob sa Windows, at sa mga Copilot+ PC ay may dagdag na modelo ng pagkilala sa device
- Magbukas ng larawan o PDF, mag-paste mula sa clipboard, mag-drag and drop o tumanggap mula sa share panel ng Windows
- Binabasa ang buong PDF o isang tumpok ng larawan sa isang takbo, sine-save ang resulta bilang isang file o isang file kada pahina
- Nag-iingat ng kasaysayang mahahanapan na nananatili kahit isara ang app, at sine-save ang alinmang resulta sa .txt o .md na file
- Binabasa ang mga QR code at barcode mula sa parehong larawan
- Nagsasalin sa mga Copilot+ PC, sa mismong device, nang walang access sa network
- Binabasa nang malakas ang resulta gamit ang alinmang boses sa PC, at inaalis ang mga line break o espasyong kailangan ng tekstong CJK
- Nananatili sa notification area, kaya gumagana ang shortcut kahit sarado na ang window; interface sa 33 wika, maliwanag o madilim
- Hindi kumakabit sa internet: hindi umaalis sa PC mo ang binabasa mo

### Mga caption ng screenshot

1. `01-text-from-a-page.png` — Nakatayo ang nabasang teksto sa tabi ng larawan sa pagkakasunod-sunod na binasa ito: handa nang kopyahin, pakinggan o pagdugtungin pabalik sa mga talata.
2. `02-any-language.png` — Ang wika sa pagbasa ay pipiliin mula sa mga language pack na naka-install sa Windows — o ang Glyfo na mismo ang magpapasya.
3. `03-qr-and-barcodes.png` — Binabasa ang mga QR code at barcode mula sa parehong larawan; hindi na kailangan ng hiwalay na scanner app.
4. `04-history.png` — Nananatili sa kasaysayan ang mga huling resulta, kaya isang click pa rin ang layo ng capture mula ilang minuto ang nakalipas.
5. `05-settings.png` — Wika ng interface, pagbukas kasabay ng Windows, pananatili sa notification area, at ang pagwawastong pumipigil sa v1.6.5 na maging vl.6.5.
6. `06-pdf-pages.png` — Bumubukas ang PDF nang pahina-pahina: agad nababasa ang nasa harap mo, at dadalhin ka ng mga arrow sa buong dokumento.
7. `07-batch.png` — Isang buong dokumento o isang salansan ng larawan ang dumadaan sa isang takbo, na iniimbak bilang isang file o isang file kada pahina.

### Mga termino sa paghahanap

`teksto mula sa larawan`, `screenshot sa teksto`, `PDF sa teksto`, `pagkilala ng teksto`, `basahin ang QR code`, `barcode scanner`, `OCR`

---

## हिन्दी — Hindi

### संक्षिप्त विवरण

Glyfo हर उस चीज़ से टेक्स्ट निकाल लेता है जो आपको दिखती है: स्क्रीनशॉट, किसी पन्ने की फ़ोटो, स्कैन
किया दस्तावेज़, PDF, प्रेज़ेंटेशन की स्लाइड, वीडियो का कोई फ़्रेम। Alt+Z दबाइए और स्क्रीन के किसी भी
हिस्से पर फ़्रेम खींचिए, या फ़ाइल खोलिए, क्लिपबोर्ड से पेस्ट कीजिए, किसी दूसरे ऐप से तस्वीर भेजिए।
पहचाना गया टेक्स्ट तस्वीर के बगल में आ जाता है — कॉपी करने, सहेजने, ढूँढ़ने या सुनने के लिए तैयार।
Glyfo पूरा PDF या तस्वीरों का पूरा ढेर एक ही बार में पढ़ लेता है और नतीजों का ऐसा इतिहास रखता है जिसमें
खोजा जा सकता है और जो ऐप दोबारा खोलने पर भी बना रहता है। यह उसी तस्वीर से QR कोड और बारकोड भी पढ़ लेता
है, और Copilot+ PC पर नतीजे का अनुवाद भी कर देता है। सब कुछ आपके अपने PC पर होता है — Glyfo इंटरनेट से
कोई कनेक्शन नहीं बनाता। मुफ़्त, बिना विज्ञापन और बिना किसी ख़रीदारी के।

### विवरण

Glyfo टेक्स्ट वाली तस्वीरों को ऐसे टेक्स्ट में बदल देता है जिसे आप काम में ले सकें।

पहचान Windows में पहले से मौजूद OCR पर चलती है: न कोई खाता, न कुछ अपलोड, न किसी सर्वर का इंतज़ार।
Copilot+ PC पर Glyfo मुश्किल तस्वीरों के लिए डिवाइस पर ही चलने वाले टेक्स्ट पहचान मॉडल का भी इस्तेमाल
करता है और नतीजे का अनुवाद कर सकता है — यह भी बिना नेटवर्क के।

**तस्वीर अंदर लाने के पाँच तरीके**
- Alt+Z स्क्रीन के किसी भी हिस्से पर फ़्रेम खींचता है। Ctrl+Shift+R पूरी स्क्रीन ले लेता है। दोनों
  शॉर्टकट आप अपनी पसंद के बदल सकते हैं।
- Ctrl+V क्लिपबोर्ड से तस्वीर चिपकाता है, टेक्स्ट भी।
- Ctrl+O तस्वीर या PDF खोलता है; उसे विंडो में खींचकर छोड़ना भी उतना ही काम करता है।
- File Explorer में किसी तस्वीर पर दायाँ क्लिक करके उसे Glyfo से खोलिए, या Windows के शेयर पैनल से
  भेजिए (Photos, Snipping Tool, ब्राउज़र)।
- क्लिपबोर्ड निगरानी चालू कर दीजिए, फिर Win+Shift+S से लिया हर स्निप अपने आप पढ़ लिया जाता है और
  टेक्स्ट क्लिपबोर्ड में ही रह जाता है, पेस्ट करने के लिए तैयार। जब तक आप न कहें, यह बंद रहती है।

**आपको क्या वापस मिलता है**
- पहचाना गया टेक्स्ट तस्वीर के बगल में, उसी क्रम में जिसमें वह सजा हुआ था।
- एक क्लिक में कॉपी, या कैप्चर पूरा होते ही उसे ख़ुद-ब-ख़ुद कॉपी हो जाने दीजिए।
- Ctrl+S से .txt या .md फ़ाइल में सहेजिए।
- Ctrl+F से टेक्स्ट में खोजिए, बगल में शब्दों और अक्षरों की गिनती के साथ।
- PC पर लगी किसी भी आवाज़ से ज़ोर से पढ़कर सुनाना।
- “लाइन ब्रेक हटाएँ” कड़े लाइन ब्रेक को दोबारा पैराग्राफ़ में जोड़ देता है और “स्पेस हटाएँ” हर स्पेस मिटा
  देता है — पहचान के बाद चीनी, जापानी और कोरियाई टेक्स्ट को यही चाहिए होता है।
- टेक्स्ट में मिले लिंक, ई-मेल पते और फ़ोन नंबरों के लिए बटन।
- उसी तस्वीर से QR कोड और बारकोड।
- ऐसा इतिहास जिसमें खोजा जा सकता है और जो ऐप दोबारा खोलने पर भी बना रहता है, इसलिए पिछले हफ़्ते लिया
  कैप्चर भी अब तक एक क्लिक की दूरी पर है। इसमें सिर्फ़ टेक्स्ट रखा जाता है, और इसे आप ख़ाली कर सकते हैं
  या बंद कर सकते हैं।

**एक बार में एक से ज़्यादा पन्ने**
- कोई PDF खोलिए और उसे पन्ना-दर-पन्ना पढ़िए, विंडो में ही आगे-पीछे जाते हुए।
- या पूरा दस्तावेज़ एक ही बार में पढ़िए और नतीजा एक फ़ाइल में या हर पन्ने की अलग फ़ाइल में सहेजिए।
- तस्वीरों का पूरा ढेर विंडो पर छोड़िए और सबको एक ही बार में पहचानिए।

**कैसे पढ़ना है, यह आप तय करते हैं**
- Windows में लगे भाषा पैकों में से पहचान की भाषा चुनिए, या Glyfo पर छोड़ दीजिए।
- एक विकल्प OCR की उस पुरानी ग़लती को सुधारता है जिसमें v1.6.5 को vl.6.5 पढ़ लिया जाता है: l या I तभी
  1 बनता है जब उसके बगल में कोई विभाजक और अंक हो, इसलिए html5 और IPv6 जस के तस रहते हैं।
- विंडो में फ़िट कीजिए या असली आकार में देखिए; पूरी तस्वीर पहचानिए या सिर्फ़ चुना हुआ हिस्सा।
- पढ़ने से पहले तस्वीर घुमाइए, या टेढ़ी खिंची फ़ोटो को सीधा कीजिए।

**यह रास्ते में नहीं आता**
- विंडो बंद करने पर Glyfo सूचना क्षेत्र में बना रहता है और कैप्चर का शॉर्टकट चलता रहता है। वहाँ से
  “बाहर निकलें” चुनने पर ही यह सचमुच बंद होता है। यह व्यवहार भी अपने आप में एक सेटिंग है।
- Glyfo Windows के साथ शुरू होकर बिना कोई विंडो खोले सीधे सूचना क्षेत्र में जा सकता है, ताकि साइन इन
  करते ही शॉर्टकट काम करने लगे। डिफ़ॉल्ट रूप से बंद; इसे आप ख़ुद चालू करते हैं।
- विंडो छिपी हो तो एक सूचना अभी-अभी पहचाने गए टेक्स्ट की पहली पंक्ति दिखा देती है।
- विंडो उसी आकार और उसी जगह लौट आती है जहाँ आपने उसे छोड़ा था, और आपकी पसंद के मुताबिक हल्की या गहरी
  रंगत में।

**भाषाएँ**
इंटरफ़ेस 33 भाषाओं में उपलब्ध है और Windows की भाषा सेटिंग के पीछे चलता है। पहचान PC पर लगे OCR भाषा
पैकों का इस्तेमाल करती है — और जोड़ने के लिए Settings › Time & language › Language & region पर जाइए।

**निजता**
Glyfo कोई नेटवर्क कनेक्शन नहीं बनाता। तस्वीरें, पहचाना गया टेक्स्ट और अनुवाद कभी आपके PC से बाहर नहीं
जाते। न कोई खाता, न टेलीमेट्री, न विज्ञापन।

### उत्पाद की विशेषताएँ

- अपनी पसंद के शॉर्टकट से स्क्रीन का कोई भी हिस्सा कैप्चर कीजिए और उसी वक़्त पहचानिए, जिस ऐप में पढ़ रहे थे उससे बाहर निकले बिना
- Windows में बने OCR पर चलता है, और Copilot+ PC पर डिवाइस पर ही चलने वाले पहचान मॉडल के साथ
- तस्वीर या PDF खोलिए, क्लिपबोर्ड से पेस्ट कीजिए, खींचकर छोड़िए या Windows के शेयर पैनल से पाइए
- पूरा PDF या तस्वीरों का ढेर एक ही बार में पढ़ता है और नतीजा एक फ़ाइल में या हर पन्ने की अलग फ़ाइल में सहेजता है
- ऐसा इतिहास रखता है जिसमें खोजा जा सके और जो ऐप दोबारा खोलने पर भी बना रहे, और किसी भी नतीजे को .txt या .md फ़ाइल में सहेजता है
- उसी तस्वीर से QR कोड और बारकोड पढ़ता है
- Copilot+ PC पर डिवाइस पर ही अनुवाद करता है, बिना किसी नेटवर्क पहुँच के
- PC की किसी भी आवाज़ से नतीजा ज़ोर से पढ़कर सुनाता है, और CJK टेक्स्ट के लिए ज़रूरी लाइन ब्रेक या स्पेस हटा देता है
- सूचना क्षेत्र में बना रहता है, इसलिए विंडो बंद करने के बाद भी शॉर्टकट चलता है; 33 भाषाओं में इंटरफ़ेस, हल्की या गहरी रंगत में
- इंटरनेट से नहीं जुड़ता: आप जो पहचानते हैं वह आपके PC से बाहर नहीं जाता

### स्क्रीनशॉट कैप्शन

1. `01-text-from-a-page.png` — पहचाना गया टेक्स्ट तस्वीर के बगल में उसी क्रम में खड़ा रहता है जिसमें उसे पढ़ा गया: कॉपी करने, सुनने या दोबारा पैराग्राफ़ में जोड़ने के लिए तैयार।
2. `02-any-language.png` — पहचान की भाषा Windows में लगे भाषा पैकों में से चुनी जाती है — या Glyfo ख़ुद तय कर लेता है।
3. `03-qr-and-barcodes.png` — QR कोड और बारकोड उसी तस्वीर से पढ़े जाते हैं; अलग से कोई स्कैनर ऐप नहीं चाहिए।
4. `04-history.png` — हाल के नतीजे इतिहास में बने रहते हैं, इसलिए कुछ मिनट पहले लिया कैप्चर अब भी एक क्लिक की दूरी पर है।
5. `05-settings.png` — इंटरफ़ेस की भाषा, Windows के साथ शुरू होना, सूचना क्षेत्र में बने रहना, और वह सुधार जो v1.6.5 को vl.6.5 बनने से रोकता है।
6. `06-pdf-pages.png` — PDF पन्ना-दर-पन्ना खुलती है: सामने वाला पन्ना तुरंत पढ़ लिया जाता है, और तीर आपको बाकी दस्तावेज़ में ले जाते हैं।
7. `07-batch.png` — पूरा दस्तावेज़ या तस्वीरों का ढेर एक ही बार में निपट जाता है — एक फ़ाइल में, या हर पन्ने की अलग फ़ाइल में।

### खोज शब्द

`छवि से टेक्स्ट`, `स्क्रीनशॉट से टेक्स्ट`, `PDF से टेक्स्ट`, `टेक्स्ट पहचान`, `QR कोड रीडर`, `बारकोड स्कैनर`, `OCR`

---

## বাংলা — Bengali

### সংক্ষিপ্ত বিবরণ

আপনি যা কিছু দেখতে পান, Glyfo তার ভেতর থেকে লেখাটা তুলে আনে: স্ক্রিনশট, কোনো পাতার ছবি, স্ক্যান করা
নথি, উপস্থাপনার স্লাইড, ভিডিওর একটা ফ্রেম। Alt+Z চাপুন আর পর্দার যেকোনো অংশে একটা ফ্রেম টানুন, কিংবা
ছবি বা PDF খুলুন, ক্লিপবোর্ড থেকে পেস্ট করুন, অন্য অ্যাপ থেকে ছবি পাঠান। শনাক্ত হওয়া লেখা ছবির পাশেই
এসে দাঁড়ায় — কপি করা, ফাইলে সংরক্ষণ করা, খুঁজে দেখা বা জোরে পড়ে শোনানোর জন্য তৈরি। একটা গোটা PDF কিংবা
একগাদা ছবি এক দফাতেই পড়ে নেওয়া যায়, আর ফলাফল থেকে যায় এমন একটা ইতিহাসে যেখানে খোঁজা যায় এবং যা অ্যাপ
আবার চালু করার পরেও টিকে থাকে। একই ছবি থেকে Glyfo QR কোড আর বারকোডও পড়ে নেয়, আর Copilot+ পিসিতে
ফলাফলটা অনুবাদও করে দেয়। সবকিছু ঘটে আপনার নিজের পিসিতেই — Glyfo ইন্টারনেটে কোনো সংযোগই তৈরি করে না।
বিনামূল্যে, বিজ্ঞাপন ছাড়া এবং কোনো কেনাকাটা ছাড়া।

### বিবরণ

লেখা আছে এমন ছবিকে Glyfo এমন লেখায় বদলে দেয় যা আপনি কাজে লাগাতে পারেন।

শনাক্তকরণ চলে Windows-এর ভেতরেই থাকা OCR-এর উপর: কোনো অ্যাকাউন্ট নেই, কিছু আপলোড করা নেই, সার্ভারের
জন্য অপেক্ষাও নেই। Copilot+ পিসিতে কঠিন ছবিগুলোর জন্য Glyfo ডিভাইসেই চলা টেক্সট শনাক্তকরণ মডেলটিও
কাজে লাগায় এবং ফলাফল অনুবাদ করতে পারে — সেটাও নেটওয়ার্ক ছাড়াই।

**ছবি ভেতরে আনার পাঁচটি উপায়**
- Alt+Z পর্দার যেকোনো অংশে ফ্রেম টানে। Ctrl+Shift+R পুরো পর্দাটাই নেয়। দুটো শর্টকাটই আপনি বদলে
  নিতে পারেন।
- Ctrl+V ক্লিপবোর্ড থেকে ছবি পেস্ট করে, লেখাও।
- Ctrl+O একটা ছবি বা একটা PDF খোলে; উইন্ডোর ভেতরে টেনে ছেড়ে দিলেও একই কাজ হয়।
- File Explorer-এ কোনো ছবিতে ডান ক্লিক করে সেটি Glyfo দিয়ে খুলুন, অথবা Windows-এর শেয়ার প্যানেল থেকে
  পাঠান (Photos, Snipping Tool, ব্রাউজার)।
- ক্লিপবোর্ড নজরদারি চালু করে দিন, তাহলে Win+Shift+S দিয়ে নেওয়া প্রতিটি স্নিপ নিজে থেকেই পড়া হয় আর
  লেখাটা ক্লিপবোর্ডে রেখে দেওয়া হয়, আপনি শুধু পেস্ট করবেন। আপনি না বলা পর্যন্ত এটি বন্ধ।

**আপনি কী ফেরত পান**
- শনাক্ত হওয়া লেখা ছবির পাশে, মূল বিন্যাসে যে ক্রমে ছিল সেই ক্রমেই।
- এক ক্লিকে কপি, কিংবা ক্যাপচার শেষ হওয়ামাত্র সেটিকে নিজে থেকেই কপি হতে দিন।
- Ctrl+S দিয়ে সেটিকে একটা .txt বা .md ফাইলে সংরক্ষণ করুন।
- Ctrl+F দিয়ে লেখার ভেতরে খুঁজুন, পাশেই শব্দ আর অক্ষরের সংখ্যা।
- পিসিতে বসানো যেকোনো কণ্ঠে জোরে পড়ে শোনানো।
- “লাইন ব্রেক সরান” শক্ত লাইন ভাঙাগুলোকে আবার অনুচ্ছেদে জুড়ে দেয় আর “স্পেস সরান” প্রতিটি ফাঁকা জায়গা
  মুছে দেয় — শনাক্তকরণের পরে চীনা, জাপানি ও কোরীয় লেখার ঠিক এটাই দরকার হয়।
- লেখার ভেতরে পাওয়া লিংক, ই-মেইল ঠিকানা আর ফোন নম্বরের জন্য আলাদা বোতাম।
- একই ছবি থেকে QR কোড আর বারকোড।
- এমন একটা ইতিহাস যেখানে খোঁজা যায় এবং যা অ্যাপ আবার চালু করার পরেও টিকে থাকে, তাই গত সপ্তাহের
  ক্যাপচারটাও এখনও এক ক্লিক দূরে। এতে কেবল লেখা জমা থাকে, আর আপনি সেটি খালি করতে বা বন্ধ করে দিতে
  পারেন।

**একবারে একের বেশি পাতা**
- একটা PDF খুলে উইন্ডোর ভেতরেই পাতা ধরে ধরে এগিয়ে যান।
- কিংবা গোটা নথিটা এক দফায় পড়ে নিন, আর ফলাফল একটাই ফাইলে অথবা প্রতি পাতায় একটা করে ফাইলে সংরক্ষণ
  করুন।
- একগাদা ছবি উইন্ডোর উপর ছেড়ে দিন আর সবগুলোকে একটা দফাতেই শনাক্ত করুন।

**কীভাবে পড়বে তা আপনিই ঠিক করেন**
- Windows-এ বসানো ভাষা প্যাকগুলোর মধ্য থেকে শনাক্তকরণের ভাষা বেছে নিন, অথবা Glyfo-র উপর ছেড়ে দিন।
- একটি বিকল্প OCR-এর সেই পুরোনো ভুলটা শুধরে দেয় যাতে v1.6.5 পড়া হয় vl.6.5 হিসেবে: l বা I কেবল
  তখনই 1 হয় যখন তার পাশে একটা বিভাজক আর একটা অঙ্ক থাকে, ফলে html5 আর IPv6 অক্ষত থাকে।
- উইন্ডোর মাপে বসান কিংবা আসল আকারে দেখুন; পুরো ছবি শনাক্ত করুন অথবা কেবল নির্বাচিত অংশটুকু।
- পড়ার আগে ছবিটা ঘুরিয়ে নিন, কিংবা কাত হয়ে তোলা ছবিটাকে সোজা করে নিন।

**এটি পথে দাঁড়ায় না**
- উইন্ডো বন্ধ করলে Glyfo বিজ্ঞপ্তি এলাকায় থেকে যায় আর ক্যাপচারের শর্টকাটও চলতে থাকে। সেখান থেকে
  “প্রস্থান” বেছে নিলে তবেই এটি সত্যিকারের বন্ধ হয়। এই আচরণটাও নিজেই একটা সেটিং।
- Glyfo Windows-এর সঙ্গে চালু হয়ে কোনো উইন্ডো না খুলেই সরাসরি বিজ্ঞপ্তি এলাকায় চলে যেতে পারে, ফলে
  সাইন ইন করার মুহূর্ত থেকেই শর্টকাট কাজ করে। ডিফল্টভাবে বন্ধ; আপনি নিজে এটি চালু করেন।
- উইন্ডো লুকানো থাকলে একটি বিজ্ঞপ্তি সদ্য শনাক্ত হওয়া লেখার প্রথম লাইনটি দেখায়।
- উইন্ডো যে মাপে আর যে জায়গায় রেখে গিয়েছিলেন, ঠিক সেখানেই ফিরে আসে — আর আপনার পছন্দমতো হালকা বা গাঢ়
  রঙে।

**ভাষা**
ইন্টারফেসটি ৩৩টি ভাষায় পাওয়া যায় এবং Windows-এর ভাষা সেটিং অনুসরণ করে। শনাক্তকরণ পিসিতে বসানো OCR
ভাষা প্যাকগুলো ব্যবহার করে — আরও যোগ করতে যান Settings › Time & language › Language & region-এ।

**গোপনীয়তা**
Glyfo কোনো নেটওয়ার্ক সংযোগ তৈরি করে না। ছবি, শনাক্ত হওয়া লেখা এবং অনুবাদ কখনোই আপনার পিসি ছেড়ে যায়
না। কোনো অ্যাকাউন্ট নেই, টেলিমেট্রি নেই, বিজ্ঞাপন নেই।

### পণ্যের বৈশিষ্ট্য

- আপনার নিজের বেছে নেওয়া শর্টকাট দিয়ে পর্দার যেকোনো অংশ ক্যাপচার করুন আর সঙ্গে সঙ্গেই শনাক্ত করুন, যে অ্যাপে পড়ছিলেন সেখান থেকে না বেরিয়েই
- Windows-এর ভেতরে থাকা OCR-এর উপর চলে, আর Copilot+ পিসিতে ডিভাইসেই চলা শনাক্তকরণ মডেলের সঙ্গে
- ছবি বা PDF খুলুন, ক্লিপবোর্ড থেকে পেস্ট করুন, টেনে ছেড়ে দিন বা Windows-এর শেয়ার প্যানেল থেকে গ্রহণ করুন
- একটা গোটা PDF কিংবা একগাদা ছবি এক দফাতেই পড়ে নেয়, আর ফলাফল একটাই ফাইলে অথবা প্রতি পাতায় একটা করে ফাইলে সংরক্ষণ করে
- এমন একটা ইতিহাস রাখে যেখানে খোঁজা যায় এবং যা অ্যাপ আবার চালু করার পরেও টিকে থাকে, আর যেকোনো ফলাফল .txt বা .md ফাইলে সংরক্ষণ করে
- একই ছবি থেকে QR কোড আর বারকোড পড়ে
- Copilot+ পিসিতে ডিভাইসেই অনুবাদ করে, কোনো নেটওয়ার্ক ব্যবহার ছাড়াই
- পিসিতে বসানো যেকোনো কণ্ঠে ফলাফল জোরে পড়ে শোনায়, আর CJK লেখার জন্য দরকারি লাইন ব্রেক বা স্পেস সরিয়ে দেয়
- বিজ্ঞপ্তি এলাকায় থেকে যায়, তাই উইন্ডো বন্ধ করার পরেও শর্টকাট কাজ করে; ৩৩টি ভাষায় ইন্টারফেস, হালকা বা গাঢ় রঙে
- ইন্টারনেটে যুক্ত হয় না: আপনি যা শনাক্ত করেন তা আপনার পিসি ছেড়ে যায় না

### স্ক্রিনশটের ক্যাপশন

1. `01-text-from-a-page.png` — শনাক্ত হওয়া লেখা ছবির পাশে সেই ক্রমেই দাঁড়িয়ে থাকে যে ক্রমে সেটি পড়া হয়েছে: কপি করতে, শুনতে বা আবার অনুচ্ছেদে জুড়তে তৈরি।
2. `02-any-language.png` — শনাক্তকরণের ভাষা Windows-এ বসানো ভাষা প্যাকগুলো থেকে বেছে নেওয়া হয় — কিংবা Glyfo নিজেই ঠিক করে নেয়।
3. `03-qr-and-barcodes.png` — QR কোড আর বারকোড একই ছবি থেকেই পড়া হয়; আলাদা কোনো স্ক্যানার অ্যাপ লাগে না।
4. `04-history.png` — সাম্প্রতিক ফলাফল ইতিহাসে থেকে যায়, তাই কয়েক মিনিট আগের ক্যাপচারটাও এখনও এক ক্লিক দূরে।
5. `05-settings.png` — ইন্টারফেসের ভাষা, Windows-এর সঙ্গে চালু হওয়া, বিজ্ঞপ্তি এলাকায় থেকে যাওয়া, আর সেই সংশোধন যা v1.6.5-কে vl.6.5 হতে দেয় না।
6. `06-pdf-pages.png` — PDF খোলে পাতা ধরে ধরে: সামনের পাতাটা সঙ্গে সঙ্গেই পড়া হয়ে যায়, আর তিরচিহ্ন দিয়ে বাকি নথিটা ঘুরে দেখা যায়।
7. `07-batch.png` — গোটা একটা নথি বা ছবির এক গাদা এক দফাতেই হয়ে যায় — একটি ফাইলে, কিংবা পাতাপিছু আলাদা ফাইলে।

### অনুসন্ধানের শব্দ

`ছবি থেকে টেক্সট`, `স্ক্রিনশট থেকে টেক্সট`, `PDF থেকে টেক্সট`, `টেক্সট শনাক্তকরণ`, `QR কোড রিডার`, `বারকোড স্ক্যানার`, `OCR`

---

## العربية — Arabic

### وصف مختصر

يستخرج Glyfo النص من كل ما تراه: لقطة شاشة، صورة لصفحة من كتاب، مستند ممسوح ضوئيًا، شريحة عرض، إطار
من مقطع فيديو. اضغط Alt+Z وحدّد إطارًا حول أي جزء من الشاشة، أو افتح صورة أو ملف PDF، أو الصق من
الحافظة، أو أرسل صورة من تطبيق آخر. يظهر النص المتعرَّف عليه بجوار الصورة، جاهزًا للنسخ أو الحفظ أو
البحث فيه أو الاستماع إليه. ويمكنك قراءة مستند PDF كامل أو كومة صور دفعةً واحدة، وتبقى النتائج في
سجلّ قابل للبحث لا يزول بإعادة تشغيل التطبيق. ويقرأ Glyfo أيضًا رموز QR والباركود من الصورة نفسها،
وعلى حاسوب Copilot+ يترجم النتيجة. كل ذلك يجري على حاسوبك وحده — لا ينشئ Glyfo أي اتصال بالإنترنت.
مجاني، بلا إعلانات وبلا مشتريات.

### الوصف

يحوّل Glyfo الصور التي تحمل نصًا إلى نص يمكنك استخدامه.

يعتمد التعرّف على محرك OCR المدمج في Windows: لا حساب تنشئه، ولا ملفات ترفعها، ولا انتظار لخادم.
وعلى حاسوب Copilot+ يستعين Glyfo إضافةً إلى ذلك بنموذج التعرّف على النص العامل داخل الجهاز في الصور
الصعبة، ويستطيع ترجمة النتيجة — ودون شبكة أيضًا.

**خمس طرق لإدخال صورة**
- اضغط Alt+Z لتحديد إطار حول أي جزء من الشاشة. ويلتقط Ctrl+Shift+R الشاشة كاملة. وكلا الاختصارين لك
  أن تغيّرهما.
- الصق صورة من الحافظة بالاختصار Ctrl+V، والنص كذلك.
- افتح صورة أو ملف PDF بالاختصار Ctrl+O؛ وسحب الملف إلى النافذة يؤدي الغرض نفسه.
- انقر بزر الفأرة الأيمن على صورة في مستكشف الملفات وافتحها بـ Glyfo، أو أرسلها من لوحة المشاركة في
  Windows (تطبيق الصور، أداة القص، المتصفح).
- فعّل مراقبة الحافظة، فتُقرأ كل قصاصة تأخذها بالاختصار Win+Shift+S وحدها، ويُترك النص في الحافظة
  لتلصقه. معطَّلة حتى تطلبها.

**ما الذي تحصل عليه**
- النص المتعرَّف عليه بجوار الصورة، بالترتيب الذي كان عليه في التخطيط الأصلي.
- النسخ بنقرة واحدة، أو دع اللقطة تنسخ نفسها بمجرد انتهائها.
- احفظ النتيجة في ملف بصيغة txt أو md بالاختصار Ctrl+S.
- ابحث داخل النص بالاختصار Ctrl+F، وإلى جواره عدد الكلمات والأحرف.
- القراءة بصوت عالٍ بأي صوت مثبَّت على الحاسوب.
- خيار «إزالة فواصل الأسطر» يعيد وصل الأسطر المقطوعة إلى فقرات، و«إزالة المسافات» يمحو كل مسافة —
  وهذا تحديدًا ما تحتاجه النصوص الصينية واليابانية والكورية بعد التعرّف عليها.
- أزرار للروابط وعناوين البريد الإلكتروني وأرقام الهاتف الموجودة في النص.
- رموز QR والباركود من الصورة نفسها.
- سجلّ قابل للبحث لا يزول بإعادة تشغيل التطبيق، فتبقى لقطة أخذتها الأسبوع الماضي على بعد نقرة واحدة.
  ولا يحفظ السجلّ سوى النص، ويمكنك إفراغه أو إيقافه.

**أكثر من صفحة واحدة في المرة**
- افتح ملف PDF واقرأه صفحةً صفحة، متنقلًا فيه داخل النافذة.
- أو اقرأ المستند كله دفعةً واحدة، واحفظ النتيجة في ملف واحد أو في ملف لكل صفحة.
- أفلت كومة من الصور على النافذة وتعرّف عليها جميعًا في تشغيلة واحدة.

**أنت من يقرر كيف يقرأ**
- اختر لغة التعرّف من بين حزم اللغات المثبَّتة في Windows، أو اترك الأمر لـ Glyfo.
- هناك خيار يصحّح خطأ OCR الشهير الذي يقرأ v1.6.5 على أنها vl.6.5: لا يتحول الحرف l أو I إلى الرقم 1
  إلا حيث يجاوره فاصل ورقم، فتبقى html5 و IPv6 على حالها.
- يمكنك ملاءمة الصورة للنافذة أو عرضها بحجمها الحقيقي؛ والتعرّف على الصورة كاملة أو على التحديد وحده.
- أدر الصورة، أو قوّم صورة التُقطت مائلة، قبل قراءتها.

**لا يقف في طريقك**
- عند إغلاق النافذة يبقى Glyfo في منطقة الإعلام، ويظل اختصار الالتقاط يعمل. أما «إنهاء» من هناك
  فيغلقه فعليًا. وهذا السلوك نفسه إعداد يمكنك تغييره.
- يستطيع Glyfo أن يبدأ مع Windows وأن ينتقل مباشرةً إلى منطقة الإعلام دون فتح أي نافذة، فيعمل
  الاختصار منذ لحظة تسجيل الدخول. معطَّل افتراضيًا؛ وأنت من يفعّله.
- عندما تكون النافذة مخفية، يعرض إشعارٌ أول سطر مما تم التعرّف عليه للتو.
- تعود النافذة بالحجم والموضع اللذين تركتها عليهما، فاتحة أو داكنة كما تفضّل.

**اللغات**
تتوفر الواجهة بـ 33 لغة وتتبع إعداد اللغة في Windows. ويستخدم التعرّف حزم لغات OCR المثبَّتة على
الحاسوب — وتضيف المزيد من الإعدادات › الوقت واللغة › اللغة والمنطقة.

**الخصوصية**
لا ينشئ Glyfo أي اتصالات شبكية. ولا تغادر الصور ولا النص المتعرَّف عليه ولا الترجمات حاسوبك أبدًا. لا
حساب، ولا قياس عن بُعد، ولا إعلانات.

### ميزات المنتج

- التقط أي جزء من الشاشة باختصار تختاره بنفسك وتعرّف عليه في الحال، دون مغادرة التطبيق الذي كنت تقرأ فيه
- يعتمد على محرك OCR المدمج في Windows، وعلى حواسيب Copilot+ على نموذج التعرّف العامل داخل الجهاز
- افتح صورة أو ملف PDF، أو الصق من الحافظة، أو اسحب وأفلت، أو استقبل من لوحة المشاركة في Windows
- يقرأ مستند PDF كاملًا أو كومة صور دفعةً واحدة، ويحفظ النتيجة في ملف واحد أو في ملف لكل صفحة
- يحتفظ بسجلّ قابل للبحث لا يزول بإعادة تشغيل التطبيق، ويحفظ أي نتيجة في ملف بصيغة txt أو md
- يقرأ رموز QR والباركود من الصورة نفسها
- يترجم على حواسيب Copilot+ داخل الجهاز، دون أي وصول إلى الشبكة
- يقرأ النتيجة بصوت عالٍ بأي صوت مثبَّت على الحاسوب، ويزيل فواصل الأسطر أو المسافات التي تحتاجها نصوص CJK
- يبقى في منطقة الإعلام، فيظل الاختصار يعمل حتى بعد إغلاق النافذة؛ وواجهة بـ 33 لغة، فاتحة أو داكنة
- لا يتصل بالإنترنت: ما تتعرّف عليه لا يغادر حاسوبك

### تعليقات لقطات الشاشة

1. `01-text-from-a-page.png` — يقف النص المتعرَّف عليه بجوار الصورة بالترتيب الذي قُرئ به: جاهزًا للنسخ أو الاستماع أو إعادة وصله إلى فقرات.
2. `02-any-language.png` — تُختار لغة التعرّف من بين حزم اللغات المثبَّتة في Windows — أو يحددها Glyfo بنفسه.
3. `03-qr-and-barcodes.png` — تُقرأ رموز QR والباركود من الصورة نفسها؛ ولا حاجة إلى تطبيق مسح منفصل.
4. `04-history.png` — تبقى النتائج الأخيرة في السجل، فتظل لقطة أخذتها قبل دقائق على بعد نقرة واحدة.
5. `05-settings.png` — لغة الواجهة، والبدء مع Windows، والبقاء في منطقة الإعلام، والتصحيح الذي يمنع تحوّل v1.6.5 إلى vl.6.5.
6. `06-pdf-pages.png` — يُفتح ملف PDF صفحةً صفحة: الصفحة التي أمامك يُتعرَّف عليها فورًا، والأسهم تنقلك في بقية المستند.
7. `07-batch.png` — مستند كامل أو كومة صور تمرّ في دفعة واحدة، وتُحفَظ في ملف واحد أو في ملف لكل صفحة.

### مصطلحات البحث

`نص من صورة`, `لقطة شاشة إلى نص`, `تحويل PDF إلى نص`, `التعرف على النص`, `قارئ رمز QR`, `ماسح الباركود`, `OCR`

---

## עברית — Hebrew

### תיאור קצר

Glyfo שולף את הטקסט מכל מה שאתם רואים: צילום מסך, תצלום של עמוד, מסמך סרוק, שקופית מצגת, פריים
מסרטון. הקישו Alt+Z וסמנו מסגרת סביב חלק כלשהו מהמסך, או פתחו תמונה או קובץ PDF, הדביקו מהלוח, שלחו
תמונה מאפליקציה אחרת. הטקסט שזוהה מופיע לצד התמונה, מוכן להעתקה, לשמירה, לחיפוש או להקראה. אפשר
לקרוא קובץ PDF שלם או ערימת תמונות בבת אחת, והתוצאות נשמרות בהיסטוריה שאפשר לחפש בה ושנשארת גם אחרי
הפעלה מחדש. Glyfo קורא גם קודי QR וברקודים מאותה תמונה עצמה, ובמחשב Copilot+ אף מתרגם את התוצאה.
הכול מתרחש במחשב שלכם — Glyfo אינו יוצר שום חיבור לאינטרנט. חינם, בלי פרסומות ובלי רכישות.

### תיאור

Glyfo הופך תמונות שיש בהן כתב לטקסט שאפשר להשתמש בו.

הזיהוי רץ על מנוע ה‑OCR המובנה ב‑Windows: בלי חשבון, בלי העלאה, בלי להמתין לשרת. במחשב Copilot+
נעזר Glyfo נוסף על כך במודל זיהוי הטקסט שרץ על המכשיר עצמו עבור התמונות הקשות, ויודע לתרגם את
התוצאה — גם זאת בלי רשת.

**חמש דרכים להכניס תמונה**
- הקישו Alt+Z כדי לסמן מסגרת סביב כל חלק במסך. Ctrl+Shift+R לוקח את המסך כולו. שני הקיצורים נתונים
  לשינוי שלכם.
- הדביקו תמונה מהלוח עם Ctrl+V, וגם טקסט.
- פתחו תמונה או קובץ PDF עם Ctrl+O; גרירה של הקובץ אל החלון עושה בדיוק את אותו הדבר.
- לחצו לחיצה ימנית על תמונה בסייר הקבצים ופתחו אותה ב‑Glyfo, או שלחו אותה מחלונית השיתוף של Windows
  (תמונות, כלי החיתוך, הדפדפן).
- הדליקו מעקב אחר הלוח, וכל גזירה שתעשו עם Win+Shift+S תיקרא בעצמה, והטקסט יישאר בלוח מוכן להדבקה.
  כבוי עד שתבקשו אותו.

**מה מקבלים בחזרה**
- הטקסט שזוהה לצד התמונה, בסדר שבו היה מסודר במקור.
- העתקה בלחיצה אחת, או לתת לצילום להעתיק את עצמו ברגע שהוא מסתיים.
- שמירה לקובץ txt או md עם Ctrl+S.
- חיפוש בתוך הטקסט עם Ctrl+F, ולצידו ספירת מילים ותווים.
- הקראה בקול בכל קול שמותקן במחשב.
- הפעולה ״הסרת שבירות שורה״ מחברת שוב שורות שנקטעו לפסקאות, ו״הסרת רווחים״ מוחקת כל רווח — בדיוק מה
  שטקסט סיני, יפני וקוריאני צריך אחרי הזיהוי.
- כפתורים לקישורים, לכתובות הדואר האלקטרוני ולמספרי הטלפון שנמצאו בטקסט.
- קודי QR וברקודים מאותה תמונה עצמה.
- היסטוריה שאפשר לחפש בה ושנשארת גם אחרי הפעלה מחדש, כך שצילום שעשיתם בשבוע שעבר עדיין במרחק לחיצה
  אחת. נשמר בה טקסט בלבד, ואפשר לרוקן אותה או לכבות אותה.

**יותר מעמוד אחד בכל פעם**
- פתחו קובץ PDF וקראו אותו עמוד אחר עמוד, תוך מעבר בו בתוך החלון.
- או קראו את המסמך כולו בבת אחת, ושמרו את התוצאה כקובץ אחד או כקובץ לכל עמוד.
- שחררו ערימת תמונות על החלון וזהו את כולן בהרצה אחת.

**אתם מחליטים איך הוא קורא**
- בחרו את שפת הזיהוי מתוך חבילות השפה המותקנות ב‑Windows, או הניחו ל‑Glyfo להחליט.
- אפשרות אחת מתקנת את שגיאת ה‑OCR הקלאסית שבה v1.6.5 נקרא vl.6.5: האות l או I הופכת ל‑1 רק במקום
  שבו ניצבים לצידה מפריד וספרה, כך ש‑html5 ו‑IPv6 נשארים כפי שהם.
- התאימו לחלון או הציגו בגודל אמיתי; זהו את התמונה כולה או רק את הבחירה.
- סובבו תמונה, או יישרו תצלום שצולם בזווית, לפני הקריאה.

**הוא לא עומד בדרך**
- סגירת החלון משאירה את Glyfo באזור ההתראות, וקיצור המקשים ממשיך לעבוד. ״יציאה״ משם סוגר אותו
  באמת. ההתנהגות הזו היא בעצמה הגדרה.
- Glyfo יכול לעלות יחד עם Windows וללכת היישר לאזור ההתראות בלי לפתוח חלון, כך שקיצור המקשים עובד
  מרגע הכניסה למערכת. כבוי כברירת מחדל; אתם מדליקים אותו.
- כשהחלון מוסתר, התראה מציגה את השורה הראשונה של מה שזה עתה זוהה.
- החלון חוזר בגודל ובמקום שבהם השארתם אותו, בהיר או כהה לפי טעמכם.

**שפות**
הממשק זמין ב‑33 שפות ועוקב אחר הגדרת השפה של Windows. הזיהוי משתמש בחבילות שפת ה‑OCR המותקנות
במחשב — מוסיפים עוד דרך הגדרות › שעה ושפה › שפה ואזור.

**פרטיות**
Glyfo אינו יוצר חיבורי רשת. תמונות, טקסט שזוהה ותרגומים לעולם אינם עוזבים את המחשב שלכם. בלי חשבון,
בלי טלמטריה, בלי פרסומות.

### תכונות המוצר

- צלמו כל חלק במסך עם קיצור מקשים שאתם בוחרים וזהו אותו מיד, בלי לצאת מהאפליקציה שבה קראתם
- רץ על מנוע ה‑OCR המובנה ב‑Windows, ובמחשבי Copilot+ גם על מודל הזיהוי שעל המכשיר
- פתיחת תמונה או קובץ PDF, הדבקה מהלוח, גרירה ושחרור או קבלה מחלונית השיתוף של Windows
- קורא קובץ PDF שלם או ערימת תמונות בהרצה אחת, ושומר את התוצאה כקובץ אחד או כקובץ לכל עמוד
- שומר היסטוריה שאפשר לחפש בה ושנשארת גם אחרי הפעלה מחדש, ושומר כל תוצאה לקובץ txt או md
- קורא קודי QR וברקודים מאותה תמונה עצמה
- מתרגם במחשבי Copilot+ על המכשיר עצמו, בלי גישה לרשת
- מקריא את התוצאה בקול בכל קול שמותקן במחשב, ומסיר שבירות שורה או רווחים שטקסט CJK זקוק להם
- נשאר באזור ההתראות, כך שקיצור המקשים עובד גם אחרי סגירת החלון; ממשק ב‑33 שפות, בהיר או כהה
- אינו מתחבר לאינטרנט: מה שאתם מזהים לא עוזב את המחשב שלכם

### כיתובים לצילומי מסך

1. `01-text-from-a-page.png` — הטקסט שזוהה עומד לצד התמונה בסדר שבו נקרא: מוכן להעתקה, להאזנה או לחיבור מחדש לפסקאות.
2. `02-any-language.png` — שפת הזיהוי נבחרת מתוך חבילות השפה המותקנות ב‑Windows — או ש‑Glyfo קובע אותה בעצמו.
3. `03-qr-and-barcodes.png` — קודי QR וברקודים נקראים מאותה תמונה עצמה; אין צורך באפליקציית סריקה נפרדת.
4. `04-history.png` — התוצאות האחרונות נשארות בהיסטוריה, כך שצילום מלפני כמה דקות עדיין במרחק לחיצה אחת.
5. `05-settings.png` — שפת הממשק, עלייה יחד עם Windows, הישארות באזור ההתראות, והתיקון שמונע מ‑v1.6.5 להפוך ל‑vl.6.5.
6. `06-pdf-pages.png` — קובץ PDF נפתח עמוד אחר עמוד: העמוד שלפניך מזוהה מיד, והחצים מעבירים אותך בשאר המסמך.
7. `07-batch.png` — מסמך שלם או ערימת תמונות עוברים בהרצה אחת, ונשמרים כקובץ אחד או כקובץ לכל עמוד.

### מונחי חיפוש

`טקסט מתמונה`, `צילום מסך לטקסט`, `המרת PDF לטקסט`, `זיהוי טקסט`, `קורא קוד QR`, `סורק ברקוד`, `OCR`

---

## فارسی — Persian

### توضیح کوتاه

Glyfo متن را از هر چیزی که می‌بینید بیرون می‌کشد: از یک اسکرین‌شات، از عکس یک صفحه، از سندی که
اسکن شده، از اسلاید ارائه، از یک فریم ویدیو. کلید Alt+Z را بزنید و دور بخشی از صفحه کادر بکشید، یا
تصویر یا فایل PDF باز کنید، از کلیپ‌بورد بچسبانید، تصویری را از برنامه‌ای دیگر بفرستید. متن
شناسایی‌شده کنار تصویر ظاهر می‌شود، آماده‌ی کپی کردن، ذخیره کردن، جست‌وجو کردن یا با صدای بلند شنیدن.
یک PDF کامل یا یک دسته تصویر را می‌شود یک‌جا خواند، و نتیجه‌ها در تاریخچه‌ای می‌مانند که جست‌وجو‌پذیر
است و با بستن و باز کردن دوباره‌ی برنامه از بین نمی‌رود. Glyfo کدهای QR و بارکد را هم از همان تصویر
می‌خواند، و روی رایانه‌ی Copilot+ نتیجه را ترجمه می‌کند. همه‌چیز روی رایانه‌ی خودتان انجام می‌شود —
Glyfo هیچ اتصالی به اینترنت برقرار نمی‌کند. رایگان، بدون تبلیغات و بدون خرید.

### توضیحات

Glyfo تصویرهایی را که در آن‌ها نوشته هست به متنی تبدیل می‌کند که می‌توانید به کارش ببرید.

شناسایی روی همان OCR کار می‌کند که در Windows تعبیه شده است: نه حسابی می‌سازید، نه چیزی آپلود
می‌کنید، نه منتظر سروری می‌مانید. روی رایانه‌ی Copilot+، افزون بر آن، Glyfo برای تصویرهای دشوار از
مدل شناسایی متن که روی خود دستگاه اجرا می‌شود کمک می‌گیرد و می‌تواند نتیجه را ترجمه کند — آن هم بدون
شبکه.

**پنج راه برای وارد کردن تصویر**
- کلید Alt+Z دور هر بخشی از صفحه کادر می‌کشد. کلید Ctrl+Shift+R تمام صفحه را برمی‌دارد. هر دو میان‌بر
  را خودتان می‌توانید عوض کنید.
- کلید Ctrl+V تصویر را از کلیپ‌بورد می‌چسباند، متن را هم همین‌طور.
- کلید Ctrl+O یک تصویر یا یک PDF را باز می‌کند؛ کشیدن فایل داخل پنجره هم دقیقاً همان کار را می‌کند.
- روی تصویری در File Explorer راست‌کلیک کنید و آن را با Glyfo باز کنید، یا از پنل اشتراک‌گذاری
  Windows بفرستید (Photos، Snipping Tool، مرورگر).
- پایش کلیپ‌بورد را روشن کنید تا هر برشی که با Win+Shift+S می‌گیرید خودبه‌خود خوانده شود و متنش روی
  کلیپ‌بورد بماند تا بچسبانید. تا نخواهید خاموش است.

**چه چیزی به دست می‌آورید**
- متن شناسایی‌شده کنار تصویر، به همان ترتیبی که در چیدمان اصلی بوده است.
- کپی با یک کلیک، یا بگذارید یک برداشت به‌محض تمام‌شدن، خودش را کپی کند.
- ذخیره‌ی نتیجه در یک فایل txt یا md با Ctrl+S.
- جست‌وجو داخل متن با Ctrl+F، و کنارش شمار واژه‌ها و نویسه‌ها.
- خواندن با صدای بلند با هر صدایی که روی رایانه نصب است.
- گزینه‌ی «حذف شکست خط» خط‌های بریده‌شده را دوباره به پاراگراف وصل می‌کند و «حذف فاصله‌ها» هر فاصله
  را پاک می‌کند — دقیقاً همان چیزی که متن چینی، ژاپنی و کره‌ای پس از شناسایی لازم دارد.
- دکمه‌هایی برای پیوندها، نشانی‌های ایمیل و شماره‌های تلفنی که در متن پیدا شده‌اند.
- کدهای QR و بارکد از همان تصویر.
- تاریخچه‌ای جست‌وجو‌پذیر که با بستن و باز کردن دوباره‌ی برنامه از بین نمی‌رود، پس برداشتی که هفته‌ی
  پیش گرفته‌اید هنوز یک کلیک فاصله دارد. تنها متن را نگه می‌دارد و می‌توانید خالی‌اش کنید یا خاموشش
  کنید.

**بیش از یک صفحه در یک نوبت**
- یک PDF را باز کنید و همان‌جا در پنجره صفحه‌به‌صفحه پیش بروید.
- یا کل سند را یک‌جا بخوانید و نتیجه را در یک فایل یا در هر صفحه یک فایل ذخیره کنید.
- یک دسته تصویر را روی پنجره رها کنید و همه را در یک نوبت شناسایی کنید.

**شما تصمیم می‌گیرید چطور بخواند**
- زبان شناسایی را از میان بسته‌های زبانی نصب‌شده در Windows انتخاب کنید، یا تصمیم را به Glyfo
  بسپارید.
- گزینه‌ای آن خطای کلاسیک OCR را درست می‌کند که v1.6.5 را vl.6.5 می‌خواند: حرف l یا I تنها جایی به
  عدد ۱ تبدیل می‌شود که کنارش یک جداکننده و یک رقم ایستاده باشد، پس html5 و IPv6 دست‌نخورده
  می‌مانند.
- می‌توانید تصویر را اندازه‌ی پنجره کنید یا در اندازه‌ی واقعی ببینید؛ کل تصویر را شناسایی کنید یا
  فقط بخش انتخاب‌شده را.
- پیش از خواندن، تصویر را بچرخانید یا عکسی را که کج گرفته شده صاف کنید.

**سر راهتان نیست**
- بستن پنجره Glyfo را در ناحیه‌ی اعلان نگه می‌دارد و میان‌بر برداشت همچنان کار می‌کند. «خروج» از
  همان‌جا واقعاً می‌بنددش. خودِ همین رفتار هم یک تنظیم است.
- Glyfo می‌تواند همراه Windows بالا بیاید و بدون باز کردن هیچ پنجره‌ای یک‌راست به ناحیه‌ی اعلان
  برود، تا میان‌بر از همان لحظه‌ی ورود به سیستم کار کند. به‌طور پیش‌فرض خاموش است؛ خودتان روشنش
  می‌کنید.
- وقتی پنجره پنهان است، یک اعلان سطر اول چیزی را که همین حالا شناسایی شده نشان می‌دهد.
- پنجره با همان اندازه و در همان جایی برمی‌گردد که رهایش کرده بودید، روشن یا تیره، هر طور که بپسندید.

**زبان‌ها**
رابط کاربری به ۳۳ زبان در دسترس است و از تنظیم زبان Windows پیروی می‌کند. شناسایی از بسته‌های زبانی
OCR نصب‌شده روی رایانه استفاده می‌کند — بسته‌های تازه را از Settings › Time & language › Language &
region اضافه کنید.

**حریم خصوصی**
Glyfo هیچ اتصال شبکه‌ای برقرار نمی‌کند. تصویرها، متن شناسایی‌شده و ترجمه‌ها هرگز از رایانه‌ی شما
بیرون نمی‌روند. نه حسابی، نه داده‌ی از راه دور، نه تبلیغی.

### ویژگی‌های محصول

- هر بخشی از صفحه را با میان‌بری که خودتان انتخاب می‌کنید بردارید و همان‌جا شناسایی کنید، بی‌آنکه از برنامه‌ای که در آن می‌خواندید بیرون بیایید
- روی OCR تعبیه‌شده در Windows کار می‌کند و روی رایانه‌های Copilot+ با مدل شناسایی روی خود دستگاه
- تصویر یا PDF باز کنید، از کلیپ‌بورد بچسبانید، بکشید و رها کنید یا از پنل اشتراک‌گذاری Windows دریافت کنید
- یک PDF کامل یا یک دسته تصویر را در یک نوبت می‌خواند و نتیجه را در یک فایل یا در هر صفحه یک فایل ذخیره می‌کند
- تاریخچه‌ای جست‌وجو‌پذیر نگه می‌دارد که با باز کردن دوباره‌ی برنامه از بین نمی‌رود، و هر نتیجه را در فایل txt یا md ذخیره می‌کند
- کدهای QR و بارکد را از همان تصویر می‌خواند
- روی رایانه‌های Copilot+ همان‌جا روی دستگاه ترجمه می‌کند، بدون دسترسی به شبکه
- نتیجه را با هر صدایی که روی رایانه نصب است با صدای بلند می‌خواند و شکست خط یا فاصله‌هایی را که متن CJK لازم دارد برمی‌دارد
- در ناحیه‌ی اعلان می‌ماند، پس میان‌بر حتی پس از بستن پنجره کار می‌کند؛ رابط کاربری به ۳۳ زبان، روشن یا تیره
- به اینترنت وصل نمی‌شود: آنچه شناسایی می‌کنید از رایانه‌ی شما بیرون نمی‌رود

### زیرنویس تصویرها

1. `01-text-from-a-page.png` — متن شناسایی‌شده به همان ترتیبی که خوانده شده کنار تصویر می‌ایستد: آماده‌ی کپی، شنیدن یا وصل شدن دوباره به پاراگراف.
2. `02-any-language.png` — زبان شناسایی از میان بسته‌های زبانی نصب‌شده در Windows انتخاب می‌شود — یا Glyfo خودش آن را تعیین می‌کند.
3. `03-qr-and-barcodes.png` — کدهای QR و بارکد از همان تصویر خوانده می‌شوند؛ به برنامه‌ی اسکن جداگانه نیازی نیست.
4. `04-history.png` — نتیجه‌های اخیر در تاریخچه می‌مانند، پس برداشتی از چند دقیقه پیش هنوز یک کلیک فاصله دارد.
5. `05-settings.png` — زبان رابط کاربری، بالا آمدن همراه Windows، ماندن در ناحیه‌ی اعلان، و اصلاحی که نمی‌گذارد v1.6.5 به vl.6.5 تبدیل شود.
6. `06-pdf-pages.png` — PDF صفحه‌به‌صفحه باز می‌شود: صفحه‌ای که پیش رویتان است بی‌درنگ شناسایی می‌شود و پیکان‌ها شما را در بقیه‌ی سند می‌گردانند.
7. `07-batch.png` — یک سند کامل یا دسته‌ای از تصویرها در یک نوبت پردازش می‌شود؛ در یک فایل، یا برای هر صفحه یک فایل.

### عبارت‌های جست‌وجو

`متن از تصویر`, `اسکرین‌شات به متن`, `تبدیل PDF به متن`, `تشخیص متن`, `خواننده کد QR`, `اسکنر بارکد`, `OCR`
