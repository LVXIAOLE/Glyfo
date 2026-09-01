# Store listing copy

Ready-to-paste text for the Partner Center **Store listings** page, one section per language.

The package declares thirty-three languages, but a Store listing is written by hand, so only the
ten with the most Store traffic for this kind of app are here. Every language the package declares
but this file does not falls back to the English listing automatically — that is the Store's own
behaviour, not a gap.

| Partner Center field | Limit | Section below |
|---|---|---|
| 产品名称 / Product name | picked from reserved names | `Glyfo — OCR & Screen Capture` for every language |
| 说明 / Description | 10,000 characters | **说明** |
| 简短说明 / Short description | 1,000 characters | **简短说明** |
| 产品功能 / Product features | 20 entries, 200 characters each | **产品功能** |
| 搜索词 / Search terms | 7 terms, 30 characters each | **搜索词** |

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

### Поисковые запросы

`текст с картинки`, `скриншот в текст`, `распознавание текста`, `сканер QR-кода`,
`сканер штрихкодов`, `извлечь текст`, `читать вслух`
