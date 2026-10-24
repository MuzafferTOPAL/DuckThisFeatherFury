# DuckThis Feather Fury: QA Test Raporu v3 (görünmez tilki düzeltmeleri sonrası)

**Test tarihi:** 24.10.2026
**Test edilen düzeltmeler:**
1. Sağ kenarı (maxX + 1,2) aşan ya da zeminin 2,5 birim altına düşen tilki elenmiş sayılıyor.
2. L41+ her dünyanın 4., 11. ve 18. bölümüne 3–8 katlı "gökdelen" eklendi.
3. Kamera yüksek yapıları sığdırmak için uzaklaşıyor.

**Yöntem (Unity editörü, yerleşik `autotest` botu):**
- Stabilite: `autotest 1 500 0` (500 bölüm).
- Oynanış: 25 dünyanın her biri `autotest A B 3 1` (UPG=1, 1500 deneme).
- 0/3 kalan 91 bölüm 6'şar denemeyle yeniden oynandı (`autotest N N 6 1`; art arda gelen 0/3'ler tek komutta, örn. `autotest 50 52 6 1`, sonuç bölüm başına aynı). Toplamda 0/9 kalan bölüm "doğrulanmış duvar" sayıldı.
- Ekran görüntüsü: L13 (başlangıç, 2 atış, 4 atış), 20 duvar, 18 gökdelen bölümü. 5 duvarda (L2, L28, L90, L100, L460) ördekler bitene kadar `aimshot` atılıp her atıştan sonra görüntü alındı.
- Ham çıktılar projede: `_out/v3_stab.txt`, `_out/v3_w01..25.txt`, `_out/v3_re_*.txt`, birleşik `_out/v3_all_main.txt` ve `_out/v3_all_re.txt`. Görüntüler `Logs/v3_*.png`.
- Bölüm başına tablo: `levels_v3.csv` (ana koşu, 3 deneme).

> Not: Unity arka plana düştüğünde (ekran kilidi / başka pencere) test duruyor ya da play moduna geri giremiyordu. Unity öne getirilerek ve komut yeniden gönderilerek devam edildi; sonuçlar etkilenmedi.

---

## 1. Ana soru: Bildirilen türde bölümler ("TİLKİ: 1" ama ekranda tilki yok) hâlâ çıkar mı?

### Cevap: **EVET, hâlâ çıkabiliyor.** Eskisinden daha seyrek, ama hata tamamen kapanmadı.

**Kanıt 1, L460 (Batık Lagün 20, boss) birebir yeniden üretildi:**
- 6. atış (`v3_rep_460_s6.png`): HUD "TİLKİ: 3", üç tilki görünüyor. Biri ekranın en sağında (x≈990 px), sağa doğru itilmiş.
- 8. atış (`v3_rep_460_s8.png`): HUD **"TİLKİ: 2"**, ama ekranda **yalnızca 1 tilki** var. Sağ kenar büyütülerek kontrol edildi; orada tilki yok. Ekranın üstünde ya da solunda da tilki yok.
- 10. atış (`v3_rep_460_s10.png`): "TİLKİLER KAZANDI!", HUD "TİLKİ: 1". Bölüm, görünmeyen ve hayatta kalan tilki yüzünden kaybedildi.

**Kanıt 2, L28 (Deniz Feneri):**
- 1.–4. atış boyunca son tilki ekranın sağ kenarına sıkışmış ve **yarısı ekran dışında**.
- Bölüm "TİLKİ: 1" ile kaybedildi (`v3_rep_28_s3.png`, `v3_rep_28_s4.png`). Bot ve oyuncu bu tilkiyi neredeyse göremiyor.

**Kök neden (kod incelemesiyle doğrulandı):**
- `CameraRig.BaseView` görüntüyü tam olarak `[-11,5 , maxX]` aralığına sığdırıyor. Bölüm genişliği belirleyici olduğunda ekranın sağ kenarı **tam maxX**.
- Yeni eleme çizgisi ise `Destructible.cs:46` satırında **maxX + 1,2**.
- Tilki yaklaşık 1 birim genişliğinde. Dolayısıyla merkezi **maxX + 0,5 ile maxX + 1,2** arasında duran bir tilki tamamen görünmez ama canlı kalıyor. maxX ile maxX + 0,5 arasında duran tilki ise yarı görünür (L28'deki durum).
- Zemin 140 birim genişliğinde. İtilen tilki kayıp tam bu 0,7 birimlik şeritte durabiliyor.
- **Sol taraf** için yalnızca |x| > 70 kontrolü var. Sapana doğru fırlatılan bir tilki ekranın solunda da canlı kalabilir; bu testte gözlenmedi.

**Bildirilen bölüm L13 "KALE DUVARI":**
- Ana koşuda **1/3 kazanıldı** (v2'de 0/9 idi), 6/7 atışla.
- Başlangıçta HUD "TİLKİ: 3" ve 3 tilki görünüyor; 2 atıştan sonra "3" ve 3 tilki; 4 atıştan sonra "2" ve 2 tilki. Hepsi tutarlı.
- Ancak L13'te miğferli tilki x = 10,6'da, sağ kenara (12,5) yalnızca 1,9 birim uzakta. Yandan vurulursa yukarıdaki şeride itilebilir. L13 artık bitirilebilir, ama aynı risk sürüyor.

**Düzeltme 1'in olumlu etkisi:**
- Toplam kazanma %49,3'ten **%62,6**'ya çıktı (939/1500).
- 0/3 bölüm sayısı 145'ten **91**'e indi.
- Bu artışın bir kısmı, sağa itilen tilkilerin artık sayılması sayesinde. Yani düzeltme çalışıyor, sadece eşik ekran kenarıyla örtüşmüyor.

---

## 2. Stabilite

- `autotest 1 500 0` sonucu: **500/500 SAĞLAM**. Kullanıcının koşusu doğrulandı.
- **Ancak stabilite testi gökdelenlerdeki bir sorunu kaçırıyor** (bkz. §3). Test, 2. kare ile 2,5. saniye arasını ölçüyor. Gökdelen çatısındaki tilki 3–8. saniyede, oyuncu daha atış yapmadan düşüyor.

---

## 3. Gökdelen bölümleri

| Soru | Sonuç |
|---|---|
| Var mı? | **Evet.** L41+ bölümlerde 4., 11. ve 18. slotlarda (69 bölüm) mevcut. İncelenen 18 bölümün hepsinde 3–8 katlı kule görüldü: 44, 51, 64, 84, 91, 98, 131, 171, 204, 218, 264, 311, 338, 358, 404, 431, 471, 498. |
| Stabil mi (autotest)? | Hepsi SAĞLAM. |
| Stabil mi (görsel)? | **Hayır, 18 bölümün 8'inde kule kendiliğinden bozuluyor.** Ayrıntılar aşağıda. |
| Kameraya sığıyor mu? | **Evet, 18/18.** En yüksek kulelerin (L91, L311, L64) çatısı HUD bandının hemen altında kalıyor; kesilen bir parça yok. |
| Zorluk | Gökdelen slotları %76,8 kazanma (v2'de aynı slotlar %50,7). 7 bölüm 0/3 kaldı, tekrar testinden sonra yalnızca **L391** duvar (0/9). |

**Kendiliğinden bozulan kuleler (7–8 katlı):**
- Çatıdaki tilki giriş animasyonu sırasında (oyuncu ateş etmeden) düşüyor.
- Düşüp öldü ve oyuncuya bedava puan verdi: L64, L91, L131, L311 (5.000 puan), L44 (yavru tilki, 3.000 puan).
- Düştü ama yaşıyor: L84 ve L218'de çatı tilkisi **sapanın önüne**, sol tarafa düştü. L264'te tilki görüntü anında havada düşüyordu.
- Kule gözle görülür eğik: L91, L264 ve L311 (giriş ekranında).
- Çatı tilkisi kenarda sallanıyor: L431, L498.
- Bu durum bölümü kazanılmaz yapmıyor (çoğunlukla kolaylaştırıyor), ama:
  - Oyuncuya "bozuk bölüm" izlenimi veriyor.
  - Sol tarafa düşen tilki ileride solda görünmez tilki riskine dönüşebilir.

---

## 4. Dünya bazında tablo (UPG=1, 3 deneme; v2 ile karşılaştırma)

"Duvar" = 0/3 + tekrar 0/6 (toplam 0/9). Boss sütununda tekrar sonucu `+x/6` olarak verildi.

| Dünya | Bölümler | Kazanma v3 | Kazanma v2 | Fark | 0/3 | Duvar | Boss | Tek satır karar |
|---|---|---|---|---|---|---|---|---|
| 1 | 1–20 | %70,0 | %65,0 | +5 | 1 | 1* | 3/3 | Dostane; L13 artık geçiliyor. *L2 bot hatası, elle kazanıldı. |
| 2 | 21–40 | %51,7 | %43,3 | +8 | 4 | 3 | 1/3 | 28/30/37 hâlâ duvar (az ördek, taş kule). |
| 3 | 41–60 | %31,7 | %31,7 | 0 | 9 | 6 | **0/3, +0/6** | **Hiç düzelmedi**; oyunun en zor dünyası, boss da duvar. |
| 4 | 61–80 | %65,0 | %58,3 | +7 | 4 | 3 | 3/3 | İyi. |
| 5 | 81–100 | %43,3 | %23,3 | **+20** | 5 | 5 | **0/3, +0/6** | Büyük iyileşme, ama 5 duvar ve duvar boss. |
| 6 | 101–120 | %46,7 | %40,0 | +7 | 8 | 6 | **0/3, +0/6** | 102–108 bloğu hâlâ duvar; boss duvar. |
| 7 | 121–140 | %56,7 | %38,3 | +18 | 5 | 2 | 2/3 | 4'lü seri kırıldı. |
| 8 | 141–160 | %66,7 | %36,7 | **+30** | 1 | 1 | 3/3 | Çok iyi; boss artık adil. |
| 9 | 161–180 | %63,3 | %51,7 | +12 | 1 | 0 | 1/3 | Duvarsız. |
| 10 | 181–200 | %75,0 | %53,3 | +22 | 3 | 2 | 3/3 | Kolaylaştı. |
| 11 | 201–220 | %83,3 | %75,0 | +8 | 1 | 1 | 3/3 | **Fazla kolay** (oyunun en kolayı). |
| 12 | 221–240 | %76,7 | %68,3 | +8 | 2 | 1 | 3/3 | Kolay. |
| 13 | 241–260 | %65,0 | %53,3 | +12 | 4 | 1 | 3/3 | Dengeli. |
| 14 | 261–280 | %61,7 | %48,3 | +13 | 4 | 4 | 3/3 | 4 duvar (taş piramit, kalkanlı tilki). |
| 15 | 281–300 | %68,3 | %60,0 | +8 | 5 | 3 | 2/3 | Boss düzeldi; 3 duvar. |
| 16 | 301–320 | %71,7 | %60,0 | +12 | 1 | 1 | 2/3 | İyi. |
| 17 | 321–340 | %50,0 | %50,0 | 0 | 5 | 2 | 3/3 | Değişmedi, orta-zor. |
| 18 | 341–360 | %76,7 | %53,3 | +23 | 1 | 1 | 3/3 | Kolaylaştı. |
| 19 | 361–380 | %65,0 | %45,0 | +20 | 5 | 2 | 3/3 | İyileşti. |
| 20 | 381–400 | %56,7 | %50,0 | +7 | 5 | 3 | 1/3 | 386/391/399 duvar. |
| 21 | 401–420 | %75,0 | %46,7 | +28 | 1 | 1 | 3/3 | Çok kolaylaştı. |
| 22 | 421–440 | %65,0 | %43,3 | +22 | 3 | 3 | 3/3 | 3 duvar kaldı. |
| 23 | 441–460 | %51,7 | %40,0 | +12 | 6 | 4 | **0/3, +0/6** | Geç oyunun en zoru; boss duvar ve görünmez tilki burada üretildi. |
| 24 | 461–480 | %65,0 | %43,3 | +22 | 3 | 3 | 3/3 | 3 duvar (466/469/474). |
| 25 | 481–500 | %63,3 | %53,3 | +10 | 4 | 3 | 2/3 | Final hâlâ W23'ten kolay. |
| **Toplam** | 1–500 | **%62,6** | %49,3 | +13 | **91** (v2: 145) | **62** (L2 hariç 61) | 4 duvar boss | |

**Ek gözlemler:**
- 100'lük bloklarda kazanma: 1–100 %52 · 101–200 %62 · 201–300 %71 · 301–400 %64 · 401–500 %64. Zorluk eğrisi L200'den sonra yükselmiyor, düşüyor.
- Ördek sayısına göre (L41+):

| Ördek | Bölüm | Kazanma | 0/3 | Duvar |
|---|---|---|---|---|
| 5 | 222 | %68 | 34 | 22 |
| 6 | 155 | %51 | 41 | 28 |
| 7 | 43 | %64 | 7 | 4 |

  6 ördekli grup hâlâ en zor grup (v2'de %33'tü).
- Yıldız dağılımı (L41+, kazanılanlar): 3★ %50, 2★ %19, 1★ %31.

---

## 5. Doğrulanmış duvarlar (0/9)

Toplam 62 bölüm. L2 elle `aimshot` ile 5 atışta 2★ kazanıldı ve oyuncunun kayıtlı en iyi skoru 30.500. Bu nedenle L2 gerçek bir duvar değil, botun rastgele nişan hatası. **Gerçek duvar sayısı: 61.**

### 5.1 Ekran görüntüsüyle incelenen duvarlar

| Bölüm | Tilki / Ördek | Görüntüde ne var | Neden kazanılmıyor |
|---|---|---|---|
| L2 | 4 / 5 | İki tahta çerçeve | **Bot hatası**; elle kazanıldı. |
| L28 | 5 / 4 | 4 katlı kule + sağ kenarda çerçeve | Az ördek; **sağ kenardaki tilki yarı ekran dışına itiliyor** (görünmez tilki şeridi). |
| L37 | 6 / 5 | Taş kule (3 kat, 2 miğferli) + 2 taş blok | Az ördek, taş kule. |
| L43 | 3 / 4 | Taş çatılı iki çerçeve, tilkiler çatı altında | Kalkanlı tilki; ortalama hasar yalnızca 250–370. |
| L47 | 7 / 5 | 3 katlı piramit (taş taban, buz üst) | Piramit çok güçlü, az ördek. |
| L55 | 7 / 6 | Taş/buz çerçeveler + sağda ikinci kale | Çok tilki, az ördek. |
| L90 | 8 / 7 | 6 miğferli tilki; 3'ü sağda taş kaidelerde | Az ördek + miğfer. Ördekler bittiğinde 6 tilki görünür durumda kaldı. |
| L100 (boss) | 8 / 8 | Boss tilki (28 can) + 6 miğferli | Boss çok güçlü; 9 atışta 2 tilki öldü. |
| L120 (boss) | 8 / 9 | 4 ayrı yapı, ekran genişliğine yayılmış | Boss + 6 miğferli, dağınık hedef. |
| L129 | 7 / 5 | Piramit + çatı tilkisi | Piramit çok güçlü. |
| L193 | 5 / 6 | Tahta kule + taş kulübeler (miğferli) | Miğferli tilkiler taş arkasında. |
| L275 | 6 / 5 | **Tamamen taş 3 katlı piramit**, 5 miğferli | Piramit çok güçlü. |
| L277 | 3 / 5 | Taş çatı + buz/taş sütun arkasında miğferli tilkiler | Kalkanlı tilki; hasar yalnızca ~650–720. |
| L309 | 6 / 6 | 3 yapı, 4 miğferli | Miğferli tilki + taş çatı. |
| L375 | 6 / 6 | L275 ile aynı taş piramit | Piramit çok güçlü. |
| L450 | 6 / 6 | Piramit (taş + tahta + buz) | Piramit çok güçlü. |
| L452 | 6 / 5 | Taş piramit | Piramit çok güçlü; hasar ~670–800. |
| L460 (boss) | 8 / 9 | 3 yapı + 2 TNT | Boss güçlü **ve görünmez tilki hatası burada yeniden üretildi** (bkz. §1). |
| L489 | 3 / 5 | Taş blokların arkasında taş kulübeler, miğferli | Kalkanlı tilki; hasar ~840–1.090. |
| L494 | 6 / 6 | L275 ile aynı taş piramit | Piramit çok güçlü. |

Hiçbir görüntüde **tamamen taşla mühürlenmiş, hiçbir yönden ulaşılamayan tilki** görülmedi. Kalkanlı tilkilere (43, 277, 489) yüksek yay atışıyla ulaşılabiliyor, ama hasar çok düşük kalıyor.

### 5.2 Tam duvar listesi (0/3 + 0/6)

| Dünya | Duvarlar |
|---|---|
| W1 | 2 (bot hatası) |
| W2 | 28, 30, 37 |
| W3 | 43, 47, 50, 54, 55, **60 (boss)** |
| W4 | 63, 66, 72 |
| W5 | 82, 83, 90, 96, **100 (boss)** |
| W6 | 102, 105, 107, 108, 117, **120 (boss)** |
| W7 | 129, 137 |
| W8 | 154 |
| W10 | 193, 196 |
| W11 | 208 |
| W12 | 227 |
| W13 | 250 |
| W14 | 265, 272, 275, 277 |
| W15 | 282, 289, 297 |
| W16 | 309 |
| W17 | 325, 339 |
| W18 | 356 |
| W19 | 369, 375 |
| W20 | 386, 391 (gökdelen), 399 |
| W21 | 412 |
| W22 | 423, 433, 436 |
| W23 | 443, 450, 452, **460 (boss)** |
| W24 | 466, 469, 474 |
| W25 | 488, 489, 494 |

- Bu 62 bölümün 46'sı v2'de de 0/3 idi. 16'sı yeni; bu 3 denemelik örneklemin gürültüsünden geliyor olabilir.
- Görüntülenmeyen duvarlarda ortalama hasara bakıldığında iki grup var:
  - **Düşük hasarlı (<1.000):** 2, 43, 47, 50, 277, 452, 466, 489. Muhtemelen kalkanlı ya da ulaşılması zor tilki.
  - **Yüksek hasarlı (>1.800):** 72, 83, 90, 100, 120, 196, 227, 275, 282, 386, 391, 460, 469, 488. Yapı yıkılıyor ama ördek yetmiyor (çok tilki / miğfer).

---

## 6. Düzeltme önerileri (sorun türüne göre)

### A. Görünmez tilki (kritik, oyuncu şikâyetinin kaynağı)

1. **Eleme çizgisini ekranla eşitle.**
   - `Destructible.cs:46` satırındaki `FieldMaxX + 1.2f` eşiği gerçek kamera sağ kenarından türetilmeli: `cam.transform.position.x + cam.orthographicSize * cam.aspect`.
   - Daha basit yol: eşiği `FieldMaxX + 0.4f` yapmak (tilkinin yarısı ekran dışına çıktığında elensin).
   - Alternatif: kamerayı `maxX + 1.5` genişliğe sığdırmak (`CameraRig.Configure(..., Level.maxX + 1.5f, ...)`).
2. **Sol ve üst kenarı da kapsa.** `x < -12.5` ya da `y > kamera üstü + 3` olup 2 saniye boyunca uyuyan (`rb.IsSleeping()`) tilki de elensin.
3. **Güvenlik ağı ekle:** Ördekler bittiğinde ekran dışında kalan canlı tilkileri otomatik ele; ya da ekran dışındaki her canlı tilki için kenarda ok göstergesi göster.
4. **AutoTester'a kontrol ekle:** Kayıp anında `EnemiesLeft > 0` iken kalan tilkilerden biri kamera görüş alanının dışındaysa satıra `GÖRÜNMEZ TİLKİ` yazdır. Böylece bu hata 1500 denemede otomatik sayılabilir.

### B. Gökdelen çatı tilkisi düşüyor

1. `TallTower` için kat sayısını en fazla 6'ya indir, ya da 6+ katta çatı tilkisini kaldır.
2. Üst katlarda tahta yerine taş kiriş ve geniş taban kullan.
3. Seviye yüklenince fizik gövdelerini ilk atışa kadar uyut (`rb.Sleep()` / `simulated`); ya da tilkilerin `spawnGrace` süresini giriş animasyonu bitene kadar uzat.
4. Stabilite testinin bekleme süresini 2,5 s'den **8 s**'ye çıkar ve başlangıç puanını kontrol et. "Atış öncesi PUAN > 0" ifadesi bir SORUN göstergesidir.

### C. "Piramit" şablonu çok güçlü (47, 129, 275, 375, 450, 452, 494)

- 6–7 tilki ve bunların 5'i miğferli, buna karşılık 5–6 ördek.
- Öneriler:
  - Piramitte miğferli tilki oranını en fazla %50 ile sınırla.
  - Orta katı taştan tahtaya çevir, ya da tabana bir TNT koy.
  - Ördek sayısını `tilki + 1` yap.

### D. Az ördek / çok tilki (28, 37, 55, 90 vb.)

- `PickDucks` için kural: **ördek ≥ tilki sayısı + (miğferli tilki / 2)**.
- 6 ördekli bölümlerde (%51 kazanma) tilki sayısını 5 ile sınırla.

### E. Kalkanlı tilki (43, 277, 489)

- Her tilkinin sapandan en az bir açık atış hattı (yay ya da düz) olmalı.
- Taş çatı + önde taş blok kombinasyonunu yasakla, ya da bunlardan birini tahtaya çevir.
- L43 el yapımı; elle düzeltilmeli (v2'den beri duvar).

### F. Duvar boss'lar (60, 100, 120, 460)

- Boss canını 28'den 20'ye indir, ya da boss bölümünde miğferli tilki sayısını en fazla 3 yap.
- Alternatif: boss bölümüne +2 ördek ver.
- L220 ve L420 boss'ları tek atışta geçiliyordu (v2). Boss zorluğu D değerine değil, tilki sayısı / ördek oranına göre ayarlanmalı.

### G. Denge

- W3 (%31,7) hiç düzelmedi ve 6 duvarı var. 41–60 aralığında tilki sayısını 1 azalt.
- W11 (%83) ve W21 (%75) fazla kolay; geç oyunda eğri yükselmiyor.

### H. Küçük

- L2'nin başlığı log'da "İKİZ KULELER", oyunda "İKİ KULE". Yerelleştirme anahtarıyla eşleşmiyor.
