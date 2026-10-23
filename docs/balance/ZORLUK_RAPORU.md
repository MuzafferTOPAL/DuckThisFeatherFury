# DuckThis Feather Fury: Zorluk ve Denge Raporu

**Test tarihi:** 23.10.2026
**Yöntem:** Unity editöründe yerleşik otomatik oyuncu (`autotest`) kullanıldı.
- 500 bölümün tamamı oynandı: her bölüm 3 deneme, UPG=1 (oyuncunun o dünyada alabileceği tahmini yükseltmelerle). Toplam 1500 deneme yapıldı.
- Mağazasız karşılaştırma için 1, 5, 10, 15, 20 ve 25. dünyalar UPG=0 ile 2'şer deneme daha oynandı.
- Şüpheli bölümler tekrar koşturuldu: 13, 18, 40, 43 ve 99 (6'şar deneme) ile 50–56 (3'er deneme).
- Ham veri: `levels.csv` (her bölüm için bir satır). Bot sonuçlarının kopyası projede `_out/world_NN.txt`, `_out/noupg_NN.txt` ve `_out/re_*.txt` dosyalarında.

> **Not:** Bot rastgele bir tilkiye gürültülü nişan alan zayıf bir oyuncu. Yorumlama ölçeği şöyle: bot kazanma oranı ≥%70 ise insan için kolay, %40–70 orta, %20–40 zor, <%15 ise çok zor ya da olası duvar. Her bölümde yalnızca 3 deneme olduğu için tek bölüm sonuçları gürültülü. Örneğin 51. bölüm ilk koşuda 0/3, tekrarda 1/3; 52. bölüm 0/3'ten 2/3'e çıktı. Bu yüzden asıl değerlendirme dünya ortalamaları ve art arda gelen 0-galibiyet serileri üzerinden yapıldı.

---

## 1. Genel bulgular (özet)

| Ölçüt | Sonuç |
|---|---|
| Toplam bot kazanma oranı (UPG=1) | **629 / 1500 = %41,9** |
| Stabilite | **500/500 bölüm SAĞLAM.** Hiçbir bölümde atış yapılmadan yapı çökmedi ya da tilki ölmedi. |
| Tasarlanan eğri (D değerleri) | Kodda formüle **birebir uyuyor**: L40 = D19,6, L100 = D45,0, L200 = D52,3, L300 = D56,0, L500 = D59,0. |
| Bot hiç kazanamayan bölümler (0/3) | **184 bölüm (%37)**. Bunların 112'si 7 ördekli büyük kalelerde. |
| 7 ördekli bölümler (L41+) | 207 bölüm, kazanma %28,7, bunların 112'si 0/3 |
| 5 ördekli bölümler (L41+) | 81 bölüm, kazanma %65 |
| Yıldız dağılımı (L41+, kazanılanlar) | 1★: 162, 2★: 56, 3★: 61. 2★ eşiği çoğu büyük bölümde çok yüksek. |
| Kümülatif altın (bot ortalaması) | L20: ~1.170, L40: ~2.630, L100: ~8.800, L200: ~22.700, L500: ~71.600 |

**Kısa özet:** Eğri kâğıt üzerinde doğru uygulanmış, fakat oyuncunun hissettiği zorluk bu eğriyi izlemiyor.

1. **3. dünyada (L41–60) büyük bir sıçrama var.** Prosedürel üretimin başladığı yerde kazanma oranı %43'ten %22'ye düşüyor. 49–56 arasında 8 bölüm üst üste kazanılamadı.
2. **5–8. dünyalar bir "zorluk platosu"** oluşturuyor (%27–28). Ardından yükseltmeler zorluğu geçtiği için 9–12. dünyalarda oyun **kolaylaşıyor** (%40 → %55).
3. **L200'den sonra D neredeyse düz** (52,3 → 59,0). Yükseltmeler büyümeye devam ettiği için 11., 16. ve 21. dünyalar bir öncekinden belirgin biçimde kolay. Tasarımcının "zorluk sürekli artsın" hedefi ikinci yarıda karşılanmıyor.
4. **Prosedürel bölümler çift tepeli (bimodal).** Bir yanda 1–2 atışta 3★ alınan bölümler (3/3), öte yanda 7 ördekli ve botun hiç kazanamadığı dev kaleler (0/3) var. Arada "orta" bölüm az. Zorluğu tilki sayısı ve kale büyüklüğü belirliyor, D değeri değil.

---

## 2. Dünya bazında puanlar

Sütunlar:
- **Kazanma:** UPG=1 ile ortalama bot kazanma oranı.
- **Mağazasız:** UPG=0 ile kazanma oranı (yalnızca test edilen dünyalar).
- **0/3:** dünyada hiç kazanılamayan bölüm sayısı.
- **Ördek kullanımı:** kazanılan denemelerde en az kullanılan ördek / mevcut ördek ortalaması.
- **Altın/bölüm:** deneme başına ortalama altın (kayıplar dahil).

| Dünya | Bölümler | D aralığı | Kazanma | Mağazasız | 0/3 | Ördek kullanımı | Boss | En zor | En kolay | Altın/bölüm | **Zorluk /10** | **Denge /10** |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 1–20 | 0,6–11,1 | %61,7 | %60 | 2 | %57 | 2/3 | L13 (0/9), L18 (0/9) | L8 (3/3, 1 atış) | 58 | **3** | **6** |
| 2 | 21–40 | 11,5–19,6 | %43,3 | – | 5 | %70 | 0/3 (tekrar 1/6) | L40 Boss | L29 (3/3, 1 atış) | 73 | **5** | **5** |
| 3 | 41–60 | 20,0–28,1 | **%21,7** | – | **11** | %76 | 2/3 | L43 (3 ördek, 0/9), 47, 49–56 | L46 (1 atış) | 75 | **8** | **2** |
| 4 | 61–80 | 28,5–36,5 | %43,3 | – | 9 | %50 | 2/3 | L79, L63, L64 | L62/65/66/71/77 (1 atış) | 111 | **6** | **4** |
| 5 | 81–100 | 37,0–45,0 | %26,7 | %22 | **12** | %59 | 1/3 | L99 (3 ördek, 0/9), L95, 81–84 | L86 (1 atış) | 124 | **7** | **3** |
| 6 | 101–120 | 45,1–46,9 | %26,7 | – | 10 | **%85** | 1/3 | L119, 117–119 | L113 | 137 | **7** | **4** |
| 7 | 121–140 | 47,0–48,5 | %26,7 | – | **12** | %58 | 2/3 | L126, 129–134 (6 seri) | L138 | 122 | **8** | **3** |
| 8 | 141–160 | 48,6–49,9 | %28,3 | – | 11 | %79 | 0/3 | L150, L160 Boss | L152 | 132 | **7** | **4** |
| 9 | 161–180 | 50,0–51,2 | %40,0 | – | 8 | %63 | 0/3 | L180 Boss, 171–173 | L177 (1 atış) | 130 | **6** | **5** |
| 10 | 181–200 | 51,3–52,3 | %46,7 | %28 | 7 | %67 | 1/3 | L197–199 | L188 | 172 | **6** | **5** |
| 11 | 201–220 | 52,3–53,3 | %55,0 | – | 7 | %71 | 0/3 | L220 Boss | L219 (1 atış) | 171 | **5** | **4** |
| 12 | 221–240 | 53,3–54,1 | %50,0 | – | 4 | %68 | 2/3 | L226, L234 | L230 | 209 | **5** | **6** |
| 13 | 241–260 | 54,1–54,8 | %46,7 | – | 6 | %58 | 2/3 | L255 | L244 (1 atış) | 128 | **5** | **6** |
| 14 | 261–280 | 54,9–55,5 | %38,3 | – | 8 | %69 | 1/3 | L273–274, 276 | L271 | 170 | **6** | **6** |
| 15 | 281–300 | 55,5–56,0 | %48,3 | %28 | 8 | %66 | 0/3 | L300 Boss, 291–292 | L295 | 139 | **6** | **5** |
| 16 | 301–320 | 56,1–56,5 | %58,3 | – | 4 | %63 | 1/3 | L311, L319 | L317 | 165 | **4** | **4** |
| 17 | 321–340 | 56,6–57,0 | %36,7 | – | 8 | %71 | 1/3 | L336–338 | L327 | 162 | **6** | **5** |
| 18 | 341–360 | 57,0–57,3 | %41,7 | – | 9 | %65 | 0/3 | L357–360 + Boss | L345 (1 atış) | 155 | **6** | **4** |
| 19 | 361–380 | 57,4–57,7 | %35,0 | – | 9 | %67 | 1/3 | L361–364 (açılışta 4 seri) | L377 | 163 | **7** | **4** |
| 20 | 381–400 | 57,7–58,0 | %41,7 | %35 | 7 | %67 | 0/3 | L387, L400 Boss | L389 | 164 | **6** | **5** |
| 21 | 401–420 | 58,0–58,2 | **%61,7** | – | 3 | %64 | 2/3 | L409, L416 | L405 (1 atış) | 191 | **4** | **4** |
| 22 | 421–440 | 58,2–58,4 | %41,7 | – | 8 | %74 | 1/3 | L436, L438 | L432 (1 atış) | 165 | **6** | **5** |
| 23 | 441–460 | 58,5–58,6 | %45,0 | – | 6 | %67 | 0/3 | L453, L459–460 | L441/444/446 | 142 | **6** | **5** |
| 24 | 461–480 | 58,6–58,8 | %41,7 | – | 6 | %74 | 1/3 | L462, L469, L471 | L475 | 171 | **6** | **5** |
| 25 | 481–500 | 58,8–59,0 | %41,7 | **%20** | 8 | %65 | 0/3 | L492–495 (4 seri), L500 Boss | L485 | 152 | **7** | **4** |

### Dünya başına tek satırlık değerlendirme

1. **Dünya 1:** Dostane bir giriş. Ancak **L13 "Kale Duvarı" ve L18 "Piramit" 0/9** ile öğretici dünyanın ortasında iki duvar oluşturuyor.
2. **Dünya 2:** Orta zorlukta ve makul. 28–32 arasında üç duvar var. Boss L40 1/9 ile sert ama dünya sonu için kabul edilebilir.
3. **Dünya 3:** **Oyunun en kötü noktası.** Prosedürel geçişte kazanma yarıya iniyor ve 49–56 arasında 8 bölüm üst üste kazanılamıyor. Oyuncu kaybı riski en yüksek yer burası.
4. **Dünya 4:** Ortalama "orta" görünse de bölümler ya 1 atışlık ya da 0/3. Akış dalgalı. Yeni ördek türü burada yardım ediyor.
5. **Dünya 5:** Zor bir dünya ve dört bölümlük açılış duvarı (81–84) var. **L99'da yalnızca 3 ördek var ve 0/9 kazanma**, muhtemelen hatalı üretim.
6. **Dünya 6:** D neredeyse sabit, ama kazanılan bölümlerde ördeklerin %85'i kullanılıyor. Sürekli "son ördekte" kazanılan, yorucu bir dünya.
7. **Dünya 7:** Çok zor. 129–134'te 6 bölüm üst üste kazanılamıyor. Yeni ördek türü açılmasına rağmen dünya kazanma oranı artmıyor.
8. **Dünya 8:** Zorluk platosu devam ediyor (%28). 11 duvar var ve boss 0/3.
9. **Dünya 9:** Yükseltmeler etkisini göstermeye başlıyor ve oranlar toparlanıyor (%40). Akış daha dengeli.
10. **Dünya 10:** Orta zorlukta. Mağazasız oran %28, yükseltmeyle %47; mağazanın etkisi en net burada görülüyor. 197–199 serisi boss öncesinde sert.
11. **Dünya 11:** **Bir önceki dünyadan kolay** (%55). Zorluk artışı hissedilmiyor ve 219'da 3 atışta 3★ alınabiliyor.
12. **Dünya 12:** Dengeli ve az duvarlı. Ancak 7 ördekli 15 bölümle "çok atışlı, düşük yıldızlı" bir dünya.
13. **Dünya 13:** Dengeli. Ördek sayısı azalıyor (ort. 5,75) ama kazanma oranı korunuyor.
14. **Dünya 14:** Hafif bir sertleşme (%38) var ve doğru yöne gidiyor. 272–274 serisi dikkat istiyor.
15. **Dünya 15:** Orta zorlukta (mağazasız %28). Boss L300 0/3.
16. **Dünya 16:** **Belirgin kolaylaşma** (%58). Yeni ördek türüyle 4 duvar dışında rahat bir dünya.
17. **Dünya 17:** Zorluk geri geliyor (%37). 336–338 üçlü duvarı var.
18. **Dünya 18:** Sonu çok sert. 357–360 ve boss 0/3; seri bir sonraki dünyaya taşıyor.
19. **Dünya 19:** Dünyanın **açılışında 4 duvar** var (361–364). 357'den itibaren üst üste **8 bölüm** kazanılamıyor; bu, oyunun en uzun ikinci duvarı.
20. **Dünya 20:** Orta-zor bir dünya. Mağazasız %35 ile beklenenden kolay.
21. **Dünya 21:** **Geç oyun için fazla kolay** (%62, yalnızca 3 duvar). Zorluk 25 dünya içinde dünya 1 ile aynı seviyeye iniyor.
22. **Dünya 22:** Orta-zor ve dalgalı, 8 duvar var.
23. **Dünya 23:** Orta zorlukta. Kolay 4–5 ördekli bölümler ile 7 ördekli duvarlar arasında gidip geliyor.
24. **Dünya 24:** Orta zorlukta ve 23. dünyayla hemen hemen aynı. Final öncesi bir yükselme yok.
25. **Dünya 25:** Final dünyası mağazasız en zor dünya (%20). 492–495 serisi ve boss 0/3 var. Zor ama "son dünya" hissi daha çok duvarlardan geliyor.

---

## 3. Genel değerlendirme

### Zorluk istenen gibi düzgün artıyor mu?
**Hayır.**
- D değerleri formüle tam uyuyor, ama bot kazanma oranı şu sırayı izliyor: %62 → %43 → **%22** → %43 → %27 → %27 → %27 → %28 → %40 → %47 → %55 → %50 → … → %62 (W21) → %42.
- Bu iniş-çıkışlı bir eğri. **En zor bölge 3. ve 5–8. dünyalar**; oyunun geri kalanı daha kolay.
- Nedenleri:
  1. L41'de el yapımı bölümlerden prosedürel üretime geçişte zorluk bir anda artıyor.
  2. L100'den sonra D'nin büyümesi yavaşlıyor (D45 → D59, 400 bölümde yalnızca +14).
  3. UPG seviyeleri (`dmg` en fazla 4, `aim` en fazla 3) 6. dünyadan sonra hızla tavana ulaşıyor ve oyuncu gücü D'den hızlı artıyor.

### Duvarlar ve sıçramalar nerede?
- **Sert duvar serileri** (≥3 bölüm üst üste 0/3): 49–56 (8 bölüm), **357–364 (8 bölüm)**, 129–134 (6), 81–84 (4), 492–495 (4), ve üçlü seriler 74–76, 107–109, 117–119, 144–146, 171–173, 197–199, 272–274, 336–338, 373–375.
- **Muhtemel hatalı ya da adaletsiz bölümler:**
  - **L43** ve **L99**: yalnızca 3 ördek var, 2★ eşiği çok düşük (L99'da 7.500), ortalama hasar ~300–400 ve tekrar testinde de **0/6**. Tilkiye ulaşılamıyor ya da tilki korumalı olabilir. Elle incelenmeli.
  - **L13** ve **L18**: öğretici dünyada **0/9**.
- **Boss'lar:** 25 boss'tan 9'u 0/3 (L40, 160, 180, 220, 300, 360, 400, 460, 500). Boss'ların çoğu 7 ördekli ve 2★ eşiği ~100.000+; bot boss'u kazandığında bile çoğunlukla 1★ alıyor.
- **Ani düşüşler (kolaylaşma):** W3 → W4, W10 → W11, W15 → W16, W20 → W21.

### Erken oyun dostane mi?
**Büyük ölçüde evet.**
- 1. dünyada kazanma %62 ve 1–12. bölümler insan oyuncu için rahat.
- Ancak **L13 ve L18 öğretici dünyada beklenmedik duvarlar**.
- 2. dünyanın ikinci yarısı (28–40) hızla sertleşiyor.
- Ardından gelen **3. dünya şoku**, dostane girişin kazandırdığı motivasyonu bozabilir.

### Oyun 10 dakikadan çok daha uzun sürer mi?
**Kesinlikle evet.**
- 500 bölüm var; botla bile her bölüm 5–30 sn sürüyor.
- İnsan oyuncu için bölüm başına ~1–2 dakika ve tekrar denemelerle toplam **10–20+ saat** içerik var.
- İlk 20 bölüm tek başına 15–30 dakika sürer.
- Hedeflenen "5–10 dakikada bitmesin" koşulu fazlasıyla karşılanıyor. Asıl risk oyunun çok kısa olması değil, **3. ve 5–8. dünyalarda bırakılması**.

### Ek ördek kaç bölümde alınabilir (300–1000 altın)?

| Dünya | Ort. altın/bölüm | 300 altın | 1000 altın |
|---|---|---|---|
| 1 | ~58 | ~5 bölüm | ~17 bölüm |
| 2–3 | ~74 | ~4 | ~14 |
| 4–8 | 110–137 | ~2–3 | ~8–9 |
| 9–25 | 130–210 | ~2 | ~5–7 |

- Hesap, kayıplar dahil deneme başına ortalamaya dayanıyor; tekrar oynamalar da altın kazandırdığı için gerçek süre biraz daha kısa.
- **Yorum:** Erken oyunda ekonomi makul, 2–3 dünyada bir pahalı ördek alınabiliyor. 9. dünyadan sonra ise altın çok bol: her 5–7 bölümde 1000 altınlık ördek alınabiliyor. Bu, geç oyundaki kolaylaşmanın ikinci nedeni.

---

## 4. Somut denge önerileri

1. **L41–60 geçişini yumuşatın (en yüksek öncelik).**
   - 49–56 serisine en az 2–3 "nefes" bölümü ekleyin (5 ördek, az tilki).
   - Prosedürel üreticinin ilk dünyasında tilki ve blok sayısını %20–30 azaltın, ya da L41–60 için D'yi geçici olarak 0,8× ölçekleyin.
2. **L43 ve L99'u elle düzeltin.** 3 ördek ile 0/9 ve çok düşük hasar, tilkinin erişilemez olduğunu düşündürüyor. Ördek sayısını 4–5'e çıkarın ya da tilki konumunu kontrol edin. Üreticiye "minimum ördek = tilki sayısı + 1" kuralı ekleyin.
3. **L13 "Kale Duvarı" ve L18 "Piramit" duvarlarını kaldırın.** Öğretici dünyada 0/9 kabul edilemez. Duvar ya da piramidin bir katını azaltın, ya da bir ördek veya TNT ekleyin.
4. **7 ördekli dev kaleleri sınırlayın.** Bu bölümlerin %54'ü 0/3 ve toplam kazanma %29. Bir dünyada en fazla 4–5 tane olsun ve **hiçbir zaman art arda 2'den fazla** gelmesin. Bu kural 129–134, 357–364 ve 492–495 serilerini doğrudan çözer.
5. **Zorluk seçimine "seri koruyucu" ekleyin.** Üretici iki zor bölümden sonra bir kolay bölüm koysun: tilki sayısı inip çıksın, ama bu bir kural olsun.
6. **L100 sonrası eğriyi dikleştirin.** 75 + 25·(1−e^(−(n−100)/150)) formülü 200'den sonra neredeyse düz kalıyor. Yükseltme gücünü dengelemek için şunlardan biri yapılabilir:
   - Üs paydasını 150'den ~250'ye çıkarıp tavanı 100'ün üzerine taşımak (örneğin D tavanını 0,6 yerine 0,7×Percent yapmak).
   - Ya da yeni ördek türlerinin açıldığı dünyalarda (10, 13, 16, 19) D'ye +2 bonus eklemek.

   Bunun hedefi W11, W16 ve W21'deki kolaylaşmaları yok etmek.
7. **Yükseltme tavanını geç oyuna yayın.** `dmg` en fazla 4 seviyesine ~11. dünyada, `aim` en fazla 3 seviyesine 9. dünyada ulaşılıyor. Seviye sayısını artırıp fiyatları üstel yapın, ya da 15. dünyadan sonra yeni bir yükseltme türü ekleyin.
8. **Geç oyun altınını sınırlayın.** 9. dünyadan sonra ~150–210 altın/bölüm geliyor. "10 yumurta / 100 hasar" oranını 12. dünyadan sonra 8'e düşürün, ya da ek ördek fiyatını dünya numarasıyla ölçekleyin (örneğin 300 + 40×dünya).
9. **2★ ve 3★ eşiklerini gözden geçirin.** Prosedürel bölümlerde kazanılanların %58'i 1★. Özellikle 7 ördekli ve boss bölümlerinde 2★ eşiğini ~%15 düşürün; örneğin L40'ta 106.500 ve L180'de 125.500 çok yüksek.
10. **Boss'ları "zor ama adil" yapın.** 25 boss'tan 9'u 0/3: 2, 8, 9, 11, 15, 18, 20, 23 ve 25. dünyaların boss'ları (L40 tekrar testinde de yalnızca 1/6). Boss'lara 8. ördek ya da boss'a özel güçlü bir ördek verin, ve boss öncesindeki 2–3 bölümü (197–199, 357–359) kolaylaştırın.
11. **W5 açılışındaki 81–84 duvarını kırın.** Dünya geçişi motivasyon anı; ilk bölümü kolay (4–5 ördek, 1–2 tilki) yapın. Aynı kural 361–364 (W19 açılışı) için de geçerli.
12. **Final için hafif bir artış ekleyin.** 23–25. dünyalarda D farkı yok denecek kadar az (58,5 → 59,0). Final dünyası duvar serileriyle değil, kontrollü bir yükselişle (ör. 3 bölümde bir zor bölüm) zor olmalı.

---

## 5. Test notları ve sınırlamalar

- **Stabilite sorunu yok.** Tüm bölümler SAĞLAM.
- **Az deneme:** Bölüm başına 3 deneme yapıldı; tek bölüm sonuçları ±1 galibiyet oynayabilir (bkz. 51–52. bölüm tekrarları). Dünya ortalamaları güvenilir.
- **Mağazasız (UPG=0) testi** yalnızca 1, 5, 10, 15, 20 ve 25. dünyalarda, 2'şer denemeyle yapıldı. 1. dünyada UPG etkisi yok (seviye 0). Sonraki dünyalarda yükseltme etkisi +5 ile +22 puan arasında.
- **Test altyapısı hakkında gözlemler:**
  - Unity penceresi öne alınmadığında test duruyor. Ara sıra başka pencereler (Unity Hub, Steam) odağı çaldı ve pencerenin tekrar öne alınması gerekti.
  - `Bitti` satırından sonra yeni bir `autotest` komutu gönderildiğinde editör iki kez play modundan çıkıp yeniden girmedi; komutun yeniden gönderilmesi gerekti.
  - İlk koşu, otomatik test başlarken başlayan bir script derlemesi (domain reload) yüzünden kesildi ve yeniden başlatıldı.
