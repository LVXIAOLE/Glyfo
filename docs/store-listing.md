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

The same five PNGs go into every language listing — the Store does not share images between them, and
none of the five has any text burned into it. Captions are per language; they are numbered below in
upload order. The images are in `docs/store-screenshots/`; `tools/store-shots/` is what produced
them, off the real packaged app.

Two things to keep true if this text is edited: the app makes no network connections, and
translation exists only on Copilot+ PCs. Both are claims a certification reviewer can check, and
the privacy policy at <https://lvxiaole.github.io/glyfo-site/privacy.html> repeats the first one.

Shortcuts named below are the ones the app actually registers (`MainWindow.xaml.cs:1710`): region
capture is Alt+Z, or Ctrl+Shift+G when another app already holds Alt+Z; whole screen is
Ctrl+Shift+R.

---

## English (default)

### 简短说明 — Short description

Glyfo pulls text out of anything you can see: a screenshot, a photo of a page, a scanned form, a
slide, a video still. Press Alt+Z and drag a box around part of the screen, or open a file, paste
from the clipboard, or send a picture over from another app. The text comes back beside the image,
ready to copy, read aloud or clean up. It reads QR codes and barcodes from the same picture, and on
a Copilot+ PC it can translate the result. Everything happens on your PC — Glyfo makes no internet
connections at all. Free, with no ads and nothing to buy.

### 说明 — Description

Glyfo turns pictures of text into text you can use.

Recognition runs on the OCR that ships with Windows, so there is no account to create, no upload
and no waiting on a server. On a Copilot+ PC, Glyfo also uses the on-device text recognition model
for harder images and can translate what it found — still without going online.

**Four ways to get an image in**
- Press Alt+Z and drag a box around any part of the screen. Ctrl+Shift+R takes the whole screen.
- Ctrl+V pastes an image, or text, straight from the clipboard.
- Ctrl+O opens a file, and dragging one onto the window works too.
- Right-click a picture in File Explorer and open it with Glyfo, or send it through the Windows
  share sheet from Photos, Snipping Tool or a browser.

**What you get back**
- The recognized text next to the image, in the order it was laid out.
- Copy with one click, or let a capture copy itself the moment it finishes.
- Read aloud, using any voice installed on your PC.
- Unwrap merges hard line breaks back into paragraphs. No spaces strips every space, which is what
  Chinese, Japanese and Korean text needs after recognition.
- QR codes and barcodes decoded from the same image.
- A history of recent results, so a capture you took two minutes ago is still one click away.

**Choosing how it reads**
- Pick the recognition language from the packs Windows has installed, or let Glyfo choose.
- An option repairs the classic OCR mistake of reading v1.6.5 as vl.6.5 — an l or an I becomes a 1
  only where a separator and a digit sit beside it, so html5 and IPv6 are left alone.
- Zoom to fit, or view at actual pixel size, and recognize the whole image or just a selection.

**It stays out of the way**
- Closing the window leaves Glyfo in the notification area, so the capture shortcut keeps working.
  Exit from there closes it for good, and the whole behaviour is a setting you can turn off.
- It can start with Windows and go straight to the notification area without opening a window, so
  the shortcut works from the moment you sign in. Off by default; you turn it on.
- When the window is hidden, a notification shows the first line of what was just recognized.

**Languages**
The interface is available in 33 languages and follows your Windows language setting. Text
recognition uses the OCR language packs installed on your PC — add more under Settings › Time &
language › Language & region.

**Privacy**
Glyfo makes no network connections. Images, recognized text and translations never leave your PC.
No account, no telemetry, no advertising.

### 产品功能 — Product features

- Capture any part of the screen with Alt+Z and recognize it immediately, without leaving the app you were reading
- Runs on the OCR built into Windows, plus the on-device recognition model on Copilot+ PCs
- Open a file, paste from the clipboard, drag and drop, or receive a picture through the Windows share sheet
- Reads QR codes and barcodes out of the same image
- Translates on Copilot+ PCs, on the device, with no network access
- Reads the result aloud with any voice installed on your PC
- Unwrap line breaks and remove spaces — the cleanup that Chinese, Japanese and Korean text needs after recognition
- Lives in the notification area so the capture shortcut keeps working after you close the window
- Interface in 33 languages, following your Windows language setting
- Makes no internet connections: nothing you recognize ever leaves your PC

### 屏幕截图标题 — Screenshot captions

1. `01-text-from-a-page.png` — Every recognized line appears beside the picture in reading order, ready to copy, read aloud, or unwrap back into paragraphs.
2. `02-any-language.png` — Pick the recognition language from the packs Windows has installed — Chinese, Japanese and Korean included — or let Glyfo choose it.
3. `03-qr-and-barcodes.png` — QR codes and barcodes are read out of the same picture, so no separate scanner app is needed.
4. `04-history.png` — Recent results stay in the history, so something you captured a few minutes ago is still one click away.
5. `05-settings.png` — Interface language, starting with Windows, staying in the notification area, and the fix that keeps v1.6.5 from becoming vl.6.5.

### 搜索词 — Search terms

`screenshot to text`, `image to text`, `extract text`, `copy text from image`, `qr code reader`,
`barcode scanner`, `read aloud`

---

## 中文(简体) — Chinese (Simplified)

### 简短说明

Glyfo 把屏幕上看得见的字变成能用的字：截图、拍下来的书页、扫描件、幻灯片、视频画面都行。按 Alt+Z
框选屏幕上的任意一块，或者打开文件、粘贴剪贴板、从别的应用分享一张图过来，识别结果就出现在图片旁边，
可以复制、朗读、清理排版。同一张图里的二维码和条形码也会一并读出来；在 Copilot+ PC 上还能就地翻译。
全部在本机完成——Glyfo 不进行任何联网。免费，无广告，无内购。

### 说明

Glyfo 把图片里的文字，变成可以直接用的文字。

识别用的是 Windows 自带的 OCR，不需要注册账号，不上传，也不用等服务器。在 Copilot+ PC 上，Glyfo 还会
调用设备端的文字识别模型来处理更难的图，并且可以翻译识别结果——同样不联网。

**四种方式把图放进来**
- 按 Alt+Z 框选屏幕上的任意一块；Ctrl+Shift+R 直接截全屏。
- Ctrl+V 从剪贴板粘贴图片，粘文字也行。
- Ctrl+O 打开文件，把文件拖进窗口同样可以。
- 在文件资源管理器里右键图片，用 Glyfo 打开；或者从"照片""截图工具"、浏览器的分享面板送过来。

**能拿到什么**
- 识别出的文字就在图片旁边，按原来的版面顺序排列。
- 一键复制；也可以让截图识别完自动复制。
- 用本机安装的任意语音朗读。
- "去换行"把硬换行拼回段落，"去空格"清掉所有空格——中日韩文本识别完通常都需要这一步。
- 同一张图里的二维码和条形码一并解出来。
- 历史记录留着最近几次结果，两分钟前那次截图仍然一点就回来。

**识别方式可以调**
- 识别语言从 Windows 已装的语言包里选，也可以交给 Glyfo 自己判断。
- 有一个开关专治 OCR 把 v1.6.5 读成 vl.6.5 的老毛病：只有当 l 或 I 紧挨着分隔符和数字时才改成 1，
  html5 和 IPv6 不受影响。
- 适应窗口缩放，或按实际像素查看；可以识别整张图，也可以只识别框选的部分。

**不碍事**
- 关掉窗口后 Glyfo 留在通知区域，截图快捷键照常可用；在那里选"退出"才是真正关掉。这一整套行为本身
  也是个开关，不想要可以关掉。
- 可以开机自启并直接进通知区域、不弹窗口，登录后立刻就能用快捷键。默认关闭，由你打开。
- 窗口收起时，识别完会用通知显示第一行内容。

**语言**
界面有 33 种语言，跟随 Windows 的语言设置。文字识别用的是本机已安装的 OCR 语言包——可以在
"设置 › 时间和语言 › 语言和区域"里添加。

**隐私**
Glyfo 不进行任何网络连接。图片、识别出的文字、翻译结果都不会离开这台电脑。没有账号，没有遥测，
没有广告。

### 产品功能

- 按 Alt+Z 框选屏幕任意区域立即识别，不用离开正在看的那个应用
- 基于 Windows 自带 OCR；在 Copilot+ PC 上叠加设备端识别模型
- 打开文件、粘贴剪贴板、拖放，或者从 Windows 分享面板把图送进来
- 顺带读出同一张图里的二维码和条形码
- 在 Copilot+ PC 上就地翻译，全程不联网
- 用本机安装的任意语音朗读识别结果
- 去换行、去空格——中日韩文本识别之后正需要的两步清理
- 常驻通知区域，关掉窗口之后截图快捷键依然可用
- 界面 33 种语言，跟随 Windows 语言设置
- 不进行任何联网：识别的内容不会离开这台电脑

### 屏幕截图标题

1. `01-text-from-a-page.png` — 识别结果按原文顺序排在图片旁边，可以直接复制、朗读，或去掉换行还原成段落。
2. `02-any-language.png` — 识别语言从 Windows 已安装的语言包里选——中日韩都在其中——也可以交给 Glyfo 自动判断。
3. `03-qr-and-barcodes.png` — 二维码和条码从同一张图里一并读出，不用再装扫码工具。
4. `04-history.png` — 最近的识别结果留在历史里，几分钟前截的那一张仍然一键可达。
5. `05-settings.png` — 界面语言、开机自启、关闭后留在通知区域，以及那条让 v1.6.5 不被读成 vl.6.5 的修正。

### 搜索词

`截图取字`, `图片转文字`, `文字识别`, `二维码识别`, `条形码扫描`, `屏幕取词`, `免费OCR`

---

## 中文(繁體) — Chinese (Traditional)

### 簡短說明

Glyfo 把螢幕上看得到的字變成能用的字：擷圖、拍下來的書頁、掃描件、投影片、影片畫面都可以。按 Alt+Z
框選螢幕上任一塊，或是開啟檔案、貼上剪貼簿、從別的應用程式分享一張圖過來，辨識結果就出現在圖片旁邊，
可以複製、朗讀、整理排版。同一張圖裡的 QR Code 和條碼也會一併讀出來；在 Copilot+ PC 上還能就地翻譯。
全部在本機完成——Glyfo 不進行任何連網。免費，沒有廣告，沒有內購。

### 說明

Glyfo 把圖片裡的文字，變成可以直接使用的文字。

辨識用的是 Windows 內建的 OCR，不必註冊帳號、不上傳，也不用等伺服器。在 Copilot+ PC 上，Glyfo 還會
呼叫裝置端的文字辨識模型來處理比較難的圖，並且可以翻譯辨識結果——同樣不連網。

**四種方式把圖放進來**
- 按 Alt+Z 框選螢幕上任一塊；Ctrl+Shift+R 直接擷取全螢幕。
- Ctrl+V 從剪貼簿貼上圖片，貼文字也可以。
- Ctrl+O 開啟檔案，把檔案拖進視窗同樣可以。
- 在檔案總管裡對圖片按右鍵，用 Glyfo 開啟；或從「相片」「剪取工具」、瀏覽器的分享面板送過來。

**能拿到什麼**
- 辨識出的文字就在圖片旁邊，照原本的版面順序排列。
- 一鍵複製；也可以讓擷圖辨識完自動複製。
- 用本機安裝的任一語音朗讀。
- 「去換行」把硬換行接回段落，「去空格」清掉所有空格——中日韓文本辨識完通常都需要這一步。
- 同一張圖裡的 QR Code 和條碼一併解出來。
- 歷史紀錄留著最近幾次結果，兩分鐘前那次擷圖依然一點就回來。

**辨識方式可以調**
- 辨識語言從 Windows 已安裝的語言套件裡選，也可以交給 Glyfo 自行判斷。
- 有一個開關專門處理 OCR 把 v1.6.5 讀成 vl.6.5 的老問題：只有當 l 或 I 緊鄰分隔符號與數字時才改成
  1，html5 和 IPv6 不受影響。
- 縮放至符合視窗，或依實際像素檢視；可以辨識整張圖，也可以只辨識框選的部分。

**不礙事**
- 關掉視窗後 Glyfo 留在通知區域，擷圖快速鍵照常可用；在那裡選「結束」才是真正關掉。這一整套行為
  本身也是個開關，不想要可以關掉。
- 可以開機自動啟動並直接進通知區域、不開視窗，登入後立刻就能用快速鍵。預設關閉，由你開啟。
- 視窗收起時，辨識完會用通知顯示第一行內容。

**語言**
介面有 33 種語言，跟隨 Windows 的語言設定。文字辨識使用本機已安裝的 OCR 語言套件——可以在
「設定 › 時間與語言 › 語言與地區」中新增。

**隱私**
Glyfo 不進行任何網路連線。圖片、辨識出的文字、翻譯結果都不會離開這台電腦。沒有帳號、沒有遙測、
沒有廣告。

### 產品功能

- 按 Alt+Z 框選螢幕任一區域立即辨識，不必離開正在看的那個應用程式
- 以 Windows 內建 OCR 為基礎；在 Copilot+ PC 上再加上裝置端辨識模型
- 開啟檔案、貼上剪貼簿、拖放，或從 Windows 分享面板把圖送進來
- 順帶讀出同一張圖裡的 QR Code 與條碼
- 在 Copilot+ PC 上就地翻譯，全程不連網
- 用本機安裝的任一語音朗讀辨識結果
- 去換行、去空格——中日韓文本辨識之後正需要的兩步整理
- 常駐通知區域，關掉視窗之後擷圖快速鍵依然可用
- 介面 33 種語言，跟隨 Windows 語言設定
- 不進行任何連網：辨識的內容不會離開這台電腦

### 螢幕擷取畫面標題

1. `01-text-from-a-page.png` — 辨識結果依原文順序排在圖片旁邊，可以直接複製、朗讀，或去掉換行還原成段落。
2. `02-any-language.png` — 辨識語言從 Windows 已安裝的語言套件裡挑——中日韓都在其中——也可以交給 Glyfo 自動判斷。
3. `03-qr-and-barcodes.png` — QR 碼和條碼從同一張圖裡一併讀出，不必再裝掃碼工具。
4. `04-history.png` — 最近的辨識結果留在歷程記錄裡，幾分鐘前擷取的那一張仍然一鍵可達。
5. `05-settings.png` — 介面語言、開機自動啟動、關閉後留在通知區域，以及那條讓 v1.6.5 不被讀成 vl.6.5 的修正。

### 搜尋詞

`螢幕擷取文字`, `圖片轉文字`, `文字辨識`, `QR Code 掃描`, `條碼掃描`, `螢幕取字`, `免費OCR`

---

## 日本語 — Japanese

### 簡単な説明

Glyfo は、画面に映っているものを使えるテキストに変えます。スクリーンショット、紙面の写真、スキャン、
スライド、動画のコマ。Alt+Z を押して画面の一部を四角く囲むだけ。ファイルを開く、クリップボードから
貼り付ける、ほかのアプリから共有で送る、どれでも構いません。認識したテキストは画像の隣に出て、コピー、
読み上げ、整形がすぐできます。同じ画像から QR コードとバーコードも読み取り、Copilot+ PC では翻訳も
その場で。すべて PC 内で完結し、Glyfo はインターネットに一切接続しません。無料、広告なし、課金なし。

### 説明

Glyfo は、文字が写った画像を、そのまま使えるテキストに変えます。

認識には Windows に組み込まれた OCR を使います。アカウント登録も、アップロードも、サーバー待ちも
ありません。Copilot+ PC では、読み取りの難しい画像にデバイス上の文字認識モデルを併用し、結果を翻訳
することもできます——こちらもオフラインのままです。

**画像を渡す 4 つの方法**
- Alt+Z で画面の好きな範囲を囲む。Ctrl+Shift+R なら画面全体。
- Ctrl+V でクリップボードから画像を貼り付け。テキストも貼れます。
- Ctrl+O でファイルを開く。ウィンドウにドラッグしても同じです。
- エクスプローラーで画像を右クリックして Glyfo で開く。フォト、Snipping Tool、ブラウザーの共有
  メニューからでも送れます。

**戻ってくるもの**
- 認識されたテキストが画像の隣に、元のレイアウトの順序で並びます。
- ワンクリックでコピー。取り込みが終わった時点で自動コピーさせることもできます。
- PC にインストールされている任意の音声で読み上げ。
- 「改行を除去」は強制改行を段落に戻し、「スペースを除去」はすべての空白を削除します。日本語・
  中国語・韓国語の認識結果には、たいていこの一手間が要ります。
- 同じ画像の QR コードとバーコードもデコード。
- 履歴に直近の結果が残るので、2 分前の取り込みもワンクリックで戻せます。

**読み取り方を選べます**
- 認識言語は Windows にインストール済みの言語パックから選択。自動に任せることもできます。
- OCR が v1.6.5 を vl.6.5 と読む昔からの誤りに対処するオプションがあります。区切り文字と数字が隣に
  ある場所でだけ l や I を 1 に直すので、html5 や IPv6 はそのままです。
- ウィンドウに合わせる表示と等倍表示。画像全体でも、選択した部分だけでも認識できます。

**邪魔になりません**
- ウィンドウを閉じても Glyfo は通知領域に残り、取り込みのショートカットは効いたままです。そこから
  「終了」を選ぶと完全に終了します。この動作自体もオン/オフできます。
- Windows と一緒に起動し、ウィンドウを開かずそのまま通知領域に入ることもできます。サインインした
  瞬間からショートカットが使えます。既定はオフで、必要なときに自分でオンにします。
- ウィンドウが隠れているときは、認識が終わると通知に先頭の 1 行が表示されます。

**言語**
インターフェイスは 33 言語。Windows の言語設定に従います。文字認識は PC にインストールされた OCR
言語パックを使います——[設定 › 時刻と言語 › 言語と地域] から追加できます。

**プライバシー**
Glyfo はネットワークに接続しません。画像、認識したテキスト、翻訳結果が PC の外に出ることはありま
せん。アカウントなし、テレメトリなし、広告なし。

### 製品の機能

- Alt+Z で画面の任意の範囲を囲んですぐ認識。読んでいたアプリから離れる必要がありません
- Windows 内蔵の OCR を使用。Copilot+ PC ではデバイス上の認識モデルも併用します
- ファイルを開く、クリップボードから貼り付け、ドラッグ＆ドロップ、Windows の共有メニューから受け取る
- 同じ画像から QR コードとバーコードも読み取ります
- Copilot+ PC ではデバイス上で翻訳。ネットワークは使いません
- PC にインストールされている任意の音声で読み上げ
- 改行の除去とスペースの除去。日本語・中国語・韓国語の認識結果に必要な整形です
- 通知領域に常駐するので、ウィンドウを閉じても取り込みのショートカットは使えます
- インターフェイスは 33 言語、Windows の言語設定に追従します
- インターネットに一切接続しません。認識した内容が PC の外に出ることはありません

### スクリーンショットのキャプション

1. `01-text-from-a-page.png` — 認識した文字は読み取った順に画像の横へ。そのままコピー、読み上げ、改行をほどいて段落に戻すこともできます。
2. `02-any-language.png` — 認識する言語は Windows に入っている言語パックから選べます。日本語・中国語・韓国語も含まれ、自動選択も可能です。
3. `03-qr-and-barcodes.png` — QR コードとバーコードは同じ画像からまとめて読み取ります。別途スキャナーアプリは要りません。
4. `04-history.png` — 直近の結果は履歴に残るので、数分前に取り込んだものにもワンクリックで戻れます。
5. `05-settings.png` — 表示言語、Windows と同時に起動、閉じても通知領域に常駐、そして v1.6.5 が vl.6.5 にならないための補正。

### 検索キーワード

`画像から文字`, `スクショ 文字起こし`, `文字認識`, `QRコード 読み取り`, `バーコード読み取り`,
`画面キャプチャ`, `無料OCR`

---

## 한국어 — Korean

### 간단한 설명

Glyfo는 화면에 보이는 것을 쓸 수 있는 텍스트로 바꿉니다. 스크린샷, 책장을 찍은 사진, 스캔 문서,
슬라이드, 동영상 한 장면까지. Alt+Z를 누르고 화면의 원하는 부분을 사각형으로 감싸면 됩니다. 파일을
열거나, 클립보드에서 붙여넣거나, 다른 앱에서 공유로 보내도 됩니다. 인식된 텍스트는 이미지 옆에 나타나
바로 복사하고, 읽어주고, 정리할 수 있습니다. 같은 이미지의 QR 코드와 바코드도 함께 읽고, Copilot+
PC에서는 그 자리에서 번역합니다. 모두 PC 안에서 처리되며 Glyfo는 인터넷에 전혀 연결하지 않습니다.
무료이고 광고나 결제도 없습니다.

### 설명

Glyfo는 글자가 담긴 이미지를 바로 쓸 수 있는 텍스트로 바꿉니다.

인식에는 Windows에 내장된 OCR을 사용합니다. 계정을 만들 필요도, 업로드할 일도, 서버를 기다릴 일도
없습니다. Copilot+ PC에서는 어려운 이미지에 온디바이스 문자 인식 모델을 함께 사용하고, 결과를 번역할
수도 있습니다 — 이때도 네트워크는 쓰지 않습니다.

**이미지를 넣는 네 가지 방법**
- Alt+Z로 화면의 원하는 부분을 감싸세요. Ctrl+Shift+R은 전체 화면입니다.
- Ctrl+V로 클립보드의 이미지를 붙여넣습니다. 텍스트도 됩니다.
- Ctrl+O로 파일을 열고, 창으로 끌어다 놓아도 됩니다.
- 파일 탐색기에서 이미지를 마우스 오른쪽 버튼으로 눌러 Glyfo로 열거나, 사진·캡처 도구·브라우저의
  공유 메뉴에서 보내세요.

**결과로 얻는 것**
- 인식된 텍스트가 이미지 옆에, 원래 배치 순서대로 놓입니다.
- 한 번 클릭으로 복사하거나, 캡처가 끝나는 즉시 자동으로 복사되게 할 수 있습니다.
- PC에 설치된 아무 음성으로나 소리 내어 읽어줍니다.
- '줄바꿈 제거'는 강제 줄바꿈을 문단으로 되돌리고, '공백 제거'는 모든 공백을 없앱니다. 한국어·중국어·
  일본어 인식 결과에는 대개 이 과정이 필요합니다.
- 같은 이미지에서 QR 코드와 바코드도 해독합니다.
- 최근 결과가 기록에 남아, 2분 전 캡처도 클릭 한 번이면 돌아옵니다.

**읽는 방식을 고를 수 있습니다**
- 인식 언어는 Windows에 설치된 언어 팩에서 고르거나 자동에 맡깁니다.
- OCR이 v1.6.5를 vl.6.5로 읽는 오래된 문제를 잡는 옵션이 있습니다. 구분 기호와 숫자가 바로 옆에 있을
  때만 l이나 I를 1로 바꾸므로 html5나 IPv6는 그대로 둡니다.
- 창에 맞추기와 실제 크기 보기. 이미지 전체를 인식할 수도, 선택한 부분만 인식할 수도 있습니다.

**방해하지 않습니다**
- 창을 닫아도 Glyfo는 알림 영역에 남아 캡처 단축키가 계속 동작합니다. 거기서 '끝내기'를 선택해야 완전히
  종료됩니다. 이 동작 자체도 끌 수 있는 설정입니다.
- Windows와 함께 시작해 창을 열지 않고 바로 알림 영역으로 들어가게 할 수 있습니다. 로그인하는 순간부터
  단축키가 동작합니다. 기본값은 꺼짐이며 직접 켜는 방식입니다.
- 창이 숨겨져 있을 때는 인식이 끝나면 알림에 첫 줄이 표시됩니다.

**언어**
인터페이스는 33개 언어이며 Windows 언어 설정을 따릅니다. 문자 인식은 PC에 설치된 OCR 언어 팩을
사용합니다 — 설정 › 시간 및 언어 › 언어 및 지역에서 추가할 수 있습니다.

**개인 정보**
Glyfo는 네트워크에 연결하지 않습니다. 이미지, 인식된 텍스트, 번역 결과가 PC를 벗어나지 않습니다.
계정도, 원격 분석도, 광고도 없습니다.

### 제품 기능

- Alt+Z로 화면의 어느 부분이든 감싸 즉시 인식합니다. 보고 있던 앱을 떠날 필요가 없습니다
- Windows에 내장된 OCR을 사용하고, Copilot+ PC에서는 온디바이스 인식 모델을 더합니다
- 파일 열기, 클립보드 붙여넣기, 끌어다 놓기, Windows 공유 메뉴로 받기
- 같은 이미지에서 QR 코드와 바코드도 읽습니다
- Copilot+ PC에서는 기기 안에서 번역하며 네트워크를 쓰지 않습니다
- PC에 설치된 아무 음성으로나 결과를 읽어줍니다
- 줄바꿈 제거와 공백 제거 — 한국어·중국어·일본어 인식 결과에 필요한 두 가지 정리
- 알림 영역에 머물러 창을 닫은 뒤에도 캡처 단축키가 동작합니다
- 33개 언어 인터페이스, Windows 언어 설정을 따릅니다
- 인터넷에 전혀 연결하지 않습니다. 인식한 내용은 PC를 벗어나지 않습니다

### 스크린샷 캡션

1. `01-text-from-a-page.png` — 인식한 글자는 읽은 순서대로 이미지 옆에 나타납니다. 그대로 복사하거나 소리 내어 읽거나 줄바꿈을 풀어 문단으로 되돌릴 수 있습니다.
2. `02-any-language.png` — 인식 언어는 Windows에 설치된 언어 팩에서 고릅니다. 한국어·중국어·일본어도 포함되며 자동 선택도 가능합니다.
3. `03-qr-and-barcodes.png` — QR 코드와 바코드를 같은 이미지에서 함께 읽어냅니다. 별도의 스캐너 앱이 필요 없습니다.
4. `04-history.png` — 최근 결과는 기록에 남아 있어 몇 분 전에 캡처한 것도 클릭 한 번이면 다시 꺼낼 수 있습니다.
5. `05-settings.png` — 인터페이스 언어, Windows 시작 시 실행, 닫아도 알림 영역에 유지, 그리고 v1.6.5가 vl.6.5로 읽히지 않게 하는 보정.

### 검색어

`이미지 텍스트 추출`, `화면 캡처 문자인식`, `문자 인식`, `QR 코드 스캔`, `바코드 스캔`,
`스크린샷 텍스트`, `무료 OCR`

---

## Deutsch — German

### Kurzbeschreibung

Glyfo holt Text aus allem heraus, was Sie sehen können: aus einem Screenshot, dem Foto einer
Buchseite, einem Scan, einer Folie, einem Videostandbild. Alt+Z drücken und einen Rahmen um einen
Teil des Bildschirms ziehen — oder eine Datei öffnen, aus der Zwischenablage einfügen, ein Bild aus
einer anderen App herüberschicken. Der erkannte Text steht neben dem Bild, bereit zum Kopieren,
Vorlesen und Aufräumen. QR-Codes und Barcodes liest Glyfo aus demselben Bild mit, und auf einem
Copilot+ PC übersetzt es das Ergebnis. Alles geschieht auf Ihrem PC — Glyfo baut überhaupt keine
Internetverbindung auf. Kostenlos, ohne Werbung, ohne Käufe.

### Beschreibung

Glyfo macht aus Bildern von Text wieder Text, mit dem Sie arbeiten können.

Die Erkennung läuft auf der OCR, die Windows mitbringt: kein Konto, kein Upload, kein Warten auf
einen Server. Auf einem Copilot+ PC nutzt Glyfo zusätzlich das Texterkennungsmodell auf dem Gerät
für schwierige Bilder und kann das Ergebnis übersetzen — ebenfalls ohne Netzverbindung.

**Vier Wege, ein Bild hineinzubekommen**
- Alt+Z drücken und einen Rahmen um einen beliebigen Teil des Bildschirms ziehen. Ctrl+Shift+R
  nimmt den ganzen Bildschirm.
- Ctrl+V fügt ein Bild aus der Zwischenablage ein, Text ebenso.
- Ctrl+O öffnet eine Datei; ins Fenster ziehen geht genauso.
- Im Explorer mit der rechten Maustaste auf ein Bild und mit Glyfo öffnen, oder über die
  Windows-Teilen-Funktion aus Fotos, dem Snipping Tool oder dem Browser schicken.

**Was zurückkommt**
- Der erkannte Text neben dem Bild, in der Reihenfolge des ursprünglichen Layouts.
- Kopieren mit einem Klick — oder eine Aufnahme kopiert sich selbst, sobald sie fertig ist.
- Vorlesen mit jeder Stimme, die auf Ihrem PC installiert ist.
- „Umbrüche entfernen" fügt harte Zeilenumbrüche wieder zu Absätzen zusammen, „Leerzeichen
  entfernen" streicht jedes Leerzeichen — was chinesischer, japanischer und koreanischer Text nach
  der Erkennung meist braucht.
- QR-Codes und Barcodes aus demselben Bild.
- Ein Verlauf der letzten Ergebnisse: die Aufnahme von vor zwei Minuten ist einen Klick entfernt.

**Sie bestimmen, wie gelesen wird**
- Erkennungssprache aus den in Windows installierten Sprachpaketen wählen oder Glyfo entscheiden
  lassen.
- Eine Option behebt den klassischen OCR-Fehler, v1.6.5 als vl.6.5 zu lesen: Ein l oder I wird nur
  dort zur 1, wo ein Trennzeichen und eine Ziffer daneben stehen — html5 und IPv6 bleiben unberührt.
- Ans Fenster anpassen oder in Originalgröße ansehen; das ganze Bild erkennen oder nur die Auswahl.

**Es steht nicht im Weg**
- Wird das Fenster geschlossen, bleibt Glyfo im Infobereich, und das Tastenkürzel funktioniert
  weiter. „Beenden" dort schließt es endgültig. Dieses Verhalten selbst ist abschaltbar.
- Glyfo kann mit Windows starten und ohne Fenster direkt in den Infobereich gehen, sodass das
  Kürzel ab der Anmeldung bereitsteht. Standardmäßig aus; Sie schalten es ein.
- Bei verstecktem Fenster zeigt eine Benachrichtigung die erste Zeile des Erkannten.

**Sprachen**
Die Oberfläche gibt es in 33 Sprachen und richtet sich nach Ihrer Windows-Spracheinstellung. Die
Texterkennung nutzt die auf dem PC installierten OCR-Sprachpakete — weitere fügen Sie unter
Einstellungen › Zeit und Sprache › Sprache und Region hinzu.

**Datenschutz**
Glyfo stellt keine Netzwerkverbindungen her. Bilder, erkannter Text und Übersetzungen verlassen
Ihren PC nicht. Kein Konto, keine Telemetrie, keine Werbung.

### Produktfunktionen

- Mit Alt+Z einen beliebigen Bildschirmausschnitt aufnehmen und sofort erkennen, ohne die App zu verlassen, in der Sie gerade lesen
- Nutzt die in Windows eingebaute OCR, auf Copilot+ PCs zusätzlich das Erkennungsmodell auf dem Gerät
- Datei öffnen, aus der Zwischenablage einfügen, per Drag-and-drop oder über die Windows-Teilen-Funktion
- Liest QR-Codes und Barcodes aus demselben Bild
- Übersetzt auf Copilot+ PCs direkt auf dem Gerät, ohne Netzzugriff
- Liest das Ergebnis mit jeder auf dem PC installierten Stimme vor
- Zeilenumbrüche und Leerzeichen entfernen — die Nachbearbeitung, die CJK-Text nach der Erkennung braucht
- Bleibt im Infobereich, damit das Tastenkürzel auch nach dem Schließen des Fensters funktioniert
- Oberfläche in 33 Sprachen, folgt der Windows-Spracheinstellung
- Baut keine Internetverbindung auf: Erkanntes verlässt Ihren PC nie

### Screenshot-Beschriftungen

1. `01-text-from-a-page.png` — Der erkannte Text steht neben dem Bild, in der Reihenfolge, in der er gelesen wurde – zum Kopieren, Vorlesen oder Zurückführen in Absätze.
2. `02-any-language.png` — Die Erkennungssprache wählen Sie aus den in Windows installierten Sprachpaketen – oder Glyfo entscheidet selbst.
3. `03-qr-and-barcodes.png` — QR-Codes und Barcodes werden aus demselben Bild gelesen; eine separate Scanner-App ist nicht nötig.
4. `04-history.png` — Die letzten Ergebnisse bleiben im Verlauf, sodass eine Aufnahme von vor ein paar Minuten weiterhin einen Klick entfernt ist.
5. `05-settings.png` — Oberflächensprache, Start mit Windows, Verbleib im Infobereich und die Korrektur, die aus v1.6.5 kein vl.6.5 macht.

### Suchbegriffe

`Text aus Bild`, `Screenshot zu Text`, `Texterkennung`, `QR-Code lesen`, `Barcode scannen`,
`Bildschirmtext kopieren`, `Vorlesen`

---

## Français — French

### Brève description

Glyfo extrait le texte de tout ce que vous voyez : une capture d'écran, la photo d'une page, un
document scanné, une diapositive, une image de vidéo. Appuyez sur Alt+Z et encadrez une partie de
l'écran — ou ouvrez un fichier, collez depuis le presse-papiers, envoyez une image depuis une autre
application. Le texte reconnu apparaît à côté de l'image, prêt à être copié, lu à voix haute ou mis
au propre. Glyfo lit aussi les QR codes et les codes-barres de la même image, et sur un PC Copilot+
il traduit le résultat. Tout se passe sur votre PC : Glyfo n'établit aucune connexion Internet.
Gratuit, sans publicité et sans achat.

### Description

Glyfo transforme les images de texte en texte utilisable.

La reconnaissance s'appuie sur l'OCR intégré à Windows : aucun compte à créer, aucun envoi, aucune
attente côté serveur. Sur un PC Copilot+, Glyfo utilise en plus le modèle de reconnaissance de texte
embarqué pour les images difficiles et peut traduire le résultat — toujours sans passer par le
réseau.

**Quatre façons d'amener une image**
- Alt+Z pour encadrer n'importe quelle partie de l'écran. Ctrl+Shift+R prend l'écran entier.
- Ctrl+V colle une image depuis le presse-papiers, du texte également.
- Ctrl+O ouvre un fichier ; le glisser dans la fenêtre fonctionne aussi.
- Clic droit sur une image dans l'Explorateur pour l'ouvrir avec Glyfo, ou envoi depuis le volet de
  partage de Windows (Photos, Outil Capture d'écran, navigateur).

**Ce que vous récupérez**
- Le texte reconnu à côté de l'image, dans l'ordre de la mise en page d'origine.
- Copie en un clic, ou copie automatique dès qu'une capture se termine.
- Lecture à voix haute avec n'importe quelle voix installée sur votre PC.
- « Supprimer les retours » recolle les retours à la ligne forcés en paragraphes ; « Supprimer les
  espaces » retire tous les espaces, ce dont le texte chinois, japonais et coréen a besoin après
  reconnaissance.
- Les QR codes et codes-barres de la même image.
- Un historique des résultats récents : la capture d'il y a deux minutes reste à un clic.

**Vous choisissez comment il lit**
- Langue de reconnaissance parmi les modules linguistiques installés dans Windows, ou choix
  automatique.
- Une option corrige l'erreur classique consistant à lire v1.6.5 comme vl.6.5 : un l ou un I ne
  devient un 1 que là où un séparateur et un chiffre se trouvent à côté, si bien que html5 et IPv6
  restent intacts.
- Ajuster à la fenêtre ou afficher à la taille réelle ; reconnaître toute l'image ou seulement la
  sélection.

**Il ne gêne pas**
- Fermer la fenêtre laisse Glyfo dans la zone de notification, et le raccourci de capture continue
  de fonctionner. « Quitter » l'arrête pour de bon. Ce comportement est lui-même une option.
- Glyfo peut démarrer avec Windows et rejoindre directement la zone de notification sans ouvrir de
  fenêtre : le raccourci est actif dès l'ouverture de session. Désactivé par défaut, c'est vous qui
  l'activez.
- Fenêtre masquée, une notification affiche la première ligne de ce qui vient d'être reconnu.

**Langues**
L'interface existe en 33 langues et suit le réglage de langue de Windows. La reconnaissance utilise
les modules OCR installés sur le PC — vous en ajoutez dans Paramètres › Heure et langue › Langue et
région.

**Confidentialité**
Glyfo n'établit aucune connexion réseau. Les images, le texte reconnu et les traductions ne quittent
jamais votre PC. Pas de compte, pas de télémétrie, pas de publicité.

### Fonctionnalités du produit

- Capturez n'importe quelle partie de l'écran avec Alt+Z et reconnaissez-la aussitôt, sans quitter l'application que vous lisiez
- Repose sur l'OCR intégré à Windows, complété par le modèle embarqué sur les PC Copilot+
- Ouvrir un fichier, coller depuis le presse-papiers, glisser-déposer ou recevoir via le partage Windows
- Lit les QR codes et les codes-barres de la même image
- Traduit sur les PC Copilot+, sur l'appareil, sans accès réseau
- Lit le résultat à voix haute avec n'importe quelle voix installée sur le PC
- Supprimer les retours à la ligne et les espaces : la remise au propre dont le texte CJC a besoin
- Reste dans la zone de notification pour que le raccourci fonctionne après la fermeture de la fenêtre
- Interface en 33 langues, alignée sur le réglage de langue de Windows
- N'établit aucune connexion Internet : ce que vous reconnaissez ne quitte pas votre PC

### Légendes des captures d'écran

1. `01-text-from-a-page.png` — Le texte reconnu s'affiche à côté de l'image, dans l'ordre où il a été lu : à copier, à faire lire à voix haute ou à remettre en paragraphes.
2. `02-any-language.png` — La langue de reconnaissance se choisit parmi les modules linguistiques installés dans Windows — ou Glyfo la détermine seul.
3. `03-qr-and-barcodes.png` — Les QR codes et les codes-barres sont lus dans la même image : aucune application de scan supplémentaire n'est nécessaire.
4. `04-history.png` — Les résultats récents restent dans l'historique ; une capture faite il y a quelques minutes reste à un clic.
5. `05-settings.png` — Langue de l'interface, démarrage avec Windows, maintien dans la zone de notification, et la correction qui évite que v1.6.5 devienne vl.6.5.

### Termes de recherche

`texte depuis image`, `capture écran texte`, `reconnaissance texte`, `lire QR code`,
`scanner code-barres`, `extraire texte`, `lecture à voix haute`

---

## Español — Spanish

### Descripción breve

Glyfo extrae el texto de todo lo que puedas ver: una captura de pantalla, la foto de una página, un
documento escaneado, una diapositiva, un fotograma. Pulsa Alt+Z y encuadra una parte de la pantalla,
o abre un archivo, pega desde el portapapeles o envía una imagen desde otra aplicación. El texto
reconocido aparece junto a la imagen, listo para copiar, escuchar o limpiar. También lee códigos QR
y de barras de esa misma imagen, y en un PC Copilot+ traduce el resultado. Todo ocurre en tu PC:
Glyfo no establece ninguna conexión a Internet. Gratis, sin anuncios y sin compras.

### Descripción

Glyfo convierte las imágenes con texto en texto que puedes usar.

El reconocimiento se apoya en el OCR que trae Windows: sin cuenta, sin subir nada y sin esperar a un
servidor. En un PC Copilot+, Glyfo suma el modelo de reconocimiento de texto del propio dispositivo
para las imágenes difíciles y puede traducir el resultado, también sin conexión.

**Cuatro formas de aportar una imagen**
- Alt+Z para encuadrar cualquier zona de la pantalla. Ctrl+Shift+R toma la pantalla completa.
- Ctrl+V pega una imagen desde el portapapeles, y también texto.
- Ctrl+O abre un archivo; arrastrarlo a la ventana funciona igual.
- Clic derecho sobre una imagen en el Explorador para abrirla con Glyfo, o envíala desde el panel de
  uso compartido de Windows (Fotos, Recorte, el navegador).

**Lo que obtienes**
- El texto reconocido junto a la imagen, en el orden en que estaba dispuesto.
- Copiar con un clic, o dejar que una captura se copie sola en cuanto termina.
- Lectura en voz alta con cualquier voz instalada en el PC.
- «Quitar saltos» une los saltos de línea forzados en párrafos y «Quitar espacios» elimina todos los
  espacios, que es lo que suele necesitar el texto chino, japonés y coreano tras el reconocimiento.
- Códigos QR y de barras de la misma imagen.
- Un historial de resultados recientes: la captura de hace dos minutos sigue a un clic.

**Tú decides cómo lee**
- Elige el idioma de reconocimiento entre los paquetes instalados en Windows, o deja que Glyfo lo
  decida.
- Una opción corrige el error clásico de leer v1.6.5 como vl.6.5: una l o una I pasa a ser 1 solo
  donde hay un separador y un dígito al lado, así que html5 e IPv6 quedan intactos.
- Ajustar a la ventana o ver a tamaño real; reconocer la imagen entera o solo la selección.

**No estorba**
- Al cerrar la ventana, Glyfo se queda en el área de notificación y el atajo de captura sigue
  funcionando. «Salir» lo cierra del todo. Ese comportamiento es, a su vez, una opción.
- Puede iniciarse con Windows y pasar directamente al área de notificación sin abrir ventana, de
  modo que el atajo funciona desde que inicias sesión. Desactivado de fábrica; lo activas tú.
- Con la ventana oculta, una notificación muestra la primera línea de lo reconocido.

**Idiomas**
La interfaz está en 33 idiomas y sigue la configuración de idioma de Windows. El reconocimiento usa
los paquetes de OCR instalados en el PC: se añaden en Configuración › Hora e idioma › Idioma y
región.

**Privacidad**
Glyfo no establece conexiones de red. Las imágenes, el texto reconocido y las traducciones nunca
salen de tu PC. Sin cuenta, sin telemetría y sin publicidad.

### Características del producto

- Captura cualquier parte de la pantalla con Alt+Z y reconócela al instante, sin salir de la aplicación que estabas leyendo
- Funciona sobre el OCR integrado en Windows y, en PC Copilot+, sobre el modelo del propio dispositivo
- Abrir un archivo, pegar del portapapeles, arrastrar y soltar o recibir por el panel de uso compartido
- Lee códigos QR y de barras de la misma imagen
- Traduce en los PC Copilot+, en el dispositivo y sin acceso a la red
- Lee el resultado en voz alta con cualquier voz instalada en el PC
- Quitar saltos de línea y espacios: la limpieza que el texto CJK necesita tras el reconocimiento
- Se queda en el área de notificación para que el atajo siga activo tras cerrar la ventana
- Interfaz en 33 idiomas, según la configuración de idioma de Windows
- No se conecta a Internet: lo que reconoces no sale de tu PC

### Leyendas de las capturas de pantalla

1. `01-text-from-a-page.png` — El texto reconocido aparece junto a la imagen, en el orden en que se leyó: listo para copiar, escuchar en voz alta o volver a unir en párrafos.
2. `02-any-language.png` — El idioma de reconocimiento se elige entre los paquetes que Windows tenga instalados, o lo decide Glyfo por su cuenta.
3. `03-qr-and-barcodes.png` — Los códigos QR y de barras se leen de la misma imagen; no hace falta otra aplicación para escanear.
4. `04-history.png` — Los resultados recientes quedan en el historial, así que una captura de hace unos minutos sigue a un clic de distancia.
5. `05-settings.png` — Idioma de la interfaz, inicio con Windows, permanencia en el área de notificación y la corrección que evita que v1.6.5 se lea vl.6.5.

### Términos de búsqueda

`texto desde imagen`, `captura a texto`, `reconocimiento de texto`, `leer código QR`,
`escanear código de barras`, `extraer texto`, `leer en voz alta`

---

## Português (Brasil) — Portuguese (Brazil)

### Descrição breve

O Glyfo extrai texto de tudo o que você consegue ver: uma captura de tela, a foto de uma página, um
documento digitalizado, um slide, um quadro de vídeo. Pressione Alt+Z e enquadre um pedaço da tela,
ou abra um arquivo, cole da área de transferência, envie uma imagem de outro aplicativo. O texto
reconhecido aparece ao lado da imagem, pronto para copiar, ouvir ou limpar. Ele também lê QR codes e
códigos de barras da mesma imagem e, em um PC Copilot+, traduz o resultado. Tudo acontece no seu PC:
o Glyfo não faz nenhuma conexão com a Internet. Gratuito, sem anúncios e sem compras.

### Descrição

O Glyfo transforma imagens de texto em texto que dá para usar.

O reconhecimento roda sobre o OCR que já vem no Windows: sem conta, sem upload e sem esperar
servidor. Em um PC Copilot+, o Glyfo ainda usa o modelo de reconhecimento de texto do próprio
dispositivo nas imagens mais difíceis e pode traduzir o resultado — também sem rede.

**Quatro jeitos de trazer uma imagem**
- Alt+Z para enquadrar qualquer parte da tela. Ctrl+Shift+R pega a tela inteira.
- Ctrl+V cola uma imagem da área de transferência, e texto também.
- Ctrl+O abre um arquivo; arrastar para a janela funciona igual.
- Clique com o botão direito em uma imagem no Explorador e abra com o Glyfo, ou mande pelo painel de
  compartilhamento do Windows (Fotos, Ferramenta de Captura, navegador).

**O que volta**
- O texto reconhecido ao lado da imagem, na ordem em que estava disposto.
- Copiar com um clique, ou deixar que a captura se copie sozinha assim que terminar.
- Leitura em voz alta com qualquer voz instalada no PC.
- "Remover quebras" junta as quebras de linha forçadas de volta em parágrafos e "Remover espaços"
  tira todos os espaços — o que o texto chinês, japonês e coreano costuma precisar depois do
  reconhecimento.
- QR codes e códigos de barras da mesma imagem.
- Um histórico dos resultados recentes: a captura de dois minutos atrás continua a um clique.

**Você decide como ele lê**
- Escolha o idioma de reconhecimento entre os pacotes instalados no Windows, ou deixe automático.
- Uma opção corrige o erro clássico de ler v1.6.5 como vl.6.5: um l ou I vira 1 apenas onde há um
  separador e um dígito ao lado, então html5 e IPv6 ficam intactos.
- Ajustar à janela ou ver em tamanho real; reconhecer a imagem inteira ou só a seleção.

**Ele não atrapalha**
- Fechar a janela deixa o Glyfo na área de notificação, e o atalho de captura continua funcionando.
  "Sair" ali encerra de vez. Esse comportamento é, ele próprio, uma opção.
- Pode iniciar com o Windows e ir direto para a área de notificação sem abrir janela, de modo que o
  atalho funciona desde o login. Vem desligado; você que liga.
- Com a janela oculta, uma notificação mostra a primeira linha do que acabou de ser reconhecido.

**Idiomas**
A interface está em 33 idiomas e segue a configuração de idioma do Windows. O reconhecimento usa os
pacotes de OCR instalados no PC — dá para adicionar em Configurações › Hora e idioma › Idioma e
região.

**Privacidade**
O Glyfo não faz conexões de rede. Imagens, texto reconhecido e traduções nunca saem do seu PC. Sem
conta, sem telemetria e sem publicidade.

### Recursos do produto

- Capture qualquer parte da tela com Alt+Z e reconheça na hora, sem sair do aplicativo que você estava lendo
- Roda sobre o OCR embutido no Windows e, em PCs Copilot+, sobre o modelo do próprio dispositivo
- Abrir arquivo, colar da área de transferência, arrastar e soltar ou receber pelo compartilhamento do Windows
- Lê QR codes e códigos de barras da mesma imagem
- Traduz em PCs Copilot+, no dispositivo, sem acesso à rede
- Lê o resultado em voz alta com qualquer voz instalada no PC
- Remover quebras de linha e espaços: a limpeza de que o texto CJK precisa após o reconhecimento
- Fica na área de notificação para o atalho continuar valendo depois que a janela é fechada
- Interface em 33 idiomas, seguindo a configuração de idioma do Windows
- Não se conecta à Internet: o que você reconhece não sai do seu PC

### Legendas das capturas de tela

1. `01-text-from-a-page.png` — O texto reconhecido aparece ao lado da imagem, na ordem em que foi lido: pronto para copiar, ouvir em voz alta ou juntar de novo em parágrafos.
2. `02-any-language.png` — O idioma de reconhecimento vem dos pacotes que o Windows tem instalados — ou o Glyfo escolhe sozinho.
3. `03-qr-and-barcodes.png` — Códigos QR e de barras são lidos da mesma imagem; não é preciso outro aplicativo para escanear.
4. `04-history.png` — Os resultados recentes ficam no histórico, então uma captura de alguns minutos atrás continua a um clique.
5. `05-settings.png` — Idioma da interface, iniciar com o Windows, continuar na área de notificação e a correção que impede v1.6.5 de virar vl.6.5.

### Termos de pesquisa

`texto de imagem`, `captura para texto`, `reconhecimento de texto`, `ler QR code`,
`escanear código de barras`, `extrair texto`, `ler em voz alta`

---

## Русский — Russian

### Краткое описание

Glyfo достаёт текст из всего, что видно на экране: из снимка экрана, фотографии страницы,
отсканированного документа, слайда, кадра видео. Нажмите Alt+Z и обведите часть экрана — либо
откройте файл, вставьте из буфера обмена, отправьте картинку из другого приложения. Распознанный
текст появляется рядом с изображением: копируйте, слушайте, приводите в порядок. QR-коды и штрихкоды
Glyfo читает из того же изображения, а на ПК Copilot+ ещё и переводит результат. Всё происходит на
вашем ПК — Glyfo не устанавливает ни одного сетевого соединения. Бесплатно, без рекламы и без
покупок.

### Описание

Glyfo превращает изображения с текстом в текст, с которым можно работать.

Распознавание работает на OCR, встроенном в Windows: не нужны учётная запись, загрузка на сервер и
ожидание ответа. На ПК Copilot+ Glyfo дополнительно использует модель распознавания текста на самом
устройстве для сложных изображений и может перевести результат — тоже без выхода в сеть.

**Четыре способа передать изображение**
- Alt+Z — обвести любую область экрана. Ctrl+Shift+R снимает экран целиком.
- Ctrl+V вставляет изображение из буфера обмена, текст тоже.
- Ctrl+O открывает файл; перетаскивание в окно работает так же.
- Правый щелчок по картинке в проводнике — открыть с помощью Glyfo, или отправить через панель
  «Поделиться» из «Фотографий», «Ножниц» или браузера.

**Что вы получаете**
- Распознанный текст рядом с изображением, в порядке исходной вёрстки.
- Копирование одним щелчком — или снимок копируется сам, как только распознавание закончилось.
- Чтение вслух любым голосом, установленным на вашем ПК.
- «Убрать переносы» собирает жёсткие переносы обратно в абзацы, «Убрать пробелы» удаляет все пробелы
  — именно это обычно требуется китайскому, японскому и корейскому тексту после распознавания.
- QR-коды и штрихкоды из того же изображения.
- Журнал последних результатов: снимок двухминутной давности по-прежнему в одном щелчке.

**Вы задаёте, как читать**
- Язык распознавания выбирается из языковых пакетов, установленных в Windows, либо определяется
  автоматически.
- Отдельная настройка исправляет классическую ошибку, когда v1.6.5 читается как vl.6.5: l или I
  становится единицей только там, где рядом стоят разделитель и цифра, так что html5 и IPv6
  остаются нетронутыми.
- Вписать в окно или показать в натуральную величину; распознать всё изображение или только
  выделенное.

**Не мешает работать**
- Закрытие окна оставляет Glyfo в области уведомлений, и сочетание клавиш продолжает работать.
  «Выход» оттуда завершает программу окончательно. Само это поведение — тоже переключатель.
- Glyfo может запускаться вместе с Windows и сразу уходить в область уведомлений, не открывая окна,
  так что сочетание клавиш доступно сразу после входа в систему. По умолчанию выключено — включаете
  вы.
- Когда окно скрыто, уведомление показывает первую строку только что распознанного текста.

**Языки**
Интерфейс доступен на 33 языках и следует языковым настройкам Windows. Распознавание использует
языковые пакеты OCR, установленные на ПК, — добавить их можно в разделе «Параметры › Время и язык ›
Язык и регион».

**Конфиденциальность**
Glyfo не устанавливает сетевых соединений. Изображения, распознанный текст и переводы никогда не
покидают ваш ПК. Ни учётной записи, ни телеметрии, ни рекламы.

### Возможности продукта

- Снимите любую часть экрана нажатием Alt+Z и сразу распознайте её, не выходя из приложения, которое читали
- Работает на встроенном в Windows OCR, а на ПК Copilot+ — ещё и на модели распознавания в самом устройстве
- Открыть файл, вставить из буфера обмена, перетащить или получить через панель «Поделиться»
- Читает QR-коды и штрихкоды из того же изображения
- Переводит на ПК Copilot+ прямо на устройстве, без доступа к сети
- Читает результат вслух любым установленным на ПК голосом
- Убрать переносы и пробелы — та самая правка, которая нужна тексту CJK после распознавания
- Остаётся в области уведомлений, поэтому сочетание клавиш работает и после закрытия окна
- Интерфейс на 33 языках, следует языковым настройкам Windows
- Не выходит в интернет: распознанное не покидает ваш ПК

### Подписи к снимкам экрана

1. `01-text-from-a-page.png` — Распознанный текст стоит рядом с изображением в том порядке, в каком он был прочитан: копируйте, слушайте вслух или собирайте обратно в абзацы.
2. `02-any-language.png` — Язык распознавания выбирается из языковых пакетов, установленных в Windows, — или Glyfo определяет его сам.
3. `03-qr-and-barcodes.png` — QR-коды и штрихкоды считываются с того же изображения; отдельное приложение-сканер не нужно.
4. `04-history.png` — Недавние результаты остаются в журнале, поэтому снимок, сделанный несколько минут назад, по-прежнему в одном клике.
5. `05-settings.png` — Язык интерфейса, запуск вместе с Windows, работа в области уведомлений и исправление, из-за которого v1.6.5 не превращается в vl.6.5.

### Поисковые запросы

`текст с картинки`, `скриншот в текст`, `распознавание текста`, `сканер QR-кода`,
`сканер штрихкодов`, `извлечь текст`, `читать вслух`

---

## Italiano — Italian

### Descrizione breve

Glyfo estrae il testo da tutto ciò che vedi: uno screenshot, la foto di una pagina, un documento
scansionato, una diapositiva, un fotogramma di un video. Premi Alt+Z e traccia un riquadro su una
parte dello schermo, oppure apri un file, incolla dagli appunti, invia un'immagine da un'altra app.
Il testo riconosciuto compare accanto all'immagine, pronto da copiare, ascoltare o ripulire. Legge
anche i codici QR e i codici a barre dalla stessa immagine e, su un PC Copilot+, ne traduce il
risultato. Tutto avviene sul tuo PC: Glyfo non stabilisce alcuna connessione a Internet. Gratis,
senza pubblicità e senza acquisti.

### Descrizione

Glyfo trasforma le immagini di testo in testo che puoi usare.

Il riconoscimento si appoggia all'OCR incluso in Windows: nessun account da creare, nessun
caricamento, nessuna attesa di un server. Su un PC Copilot+, Glyfo usa in più il modello di
riconoscimento del testo sul dispositivo per le immagini difficili e può tradurre il risultato —
anche in questo caso senza rete.

**Quattro modi per far entrare un'immagine**
- Alt+Z traccia un riquadro su una parte qualsiasi dello schermo. Ctrl+Shift+R prende tutto lo
  schermo.
- Ctrl+V incolla un'immagine dagli appunti, e anche del testo.
- Ctrl+O apre un file; trascinarlo nella finestra funziona allo stesso modo.
- Clic destro su un'immagine in Esplora file per aprirla con Glyfo, oppure inviala dal riquadro di
  condivisione di Windows (Foto, Strumento di cattura, browser).

**Cosa ottieni**
- Il testo riconosciuto accanto all'immagine, nell'ordine in cui era disposto.
- Copia con un clic, oppure lascia che una cattura si copi da sola appena finisce.
- Lettura ad alta voce con qualsiasi voce installata sul PC.
- «Rimuovi a capo» ricompone le interruzioni di riga forzate in paragrafi, «Rimuovi spazi» elimina
  tutti gli spazi: è ciò di cui il testo cinese, giapponese e coreano ha bisogno dopo il
  riconoscimento.
- Codici QR e codici a barre dalla stessa immagine.
- Una cronologia dei risultati recenti: la cattura di due minuti fa è ancora a un clic.

**Decidi tu come legge**
- Scegli la lingua di riconoscimento tra i pacchetti installati in Windows, oppure lascia decidere a
  Glyfo.
- Un'opzione corregge il classico errore dell'OCR che legge v1.6.5 come vl.6.5: una l o una I
  diventa 1 solo dove accanto ci sono un separatore e una cifra, così html5 e IPv6 restano intatti.
- Adatta alla finestra o visualizza a dimensione reale; riconosci l'immagine intera o solo la
  selezione.

**Non sta tra i piedi**
- Chiudendo la finestra Glyfo resta nell'area di notifica e la scorciatoia di cattura continua a
  funzionare. «Esci» da lì lo chiude davvero. Questo comportamento è a sua volta un'impostazione.
- Può avviarsi con Windows e andare dritto nell'area di notifica senza aprire finestre, così la
  scorciatoia è pronta dal momento dell'accesso. Disattivato per impostazione predefinita: sei tu
  ad attivarlo.
- A finestra nascosta, una notifica mostra la prima riga di ciò che è stato appena riconosciuto.

**Lingue**
L'interfaccia è disponibile in 33 lingue e segue l'impostazione della lingua di Windows. Il
riconoscimento usa i pacchetti OCR installati sul PC: se ne aggiungono da Impostazioni › Data/ora e
lingua › Lingua e area geografica.

**Privacy**
Glyfo non stabilisce connessioni di rete. Immagini, testo riconosciuto e traduzioni non lasciano mai
il tuo PC. Nessun account, nessuna telemetria, nessuna pubblicità.

### Funzionalità del prodotto

- Cattura una parte qualsiasi dello schermo con Alt+Z e riconoscila subito, senza uscire dall'app che stavi leggendo
- Si basa sull'OCR integrato in Windows e, sui PC Copilot+, sul modello di riconoscimento sul dispositivo
- Apri un file, incolla dagli appunti, trascina e rilascia o ricevi dal riquadro di condivisione di Windows
- Legge codici QR e codici a barre dalla stessa immagine
- Traduce sui PC Copilot+, sul dispositivo, senza accesso alla rete
- Legge il risultato ad alta voce con qualsiasi voce installata sul PC
- Rimuovi gli a capo e gli spazi: la ripulitura di cui il testo CJK ha bisogno dopo il riconoscimento
- Resta nell'area di notifica, così la scorciatoia funziona anche dopo aver chiuso la finestra
- Interfaccia in 33 lingue, allineata all'impostazione della lingua di Windows
- Non si collega a Internet: ciò che riconosci non lascia il tuo PC

### Didascalie degli screenshot

1. `01-text-from-a-page.png` — Il testo riconosciuto compare accanto all'immagine, nell'ordine in cui è stato letto: pronto da copiare, ascoltare o ricomporre in paragrafi.
2. `02-any-language.png` — La lingua di riconoscimento si sceglie tra i pacchetti installati in Windows — oppure decide Glyfo da solo.
3. `03-qr-and-barcodes.png` — Codici QR e codici a barre vengono letti dalla stessa immagine: non serve un'altra app per la scansione.
4. `04-history.png` — I risultati recenti restano nella cronologia, così una cattura di qualche minuto fa è ancora a un clic.
5. `05-settings.png` — Lingua dell'interfaccia, avvio con Windows, permanenza nell'area di notifica e la correzione che evita che v1.6.5 diventi vl.6.5.

### Termini di ricerca

`testo da immagine`, `screenshot in testo`, `riconoscimento testo`, `leggere codice QR`,
`scanner codice a barre`, `estrarre testo`, `lettura ad alta voce`

---

## Polski — Polish

### Krótki opis

Glyfo wyciąga tekst ze wszystkiego, co widzisz: ze zrzutu ekranu, zdjęcia strony, skanu, slajdu,
klatki filmu. Naciśnij Alt+Z i zaznacz ramką fragment ekranu albo otwórz plik, wklej ze schowka,
prześlij obraz z innej aplikacji. Rozpoznany tekst pojawia się obok obrazu — gotowy do skopiowania,
odczytania na głos lub uporządkowania. Glyfo odczytuje też kody QR i kody kreskowe z tego samego
obrazu, a na komputerze Copilot+ tłumaczy wynik. Wszystko dzieje się na Twoim komputerze — Glyfo nie
nawiązuje żadnych połączeń internetowych. Bezpłatnie, bez reklam i bez zakupów.

### Opis

Glyfo zamienia obrazy z tekstem w tekst, którego można używać.

Rozpoznawanie działa na OCR wbudowanym w Windows: bez zakładania konta, bez wysyłania czegokolwiek i
bez czekania na serwer. Na komputerze Copilot+ Glyfo dodatkowo korzysta z modelu rozpoznawania
tekstu działającego na urządzeniu przy trudniejszych obrazach i potrafi przetłumaczyć wynik —
również bez sieci.

**Cztery sposoby na wczytanie obrazu**
- Alt+Z zaznacza ramką dowolny fragment ekranu. Ctrl+Shift+R robi zrzut całego ekranu.
- Ctrl+V wkleja obraz ze schowka, tekst również.
- Ctrl+O otwiera plik; przeciągnięcie go do okna działa tak samo.
- Kliknij obraz prawym przyciskiem w Eksploratorze plików i otwórz go w Glyfo albo prześlij przez
  panel udostępniania Windows (Zdjęcia, Narzędzie Wycinanie, przeglądarka).

**Co dostajesz**
- Rozpoznany tekst obok obrazu, w kolejności pierwotnego układu.
- Kopiowanie jednym kliknięciem albo automatyczne kopiowanie zaraz po zakończeniu zrzutu.
- Czytanie na głos dowolnym głosem zainstalowanym na komputerze.
- „Usuń podziały wierszy” skleja twarde złamania z powrotem w akapity, a „Usuń spacje” kasuje
  wszystkie spacje — tego zwykle wymaga tekst chiński, japoński i koreański po rozpoznaniu.
- Kody QR i kody kreskowe z tego samego obrazu.
- Historia ostatnich wyników: zrzut sprzed dwóch minut wciąż jest o jedno kliknięcie.

**To Ty decydujesz, jak czyta**
- Język rozpoznawania wybierasz spośród pakietów zainstalowanych w Windows albo zostawiasz decyzję
  Glyfo.
- Osobna opcja naprawia klasyczny błąd OCR, przez który v1.6.5 czytane jest jako vl.6.5: l lub I
  zamienia się w 1 tylko tam, gdzie obok stoi separator i cyfra, więc html5 i IPv6 pozostają
  nietknięte.
- Dopasowanie do okna albo podgląd w rzeczywistym rozmiarze; rozpoznawanie całego obrazu lub tylko
  zaznaczenia.

**Nie wchodzi w drogę**
- Po zamknięciu okna Glyfo zostaje w obszarze powiadomień, a skrót do zrzutu nadal działa. Dopiero
  „Zakończ” zamyka program na dobre. Samo to zachowanie też jest przełącznikiem.
- Glyfo może uruchamiać się razem z Windows i od razu przechodzić do obszaru powiadomień, bez
  otwierania okna — skrót działa od momentu zalogowania. Domyślnie wyłączone; włączasz je sam.
- Gdy okno jest ukryte, powiadomienie pokazuje pierwszy wiersz właśnie rozpoznanego tekstu.

**Języki**
Interfejs jest dostępny w 33 językach i podąża za ustawieniem języka Windows. Rozpoznawanie korzysta
z pakietów OCR zainstalowanych na komputerze — kolejne dodasz w Ustawienia › Czas i język › Język i
region.

**Prywatność**
Glyfo nie nawiązuje połączeń sieciowych. Obrazy, rozpoznany tekst i tłumaczenia nigdy nie opuszczają
Twojego komputera. Bez konta, bez telemetrii, bez reklam.

### Funkcje produktu

- Zaznacz dowolny fragment ekranu skrótem Alt+Z i rozpoznaj go od razu, nie wychodząc z aplikacji, którą właśnie czytasz
- Działa na OCR wbudowanym w Windows, a na komputerach Copilot+ dodatkowo na modelu rozpoznawania na urządzeniu
- Otwórz plik, wklej ze schowka, przeciągnij i upuść albo odbierz przez panel udostępniania Windows
- Odczytuje kody QR i kody kreskowe z tego samego obrazu
- Tłumaczy na komputerach Copilot+, na urządzeniu, bez dostępu do sieci
- Czyta wynik na głos dowolnym głosem zainstalowanym na komputerze
- Usuwanie podziałów wierszy i spacji — porządki, których tekst CJK wymaga po rozpoznaniu
- Zostaje w obszarze powiadomień, więc skrót działa także po zamknięciu okna
- Interfejs w 33 językach, zgodnie z ustawieniem języka Windows
- Nie łączy się z internetem: to, co rozpoznajesz, nie opuszcza Twojego komputera

### Podpisy zrzutów ekranu

1. `01-text-from-a-page.png` — Rozpoznany tekst stoi obok obrazu w kolejności, w jakiej został odczytany: do skopiowania, odczytania na głos albo złożenia z powrotem w akapity.
2. `02-any-language.png` — Język rozpoznawania wybierasz spośród pakietów zainstalowanych w Windows — albo Glyfo ustala go sam.
3. `03-qr-and-barcodes.png` — Kody QR i kreskowe są odczytywane z tego samego obrazu; osobna aplikacja do skanowania nie jest potrzebna.
4. `04-history.png` — Ostatnie wyniki zostają w historii, więc zrzut sprzed kilku minut wciąż jest o jedno kliknięcie.
5. `05-settings.png` — Język interfejsu, uruchamianie z Windows, pozostawanie w obszarze powiadomień i poprawka, dzięki której v1.6.5 nie staje się vl.6.5.

### Wyszukiwane hasła

`tekst ze zdjęcia`, `zrzut ekranu na tekst`, `rozpoznawanie tekstu`, `czytnik kodów QR`,
`skaner kodów kreskowych`, `wyodrębnij tekst`, `czytanie na głos`

---

## Nederlands — Dutch

### Korte beschrijving

Glyfo haalt tekst uit alles wat je kunt zien: een schermafbeelding, een foto van een bladzijde, een
scan, een dia, een videobeeld. Druk op Alt+Z en trek een kader om een deel van het scherm, of open
een bestand, plak vanaf het klembord, stuur een afbeelding vanuit een andere app. De herkende tekst
verschijnt naast de afbeelding, klaar om te kopiëren, voor te laten lezen of op te schonen. Glyfo
leest ook QR-codes en streepjescodes uit dezelfde afbeelding, en op een Copilot+ pc vertaalt het het
resultaat. Alles gebeurt op je eigen pc — Glyfo maakt geen enkele internetverbinding. Gratis, zonder
advertenties en zonder aankopen.

### Beschrijving

Glyfo maakt van afbeeldingen met tekst weer tekst waarmee je kunt werken.

De herkenning draait op de OCR die in Windows zit: geen account, geen upload, geen wachten op een
server. Op een Copilot+ pc gebruikt Glyfo daarnaast het tekstherkenningsmodel op het apparaat voor
lastigere afbeeldingen en kan het het resultaat vertalen — ook dat zonder netwerk.

**Vier manieren om een afbeelding binnen te krijgen**
- Alt+Z trekt een kader om een willekeurig deel van het scherm. Ctrl+Shift+R neemt het hele scherm.
- Ctrl+V plakt een afbeelding vanaf het klembord, tekst ook.
- Ctrl+O opent een bestand; het in het venster slepen werkt net zo goed.
- Klik met de rechtermuisknop op een afbeelding in Verkenner en open die met Glyfo, of stuur hem via
  het deelvenster van Windows (Foto's, Knipprogramma, browser).

**Wat je terugkrijgt**
- De herkende tekst naast de afbeelding, in de volgorde van de oorspronkelijke opmaak.
- Kopiëren met één klik, of een opname zichzelf laten kopiëren zodra hij klaar is.
- Voorlezen met elke stem die op je pc is geïnstalleerd.
- ‘Regeleinden verwijderen’ voegt harde regelovergangen weer samen tot alinea's en ‘Spaties
  verwijderen’ haalt elke spatie weg — precies wat Chinese, Japanse en Koreaanse tekst na herkenning
  nodig heeft.
- QR-codes en streepjescodes uit dezelfde afbeelding.
- Een geschiedenis van recente resultaten: de opname van twee minuten geleden is nog één klik weg.

**Jij bepaalt hoe het leest**
- Kies de herkenningstaal uit de taalpakketten die in Windows zijn geïnstalleerd, of laat Glyfo
  kiezen.
- Een optie herstelt de klassieke OCR-fout waarbij v1.6.5 als vl.6.5 wordt gelezen: een l of I wordt
  alleen een 1 waar er een scheidingsteken en een cijfer naast staan, dus html5 en IPv6 blijven
  ongemoeid.
- Passend maken aan het venster of op ware grootte bekijken; de hele afbeelding herkennen of alleen
  de selectie.

**Het zit niet in de weg**
- Het venster sluiten laat Glyfo in het systeemvak achter, zodat de sneltoets blijft werken.
  ‘Afsluiten’ daar stopt het echt. Dat gedrag is zelf ook een instelling.
- Glyfo kan met Windows meestarten en meteen naar het systeemvak gaan zonder venster, zodat de
  sneltoets werkt vanaf het moment dat je je aanmeldt. Standaard uit; jij zet het aan.
- Is het venster verborgen, dan toont een melding de eerste regel van wat er zojuist is herkend.

**Talen**
De interface is er in 33 talen en volgt je taalinstelling in Windows. De tekstherkenning gebruikt de
OCR-taalpakketten die op je pc staan — meer voeg je toe via Instellingen › Tijd en taal › Taal en
regio.

**Privacy**
Glyfo maakt geen netwerkverbindingen. Afbeeldingen, herkende tekst en vertalingen verlaten je pc
nooit. Geen account, geen telemetrie, geen advertenties.

### Productfuncties

- Leg met Alt+Z een willekeurig deel van het scherm vast en herken het meteen, zonder de app te verlaten waarin je aan het lezen was
- Draait op de OCR die in Windows is ingebouwd, op Copilot+ pc's aangevuld met het herkenningsmodel op het apparaat
- Een bestand openen, plakken vanaf het klembord, slepen en neerzetten of ontvangen via het deelvenster van Windows
- Leest QR-codes en streepjescodes uit dezelfde afbeelding
- Vertaalt op Copilot+ pc's, op het apparaat zelf, zonder netwerktoegang
- Leest het resultaat voor met elke stem die op de pc is geïnstalleerd
- Regeleinden en spaties verwijderen — het opschonen dat CJK-tekst na herkenning nodig heeft
- Blijft in het systeemvak, zodat de sneltoets ook na het sluiten van het venster werkt
- Interface in 33 talen, volgt de taalinstelling van Windows
- Maakt geen internetverbinding: wat je herkent verlaat je pc niet

### Bijschriften bij schermafbeeldingen

1. `01-text-from-a-page.png` — De herkende tekst staat naast de afbeelding, in de volgorde waarin hij gelezen is: klaar om te kopiëren, voor te laten lezen of terug te voegen tot alinea's.
2. `02-any-language.png` — De herkenningstaal kies je uit de taalpakketten die in Windows zijn geïnstalleerd — of Glyfo bepaalt hem zelf.
3. `03-qr-and-barcodes.png` — QR-codes en streepjescodes worden uit dezelfde afbeelding gelezen; een aparte scan-app is niet nodig.
4. `04-history.png` — Recente resultaten blijven in de geschiedenis staan, dus een opname van een paar minuten geleden is nog één klik weg.
5. `05-settings.png` — Interfacetaal, meestarten met Windows, in het systeemvak blijven, en de correctie die voorkomt dat v1.6.5 vl.6.5 wordt.

### Zoektermen

`tekst uit afbeelding`, `schermafbeelding naar tekst`, `tekstherkenning`, `QR-code lezen`,
`streepjescode scannen`, `tekst extraheren`, `voorlezen`

---

## Čeština — Czech

### Stručný popis

Glyfo vytáhne text ze všeho, co vidíte: ze snímku obrazovky, z fotky stránky, ze skenu, ze snímku
prezentace i z videa. Stiskněte Alt+Z a orámujte část obrazovky, nebo otevřete soubor, vložte ze
schránky, pošlete obrázek z jiné aplikace. Rozpoznaný text se objeví vedle obrázku — připravený ke
zkopírování, přečtení nahlas nebo úpravě. Glyfo přečte ze stejného obrázku i QR kódy a čárové kódy
a na počítači Copilot+ výsledek přeloží. Všechno probíhá ve vašem počítači — Glyfo se vůbec
nepřipojuje k internetu. Zdarma, bez reklam a bez nákupů.

### Popis

Glyfo mění obrázky s textem v text, se kterým se dá pracovat.

Rozpoznávání běží na OCR, které je součástí Windows: žádný účet, žádné nahrávání, žádné čekání na
server. Na počítači Copilot+ Glyfo u složitějších obrázků navíc využije model rozpoznávání textu
přímo v zařízení a dokáže výsledek přeložit — také bez sítě.

**Čtyři způsoby, jak dostat obrázek dovnitř**
- Alt+Z orámuje libovolnou část obrazovky. Ctrl+Shift+R sejme celou obrazovku.
- Ctrl+V vloží obrázek ze schránky, text také.
- Ctrl+O otevře soubor; přetažení do okna funguje stejně.
- Klepněte na obrázek v Průzkumníku pravým tlačítkem a otevřete ho v Glyfo, nebo ho pošlete přes
  panel sdílení Windows (Fotky, Nástroj pro vystřižení, prohlížeč).

**Co dostanete zpět**
- Rozpoznaný text vedle obrázku, v pořadí původního rozvržení.
- Kopírování jedním klepnutím, nebo ať se snímek zkopíruje sám, jakmile je hotový.
- Čtení nahlas libovolným hlasem nainstalovaným v počítači.
- „Odstranit zalomení“ spojí tvrdé konce řádků zpět do odstavců, „Odstranit mezery“ smaže všechny
  mezery — právě to čínský, japonský a korejský text po rozpoznání obvykle potřebuje.
- QR kódy a čárové kódy ze stejného obrázku.
- Historie posledních výsledků: snímek z doby před dvěma minutami je pořád jedno klepnutí daleko.

**Vy určujete, jak čte**
- Jazyk rozpoznávání vyberete z jazykových sad nainstalovaných ve Windows, nebo ho nechte na Glyfo.
- Volba opravuje klasickou chybu OCR, kdy se v1.6.5 čte jako vl.6.5: l nebo I se změní na 1 jen tam,
  kde vedle stojí oddělovač a číslice, takže html5 a IPv6 zůstanou nedotčené.
- Přizpůsobit oknu nebo zobrazit ve skutečné velikosti; rozpoznat celý obrázek, nebo jen výběr.

**Nepřekáží**
- Zavřením okna Glyfo zůstane v oznamovací oblasti a klávesová zkratka dál funguje. Teprve
  „Ukončit“ ho zavře nadobro. Celé toto chování je samo o sobě přepínač.
- Glyfo se může spouštět s Windows a jít rovnou do oznamovací oblasti bez okna, takže zkratka
  funguje od chvíle přihlášení. Ve výchozím stavu vypnuto; zapnete si ho sami.
- Když je okno skryté, oznámení ukáže první řádek právě rozpoznaného textu.

**Jazyky**
Rozhraní je k dispozici ve 33 jazycích a řídí se nastavením jazyka ve Windows. Rozpoznávání používá
jazykové sady OCR nainstalované v počítači — další přidáte v Nastavení › Čas a jazyk › Jazyk a
oblast.

**Soukromí**
Glyfo nenavazuje žádná síťová spojení. Obrázky, rozpoznaný text ani překlady nikdy neopustí váš
počítač. Žádný účet, žádná telemetrie, žádná reklama.

### Funkce produktu

- Sejměte klávesou Alt+Z libovolnou část obrazovky a hned ji rozpoznejte, aniž byste opustili aplikaci, kterou jste právě četli
- Staví na OCR vestavěném ve Windows, na počítačích Copilot+ navíc na modelu rozpoznávání v zařízení
- Otevřít soubor, vložit ze schránky, přetáhnout nebo přijmout přes panel sdílení Windows
- Přečte ze stejného obrázku QR kódy i čárové kódy
- Na počítačích Copilot+ překládá přímo v zařízení, bez přístupu k síti
- Přečte výsledek nahlas libovolným hlasem nainstalovaným v počítači
- Odstranění zalomení řádků a mezer — úklid, který text CJK po rozpoznání potřebuje
- Zůstává v oznamovací oblasti, takže zkratka funguje i po zavření okna
- Rozhraní ve 33 jazycích, podle nastavení jazyka ve Windows
- Nepřipojuje se k internetu: co rozpoznáte, neopustí váš počítač

### Popisky snímků obrazovky

1. `01-text-from-a-page.png` — Rozpoznaný text stojí vedle obrázku v pořadí, ve kterém byl přečten: ke zkopírování, přečtení nahlas nebo spojení zpět do odstavců.
2. `02-any-language.png` — Jazyk rozpoznávání vyberete z jazykových sad nainstalovaných ve Windows — nebo si ho Glyfo určí sám.
3. `03-qr-and-barcodes.png` — QR kódy a čárové kódy se čtou ze stejného obrázku; samostatná aplikace na skenování není potřeba.
4. `04-history.png` — Poslední výsledky zůstávají v historii, takže snímek z doby před pár minutami je pořád jedno klepnutí daleko.
5. `05-settings.png` — Jazyk rozhraní, spouštění s Windows, setrvání v oznamovací oblasti a oprava, díky které se z v1.6.5 nestane vl.6.5.

### Hledané výrazy

`text z obrázku`, `snímek obrazovky na text`, `rozpoznávání textu`, `čtečka QR kódů`,
`skener čárových kódů`, `extrahovat text`, `čtení nahlas`

---

## Türkçe — Turkish

### Kısa açıklama

Glyfo gördüğünüz her şeyden metni çıkarır: ekran görüntüsü, bir sayfanın fotoğrafı, taranmış belge,
sunu slaydı, video karesi. Alt+Z tuşuna basıp ekranın bir bölümünü çerçeveleyin; ya da dosya açın,
panodan yapıştırın, başka bir uygulamadan görsel gönderin. Tanınan metin görselin yanında belirir;
kopyalamaya, sesli okutmaya veya düzeltmeye hazırdır. Aynı görseldeki QR kodlarını ve barkodları da
okur, Copilot+ bilgisayarlarda sonucu çevirir. Her şey kendi bilgisayarınızda olur — Glyfo hiçbir
internet bağlantısı kurmaz. Ücretsiz, reklamsız ve satın alma içermez.

### Açıklama

Glyfo metin içeren görselleri kullanabileceğiniz metne dönüştürür.

Tanıma, Windows ile birlikte gelen OCR üzerinde çalışır: hesap açmak, dosya yüklemek ya da sunucu
beklemek yok. Copilot+ bilgisayarlarda Glyfo, zor görseller için cihaz üzerindeki metin tanıma
modelini de kullanır ve sonucu çevirebilir — bu da yine çevrimdışı gerçekleşir.

**Görseli içeri almanın dört yolu**
- Alt+Z ile ekranın herhangi bir bölümünü çerçeveleyin. Ctrl+Shift+R ekranın tamamını alır.
- Ctrl+V panodaki görseli yapıştırır, metni de.
- Ctrl+O bir dosya açar; pencereye sürüklemek de aynı işi görür.
- Dosya Gezgini'nde bir görsele sağ tıklayıp Glyfo ile açın ya da Windows paylaşım panelinden
  gönderin (Fotoğraflar, Ekran Alıntısı Aracı, tarayıcı).

**Elinize ne geçer**
- Tanınan metin görselin yanında, özgün yerleşim sırasıyla.
- Tek tıkla kopyalama ya da alıntı biter bitmez kendiliğinden kopyalanması.
- Bilgisayarınızda yüklü herhangi bir sesle sesli okuma.
- “Satır sonlarını kaldır” zorunlu satır sonlarını paragraflara geri birleştirir, “Boşlukları
  kaldır” tüm boşlukları siler — Çince, Japonca ve Korece metnin tanıma sonrasında genelde ihtiyaç
  duyduğu şey budur.
- Aynı görselden QR kodları ve barkodlar.
- Son sonuçların tutulduğu bir geçmiş: iki dakika önceki alıntı hâlâ bir tık uzağınızda.

**Nasıl okuyacağına siz karar verirsiniz**
- Tanıma dilini Windows'ta yüklü dil paketleri arasından seçin ya da kararı Glyfo'ya bırakın.
- Bir seçenek, OCR'nin v1.6.5'i vl.6.5 diye okuduğu klasik hatayı düzeltir: l ya da I yalnızca
  yanında bir ayırıcı ve bir rakam varken 1'e dönüşür, böylece html5 ve IPv6 olduğu gibi kalır.
- Pencereye sığdırın veya gerçek boyutta görün; görselin tamamını ya da yalnızca seçili bölümü
  tanıyın.

**Ayak altında dolaşmaz**
- Pencereyi kapattığınızda Glyfo bildirim alanında kalır ve alıntı kısayolu çalışmaya devam eder.
  Oradan “Çıkış” demek onu tamamen kapatır. Bu davranışın kendisi de bir ayardır.
- Glyfo Windows ile birlikte başlayıp pencere açmadan doğrudan bildirim alanına geçebilir; böylece
  kısayol oturum açtığınız andan itibaren hazırdır. Varsayılan olarak kapalıdır; açan siz olursunuz.
- Pencere gizliyken bir bildirim, yeni tanınan metnin ilk satırını gösterir.

**Diller**
Arayüz 33 dilde sunulur ve Windows dil ayarınızı izler. Metin tanıma, bilgisayarınızda yüklü OCR dil
paketlerini kullanır — yenilerini Ayarlar › Saat ve dil › Dil ve bölge altından eklersiniz.

**Gizlilik**
Glyfo ağ bağlantısı kurmaz. Görseller, tanınan metin ve çeviriler bilgisayarınızdan asla çıkmaz.
Hesap yok, telemetri yok, reklam yok.

### Ürün özellikleri

- Alt+Z ile ekranın herhangi bir bölümünü alın ve okumakta olduğunuz uygulamadan çıkmadan anında tanıyın
- Windows'un yerleşik OCR'si üzerinde çalışır, Copilot+ bilgisayarlarda cihaz üzerindeki tanıma modeliyle desteklenir
- Dosya açma, panodan yapıştırma, sürükle bırak ya da Windows paylaşım panelinden alma
- Aynı görselden QR kodlarını ve barkodları okur
- Copilot+ bilgisayarlarda cihaz üzerinde, ağ erişimi olmadan çeviri yapar
- Sonucu bilgisayarda yüklü herhangi bir sesle sesli okur
- Satır sonlarını ve boşlukları kaldırma — CJK metninin tanıma sonrasında ihtiyaç duyduğu düzeltme
- Bildirim alanında kalır, böylece pencere kapandıktan sonra da kısayol çalışır
- 33 dilde arayüz, Windows dil ayarını izler
- İnternete hiç bağlanmaz: tanıdığınız hiçbir şey bilgisayarınızdan çıkmaz

### Ekran görüntüsü açıklamaları

1. `01-text-from-a-page.png` — Tanınan metin, okunduğu sırayla görselin yanında durur: kopyalamaya, sesli dinlemeye ya da paragraflara geri birleştirmeye hazır.
2. `02-any-language.png` — Tanıma dili Windows'ta yüklü dil paketleri arasından seçilir — ya da Glyfo kendisi belirler.
3. `03-qr-and-barcodes.png` — QR kodları ve barkodlar aynı görselden okunur; ayrı bir tarayıcı uygulamasına gerek yoktur.
4. `04-history.png` — Son sonuçlar geçmişte kalır; birkaç dakika önce aldığınız bir alıntı hâlâ bir tık uzağınızdadır.
5. `05-settings.png` — Arayüz dili, Windows ile başlatma, kapatınca bildirim alanında kalma ve v1.6.5'in vl.6.5 olmasını engelleyen düzeltme.

### Arama terimleri

`görselden metin`, `ekran görüntüsü metne`, `metin tanıma`, `QR kod okuyucu`,
`barkod tarayıcı`, `metin çıkarma`, `sesli okuma`

---

## Svenska — Swedish

### Kort beskrivning

Glyfo plockar ut texten ur allt du kan se: en skärmbild, ett foto av en sida, en inskannad handling,
en presentationsbild, en filmruta. Tryck Alt+Z och rama in en del av skärmen, eller öppna en fil,
klistra in från Urklipp, skicka en bild från en annan app. Den avlästa texten hamnar bredvid bilden,
klar att kopiera, lyssna på eller städa upp. Glyfo läser också QR-koder och streckkoder ur samma
bild, och på en Copilot+-dator översätter den resultatet. Allt sker på din egen dator — Glyfo
upprättar inga internetanslutningar. Gratis, utan annonser och utan köp.

### Beskrivning

Glyfo gör om bilder med text till text du kan använda.

Avläsningen bygger på den OCR som redan finns i Windows: inget konto, ingen uppladdning, ingen
väntan på en server. På en Copilot+-dator använder Glyfo dessutom textigenkänningsmodellen på
enheten för svårare bilder och kan översätta resultatet — även det utan nätverk.

**Fyra sätt att få in en bild**
- Alt+Z ramar in vilken del av skärmen som helst. Ctrl+Shift+R tar hela skärmen.
- Ctrl+V klistrar in en bild från Urklipp, text också.
- Ctrl+O öppnar en fil; att dra in den i fönstret fungerar lika bra.
- Högerklicka en bild i Utforskaren och öppna den med Glyfo, eller skicka den via delningsfönstret i
  Windows (Foton, Skärmklipp, webbläsaren).

**Vad du får tillbaka**
- Den avlästa texten bredvid bilden, i den ordning den låg i.
- Kopiera med ett klick, eller låt en skärmbild kopiera sig själv så fort den är klar.
- Uppläsning med vilken röst som helst som är installerad på datorn.
- ”Ta bort radbrytningar” fogar ihop hårda radbrytningar till stycken igen och ”Ta bort blanksteg”
  tar bort varje mellanslag — precis vad kinesisk, japansk och koreansk text behöver efter
  avläsning.
- QR-koder och streckkoder ur samma bild.
- En historik över de senaste resultaten: skärmbilden från två minuter sedan är fortfarande ett
  klick bort.

**Du bestämmer hur den läser**
- Välj avläsningsspråk bland de språkpaket som är installerade i Windows, eller låt Glyfo välja.
- En inställning rättar det klassiska OCR-felet där v1.6.5 läses som vl.6.5: ett l eller I blir en
  1:a bara där det står en avgränsare och en siffra intill, så html5 och IPv6 lämnas i fred.
- Anpassa till fönstret eller visa i verklig storlek; läs av hela bilden eller bara markeringen.

**Den är inte i vägen**
- Stänger du fönstret ligger Glyfo kvar i meddelandefältet och kortkommandot fortsätter fungera.
  ”Avsluta” därifrån stänger den på riktigt. Beteendet är i sin tur en inställning.
- Glyfo kan starta med Windows och gå rakt ned i meddelandefältet utan att öppna något fönster, så
  kortkommandot fungerar från det att du loggar in. Avstängt som standard; du slår på det själv.
- När fönstret är dolt visar en avisering första raden av det som just lästes av.

**Språk**
Gränssnittet finns på 33 språk och följer språkinställningen i Windows. Avläsningen använder de
OCR-språkpaket som finns på datorn — fler lägger du till under Inställningar › Tid och språk › Språk
och region.

**Integritet**
Glyfo upprättar inga nätverksanslutningar. Bilder, avläst text och översättningar lämnar aldrig din
dator. Inget konto, ingen telemetri, inga annonser.

### Produktfunktioner

- Fånga vilken del av skärmen som helst med Alt+Z och läs av den direkt, utan att lämna appen du läste i
- Bygger på den OCR som finns i Windows, och på Copilot+-datorer även på igenkänningsmodellen på enheten
- Öppna en fil, klistra in från Urklipp, dra och släpp eller ta emot via delningsfönstret i Windows
- Läser QR-koder och streckkoder ur samma bild
- Översätter på Copilot+-datorer, på enheten, utan nätverksåtkomst
- Läser upp resultatet med vilken röst som helst som är installerad på datorn
- Ta bort radbrytningar och blanksteg — den städning som CJK-text behöver efter avläsning
- Ligger kvar i meddelandefältet, så kortkommandot fungerar även när fönstret är stängt
- Gränssnitt på 33 språk, följer språkinställningen i Windows
- Ansluter aldrig till internet: det du läser av lämnar inte din dator

### Bildtexter till skärmbilder

1. `01-text-from-a-page.png` — Den avlästa texten står bredvid bilden i den ordning den lästes: klar att kopiera, lyssna på eller foga ihop till stycken igen.
2. `02-any-language.png` — Avläsningsspråket väljer du bland språkpaketen som är installerade i Windows — eller så avgör Glyfo det själv.
3. `03-qr-and-barcodes.png` — QR-koder och streckkoder läses ur samma bild; någon separat skanningsapp behövs inte.
4. `04-history.png` — De senaste resultaten ligger kvar i historiken, så en skärmbild från några minuter sedan är fortfarande ett klick bort.
5. `05-settings.png` — Gränssnittsspråk, start med Windows, kvar i meddelandefältet, och rättelsen som hindrar v1.6.5 från att bli vl.6.5.

### Söktermer

`text från bild`, `skärmbild till text`, `textigenkänning`, `läsa QR-kod`,
`streckkodsläsare`, `extrahera text`, `läs upp text`

---

## Dansk — Danish

### Kort beskrivelse

Glyfo henter teksten ud af alt, hvad du kan se: et skærmbillede, et foto af en side, et scannet
dokument, et dias, et videobillede. Tryk Alt+Z og træk en ramme om en del af skærmen, eller åbn en
fil, indsæt fra udklipsholderen, send et billede fra en anden app. Den genkendte tekst står ved
siden af billedet, klar til at kopiere, lytte til eller rydde op i. Glyfo læser også QR-koder og
stregkoder i det samme billede, og på en Copilot+-pc oversætter den resultatet. Det hele sker på din
egen pc — Glyfo opretter ingen internetforbindelser. Gratis, uden reklamer og uden køb.

### Beskrivelse

Glyfo laver billeder med tekst om til tekst, du kan bruge.

Genkendelsen kører på den OCR, der allerede findes i Windows: ingen konto, ingen upload, ingen
ventetid på en server. På en Copilot+-pc bruger Glyfo desuden tekstgenkendelsesmodellen på enheden
til de svære billeder og kan oversætte resultatet — også det uden netværk.

**Fire måder at få et billede ind på**
- Alt+Z trækker en ramme om en hvilken som helst del af skærmen. Ctrl+Shift+R tager hele skærmen.
- Ctrl+V indsætter et billede fra udklipsholderen, tekst også.
- Ctrl+O åbner en fil; at trække den ind i vinduet virker lige så godt.
- Højreklik et billede i Stifinder og åbn det med Glyfo, eller send det via delingspanelet i Windows
  (Billeder, Klippeværktøj, browseren).

**Hvad du får igen**
- Den genkendte tekst ved siden af billedet, i den rækkefølge den lå i.
- Kopiér med ét klik, eller lad et udklip kopiere sig selv, så snart det er færdigt.
- Oplæsning med enhver stemme, der er installeret på pc'en.
- ”Fjern linjeskift” samler hårde linjeskift til afsnit igen, og ”Fjern mellemrum” fjerner hvert
  eneste mellemrum — netop det, kinesisk, japansk og koreansk tekst har brug for efter genkendelse.
- QR-koder og stregkoder fra det samme billede.
- En historik over de seneste resultater: udklippet fra to minutter siden er stadig ét klik væk.

**Du bestemmer, hvordan den læser**
- Vælg genkendelsessprog blandt de sprogpakker, der er installeret i Windows, eller lad Glyfo vælge.
- En indstilling retter den klassiske OCR-fejl, hvor v1.6.5 læses som vl.6.5: et l eller I bliver
  kun til et 1-tal, hvor der står et skilletegn og et ciffer ved siden af, så html5 og IPv6 får lov
  at være.
- Tilpas til vinduet eller vis i faktisk størrelse; genkend hele billedet eller kun markeringen.

**Den er ikke i vejen**
- Lukker du vinduet, bliver Glyfo liggende i meddelelsesområdet, og genvejen virker stadig. ”Afslut”
  derfra lukker den for alvor. Den adfærd er selv en indstilling.
- Glyfo kan starte med Windows og gå direkte i meddelelsesområdet uden at åbne et vindue, så
  genvejen virker fra det øjeblik, du logger på. Slået fra som standard; du slår den til selv.
- Når vinduet er skjult, viser en meddelelse den første linje af det, der lige er genkendt.

**Sprog**
Brugerfladen findes på 33 sprog og følger sprogindstillingen i Windows. Genkendelsen bruger de
OCR-sprogpakker, der ligger på pc'en — flere tilføjer du under Indstillinger › Klokkeslæt og sprog ›
Sprog og område.

**Beskyttelse af personlige oplysninger**
Glyfo opretter ingen netværksforbindelser. Billeder, genkendt tekst og oversættelser forlader aldrig
din pc. Ingen konto, ingen telemetri, ingen reklamer.

### Produktfunktioner

- Tag et udklip af en hvilken som helst del af skærmen med Alt+Z og genkend det straks, uden at forlade den app, du læste i
- Bygger på den OCR, der er indbygget i Windows, og på Copilot+-pc'er også på genkendelsesmodellen på enheden
- Åbn en fil, indsæt fra udklipsholderen, træk og slip eller modtag via delingspanelet i Windows
- Læser QR-koder og stregkoder fra det samme billede
- Oversætter på Copilot+-pc'er, på enheden, uden netværksadgang
- Læser resultatet op med enhver stemme, der er installeret på pc'en
- Fjern linjeskift og mellemrum — den oprydning, CJK-tekst har brug for efter genkendelse
- Bliver liggende i meddelelsesområdet, så genvejen virker, også når vinduet er lukket
- Brugerflade på 33 sprog, følger sprogindstillingen i Windows
- Opretter aldrig forbindelse til internettet: det, du genkender, forlader ikke din pc

### Billedtekster til skærmbilleder

1. `01-text-from-a-page.png` — Den genkendte tekst står ved siden af billedet i den rækkefølge, den blev læst: klar til at kopiere, lytte til eller samle til afsnit igen.
2. `02-any-language.png` — Genkendelsessproget vælger du blandt de sprogpakker, der er installeret i Windows — eller Glyfo afgør det selv.
3. `03-qr-and-barcodes.png` — QR-koder og stregkoder læses fra det samme billede; en separat scannerapp er ikke nødvendig.
4. `04-history.png` — De seneste resultater bliver i historikken, så et udklip fra få minutter siden er stadig ét klik væk.
5. `05-settings.png` — Sprog i brugerfladen, start med Windows, bliv i meddelelsesområdet, og rettelsen der forhindrer v1.6.5 i at blive til vl.6.5.

### Søgetermer

`tekst fra billede`, `skærmbillede til tekst`, `tekstgenkendelse`, `læs QR-kode`,
`stregkodescanner`, `udtræk tekst`, `læs op`

---

## Norsk bokmål — Norwegian

### Kort beskrivelse

Glyfo henter teksten ut av alt du kan se: et skjermbilde, et foto av en side, et skannet dokument,
et lysbilde, en videorute. Trykk Alt+Z og ramme inn en del av skjermen, eller åpne en fil, lim inn
fra utklippstavlen, send et bilde fra en annen app. Den gjenkjente teksten står ved siden av bildet,
klar til å kopieres, lyttes til eller ryddes opp i. Glyfo leser også QR-koder og strekkoder fra det
samme bildet, og på en Copilot+-PC oversetter den resultatet. Alt skjer på din egen PC — Glyfo
oppretter ingen internettforbindelser. Gratis, uten annonser og uten kjøp.

### Beskrivelse

Glyfo gjør bilder med tekst om til tekst du kan bruke.

Gjenkjenningen bygger på OCR-en som allerede finnes i Windows: ingen konto, ingen opplasting, ingen
venting på en server. På en Copilot+-PC bruker Glyfo i tillegg tekstgjenkjenningsmodellen på enheten
til de vanskelige bildene og kan oversette resultatet — også det uten nettverk.

**Fire måter å få inn et bilde på**
- Alt+Z rammer inn hvilken som helst del av skjermen. Ctrl+Shift+R tar hele skjermen.
- Ctrl+V limer inn et bilde fra utklippstavlen, tekst også.
- Ctrl+O åpner en fil; å dra den inn i vinduet fungerer like godt.
- Høyreklikk et bilde i Filutforsker og åpne det med Glyfo, eller send det via delingspanelet i
  Windows (Bilder, Utklippsverktøy, nettleseren).

**Hva du får tilbake**
- Den gjenkjente teksten ved siden av bildet, i den rekkefølgen den lå i.
- Kopier med ett klikk, eller la et utklipp kopiere seg selv så snart det er ferdig.
- Opplesing med hvilken som helst stemme som er installert på PC-en.
- ”Fjern linjeskift” setter harde linjeskift sammen til avsnitt igjen, og ”Fjern mellomrom” fjerner
  hvert mellomrom — akkurat det kinesisk, japansk og koreansk tekst trenger etter gjenkjenning.
- QR-koder og strekkoder fra det samme bildet.
- En historikk over de siste resultatene: utklippet fra to minutter siden er fortsatt ett klikk unna.

**Du bestemmer hvordan den leser**
- Velg gjenkjenningsspråk blant språkpakkene som er installert i Windows, eller la Glyfo velge.
- En innstilling retter den klassiske OCR-feilen der v1.6.5 leses som vl.6.5: en l eller I blir et
  1-tall bare der det står et skilletegn og et siffer ved siden av, så html5 og IPv6 blir stående.
- Tilpass til vinduet eller vis i faktisk størrelse; gjenkjenn hele bildet eller bare utvalget.

**Den er ikke i veien**
- Lukker du vinduet, blir Glyfo liggende i systemstatusfeltet, og hurtigtasten virker fortsatt.
  ”Avslutt” derfra lukker den for godt. Denne oppførselen er i seg selv en innstilling.
- Glyfo kan starte med Windows og gå rett til systemstatusfeltet uten å åpne et vindu, slik at
  hurtigtasten virker fra det øyeblikket du logger på. Av som standard; du slår den på selv.
- Når vinduet er skjult, viser et varsel den første linjen av det som nettopp ble gjenkjent.

**Språk**
Grensesnittet finnes på 33 språk og følger språkinnstillingen i Windows. Gjenkjenningen bruker
OCR-språkpakkene som ligger på PC-en — flere legger du til under Innstillinger › Tid og språk ›
Språk og område.

**Personvern**
Glyfo oppretter ingen nettverksforbindelser. Bilder, gjenkjent tekst og oversettelser forlater aldri
PC-en din. Ingen konto, ingen telemetri, ingen annonser.

### Produktfunksjoner

- Ta et utklipp av hvilken som helst del av skjermen med Alt+Z og gjenkjenn det med én gang, uten å forlate appen du leste i
- Bygger på OCR-en som er innebygd i Windows, og på Copilot+-PC-er også på gjenkjenningsmodellen på enheten
- Åpne en fil, lim inn fra utklippstavlen, dra og slipp eller motta via delingspanelet i Windows
- Leser QR-koder og strekkoder fra det samme bildet
- Oversetter på Copilot+-PC-er, på enheten, uten nettverkstilgang
- Leser opp resultatet med hvilken som helst stemme som er installert på PC-en
- Fjern linjeskift og mellomrom — oppryddingen CJK-tekst trenger etter gjenkjenning
- Blir liggende i systemstatusfeltet, så hurtigtasten virker også når vinduet er lukket
- Grensesnitt på 33 språk, følger språkinnstillingen i Windows
- Kobler seg aldri til internett: det du gjenkjenner, forlater ikke PC-en din

### Bildetekster til skjermbilder

1. `01-text-from-a-page.png` — Den gjenkjente teksten står ved siden av bildet i den rekkefølgen den ble lest: klar til å kopieres, lyttes til eller settes sammen til avsnitt igjen.
2. `02-any-language.png` — Gjenkjenningsspråket velger du blant språkpakkene som er installert i Windows — eller Glyfo bestemmer det selv.
3. `03-qr-and-barcodes.png` — QR-koder og strekkoder leses fra det samme bildet; en egen skanne-app er ikke nødvendig.
4. `04-history.png` — De siste resultatene blir liggende i historikken, så et utklipp fra noen minutter siden er fortsatt ett klikk unna.
5. `05-settings.png` — Språk i grensesnittet, start med Windows, bli i systemstatusfeltet, og rettelsen som hindrer v1.6.5 i å bli vl.6.5.

### Søkeord

`tekst fra bilde`, `skjermbilde til tekst`, `tekstgjenkjenning`, `lese QR-kode`,
`strekkodeleser`, `hente ut tekst`, `les opp`

---

## Suomi — Finnish

### Lyhyt kuvaus

Glyfo poimii tekstin kaikesta, mitä näet: kuvakaappauksesta, sivun valokuvasta, skannatusta
asiakirjasta, diasta, videoruudusta. Paina Alt+Z ja rajaa osa näytöstä, tai avaa tiedosto, liitä
leikepöydältä, lähetä kuva toisesta sovelluksesta. Tunnistettu teksti ilmestyy kuvan viereen valmiina
kopioitavaksi, kuunneltavaksi tai siistittäväksi. Glyfo lukee samasta kuvasta myös QR-koodit ja
viivakoodit, ja Copilot+-tietokoneessa se kääntää tuloksen. Kaikki tapahtuu omalla koneellasi — Glyfo
ei muodosta lainkaan internet-yhteyksiä. Ilmainen, ei mainoksia eikä ostoksia.

### Kuvaus

Glyfo muuttaa tekstiä sisältävät kuvat tekstiksi, jota voi käyttää.

Tunnistus toimii Windowsin omalla OCR:llä: ei tiliä, ei latausta palvelimelle, ei odottelua.
Copilot+-tietokoneessa Glyfo käyttää vaikeisiin kuviin lisäksi laitteessa toimivaa
tekstintunnistusmallia ja osaa kääntää tuloksen — myös se tapahtuu ilman verkkoa.

**Neljä tapaa tuoda kuva sisään**
- Alt+Z rajaa minkä tahansa osan näytöstä. Ctrl+Shift+R ottaa koko näytön.
- Ctrl+V liittää kuvan leikepöydältä, tekstin myös.
- Ctrl+O avaa tiedoston; ikkunaan raahaaminen toimii yhtä hyvin.
- Napsauta kuvaa Resurssienhallinnassa hiiren kakkospainikkeella ja avaa se Glyfossa, tai lähetä se
  Windowsin jakopaneelista (Kuvat, Kuvakaappaustyökalu, selain).

**Mitä saat takaisin**
- Tunnistetun tekstin kuvan vieressä, alkuperäisen asettelun järjestyksessä.
- Kopiointi yhdellä napsautuksella, tai anna kaappauksen kopioida itsensä heti kun se on valmis.
- Ääneen lukeminen millä tahansa koneelle asennetulla äänellä.
- ”Poista rivinvaihdot” liittää kovat rivinvaihdot takaisin kappaleiksi ja ”Poista välilyönnit”
  poistaa jokaisen välilyönnin — juuri sitä kiinan-, japanin- ja koreankielinen teksti tarvitsee
  tunnistuksen jälkeen.
- QR-koodit ja viivakoodit samasta kuvasta.
- Historia viimeisimmistä tuloksista: kahden minuutin takainen kaappaus on yhä yhden napsautuksen
  päässä.

**Sinä päätät, miten se lukee**
- Valitse tunnistuskieli Windowsiin asennetuista kielipaketeista tai anna Glyfon valita.
- Yksi asetus korjaa klassisen OCR-virheen, jossa v1.6.5 luetaan muodossa vl.6.5: l tai I muuttuu
  ykköseksi vain silloin, kun vieressä on erotin ja numero, joten html5 ja IPv6 jäävät ennalleen.
- Sovita ikkunaan tai näytä todellisessa koossa; tunnista koko kuva tai vain valinta.

**Se ei ole tiellä**
- Kun suljet ikkunan, Glyfo jää ilmoitusalueelle ja kaappauspikanäppäin toimii edelleen. Sieltä
  valittu ”Lopeta” sulkee sen kokonaan. Tämäkin toiminta on oma asetuksensa.
- Glyfo voi käynnistyä Windowsin mukana ja mennä suoraan ilmoitusalueelle avaamatta ikkunaa, jolloin
  pikanäppäin toimii heti kirjautumisesta alkaen. Oletuksena pois päältä; sinä otat sen käyttöön.
- Kun ikkuna on piilossa, ilmoitus näyttää juuri tunnistetun tekstin ensimmäisen rivin.

**Kielet**
Käyttöliittymä on saatavilla 33 kielellä ja noudattaa Windowsin kieliasetusta. Tunnistus käyttää
koneelle asennettuja OCR-kielipaketteja — lisää niitä kohdasta Asetukset › Aika ja kieli › Kieli ja
alue.

**Tietosuoja**
Glyfo ei muodosta verkkoyhteyksiä. Kuvat, tunnistettu teksti ja käännökset eivät poistu koneeltasi
koskaan. Ei tiliä, ei telemetriaa, ei mainoksia.

### Tuotteen ominaisuudet

- Kaappaa mikä tahansa osa näytöstä Alt+Z:lla ja tunnista se heti poistumatta sovelluksesta, jota olit lukemassa
- Perustuu Windowsin sisäänrakennettuun OCR:ään, ja Copilot+-koneissa myös laitteessa toimivaan tunnistusmalliin
- Avaa tiedosto, liitä leikepöydältä, raahaa ja pudota tai vastaanota Windowsin jakopaneelista
- Lukee samasta kuvasta QR-koodit ja viivakoodit
- Kääntää Copilot+-koneissa laitteessa, ilman verkkoyhteyttä
- Lukee tuloksen ääneen millä tahansa koneelle asennetulla äänellä
- Rivinvaihtojen ja välilyöntien poisto — se siistiminen, jota CJK-teksti tunnistuksen jälkeen tarvitsee
- Jää ilmoitusalueelle, joten pikanäppäin toimii myös ikkunan sulkemisen jälkeen
- Käyttöliittymä 33 kielellä, noudattaa Windowsin kieliasetusta
- Ei yhdistä internetiin: se, minkä tunnistat, ei poistu koneeltasi

### Kuvakaappausten tekstit

1. `01-text-from-a-page.png` — Tunnistettu teksti on kuvan vieressä siinä järjestyksessä, jossa se luettiin: valmiina kopioitavaksi, kuunneltavaksi tai koottavaksi takaisin kappaleiksi.
2. `02-any-language.png` — Tunnistuskielen valitset Windowsiin asennetuista kielipaketeista — tai Glyfo päättää sen itse.
3. `03-qr-and-barcodes.png` — QR-koodit ja viivakoodit luetaan samasta kuvasta; erillistä skannaussovellusta ei tarvita.
4. `04-history.png` — Viimeisimmät tulokset jäävät historiaan, joten muutaman minuutin takainen kaappaus on yhä yhden napsautuksen päässä.
5. `05-settings.png` — Käyttöliittymän kieli, käynnistys Windowsin mukana, ilmoitusalueelle jääminen ja korjaus, joka estää v1.6.5:tä muuttumasta muotoon vl.6.5.

### Hakutermit

`teksti kuvasta`, `kuvakaappaus tekstiksi`, `tekstintunnistus`, `QR-koodin lukija`,
`viivakoodinlukija`, `poimi teksti`, `lue ääneen`

---

## Ελληνικά — Greek

### Σύντομη περιγραφή

Το Glyfo βγάζει το κείμενο από οτιδήποτε βλέπετε: ένα στιγμιότυπο οθόνης, τη φωτογραφία μιας
σελίδας, ένα σαρωμένο έγγραφο, μια διαφάνεια, ένα καρέ βίντεο. Πατήστε Alt+Z και πλαισιώστε ένα
τμήμα της οθόνης, ή ανοίξτε ένα αρχείο, επικολλήστε από το πρόχειρο, στείλτε μια εικόνα από άλλη
εφαρμογή. Το κείμενο που αναγνωρίστηκε εμφανίζεται δίπλα στην εικόνα, έτοιμο για αντιγραφή, ακρόαση
ή καθάρισμα. Το Glyfo διαβάζει επίσης κωδικούς QR και barcode από την ίδια εικόνα, και σε έναν
υπολογιστή Copilot+ μεταφράζει το αποτέλεσμα. Όλα γίνονται στον υπολογιστή σας — το Glyfo δεν κάνει
καμία σύνδεση στο διαδίκτυο. Δωρεάν, χωρίς διαφημίσεις και χωρίς αγορές.

### Περιγραφή

Το Glyfo μετατρέπει τις εικόνες με κείμενο σε κείμενο που μπορείτε να χρησιμοποιήσετε.

Η αναγνώριση στηρίζεται στο OCR που είναι ήδη μέσα στα Windows: χωρίς λογαριασμό, χωρίς αποστολή
αρχείων, χωρίς αναμονή για κάποιον διακομιστή. Σε υπολογιστή Copilot+, το Glyfo αξιοποιεί επιπλέον
το μοντέλο αναγνώρισης κειμένου που τρέχει στη συσκευή για τις δύσκολες εικόνες και μπορεί να
μεταφράσει το αποτέλεσμα — και αυτό χωρίς δίκτυο.

**Τέσσερις τρόποι να μπει μια εικόνα**
- Alt+Z για να πλαισιώσετε οποιοδήποτε τμήμα της οθόνης. Ctrl+Shift+R παίρνει ολόκληρη την οθόνη.
- Ctrl+V επικολλά εικόνα από το πρόχειρο, και κείμενο επίσης.
- Ctrl+O ανοίγει ένα αρχείο· η μεταφορά του μέσα στο παράθυρο δουλεύει το ίδιο καλά.
- Δεξί κλικ σε μια εικόνα στην Εξερεύνηση αρχείων και άνοιγμα με το Glyfo, ή αποστολή από το
  παράθυρο κοινής χρήσης των Windows (Φωτογραφίες, Εργαλείο αποκομμάτων, πρόγραμμα περιήγησης).

**Τι παίρνετε πίσω**
- Το αναγνωρισμένο κείμενο δίπλα στην εικόνα, με τη σειρά που είχε στη διάταξη.
- Αντιγραφή με ένα κλικ, ή αφήστε το απόκομμα να αντιγραφεί μόνο του μόλις ολοκληρωθεί.
- Ανάγνωση φωναχτά με όποια φωνή είναι εγκατεστημένη στον υπολογιστή.
- Η «Αφαίρεση αλλαγών γραμμής» ενώνει ξανά τις σκληρές αλλαγές γραμμής σε παραγράφους και η
  «Αφαίρεση κενών» σβήνει κάθε κενό — ακριβώς αυτό που χρειάζεται το κινεζικό, ιαπωνικό και κορεατικό
  κείμενο μετά την αναγνώριση.
- Κωδικοί QR και barcode από την ίδια εικόνα.
- Ένα ιστορικό των πρόσφατων αποτελεσμάτων: το απόκομμα των δύο λεπτών πριν είναι ακόμη ένα κλικ
  μακριά.

**Εσείς αποφασίζετε πώς διαβάζει**
- Διαλέξτε γλώσσα αναγνώρισης ανάμεσα στα πακέτα που είναι εγκατεστημένα στα Windows, ή αφήστε το
  Glyfo να αποφασίσει.
- Μια επιλογή διορθώνει το κλασικό λάθος του OCR που διαβάζει το v1.6.5 ως vl.6.5: το l ή το I
  γίνεται 1 μόνο εκεί που δίπλα του υπάρχει διαχωριστικό και ψηφίο, οπότε το html5 και το IPv6 μένουν
  ανέπαφα.
- Προσαρμογή στο παράθυρο ή προβολή σε πραγματικό μέγεθος· αναγνώριση ολόκληρης της εικόνας ή μόνο
  της επιλογής.

**Δεν σας μπαίνει εμπόδιο**
- Κλείνοντας το παράθυρο, το Glyfo παραμένει στην περιοχή ειδοποιήσεων και η συντόμευση συνεχίζει να
  δουλεύει. Η «Έξοδος» από εκεί το κλείνει στ' αλήθεια. Η ίδια αυτή συμπεριφορά είναι κι αυτή
  ρύθμιση.
- Το Glyfo μπορεί να ξεκινά μαζί με τα Windows και να πηγαίνει κατευθείαν στην περιοχή ειδοποιήσεων
  χωρίς να ανοίγει παράθυρο, ώστε η συντόμευση να δουλεύει από τη στιγμή που συνδέεστε.
  Απενεργοποιημένο εξ ορισμού· εσείς το ανοίγετε.
- Με το παράθυρο κρυμμένο, μια ειδοποίηση δείχνει την πρώτη γραμμή αυτού που μόλις αναγνωρίστηκε.

**Γλώσσες**
Το περιβάλλον είναι διαθέσιμο σε 33 γλώσσες και ακολουθεί τη ρύθμιση γλώσσας των Windows. Η
αναγνώριση χρησιμοποιεί τα πακέτα γλώσσας OCR που υπάρχουν στον υπολογιστή — προσθέτετε κι άλλα από
τις Ρυθμίσεις › Ώρα και γλώσσα › Γλώσσα και περιοχή.

**Απόρρητο**
Το Glyfo δεν κάνει συνδέσεις δικτύου. Οι εικόνες, το αναγνωρισμένο κείμενο και οι μεταφράσεις δεν
φεύγουν ποτέ από τον υπολογιστή σας. Χωρίς λογαριασμό, χωρίς τηλεμετρία, χωρίς διαφημίσεις.

### Δυνατότητες προϊόντος

- Αποτυπώστε οποιοδήποτε τμήμα της οθόνης με Alt+Z και αναγνωρίστε το αμέσως, χωρίς να φύγετε από την εφαρμογή που διαβάζατε
- Στηρίζεται στο ενσωματωμένο OCR των Windows και, σε υπολογιστές Copilot+, στο μοντέλο αναγνώρισης της συσκευής
- Άνοιγμα αρχείου, επικόλληση από το πρόχειρο, μεταφορά και απόθεση ή λήψη από το παράθυρο κοινής χρήσης
- Διαβάζει κωδικούς QR και barcode από την ίδια εικόνα
- Μεταφράζει σε υπολογιστές Copilot+, στη συσκευή, χωρίς πρόσβαση στο δίκτυο
- Διαβάζει το αποτέλεσμα φωναχτά με όποια φωνή είναι εγκατεστημένη στον υπολογιστή
- Αφαίρεση αλλαγών γραμμής και κενών — το καθάρισμα που χρειάζεται το κείμενο CJK μετά την αναγνώριση
- Παραμένει στην περιοχή ειδοποιήσεων, ώστε η συντόμευση να δουλεύει και με κλειστό παράθυρο
- Περιβάλλον σε 33 γλώσσες, ακολουθεί τη ρύθμιση γλώσσας των Windows
- Δεν συνδέεται στο διαδίκτυο: ό,τι αναγνωρίζετε δεν φεύγει από τον υπολογιστή σας

### Λεζάντες στιγμιότυπων

1. `01-text-from-a-page.png` — Το αναγνωρισμένο κείμενο στέκεται δίπλα στην εικόνα με τη σειρά που διαβάστηκε: έτοιμο για αντιγραφή, ακρόαση ή ένωση ξανά σε παραγράφους.
2. `02-any-language.png` — Τη γλώσσα αναγνώρισης τη διαλέγετε ανάμεσα στα πακέτα που είναι εγκατεστημένα στα Windows — ή την ορίζει μόνο του το Glyfo.
3. `03-qr-and-barcodes.png` — Κωδικοί QR και barcode διαβάζονται από την ίδια εικόνα· δεν χρειάζεται ξεχωριστή εφαρμογή σάρωσης.
4. `04-history.png` — Τα πρόσφατα αποτελέσματα μένουν στο ιστορικό, οπότε ένα απόκομμα λίγων λεπτών πριν είναι ακόμη ένα κλικ μακριά.
5. `05-settings.png` — Γλώσσα περιβάλλοντος, εκκίνηση με τα Windows, παραμονή στην περιοχή ειδοποιήσεων και η διόρθωση που δεν αφήνει το v1.6.5 να γίνει vl.6.5.

### Όροι αναζήτησης

`κείμενο από εικόνα`, `στιγμιότυπο σε κείμενο`, `αναγνώριση κειμένου`, `ανάγνωση QR κωδικού`,
`σαρωτής barcode`, `εξαγωγή κειμένου`, `ανάγνωση φωναχτά`

---

## Magyar — Hungarian

### Rövid leírás

A Glyfo mindenből kiszedi a szöveget, amit látsz: képernyőképből, egy oldalról készült fotóból,
beszkennelt iratból, diából, videokockából. Nyomd meg az Alt+Z billentyűt, és keretezz be egy részt a
képernyőből, vagy nyiss meg egy fájlt, illessz be a vágólapról, küldj át egy képet egy másik
alkalmazásból. A felismert szöveg a kép mellett jelenik meg, készen a másolásra, felolvasásra vagy
rendbe tételre. A Glyfo ugyanabból a képből a QR-kódokat és vonalkódokat is kiolvassa, Copilot+ gépen
pedig le is fordítja az eredményt. Minden a saját gépeden történik — a Glyfo egyáltalán nem
kapcsolódik az internethez. Ingyenes, hirdetések és vásárlások nélkül.

### Leírás

A Glyfo a szöveget tartalmazó képekből használható szöveget csinál.

A felismerés a Windowsba épített OCR-re támaszkodik: nincs fiók, nincs feltöltés, nincs várakozás egy
kiszolgálóra. Copilot+ gépen a Glyfo a nehezebb képekhez az eszközön futó szövegfelismerő modellt is
igénybe veszi, és le tudja fordítani az eredményt — ez is hálózat nélkül.

**Négyféleképpen kerülhet be egy kép**
- Az Alt+Z bekeretezi a képernyő tetszőleges részét. A Ctrl+Shift+R az egész képernyőt viszi.
- A Ctrl+V képet illeszt be a vágólapról, szöveget is.
- A Ctrl+O fájlt nyit meg; az ablakba húzás ugyanúgy működik.
- Kattints jobb gombbal egy képre a Fájlkezelőben, és nyisd meg a Glyfóval, vagy küldd át a Windows
  megosztási paneljéről (Fényképek, Képmetsző, böngésző).

**Mit kapsz vissza**
- A felismert szöveget a kép mellett, az eredeti elrendezés sorrendjében.
- Másolás egy kattintással, vagy hagyd, hogy a felvétel magától a vágólapra kerüljön, amint elkészül.
- Felolvasás bármelyik, a gépre telepített hanggal.
- A „Sortörések eltávolítása” a kemény sortöréseket visszafűzi bekezdésekké, a „Szóközök
  eltávolítása” pedig minden szóközt kitöröl — pontosan erre van szüksége a kínai, japán és koreai
  szövegnek a felismerés után.
- QR-kódok és vonalkódok ugyanabból a képből.
- A legutóbbi eredmények előzménye: a két perccel ezelőtti felvétel még mindig egy kattintásnyira van.

**Te döntöd el, hogyan olvas**
- A felismerés nyelvét a Windowsban telepített nyelvi csomagok közül választhatod ki, vagy a Glyfóra
  bízhatod.
- Egy beállítás javítja azt a klasszikus OCR-hibát, amelytől a v1.6.5 vl.6.5 lesz: az l vagy az I
  csak ott válik 1-essé, ahol elválasztó és számjegy áll mellette, így a html5 és az IPv6 érintetlen
  marad.
- Igazítás az ablakhoz vagy valódi méret; az egész kép vagy csak a kijelölés felismerése.

**Nincs útban**
- Az ablak bezárásakor a Glyfo az értesítési területen marad, és a gyorsbillentyű továbbra is
  működik. Az ottani „Kilépés” zárja be igazán. Maga ez a viselkedés is beállítás.
- A Glyfo elindulhat a Windowsszal, és ablak nyitása nélkül egyenesen az értesítési területre mehet,
  így a gyorsbillentyű a bejelentkezés pillanatától működik. Alapértelmezés szerint kikapcsolva; te
  kapcsolod be.
- Rejtett ablaknál egy értesítés mutatja az imént felismert szöveg első sorát.

**Nyelvek**
A felület 33 nyelven érhető el, és a Windows nyelvi beállítását követi. A felismerés a gépre
telepített OCR-nyelvi csomagokat használja — továbbiakat a Beállítások › Idő és nyelv › Nyelv és
régió alatt adhatsz hozzá.

**Adatvédelem**
A Glyfo nem létesít hálózati kapcsolatot. A képek, a felismert szöveg és a fordítások soha nem
hagyják el a gépedet. Nincs fiók, nincs telemetria, nincs hirdetés.

### Termékjellemzők

- Vágd ki a képernyő bármelyik részét az Alt+Z billentyűvel, és ismerd fel azonnal, anélkül hogy kilépnél abból az alkalmazásból, amelyet éppen olvastál
- A Windowsba épített OCR-re épül, Copilot+ gépeken pedig az eszközön futó felismerő modellre is
- Fájl megnyitása, beillesztés a vágólapról, húzd és ejtsd, vagy fogadás a Windows megosztási paneljéről
- Kiolvassa ugyanabból a képből a QR-kódokat és a vonalkódokat
- Copilot+ gépeken az eszközön fordít, hálózati hozzáférés nélkül
- Felolvassa az eredményt bármelyik, a gépre telepített hanggal
- Sortörések és szóközök eltávolítása — az a rendrakás, amire a CJK-szövegnek felismerés után szüksége van
- Az értesítési területen marad, így a gyorsbillentyű az ablak bezárása után is működik
- 33 nyelvű felület, a Windows nyelvi beállítását követve
- Nem kapcsolódik az internethez: amit felismersz, nem hagyja el a gépedet

### Képernyőképek feliratai

1. `01-text-from-a-page.png` — A felismert szöveg a kép mellett áll, abban a sorrendben, ahogy beolvasásra került: másolható, felolvastatható vagy visszafűzhető bekezdésekké.
2. `02-any-language.png` — A felismerés nyelvét a Windowsban telepített nyelvi csomagok közül választod ki — vagy a Glyfo dönti el magától.
3. `03-qr-and-barcodes.png` — A QR-kódok és a vonalkódok ugyanabból a képből olvashatók ki; külön szkennelő alkalmazásra nincs szükség.
4. `04-history.png` — A legutóbbi eredmények az előzményekben maradnak, így egy néhány perccel ezelőtti felvétel még mindig egy kattintásnyira van.
5. `05-settings.png` — A felület nyelve, indulás a Windowsszal, az értesítési területen maradás, és a javítás, amitől a v1.6.5 nem lesz vl.6.5.

### Keresési kifejezések

`szöveg képből`, `képernyőkép szöveggé`, `szövegfelismerés`, `QR-kód olvasó`,
`vonalkód olvasó`, `szöveg kinyerése`, `felolvasás`

---

## Română — Romanian

### Descriere scurtă

Glyfo extrage textul din tot ce vezi: o captură de ecran, fotografia unei pagini, un document
scanat, un diapozitiv, un cadru dintr-un clip. Apasă Alt+Z și încadrează o parte a ecranului, ori
deschide un fișier, lipește din clipboard, trimite o imagine din altă aplicație. Textul recunoscut
apare lângă imagine, gata de copiat, de ascultat sau de curățat. Glyfo citește din aceeași imagine
și codurile QR și codurile de bare, iar pe un PC Copilot+ traduce rezultatul. Totul se întâmplă pe
calculatorul tău — Glyfo nu deschide nicio conexiune la internet. Gratuit, fără reclame și fără
achiziții.

### Descriere

Glyfo transformă imaginile cu text în text pe care îl poți folosi.

Recunoașterea se sprijină pe OCR-ul din Windows: fără cont, fără încărcare, fără așteptare după un
server. Pe un PC Copilot+, Glyfo folosește în plus modelul de recunoaștere a textului de pe
dispozitiv pentru imaginile dificile și poate traduce rezultatul — tot fără rețea.

**Patru feluri de a aduce o imagine înăuntru**
- Alt+Z încadrează orice parte a ecranului. Ctrl+Shift+R ia tot ecranul.
- Ctrl+V lipește o imagine din clipboard, și text la fel.
- Ctrl+O deschide un fișier; tragerea lui în fereastră funcționează la fel de bine.
- Clic dreapta pe o imagine în Explorer și deschide-o cu Glyfo, sau trimite-o din panoul de
  partajare Windows (Fotografii, Instrument de decupare, browser).

**Ce primești înapoi**
- Textul recunoscut lângă imagine, în ordinea în care era așezat.
- Copiere cu un clic, sau lasă o captură să se copieze singură imediat ce e gata.
- Citire cu voce tare cu orice voce instalată pe calculator.
- „Elimină întreruperile de rând” lipește la loc rândurile rupte în paragrafe, iar „Elimină
  spațiile” șterge fiecare spațiu — exact ce îi trebuie textului chinezesc, japonez și coreean după
  recunoaștere.
- Coduri QR și coduri de bare din aceeași imagine.
- Un istoric al rezultatelor recente: captura de acum două minute e tot la un clic distanță.

**Tu hotărăști cum citește**
- Alege limba de recunoaștere dintre pachetele instalate în Windows, sau las-o pe Glyfo să aleagă.
- O opțiune repară eroarea clasică de OCR prin care v1.6.5 e citit vl.6.5: un l sau un I devine 1
  doar acolo unde alături stau un separator și o cifră, așa că html5 și IPv6 rămân neatinse.
- Potrivire la fereastră sau vizualizare la dimensiunea reală; recunoaște toată imaginea sau doar
  selecția.

**Nu îți stă în cale**
- Dacă închizi fereastra, Glyfo rămâne în zona de notificare, iar scurtătura funcționează în
  continuare. „Ieșire” de acolo îl închide cu adevărat. Chiar și comportamentul acesta e o setare.
- Glyfo poate porni odată cu Windows și poate merge direct în zona de notificare fără să deschidă o
  fereastră, așa încât scurtătura funcționează din clipa în care te conectezi. Dezactivat implicit;
  tu îl activezi.
- Cu fereastra ascunsă, o notificare arată primul rând din ce tocmai a fost recunoscut.

**Limbi**
Interfața există în 33 de limbi și urmează setarea de limbă din Windows. Recunoașterea folosește
pachetele de limbă OCR instalate pe calculator — mai adaugi din Setări › Oră și limbă › Limbă și
regiune.

**Confidențialitate**
Glyfo nu deschide conexiuni de rețea. Imaginile, textul recunoscut și traducerile nu îți părăsesc
niciodată calculatorul. Fără cont, fără telemetrie, fără reclame.

### Caracteristicile produsului

- Capturează orice parte a ecranului cu Alt+Z și recunoaște-o pe loc, fără să ieși din aplicația în care citeai
- Se bazează pe OCR-ul integrat în Windows, iar pe PC-urile Copilot+ și pe modelul de recunoaștere de pe dispozitiv
- Deschide un fișier, lipește din clipboard, trage și plasează sau primește din panoul de partajare Windows
- Citește coduri QR și coduri de bare din aceeași imagine
- Traduce pe PC-urile Copilot+, pe dispozitiv, fără acces la rețea
- Citește rezultatul cu voce tare cu orice voce instalată pe calculator
- Eliminarea întreruperilor de rând și a spațiilor — curățenia de care are nevoie textul CJK după recunoaștere
- Rămâne în zona de notificare, așa că scurtătura merge și după închiderea ferestrei
- Interfață în 33 de limbi, după setarea de limbă din Windows
- Nu se conectează la internet: ce recunoști nu îți părăsește calculatorul

### Subtitrări pentru capturi de ecran

1. `01-text-from-a-page.png` — Textul recunoscut stă lângă imagine în ordinea în care a fost citit: gata de copiat, de ascultat sau de lipit la loc în paragrafe.
2. `02-any-language.png` — Limba de recunoaștere se alege dintre pachetele instalate în Windows — sau o stabilește Glyfo singur.
3. `03-qr-and-barcodes.png` — Codurile QR și codurile de bare se citesc din aceeași imagine; nu îți trebuie o aplicație separată de scanare.
4. `04-history.png` — Rezultatele recente rămân în istoric, așa că o captură de acum câteva minute e tot la un clic distanță.
5. `05-settings.png` — Limba interfeței, pornirea odată cu Windows, rămânerea în zona de notificare și corecția care nu lasă v1.6.5 să devină vl.6.5.

### Termeni de căutare

`text din imagine`, `captură de ecran în text`, `recunoaștere text`, `citire cod QR`,
`scaner coduri de bare`, `extrage text`, `citire cu voce tare`

---

## Українська — Ukrainian

### Короткий опис

Glyfo дістає текст з усього, що ви бачите: зі знімка екрана, з фотографії сторінки, зі сканованого
документа, зі слайда, з кадру відео. Натисніть Alt+Z і обведіть частину екрана, або відкрийте файл,
вставте з буфера обміну, надішліть зображення з іншої програми. Розпізнаний текст з'являється поруч
із зображенням — готовий скопіювати, прослухати або причесати. Glyfo зчитує з того самого
зображення й QR-коди та штрихкоди, а на комп'ютері Copilot+ ще й перекладає результат. Усе
відбувається на вашому комп'ютері — Glyfo не встановлює жодних інтернет-з'єднань. Безкоштовно, без
реклами та без покупок.

### Опис

Glyfo перетворює зображення з текстом на текст, яким можна користуватися.

Розпізнавання працює на OCR, вбудованому у Windows: без облікового запису, без завантаження на
сервер, без очікування. На комп'ютері Copilot+ Glyfo додатково задіює модель розпізнавання тексту
просто на пристрої для складних зображень і вміє перекласти результат — теж без мережі.

**Чотири способи завести зображення**
- Alt+Z обводить будь-яку частину екрана. Ctrl+Shift+R знімає весь екран.
- Ctrl+V вставляє зображення з буфера обміну, а також текст.
- Ctrl+O відкриває файл; перетягування у вікно працює так само.
- Клацніть зображення правою кнопкою у Провіднику й відкрийте його в Glyfo або надішліть із панелі
  спільного доступу Windows (Фотографії, Ножиці, браузер).

**Що ви отримуєте**
- Розпізнаний текст поруч із зображенням, у порядку початкового розташування.
- Копіювання одним клацанням або автоматичне копіювання знімка одразу після завершення.
- Читання вголос будь-яким голосом, встановленим на комп'ютері.
- «Прибрати переноси» зшиває жорсткі розриви рядків назад в абзаци, а «Прибрати пробіли» видаляє
  кожен пробіл — саме це потрібно китайському, японському та корейському тексту після
  розпізнавання.
- QR-коди та штрихкоди з того самого зображення.
- Історія останніх результатів: знімок дводавнини все ще за одне клацання.

**Ви вирішуєте, як воно читає**
- Мову розпізнавання оберіть із мовних пакетів, встановлених у Windows, або довіртеся Glyfo.
- Окремий параметр виправляє класичну помилку OCR, через яку v1.6.5 читається як vl.6.5: l або I
  стає одиницею лише там, де поруч стоять роздільник і цифра, тож html5 та IPv6 лишаються цілими.
- Вписати у вікно або показати в справжньому розмірі; розпізнати все зображення чи тільки виділене.

**Воно не заважає**
- Після закриття вікна Glyfo лишається в області сповіщень, і комбінація клавіш працює далі.
  «Вийти» звідти закриває його по-справжньому. Сама ця поведінка теж є перемикачем.
- Glyfo може запускатися разом із Windows і одразу йти в область сповіщень, не відкриваючи вікна,
  тож комбінація працює з моменту входу в систему. Типово вимкнено; вмикаєте ви самі.
- Коли вікно приховане, сповіщення показує перший рядок щойно розпізнаного тексту.

**Мови**
Інтерфейс доступний 33 мовами й слідує за мовним налаштуванням Windows. Розпізнавання використовує
мовні пакети OCR, встановлені на комп'ютері — нові додаються в Параметри › Час і мова › Мова та
регіон.

**Конфіденційність**
Glyfo не встановлює мережевих з'єднань. Зображення, розпізнаний текст і переклади ніколи не
залишають ваш комп'ютер. Без облікового запису, без телеметрії, без реклами.

### Функції продукту

- Зніміть будь-яку частину екрана комбінацією Alt+Z і розпізнайте її одразу, не виходячи з програми, яку читали
- Спирається на вбудований в Windows OCR, а на комп'ютерах Copilot+ ще й на модель розпізнавання на пристрої
- Відкрити файл, вставити з буфера обміну, перетягнути або прийняти з панелі спільного доступу Windows
- Зчитує QR-коди та штрихкоди з того самого зображення
- Перекладає на комп'ютерах Copilot+, просто на пристрої, без доступу до мережі
- Читає результат уголос будь-яким голосом, встановленим на комп'ютері
- Прибирання переносів і пробілів — те прибирання, якого текст CJK потребує після розпізнавання
- Лишається в області сповіщень, тож комбінація клавіш працює й після закриття вікна
- Інтерфейс 33 мовами, за мовним налаштуванням Windows
- Не під'єднується до інтернету: те, що ви розпізнаєте, не залишає ваш комп'ютер

### Підписи до знімків екрана

1. `01-text-from-a-page.png` — Розпізнаний текст стоїть поруч із зображенням у порядку, в якому його прочитали: копіюйте, слухайте або зшивайте назад в абзаци.
2. `02-any-language.png` — Мову розпізнавання ви обираєте з мовних пакетів, встановлених у Windows — або Glyfo визначає її сам.
3. `03-qr-and-barcodes.png` — QR-коди та штрихкоди зчитуються з того самого зображення; окрема програма-сканер не потрібна.
4. `04-history.png` — Останні результати лишаються в історії, тож знімок кількахвилинної давнини все ще за одне клацання.
5. `05-settings.png` — Мова інтерфейсу, запуск разом із Windows, перебування в області сповіщень і виправлення, яке не дає v1.6.5 стати vl.6.5.

### Пошукові терміни

`текст із зображення`, `скриншот у текст`, `розпізнавання тексту`, `сканер QR-коду`,
`сканер штрихкодів`, `витягти текст`, `читати вголос`

---

## Tiếng Việt — Vietnamese

### Mô tả ngắn

Glyfo lấy chữ ra khỏi mọi thứ bạn nhìn thấy: một ảnh chụp màn hình, ảnh chụp một trang sách, một
bản quét, một trang chiếu, một khung hình video. Nhấn Alt+Z rồi khoanh một vùng màn hình, hoặc mở
tệp, dán từ bảng tạm, gửi ảnh sang từ ứng dụng khác. Văn bản nhận dạng được hiện ngay bên cạnh ảnh,
sẵn sàng để sao chép, nghe đọc hoặc dọn dẹp. Glyfo cũng đọc mã QR và mã vạch từ chính tấm ảnh đó, và
trên máy Copilot+ thì dịch luôn kết quả. Mọi thứ diễn ra trên máy của bạn — Glyfo không hề tạo kết
nối internet nào. Miễn phí, không quảng cáo và không mua thêm.

### Mô tả

Glyfo biến những tấm ảnh có chữ thành văn bản bạn dùng được.

Việc nhận dạng chạy trên OCR có sẵn trong Windows: không cần tài khoản, không tải lên, không chờ máy
chủ. Trên máy Copilot+, Glyfo dùng thêm mô hình nhận dạng văn bản ngay trên thiết bị cho những tấm
ảnh khó và có thể dịch kết quả — cũng không cần mạng.

**Bốn cách đưa ảnh vào**
- Alt+Z khoanh bất kỳ vùng nào trên màn hình. Ctrl+Shift+R lấy toàn màn hình.
- Ctrl+V dán ảnh từ bảng tạm, dán chữ cũng được.
- Ctrl+O mở một tệp; kéo tệp vào cửa sổ cũng cho kết quả như vậy.
- Bấm chuột phải vào một tấm ảnh trong File Explorer rồi mở bằng Glyfo, hoặc gửi từ bảng chia sẻ của
  Windows (Photos, Snipping Tool, trình duyệt).

**Bạn nhận lại được gì**
- Văn bản nhận dạng nằm cạnh ảnh, theo đúng thứ tự bố cục ban đầu.
- Sao chép bằng một cú bấm, hoặc để bản chụp tự sao chép ngay khi xong.
- Đọc to bằng bất kỳ giọng nào đã cài trên máy.
- “Bỏ ngắt dòng” nối những chỗ xuống dòng cứng trở lại thành đoạn văn, còn “Bỏ khoảng trắng” xoá
  từng khoảng trắng — đúng thứ mà văn bản tiếng Trung, tiếng Nhật và tiếng Hàn cần sau khi nhận
  dạng.
- Mã QR và mã vạch từ chính tấm ảnh đó.
- Một lịch sử các kết quả gần đây: bản chụp hai phút trước vẫn chỉ cách một cú bấm.

**Bạn quyết định cách nó đọc**
- Chọn ngôn ngữ nhận dạng trong số các gói đã cài trong Windows, hoặc để Glyfo tự chọn.
- Một tuỳ chọn sửa lỗi OCR kinh điển khiến v1.6.5 bị đọc thành vl.6.5: chữ l hay I chỉ biến thành số
  1 ở chỗ có dấu phân cách và chữ số đứng cạnh, nên html5 và IPv6 vẫn nguyên vẹn.
- Vừa khung cửa sổ hoặc xem đúng kích thước thật; nhận dạng cả tấm ảnh hoặc chỉ phần đã chọn.

**Nó không vướng chân bạn**
- Đóng cửa sổ thì Glyfo vẫn nằm ở khay thông báo và phím tắt chụp vẫn chạy. Chọn “Thoát” ở đó mới
  thực sự đóng hẳn. Bản thân cách hoạt động này cũng là một tuỳ chọn.
- Glyfo có thể khởi động cùng Windows và đi thẳng xuống khay thông báo mà không mở cửa sổ nào, nên
  phím tắt dùng được ngay từ lúc bạn đăng nhập. Mặc định tắt; bạn tự bật.
- Khi cửa sổ đang ẩn, một thông báo hiện dòng đầu tiên của phần vừa nhận dạng xong.

**Ngôn ngữ**
Giao diện có 33 ngôn ngữ và đi theo thiết lập ngôn ngữ của Windows. Việc nhận dạng dùng các gói ngôn
ngữ OCR đã cài trên máy — thêm gói mới ở Settings › Time & language › Language & region.

**Quyền riêng tư**
Glyfo không tạo kết nối mạng. Ảnh, văn bản nhận dạng và bản dịch không bao giờ rời khỏi máy bạn.
Không tài khoản, không thu thập dữ liệu, không quảng cáo.

### Tính năng sản phẩm

- Chụp bất kỳ vùng nào trên màn hình bằng Alt+Z và nhận dạng ngay, không phải rời khỏi ứng dụng bạn đang đọc
- Chạy trên OCR có sẵn của Windows, và trên máy Copilot+ còn dùng thêm mô hình nhận dạng trên thiết bị
- Mở tệp, dán từ bảng tạm, kéo thả hoặc nhận từ bảng chia sẻ của Windows
- Đọc mã QR và mã vạch từ chính tấm ảnh đó
- Dịch trên máy Copilot+, ngay trên thiết bị, không cần truy cập mạng
- Đọc to kết quả bằng bất kỳ giọng nào đã cài trên máy
- Bỏ ngắt dòng và khoảng trắng — phần dọn dẹp mà văn bản CJK cần sau khi nhận dạng
- Nằm lại ở khay thông báo, nên phím tắt vẫn chạy sau khi bạn đóng cửa sổ
- Giao diện 33 ngôn ngữ, đi theo thiết lập ngôn ngữ của Windows
- Không kết nối internet: thứ bạn nhận dạng không rời khỏi máy bạn

### Chú thích ảnh chụp màn hình

1. `01-text-from-a-page.png` — Văn bản nhận dạng nằm cạnh tấm ảnh theo đúng thứ tự đã đọc: sẵn sàng để sao chép, nghe đọc hoặc nối lại thành đoạn văn.
2. `02-any-language.png` — Ngôn ngữ nhận dạng chọn trong số các gói đã cài trong Windows — hoặc để Glyfo tự quyết định.
3. `03-qr-and-barcodes.png` — Mã QR và mã vạch được đọc từ chính tấm ảnh đó; không cần thêm ứng dụng quét riêng.
4. `04-history.png` — Các kết quả gần đây nằm lại trong lịch sử, nên bản chụp vài phút trước vẫn chỉ cách một cú bấm.
5. `05-settings.png` — Ngôn ngữ giao diện, khởi động cùng Windows, nằm lại ở khay thông báo, và phần sửa lỗi giữ cho v1.6.5 không thành vl.6.5.

### Từ khoá tìm kiếm

`văn bản từ hình ảnh`, `ảnh chụp màn hình sang chữ`, `nhận dạng văn bản`, `đọc mã QR`,
`quét mã vạch`, `trích xuất văn bản`, `đọc to văn bản`

---

## ไทย — Thai

### คำอธิบายแบบสั้น

Glyfo ดึงข้อความออกมาจากทุกอย่างที่คุณเห็น ไม่ว่าจะเป็นภาพหน้าจอ ภาพถ่ายของหน้าหนังสือ เอกสารที่สแกนมา
สไลด์นำเสนอ หรือเฟรมจากวิดีโอ กด Alt+Z แล้วลากกรอบครอบส่วนใดก็ได้ของหน้าจอ หรือจะเปิดไฟล์ วางจากคลิปบอร์ด
ส่งภาพมาจากแอปอื่นก็ได้ ข้อความที่อ่านได้จะปรากฏข้างภาพ พร้อมให้คัดลอก ฟังเสียงอ่าน หรือจัดให้เรียบร้อย
Glyfo ยังอ่านคิวอาร์โค้ดและบาร์โค้ดจากภาพเดียวกันนี้ด้วย และบนเครื่อง Copilot+ ยังแปลผลลัพธ์ให้อีก
ทุกอย่างเกิดขึ้นบนเครื่องของคุณเอง Glyfo ไม่เชื่อมต่ออินเทอร์เน็ตเลยแม้แต่ครั้งเดียว ใช้ฟรี ไม่มีโฆษณา
และไม่มีการซื้อเพิ่ม

### คำอธิบาย

Glyfo เปลี่ยนภาพที่มีตัวหนังสือให้กลายเป็นข้อความที่คุณเอาไปใช้งานต่อได้

การอ่านข้อความทำงานบน OCR ที่มีอยู่แล้วในตัว Windows ไม่ต้องสมัครบัญชี ไม่ต้องอัปโหลด
ไม่ต้องรอเซิร์ฟเวอร์ บนเครื่อง Copilot+ นั้น Glyfo จะเรียกใช้โมเดลรู้จำข้อความที่ทำงานบนเครื่องเพิ่มอีกชั้น
สำหรับภาพที่อ่านยาก และแปลผลลัพธ์ให้ได้ด้วย ซึ่งก็ไม่ต้องใช้เครือข่ายเช่นกัน

**สี่วิธีในการนำภาพเข้ามา**
- Alt+Z ลากกรอบครอบส่วนใดก็ได้ของหน้าจอ ส่วน Ctrl+Shift+R จับภาพทั้งหน้าจอ
- Ctrl+V วางภาพจากคลิปบอร์ด วางข้อความก็ได้เช่นกัน
- Ctrl+O เปิดไฟล์ และการลากไฟล์เข้ามาในหน้าต่างก็ได้ผลเหมือนกัน
- คลิกขวาที่ภาพใน File Explorer แล้วเปิดด้วย Glyfo หรือส่งมาจากแผงแชร์ของ Windows (Photos,
  Snipping Tool, เว็บเบราว์เซอร์)

**สิ่งที่คุณจะได้กลับมา**
- ข้อความที่อ่านได้วางอยู่ข้างภาพ เรียงตามลำดับเดิมของเนื้อหา
- คัดลอกด้วยคลิกเดียว หรือจะให้ภาพที่เพิ่งจับคัดลอกตัวเองทันทีที่อ่านเสร็จก็ได้
- อ่านออกเสียงด้วยเสียงใดก็ได้ที่ติดตั้งอยู่บนเครื่อง
- “ลบการขึ้นบรรทัด” จะเชื่อมบรรทัดที่ถูกตัดแข็ง ๆ กลับเป็นย่อหน้า ส่วน “ลบช่องว่าง” จะลบช่องว่างทุกตัว
  ซึ่งเป็นสิ่งที่ข้อความภาษาจีน ญี่ปุ่น และเกาหลีต้องการหลังการอ่าน
- คิวอาร์โค้ดและบาร์โค้ดจากภาพเดียวกัน
- ประวัติผลลัพธ์ล่าสุด ภาพที่จับไว้เมื่อสองนาทีก่อนยังอยู่ห่างแค่คลิกเดียว

**คุณเป็นคนกำหนดว่าจะให้อ่านอย่างไร**
- เลือกภาษาที่จะใช้อ่านจากชุดภาษาที่ติดตั้งไว้ใน Windows หรือปล่อยให้ Glyfo เลือกเอง
- มีตัวเลือกหนึ่งที่แก้ข้อผิดพลาดคลาสสิกของ OCR ที่อ่าน v1.6.5 เป็น vl.6.5 โดยตัว l หรือ I
  จะกลายเป็นเลข 1 เฉพาะตรงที่มีตัวคั่นและตัวเลขอยู่ข้าง ๆ เท่านั้น html5 และ IPv6 จึงไม่ถูกแตะต้อง
- ย่อให้พอดีหน้าต่างหรือดูขนาดจริง จะอ่านทั้งภาพหรืออ่านเฉพาะส่วนที่เลือกไว้ก็ได้

**มันไม่เกะกะ**
- ปิดหน้าต่างแล้ว Glyfo จะยังอยู่ในพื้นที่แจ้งเตือน และปุ่มลัดสำหรับจับภาพก็ยังใช้ได้ ต้องเลือก “ออก”
  จากตรงนั้นจึงจะปิดจริง ๆ พฤติกรรมนี้เองก็เป็นตัวเลือกที่เปิดปิดได้
- Glyfo เริ่มทำงานพร้อม Windows แล้วลงไปอยู่ในพื้นที่แจ้งเตือนโดยไม่เปิดหน้าต่างเลยก็ได้
  ปุ่มลัดจึงพร้อมใช้ตั้งแต่วินาทีที่คุณลงชื่อเข้าใช้ ค่าเริ่มต้นคือปิดไว้ คุณเป็นคนเปิดเอง
- เมื่อหน้าต่างถูกซ่อนอยู่ การแจ้งเตือนจะแสดงบรรทัดแรกของข้อความที่เพิ่งอ่านได้

**ภาษา**
หน้าตาโปรแกรมมีให้เลือก 33 ภาษา และจะเดินตามการตั้งค่าภาษาของ Windows ส่วนการอ่านข้อความจะใช้ชุดภาษา OCR
ที่ติดตั้งอยู่บนเครื่อง เพิ่มชุดใหม่ได้ที่ การตั้งค่า › เวลาและภาษา › ภาษาและภูมิภาค

**ความเป็นส่วนตัว**
Glyfo ไม่เปิดการเชื่อมต่อเครือข่ายใด ๆ ภาพ ข้อความที่อ่านได้ และคำแปล จะไม่ออกจากเครื่องของคุณเลย
ไม่มีบัญชีผู้ใช้ ไม่มีการเก็บข้อมูลการใช้งาน ไม่มีโฆษณา

### คุณสมบัติของผลิตภัณฑ์

- จับภาพส่วนใดก็ได้ของหน้าจอด้วย Alt+Z แล้วอ่านข้อความทันที โดยไม่ต้องออกจากแอปที่คุณกำลังอ่านอยู่
- ทำงานบน OCR ที่มีมาในตัว Windows และบนเครื่อง Copilot+ ยังเสริมด้วยโมเดลรู้จำข้อความบนเครื่อง
- เปิดไฟล์ วางจากคลิปบอร์ด ลากมาวาง หรือรับมาจากแผงแชร์ของ Windows
- อ่านคิวอาร์โค้ดและบาร์โค้ดจากภาพเดียวกัน
- แปลบนเครื่อง Copilot+ โดยประมวลผลบนเครื่อง ไม่ต้องต่อเครือข่าย
- อ่านผลลัพธ์ออกเสียงด้วยเสียงใดก็ได้ที่ติดตั้งอยู่บนเครื่อง
- ลบการขึ้นบรรทัดและช่องว่าง คือการจัดเก็บกวาดที่ข้อความ CJK ต้องการหลังการอ่าน
- อยู่ต่อในพื้นที่แจ้งเตือน ปุ่มลัดจึงยังใช้ได้แม้ปิดหน้าต่างไปแล้ว
- หน้าตาโปรแกรม 33 ภาษา เดินตามการตั้งค่าภาษาของ Windows
- ไม่ต่ออินเทอร์เน็ต สิ่งที่คุณอ่านจะไม่ออกไปจากเครื่องของคุณ

### คำบรรยายภาพหน้าจอ

1. `01-text-from-a-page.png` — ข้อความที่อ่านได้วางอยู่ข้างภาพตามลำดับที่อ่านมา พร้อมให้คัดลอก ฟังเสียงอ่าน หรือเชื่อมกลับเป็นย่อหน้า
2. `02-any-language.png` — เลือกภาษาที่จะใช้อ่านได้จากชุดภาษาที่ติดตั้งไว้ใน Windows หรือจะให้ Glyfo ตัดสินใจเองก็ได้
3. `03-qr-and-barcodes.png` — คิวอาร์โค้ดและบาร์โค้ดถูกอ่านจากภาพเดียวกันนี้ ไม่ต้องหาแอปสแกนอีกตัว
4. `04-history.png` — ผลลัพธ์ล่าสุดยังอยู่ในประวัติ ภาพที่จับไว้เมื่อไม่กี่นาทีก่อนจึงยังห่างแค่คลิกเดียว
5. `05-settings.png` — ภาษาของหน้าตาโปรแกรม การเริ่มพร้อม Windows การอยู่ต่อในพื้นที่แจ้งเตือน และตัวแก้ที่กัน v1.6.5 ไม่ให้กลายเป็น vl.6.5

### คำค้นหา

`ข้อความจากรูปภาพ`, `ภาพหน้าจอเป็นข้อความ`, `รู้จำข้อความ`, `อ่านคิวอาร์โค้ด`,
`สแกนบาร์โค้ด`, `ดึงข้อความ`, `อ่านออกเสียง`

---

## Bahasa Indonesia — Indonesian

### Deskripsi singkat

Glyfo menarik teks dari apa pun yang Anda lihat: tangkapan layar, foto sebuah halaman, dokumen hasil
pindai, salindia presentasi, satu bingkai video. Tekan Alt+Z lalu tarik kotak pada bagian layar mana
pun, atau buka berkas, tempel dari papan klip, kirim gambar dari aplikasi lain. Teks yang terbaca
muncul di samping gambar, siap disalin, didengarkan, atau dirapikan. Glyfo juga membaca kode QR dan
kode batang dari gambar yang sama, dan pada PC Copilot+ ia menerjemahkan hasilnya. Semuanya terjadi
di PC Anda sendiri — Glyfo tidak membuka koneksi internet sama sekali. Gratis, tanpa iklan dan tanpa
pembelian.

### Deskripsi

Glyfo mengubah gambar berisi tulisan menjadi teks yang bisa Anda pakai.

Pembacaannya berjalan di atas OCR bawaan Windows: tanpa akun, tanpa unggahan, tanpa menunggu server.
Pada PC Copilot+, Glyfo juga memakai model pengenalan teks yang berjalan di perangkat untuk gambar
yang sulit dan dapat menerjemahkan hasilnya — ini pun tanpa jaringan.

**Empat cara memasukkan gambar**
- Alt+Z menarik kotak pada bagian layar mana pun. Ctrl+Shift+R mengambil seluruh layar.
- Ctrl+V menempelkan gambar dari papan klip, teks juga bisa.
- Ctrl+O membuka berkas; menyeretnya ke jendela sama saja hasilnya.
- Klik kanan sebuah gambar di File Explorer lalu buka dengan Glyfo, atau kirim lewat panel berbagi
  Windows (Photos, Snipping Tool, peramban).

**Apa yang Anda dapatkan**
- Teks yang terbaca di samping gambar, dalam urutan tata letak aslinya.
- Salin dengan satu klik, atau biarkan hasil tangkapan menyalin dirinya sendiri begitu selesai.
- Pembacaan nyaring dengan suara mana pun yang terpasang di PC.
- “Hapus pemenggalan baris” menyambung kembali baris yang terpotong menjadi paragraf, dan “Hapus
  spasi” menghapus setiap spasi — persis yang dibutuhkan teks Tionghoa, Jepang, dan Korea setelah
  dibaca.
- Kode QR dan kode batang dari gambar yang sama.
- Riwayat hasil terbaru: tangkapan dua menit lalu masih berjarak satu klik.

**Anda yang menentukan cara membacanya**
- Pilih bahasa pembacaan dari paket bahasa yang terpasang di Windows, atau serahkan pada Glyfo.
- Sebuah opsi memperbaiki kesalahan klasik OCR yang membaca v1.6.5 sebagai vl.6.5: huruf l atau I
  berubah menjadi angka 1 hanya di tempat yang bersebelahan dengan pemisah dan angka, jadi html5 dan
  IPv6 tetap utuh.
- Sesuaikan dengan jendela atau tampilkan pada ukuran asli; baca seluruh gambar atau hanya bagian
  yang dipilih.

**Ia tidak menghalangi**
- Menutup jendela membuat Glyfo tetap tinggal di area pemberitahuan, dan pintasan tangkapan tetap
  bekerja. “Keluar” dari sana barulah benar-benar menutupnya. Perilaku ini sendiri pun sebuah
  pengaturan.
- Glyfo bisa ikut menyala bersama Windows dan langsung masuk ke area pemberitahuan tanpa membuka
  jendela, sehingga pintasan siap sejak Anda masuk. Mati secara bawaan; Anda sendiri yang
  menyalakannya.
- Saat jendela tersembunyi, sebuah pemberitahuan menampilkan baris pertama dari yang baru terbaca.

**Bahasa**
Antarmukanya tersedia dalam 33 bahasa dan mengikuti pengaturan bahasa Windows. Pembacaan memakai
paket bahasa OCR yang terpasang di PC — tambahkan lainnya lewat Settings › Time & language ›
Language & region.

**Privasi**
Glyfo tidak membuka koneksi jaringan. Gambar, teks yang terbaca, dan terjemahan tidak pernah keluar
dari PC Anda. Tanpa akun, tanpa telemetri, tanpa iklan.

### Fitur produk

- Tangkap bagian layar mana pun dengan Alt+Z dan baca saat itu juga, tanpa keluar dari aplikasi yang sedang Anda baca
- Berjalan di atas OCR bawaan Windows, dan pada PC Copilot+ ditambah model pengenalan di perangkat
- Buka berkas, tempel dari papan klip, seret dan lepas, atau terima lewat panel berbagi Windows
- Membaca kode QR dan kode batang dari gambar yang sama
- Menerjemahkan pada PC Copilot+, di perangkat, tanpa akses jaringan
- Membacakan hasilnya dengan suara mana pun yang terpasang di PC
- Hapus pemenggalan baris dan spasi — perapian yang dibutuhkan teks CJK setelah dibaca
- Tetap tinggal di area pemberitahuan, jadi pintasan bekerja walau jendela sudah ditutup
- Antarmuka dalam 33 bahasa, mengikuti pengaturan bahasa Windows
- Tidak menyambung ke internet: apa yang Anda baca tidak keluar dari PC Anda

### Keterangan tangkapan layar

1. `01-text-from-a-page.png` — Teks yang terbaca berdiri di samping gambar dalam urutan saat dibaca: siap disalin, didengarkan, atau disambung kembali menjadi paragraf.
2. `02-any-language.png` — Bahasa pembacaan dipilih dari paket bahasa yang terpasang di Windows — atau Glyfo yang menentukannya sendiri.
3. `03-qr-and-barcodes.png` — Kode QR dan kode batang dibaca dari gambar yang sama; aplikasi pemindai terpisah tidak diperlukan.
4. `04-history.png` — Hasil terbaru tersimpan di riwayat, jadi tangkapan beberapa menit lalu masih berjarak satu klik.
5. `05-settings.png` — Bahasa antarmuka, menyala bersama Windows, tinggal di area pemberitahuan, dan koreksi yang menjaga v1.6.5 tidak menjadi vl.6.5.

### Istilah pencarian

`teks dari gambar`, `tangkapan layar ke teks`, `pengenalan teks`, `baca kode QR`,
`pemindai barcode`, `ekstrak teks`, `baca dengan suara`

---

## Bahasa Melayu — Malay

### Penerangan ringkas

Glyfo mengeluarkan teks daripada apa sahaja yang anda lihat: tangkapan skrin, foto sesebuah halaman,
dokumen yang diimbas, slaid pembentangan, satu bingkai video. Tekan Alt+Z lalu bingkaikan mana-mana
bahagian skrin, atau buka fail, tampal daripada papan keratan, hantar imej dari aplikasi lain. Teks
yang dikenali muncul di sebelah imej, sedia untuk disalin, didengar atau dikemaskan. Glyfo turut
membaca kod QR dan kod bar daripada imej yang sama, dan pada PC Copilot+ ia menterjemah hasilnya.
Semuanya berlaku pada PC anda sendiri — Glyfo tidak membuka sebarang sambungan internet. Percuma,
tanpa iklan dan tanpa pembelian.

### Penerangan

Glyfo menukar imej bertulisan menjadi teks yang boleh anda guna.

Pengecaman berjalan di atas OCR yang sedia ada dalam Windows: tiada akaun, tiada muat naik, tiada
menunggu pelayan. Pada PC Copilot+, Glyfo turut menggunakan model pengecaman teks yang berjalan pada
peranti untuk imej yang sukar dan boleh menterjemah hasilnya — itu pun tanpa rangkaian.

**Empat cara memasukkan imej**
- Alt+Z membingkaikan mana-mana bahagian skrin. Ctrl+Shift+R mengambil keseluruhan skrin.
- Ctrl+V menampal imej daripada papan keratan, teks juga boleh.
- Ctrl+O membuka fail; menyeretnya ke dalam tetingkap memberi hasil yang sama.
- Klik kanan sesebuah imej dalam File Explorer lalu buka dengan Glyfo, atau hantar melalui panel
  perkongsian Windows (Photos, Snipping Tool, pelayar web).

**Apa yang anda dapat**
- Teks yang dikenali di sebelah imej, mengikut susunan asalnya.
- Salin dengan satu klik, atau biarkan tangkapan menyalin dirinya sendiri sebaik sahaja siap.
- Bacaan kuat dengan mana-mana suara yang dipasang pada PC.
- “Buang pemisah baris” menyambung semula baris yang terputus menjadi perenggan, dan “Buang ruang”
  membuang setiap ruang — itulah yang diperlukan teks Cina, Jepun dan Korea selepas dikenali.
- Kod QR dan kod bar daripada imej yang sama.
- Sejarah hasil terkini: tangkapan dua minit lalu masih sejauh satu klik.

**Anda yang menentukan cara ia membaca**
- Pilih bahasa pengecaman daripada pakej bahasa yang dipasang dalam Windows, atau serahkan kepada
  Glyfo.
- Satu pilihan membetulkan ralat OCR klasik yang membaca v1.6.5 sebagai vl.6.5: huruf l atau I
  bertukar menjadi angka 1 hanya di tempat yang bersebelahan dengan pemisah dan digit, jadi html5
  dan IPv6 kekal seperti asal.
- Muatkan pada tetingkap atau lihat pada saiz sebenar; kenali keseluruhan imej atau bahagian yang
  dipilih sahaja.

**Ia tidak menghalang**
- Menutup tetingkap membuatkan Glyfo kekal di kawasan pemberitahuan, dan pintasan tangkapan terus
  berfungsi. “Keluar” dari situ barulah menutupnya betul-betul. Kelakuan ini sendiri pun satu
  tetapan.
- Glyfo boleh dimulakan bersama Windows dan terus masuk ke kawasan pemberitahuan tanpa membuka
  tetingkap, jadi pintasan berfungsi sebaik sahaja anda log masuk. Dimatikan secara lalai; anda yang
  menghidupkannya.
- Ketika tetingkap tersembunyi, satu pemberitahuan menunjukkan baris pertama yang baru dikenali.

**Bahasa**
Antara mukanya tersedia dalam 33 bahasa dan mengikut tetapan bahasa Windows. Pengecaman menggunakan
pakej bahasa OCR yang dipasang pada PC — tambah lagi melalui Settings › Time & language › Language &
region.

**Privasi**
Glyfo tidak membuka sambungan rangkaian. Imej, teks yang dikenali dan terjemahan tidak pernah keluar
daripada PC anda. Tiada akaun, tiada telemetri, tiada iklan.

### Ciri produk

- Tangkap mana-mana bahagian skrin dengan Alt+Z dan kenali serta-merta, tanpa keluar daripada aplikasi yang sedang anda baca
- Berjalan di atas OCR terbina dalam Windows, dan pada PC Copilot+ ditambah model pengecaman pada peranti
- Buka fail, tampal daripada papan keratan, seret dan lepas, atau terima melalui panel perkongsian Windows
- Membaca kod QR dan kod bar daripada imej yang sama
- Menterjemah pada PC Copilot+, pada peranti, tanpa akses rangkaian
- Membacakan hasilnya dengan mana-mana suara yang dipasang pada PC
- Buang pemisah baris dan ruang — pengemasan yang diperlukan teks CJK selepas dikenali
- Kekal di kawasan pemberitahuan, jadi pintasan berfungsi walaupun tetingkap sudah ditutup
- Antara muka dalam 33 bahasa, mengikut tetapan bahasa Windows
- Tidak menyambung ke internet: apa yang anda kenali tidak keluar daripada PC anda

### Kapsyen tangkapan skrin

1. `01-text-from-a-page.png` — Teks yang dikenali berdiri di sebelah imej mengikut susunan ia dibaca: sedia untuk disalin, didengar atau disambung semula menjadi perenggan.
2. `02-any-language.png` — Bahasa pengecaman dipilih daripada pakej bahasa yang dipasang dalam Windows — atau Glyfo menentukannya sendiri.
3. `03-qr-and-barcodes.png` — Kod QR dan kod bar dibaca daripada imej yang sama; aplikasi pengimbas berasingan tidak diperlukan.
4. `04-history.png` — Hasil terkini kekal dalam sejarah, jadi tangkapan beberapa minit lalu masih sejauh satu klik.
5. `05-settings.png` — Bahasa antara muka, mula bersama Windows, kekal di kawasan pemberitahuan, dan pembetulan yang menghalang v1.6.5 daripada menjadi vl.6.5.

### Istilah carian

`teks daripada imej`, `tangkapan skrin ke teks`, `pengecaman teks`, `baca kod QR`,
`pengimbas kod bar`, `ekstrak teks`, `baca dengan suara`

---

## Filipino

### Maikling paglalarawan

Kinukuha ng Glyfo ang teksto mula sa kahit anong nakikita mo: isang screenshot, litrato ng isang
pahina, na-scan na dokumento, slide sa presentasyon, isang frame ng video. Pindutin ang Alt+Z at
kahunan ang alinmang bahagi ng screen, o magbukas ng file, mag-paste mula sa clipboard, magpadala ng
larawan mula sa ibang app. Lumalabas ang nabasang teksto sa tabi ng larawan, handa nang kopyahin,
pakinggan o linisin. Binabasa rin ng Glyfo ang mga QR code at barcode mula sa parehong larawan, at
sa isang Copilot+ PC ay isinasalin pa nito ang resulta. Lahat ay nangyayari sa sarili mong PC — hindi
kumakabit ang Glyfo sa internet kahit kailan. Libre, walang ad at walang bibilhin.

### Paglalarawan

Ginagawang magamit na teksto ng Glyfo ang mga larawang may nakasulat.

Umaandar ang pagbasa sa OCR na nasa Windows na: walang account, walang ia-upload, walang hinihintay
na server. Sa isang Copilot+ PC, ginagamit din ng Glyfo ang modelo ng pagkilala ng teksto na tumatakbo
sa mismong device para sa mahihirap na larawan, at kayang isalin ang resulta — wala ring network
dito.

**Apat na paraan para makapasok ang larawan**
- Kinakahunan ng Alt+Z ang alinmang bahagi ng screen. Kinukuha ng Ctrl+Shift+R ang buong screen.
- Nagpe-paste ang Ctrl+V ng larawan mula sa clipboard, pati teksto.
- Nagbubukas ng file ang Ctrl+O; ganoon din kung kakaladkarin mo ito papasok sa window.
- I-right-click ang isang larawan sa File Explorer at buksan sa Glyfo, o ipadala mula sa share panel
  ng Windows (Photos, Snipping Tool, browser).

**Ano ang makukuha mo**
- Ang nabasang teksto sa tabi ng larawan, sa pagkakasunod-sunod ng orihinal na ayos nito.
- Kopyahin sa isang click, o hayaang kusang kumopya ang isang capture pagkatapos nito.
- Pagbabasa nang malakas gamit ang alinmang boses na naka-install sa PC.
- Pinagdurugtong muli ng “Alisin ang mga line break” ang matitigas na putol ng linya pabalik sa mga
  talata, at inaalis ng “Alisin ang mga espasyo” ang bawat espasyo — iyon mismo ang kailangan ng
  tekstong Tsino, Hapon at Koreano pagkatapos basahin.
- Mga QR code at barcode mula sa parehong larawan.
- Kasaysayan ng mga huling resulta: isang click pa rin ang layo ng capture kanina lang.

**Ikaw ang nagpapasya kung paano ito magbasa**
- Piliin ang wikang gagamitin sa pagbasa mula sa mga language pack na naka-install sa Windows, o
  ipaubaya sa Glyfo.
- May opsyong nag-aayos sa klasikong mali ng OCR kung saan nababasa ang v1.6.5 bilang vl.6.5: nagiging
  1 lamang ang l o I kung may separator at digit sa tabi nito, kaya nananatiling buo ang html5 at
  IPv6.
- Ikasya sa window o tingnan sa totoong laki; basahin ang buong larawan o ang napiling bahagi lang.

**Hindi ito nakakaabala**
- Kapag isinara mo ang window, nananatili ang Glyfo sa notification area at gumagana pa rin ang
  shortcut sa pagkuha. Ang “Lumabas” doon ang tunay na nagsasara nito. Isa ring setting ang ugaling
  ito.
- Puwedeng bumukas ang Glyfo kasabay ng Windows at dumiretso sa notification area nang hindi
  nagbubukas ng window, kaya gumagana ang shortcut mula sa sandaling mag-sign in ka. Naka-off bilang
  default; ikaw ang magbubukas nito.
- Kapag nakatago ang window, ipinapakita ng isang notification ang unang linya ng katatapos basahin.

**Mga wika**
Makukuha ang interface sa 33 wika at sumusunod ito sa setting ng wika ng Windows. Ginagamit ng
pagbasa ang mga OCR language pack na nasa PC — magdagdag pa sa Settings › Time & language › Language
& region.

**Privacy**
Walang binubuksang koneksyon sa network ang Glyfo. Hindi kailanman umaalis sa PC mo ang mga larawan,
ang nabasang teksto at ang mga salin. Walang account, walang telemetry, walang ad.

### Mga tampok ng produkto

- Kunan ang alinmang bahagi ng screen gamit ang Alt+Z at basahin agad, nang hindi umaalis sa app na binabasa mo
- Umaandar sa OCR na nakapaloob sa Windows, at sa mga Copilot+ PC ay may dagdag na modelo ng pagkilala sa device
- Magbukas ng file, mag-paste mula sa clipboard, mag-drag and drop o tumanggap mula sa share panel ng Windows
- Binabasa ang mga QR code at barcode mula sa parehong larawan
- Nagsasalin sa mga Copilot+ PC, sa mismong device, nang walang access sa network
- Binabasa nang malakas ang resulta gamit ang alinmang boses na naka-install sa PC
- Pag-alis ng mga line break at espasyo — ang paglilinis na kailangan ng tekstong CJK pagkatapos basahin
- Nananatili sa notification area, kaya gumagana ang shortcut kahit sarado na ang window
- Interface sa 33 wika, sumusunod sa setting ng wika ng Windows
- Hindi kumakabit sa internet: hindi umaalis sa PC mo ang binabasa mo

### Mga caption ng screenshot

1. `01-text-from-a-page.png` — Nakatayo ang nabasang teksto sa tabi ng larawan sa pagkakasunod-sunod na binasa ito: handa nang kopyahin, pakinggan o pagdugtungin pabalik sa mga talata.
2. `02-any-language.png` — Ang wika sa pagbasa ay pipiliin mula sa mga language pack na naka-install sa Windows — o ang Glyfo na mismo ang magpapasya.
3. `03-qr-and-barcodes.png` — Binabasa ang mga QR code at barcode mula sa parehong larawan; hindi na kailangan ng hiwalay na scanner app.
4. `04-history.png` — Nananatili sa kasaysayan ang mga huling resulta, kaya isang click pa rin ang layo ng capture mula ilang minuto ang nakalipas.
5. `05-settings.png` — Wika ng interface, pagbukas kasabay ng Windows, pananatili sa notification area, at ang pagwawastong pumipigil sa v1.6.5 na maging vl.6.5.

### Mga termino sa paghahanap

`teksto mula sa larawan`, `screenshot sa teksto`, `pagkilala ng teksto`, `basahin ang QR code`,
`barcode scanner`, `kunin ang teksto`, `basahin nang malakas`

---

## हिन्दी — Hindi

### संक्षिप्त विवरण

Glyfo हर उस चीज़ से टेक्स्ट निकाल लेता है जो आपको दिखती है: स्क्रीनशॉट, किसी पन्ने की फ़ोटो, स्कैन
किया दस्तावेज़, प्रेज़ेंटेशन की स्लाइड, वीडियो का कोई फ़्रेम। Alt+Z दबाइए और स्क्रीन के किसी भी हिस्से
पर फ़्रेम खींचिए, या फ़ाइल खोलिए, क्लिपबोर्ड से पेस्ट कीजिए, किसी दूसरे ऐप से तस्वीर भेजिए। पहचाना गया
टेक्स्ट तस्वीर के बगल में आ जाता है — कॉपी करने, सुनने या साफ़ करने के लिए तैयार। Glyfo उसी तस्वीर से
QR कोड और बारकोड भी पढ़ लेता है, और Copilot+ PC पर नतीजे का अनुवाद भी कर देता है। सब कुछ आपके अपने PC
पर होता है — Glyfo इंटरनेट से कोई कनेक्शन नहीं बनाता। मुफ़्त, बिना विज्ञापन और बिना किसी ख़रीदारी के।

### विवरण

Glyfo टेक्स्ट वाली तस्वीरों को ऐसे टेक्स्ट में बदल देता है जिसे आप काम में ले सकें।

पहचान Windows में पहले से मौजूद OCR पर चलती है: न कोई खाता, न कुछ अपलोड, न किसी सर्वर का इंतज़ार।
Copilot+ PC पर Glyfo मुश्किल तस्वीरों के लिए डिवाइस पर ही चलने वाले टेक्स्ट पहचान मॉडल का भी इस्तेमाल
करता है और नतीजे का अनुवाद कर सकता है — यह भी बिना नेटवर्क के।

**तस्वीर अंदर लाने के चार तरीके**
- Alt+Z स्क्रीन के किसी भी हिस्से पर फ़्रेम खींचता है। Ctrl+Shift+R पूरी स्क्रीन ले लेता है।
- Ctrl+V क्लिपबोर्ड से तस्वीर चिपकाता है, टेक्स्ट भी।
- Ctrl+O फ़ाइल खोलता है; उसे विंडो में खींचकर छोड़ना भी उतना ही काम करता है।
- File Explorer में किसी तस्वीर पर दायाँ क्लिक करके उसे Glyfo से खोलिए, या Windows के शेयर पैनल से
  भेजिए (Photos, Snipping Tool, ब्राउज़र)।

**आपको क्या वापस मिलता है**
- पहचाना गया टेक्स्ट तस्वीर के बगल में, उसी क्रम में जिसमें वह सजा हुआ था।
- एक क्लिक में कॉपी, या कैप्चर पूरा होते ही उसे ख़ुद-ब-ख़ुद कॉपी हो जाने दीजिए।
- PC पर लगी किसी भी आवाज़ से ज़ोर से पढ़कर सुनाना।
- “लाइन ब्रेक हटाएँ” कड़े लाइन ब्रेक को दोबारा पैराग्राफ़ में जोड़ देता है और “स्पेस हटाएँ” हर स्पेस मिटा
  देता है — पहचान के बाद चीनी, जापानी और कोरियाई टेक्स्ट को यही चाहिए होता है।
- उसी तस्वीर से QR कोड और बारकोड।
- हाल के नतीजों का इतिहास: दो मिनट पहले लिया गया कैप्चर अब भी एक क्लिक की दूरी पर है।

**कैसे पढ़ना है, यह आप तय करते हैं**
- Windows में लगे भाषा पैकों में से पहचान की भाषा चुनिए, या Glyfo पर छोड़ दीजिए।
- एक विकल्प OCR की उस पुरानी ग़लती को सुधारता है जिसमें v1.6.5 को vl.6.5 पढ़ लिया जाता है: l या I तभी
  1 बनता है जब उसके बगल में कोई विभाजक और अंक हो, इसलिए html5 और IPv6 जस के तस रहते हैं।
- विंडो में फ़िट कीजिए या असली आकार में देखिए; पूरी तस्वीर पहचानिए या सिर्फ़ चुना हुआ हिस्सा।

**यह रास्ते में नहीं आता**
- विंडो बंद करने पर Glyfo सूचना क्षेत्र में बना रहता है और कैप्चर का शॉर्टकट चलता रहता है। वहाँ से
  “बाहर निकलें” चुनने पर ही यह सचमुच बंद होता है। यह व्यवहार भी अपने आप में एक सेटिंग है।
- Glyfo Windows के साथ शुरू होकर बिना कोई विंडो खोले सीधे सूचना क्षेत्र में जा सकता है, ताकि साइन इन
  करते ही शॉर्टकट काम करने लगे। डिफ़ॉल्ट रूप से बंद; इसे आप ख़ुद चालू करते हैं।
- विंडो छिपी हो तो एक सूचना अभी-अभी पहचाने गए टेक्स्ट की पहली पंक्ति दिखा देती है।

**भाषाएँ**
इंटरफ़ेस 33 भाषाओं में उपलब्ध है और Windows की भाषा सेटिंग के पीछे चलता है। पहचान PC पर लगे OCR भाषा
पैकों का इस्तेमाल करती है — और जोड़ने के लिए Settings › Time & language › Language & region पर जाइए।

**निजता**
Glyfo कोई नेटवर्क कनेक्शन नहीं बनाता। तस्वीरें, पहचाना गया टेक्स्ट और अनुवाद कभी आपके PC से बाहर नहीं
जाते। न कोई खाता, न टेलीमेट्री, न विज्ञापन।

### उत्पाद की विशेषताएँ

- Alt+Z से स्क्रीन का कोई भी हिस्सा कैप्चर कीजिए और उसी वक़्त पहचानिए, जिस ऐप में पढ़ रहे थे उससे बाहर निकले बिना
- Windows में बने OCR पर चलता है, और Copilot+ PC पर डिवाइस पर ही चलने वाले पहचान मॉडल के साथ
- फ़ाइल खोलिए, क्लिपबोर्ड से पेस्ट कीजिए, खींचकर छोड़िए या Windows के शेयर पैनल से पाइए
- उसी तस्वीर से QR कोड और बारकोड पढ़ता है
- Copilot+ PC पर डिवाइस पर ही अनुवाद करता है, बिना किसी नेटवर्क पहुँच के
- PC पर लगी किसी भी आवाज़ से नतीजा ज़ोर से पढ़कर सुनाता है
- लाइन ब्रेक और स्पेस हटाना — पहचान के बाद CJK टेक्स्ट को जिस सफ़ाई की ज़रूरत होती है
- सूचना क्षेत्र में बना रहता है, इसलिए विंडो बंद करने के बाद भी शॉर्टकट चलता है
- 33 भाषाओं में इंटरफ़ेस, Windows की भाषा सेटिंग के अनुसार
- इंटरनेट से नहीं जुड़ता: आप जो पहचानते हैं वह आपके PC से बाहर नहीं जाता

### स्क्रीनशॉट कैप्शन

1. `01-text-from-a-page.png` — पहचाना गया टेक्स्ट तस्वीर के बगल में उसी क्रम में खड़ा रहता है जिसमें उसे पढ़ा गया: कॉपी करने, सुनने या दोबारा पैराग्राफ़ में जोड़ने के लिए तैयार।
2. `02-any-language.png` — पहचान की भाषा Windows में लगे भाषा पैकों में से चुनी जाती है — या Glyfo ख़ुद तय कर लेता है।
3. `03-qr-and-barcodes.png` — QR कोड और बारकोड उसी तस्वीर से पढ़े जाते हैं; अलग से कोई स्कैनर ऐप नहीं चाहिए।
4. `04-history.png` — हाल के नतीजे इतिहास में बने रहते हैं, इसलिए कुछ मिनट पहले लिया कैप्चर अब भी एक क्लिक की दूरी पर है।
5. `05-settings.png` — इंटरफ़ेस की भाषा, Windows के साथ शुरू होना, सूचना क्षेत्र में बने रहना, और वह सुधार जो v1.6.5 को vl.6.5 बनने से रोकता है।

### खोज शब्द

`छवि से टेक्स्ट`, `स्क्रीनशॉट से टेक्स्ट`, `टेक्स्ट पहचान`, `QR कोड रीडर`,
`बारकोड स्कैनर`, `टेक्स्ट निकालें`, `ज़ोर से पढ़ें`

---

## বাংলা — Bengali

### সংক্ষিপ্ত বিবরণ

আপনি যা কিছু দেখতে পান, Glyfo তার ভেতর থেকে লেখাটা তুলে আনে: স্ক্রিনশট, কোনো পাতার ছবি, স্ক্যান করা
নথি, উপস্থাপনার স্লাইড, ভিডিওর একটা ফ্রেম। Alt+Z চাপুন আর পর্দার যেকোনো অংশে একটা ফ্রেম টানুন, কিংবা
ফাইল খুলুন, ক্লিপবোর্ড থেকে পেস্ট করুন, অন্য অ্যাপ থেকে ছবি পাঠান। শনাক্ত হওয়া লেখা ছবির পাশেই এসে
দাঁড়ায় — কপি করা, শুনে নেওয়া বা গুছিয়ে নেওয়ার জন্য তৈরি। একই ছবি থেকে Glyfo QR কোড আর বারকোডও পড়ে
নেয়, আর Copilot+ পিসিতে ফলাফলটা অনুবাদও করে দেয়। সবকিছু ঘটে আপনার নিজের পিসিতেই — Glyfo ইন্টারনেটে
কোনো সংযোগই তৈরি করে না। বিনামূল্যে, বিজ্ঞাপন ছাড়া এবং কোনো কেনাকাটা ছাড়া।

### বিবরণ

লেখা আছে এমন ছবিকে Glyfo এমন লেখায় বদলে দেয় যা আপনি কাজে লাগাতে পারেন।

শনাক্তকরণ চলে Windows-এর ভেতরেই থাকা OCR-এর উপর: কোনো অ্যাকাউন্ট নেই, কিছু আপলোড করা নেই, সার্ভারের
জন্য অপেক্ষাও নেই। Copilot+ পিসিতে কঠিন ছবিগুলোর জন্য Glyfo ডিভাইসেই চলা টেক্সট শনাক্তকরণ মডেলটিও
কাজে লাগায় এবং ফলাফল অনুবাদ করতে পারে — সেটাও নেটওয়ার্ক ছাড়াই।

**ছবি ভেতরে আনার চারটি উপায়**
- Alt+Z পর্দার যেকোনো অংশে ফ্রেম টানে। Ctrl+Shift+R পুরো পর্দাটাই নেয়।
- Ctrl+V ক্লিপবোর্ড থেকে ছবি পেস্ট করে, লেখাও।
- Ctrl+O ফাইল খোলে; উইন্ডোর ভেতরে টেনে ছেড়ে দিলেও একই কাজ হয়।
- File Explorer-এ কোনো ছবিতে ডান ক্লিক করে সেটি Glyfo দিয়ে খুলুন, অথবা Windows-এর শেয়ার প্যানেল থেকে
  পাঠান (Photos, Snipping Tool, ব্রাউজার)।

**আপনি কী ফেরত পান**
- শনাক্ত হওয়া লেখা ছবির পাশে, মূল বিন্যাসে যে ক্রমে ছিল সেই ক্রমেই।
- এক ক্লিকে কপি, কিংবা ক্যাপচার শেষ হওয়ামাত্র সেটিকে নিজে থেকেই কপি হতে দিন।
- পিসিতে বসানো যেকোনো কণ্ঠে জোরে পড়ে শোনানো।
- “লাইন ব্রেক সরান” শক্ত লাইন ভাঙাগুলোকে আবার অনুচ্ছেদে জুড়ে দেয় আর “স্পেস সরান” প্রতিটি ফাঁকা জায়গা
  মুছে দেয় — শনাক্তকরণের পরে চীনা, জাপানি ও কোরীয় লেখার ঠিক এটাই দরকার হয়।
- একই ছবি থেকে QR কোড আর বারকোড।
- সাম্প্রতিক ফলাফলের একটা ইতিহাস: দুই মিনিট আগের ক্যাপচারটা এখনও এক ক্লিক দূরে।

**কীভাবে পড়বে তা আপনিই ঠিক করেন**
- Windows-এ বসানো ভাষা প্যাকগুলোর মধ্য থেকে শনাক্তকরণের ভাষা বেছে নিন, অথবা Glyfo-র উপর ছেড়ে দিন।
- একটি বিকল্প OCR-এর সেই পুরোনো ভুলটা শুধরে দেয় যাতে v1.6.5 পড়া হয় vl.6.5 হিসেবে: l বা I কেবল
  তখনই 1 হয় যখন তার পাশে একটা বিভাজক আর একটা অঙ্ক থাকে, ফলে html5 আর IPv6 অক্ষত থাকে।
- উইন্ডোর মাপে বসান কিংবা আসল আকারে দেখুন; পুরো ছবি শনাক্ত করুন অথবা কেবল নির্বাচিত অংশটুকু।

**এটি পথে দাঁড়ায় না**
- উইন্ডো বন্ধ করলে Glyfo বিজ্ঞপ্তি এলাকায় থেকে যায় আর ক্যাপচারের শর্টকাটও চলতে থাকে। সেখান থেকে
  “প্রস্থান” বেছে নিলে তবেই এটি সত্যিকারের বন্ধ হয়। এই আচরণটাও নিজেই একটা সেটিং।
- Glyfo Windows-এর সঙ্গে চালু হয়ে কোনো উইন্ডো না খুলেই সরাসরি বিজ্ঞপ্তি এলাকায় চলে যেতে পারে, ফলে
  সাইন ইন করার মুহূর্ত থেকেই শর্টকাট কাজ করে। ডিফল্টভাবে বন্ধ; আপনি নিজে এটি চালু করেন।
- উইন্ডো লুকানো থাকলে একটি বিজ্ঞপ্তি সদ্য শনাক্ত হওয়া লেখার প্রথম লাইনটি দেখায়।

**ভাষা**
ইন্টারফেসটি ৩৩টি ভাষায় পাওয়া যায় এবং Windows-এর ভাষা সেটিং অনুসরণ করে। শনাক্তকরণ পিসিতে বসানো OCR
ভাষা প্যাকগুলো ব্যবহার করে — আরও যোগ করতে যান Settings › Time & language › Language & region-এ।

**গোপনীয়তা**
Glyfo কোনো নেটওয়ার্ক সংযোগ তৈরি করে না। ছবি, শনাক্ত হওয়া লেখা এবং অনুবাদ কখনোই আপনার পিসি ছেড়ে যায়
না। কোনো অ্যাকাউন্ট নেই, টেলিমেট্রি নেই, বিজ্ঞাপন নেই।

### পণ্যের বৈশিষ্ট্য

- Alt+Z দিয়ে পর্দার যেকোনো অংশ ক্যাপচার করুন আর সঙ্গে সঙ্গেই শনাক্ত করুন, যে অ্যাপে পড়ছিলেন সেখান থেকে না বেরিয়েই
- Windows-এর ভেতরে থাকা OCR-এর উপর চলে, আর Copilot+ পিসিতে ডিভাইসেই চলা শনাক্তকরণ মডেলের সঙ্গে
- ফাইল খুলুন, ক্লিপবোর্ড থেকে পেস্ট করুন, টেনে ছেড়ে দিন বা Windows-এর শেয়ার প্যানেল থেকে গ্রহণ করুন
- একই ছবি থেকে QR কোড আর বারকোড পড়ে
- Copilot+ পিসিতে ডিভাইসেই অনুবাদ করে, কোনো নেটওয়ার্ক ব্যবহার ছাড়াই
- পিসিতে বসানো যেকোনো কণ্ঠে ফলাফল জোরে পড়ে শোনায়
- লাইন ব্রেক আর স্পেস সরানো — শনাক্তকরণের পরে CJK লেখার যে গোছগাছটা দরকার
- বিজ্ঞপ্তি এলাকায় থেকে যায়, তাই উইন্ডো বন্ধ করার পরেও শর্টকাট কাজ করে
- ৩৩টি ভাষায় ইন্টারফেস, Windows-এর ভাষা সেটিং অনুযায়ী
- ইন্টারনেটে যুক্ত হয় না: আপনি যা শনাক্ত করেন তা আপনার পিসি ছেড়ে যায় না

### স্ক্রিনশটের ক্যাপশন

1. `01-text-from-a-page.png` — শনাক্ত হওয়া লেখা ছবির পাশে সেই ক্রমেই দাঁড়িয়ে থাকে যে ক্রমে সেটি পড়া হয়েছে: কপি করতে, শুনতে বা আবার অনুচ্ছেদে জুড়তে তৈরি।
2. `02-any-language.png` — শনাক্তকরণের ভাষা Windows-এ বসানো ভাষা প্যাকগুলো থেকে বেছে নেওয়া হয় — কিংবা Glyfo নিজেই ঠিক করে নেয়।
3. `03-qr-and-barcodes.png` — QR কোড আর বারকোড একই ছবি থেকেই পড়া হয়; আলাদা কোনো স্ক্যানার অ্যাপ লাগে না।
4. `04-history.png` — সাম্প্রতিক ফলাফল ইতিহাসে থেকে যায়, তাই কয়েক মিনিট আগের ক্যাপচারটাও এখনও এক ক্লিক দূরে।
5. `05-settings.png` — ইন্টারফেসের ভাষা, Windows-এর সঙ্গে চালু হওয়া, বিজ্ঞপ্তি এলাকায় থেকে যাওয়া, আর সেই সংশোধন যা v1.6.5-কে vl.6.5 হতে দেয় না।

### অনুসন্ধানের শব্দ

`ছবি থেকে টেক্সট`, `স্ক্রিনশট থেকে টেক্সট`, `টেক্সট শনাক্তকরণ`, `QR কোড রিডার`,
`বারকোড স্ক্যানার`, `টেক্সট বের করুন`, `জোরে পড়ুন`

---

## العربية — Arabic

### وصف مختصر

يستخرج Glyfo النص من كل ما تراه: لقطة شاشة، صورة لصفحة من كتاب، مستند ممسوح ضوئيًا، شريحة عرض، إطار
من مقطع فيديو. اضغط Alt+Z وحدّد إطارًا حول أي جزء من الشاشة، أو افتح ملفًا، أو الصق من الحافظة، أو
أرسل صورة من تطبيق آخر. يظهر النص المتعرَّف عليه بجوار الصورة، جاهزًا للنسخ أو الاستماع أو التنظيف.
ويقرأ Glyfo أيضًا رموز QR والباركود من الصورة نفسها، وعلى حاسوب Copilot+ يترجم النتيجة. كل ذلك يجري
على حاسوبك وحده — لا ينشئ Glyfo أي اتصال بالإنترنت. مجاني، بلا إعلانات وبلا مشتريات.

### الوصف

يحوّل Glyfo الصور التي تحمل نصًا إلى نص يمكنك استخدامه.

يعتمد التعرّف على محرك OCR المدمج في Windows: لا حساب تنشئه، ولا ملفات ترفعها، ولا انتظار لخادم.
وعلى حاسوب Copilot+ يستعين Glyfo إضافةً إلى ذلك بنموذج التعرّف على النص العامل داخل الجهاز في الصور
الصعبة، ويستطيع ترجمة النتيجة — ودون شبكة أيضًا.

**أربع طرق لإدخال صورة**
- اضغط Alt+Z لتحديد إطار حول أي جزء من الشاشة. ويلتقط Ctrl+Shift+R الشاشة كاملة.
- الصق صورة من الحافظة بالاختصار Ctrl+V، والنص كذلك.
- افتح ملفًا بالاختصار Ctrl+O؛ وسحب الملف إلى النافذة يؤدي الغرض نفسه.
- انقر بزر الفأرة الأيمن على صورة في مستكشف الملفات وافتحها بـ Glyfo، أو أرسلها من لوحة المشاركة في
  Windows (تطبيق الصور، أداة القص، المتصفح).

**ما الذي تحصل عليه**
- النص المتعرَّف عليه بجوار الصورة، بالترتيب الذي كان عليه في التخطيط الأصلي.
- النسخ بنقرة واحدة، أو دع اللقطة تنسخ نفسها بمجرد انتهائها.
- القراءة بصوت عالٍ بأي صوت مثبَّت على الحاسوب.
- خيار «إزالة فواصل الأسطر» يعيد وصل الأسطر المقطوعة إلى فقرات، و«إزالة المسافات» يمحو كل مسافة —
  وهذا تحديدًا ما تحتاجه النصوص الصينية واليابانية والكورية بعد التعرّف عليها.
- رموز QR والباركود من الصورة نفسها.
- سجلّ بالنتائج الأخيرة: اللقطة التي أخذتها قبل دقيقتين لا تزال على بعد نقرة واحدة.

**أنت من يقرر كيف يقرأ**
- اختر لغة التعرّف من بين حزم اللغات المثبَّتة في Windows، أو اترك الأمر لـ Glyfo.
- هناك خيار يصحّح خطأ OCR الشهير الذي يقرأ v1.6.5 على أنها vl.6.5: لا يتحول الحرف l أو I إلى الرقم 1
  إلا حيث يجاوره فاصل ورقم، فتبقى html5 و IPv6 على حالها.
- يمكنك ملاءمة الصورة للنافذة أو عرضها بحجمها الحقيقي؛ والتعرّف على الصورة كاملة أو على التحديد وحده.

**لا يقف في طريقك**
- عند إغلاق النافذة يبقى Glyfo في منطقة الإعلام، ويظل اختصار الالتقاط يعمل. أما «إنهاء» من هناك
  فيغلقه فعليًا. وهذا السلوك نفسه إعداد يمكنك تغييره.
- يستطيع Glyfo أن يبدأ مع Windows وأن ينتقل مباشرةً إلى منطقة الإعلام دون فتح أي نافذة، فيعمل
  الاختصار منذ لحظة تسجيل الدخول. معطَّل افتراضيًا؛ وأنت من يفعّله.
- عندما تكون النافذة مخفية، يعرض إشعارٌ أول سطر مما تم التعرّف عليه للتو.

**اللغات**
تتوفر الواجهة بـ 33 لغة وتتبع إعداد اللغة في Windows. ويستخدم التعرّف حزم لغات OCR المثبَّتة على
الحاسوب — وتضيف المزيد من الإعدادات › الوقت واللغة › اللغة والمنطقة.

**الخصوصية**
لا ينشئ Glyfo أي اتصالات شبكية. ولا تغادر الصور ولا النص المتعرَّف عليه ولا الترجمات حاسوبك أبدًا. لا
حساب، ولا قياس عن بُعد، ولا إعلانات.

### ميزات المنتج

- التقط أي جزء من الشاشة بالاختصار Alt+Z وتعرّف عليه في الحال، دون مغادرة التطبيق الذي كنت تقرأ فيه
- يعتمد على محرك OCR المدمج في Windows، وعلى حواسيب Copilot+ على نموذج التعرّف العامل داخل الجهاز
- افتح ملفًا، أو الصق من الحافظة، أو اسحب وأفلت، أو استقبل من لوحة المشاركة في Windows
- يقرأ رموز QR والباركود من الصورة نفسها
- يترجم على حواسيب Copilot+ داخل الجهاز، دون أي وصول إلى الشبكة
- يقرأ النتيجة بصوت عالٍ بأي صوت مثبَّت على الحاسوب
- إزالة فواصل الأسطر والمسافات — وهو التنظيف الذي تحتاجه نصوص CJK بعد التعرّف عليها
- يبقى في منطقة الإعلام، فيظل الاختصار يعمل حتى بعد إغلاق النافذة
- واجهة بـ 33 لغة، تتبع إعداد اللغة في Windows
- لا يتصل بالإنترنت: ما تتعرّف عليه لا يغادر حاسوبك

### تعليقات لقطات الشاشة

1. `01-text-from-a-page.png` — يقف النص المتعرَّف عليه بجوار الصورة بالترتيب الذي قُرئ به: جاهزًا للنسخ أو الاستماع أو إعادة وصله إلى فقرات.
2. `02-any-language.png` — تُختار لغة التعرّف من بين حزم اللغات المثبَّتة في Windows — أو يحددها Glyfo بنفسه.
3. `03-qr-and-barcodes.png` — تُقرأ رموز QR والباركود من الصورة نفسها؛ ولا حاجة إلى تطبيق مسح منفصل.
4. `04-history.png` — تبقى النتائج الأخيرة في السجل، فتظل لقطة أخذتها قبل دقائق على بعد نقرة واحدة.
5. `05-settings.png` — لغة الواجهة، والبدء مع Windows، والبقاء في منطقة الإعلام، والتصحيح الذي يمنع تحوّل v1.6.5 إلى vl.6.5.

### مصطلحات البحث

`نص من صورة`, `لقطة شاشة إلى نص`, `التعرف على النص`, `قارئ رمز QR`,
`ماسح الباركود`, `استخراج النص`, `قراءة بصوت عال`

---

## עברית — Hebrew

### תיאור קצר

Glyfo שולף את הטקסט מכל מה שאתם רואים: צילום מסך, תצלום של עמוד, מסמך סרוק, שקופית מצגת, פריים
מסרטון. הקישו Alt+Z וסמנו מסגרת סביב חלק כלשהו מהמסך, או פתחו קובץ, הדביקו מהלוח, שלחו תמונה
מאפליקציה אחרת. הטקסט שזוהה מופיע לצד התמונה, מוכן להעתקה, להאזנה או לניקוי. Glyfo קורא גם קודי QR
וברקודים מאותה תמונה עצמה, ובמחשב Copilot+ אף מתרגם את התוצאה. הכול מתרחש במחשב שלכם — Glyfo אינו
יוצר שום חיבור לאינטרנט. חינם, בלי פרסומות ובלי רכישות.

### תיאור

Glyfo הופך תמונות שיש בהן כתב לטקסט שאפשר להשתמש בו.

הזיהוי רץ על מנוע ה‑OCR המובנה ב‑Windows: בלי חשבון, בלי העלאה, בלי להמתין לשרת. במחשב Copilot+
נעזר Glyfo נוסף על כך במודל זיהוי הטקסט שרץ על המכשיר עצמו עבור התמונות הקשות, ויודע לתרגם את
התוצאה — גם זאת בלי רשת.

**ארבע דרכים להכניס תמונה**
- הקישו Alt+Z כדי לסמן מסגרת סביב כל חלק במסך. Ctrl+Shift+R לוקח את המסך כולו.
- הדביקו תמונה מהלוח עם Ctrl+V, וגם טקסט.
- פתחו קובץ עם Ctrl+O; גרירה של הקובץ אל החלון עושה בדיוק את אותו הדבר.
- לחצו לחיצה ימנית על תמונה בסייר הקבצים ופתחו אותה ב‑Glyfo, או שלחו אותה מחלונית השיתוף של Windows
  (תמונות, כלי החיתוך, הדפדפן).

**מה מקבלים בחזרה**
- הטקסט שזוהה לצד התמונה, בסדר שבו היה מסודר במקור.
- העתקה בלחיצה אחת, או לתת לצילום להעתיק את עצמו ברגע שהוא מסתיים.
- הקראה בקול בכל קול שמותקן במחשב.
- הפעולה ״הסרת שבירות שורה״ מחברת שוב שורות שנקטעו לפסקאות, ו״הסרת רווחים״ מוחקת כל רווח — בדיוק מה
  שטקסט סיני, יפני וקוריאני צריך אחרי הזיהוי.
- קודי QR וברקודים מאותה תמונה עצמה.
- היסטוריה של התוצאות האחרונות: הצילום מלפני שתי דקות עדיין במרחק לחיצה אחת.

**אתם מחליטים איך הוא קורא**
- בחרו את שפת הזיהוי מתוך חבילות השפה המותקנות ב‑Windows, או הניחו ל‑Glyfo להחליט.
- אפשרות אחת מתקנת את שגיאת ה‑OCR הקלאסית שבה v1.6.5 נקרא vl.6.5: האות l או I הופכת ל‑1 רק במקום
  שבו ניצבים לצידה מפריד וספרה, כך ש‑html5 ו‑IPv6 נשארים כפי שהם.
- התאימו לחלון או הציגו בגודל אמיתי; זהו את התמונה כולה או רק את הבחירה.

**הוא לא עומד בדרך**
- סגירת החלון משאירה את Glyfo באזור ההתראות, וקיצור המקשים ממשיך לעבוד. ״יציאה״ משם סוגר אותו
  באמת. ההתנהגות הזו היא בעצמה הגדרה.
- Glyfo יכול לעלות יחד עם Windows וללכת היישר לאזור ההתראות בלי לפתוח חלון, כך שקיצור המקשים עובד
  מרגע הכניסה למערכת. כבוי כברירת מחדל; אתם מדליקים אותו.
- כשהחלון מוסתר, התראה מציגה את השורה הראשונה של מה שזה עתה זוהה.

**שפות**
הממשק זמין ב‑33 שפות ועוקב אחר הגדרת השפה של Windows. הזיהוי משתמש בחבילות שפת ה‑OCR המותקנות
במחשב — מוסיפים עוד דרך הגדרות › שעה ושפה › שפה ואזור.

**פרטיות**
Glyfo אינו יוצר חיבורי רשת. תמונות, טקסט שזוהה ותרגומים לעולם אינם עוזבים את המחשב שלכם. בלי חשבון,
בלי טלמטריה, בלי פרסומות.

### תכונות המוצר

- צלמו כל חלק במסך עם Alt+Z וזהו אותו מיד, בלי לצאת מהאפליקציה שבה קראתם
- רץ על מנוע ה‑OCR המובנה ב‑Windows, ובמחשבי Copilot+ גם על מודל הזיהוי שעל המכשיר
- פתיחת קובץ, הדבקה מהלוח, גרירה ושחרור או קבלה מחלונית השיתוף של Windows
- קורא קודי QR וברקודים מאותה תמונה עצמה
- מתרגם במחשבי Copilot+ על המכשיר עצמו, בלי גישה לרשת
- מקריא את התוצאה בקול בכל קול שמותקן במחשב
- הסרת שבירות שורה ורווחים — הניקוי שטקסט CJK זקוק לו אחרי הזיהוי
- נשאר באזור ההתראות, כך שקיצור המקשים עובד גם אחרי סגירת החלון
- ממשק ב‑33 שפות, בהתאם להגדרת השפה של Windows
- אינו מתחבר לאינטרנט: מה שאתם מזהים לא עוזב את המחשב שלכם

### כיתובים לצילומי מסך

1. `01-text-from-a-page.png` — הטקסט שזוהה עומד לצד התמונה בסדר שבו נקרא: מוכן להעתקה, להאזנה או לחיבור מחדש לפסקאות.
2. `02-any-language.png` — שפת הזיהוי נבחרת מתוך חבילות השפה המותקנות ב‑Windows — או ש‑Glyfo קובע אותה בעצמו.
3. `03-qr-and-barcodes.png` — קודי QR וברקודים נקראים מאותה תמונה עצמה; אין צורך באפליקציית סריקה נפרדת.
4. `04-history.png` — התוצאות האחרונות נשארות בהיסטוריה, כך שצילום מלפני כמה דקות עדיין במרחק לחיצה אחת.
5. `05-settings.png` — שפת הממשק, עלייה יחד עם Windows, הישארות באזור ההתראות, והתיקון שמונע מ‑v1.6.5 להפוך ל‑vl.6.5.

### מונחי חיפוש

`טקסט מתמונה`, `צילום מסך לטקסט`, `זיהוי טקסט`, `קורא קוד QR`,
`סורק ברקוד`, `חילוץ טקסט`, `הקראה בקול`

---

## فارسی — Persian

### توضیح کوتاه

Glyfo متن را از هر چیزی که می‌بینید بیرون می‌کشد: از یک اسکرین‌شات، از عکس یک صفحه، از سندی که
اسکن شده، از اسلاید ارائه، از یک فریم ویدیو. کلید Alt+Z را بزنید و دور بخشی از صفحه کادر بکشید، یا
فایلی باز کنید، از کلیپ‌بورد بچسبانید، تصویری را از برنامه‌ای دیگر بفرستید. متن شناسایی‌شده کنار
تصویر ظاهر می‌شود، آماده‌ی کپی کردن، شنیدن یا مرتب کردن. Glyfo کدهای QR و بارکد را هم از همان تصویر
می‌خواند، و روی رایانه‌ی Copilot+ نتیجه را ترجمه می‌کند. همه‌چیز روی رایانه‌ی خودتان انجام می‌شود —
Glyfo هیچ اتصالی به اینترنت برقرار نمی‌کند. رایگان، بدون تبلیغات و بدون خرید.

### توضیحات

Glyfo تصویرهایی را که در آن‌ها نوشته هست به متنی تبدیل می‌کند که می‌توانید به کارش ببرید.

شناسایی روی همان OCR کار می‌کند که در Windows تعبیه شده است: نه حسابی می‌سازید، نه چیزی آپلود
می‌کنید، نه منتظر سروری می‌مانید. روی رایانه‌ی Copilot+، افزون بر آن، Glyfo برای تصویرهای دشوار از
مدل شناسایی متن که روی خود دستگاه اجرا می‌شود کمک می‌گیرد و می‌تواند نتیجه را ترجمه کند — آن هم بدون
شبکه.

**چهار راه برای وارد کردن تصویر**
- کلید Alt+Z دور هر بخشی از صفحه کادر می‌کشد. کلید Ctrl+Shift+R تمام صفحه را برمی‌دارد.
- کلید Ctrl+V تصویر را از کلیپ‌بورد می‌چسباند، متن را هم همین‌طور.
- کلید Ctrl+O فایلی را باز می‌کند؛ کشیدن فایل داخل پنجره هم دقیقاً همان کار را می‌کند.
- روی تصویری در File Explorer راست‌کلیک کنید و آن را با Glyfo باز کنید، یا از پنل اشتراک‌گذاری
  Windows بفرستید (Photos، Snipping Tool، مرورگر).

**چه چیزی به دست می‌آورید**
- متن شناسایی‌شده کنار تصویر، به همان ترتیبی که در چیدمان اصلی بوده است.
- کپی با یک کلیک، یا بگذارید یک برداشت به‌محض تمام‌شدن، خودش را کپی کند.
- خواندن با صدای بلند با هر صدایی که روی رایانه نصب است.
- گزینه‌ی «حذف شکست خط» خط‌های بریده‌شده را دوباره به پاراگراف وصل می‌کند و «حذف فاصله‌ها» هر فاصله
  را پاک می‌کند — دقیقاً همان چیزی که متن چینی، ژاپنی و کره‌ای پس از شناسایی لازم دارد.
- کدهای QR و بارکد از همان تصویر.
- تاریخچه‌ای از نتیجه‌های اخیر: برداشتی که دو دقیقه پیش گرفته‌اید هنوز یک کلیک فاصله دارد.

**شما تصمیم می‌گیرید چطور بخواند**
- زبان شناسایی را از میان بسته‌های زبانی نصب‌شده در Windows انتخاب کنید، یا تصمیم را به Glyfo
  بسپارید.
- گزینه‌ای آن خطای کلاسیک OCR را درست می‌کند که v1.6.5 را vl.6.5 می‌خواند: حرف l یا I تنها جایی به
  عدد ۱ تبدیل می‌شود که کنارش یک جداکننده و یک رقم ایستاده باشد، پس html5 و IPv6 دست‌نخورده
  می‌مانند.
- می‌توانید تصویر را اندازه‌ی پنجره کنید یا در اندازه‌ی واقعی ببینید؛ کل تصویر را شناسایی کنید یا
  فقط بخش انتخاب‌شده را.

**سر راهتان نیست**
- بستن پنجره Glyfo را در ناحیه‌ی اعلان نگه می‌دارد و میان‌بر برداشت همچنان کار می‌کند. «خروج» از
  همان‌جا واقعاً می‌بنددش. خودِ همین رفتار هم یک تنظیم است.
- Glyfo می‌تواند همراه Windows بالا بیاید و بدون باز کردن هیچ پنجره‌ای یک‌راست به ناحیه‌ی اعلان
  برود، تا میان‌بر از همان لحظه‌ی ورود به سیستم کار کند. به‌طور پیش‌فرض خاموش است؛ خودتان روشنش
  می‌کنید.
- وقتی پنجره پنهان است، یک اعلان سطر اول چیزی را که همین حالا شناسایی شده نشان می‌دهد.

**زبان‌ها**
رابط کاربری به ۳۳ زبان در دسترس است و از تنظیم زبان Windows پیروی می‌کند. شناسایی از بسته‌های زبانی
OCR نصب‌شده روی رایانه استفاده می‌کند — بسته‌های تازه را از Settings › Time & language › Language &
region اضافه کنید.

**حریم خصوصی**
Glyfo هیچ اتصال شبکه‌ای برقرار نمی‌کند. تصویرها، متن شناسایی‌شده و ترجمه‌ها هرگز از رایانه‌ی شما
بیرون نمی‌روند. نه حسابی، نه داده‌ی از راه دور، نه تبلیغی.

### ویژگی‌های محصول

- هر بخشی از صفحه را با Alt+Z بردارید و همان‌جا شناسایی کنید، بی‌آنکه از برنامه‌ای که در آن می‌خواندید بیرون بیایید
- روی OCR تعبیه‌شده در Windows کار می‌کند و روی رایانه‌های Copilot+ با مدل شناسایی روی خود دستگاه
- فایل باز کنید، از کلیپ‌بورد بچسبانید، بکشید و رها کنید یا از پنل اشتراک‌گذاری Windows دریافت کنید
- کدهای QR و بارکد را از همان تصویر می‌خواند
- روی رایانه‌های Copilot+ همان‌جا روی دستگاه ترجمه می‌کند، بدون دسترسی به شبکه
- نتیجه را با هر صدایی که روی رایانه نصب است با صدای بلند می‌خواند
- حذف شکست خط و فاصله‌ها — همان مرتب‌سازی که متن CJK پس از شناسایی لازم دارد
- در ناحیه‌ی اعلان می‌ماند، پس میان‌بر حتی پس از بستن پنجره کار می‌کند
- رابط کاربری به ۳۳ زبان، بر پایه‌ی تنظیم زبان Windows
- به اینترنت وصل نمی‌شود: آنچه شناسایی می‌کنید از رایانه‌ی شما بیرون نمی‌رود

### زیرنویس تصویرها

1. `01-text-from-a-page.png` — متن شناسایی‌شده به همان ترتیبی که خوانده شده کنار تصویر می‌ایستد: آماده‌ی کپی، شنیدن یا وصل شدن دوباره به پاراگراف.
2. `02-any-language.png` — زبان شناسایی از میان بسته‌های زبانی نصب‌شده در Windows انتخاب می‌شود — یا Glyfo خودش آن را تعیین می‌کند.
3. `03-qr-and-barcodes.png` — کدهای QR و بارکد از همان تصویر خوانده می‌شوند؛ به برنامه‌ی اسکن جداگانه نیازی نیست.
4. `04-history.png` — نتیجه‌های اخیر در تاریخچه می‌مانند، پس برداشتی از چند دقیقه پیش هنوز یک کلیک فاصله دارد.
5. `05-settings.png` — زبان رابط کاربری، بالا آمدن همراه Windows، ماندن در ناحیه‌ی اعلان، و اصلاحی که نمی‌گذارد v1.6.5 به vl.6.5 تبدیل شود.

### عبارت‌های جست‌وجو

`متن از تصویر`, `اسکرین‌شات به متن`, `تشخیص متن`, `خواننده کد QR`,
`اسکنر بارکد`, `استخراج متن`, `خواندن با صدای بلند`
