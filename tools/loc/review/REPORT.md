# DuckThis Feather Fury: Yerelleştirme Kalite İncelemesi (LQA)

İncelenen: l_en_tr.py, l_eu.py, l_latin2.py, l_cyr.py, l_cjk.py, l_indic.py, l_rtl_th.py ve lvl_{en,tr,de,es,fr,pt,it}.json. 30 dil.
Makine tarafından okunabilir düzeltmeler: fixes.json (78 değişiklik). Oyuna uygulanan hali: `tools/loc/fixes_lqa.json`.

> **Uygulama notu:** Türkçede "BÖLÜM = dünya/sahne, LEVEL = level" ayrımı geliştiricinin bilinçli tercihi olduğu için
> world/level/levels/restart_level/level_done/unlock_at/locked_world/gift_hint önerileri **uygulanmadı**.
> Diğer 70 düzeltme uygulandı.

Güven düzeyi: Avrupa dilleri, ru/uk ve CJK için yüksek. Hint dilleri, th, ar, ur ve fa için orta. **sw, ha, fil ve id kuş adları için düşük.** Bu dillerde puanlar bilinçli olarak temkinli. Yayından önce ana dil konuşanına kontrol ettirin.

## Özet tablo (düzeltmelerden önce)
| Dil | Dilbilgisi | Yerel kullanım | Ördek adları | Genel | Karar |
|---|---|---|---|---|---|
| en | 10 | 9.5 | 9.5 | 9.5 | Referans metin temiz. |
| tr | 8.5 | 7 | 9 | 8 | Bölüm/level terimleri; DÜKKAN ve YETERSİZ YUMURTA doğal değil. |
| de | 9.5 | 8 | 10 | 9 | Çok iyi. LADEN ve BESITZ düzeltilmeli. |
| es | 8.5 | 7.5 | 10 | 8.5 | İspanya ve Latin Amerika İspanyolcası karışık. "NO TIENES SUFICIENTES HUEVOS" argo bir deyim. |
| fr | 9.5 | 9 | 10 | 9.5 | Mükemmel. |
| pt | 9 | 8.5 | 7.5 | 8.5 | 3 ördek adı yanlış. |
| it | 9 | 9 | 10 | 9.5 | POSSEDUTE'de cinsiyet uyumu yanlış. |
| pl | 8.5 | 8 | 8.5 | 8.5 | RODENIEC → ROŻENIEC yazım hatası; sayı uyumu sorunu. |
| id | 8.5 | 8 | 6 | 7.5 | BELIBIS yanlış kuş cinsinin adı; PERTANIAN "tarım" demek, çiftlik değil. |
| fil | 8 | 7.5 | 7 | 7.5 | Kabul edilebilir Taglish. |
| sw | 6.5 | 6.5 | 5 | 6 | BATA MZINGA "hindi" demek; UKIRUKA'nın öznesi yanlış. Ana dil kontrolü şart. |
| ha | 7 | 6.5 | 6 | 6.5 | OYNA ve AÇIK aynı kelime (KUNNA). Ana dil kontrolü şart. |
| vi | 8.5 | 8.5 | 8 | 8.5 | Tonlar doğru; 1 ördek adı ve 1 level adı yanlış. |
| ru | 9 | 9 | 10 | 9 | tip_nest'te sayı uyumu. |
| uk | 9 | 8.5 | 10 | 9 | Birkaç Rusçaya kaçan ifade var. |
| zh-Hans | 9.5 | 9 | 10 | 9.5 | Karakter karışması yok. |
| zh-Hant | 9.5 | 9 | 9.5 | 9.5 | Tayvan kullanımına uygun. |
| ja | 10 | 9.5 | 10 | 9.5 | Kusursuza yakın. |
| ko | 9 | 8.5 | 9 | 9 | 을(를) kalıbı; "다시" yeniden başlat butonu için zayıf. |
| hi | 9 | 8.5 | 8 | 8.5 | Doğal. |
| bn | 9 | 8.5 | 8.5 | 8.5 | Bengal ve Batı rakamları karışık. |
| mr | 8.5 | 8 | 8 | 8 | Biraz resmi. |
| te | 8 | 7.5 | 8 | 8 | HAYIR için కాదు yanlış. |
| ta | 8.5 | 7.5 | 8 | 8 | AÇIK/KAPALI fiil olarak çevrilmiş. |
| gu | 8 | 8 | 8 | 8 | Alt başlık "kış yumurtaları çaldı" diye okunuyor. |
| pa | 8.5 | 8 | 8 | 8 | Bazı etiketler uzun. |
| th | 9 | 9 | 8.5 | 9 | Doğal. |
| ar | 8.5 | 8.5 | 9 | 8.5 | Sayı-isim uyumu sorunu; egg_short tekil. |
| ur | 8.5 | 8 | 8.5 | 8.5 | MAKS etiketi uzun. |
| fa | 9 | 8.5 | 8 | 8.5 | مرگوس yerine yerel ad kullanılmalı. |

## Uygulanan başlıca düzeltmeler
- **tr:** MAĞAZA, TEKRAR, YETERLİ YUMURTA YOK!, AHŞAP KULE, YUVARLANAN KAYALAR
- **de:** SHOP, + ENTE, IM BESITZ
- **es:** ¡HUEVOS INSUFICIENTES!, OK, GOLPE AL SUELO, BONUS PRIMERA VICTORIA, EN VUELO SE DIRIGE A LOS ZORROS, … DE LOS ZORROS
- **fr:** REJOUER
- **pt:** NEGROLA, PATO-FERRUGÍNEO, ARRÁBIO
- **it:** IN POSSESSO, SBATTE LE ALI E SALE
- **pl:** ROŻENIEC, sayı uyumu, + KACZKA, W KAŻDYM ŚWIECIE
- **id:** MERGANSER BESAR, BEBEK EKOR LANCIP, LADANG SENJA
- **sw:**
  - BATA KICHWA-KIJANI
  - AKIRUKA (öznenin ördek olduğu hal)
  - HUJIGAWA KUWA WATATU
  - WIMBI LA MSHTUKO
  - UPEO
- **ha:** FARA WASA (oyna), JAR AGWAGWA
- **vi:** VỊT MỐC (kılkuyruk), soru cümlesi, level adı
- **ru/uk:** tip_nest sayı uyumu, ТЯЖЕЛОВЕС / ВАЖКОВАГОВИК, ТРОЩИТЬ, СТОДОЛА
- **zh:** 第一击 / 第一擊
- **ko:** 월드 {0} 클리어 필요, 재시작
- **te:** లేదు
- **ta:** ஆன் / ஆஃப், லாவா பாலம்
- **gu:** શિયાળ ઈંડાં ચોરી ગયાં!
- **pa:** ਅਧਿਕਤਮ
- **ar:** بيض ذهبي +{0}, بيض, الحد الأقصى, لديك
- **fa:** اردک‌ماهی‌خوار بزرگ

## Açık kalanlar (ana dil kontrolü önerilir)
- **sw:** owned ULIO NAO, W22 RASI, LVL0 RISASI
- **ha:** KOGON MACIJI, WAHAR GIZO, DALA
- **id:** BERUWOK MERAH
- **pt:** ÊIDER-EDREDÃO
- **ko:** 솜털오리 / 참솜깃오리
- **fa:** اردک سیاه
- **Rakam karışıklığı:** bn ve fa'da yerel rakamlar ile Batı rakamları karışık.
