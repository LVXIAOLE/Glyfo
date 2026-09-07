# Store release notes — 1.3.0

Ready-to-paste text for the Partner Center **此版本的新增功能 / What's new in this version** field,
one section per language. Limit is 1,500 characters per language; every section below is well under.

This covers **two** releases, not one. 1.2.0 was built and committed but never submitted to the
Store, so a customer updating from 1.1.0 receives both at once and has to be told about both. The
in-app release notes handle this by themselves — `Changelog.Since` returns every release newer than
the one the user last saw — but the Store field is a single free-text box with no such logic, so the
eleven features are simply listed together, 1.3.0 first.

This is deliberately **not** part of `store-listing.md`. That file's sections are keyed by position
by `tools/Fill-ListingCsv.ps1`, which throws if the count is wrong, and the release-notes field is
not in the listing CSV export at all. Keeping it here leaves that contract untouched.

Arabic, Hebrew and Persian carry no directional control characters, for the same reason as in
`store-listing.md`: every line begins with a word in its own script, so first-strong bidi detection
resolves it right-to-left on its own, and that survives a round trip through a text box in a way an
invisible U+200F may not.

---

## English (default)

- History that survives a restart: everything Glyfo reads is kept and can be searched. Text only, and you can clear it or switch it off.
- Save the recognized text straight to a .txt or .md file with Ctrl+S.
- Batch recognition: open or drop a stack of files, or read a whole PDF, and save the results merged or one file per item.
- Open PDFs the way you open pictures, and read them page by page.
- Clipboard watching: press Win+Shift+S and Glyfo reads the text out of the snip, leaving it on the clipboard. Off by default.
- Links, e-mail addresses and phone numbers found in the text now get their own buttons.
- Choose your own capture shortcuts instead of the built-in ones.
- The window opens at the size and position you left it.
- Rotate a picture, or straighten a crooked photo in one click.
- Find in the text with Ctrl+F, with a word and character count beside it.
- Pick a light or dark appearance, or follow Windows.

## 中文（简体）

- 历史记录能活过重启，并且可以搜索。只保存文字，随时可以清空或关闭。
- Ctrl+S 把识别出的文字直接存成 .txt 或 .md 文件。
- 批量识别：一次打开或拖入多个文件，或者识别整本 PDF，结果可以合并成一个文件，也可以每项一个。
- 像打开图片一样打开 PDF，逐页识别。
- 剪贴板监听：按 Win+Shift+S 截图后，Glyfo 自动读出里面的文字并留在剪贴板里。默认关闭。
- 文本里认出的网址、邮箱和电话，下面会出现对应的按钮。
- 截图快捷键可以自己定，不必迁就内置的组合。
- 窗口按上次关闭时的大小和位置打开。
- 图片可以旋转，拍歪的照片一键摆正。
- Ctrl+F 在识别结果里查找，旁边还有字数和字符数。
- 界面可以选浅色或深色，也可以跟随 Windows。

## 中文（繁體）

- 歷程記錄能活過重新啟動，而且可以搜尋。只保存文字，隨時可以清空或關閉。
- Ctrl+S 把辨識出的文字直接存成 .txt 或 .md 檔案。
- 批次辨識：一次開啟或拖入多個檔案，或者辨識整本 PDF，結果可以合併成一個檔案，也可以每項一個。
- 像開啟圖片一樣開啟 PDF，逐頁辨識。
- 剪貼簿監看：按 Win+Shift+S 擷取畫面後，Glyfo 自動讀出裡面的文字並留在剪貼簿裡。預設關閉。
- 文字裡認出的網址、電子郵件和電話，下面會出現對應的按鈕。
- 擷取快速鍵可以自己決定，不必遷就內建的組合。
- 視窗照上次關閉時的大小和位置開啟。
- 圖片可以旋轉，拍歪的照片一鍵擺正。
- Ctrl+F 在辨識結果裡尋找，旁邊還有字數和字元數。
- 介面可以選淺色或深色，也可以跟隨 Windows。

## العربية — Arabic

- السجل يبقى بعد إعادة التشغيل ويمكن البحث فيه. النصوص فقط، ويمكنك مسحه أو إيقافه متى شئت.
- الاختصار Ctrl+S يحفظ النص المتعرَّف عليه مباشرة في ملف بصيغة txt أو md.
- التعرف الجماعي: افتح أو أفلت عدة ملفات، أو اقرأ ملف PDF كاملًا، واحفظ النتائج مدمجة أو ملفًا لكل عنصر.
- ملفات PDF تُفتح مثل الصور تمامًا، وتُقرأ صفحة بعد صفحة.
- مراقبة الحافظة: اضغط Win+Shift+S فيقرأ Glyfo نص اللقطة ويتركه في الحافظة. معطّلة افتراضيًا.
- الروابط وعناوين البريد وأرقام الهاتف التي يجدها في النص صار لكل منها زر خاص.
- اختصارات الالتقاط صارت من اختيارك بدلًا من المدمجة.
- النافذة تُفتح بالحجم والموضع اللذين تركتها عليهما.
- الصور يمكن تدويرها، والصورة المائلة تُقوَّم بنقرة واحدة.
- البحث داخل النص بـ Ctrl+F، مع عدد الكلمات والحروف بجانبه.
- المظهر يمكن أن يكون فاتحًا أو داكنًا أو تابعًا لإعداد Windows.

## বাংলা — Bengali

- ইতিহাস এখন পুনরায় চালুর পরেও থাকে এবং তাতে খোঁজা যায়। কেবল লেখা রাখে, আর যখন খুশি মোছা বা বন্ধ করা যায়।
- Ctrl+S দিয়ে শনাক্ত করা লেখা সরাসরি .txt বা .md ফাইলে সংরক্ষণ করুন।
- ব্যাচ শনাক্তকরণ: একসঙ্গে অনেক ফাইল খুলুন বা টেনে আনুন, কিংবা গোটা PDF পড়ুন, ফল একটি ফাইলে মিলিয়ে বা প্রতিটির আলাদা ফাইলে রাখুন।
- ছবির মতো করেই PDF খুলুন এবং পাতায় পাতায় পড়ুন।
- ক্লিপবোর্ড নজরদারি: Win+Shift+S চাপলে Glyfo সেই স্ক্রিনশটের লেখা পড়ে ক্লিপবোর্ডেই রেখে দেয়। শুরুতে বন্ধ থাকে।
- লেখার মধ্যে পাওয়া লিংক, ইমেল ঠিকানা ও ফোন নম্বরের জন্য আলাদা বোতাম আসে।
- ক্যাপচারের শর্টকাট এখন নিজের পছন্দমতো বেছে নিন।
- উইন্ডো যে আকারে ও যেখানে রেখেছিলেন, সেভাবেই খোলে।
- ছবি ঘোরানো যায়, আর বাঁকা ছবি এক ক্লিকে সোজা হয়।
- Ctrl+F দিয়ে লেখায় খুঁজুন, পাশেই শব্দ ও অক্ষরের সংখ্যা।
- হালকা বা গাঢ় চেহারা বেছে নিন, কিংবা Windows অনুসরণ করান।

## Čeština — Czech

- Historie přežije restart a dá se v ní hledat. Ukládá jen text a kdykoli ji lze vymazat nebo vypnout.
- Ctrl+S uloží rozpoznaný text rovnou do souboru .txt nebo .md.
- Dávkové rozpoznávání: otevřete nebo přetáhněte více souborů, nebo přečtěte celé PDF, a výsledky uložte spojené nebo po jednom souboru.
- PDF se otevírá stejně jako obrázek a čte se stránku po stránce.
- Sledování schránky: stisknete Win+Shift+S a Glyfo z výřezu přečte text a nechá ho ve schránce. Ve výchozím stavu vypnuto.
- Odkazy, e-mailové adresy a telefonní čísla nalezená v textu mají nově vlastní tlačítka.
- Zkratky pro zachytávání si nastavíte sami místo vestavěných.
- Okno se otevře v takové velikosti a na tom místě, kde jste ho nechali.
- Obrázek lze otočit a pokřivenou fotku jedním klepnutím narovnat.
- Hledání v textu pomocí Ctrl+F, vedle něj počet slov a znaků.
- Vzhled si vyberete světlý nebo tmavý, případně podle Windows.

## Dansk — Danish

- Historikken overlever en genstart, og der kan søges i den. Kun tekst, og den kan ryddes eller slås fra.
- Ctrl+S gemmer den aflæste tekst direkte i en .txt- eller .md-fil.
- Batchaflæsning: åbn eller slip en stak filer, eller læs en hel PDF, og gem resultaterne samlet eller som én fil pr. element.
- PDF-filer åbnes som billeder og læses side for side.
- Overvågning af udklipsholderen: tryk Win+Shift+S, og Glyfo læser teksten i klippet og lader den blive i udklipsholderen. Slået fra som standard.
- Links, mailadresser og telefonnumre i teksten får nu deres egne knapper.
- Vælg dine egne genveje til skærmklip i stedet for de indbyggede.
- Vinduet åbner i den størrelse og på den plads, du forlod det.
- Billeder kan roteres, og et skævt foto rettes op med ét klik.
- Søg i teksten med Ctrl+F, med ord- og tegntælling ved siden af.
- Vælg et lyst eller mørkt udseende, eller følg Windows.

## Deutsch — German

- Der Verlauf übersteht einen Neustart und lässt sich durchsuchen. Nur Text, und jederzeit löschbar oder abschaltbar.
- Mit Strg+S wird der erkannte Text direkt als .txt- oder .md-Datei gespeichert.
- Stapelerkennung: mehrere Dateien öffnen oder hineinziehen, oder ein ganzes PDF lesen, und die Ergebnisse zusammengefasst oder als je eine Datei speichern.
- PDFs lassen sich wie Bilder öffnen und Seite für Seite lesen.
- Zwischenablage-Überwachung: Win+Umschalt+S drücken, und Glyfo liest den Text aus dem Ausschnitt und lässt ihn in der Zwischenablage. Standardmäßig aus.
- Links, E-Mail-Adressen und Telefonnummern im Text bekommen jetzt eigene Schaltflächen.
- Die Tastenkürzel für Aufnahmen wählen Sie selbst statt der eingebauten.
- Das Fenster öffnet sich in der Größe und an der Stelle, wo Sie es verlassen haben.
- Bilder lassen sich drehen, ein schiefes Foto mit einem Klick gerade richten.
- Suche im Text mit Strg+F, daneben die Wort- und Zeichenzahl.
- Helles oder dunkles Aussehen wählen, oder Windows folgen.

## Ελληνικά — Greek

- Το ιστορικό επιβιώνει μιας επανεκκίνησης και μπορείτε να το αναζητήσετε. Μόνο κείμενο, και σβήνεται ή απενεργοποιείται όποτε θέλετε.
- Το Ctrl+S αποθηκεύει το αναγνωρισμένο κείμενο κατευθείαν σε αρχείο .txt ή .md.
- Μαζική αναγνώριση: ανοίξτε ή αφήστε πολλά αρχεία, ή διαβάστε ολόκληρο PDF, και αποθηκεύστε τα αποτελέσματα ενωμένα ή ένα αρχείο ανά στοιχείο.
- Τα PDF ανοίγουν όπως οι εικόνες και διαβάζονται σελίδα σελίδα.
- Παρακολούθηση προχείρου: πατάτε Win+Shift+S και το Glyfo διαβάζει το κείμενο του στιγμιότυπου και το αφήνει στο πρόχειρο. Ανενεργό εξ ορισμού.
- Οι σύνδεσμοι, οι διευθύνσεις email και τα τηλέφωνα μέσα στο κείμενο αποκτούν δικά τους κουμπιά.
- Οι συντομεύσεις λήψης ορίζονται πλέον από εσάς αντί για τις ενσωματωμένες.
- Το παράθυρο ανοίγει στο μέγεθος και στη θέση που το αφήσατε.
- Οι εικόνες περιστρέφονται, και μια στραβή φωτογραφία ισιώνει με ένα κλικ.
- Αναζήτηση στο κείμενο με Ctrl+F, με μέτρηση λέξεων και χαρακτήρων δίπλα.
- Διαλέγετε ανοιχτή ή σκούρα εμφάνιση, ή ακολουθείτε τα Windows.

## Español — Spanish

- El historial sobrevive a un reinicio y se puede buscar. Solo texto, y puedes vaciarlo o desactivarlo cuando quieras.
- Ctrl+S guarda el texto reconocido directamente en un archivo .txt o .md.
- Reconocimiento por lotes: abre o arrastra varios archivos, o lee un PDF entero, y guarda los resultados juntos o uno por elemento.
- Los PDF se abren igual que las imágenes y se leen página a página.
- Vigilancia del portapapeles: pulsa Win+Mayús+S y Glyfo lee el texto del recorte y lo deja en el portapapeles. Desactivada de serie.
- Los enlaces, las direcciones de correo y los teléfonos que aparecen en el texto tienen ahora sus propios botones.
- Elige tus propios atajos de captura en vez de los de fábrica.
- La ventana se abre con el tamaño y en la posición en que la dejaste.
- Las imágenes se pueden girar, y una foto torcida se endereza de un clic.
- Busca en el texto con Ctrl+F, con el recuento de palabras y caracteres al lado.
- Elige un aspecto claro u oscuro, o sigue a Windows.

## فارسی — Persian

- تاریخچه پس از راه‌اندازی دوباره هم می‌ماند و می‌توان در آن جست‌وجو کرد. فقط متن، و هر وقت بخواهید پاک یا خاموشش می‌کنید.
- کلید Ctrl+S متن تشخیص‌داده‌شده را مستقیم در فایلی با پسوند txt یا md ذخیره می‌کند.
- تشخیص دسته‌ای: چند فایل را با هم باز کنید یا رها کنید، یا یک PDF کامل را بخوانید، و نتیجه را یکجا یا هر مورد در یک فایل ذخیره کنید.
- فایل‌های PDF مثل تصویر باز می‌شوند و صفحه‌به‌صفحه خوانده می‌شوند.
- پایش کلیپ‌بورد: Win+Shift+S را بزنید تا Glyfo متن آن برش را بخواند و در کلیپ‌بورد نگه دارد. به‌طور پیش‌فرض خاموش است.
- پیوندها، نشانی‌های ایمیل و شماره‌های تلفن درون متن حالا دکمه مخصوص خود را دارند.
- میان‌برهای گرفتن تصویر را به‌جای میان‌برهای پیش‌فرض خودتان انتخاب می‌کنید.
- پنجره با همان اندازه و در همان جایی که رهایش کردید باز می‌شود.
- تصویر را می‌توان چرخاند و عکس کج را با یک کلیک صاف کرد.
- جست‌وجو در متن با Ctrl+F، همراه شمار واژه‌ها و نویسه‌ها در کنارش.
- ظاهر روشن یا تیره را برگزینید، یا بگذارید از Windows پیروی کند.

## Suomi — Finnish

- Historia säilyy uudelleenkäynnistyksen yli, ja siitä voi hakea. Vain teksti, ja sen voi tyhjentää tai kytkeä pois.
- Ctrl+S tallentaa tunnistetun tekstin suoraan .txt- tai .md-tiedostoon.
- Erätunnistus: avaa tai pudota useita tiedostoja tai lue koko PDF, ja tallenna tulokset yhtenä tiedostona tai kohde kerrallaan.
- PDF-tiedostot avautuvat kuten kuvat ja luetaan sivu kerrallaan.
- Leikepöydän valvonta: paina Win+Vaihto+S, niin Glyfo lukee tekstin kuvakaappauksesta ja jättää sen leikepöydälle. Oletuksena pois päältä.
- Tekstistä löytyvät linkit, sähköpostiosoitteet ja puhelinnumerot saavat nyt omat painikkeensa.
- Kaappauksen pikanäppäimet saa valita itse sisäänrakennettujen sijaan.
- Ikkuna avautuu siinä koossa ja siinä paikassa, mihin sen jätit.
- Kuvaa voi kääntää, ja vinon valokuvan suoristaa yhdellä napsautuksella.
- Tekstistä haetaan Ctrl+F:llä, ja vieressä näkyy sanojen ja merkkien määrä.
- Valitse vaalea tai tumma ulkoasu, tai seuraa Windowsia.

## Filipino

- Nananatili ang kasaysayan kahit ma-restart, at puwede itong hanapan. Teksto lamang, at maaari itong burahin o patayin.
- Sine-save ng Ctrl+S ang nakilalang teksto nang diretso sa .txt o .md na file.
- Batch na pagkilala: magbukas o mag-drop ng maraming file, o basahin ang buong PDF, at i-save ang resulta nang pinagsama o isang file bawat item.
- Nabubuksan ang PDF gaya ng larawan at nababasa pahina bawat pahina.
- Pagbabantay sa clipboard: pindutin ang Win+Shift+S at babasahin ng Glyfo ang teksto sa snip at iiwan ito sa clipboard. Nakapatay bilang default.
- May sariling button na ngayon ang mga link, email address at numero ng telepono na nakita sa teksto.
- Ikaw na ang pumipili ng mga shortcut sa pagkuha, hindi na ang mga nakalaan.
- Bumubukas ang window sa laki at lugar na iniwan mo.
- Puwedeng iikot ang larawan, at maituwid ang tabingi nito sa isang click.
- Maghanap sa teksto gamit ang Ctrl+F, may bilang ng salita at karakter sa tabi.
- Pumili ng maliwanag o madilim na anyo, o sumunod sa Windows.

## Français — French

- L’historique survit à un redémarrage et peut être fouillé. Du texte seulement, et il se vide ou se désactive quand vous voulez.
- Ctrl+S enregistre le texte reconnu directement dans un fichier .txt ou .md.
- Reconnaissance par lots : ouvrez ou déposez plusieurs fichiers, ou lisez un PDF entier, et enregistrez les résultats regroupés ou un fichier par élément.
- Les PDF s’ouvrent comme des images et se lisent page par page.
- Surveillance du presse-papiers : appuyez sur Win+Maj+S et Glyfo lit le texte de la capture et le laisse dans le presse-papiers. Désactivée par défaut.
- Les liens, adresses e-mail et numéros de téléphone trouvés dans le texte ont désormais leurs propres boutons.
- Choisissez vos propres raccourcis de capture au lieu de ceux d’origine.
- La fenêtre s’ouvre à la taille et à l’endroit où vous l’avez laissée.
- Une image se fait pivoter, et une photo de travers se redresse d’un clic.
- Recherchez dans le texte avec Ctrl+F, avec le nombre de mots et de caractères à côté.
- Choisissez une apparence claire ou sombre, ou suivez Windows.

## עברית — Hebrew

- ההיסטוריה שורדת הפעלה מחדש וניתן לחפש בה. טקסט בלבד, ואפשר לנקות אותה או לכבות אותה בכל רגע.
- הצירוף Ctrl+S שומר את הטקסט שזוהה ישירות לקובץ מסוג txt או md.
- זיהוי מרובה: פתחו או גררו כמה קבצים, או קראו קובץ PDF שלם, ושמרו את התוצאות מאוחדות או קובץ לכל פריט.
- קובצי PDF נפתחים כמו תמונות ונקראים עמוד אחר עמוד.
- מעקב אחר הלוח: הקישו Win+Shift+S ו‑Glyfo יקרא את הטקסט מהגזירה וישאיר אותו בלוח. כבוי כברירת מחדל.
- קישורים, כתובות דוא״ל ומספרי טלפון שנמצאו בטקסט מקבלים כעת כפתור משלהם.
- קיצורי הצילום נבחרים על ידיכם במקום המובנים.
- החלון נפתח בגודל ובמקום שבהם השארתם אותו.
- אפשר לסובב תמונה, ולהזקיף צילום עקום בלחיצה אחת.
- חיפוש בטקסט עם Ctrl+F, ולצידו ספירת מילים ותווים.
- בחרו מראה בהיר או כהה, או עקבו אחרי Windows.

## हिन्दी — Hindi

- इतिहास अब पुनरारंभ के बाद भी बना रहता है और उसमें खोजा जा सकता है। सिर्फ़ पाठ, और जब चाहें मिटा या बंद कर दें।
- Ctrl+S पहचाने गए पाठ को सीधे .txt या .md फ़ाइल में सहेजता है।
- बैच पहचान: एक साथ कई फ़ाइलें खोलें या छोड़ें, या पूरी PDF पढ़ें, और परिणाम एक साथ या हर वस्तु की अलग फ़ाइल में सहेजें।
- PDF तस्वीर की तरह ही खुलती है और पन्ना दर पन्ना पढ़ी जाती है।
- क्लिपबोर्ड निगरानी: Win+Shift+S दबाइए और Glyfo उस कतरन का पाठ पढ़कर क्लिपबोर्ड में ही छोड़ देता है। शुरू में बंद रहती है।
- पाठ में मिले लिंक, ईमेल पते और फ़ोन नंबर को अब अपने अलग बटन मिलते हैं।
- कैप्चर के शॉर्टकट अब अंतर्निहित के बजाय आप ख़ुद चुनते हैं।
- खिड़की उसी आकार और उसी जगह खुलती है जहाँ आपने छोड़ी थी।
- तस्वीर घुमाई जा सकती है, और टेढ़ी तस्वीर एक क्लिक में सीधी हो जाती है।
- Ctrl+F से पाठ में खोजिए, साथ में शब्द और अक्षर की गिनती।
- हल्का या गहरा रूप चुनिए, या Windows का अनुसरण कीजिए।

## Magyar — Hungarian

- Az előzmények túlélik az újraindítást, és kereshetők. Csak szöveg, és bármikor törölhető vagy kikapcsolható.
- A Ctrl+S a felismert szöveget egyenesen .txt vagy .md fájlba menti.
- Kötegelt felismerés: nyisson meg vagy húzzon be több fájlt, vagy olvasson be egy teljes PDF-et, és mentse az eredményt egybefűzve vagy elemenként külön fájlba.
- A PDF ugyanúgy nyílik meg, mint egy kép, és oldalról oldalra olvasható.
- Vágólapfigyelés: nyomja meg a Win+Shift+S billentyűt, és a Glyfo kiolvassa a kivágás szövegét, a vágólapon hagyva azt. Alapból kikapcsolva.
- A szövegben talált hivatkozások, e-mail-címek és telefonszámok mostantól saját gombot kapnak.
- A képrögzítés gyorsbillentyűit a beépítettek helyett Ön választja meg.
- Az ablak abban a méretben és azon a helyen nyílik meg, ahol hagyta.
- A kép elforgatható, a ferde fénykép egyetlen kattintással kiegyenesíthető.
- Keresés a szövegben Ctrl+F-fel, mellette a szavak és karakterek száma.
- Válasszon világos vagy sötét megjelenést, vagy kövesse a Windowst.

## Bahasa Indonesia — Indonesian

- Riwayat bertahan setelah dimulai ulang dan bisa dicari. Hanya teks, dan dapat dikosongkan atau dimatikan kapan saja.
- Ctrl+S menyimpan teks hasil pengenalan langsung ke berkas .txt atau .md.
- Pengenalan massal: buka atau jatuhkan banyak berkas, atau baca satu PDF utuh, lalu simpan hasilnya digabung atau satu berkas per butir.
- Berkas PDF dibuka seperti gambar dan dibaca halaman demi halaman.
- Pemantauan papan klip: tekan Win+Shift+S dan Glyfo membaca teks pada potongan itu serta meninggalkannya di papan klip. Mati secara bawaan.
- Tautan, alamat surel, dan nomor telepon yang ditemukan dalam teks kini punya tombolnya sendiri.
- Pintasan tangkapan layar kini Anda pilih sendiri, bukan yang bawaan.
- Jendela terbuka pada ukuran dan posisi terakhir Anda meninggalkannya.
- Gambar bisa diputar, dan foto yang miring diluruskan dengan satu klik.
- Cari dalam teks dengan Ctrl+F, lengkap dengan jumlah kata dan karakter di sebelahnya.
- Pilih tampilan terang atau gelap, atau ikuti Windows.

## Italiano — Italian

- La cronologia sopravvive al riavvio e si può cercare. Solo testo, e si svuota o si disattiva quando vuoi.
- Ctrl+S salva il testo riconosciuto direttamente in un file .txt o .md.
- Riconoscimento in blocco: apri o trascina più file, oppure leggi un intero PDF, e salva i risultati uniti o un file per elemento.
- I PDF si aprono come le immagini e si leggono pagina per pagina.
- Sorveglianza degli appunti: premi Win+Maiusc+S e Glyfo legge il testo del ritaglio lasciandolo negli appunti. Disattivata per impostazione predefinita.
- Collegamenti, indirizzi e-mail e numeri di telefono trovati nel testo hanno ora un pulsante dedicato.
- Le scorciatoie di cattura le scegli tu, al posto di quelle predefinite.
- La finestra si apre con la dimensione e nella posizione in cui l’hai lasciata.
- Un’immagine si può ruotare, e una foto storta si raddrizza con un clic.
- Cerca nel testo con Ctrl+F, con il conteggio di parole e caratteri accanto.
- Scegli un aspetto chiaro o scuro, oppure segui Windows.

## 日本語 — Japanese

- 履歴は再起動後も残り、検索できます。保存するのは文字だけで、いつでも消去も無効化もできます。
- Ctrl+S で認識した文字をそのまま .txt や .md ファイルに保存できます。
- 一括認識：複数のファイルをまとめて開くかドロップする、または PDF 全体を読み取り、結果は 1 つにまとめても項目ごとに分けても保存できます。
- PDF は画像と同じように開いて、ページごとに読み取れます。
- クリップボード監視：Win+Shift+S を押すと、Glyfo がその切り取り範囲の文字を読み取り、クリップボードに残します。既定では無効です。
- 文中で見つかったリンク、メールアドレス、電話番号に、それぞれのボタンが付くようになりました。
- 取り込みのショートカットを、組み込みのものではなく自分で決められます。
- ウィンドウは前回閉じたときの大きさと位置で開きます。
- 画像を回転でき、傾いた写真はワンクリックでまっすぐになります。
- Ctrl+F で本文内を検索でき、隣に単語数と文字数が出ます。
- 明るい外観と暗い外観を選べます。Windows に合わせることもできます。

## 한국어 — Korean

- 기록이 다시 시작한 뒤에도 남고 검색할 수 있습니다. 글자만 저장하며, 언제든 지우거나 끌 수 있습니다.
- Ctrl+S로 인식한 글을 바로 .txt나 .md 파일로 저장합니다.
- 일괄 인식: 여러 파일을 한꺼번에 열거나 끌어다 놓고, 또는 PDF 한 권을 통째로 읽어, 결과를 하나로 합치거나 항목마다 따로 저장합니다.
- PDF도 그림과 똑같이 열어 한 쪽씩 읽습니다.
- 클립보드 감시: Win+Shift+S를 누르면 Glyfo가 그 캡처의 글을 읽어 클립보드에 남깁니다. 기본값은 꺼짐입니다.
- 글에서 찾아낸 링크, 메일 주소, 전화번호마다 전용 단추가 생겼습니다.
- 캡처 단축키를 기본 조합 대신 직접 고를 수 있습니다.
- 창이 지난번에 두었던 크기와 자리에서 열립니다.
- 그림을 돌릴 수 있고, 비뚤어진 사진은 한 번 눌러 바로 세웁니다.
- Ctrl+F로 글 안을 찾고, 옆에 낱말 수와 글자 수가 보입니다.
- 밝은 모습과 어두운 모습 중에서 고르거나 Windows를 따르게 할 수 있습니다.

## Bahasa Melayu — Malay

- Sejarah kekal selepas dimulakan semula dan boleh dicari. Teks sahaja, dan boleh dikosongkan atau dimatikan bila-bila masa.
- Ctrl+S menyimpan teks yang dikenali terus ke dalam fail .txt atau .md.
- Pengecaman pukal: buka atau lepaskan beberapa fail, atau baca satu PDF penuh, dan simpan hasilnya bercantum atau satu fail bagi setiap butiran.
- Fail PDF dibuka seperti gambar dan dibaca muka surat demi muka surat.
- Pemantauan papan keratan: tekan Win+Shift+S dan Glyfo membaca teks pada keratan itu serta membiarkannya di papan keratan. Dimatikan secara lalai.
- Pautan, alamat e-mel dan nombor telefon yang ditemui dalam teks kini mempunyai butang tersendiri.
- Pintasan tangkapan kini anda pilih sendiri, bukan yang terbina.
- Tetingkap dibuka pada saiz dan kedudukan terakhir anda meninggalkannya.
- Gambar boleh diputar, dan foto yang senget diluruskan dengan satu klik.
- Cari dalam teks dengan Ctrl+F, dengan bilangan perkataan dan aksara di sebelahnya.
- Pilih rupa cerah atau gelap, atau ikut Windows.

## Norsk bokmål — Norwegian

- Loggen overlever en omstart, og det går an å søke i den. Bare tekst, og den kan tømmes eller slås av.
- Ctrl+S lagrer den gjenkjente teksten rett i en .txt- eller .md-fil.
- Gjenkjenning i bulk: åpne eller slipp flere filer, eller les en hel PDF, og lagre resultatet samlet eller som én fil per element.
- PDF-filer åpnes som bilder og leses side for side.
- Utklippstavleovervåking: trykk Win+Shift+S, så leser Glyfo teksten i utklippet og lar den bli liggende på utklippstavlen. Av som standard.
- Lenker, e-postadresser og telefonnumre som finnes i teksten, får nå egne knapper.
- Snarveiene for skjermklipp velger du selv i stedet for de innebygde.
- Vinduet åpnes i den størrelsen og på det stedet du forlot det.
- Bilder kan roteres, og et skjevt fotografi rettes opp med ett klikk.
- Søk i teksten med Ctrl+F, med ord- og tegntelling ved siden av.
- Velg lyst eller mørkt utseende, eller følg Windows.

## Nederlands — Dutch

- De geschiedenis overleeft een herstart en is doorzoekbaar. Alleen tekst, en je kunt hem wissen of uitschakelen.
- Ctrl+S bewaart de herkende tekst rechtstreeks in een .txt- of .md-bestand.
- Herkennen in batch: open of sleep meerdere bestanden, of lees een hele PDF, en bewaar het resultaat samengevoegd of één bestand per onderdeel.
- PDF-bestanden open je zoals afbeeldingen en lees je pagina voor pagina.
- Klembord in de gaten houden: druk op Win+Shift+S en Glyfo leest de tekst uit de knip en laat die op het klembord staan. Standaard uit.
- Links, e-mailadressen en telefoonnummers in de tekst krijgen nu een eigen knop.
- De sneltoetsen voor schermknipsels kies je zelf in plaats van de ingebouwde.
- Het venster opent in de grootte en op de plek waar je het liet staan.
- Een afbeelding kun je draaien, en een scheve foto zet je met één klik recht.
- Zoek in de tekst met Ctrl+F, met het aantal woorden en tekens ernaast.
- Kies een licht of donker uiterlijk, of volg Windows.

## Polski — Polish

- Historia przetrwa ponowne uruchomienie i można ją przeszukiwać. Wyłącznie tekst, w każdej chwili do wyczyszczenia lub wyłączenia.
- Ctrl+S zapisuje rozpoznany tekst wprost do pliku .txt lub .md.
- Rozpoznawanie wsadowe: otwórz lub upuść wiele plików albo odczytaj cały PDF, a wyniki zapisz scalone lub po jednym pliku na pozycję.
- Pliki PDF otwiera się tak samo jak obrazy i czyta strona po stronie.
- Nasłuch schowka: po naciśnięciu Win+Shift+S Glyfo odczytuje tekst z wycinka i zostawia go w schowku. Domyślnie wyłączony.
- Odnośniki, adresy e-mail i numery telefonów znalezione w tekście mają teraz własne przyciski.
- Skróty do przechwytywania wybierasz samodzielnie, zamiast korzystać z wbudowanych.
- Okno otwiera się w tym samym rozmiarze i miejscu co poprzednio.
- Obraz można obrócić, a przekrzywione zdjęcie wyprostować jednym kliknięciem.
- Wyszukiwanie w tekście przez Ctrl+F, obok liczba słów i znaków.
- Wygląd jasny lub ciemny do wyboru, albo zgodny z Windows.

## Português — Portuguese

- O histórico sobrevive a um reinício e pode ser pesquisado. Apenas texto, e pode ser limpo ou desligado quando quiser.
- Ctrl+S guarda o texto reconhecido diretamente num ficheiro .txt ou .md.
- Reconhecimento em lote: abra ou largue vários ficheiros, ou leia um PDF inteiro, e guarde os resultados juntos ou um ficheiro por item.
- Os PDF abrem-se como imagens e leem-se página a página.
- Vigilância da área de transferência: prima Win+Shift+S e o Glyfo lê o texto do recorte, deixando-o na área de transferência. Desligada de origem.
- As ligações, os endereços de e-mail e os números de telefone encontrados no texto passam a ter botões próprios.
- Os atalhos de captura passam a ser escolhidos por si, em vez dos de origem.
- A janela abre no tamanho e no lugar onde a deixou.
- Uma imagem pode ser rodada, e uma fotografia torta endireita-se com um clique.
- Procure no texto com Ctrl+F, com a contagem de palavras e caracteres ao lado.
- Escolha um aspeto claro ou escuro, ou siga o Windows.

## Română — Romanian

- Istoricul supraviețuiește unei reporniri și poate fi căutat. Doar text, iar el poate fi golit sau oprit oricând.
- Ctrl+S salvează textul recunoscut direct într-un fișier .txt sau .md.
- Recunoaștere în lot: deschideți sau trageți mai multe fișiere ori citiți un PDF întreg, iar rezultatele se salvează unite sau câte un fișier de element.
- Fișierele PDF se deschid ca imaginile și se citesc pagină cu pagină.
- Supravegherea clipboardului: apăsați Win+Shift+S, iar Glyfo citește textul din decupaj și îl lasă în clipboard. Oprită implicit.
- Legăturile, adresele de e-mail și numerele de telefon găsite în text au acum butoane proprii.
- Scurtăturile de captură le alegeți dumneavoastră, nu cele încorporate.
- Fereastra se deschide la dimensiunea și în locul unde ați lăsat-o.
- O imagine poate fi rotită, iar o fotografie strâmbă se îndreaptă dintr-un clic.
- Căutați în text cu Ctrl+F, cu numărul de cuvinte și de caractere alături.
- Alegeți un aspect luminos sau întunecat, ori urmați Windows.

## Русский — Russian

- История переживает перезапуск, и по ней можно искать. Только текст, и её можно очистить или отключить.
- Ctrl+S сохраняет распознанный текст прямо в файл .txt или .md.
- Пакетное распознавание: откройте или перетащите сразу несколько файлов либо прочитайте целый PDF, а результаты сохраните вместе или по файлу на каждый.
- PDF открывается так же, как картинка, и читается страница за страницей.
- Наблюдение за буфером обмена: нажмите Win+Shift+S, и Glyfo прочитает текст с этого снимка и оставит его в буфере. По умолчанию выключено.
- У найденных в тексте ссылок, адресов почты и телефонов теперь есть собственные кнопки.
- Сочетания клавиш для снимка вы выбираете сами, а не берёте встроенные.
- Окно открывается там же и такого же размера, каким вы его оставили.
- Изображение можно повернуть, а перекошенный снимок выпрямить одним щелчком.
- Поиск по тексту через Ctrl+F, рядом счётчик слов и знаков.
- Выберите светлый или тёмный вид либо следуйте за Windows.

## Svenska — Swedish

- Historiken överlever en omstart och går att söka i. Bara text, och den kan tömmas eller stängas av.
- Ctrl+S sparar den avlästa texten direkt till en .txt- eller .md-fil.
- Avläsning i grupp: öppna eller släpp flera filer, eller läs av en hel PDF, och spara resultatet sammanslaget eller som en fil per post.
- PDF-filer öppnas som bilder och läses av sida för sida.
- Bevakning av urklipp: tryck Win+Skift+S, så läser Glyfo av texten i klippet och låter den ligga kvar i urklipp. Avstängd från början.
- Länkar, e-postadresser och telefonnummer i texten får nu egna knappar.
- Kortkommandona för skärmklipp väljer du själv i stället för de inbyggda.
- Fönstret öppnas i den storlek och på den plats där du lämnade det.
- En bild går att vrida, och ett snett foto rätas upp med ett klick.
- Sök i texten med Ctrl+F, med antal ord och tecken bredvid.
- Välj ett ljust eller mörkt utseende, eller följ Windows.

## ไทย — Thai

- ประวัติอยู่รอดข้ามการเริ่มระบบใหม่และค้นหาได้ เก็บเฉพาะข้อความ ล้างหรือปิดเมื่อใดก็ได้
- Ctrl+S บันทึกข้อความที่อ่านได้ลงไฟล์ .txt หรือ .md โดยตรง
- อ่านทีละหลายไฟล์ เปิดหรือลากไฟล์เข้ามาพร้อมกัน หรืออ่าน PDF ทั้งเล่ม แล้วบันทึกผลรวมเป็นไฟล์เดียวหรือแยกไฟล์ต่อรายการ
- เปิดไฟล์ PDF ได้เหมือนเปิดรูป แล้วอ่านทีละหน้า
- เฝ้าดูคลิปบอร์ด กด Win+Shift+S แล้ว Glyfo จะอ่านข้อความในภาพที่ตัดมาและวางไว้ในคลิปบอร์ด ปิดไว้ตั้งแต่แรก
- ลิงก์ ที่อยู่อีเมล และเบอร์โทรที่พบในข้อความ มีปุ่มของตัวเองแล้ว
- เลือกปุ่มลัดสำหรับจับภาพได้เอง แทนของที่ติดมา
- หน้าต่างเปิดขึ้นในขนาดและตำแหน่งเดิมที่คุณทิ้งไว้
- หมุนรูปได้ และรูปถ่ายที่เอียงก็ตั้งตรงได้ในคลิกเดียว
- ค้นในข้อความด้วย Ctrl+F พร้อมจำนวนคำและตัวอักษรอยู่ข้าง ๆ
- เลือกหน้าตาแบบสว่างหรือมืด หรือให้ตามระบบ Windows

## Türkçe — Turkish

- Geçmiş yeniden başlatmadan sonra da duruyor ve içinde arama yapılabiliyor. Yalnızca metin tutar, istediğinizde temizlenir veya kapatılır.
- Ctrl+S tanınan metni doğrudan bir .txt veya .md dosyasına kaydeder.
- Toplu tanıma: birden çok dosyayı açın ya da sürükleyin, veya bir PDF’in tamamını okuyun; sonuçları birleştirerek ya da her öğe için ayrı dosya olarak kaydedin.
- PDF dosyaları resim gibi açılır ve sayfa sayfa okunur.
- Pano izleme: Win+Shift+S tuşlarına basın, Glyfo o kırpmadaki metni okuyup panoda bırakır. Başlangıçta kapalıdır.
- Metinde bulunan bağlantılar, e-posta adresleri ve telefon numaraları artık kendi düğmelerine sahip.
- Yakalama kısayollarını yerleşik olanlar yerine kendiniz seçiyorsunuz.
- Pencere, bıraktığınız boyutta ve yerde açılıyor.
- Resim döndürülebiliyor, eğri çekilmiş bir fotoğraf tek tıklamayla doğrultuluyor.
- Ctrl+F ile metinde arayın; yanında sözcük ve karakter sayısı görünür.
- Açık ya da koyu bir görünüm seçin, veya Windows’u izleyin.

## Українська — Ukrainian

- Історія переживає перезапуск, і в ній можна шукати. Лише текст, і її будь-коли можна очистити чи вимкнути.
- Ctrl+S зберігає розпізнаний текст просто у файл .txt або .md.
- Пакетне розпізнавання: відкрийте чи перетягніть кілька файлів або прочитайте цілий PDF, а результати збережіть разом чи по файлу на кожен.
- PDF відкривається так само, як зображення, і читається сторінка за сторінкою.
- Стеження за буфером обміну: натисніть Win+Shift+S, і Glyfo прочитає текст із того знімка та залишить його в буфері. Типово вимкнено.
- Посилання, поштові адреси й телефони, знайдені в тексті, тепер мають власні кнопки.
- Сполучення клавіш для знімка ви обираєте самі, а не берете вбудовані.
- Вікно відкривається того ж розміру й на тому ж місці, де ви його лишили.
- Зображення можна повернути, а перекошене фото вирівняти одним клацанням.
- Пошук у тексті через Ctrl+F, поруч лічильник слів і символів.
- Оберіть світлий чи темний вигляд або слідуйте за Windows.

## Tiếng Việt — Vietnamese

- Lịch sử vẫn còn sau khi khởi động lại và có thể tìm kiếm. Chỉ lưu chữ, và có thể xóa hoặc tắt bất cứ lúc nào.
- Ctrl+S lưu phần chữ đã nhận dạng thẳng vào tệp .txt hoặc .md.
- Nhận dạng hàng loạt: mở hoặc thả nhiều tệp cùng lúc, hoặc đọc trọn một tệp PDF, rồi lưu kết quả gộp chung hay mỗi mục một tệp.
- Tệp PDF mở ra như ảnh và đọc từng trang một.
- Theo dõi bảng nhớ tạm: nhấn Win+Shift+S, Glyfo sẽ đọc chữ trong phần vừa cắt và để lại trong bảng nhớ tạm. Mặc định tắt.
- Các liên kết, địa chỉ e-mail và số điện thoại tìm thấy trong văn bản nay có nút riêng.
- Phím tắt chụp màn hình do bạn tự chọn thay cho phím có sẵn.
- Cửa sổ mở ra đúng kích thước và vị trí bạn để lại.
- Ảnh có thể xoay, và ảnh chụp bị nghiêng được dựng thẳng chỉ bằng một cú nhấp.
- Tìm trong văn bản bằng Ctrl+F, bên cạnh có số từ và số ký tự.
- Chọn giao diện sáng hoặc tối, hoặc theo Windows.
