# Turtle Path - Art Direction

**Versione:** 1.0
**Data:** 2026-02-22
**Specs di riferimento:** [specs.md](specs.md)

---

## 1. Stile Visivo

**Direzione:** Flat 2D con tocco cozy
**Riferimenti:** Block Blast (chiarezza), Cats & Soup (calore emotivo), Water Connect Puzzle (tema acquatico)

### Principi
- Forme semplici, bordi morbidi/arrotondati
- Nessun gradiente complesso: massimo 2 toni per colore
- Alto contrasto tra elementi interattivi e sfondo
- Leggibilità immediata: il giocatore capisce cosa toccare in < 1 secondo
- Atmosfera calda, accogliente, rilassante

### Regole di stile
- Dimensione tile standardizzata: **64x64 px**
- Contorno uniforme: **2px**, colore Deep Brown (`#5D4037`)
- Angoli arrotondati su tutti gli elementi interattivi
- Ombre morbide, proiettate verso il basso-destra (luce solare)
- Nessun effetto 3D o pseudo-isometrico

### Unificazione stili (CRITICO)
Gli asset provengono da 3 fonti con stili diversi. Devono convergere verso **un unico look flat 2D**:

| Fonte | Stile nativo | Adattamento necessario |
|---|---|---|
| Kenney (UI, Puzzle, Tower Defense) | Flat vector, pulito | Nessuno - è lo stile target. Ricolorare alla palette. |
| OpenGameArt (Turtle Sprite Oiboo) | Pixel art 64x64 | Palette remapping. L'outline shader 2px (M4) uniforma il look. |
| Stealthix (Pipes Tileset) | Pixel art 32x32 | Upscale 2x con filtering **Point (no filter)** a 64x64. Palette remapping. |
| Custom pixel art (collezionabili, nido, mare) | Pixel art | Creare direttamente in stile flat coerente con Kenney, usando la palette master. |

**Regola:** Se un asset dopo il reskin non sembra coerente con i Kenney, va ridisegnato in stile flat. Il look Kenney è l'ancora di coerenza.

### Risoluzione e scaling
Tutti gli asset finali devono essere **64x64 px** (o multipli per elementi più grandi).

| Asset nativo | Risoluzione | Azione | Milestone |
|---|---|---|---|
| Pipes Tileset (Stealthix) | 32x32 | Upscale 2x con Point filter → 64x64 | M2-Bis |
| Turtle Sprite (Oiboo) | 64x64 | Nessun resize, solo palette remap | M2-Bis |
| Custom collectibles | 32x32 | Creare direttamente a 32x32 | M2-Bis |
| Custom cells (nido, mare) | 64x64 | Creare direttamente a 64x64 | M2-Bis |
| Animated Water Tiles | 32x32 | Upscale 2x con Point filter → 64x64 | M4 |
| ZRPG Beach | Variabile | Resize a 64x64 per tile | M4 |

**Import settings Unity per TUTTI gli sprite:**
- Filter Mode: **Point (no filter)**
- Compression: None
- Pixels Per Unit: **64**
- Sprite Mode: Single (o Multiple per spritesheet)

---

## 2. Palette Colori Master

Tutti gli asset devono essere rimappati a questa palette. Nessun colore fuori palette.

### Colori principali (gameplay)

| Nome | Hex | Uso |
|---|---|---|
| Sand Light | `#FFEEAD` | Sfondo sabbia, tessere base |
| Sand Warm | `#FFCC5C` | Bordi tessere, accenti caldi |
| Ocean Teal | `#96CEB4` | Percorso illuminato, acqua bassa |
| Ocean Deep | `#1ECDCB` | Acqua profonda, mare di arrivo |
| Coral Pink | `#FF6F69` | Conchiglie, accenti UI, evidenziazioni |
| Palm Green | `#88D8B0` | Vegetazione, decorazioni |
| Rock Grey | `#A0937D` | Rocce, ostacoli |
| Turtle Green | `#4CAF50` | Tartaruga principale |
| Turtle Shell | `#8B6914` | Guscio tartaruga |
| Baby Pink | `#FFB6C1` | Baby turtles |
| Deep Brown | `#5D4037` | Contorni, ombre |

### Colori UI

| Nome | Hex | Uso |
|---|---|---|
| Shell White | `#F2F0DB` | UI background, testi chiari |
| Sky Blue | `#87CEEB` | Sfondo cielo |
| Star Gold | `#FFD700` | Stelle punteggio |
| UI Dark | `#2C3E50` | Testi scuri, icone |
| BG Off-White | `#EBE5E1` | Background pannelli UI |

### Note
- Mai usare bianco puro (`#FFFFFF`) o nero puro (`#000000`)
- Per variazioni di tonalita usare opacita, non nuovi colori
- Testare la palette sia su display OLED che LCD

---

## 2b. Celle Speciali e Stati Visivi

### Cella Nido (start)
- Sprite dedicato: piccola buca nella sabbia con frammenti di guscio
- Colore base: Sand Warm `#FFCC5C` con dettagli Turtle Shell `#8B6914`
- Posizione: sempre riga 0 (alto)
- Non ruotabile: nessun feedback al tap
- La tartaruga appare qui con animazione Idle prima della partenza
- Dimensione: 64x64 px, stessa della griglia

### Cella Mare (end)
- Sprite dedicato: transizione sabbia → acqua con piccole onde
- Colore: gradiente Sand Light `#FFEEAD` → Ocean Teal `#96CEB4` → Ocean Deep `#1ECDCB`
- Posizione: sempre ultima riga (basso)
- Non ruotabile: nessun feedback al tap
- Animazione idle: piccole onde in loop (da implementare in M4, non in M2-Bis. Se si usa Animated Water Tiles: 4 frame. Se custom: 2-3 frame sufficienti.)
- Dimensione: 64x64 px

### Stati visivi delle tessere

Ogni tessera ha 4 possibili stati visivi. Tutti devono essere chiaramente distinguibili.

| Stato | Aspetto | Colori |
|---|---|---|
| **Default** (non connessa) | Tessera con percorso visibile, colore neutro | Percorso: Sand Warm `#FFCC5C`, sfondo: Sand Light `#FFEEAD`. Contorno Deep Brown `#5D4037` 2px **solo da M4** (outline shader). In M2-Bis: nessun contorno. |
| **Connessa** (path valido dal nido) | Tessera illuminata, overlay colorato | Percorso base invariato + child SpriteRenderer overlay Ocean Teal `#96CEB4` alpha 50%. Contorno: Deep Brown `#5D4037` (M4). **NON usare SpriteRenderer.color** (moltiplicativo, produce colori sporchi). Bloom URP (Intensity 0.2, Threshold 1.5) amplifica il glow dell'overlay. |
| **Fissa** (non ruotabile) | Come default ma con icona lucchetto piccola (8x8 px) nell'angolo in basso a destra | Lucchetto: UI Dark `#2C3E50` con opacita 50% |
| **In drag** (dall'inventario) | Tessera leggermente ingrandita (1.1x), ombra più pronunciata, opacità 80% | Stessi colori di Default con ombra Deep Brown al 30% |

### Collezionabili sulla tessera

Gli oggetti collezionabili sono **sovrapposti al centro della tessera**, dimensione 32x32 px (meta tessera).

| Collezionabile | Aspetto | Animazione |
|---|---|---|
| **Conchiglia** | Sprite conchiglia centrata sulla tessera, sopra il percorso | Leggero bobbing verticale (2px su/giu, 1s loop) |
| **Baby Turtle** | Sprite baby turtle centrata, leggermente più grande della conchiglia | Leggero bobbing + rotazione lenta (±5 gradi, 2s loop) |

Quando la tartaruga principale passa sulla tessera:
- Conchiglia: animazione scale-down + particelle Coral Pink, poi scompare
- Baby Turtle: animazione di "aggancio" (salta verso la tartaruga), poi segue in coda. Lo sprite in convoglio è lo sprite custom `collectible_baby.png` scalato a 0.6x della tartaruga principale (0.6 × 64 = ~38px), tinta Baby Pink `#FFB6C1`. Per M2-Bis: nessun walk cycle proprio (sprite statico come la principale). Walk cycle baby in M4.

### Pannello Inventario (livelli 11-15)

- Posizione: barra orizzontale in basso, sotto la griglia
- Sfondo: pannello BG Off-White `#EBE5E1` con bordi arrotondati e contorno Deep Brown 2px
- Altezza: 80px (tessera 64px + 8px padding sopra/sotto)
- Le tessere nell'inventario sono mostrate a 64x64 px, in stato Default
- Tra una tessera e l'altra: 16px di gap
- Quando il giocatore trascina una tessera dall'inventario, lo slot si svuota (sfondo tratteggiato Sand Light)
- Se la tessera viene rilasciata fuori dalla griglia, torna all'inventario con animazione snap-back (0.2s)

---

## 3. Asset Stack

Tutti gli asset sono gratuiti e con licenze che permettono uso commerciale.

### 3.1 Tessere / Percorsi

| Asset | Fonte | Licenza | Uso in Turtle Path |
|---|---|---|---|
| Pipes Tileset (Stealthix) | [itch.io](https://stealthix.itch.io/pipes-tileset) | CC0 | Base per tessere Dritto, Curva, T. Da reskinare: tubi → sentieri di sabbia. Rimappare a palette master. |
| Tower Defense Top-Down (Kenney) | [kenney.nl](https://kenney.nl/assets/tower-defense-top-down) | CC0 | 300 asset: path tiles top-down + terreno + decorazioni. Stile flat già coerente con la direzione artistica. |
| Puzzle Pack 2 (Kenney) | [kenney.nl](https://www.kenney.nl/assets/puzzle-pack-2) | CC0 | 795 asset supplementari: forme, connettori, elementi puzzle. Sorgenti vettoriali inclusi. |

**Lavorazione tessere (verificato 2026-02-22):**

Il file `Pipes.png` è 480x128 px, contiene 15 pezzi × 4 varianti colore (righe). Pezzi utili (verificato tramite ispezione immagine):
- Colonna 0: Dritto orizzontale (W-E) — non usare per rot 0°
- **Colonna 1: Dritto verticale (N-S)** — corrisponde a `Tile.cs` Straight rot 0° = {N, S}
- **Colonna 2: Curva (N-E)** — corrisponde a `Tile.cs` Curve rot 0° = {N, E}
- Colonne 3-5: Curva nelle altre rotazioni (E-S, S-W, W-N) — non servono (rotazione via codice)
- Colonne 6-9: T-piece (per M3)
- Colonna 10: Croce (non serve per MVP)

Workflow:
1. Estrarre tile 32x32 dal Pipes Tileset (usare riga 3 = azzurro, più vicina al target)
2. Rimuovere sfondo (rendere trasparente i pixel di sfondo)
3. Reskinare da "tubo" a "sentiero di sabbia compatta": palette remap a Sand Light `#FFEEAD` / Sand Warm `#FFCC5C`
4. Upscale 2x con Point filter → 64x64 px
5. Contorno 2px Deep Brown: **rimandato a M4** (outline shader). Per M2-Bis sprite senza contorno.
6. Rotazioni gestite via codice (`transform.rotation` in TileView.cs) — 1 sprite per tipo, non 4

### 3.2 Ambiente Spiaggia

| Asset | Fonte | Licenza | Uso in Turtle Path |
|---|---|---|---|
| ZRPG Beach | [OpenGameArt](https://opengameart.org/content/zrpg-beach) | CC-BY-SA 3.0 | Sabbia, conchiglie, 12 varianti banano, 16 palme con ombre. Sfondo e decorazioni attorno alla griglia. |
| Top Down Grass, Beach and Water | [OpenGameArt](https://opengameart.org/content/top-down-grass-beach-and-water-tileset) | Verificare | Transizioni acqua → sabbia → erba. Bordi della griglia e zona mare. |
| Animated Water Tiles | [OpenGameArt](https://opengameart.org/content/animated-water-tiles-0) | Verificare | Acqua animata 32x32, 4 frame. Per la zona mare in basso. |
| ~~Free Top-Down Seabed Objects~~ | [CraftPix](https://craftpix.net/freebies/free-top-down-seabed-objects-pixel-art/) | Royalty-free | ~~Coralli, alghe animate, pietre.~~ **NON DISPONIBILE:** richiede pagamento/abbonamento nonostante la pagina dica "free". Usare alternative custom o altri pack gratuiti. |

**Nota licenza CC-BY-SA 3.0 (ZRPG Beach):** Richiede attribuzione e share-alike. Inserire crediti nel gioco (schermata Credits). Le modifiche derivate devono mantenere la stessa licenza.

### 3.3 Tartaruga

| Asset | Fonte | Licenza | Uso in Turtle Path | Stato |
|---|---|---|---|---|
| **Turtle Sprite (64x64)** | [OpenGameArt - Oiboo](https://opengameart.org/content/turtle-sprite) | CC0 | **SCELTA come tartaruga principale.** Spritesheet 256x256, 16 frame (4x4), top-down, RGBA con trasparenza. Ha: idle, shell retreat, squished, power-up. **Non ha walk cycle.** | Scaricato e verificato |
| ~~Pixel Turtle (66x66)~~ | [OpenGameArt - alizard](https://opengameart.org/content/pixel-turtle) | CC0 | ~~Walk cycle 6 frame.~~ **SCARTATO:** è side-view, incompatibile con la vista top-down del gioco. | Scaricato e verificato — non usabile |
| ~~Baby Turtle (Schwarnhild)~~ | [itch.io](https://schwarnhild.itch.io/) | Royalty-free | **SCARTATO:** il link punta al profilo dell'autore ma l'asset "Baby Turtle" non esiste nella pagina. Nessun asset baby turtle trovato. | Verificato — non esiste |
| Octopus, Jellyfish, Shark, Turtle | [CraftPix](https://craftpix.net/freebies/octopus-jellyfish-shark-and-turtle-free-sprite-pixel-art/) | Royalty-free | Tartaruga alternativa + creature marine per decorazioni future. Non verificato. | Da verificare |

**Decisione tartaruga (presa 2026-02-22):**
- **Principale:** Turtle Sprite (Oiboo), frame idle (riga 0, colonna 0), top-down 64x64
- **Walk cycle:** non presente nell'asset. Rimandato a M4 (creare 4-6 frame custom o cercare altro asset)
- **Direzione movimento:** rimandato a M4. Per M2-Bis lo sprite è fisso (non ruota verso la direzione)
- **Baby turtle collezionabile:** custom pixel art 32x32 (nessun asset esterno disponibile)
- **Baby turtle in convoglio:** usa lo sprite custom della baby, scala 0.6x della tartaruga principale (0.6 × 64 = ~38px), tinta Baby Pink

### 3.4 UI

| Asset | Fonte | Licenza | Uso in Turtle Path |
|---|---|---|---|
| UI Pack (430+ asset) | [Kenney](https://kenney.nl/assets/ui-pack) | CC0 | Bottoni, pannelli, slider, checkbox. Sorgenti vettoriali per customizzare colori alla palette. |
| Game Icons (105 icone) | [Kenney](https://kenney.nl/assets/game-icons) | CC0 | Stelle, cuori, settings, pausa. |
| Mobile Controls (900 asset) | [Kenney](https://kenney.nl/assets/mobile-controls) | CC0 | Indicatori touch, gesti. |

### 3.5 Audio

| Asset | Fonte | Licenza | Uso in Turtle Path |
|---|---|---|---|
| Interface Sounds | [Kenney](https://kenney.nl/assets/interface-sounds) | CC0 | Click tessera, feedback connessione, navigazione UI |
| Impact Sounds | [Kenney](https://kenney.nl/assets/impact-sounds) | CC0 | Splash mare, raccolta collezionabili |
| Music Loops - Casual & Cute | [OpenGameArt](https://opengameart.org/) | Verificare per asset specifico | Musica ambient loop (cercare "ukulele", "tropical", "casual puzzle") |
| Freesound.org | [Freesound](https://freesound.org/) | CC0 / CC-BY (varia per asset) | SFX supplementari: onde, sabbia, tintinnio conchiglia |

**Note audio:**
- Per la musica ambient: cercare loop royalty-free con strumenti tropicali (ukulele, marimba, steel drum)
- Alternative musica: [Kevin MacLeod / Incompetech](https://incompetech.com/) (royalty-free con attribuzione), [Pixabay Music](https://pixabay.com/music/) (royalty-free)
- Tutti gli SFX devono essere brevi (< 1 secondo) e a tono cozy (nessun suono aggressivo)
- Verificare licenze specifiche prima dell'uso, specialmente per la musica

**Lavorazione UI Kenney:**
1. Scaricare i sorgenti vettoriali (SVG)
2. Ricolorare con palette UI (Shell White, UI Dark, Star Gold, Coral Pink)
3. Esportare in PNG alle risoluzioni necessarie (1x, 2x, 3x per mobile)
4. Mantenere lo stile flat Kenney così com'è: è già coerente con la direzione artistica

---

## 4. Tecniche di Coerenza Visiva

Queste tecniche servono a far sembrare asset da fonti diverse parte dello stesso gioco.

### 4.1 Palette Remapping (obbligatorio)
Ogni asset importato deve essere rimappato alla palette master (sezione 2).
- **Tool:** Aseprite (palette swap), Photoshop (Color Table), GIMP (Color Map)
- **Workflow:** Importa asset → Riduci colori → Rimappa a palette master → Esporta

### 4.2 Outline Shader (Unity)
Applicare un contorno uniforme a tutti gli sprite di gameplay.
- **Implementazione:** Shader Graph URP, sampling alfa ai bordi
- **Parametri:** Spessore 2px, colore Deep Brown (`#5D4037`)
- **Reference:** [Daniel Ilett - 2D Outlines in Shader Graph and URP](https://danielilett.com/2020-04-27-tut5-6-urp-2d-outlines/)
- **Repo:** [github.com/daniel-ilett/2d-outlines-urp](https://github.com/daniel-ilett/2d-outlines-urp)

### 4.3 Post-Processing (Unity URP)
Effetti globali che unificano tutto ciò che appare a schermo.

| Effetto | Impostazione | Scopo |
|---|---|---|
| Color Grading | Temperatura calda (+10), saturazione leggermente ridotta (-5) | Tono cozy uniforme |
| Bloom | Intensita bassa (0.2), soglia alta | Glow morbido sugli elementi luminosi (percorso illuminato, stelle) |
| Vignette | Intensita 0.15, smoothness 0.4 | Focus morbido al centro della griglia |

### 4.4 Effetti Particellari

| Effetto | Quando | Colore | Comportamento |
|---|---|---|---|
| **Raccolta conchiglia** | Tartaruga passa su conchiglia | Coral Pink `#FF6F69` | 5-8 particelle piccole (4x4 px) che esplodono verso l'alto e svaniscono in 0.3s |
| **Raccolta baby turtle** | Tartaruga passa su baby turtle | Baby Pink `#FFB6C1` + Star Gold `#FFD700` | 3-4 cuoricini che salgono e svaniscono in 0.5s |
| **Splash mare** | Tartaruga entra nel mare | Ocean Teal `#96CEB4` + Shell White `#F2F0DB` | Gocce d'acqua a semicerchio verso l'alto, 10-12 particelle, durata 0.5s |
| **Stelle risultato** | Schermata risultato, animazione stelle | Star Gold `#FFD700` | Scintille attorno a ogni stella che si accende, 6-8 particelle per stella |
| **Percorso completato** | Quando l'ultimo collegamento si chiude | Ocean Teal `#96CEB4` | Onda luminosa che percorre tutto il path dal nido al mare in 0.3s |

Tutti gli effetti usano il Particle System di Unity con sprite semplici (cerchio, cuore, goccia) rimappati alla palette master.

### 4.5 Schermate: Dettagli Visivi

**Selezione Livello**

| Elemento | Aspetto |
|---|---|
| Livello sbloccato | Cerchio Sand Warm `#FFCC5C` con numero al centro (UI Dark `#2C3E50`), contorno Deep Brown 2px |
| Livello bloccato | Cerchio Rock Grey `#A0937D` con icona lucchetto (UI Dark `#2C3E50`), opacita 60% |
| Stelle sotto il livello | 3 icone stella in riga: ottenuta = Star Gold `#FFD700`, non ottenuta = Rock Grey `#A0937D` con opacita 30% |
| Livello corrente | Come sbloccato ma con glow pulse (scala 1.0→1.05, 1s loop) |
| Layout | Griglia 3x5 centrata, scrollabile se servono più livelli in futuro |

**Schermata Risultato**

| Elemento | Animazione |
|---|---|
| Stelle (1-3) | Appaiono in sequenza (0.3s delay tra una e l'altra). Ogni stella: scale da 0 a 1.2 poi rimbalzo a 1.0 (0.4s). Particelle scintilla attorno. |
| Baby turtles salvate | Le tartarughine camminano da sinistra verso il mare a destra, una alla volta (0.5s delay). Splash quando entrano. |
| Pannello sfondo | BG Off-White `#EBE5E1`, bordi arrotondati, contorno Deep Brown 2px |
| Bottoni | Stile Kenney UI, colore primario Ocean Teal `#96CEB4` ("Prossimo") e Sand Warm `#FFCC5C` ("Rigioca") |

**Menu Pausa**

| Elemento | Aspetto |
|---|---|
| Overlay | Sfondo scurito (nero 50% opacita) sopra il gameplay |
| Pannello | Centrato, BG Off-White `#EBE5E1`, bordi arrotondati, contorno Deep Brown 2px |
| Bottone "Riprendi" | Kenney UI, colore Ocean Teal `#96CEB4` |
| Bottone "Ricomincia" | Kenney UI, colore Sand Warm `#FFCC5C` |
| Bottone "Menu" | Kenney UI, colore Coral Pink `#FF6F69` |
| Toggle Audio | Icona speaker da Kenney Game Icons, on/off |

### 4.6 Sprite Atlas (Unity)
- **Dimensione massima:** 2048x2048 px per atlas
- **Raggruppamento per co-occorrenza:**
  - Atlas 1: Tessere gameplay (tutte le tessere + ostacoli)
  - Atlas 2: Tartaruga (tutte le animazioni)
  - Atlas 3: UI (bottoni, pannelli, icone)
  - Atlas 4: Ambiente (sabbia, acqua, decorazioni)
- **Beneficio:** riduzione draw calls del 30-50%

---

## 5. Mapping Asset → Milestones

| Milestone | Asset da preparare | Lavoro grafico |
|---|---|---|
| **M1 - Prototype** | Nessun asset esterno. COMPLETATA. | Zero. |
| **M2 - Core Loop** | Nessun asset esterno (logica only). COMPLETATA. | Zero (grafica rinviata a M2-Bis). |
| **M2-Bis - Visual Base** | Pipes Tileset reskinato (2 tipi: Dritto e Curva). Turtle Sprite statico (Oiboo, no walk cycle). Icone collezionabili custom (conchiglia + baby turtle). Sprite nido e mare custom. Setup bloom URP. | Reskin tessere (~2h). Reskin tartaruga (~1h). Custom pixel art collectibles + nido + mare (~2h). Setup bloom (~0.5h). Totale: **~5.5h**. |
| **M3 - Content** | Sprite tessera T (1 tipo, da Pipes Tileset col 6-9). Sprite roccia. Sprite buco. | Reskin tessera T (~1h). Custom pixel art roccia + buco (~2h). Totale: ~3h. |
| **M4 - Polish** | Walk cycle tartaruga (custom o nuovo asset). Direzione tartaruga. Outline shader 2px. Sfondo spiaggia (ZRPG Beach reskinato). Acqua animata. UI Kenney ricolorata. Effetti particellari. Post-processing completo. | Walk cycle + direzione (~3h). Outline shader (~2h). Reskin ambiente (~6h). UI customization (~3h). Particelle (~4h). Post-processing (~1h). **~19h**. |
| **M5 - Release** | Splash screen. Icona app. Screenshot per store. | Design finale (~4h). |

**Stima lavoro grafico totale MVP: ~31.5 ore** (M2-Bis: ~5.5h, M3: ~3h, M4: ~19h, M5: ~4h)

---

## 6. Setup Unity

### Versione Unity
- **Raccomandata:** Unity 6 LTS (6000.x) o successiva
- Verificare compatibilità URP e packages al momento dell'installazione
- Usare la versione LTS più recente disponibile per stabilità

### Packages richiesti
| Package | Scopo | Da quale milestone |
|---|---|---|
| Universal Render Pipeline (URP) | Rendering 2D + 2D lighting + post-processing | M1 |
| 2D Sprite | Sprite Editor | M1 |
| Shader Graph | Outline shader + effetti custom | M4 |
| TextMeshPro | Testi UI | M1 |
| DOTween (free, Asset Store) | Tweening animazioni (rotazione tessere 0.15s, movimento tartaruga) | **M1** (installare subito, usato per la rotazione tessere) |

**Nota su 2D Tilemap:** La griglia di gioco usa un sistema custom (`GridManager.cs`) e NON il Tilemap di Unity. I package "2D Tilemap" e "2D Tilemap Extras" non sono necessari per il gameplay. Se in futuro servissero per lo sfondo ambiente (M4), valutare l'aggiunta in quel momento.

### Struttura cartelle progetto
```
Assets/
  Art/
    Tiles/           # Tessere percorso (dritto, curva, T) - 64x64
    Environment/     # Sabbia, acqua, palme, decorazioni
    Characters/      # Tartaruga principale, baby turtles
    Collectibles/    # Conchiglie, stelle
    Obstacles/       # Rocce, buchi
    UI/              # Bottoni, pannelli, icone (da Kenney)
    Effects/         # Particelle, glow
  Tilemaps/          # Tilemap palettes e rule tiles
  Atlases/           # Sprite Atlas (4 atlas)
  Materials/         # Materiali e shader
  PostProcessing/    # URP post-processing profiles
```

---

## 7. Licenze e Crediti

| Asset | Licenza | Obbligo | Stato |
|---|---|---|---|
| Kenney (UI, Puzzle, Tower Defense, Icons, Mobile Controls) | CC0 | Nessuno (ma credito apprezzato) | Da usare (M2-Bis per UI futura, M4 per ambiente) |
| Turtle Sprite - Oiboo (OpenGameArt) | CC0 | Nessuno | **In uso M2-Bis** |
| Pipes Tileset - Stealthix (itch.io) | CC0 | Nessuno | **In uso M2-Bis** |
| ZRPG Beach (OpenGameArt) | CC-BY-SA 3.0 | Attribuzione obbligatoria + share-alike | Da usare in M4 |
| Animated Water Tiles (OpenGameArt) | Verificare | Verificare prima dell'uso | Da usare in M4 |
| Top Down Grass Beach Water (OpenGameArt) | Verificare | Verificare prima dell'uso | Da usare in M4 |
| ~~Pixel Turtle - alizard (OpenGameArt)~~ | CC0 | - | **SCARTATO** (side-view) |
| ~~Baby Turtle - Schwarnhild (itch.io)~~ | - | - | **SCARTATO** (asset inesistente) |
| ~~Top-Down Seabed Objects (CraftPix)~~ | - | - | **SCARTATO** (a pagamento) |

**Azione richiesta prima di M4:** Verificare le licenze degli asset marcati "Verificare" e inserire i crediti obbligatori nella schermata Credits del gioco.

---

## 8. Checklist Pre-Implementazione per Milestone

Prima di iniziare ogni milestone, verificare:

- [ ] Tutti gli asset necessari sono stati scaricati o creati
- [ ] Tutti gli asset sono stati rimappati alla palette master
- [ ] Tutti gli asset sono alla dimensione corretta (64x64 o 32x32 per collectibles)
- [ ] Il contorno 2px e stato applicato (o lo shader e configurato) — **N/A per M2-Bis** (rimandato a M4)
- [ ] Gli sprite sono stati aggiunti al Sprite Atlas corretto — **N/A per M2-Bis** (Sprite Atlas in M4)
- [ ] Le licenze sono state verificate
- [ ] Gli sprite sono in `Assets/Art/` con le sottocartelle corrette (NON in Resources/)
- [ ] Import settings: Point filter, Compression None, PPU 64, Sprite Mode Single

### Checklist specifica M2-Bis

**Asset da fonti esterne (reskin):**
- [ ] `tile_straight.png` — 64x64, da Pipes.png col 0, palette Sand
- [ ] `tile_curve.png` — 64x64, da Pipes.png col 2, palette Sand
- [ ] `turtle.png` — 64x64, da Oiboo frame 0,0, palette Turtle Green

**Asset custom (pixel art da creare):**
- [ ] `collectible_shell.png` — 32x32, spirale Coral Pink
- [ ] `collectible_baby.png` — 32x32, tartarughina Baby Pink
- [ ] `cell_nest.png` — 64x64, buca sabbia Sand Warm + guscio
- [ ] `cell_sea.png` — 64x64, transizione sabbia-mare

**Decisioni grafiche prese:**
- [x] Tint connessione: overlay semitrasparente (non SpriteRenderer.color)
- [x] Sfondo celle normali: Sand Light #FFEEAD (tint su bianco, nessun sprite)
- [x] Port indicators: nascosti
- [x] Camera background: Sky Blue #87CEEB
- [x] Caricamento sprite: SceneSetup + serializzazione scena (sprite in Assets/Art/)
- [x] Sprite colorati WYSIWYG: colore finale nel PNG, codice usa Color.white (nessun tint)
- [x] Colonne Pipes: Dritto = col 1 (N-S), Curva = col 2 (N-E)
- [x] Baby follower: scala assoluta 0.72 (60% tartaruga, ~23px)
- [x] Contorno 2px: rimandato a M4
- [x] Walk cycle: rimandato a M4
- [x] Direzione tartaruga: rimandato a M4
- [x] Bobbing collectibles: rimandato a M4
- [x] Particelle raccolta: rimandato a M4
