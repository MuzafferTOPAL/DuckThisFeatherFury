using System.Collections.Generic;
using UnityEngine;

// Çoklu dil desteği: Türkçe, İngilizce, Almanca, İspanyolca, Fransızca, Portekizce, İtalyanca.
// İlk açılışta cihaz dili seçilir, ana menüdeki dil butonuyla değiştirilebilir.
public static class Loc
{
    public static readonly string[] Codes = { "TR", "EN", "DE", "ES", "FR", "PT", "IT" };
    public static readonly string[] Names = { "TÜRKÇE", "ENGLISH", "DEUTSCH", "ESPAÑOL", "FRANÇAIS", "PORTUGUÊS", "ITALIANO" };

    static int lang = -1;

    public static int Lang
    {
        get
        {
            if (lang < 0) lang = PlayerPrefs.HasKey("lang") ? PlayerPrefs.GetInt("lang") : Detect();
            return Mathf.Clamp(lang, 0, Codes.Length - 1);
        }
        set
        {
            lang = Mathf.Clamp(value, 0, Codes.Length - 1);
            PlayerPrefs.SetInt("lang", lang);
            PlayerPrefs.Save();
        }
    }

    public static void Next() => Lang = (Lang + 1) % Codes.Length;

    static int Detect()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Turkish: return 0;
            case SystemLanguage.German: return 2;
            case SystemLanguage.Spanish: return 3;
            case SystemLanguage.French: return 4;
            case SystemLanguage.Portuguese: return 5;
            case SystemLanguage.Italian: return 6;
            default: return 1;
        }
    }

    public static string T(string key)
    {
        if (Table.TryGetValue(key, out var arr)) return arr[Lang];
        return key;
    }

    public static string World(int i) => T("w" + i);
    public static string LevelTitle(int i)
    {
        var d = Levels.Get(i);
        if (!string.IsNullOrEmpty(d.titleKey)) return T(d.titleKey);
        return World(d.world) + " " + (d.slot + 1);
    }
    public static string BirdName(BirdType t) => T("bird" + (int)t);
    public static string BirdAbility(BirdType t) => T("abil" + (int)t);

    //                                     TR                         EN                      DE                          ES                          FR                            PT                           IT
    static readonly Dictionary<string, string[]> Table = new Dictionary<string, string[]>
    {
        { "title",      new[] { "DUCKTHIS", "DUCKTHIS", "DUCKTHIS", "DUCKTHIS", "DUCKTHIS", "DUCKTHIS", "DUCKTHIS" } },
        { "title2",     new[] { "FEATHER FURY", "FEATHER FURY", "FEATHER FURY", "FEATHER FURY", "FEATHER FURY", "FEATHER FURY", "FEATHER FURY" } },
        { "dev_by",     new[] { "DUCKTHISGAMES TARAFINDAN GELİŞTİRİLDİ", "DEVELOPED BY DUCKTHISGAMES", "ENTWICKELT VON DUCKTHISGAMES", "DESARROLLADO POR DUCKTHISGAMES", "DÉVELOPPÉ PAR DUCKTHISGAMES", "DESENVOLVIDO POR DUCKTHISGAMES", "SVILUPPATO DA DUCKTHISGAMES" } },
        { "shop",       new[] { "DÜKKAN", "SHOP", "LADEN", "TIENDA", "BOUTIQUE", "LOJA", "NEGOZIO" } },
        { "eggs",       new[] { "ALTIN YUMURTA", "GOLDEN EGGS", "GOLDENE EIER", "HUEVOS DE ORO", "ŒUFS D'OR", "OVOS DE OURO", "UOVA D'ORO" } },
        { "damage",     new[] { "HASAR", "DAMAGE", "SCHADEN", "DAÑO", "DÉGÂTS", "DANO", "DANNO" } },
        { "damage_d",   new[] { "HER SEVİYE +%15 HASAR", "+15% DAMAGE PER LEVEL", "+15% SCHADEN PRO STUFE", "+15% DE DAÑO POR NIVEL", "+15 % DE DÉGÂTS PAR NIVEAU", "+15% DE DANO POR NÍVEL", "+15% DI DANNO PER LIVELLO" } },
        { "aim",        new[] { "OTOMATİK NİŞAN", "AUTO-AIM", "AUTO-ZIEL", "AUTOPUNTERÍA", "VISÉE AUTO", "MIRA AUTOMÁTICA", "MIRA AUTOMATICA" } },
        { "aim_d",      new[] { "UÇARKEN TİLKİYE DOĞRU YÖNELİR", "STEERS TOWARD FOXES IN FLIGHT", "LENKT IM FLUG ZU DEN FÜCHSEN", "VUELA HACIA LOS ZORROS", "SE DIRIGE VERS LES RENARDS", "VOA EM DIREÇÃO ÀS RAPOSAS", "IN VOLO PUNTA LE VOLPI" } },
        { "extra",      new[] { "EKSTRA ÖRDEK", "EXTRA DUCK", "EXTRA-ENTE", "PATO EXTRA", "CANARD BONUS", "PATO EXTRA", "ANATRA EXTRA" } },
        { "extra_d",    new[] { "OYUNDA + BUTONUYLA KULLAN", "USE IN-GAME WITH THE + BUTTON", "IM SPIEL MIT + EINSETZEN", "ÚSALO EN EL JUEGO CON +", "UTILISE-LE EN JEU AVEC +", "USE NO JOGO COM O BOTÃO +", "USALA IN GIOCO CON IL TASTO +" } },
        { "owned",      new[] { "ELDE", "OWNED", "BESITZ", "TIENES", "EN STOCK", "TEM", "POSSEDUTE" } },
        { "max",        new[] { "MAKS", "MAX", "MAX", "MÁX", "MAX", "MÁX", "MAX" } },
        { "destruction",new[] { "YIKIM", "DESTRUCTION", "ZERSTÖRUNG", "DESTRUCCIÓN", "DESTRUCTION", "DESTRUIÇÃO", "DISTRUZIONE" } },
        { "add_duck",   new[] { "ÖRDEK EKLE", "ADD A DUCK", "ENTE HINZUFÜGEN", "AÑADIR UN PATO", "AJOUTER UN CANARD", "ADICIONAR PATO", "AGGIUNGI ANATRA" } },
        { "use",        new[] { "KULLAN", "USE", "NUTZEN", "USAR", "UTILISER", "USAR", "USA" } },
        { "continue",   new[] { "ÖRDEKLE DEVAM", "CONTINUE", "WEITER", "CONTINUAR", "CONTINUER", "CONTINUAR", "CONTINUA" } },
        { "unlock_at", new[] { "LEVEL {0} İLE AÇILIR", "UNLOCKS AT LEVEL {0}", "AB LEVEL {0}", "SE ABRE EN EL NIVEL {0}", "DÉBLOQUÉ AU NIVEAU {0}", "LIBERA NO NÍVEL {0}", "SBLOCCATA AL LIVELLO {0}" } },
        { "no_eggs",    new[] { "YETERSİZ YUMURTA!", "NOT ENOUGH EGGS!", "NICHT GENUG EIER!", "¡FALTAN HUEVOS!", "PAS ASSEZ D'ŒUFS !", "OVOS INSUFICIENTES!", "UOVA INSUFFICIENTI!" } },
        { "first_clear",new[] { "İLK GEÇİŞ BONUSU", "FIRST CLEAR BONUS", "ERSTSIEG-BONUS", "BONO PRIMERA VEZ", "BONUS 1RE VICTOIRE", "BÔNUS 1ª VITÓRIA", "BONUS PRIMA VITTORIA" } },
        { "subtitle",   new[] { "TİLKİLER YUMURTALARI ÇALDI!", "THE FOXES STOLE THE EGGS!", "DIE FÜCHSE HABEN DIE EIER GESTOHLEN!", "¡LOS ZORROS ROBARON LOS HUEVOS!", "LES RENARDS ONT VOLÉ LES ŒUFS !", "AS RAPOSAS ROUBARAM OS OVOS!", "LE VOLPI HANNO RUBATO LE UOVA!" } },
        { "play",       new[] { "OYNA", "PLAY", "SPIELEN", "JUGAR", "JOUER", "JOGAR", "GIOCA" } },
        { "quit",       new[] { "ÇIKIŞ", "QUIT", "BEENDEN", "SALIR", "QUITTER", "SAIR", "ESCI" } },
        { "sound_on",   new[] { "SES\nAÇIK", "SOUND\nON", "TON\nAN", "SONIDO\nSÍ", "SON\nOUI", "SOM\nSIM", "AUDIO\nSÌ" } },
        { "sound_off",  new[] { "SES\nKAPALI", "SOUND\nOFF", "TON\nAUS", "SONIDO\nNO", "SON\nNON", "SOM\nNÃO", "AUDIO\nNO" } },
        { "world", new[] { "BÖLÜM", "WORLD", "WELT", "MUNDO", "MONDE", "MUNDO", "MONDO" } },
        { "level", new[] { "LEVEL", "LEVEL", "LEVEL", "NIVEL", "NIVEAU", "NÍVEL", "LIVELLO" } },
        { "score",      new[] { "PUAN", "SCORE", "PUNKTE", "PUNTOS", "SCORE", "PONTOS", "PUNTI" } },
        { "best",       new[] { "EN İYİ", "BEST", "BESTER", "MEJOR", "RECORD", "RECORDE", "RECORD" } },
        { "foxes",      new[] { "TİLKİ", "FOXES", "FÜCHSE", "ZORROS", "RENARDS", "RAPOSAS", "VOLPI" } },
        { "new_duck",   new[] { "YENİ ÖRDEK!", "NEW DUCK!", "NEUE ENTE!", "¡NUEVO PATO!", "NOUVEAU CANARD !", "NOVO PATO!", "NUOVA ANATRA!" } },
        { "tap_start",  new[] { "BAŞLAMAK İÇİN DOKUN", "TAP TO START", "ZUM STARTEN TIPPEN", "TOCA PARA EMPEZAR", "TOUCHE POUR COMMENCER", "TOQUE PARA COMEÇAR", "TOCCA PER INIZIARE" } },
        { "paused",     new[] { "DURAKLATILDI", "PAUSED", "PAUSE", "PAUSA", "PAUSE", "PAUSA", "PAUSA" } },
        { "resume",     new[] { "DEVAM", "RESUME", "WEITER", "SEGUIR", "REPRENDRE", "CONTINUAR", "RIPRENDI" } },
        { "retry",      new[] { "TEKRAR", "RETRY", "NOCHMAL", "REPETIR", "REJOUER", "REPETIR", "RIPROVA" } },
        { "levels", new[] { "LEVELLER", "LEVELS", "LEVELS", "NIVELES", "NIVEAUX", "NÍVEIS", "LIVELLI" } },
        { "main_menu",  new[] { "ANA MENÜ", "MAIN MENU", "HAUPTMENÜ", "MENÚ", "MENU", "MENU", "MENU" } },
        { "quit_q",     new[] { "OYUNDAN ÇIKMAK İSTİYOR MUSUN?", "DO YOU WANT TO QUIT?", "SPIEL WIRKLICH BEENDEN?", "¿QUIERES SALIR?", "VEUX-TU QUITTER ?", "QUER SAIR DO JOGO?", "VUOI DAVVERO USCIRE?" } },
        { "yes",        new[] { "EVET", "YES", "JA", "SÍ", "OUI", "SIM", "SÌ" } },
        { "no",         new[] { "HAYIR", "NO", "NEIN", "NO", "NON", "NÃO", "NO" } },
        { "level_done", new[] { "BÖLÜM TAMAM!", "LEVEL CLEAR!", "GESCHAFFT!", "¡NIVEL SUPERADO!", "NIVEAU RÉUSSI !", "NÍVEL CONCLUÍDO!", "LIVELLO SUPERATO!" } },
        { "foxes_won",  new[] { "TİLKİLER KAZANDI!", "THE FOXES WON!", "DIE FÜCHSE GEWINNEN!", "¡GANARON LOS ZORROS!", "LES RENARDS GAGNENT !", "AS RAPOSAS VENCERAM!", "HANNO VINTO LE VOLPI!" } },
        { "new_record", new[] { "YENİ REKOR!", "NEW RECORD!", "NEUER REKORD!", "¡NUEVO RÉCORD!", "NOUVEAU RECORD !", "NOVO RECORDE!", "NUOVO RECORD!" } },

        { "w0", new[] { "GÖL KIYISI", "LAKESIDE", "SEEUFER", "ORILLA DEL LAGO", "BORD DU LAC", "BEIRA DO LAGO", "RIVA DEL LAGO" } },
        { "w1", new[] { "SONBAHAR ORMANI", "AUTUMN FOREST", "HERBSTWALD", "BOSQUE OTOÑAL", "FORÊT D'AUTOMNE", "FLORESTA DE OUTONO", "BOSCO D'AUTUNNO" } },
        { "w2", new[] { "KARLI DAĞLAR", "SNOWY PEAKS", "SCHNEEBERGE", "MONTAÑAS NEVADAS", "MONTAGNES ENNEIGÉES", "MONTANHAS NEVADAS", "MONTAGNE INNEVATE" } },
        { "w3", new[] { "GÜN BATIMI ÇİFTLİĞİ", "SUNSET FARM", "ABENDROT-HOF", "GRANJA DEL OCASO", "FERME DU COUCHANT", "FAZENDA DO PÔR DO SOL", "FATTORIA AL TRAMONTO" } },

        { "bird0", new[] { "YEŞİLBAŞ", "MALLARD", "STOCKENTE", "ÁNADE REAL", "COLVERT", "PATO-REAL", "GERMANO REALE" } },
        { "bird1", new[] { "MANDARİN ÖRDEĞİ", "MANDARIN DUCK", "MANDARINENTE", "PATO MANDARÍN", "CANARD MANDARIN", "PATO-MANDARIM", "ANATRA MANDARINA" } },
        { "bird2", new[] { "TARAKDİŞ", "MERGANSER", "GÄNSESÄGER", "SERRETA", "HARLE", "MERGANSO", "SMERGO" } },
        { "bird3", new[] { "MİSK ÖRDEĞİ", "MUSCOVY DUCK", "MOSCHUSENTE", "PATO CRIOLLO", "CANARD DE BARBARIE", "PATO-DO-MATO", "ANATRA MUSCHIATA" } },
        { "bird4", new[] { "KARA ÖRDEK", "BLACK SCOTER", "TRAUERENTE", "NEGRÓN", "MACREUSE NOIRE", "NEGRELHO", "ORCO MARINO" } },
        { "bird5", new[] { "ANGUT", "RUDDY SHELDUCK", "ROSTGANS", "TARRO CANELO", "TADORNE CASARCA", "PATO-CASARCA", "CASARCA" } },

        { "abil0", new[] { "HAVADA DOKUN: KANAT ÇIRP (2 KEZ YÜKSELİR)", "TAP IN FLIGHT: FLAP UP (TWICE)", "IM FLUG TIPPEN: FLATTERN (2-MAL)", "TOCA EN VUELO: ALETEA (2 VECES)", "TOUCHE EN VOL : BATTEMENT D'AILES (2 FOIS)", "TOQUE NO AR: BATER ASAS (2 VEZES)", "TOCCA IN VOLO: SBATTI LE ALI (2 VOLTE)" } },
        { "abil1", new[] { "HAVADA DOKUN: ÜÇE BÖLÜN! BUZU KOLAY KIRAR", "TAP IN FLIGHT: SPLIT INTO THREE! GREAT ON ICE", "IM FLUG TIPPEN: DREIFACH-TEILUNG! STARK GEGEN EIS", "TOCA EN VUELO: ¡SE DIVIDE EN TRES! ROMPE EL HIELO", "TOUCHE EN VOL : SE DIVISE EN TROIS ! BRISE LA GLACE", "TOQUE NO AR: DIVIDE-SE EM TRÊS! QUEBRA GELO", "TOCCA IN VOLO: SI DIVIDE IN TRE! ROMPE IL GHIACCIO" } },
        { "abil2", new[] { "HAVADA DOKUN: OK GİBİ DALIŞ! TAHTAYI DELER", "TAP IN FLIGHT: ARROW DIVE! PIERCES WOOD", "IM FLUG TIPPEN: PFEILSTURZ! DURCHBRICHT HOLZ", "TOCA EN VUELO: ¡PICADO VELOZ! ATRAVIESA MADERA", "TOUCHE EN VOL : PIQUÉ ÉCLAIR ! PERCE LE BOIS", "TOQUE NO AR: MERGULHO VELOZ! FURA MADEIRA", "TOCCA IN VOLO: PICCHIATA! TRAPASSA IL LEGNO" } },
        { "abil3", new[] { "HAVADA DOKUN: YERE ÇAKIL! AĞIRDIR, TAŞI EZER", "TAP IN FLIGHT: GROUND POUND! CRUSHES STONE", "IM FLUG TIPPEN: STAMPFER! ZERMALMT STEIN", "TOCA EN VUELO: ¡EN PICADA! APLASTA PIEDRA", "TOUCHE EN VOL : PLONGEON ! ÉCRASE LA PIERRE", "TOQUE NO AR: QUEDA PESADA! ESMAGA PEDRA", "TOCCA IN VOLO: SCHIACCIATA! FRANTUMA LA PIETRA" } },
        { "abil4", new[] { "HAVADA DOKUN: KABUM! PATLAYARAK HER ŞEYİ SAVURUR", "TAP IN FLIGHT: KABOOM! BLASTS EVERYTHING AWAY", "IM FLUG TIPPEN: KABUMM! SPRENGT ALLES WEG", "TOCA EN VUELO: ¡KABUM! LO HACE VOLAR TODO", "TOUCHE EN VOL : BOUM ! TOUT EXPLOSE", "TOQUE NO AR: KABUM! EXPLODE TUDO", "TOCCA IN VOLO: KABOOM! FA ESPLODERE TUTTO" } },
        { "abil5", new[] { "HAVADA DOKUN: YUMURTA BOMBASI BIRAKIR, YUKARI FIRLAR", "TAP IN FLIGHT: DROPS AN EGG BOMB AND SHOOTS UP", "IM FLUG TIPPEN: EIERBOMBE ABWERFEN UND HOCHSCHIESSEN", "TOCA EN VUELO: SUELTA UN HUEVO BOMBA Y SUBE", "TOUCHE EN VOL : LÂCHE UN ŒUF-BOMBE ET REMONTE", "TOQUE NO AR: SOLTA UM OVO-BOMBA E SOBE", "TOCCA IN VOLO: SGANCIA UN UOVO-BOMBA E RISALE" } },

        { "lvl0",  new[] { "İLK ATIŞ", "FIRST SHOT", "ERSTER SCHUSS", "PRIMER TIRO", "PREMIER TIR", "PRIMEIRO TIRO", "PRIMO TIRO" } },
        { "lvl1",  new[] { "İKİ KULE", "TWO TOWERS", "ZWEI TÜRME", "DOS TORRES", "DEUX TOURS", "DUAS TORRES", "DUE TORRI" } },
        { "lvl2",  new[] { "TAŞ TEMEL", "STONE BASE", "STEINFUNDAMENT", "BASE DE PIEDRA", "BASE DE PIERRE", "BASE DE PEDRA", "BASE DI PIETRA" } },
        { "lvl3",  new[] { "BUZ SARAYI", "ICE PALACE", "EISPALAST", "PALACIO DE HIELO", "PALAIS DE GLACE", "PALÁCIO DE GELO", "PALAZZO DI GHIACCIO" } },
        { "lvl4",  new[] { "GÖL KALESİ", "LAKE FORT", "SEEFESTUNG", "FUERTE DEL LAGO", "FORT DU LAC", "FORTE DO LAGO", "FORTE DEL LAGO" } },
        { "lvl5",  new[] { "TARAKDİŞ DALIŞI", "MERGANSER DIVE", "SÄGER-STURZ", "PICADO DE SERRETA", "PIQUÉ DU HARLE", "MERGULHO DO MERGANSO", "TUFFO DELLO SMERGO" } },
        { "lvl6",  new[] { "ODUN KULESİ", "TIMBER TOWER", "HOLZTURM", "TORRE DE MADERA", "TOUR EN BOIS", "TORRE DE MADEIRA", "TORRE DI LEGNO" } },
        { "lvl7",  new[] { "PATLAYAN SANDIK", "EXPLODING CRATE", "EXPLOSIVE KISTE", "CAJA EXPLOSIVA", "CAISSE EXPLOSIVE", "CAIXA EXPLOSIVA", "CASSA ESPLOSIVA" } },
        { "lvl8",  new[] { "AĞAÇ EVLERİ", "TREEHOUSES", "BAUMHÄUSER", "CASAS DEL ÁRBOL", "CABANES PERCHÉES", "CASAS NA ÁRVORE", "CASE SUGLI ALBERI" } },
        { "lvl9",  new[] { "TİLKİ İNİ", "FOX DEN", "FUCHSBAU", "MADRIGUERA", "TANIÈRE DU RENARD", "TOCA DA RAPOSA", "TANA DELLA VOLPE" } },
        { "lvl10", new[] { "AĞIR SİKLET", "HEAVYWEIGHT", "SCHWERGEWICHT", "PESO PESADO", "POIDS LOURD", "PESO PESADO", "PESO MASSIMO" } },
        { "lvl11", new[] { "BUZ KULELERİ", "ICE TOWERS", "EISTÜRME", "TORRES DE HIELO", "TOURS DE GLACE", "TORRES DE GELO", "TORRI DI GHIACCIO" } },
        { "lvl12", new[] { "KALE DUVARI", "CASTLE WALL", "BURGMAUER", "LA MURALLA", "LE REMPART", "A MURALHA", "LE MURA" } },
        { "lvl13", new[] { "KAYAN KAYALAR", "ROLLING ROCKS", "ROLLENDE FELSEN", "ROCAS RODANTES", "ROCHERS ROULANTS", "ROCHAS ROLANTES", "MASSI ROTOLANTI" } },
        { "lvl14", new[] { "BUZ KALESİ", "ICE CASTLE", "EISBURG", "CASTILLO DE HIELO", "CHÂTEAU DE GLACE", "CASTELO DE GELO", "CASTELLO DI GHIACCIO" } },
        { "lvl15", new[] { "KARA BOMBA", "BLACK BOMB", "SCHWARZE BOMBE", "BOMBA NEGRA", "BOMBE NOIRE", "BOMBA NEGRA", "BOMBA NERA" } },
        { "lvl16", new[] { "AHIR", "THE BARN", "DIE SCHEUNE", "EL GRANERO", "LA GRANGE", "O CELEIRO", "IL FIENILE" } },
        { "lvl17", new[] { "PİRAMİT", "PYRAMID", "PYRAMIDE", "PIRÁMIDE", "PYRAMIDE", "PIRÂMIDE", "PIRAMIDE" } },
        { "lvl18", new[] { "ÜÇ KALE", "THREE FORTS", "DREI FESTUNGEN", "TRES FUERTES", "TROIS FORTS", "TRÊS FORTES", "TRE FORTI" } },
        { "lvl19", new[] { "TİLKİ KRAL", "FOX KING", "FUCHSKÖNIG", "REY ZORRO", "ROI RENARD", "REI RAPOSA", "RE VOLPE" } },
        { "gift_hint", new[] { "HER BÖLÜMDE 2 HEDİYE ÖRDEK", "2 GIFT DUCKS PER WORLD", "2 GESCHENK-ENTEN PRO WELT", "2 PATOS DE REGALO POR MUNDO", "2 CANARDS CADEAUX PAR MONDE", "2 PATOS DE PRESENTE POR MUNDO", "2 ANATRE REGALO PER MONDO" } },
        { "gift_take", new[] { "AL!", "CLAIM!", "HOLEN!", "¡RECLAMAR!", "PRENDRE !", "PEGAR!", "PRENDI!" } },
        { "gift_title", new[] { "HEDİYE ÖRDEK!", "GIFT DUCK!", "GESCHENK-ENTE!", "¡PATO DE REGALO!", "CANARD CADEAU !", "PATO DE PRESENTE!", "ANATRA IN REGALO!" } },
        { "gift_ready", new[] { "HEDİYE ÖRDEK HAZIR!", "A GIFT DUCK IS READY!", "GESCHENK-ENTE BEREIT!", "¡PATO DE REGALO LISTO!", "CANARD CADEAU DISPONIBLE !", "PATO DE PRESENTE PRONTO!", "ANATRA REGALO PRONTA!" } },
        { "ok", new[] { "TAMAM", "OK", "OK", "VALE", "OK", "OK", "OK" } },
        { "w4", new[] { "ÇÖL KANYONU", "DESERT CANYON", "WÜSTENSCHLUCHT", "CAÑÓN DEL DESIERTO", "CANYON DU DÉSERT", "CÂNION DO DESERTO", "CANYON DEL DESERTO" } },
        { "w5", new[] { "TROPİK SAHİL", "TROPICAL BEACH", "TROPENSTRAND", "PLAYA TROPICAL", "PLAGE TROPICALE", "PRAIA TROPICAL", "SPIAGGIA TROPICALE" } },
        { "w6", new[] { "KİRAZ VADİSİ", "BLOSSOM VALLEY", "KIRSCHBLÜTENTAL", "VALLE DE LOS CEREZOS", "VALLÉE DES CERISIERS", "VALE DAS CEREJEIRAS", "VALLE DEI CILIEGI" } },
        { "w7", new[] { "ATEŞ DAĞI", "FIRE MOUNTAIN", "FEUERBERG", "MONTAÑA DE FUEGO", "MONTAGNE DE FEU", "MONTANHA DE FOGO", "MONTAGNA DI FUOCO" } },
        { "locked_world", new[] { "ÖNCE {0}. BÖLÜMÜ BİTİR", "FINISH WORLD {0} FIRST", "ERST WELT {0} ABSCHLIESSEN", "TERMINA EL MUNDO {0} PRIMERO", "TERMINE D'ABORD LE MONDE {0}", "TERMINE O MUNDO {0} PRIMEIRO", "FINISCI PRIMA IL MONDO {0}" } },
        { "restart_level", new[] { "LEVELİ YENİDEN BAŞLAT", "RESTART LEVEL", "LEVEL NEU STARTEN", "REINICIAR NIVEL", "RECOMMENCER LE NIVEAU", "REINICIAR NÍVEL", "RICOMINCIA LIVELLO" } },
        { "w9", new[] { "TİLKİ KRALLIĞI", "FOX KINGDOM", "FUCHSKÖNIGREICH", "REINO ZORRO", "ROYAUME DES RENARDS", "REINO DAS RAPOSAS", "REGNO DELLE VOLPI" } },
        { "w8", new[] { "SİSLİ BATAKLIK", "MISTY SWAMP", "NEBELSUMPF", "PANTANO BRUMOSO", "MARAIS BRUMEUX", "PÂNTANO NEBULOSO", "PALUDE NEBBIOSA" } },
        { "lvl20", new[] { "KUM FIRTINASI", "SANDSTORM", "SANDSTURM", "TORMENTA DE ARENA", "TEMPÊTE DE SABLE", "TEMPESTADE DE AREIA", "TEMPESTA DI SABBIA" } },
        { "lvl21", new[] { "KAKTÜS KULESİ", "CACTUS TOWER", "KAKTUSTURM", "TORRE DEL CACTUS", "TOUR DU CACTUS", "TORRE DO CACTO", "TORRE DEL CACTUS" } },
        { "lvl22", new[] { "VAHA", "OASIS", "OASE", "OASIS", "OASIS", "OÁSIS", "OASI" } },
        { "lvl23", new[] { "KANYON KÖPRÜSÜ", "CANYON BRIDGE", "SCHLUCHTBRÜCKE", "PUENTE DEL CAÑÓN", "PONT DU CANYON", "PONTE DO CÂNION", "PONTE DEL CANYON" } },
        { "lvl24", new[] { "FİRAVUN TİLKİ", "PHARAOH FOX", "PHARAO-FUCHS", "ZORRO FARAÓN", "RENARD PHARAON", "RAPOSA FARAÓ", "VOLPE FARAONE" } },
        { "lvl25", new[] { "KUMDAN KALE", "SANDCASTLE", "SANDBURG", "CASTILLO DE ARENA", "CHÂTEAU DE SABLE", "CASTELO DE AREIA", "CASTELLO DI SABBIA" } },
        { "lvl26", new[] { "HİNDİSTAN CEVİZİ", "COCONUTS", "KOKOSNÜSSE", "COCOS", "NOIX DE COCO", "COCOS", "NOCI DI COCCO" } },
        { "lvl27", new[] { "DENİZ FENERİ", "LIGHTHOUSE", "LEUCHTTURM", "FARO", "PHARE", "FAROL", "FARO" } },
        { "lvl28", new[] { "KORSAN GEMİSİ", "PIRATE SHIP", "PIRATENSCHIFF", "BARCO PIRATA", "BATEAU PIRATE", "NAVIO PIRATA", "NAVE PIRATA" } },
        { "lvl29", new[] { "TİLKİ ADASI", "FOX ISLAND", "FUCHSINSEL", "ISLA ZORRO", "ÎLE AUX RENARDS", "ILHA DAS RAPOSAS", "ISOLA DELLE VOLPI" } },
        { "lvl30", new[] { "PAGODA", "PAGODA", "PAGODE", "PAGODA", "PAGODE", "PAGODE", "PAGODA" } },
        { "lvl31", new[] { "BAMBU ORMANI", "BAMBOO GROVE", "BAMBUSWALD", "BOSQUE DE BAMBÚ", "BAMBOUSERAIE", "BAMBUZAL", "BOSCO DI BAMBÙ" } },
        { "lvl32", new[] { "TAŞ BAHÇE", "STONE GARDEN", "STEINGARTEN", "JARDÍN DE PIEDRA", "JARDIN DE PIERRE", "JARDIM DE PEDRA", "GIARDINO DI PIETRA" } },
        { "lvl33", new[] { "ÇAY EVİ", "TEA HOUSE", "TEEHAUS", "CASA DE TÉ", "MAISON DE THÉ", "CASA DE CHÁ", "CASA DEL TÈ" } },
        { "lvl34", new[] { "ŞOGUN TİLKİ", "SHOGUN FOX", "SHOGUN-FUCHS", "ZORRO SHOGUN", "RENARD SHOGUN", "RAPOSA XOGUM", "VOLPE SHOGUN" } },
        { "lvl35", new[] { "KÜL YOLU", "ASH ROAD", "ASCHEPFAD", "CAMINO DE CENIZA", "ROUTE DES CENDRES", "ESTRADA DE CINZAS", "STRADA DI CENERE" } },
        { "lvl36", new[] { "OBSİDYEN KULE", "OBSIDIAN TOWER", "OBSIDIANTURM", "TORRE DE OBSIDIANA", "TOUR D'OBSIDIENNE", "TORRE DE OBSIDIANA", "TORRE DI OSSIDIANA" } },
        { "lvl37", new[] { "LAV KÖPRÜSÜ", "LAVA BRIDGE", "LAVABRÜCKE", "PUENTE DE LAVA", "PONT DE LAVE", "PONTE DE LAVA", "PONTE DI LAVA" } },
        { "lvl38", new[] { "EJDERHA MAĞARASI", "DRAGON'S CAVE", "DRACHENHÖHLE", "CUEVA DEL DRAGÓN", "CAVERNE DU DRAGON", "CAVERNA DO DRAGÃO", "GROTTA DEL DRAGO" } },
        { "lvl39", new[] { "TİLKİ İMPARATOR", "FOX EMPEROR", "FUCHSKAISER", "EMPERADOR ZORRO", "EMPEREUR RENARD", "IMPERADOR RAPOSA", "IMPERATORE VOLPE" } },
        { "tip_nest", new[] { "YENİ: ALTIN YUVA! KIR VE +3 ALTIN YUMURTA KAP (HER YUVA BİR KEZ)", "NEW: GOLDEN NEST! BREAK IT FOR +3 GOLDEN EGGS (ONCE PER NEST)", "NEU: GOLDENES NEST! ZERSTÖREN FÜR +3 GOLDENE EIER (EINMALIG)", "¡NUEVO: NIDO DORADO! RÓMPELO Y GANA +3 HUEVOS DE ORO (UNA VEZ)", "NOUVEAU : NID DORÉ ! CASSE-LE POUR +3 ŒUFS D'OR (UNE FOIS)", "NOVO: NINHO DOURADO! QUEBRE PARA +3 OVOS DE OURO (UMA VEZ)", "NOVITÀ: NIDO D'ORO! ROMPILO PER +3 UOVA D'ORO (UNA VOLTA)" } },
        { "tier_up", new[] { "{0}. BÖLÜM: TİLKİLER %{1} DAHA DAYANIKLI!", "WORLD {0}: FOXES ARE {1}% TOUGHER!", "WELT {0}: FÜCHSE SIND {1}% ZÄHER!", "MUNDO {0}: ¡ZORROS UN {1}% MÁS DUROS!", "MONDE {0} : RENARDS {1} % PLUS COSTAUDS !", "MUNDO {0}: RAPOSAS {1}% MAIS FORTES!", "MONDO {0}: VOLPI PIÙ FORTI DEL {1}%!" } },
        { "egg_short", new[] { "YUMURTA", "EGGS", "EIER", "HUEVOS", "ŒUFS", "OVOS", "UOVA" } },
        { "music_on", new[] { "MÜZİK\nAÇIK", "MUSIC\nON", "MUSIK\nAN", "MÚSICA\nSÍ", "MUSIQUE\nOUI", "MÚSICA\nSIM", "MUSICA\nSÌ" } },
        { "music_off", new[] { "MÜZİK\nKAPALI", "MUSIC\nOFF", "MUSIK\nAUS", "MÚSICA\nNO", "MUSIQUE\nNON", "MÚSICA\nNÃO", "MUSICA\nNO" } },
    };
}
