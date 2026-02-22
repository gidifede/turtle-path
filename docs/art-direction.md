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
| OpenGameArt (turtle, beach, water) | Pixel art 16/32-bit | Upscale a 64x64 con filtering **Point (no filter)** per mantenere pixel-crisp. Poi palette remapping. L'outline shader 2px uniforma il look. |
| Schwarnhild (baby turtle) | Hand-drawn | Ridisegnare in stile flat OPPURE usare solo come reference per creare una versione flat coerente. Non usare l'asset raw. |
| CraftPix (seabed objects) | Pixel art | Come OpenGameArt: upscale Point + palette remapping + outline shader. |

**Regola:** Se un asset dopo il reskin non sembra coerente con i Kenney, va ridisegnato in stile flat. Il look Kenney è l'ancora di coerenza.

### Risoluzione e scaling
Tutti gli asset finali devono essere **64x64 px** (o multipli per elementi più grandi).

| Asset nativo | Risoluzione | Azione |
|---|---|---|
| Pipes Tileset (Stealthix) | 32x32 | Upscale 2x con Point filter → 64x64 |
| Animated Water Tiles | 32x32 | Upscale 2x con Point filter → 64x64 |
| Turtle Sprite | 64x64 | Nessun resize |
| Pixel Turtle | 66x66 | Crop/resize a 64x64 |
| ZRPG Beach | Variabile | Resize a 64x64 per tile |
| Schwarnhild baby turtle | Variabile | Resize a 32x32 (meta della tessera, sovrapposta) |
| CraftPix seabed | Variabile | Resize a 64x64 per decorazioni |

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
- Animazione idle: piccole onde in loop (2-3 frame)
- Dimensione: 64x64 px

### Stati visivi delle tessere

Ogni tessera ha 4 possibili stati visivi. Tutti devono essere chiaramente distinguibili.

| Stato | Aspetto | Colori |
|---|---|---|
| **Default** (non connessa) | Tessera con percorso visibile, colore neutro, contorno 2px | Percorso: Sand Warm `#FFCC5C`, sfondo: Sand Light `#FFEEAD`, contorno: Deep Brown `#5D4037` |
| **Connessa** (path valido dal nido) | Tessera illuminata, percorso cambia colore | Percorso: Ocean Teal `#96CEB4`, sfondo: Sand Light `#FFEEAD`, contorno: Deep Brown `#5D4037`, glow leggero (bloom) |
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
- Baby Turtle: animazione di "aggancio" (salta verso la tartaruga), poi segue in coda con walk cycle proprio (stessi frame della principale ma scala 0.6x e tinta Baby Pink `#FFB6C1`)

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

**Lavorazione tessere:**
1. Partire dal Pipes Tileset di Stealthix (geometria corretta per Dritto/Curva/T)
2. Reskinare da "tubo metallico" a "sentiero di sabbia compatta" usando la palette Sand Light / Sand Warm
3. Aggiungere contorno 2px Deep Brown
4. Scalare a 64x64 px
5. Creare 4 rotazioni per tipo (0, 90, 180, 270 gradi) come sprite separate O ruotare via codice

### 3.2 Ambiente Spiaggia

| Asset | Fonte | Licenza | Uso in Turtle Path |
|---|---|---|---|
| ZRPG Beach | [OpenGameArt](https://opengameart.org/content/zrpg-beach) | CC-BY-SA 3.0 | Sabbia, conchiglie, 12 varianti banano, 16 palme con ombre. Sfondo e decorazioni attorno alla griglia. |
| Top Down Grass, Beach and Water | [OpenGameArt](https://opengameart.org/content/top-down-grass-beach-and-water-tileset) | Verificare | Transizioni acqua → sabbia → erba. Bordi della griglia e zona mare. |
| Animated Water Tiles | [OpenGameArt](https://opengameart.org/content/animated-water-tiles-0) | Verificare | Acqua animata 32x32, 4 frame. Per la zona mare in basso. |
| Free Top-Down Seabed Objects | [CraftPix](https://craftpix.net/freebies/free-top-down-seabed-objects-pixel-art/) | Royalty-free | Coralli, alghe animate, pietre. Decorazione bordi e zona mare. |

**Nota licenza CC-BY-SA 3.0 (ZRPG Beach):** Richiede attribuzione e share-alike. Inserire crediti nel gioco (schermata Credits). Le modifiche derivate devono mantenere la stessa licenza.

### 3.3 Tartaruga

| Asset | Fonte | Licenza | Uso in Turtle Path |
|---|---|---|---|
| Turtle Sprite (64x64) | [OpenGameArt](https://opengameart.org/content/turtle-sprite) | CC0 | Tartaruga principale. Animazioni: Idle, Walk, Retreat, Power up. |
| Pixel Turtle (66x66) | [OpenGameArt](https://opengameart.org/content/pixel-turtle) | CC0 | Walk cycle alternativo (6 frame). Retract cycle (5 frame). |
| Baby Turtle & Fish (Schwarnhild) | [itch.io](https://schwarnhild.itch.io/) | Royalty-free | Baby turtle per i collezionabili. Stile hand-drawn. **Nota:** il link punta al profilo dell'autore; cercare l'asset specifico "Baby Turtle" o "Sea Creatures" nella pagina. Verificare il link esatto prima del download. |
| Octopus, Jellyfish, Shark, Turtle | [CraftPix](https://craftpix.net/freebies/octopus-jellyfish-shark-and-turtle-free-sprite-pixel-art/) | Royalty-free | Tartaruga alternativa + creature marine per decorazioni future. |

**Scelta tartaruga principale:**
- Scaricare sia Turtle Sprite (64x64) che Pixel Turtle (66x66)
- Valutare quale si adatta meglio alla palette dopo il reskin
- Scegliere UNA sola come tartaruga principale per coerenza
- L'altra puo servire come reference o per le baby turtles

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
| **M1 - Prototype** | Nessun asset esterno. Quadrati colorati generati in Unity (bianco = tessera, grigio = ostacolo, verde = tartaruga, blu = mare). | Zero lavoro artistico. Pura validazione meccanica. |
| **M2 - Core Loop** | Pipes Tileset reskinato (2 tipi × 4 rotazioni = 8 sprite: Dritto e Curva). Turtle Sprite con walk cycle. Sprite connessione luminosa. | Reskin tessere alla palette (~3h). Reskin tartaruga (~2h). Creare sprite glow percorso (~1h). |
| **M3 - Content** | Sprite tessera T (1 tipo × 4 rotazioni = 4 sprite). Sprite conchiglia. Sprite baby turtle. Sprite roccia. Sprite buco. | Reskin tessera T alla palette (~1h). Adattare da CraftPix/Schwarnhild (~3h). |
| **M4 - Polish** | Sfondo spiaggia completo (ZRPG Beach reskinato). Acqua animata. Palme e decorazioni. UI Kenney ricolorata. Effetti particellari (splash, stelle). Post-processing setup. | Reskin ambiente (~6h). UI customization (~3h). Particelle in Unity (~4h). Post-processing (~1h). |
| **M5 - Release** | Splash screen. Icona app. Screenshot per store. | Design finale (~4h). |

**Stima lavoro grafico totale MVP: ~29 ore**

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

| Asset | Licenza | Obbligo |
|---|---|---|
| Kenney (UI, Puzzle, Tower Defense, Icons, Mobile Controls) | CC0 | Nessuno (ma credito apprezzato) |
| Turtle Sprite, Pixel Turtle (OpenGameArt) | CC0 | Nessuno |
| Pipes Tileset (Stealthix) | CC0 | Nessuno |
| ZRPG Beach (OpenGameArt) | CC-BY-SA 3.0 | Attribuzione obbligatoria + share-alike |
| Top-Down Seabed Objects (CraftPix) | CraftPix Free License | Uso commerciale permesso, no redistribuzione raw |
| Baby Turtle (Schwarnhild) | Royalty-free | Verificare termini specifici |
| Animated Water Tiles (OpenGameArt) | Verificare | Verificare prima dell'uso |
| Top Down Grass Beach Water (OpenGameArt) | Verificare | Verificare prima dell'uso |

**Azione richiesta prima di M4:** Verificare le licenze degli asset marcati "Verificare" e inserire i crediti obbligatori nella schermata Credits del gioco.

---

## 8. Checklist Pre-Implementazione per Milestone

Prima di iniziare ogni milestone, verificare:

- [ ] Tutti gli asset necessari sono stati scaricati
- [ ] Tutti gli asset sono stati rimappati alla palette master
- [ ] Tutti gli asset sono alla dimensione corretta (64x64 o multipli)
- [ ] Il contorno 2px e stato applicato (o lo shader e configurato)
- [ ] Gli sprite sono stati aggiunti al Sprite Atlas corretto
- [ ] Le licenze sono state verificate
