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
    public static string LevelTitle(int i) => T("lvl" + i);
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
        { "unlock_at",  new[] { "BÖLÜM {0} İLE AÇILIR", "UNLOCKS AT LEVEL {0}", "AB LEVEL {0}", "SE ABRE EN EL NIVEL {0}", "DÉBLOQUÉ AU NIVEAU {0}", "LIBERA NO NÍVEL {0}", "SBLOCCATA AL LIVELLO {0}" } },
        { "no_eggs",    new[] { "YETERSİZ YUMURTA!", "NOT ENOUGH EGGS!", "NICHT GENUG EIER!", "¡FALTAN HUEVOS!", "PAS ASSEZ D'ŒUFS !", "OVOS INSUFICIENTES!", "UOVA INSUFFICIENTI!" } },
        { "first_clear",new[] { "İLK GEÇİŞ BONUSU", "FIRST CLEAR BONUS", "ERSTSIEG-BONUS", "BONO PRIMERA VEZ", "BONUS 1RE VICTOIRE", "BÔNUS 1ª VITÓRIA", "BONUS PRIMA VITTORIA" } },
        { "subtitle",   new[] { "TİLKİLER YUMURTALARI ÇALDI!", "THE FOXES STOLE THE EGGS!", "DIE FÜCHSE HABEN DIE EIER GESTOHLEN!", "¡LOS ZORROS ROBARON LOS HUEVOS!", "LES RENARDS ONT VOLÉ LES ŒUFS !", "AS RAPOSAS ROUBARAM OS OVOS!", "LE VOLPI HANNO RUBATO LE UOVA!" } },
        { "play",       new[] { "OYNA", "PLAY", "SPIELEN", "JUGAR", "JOUER", "JOGAR", "GIOCA" } },
        { "quit",       new[] { "ÇIKIŞ", "QUIT", "BEENDEN", "SALIR", "QUITTER", "SAIR", "ESCI" } },
        { "sound_on",   new[] { "SES\nAÇIK", "SOUND\nON", "TON\nAN", "SONIDO\nSÍ", "SON\nOUI", "SOM\nSIM", "AUDIO\nSÌ" } },
        { "sound_off",  new[] { "SES\nKAPALI", "SOUND\nOFF", "TON\nAUS", "SONIDO\nNO", "SON\nNON", "SOM\nNÃO", "AUDIO\nNO" } },
        { "world",      new[] { "DÜNYA", "WORLD", "WELT", "MUNDO", "MONDE", "MUNDO", "MONDO" } },
        { "level",      new[] { "BÖLÜM", "LEVEL", "LEVEL", "NIVEL", "NIVEAU", "NÍVEL", "LIVELLO" } },
        { "score",      new[] { "PUAN", "SCORE", "PUNKTE", "PUNTOS", "SCORE", "PONTOS", "PUNTI" } },
        { "best",       new[] { "EN İYİ", "BEST", "BESTER", "MEJOR", "RECORD", "RECORDE", "RECORD" } },
        { "foxes",      new[] { "TİLKİ", "FOXES", "FÜCHSE", "ZORROS", "RENARDS", "RAPOSAS", "VOLPI" } },
        { "new_duck",   new[] { "YENİ ÖRDEK!", "NEW DUCK!", "NEUE ENTE!", "¡NUEVO PATO!", "NOUVEAU CANARD !", "NOVO PATO!", "NUOVA ANATRA!" } },
        { "tap_start",  new[] { "BAŞLAMAK İÇİN DOKUN", "TAP TO START", "ZUM STARTEN TIPPEN", "TOCA PARA EMPEZAR", "TOUCHE POUR COMMENCER", "TOQUE PARA COMEÇAR", "TOCCA PER INIZIARE" } },
        { "paused",     new[] { "DURAKLATILDI", "PAUSED", "PAUSE", "PAUSA", "PAUSE", "PAUSA", "PAUSA" } },
        { "resume",     new[] { "DEVAM", "RESUME", "WEITER", "SEGUIR", "REPRENDRE", "CONTINUAR", "RIPRENDI" } },
        { "retry",      new[] { "TEKRAR", "RETRY", "NOCHMAL", "REPETIR", "REJOUER", "REPETIR", "RIPROVA" } },
        { "levels",     new[] { "BÖLÜMLER", "LEVELS", "LEVELS", "NIVELES", "NIVEAUX", "NÍVEIS", "LIVELLI" } },
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
    };
}
