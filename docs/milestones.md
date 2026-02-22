# Turtle Path - Milestones di Sviluppo

**Versione:** 1.0
**Data:** 2026-02-22
**Specs di riferimento:** [specs.md](specs.md)
**Art Direction di riferimento:** [art-direction.md](art-direction.md)

---

## Principi

### Ogni milestone produce una build giocabile
- Ogni milestone genera una **build standalone Windows** testabile sul PC
- I controlli mouse mappano 1:1 i controlli mobile: click sinistro = tap, drag = drag
- La build gira in una finestra **portrait 9:16** (es. 450x800 px) per simulare lo schermo mobile
- Al termine di ogni milestone si esegue una sessione di playtest manuale con la checklist di accettazione

### Regola di avanzamento
Non si passa alla milestone successiva finche TUTTI i criteri di accettazione della milestone corrente non sono soddisfatti. Se un criterio fallisce, si corregge prima di procedere.

### Build settings (Unity)
- Platform: **PC, Mac & Linux Standalone** (per test)
- Resolution: **450x800** (portrait, simula mobile 9:16)
- Windowed mode
- Input: mouse (click = tap, drag = drag)
- Build output: `Builds/M{N}/TurtlePath.exe`

---

## M1 - Prototype

**Obiettivo:** Dimostrare che la meccanica "ruota tessere per creare un percorso" funziona e che un oggetto si muove dal punto A al punto B.

**Durata stimata:** 3-5 giorni

### Cosa implementare

1. **Griglia quadrata 4x4**
   - Matrice di celle renderizzate come quadrati colorati
   - Coordinate (x, y) visibili in debug
   - Cella nido (verde) in alto, cella mare (blu) in basso
   - Riferimento specs: [specs.md - Sezione 3](specs.md#3-griglia)

2. **3 tipi di tessera con rotazione**
   - Dritto (2 porte opposte), Curva (2 porte adiacenti), T (3 porte)
   - Click sinistro su tessera → ruota 90 gradi in senso orario
   - Animazione di rotazione con DOTween (0.15s ease-out). Installare DOTween da M1.
   - Porte visualizzate come linee colorate sui lati del quadrato
   - Riferimento specs: [specs.md - Sezione 4](specs.md#4-tessere)

3. **Validazione percorso (pathfinding)**
   - Algoritmo BFS/DFS dal nido: segui le porte connesse tra tessere adiacenti
   - Tessere connesse al nido cambiano colore (es. da bianco a azzurro)
   - Ricalcolo ad ogni rotazione
   - Riferimento specs: [specs.md - Sezione 6.1](specs.md#61-validazione-continua)

4. **Movimento automatico della tartaruga**
   - Quando il percorso raggiunge il mare → un quadrato verde si muove lungo il path
   - Movimento lineare da cella a cella (velocita costante)
   - Al raggiungimento del mare: messaggio "Livello completato!" in console/UI
   - Riferimento specs: [specs.md - Sezione 6.2 e 6.3](specs.md#62-completamento-percorso)

5. **1 livello di test hardcoded**
   - Griglia 4x4 con tessere pre-piazzate in posizioni fisse
   - Tutte le tessere ruotate in modo casuale all'avvio
   - Il livello ha almeno 1 soluzione valida

### Note tecniche di implementazione

**Rendering (Unity)**
- Ogni cella: GameObject con SpriteRenderer (quad bianco) + BoxCollider2D per input
- Le porte: LineRenderer o child GameObject con SpriteRenderer sottile sui lati
- Tartaruga: GameObject separato con SpriteRenderer, mosso via DOTween
- Camera: Orthographic, size calcolato per contenere la griglia 4x4 + margini
  - orthographicSize = (gridHeight * cellWorldSize + marginTop + marginBottom) / 2
  - La finestra 450x800 ha aspect ratio 0.5625
  - cellWorldSize = 1.0 unità Unity (= 64px a PPU 64)

**Input**
- Usare Unity legacy Input (Input.GetMouseButtonDown(0)) per M1
- Raycast 2D dalla camera al punto di click per identificare la tessera
- Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition))

**Livello di test M1**
Livello 4x4 hardcoded con soluzione verificata:

  Griglia (N=nido, M=mare, S=dritto, C=curva, T=t-piece):

  riga 0:  _  N  _  _       N ha porta south
  riga 1:  _  S  C  _       S(0°)=N,S  C(0°)=N,E
  riga 2:  _  C  C  _       C(90°)=E,S  C(180°)=S,W
  riga 3:  _  _  M  _       M ha porta north

  Soluzione: ruotare le tessere finché il percorso
  nido(1,0) → S(1,1) → C(1,2) → rotata per collegare a (2,2) → C(2,2) → M(2,3)

  Le tessere partono con rotazione randomizzata.
  Il livello è semplice intenzionalmente: valida la meccanica base.

### Grafica
- **Nessun asset esterno.** Solo primitive Unity:
  - Quadrati bianchi = tessere normali
  - Quadrato grigio = ostacolo (se presente)
  - Quadrato verde = nido / tartaruga
  - Quadrato blu = mare
  - Linee colorate = porte delle tessere
  - Azzurro = tessere connesse al percorso valido
- Riferimento art: [art-direction.md - Sezione 5, M1](art-direction.md#5-mapping-asset--milestones)

### Struttura progetto Unity
```
Assets/
  Scripts/
    Grid/
      GridManager.cs          # Crea e gestisce la griglia
      Cell.cs                 # Dati cella (tipo, posizione)
    Tiles/
      Tile.cs                 # Dati tessera (tipo, rotazione, porte)
      TileView.cs             # Rendering tessera + gestione input click
    Path/
      PathValidator.cs        # BFS/DFS dal nido, calcola percorso valido
    Turtle/
      TurtleController.cs     # Movimento lungo il percorso
    Level/
      LevelData.cs            # Struttura dati livello (ScriptableObject o JSON)
      LevelLoader.cs          # Carica e inizializza un livello
    Core/
      GameManager.cs          # Stato del gioco (playing, completed)
  Scenes/
    GameScene.unity           # Scena principale
```

### Criteri di accettazione (TUTTI devono passare)

- [ ] La griglia 4x4 appare centrata nello schermo in finestra 450x800
- [ ] Click sinistro su una tessera la ruota di 90 gradi con animazione visibile
- [ ] Le porte della tessera ruotano correttamente (verificare visivamente che le linee cambino lato)
- [ ] Il percorso valido dal nido si illumina (cambia colore) in tempo reale ad ogni rotazione
- [ ] Tessere non connesse al nido restano nel colore default
- [ ] Quando il percorso collega nido e mare, la tartaruga (quadrato verde) parte automaticamente
- [ ] La tartaruga si muove lungo il percorso cella per cella, senza salti
- [ ] Al raggiungimento del mare, appare un feedback "Completato"
- [ ] Il nido e il mare non sono cliccabili/ruotabili
- [ ] Il livello è rigiocabile (tasto R o bottone per ricominciare)
- [ ] La build Windows si avvia senza errori e gira a 60 fps

### Come testare
1. Aprire la build `Builds/M1/TurtlePath.exe`
2. La finestra si apre in formato portrait (450x800)
3. Cliccare sulle tessere per ruotarle
4. Osservare che il percorso si illumina progressivamente
5. Continuare a ruotare finche il percorso collega nido→mare
6. Verificare che la tartaruga parta e arrivi al mare
7. Premere R per ricominciare e verificare che le tessere tornino in posizioni casuali

---

## M2 - Core Loop

**Obiettivo:** Il core loop completo è giocabile: ruota tessere → percorso si illumina → tartaruga parte → raccoglie bonus → stelle assegnate. 4 livelli con progressione (griglia 4x4, tessere Dritto e Curva).

**Durata stimata:** 5-7 giorni

**Prerequisito:** M1 completata e tutti i criteri di accettazione soddisfatti.

### Cosa implementare

1. **Sistema livelli (4 livelli)**
   - Livelli 1-4 dalla tabella di progressione (tutti 4x4, tessere Dritto e Curva)
   - Livelli caricati da file JSON (formato definito in specs)
   - Transizione tra livelli (livello completato → prossimo livello)
   - Riferimento specs: [specs.md - Sezione 8](specs.md#8-progressione-e-struttura-livelli)

2. **Collezionabili**
   - Conchiglie e baby turtles piazzate su tessere specifiche (da dati livello)
   - La tartaruga li raccoglie automaticamente al passaggio
   - Baby turtle raccolta segue la tartaruga principale (scala ridotta, stessa velocita)
   - Riferimento specs: [specs.md - Sezione 7.1](specs.md#71-oggetti-sul-percorso)

3. **Sistema stelle (1-3)**
   - 1 stella: completa il livello
   - 2 stelle: raccogli tutte le conchiglie
   - 3 stelle: raccogli tutte le conchiglie + salva tutte le baby turtles
   - Stelle persistenti (salvate in PlayerPrefs)
   - Riferimento specs: [specs.md - Sezione 7.2](specs.md#72-sistema-stelle-per-livello)

4. **Schermata risultato livello**
   - Mostra stelle ottenute
   - Bottoni: Prossimo livello, Rigioca, Menu
   - Riferimento specs: [specs.md - Sezione 9.2, Risultato Livello](specs.md#92-schermate)

5. **Selezione livello**
   - Lista/griglia dei 4 livelli con stelle e stato locked/unlocked
   - Riferimento specs: [specs.md - Sezione 9.2, Selezione Livello](specs.md#92-schermate)

6. **Menu principale**
   - Titolo, bottone "Gioca", contatore tartarughe salvate
   - Riferimento specs: [specs.md - Sezione 9.2, Menu Principale](specs.md#92-schermate)

7. **Contatore globale tartarughe**
   - Somma di tutte le baby turtles salvate, persistente (PlayerPrefs)
   - Riferimento specs: [specs.md - Sezione 7.4](specs.md#74-contatore-globale)

### Note tecniche di implementazione

**Scene management**
- Approccio: scena unica (GameScene) con UI overlay gestiti via SetActive
  - MainMenuPanel, LevelSelectPanel, GameplayPanel, ResultPanel, PausePanel
  - Alternativa accettabile: scene separate con SceneManager.LoadScene
  - Scegliere UN approccio e mantenerlo per tutto il progetto

**PlayerPrefs schema**
- "level_{id}_stars" : int (0-3) — stelle ottenute per livello
- "level_{id}_unlocked" : int (0 o 1) — livello sbloccato
- "total_baby_turtles" : int — contatore globale
- Nota: il livello 1 è sempre sbloccato (default)

**Valutazione stelle**
- Le stelle vengono calcolate alla fine dell'animazione tartaruga (stato Result)
- Contare: shellsCollected vs totalShells, babiesCollected vs totalBabies
- Salvare SOLO se il nuovo punteggio è migliore del precedente (max)

**Baby turtle following**
- Le baby turtles raccolte seguono la tartaruga in coda indiana
- Ogni baby segue la posizione della turtle/baby davanti con delay 0.3s
- Implementazione: salvare la lista di posizioni della tartaruga (breadcrumbs)
  ogni baby segue il breadcrumb N posizioni indietro
- Scala: 0.6x rispetto alla tartaruga principale

**Livelli 1-4: criteri di design**
- Livello 1 (4x4): 3-4 tessere, solo Dritto+Curva, 1 conchiglia. Tutorial: insegna il tap.
- Livello 2 (4x4): 5-6 tessere, Dritto+Curva, 2 conchiglie. Percorsi curvi.
- Livello 3 (4x4): 5-6 tessere, Dritto+Curva, 1 conchiglia + 1 baby. Introduce baby turtle.
- Livello 4 (4x4): 6-7 tessere, Dritto+Curva, 2 conchiglie + 1 baby. Due percorsi possibili: uno diretto, uno che raccoglie tutto.
- Ogni livello sarà definito come JSON nel formato specs.md sezione 8.2.

### Grafica
- **Primi asset reskinati:** tessere con percorso visibile (non più linee ma sprite), tartaruga con walk cycle base
- Placeholder accettabili per UI (bottoni Unity default con testo)
- Collezionabili come icone semplici (cerchio rosa = conchiglia, cerchietto verde = baby turtle)
- Riferimento art: [art-direction.md - Sezione 5, M2](art-direction.md#5-mapping-asset--milestones)
- Checklist art pre-M2: [art-direction.md - Sezione 8](art-direction.md#8-checklist-pre-implementazione-per-milestone)

### Struttura progetto (aggiunte a M1)
```
Assets/
  Scripts/
    Level/
      LevelManager.cs         # Gestione progressione livelli, sblocco
      LevelSelectUI.cs        # Schermata selezione livello
    Collectibles/
      Collectible.cs          # Dati collezionabile (tipo, posizione)
      CollectibleView.cs      # Rendering e raccolta
    UI/
      MainMenuUI.cs           # Menu principale
      ResultScreenUI.cs       # Schermata risultato con stelle
      StarDisplay.cs          # Componente stelle (1-3)
    Save/
      SaveManager.cs          # Salvataggio stelle e contatore (PlayerPrefs)
  Data/
    Levels/
      level_01.json           # Livelli 1-4 in formato JSON
      level_02.json
      level_03.json
      level_04.json
  Scenes/
    MainMenuScene.unity
    LevelSelectScene.unity
    GameScene.unity
    ResultScene.unity          # Oppure overlay/popup nella GameScene
```

### Criteri di accettazione

- [ ] Dal menu principale, premere "Gioca" porta alla selezione livelli
- [ ] La selezione mostra 4 livelli: il primo sbloccato, gli altri bloccati
- [ ] Completare il livello N sblocca il livello N+1
- [ ] Nel gameplay, conchiglie e baby turtles sono visibili sulle tessere
- [ ] La tartaruga raccoglie i collezionabili al passaggio (scompaiono)
- [ ] Le baby turtles raccolte seguono la tartaruga in coda
- [ ] Al completamento, la schermata risultato mostra le stelle corrette (1, 2 o 3)
- [ ] Le stelle sono persistenti: chiudere e riaprire il gioco le mantiene
- [ ] Il contatore "Tartarughe salvate" si aggiorna nel menu principale
- [ ] I 4 livelli hanno difficoltà crescente (griglia 4x4, tessere Dritto e Curva)
- [ ] Si può rigiocare un livello per migliorare le stelle
- [ ] Il gioco gira a 60 fps su PC

### Come testare
1. Avviare `Builds/M2/TurtlePath.exe`
2. Dal menu, premere "Gioca"
3. Completare il livello 1 senza raccogliere bonus → verificare 1 stella
4. Rigiocarlo raccogliendo tutte le conchiglie → verificare 2 stelle
5. Rigiocarlo raccogliendo tutto → verificare 3 stelle
6. Verificare che il livello 2 sia sbloccato
7. Completare tutti e 4 i livelli
8. Chiudere il gioco, riaprirlo, verificare che stelle e progressione siano salvate
9. Verificare il contatore tartarughe nel menu

---

## M3 - Content

**Obiettivo:** Tutti i 15 livelli completi, bilanciati e testabili. Tessera T, griglie 5x5 e 6x6, ostacoli e sistema inventario tessere. Tutti i tipi di tessera e ostacoli presenti.

**Durata stimata:** 5-7 giorni

**Prerequisito:** M2 completata e tutti i criteri di accettazione soddisfatti.

### Cosa implementare

1. **Tessera T**
   - Terzo tipo di tessera (3 porte), introdotta al livello 5 (griglia 5x5)
   - Stessa logica di rotazione e connessione
   - Riferimento specs: [specs.md - Sezione 4.1](specs.md#41-tipi)

2. **Griglie 5x5 e 6x6**
   - Griglia 5x5 per livelli 5-10, griglia 6x6 per livelli 11-15
   - La griglia deve restare centrata e adattarsi alla finestra
   - Riferimento specs: [specs.md - Sezione 3.2](specs.md#32-dimensioni-per-livello)

3. **Ostacoli**
   - Roccia: cella non attraversabile, nessuna tessera sopra (introdotta livello 7)
   - Buco: cella vuota permanente, nessuna tessera piazzabile (introdotto livello 9)
   - Riferimento specs: [specs.md - Sezione 5](specs.md#5-ostacoli)

4. **Sistema inventario**
   - Barra in basso con 2-3 tessere trascinabili
   - Drag & drop dalla barra alla griglia (su caselle vuote)
   - Le tessere piazzate dall'inventario sono ruotabili come le altre
   - Se rilasciate fuori dalla griglia, tornano all'inventario
   - Attivo solo nei livelli 11-15
   - Riferimento specs: [specs.md - Sezione 4.3](specs.md#43-layout-griglia)
   - Riferimento art: [art-direction.md - Pannello Inventario](art-direction.md#2b-celle-speciali-e-stati-visivi)

5. **15 livelli completi (aggiunge livelli 5-15 ai 4 di M2)**
   - Tutti i livelli dalla tabella di progressione (specs sezione 8.1)
   - Livello 5: prima griglia 5x5 con tessera T
   - Ogni livello ha almeno 2 percorsi validi (uno diretto, uno che raccoglie tutti i bonus)
   - Ogni livello è risolvibile e testato
   - File JSON per ogni livello (level_05.json ... level_15.json)
   - Riferimento specs: [specs.md - Sezione 8.1](specs.md#81-curva-di-apprendimento-15-livelli)

6. **Menu pausa**
   - Bottone pausa in alto a destra durante il gameplay
   - Overlay con: Riprendi, Ricomincia, Menu
   - Toggle audio (anche se l'audio non c'e ancora, il toggle deve funzionare)
   - Riferimento specs: [specs.md - Sezione 9.2, Menu Pausa](specs.md#92-schermate)

### Note tecniche di implementazione

**Grid scaling formula**
- La griglia deve stare nella viewport 450x800 con margini
- Area utile: larghezza = 450 - 2*margin, altezza = 800 - topUI - bottomInventory - 2*margin
  - margin = 16px, topUI = 80px (contatori), bottomInventory = 80px (se presente, 0 se no)
- cellPixelSize = min(usableWidth / gridWidth, usableHeight / gridHeight)
- cellWorldSize = cellPixelSize / PPU (PPU = 64)
- Centrato orizzontalmente e verticalmente nell'area utile

**Drag & drop (inventario)**
- Implementazione: IBeginDragHandler, IDragHandler, IEndDragHandler su ogni tessera inventario
- Durante il drag: la tessera segue il mouse/touch, scala 1.1x, opacità 80%
- Al release:
  1. Raycast sulla griglia per trovare la cella sotto il cursore
  2. Se cella valida (type=normal, tile=null): piazza la tessera, rimuovi dallo slot inventario
  3. Se cella non valida o fuori griglia: animazione snap-back DOTween (0.2s) allo slot originale
- Tessera piazzata diventa una tessera normale (ruotabile con click)
- Lo slot inventario svuotato mostra sfondo tratteggiato

**Tessera T: comportamento ai bivi**
- La T ha 3 porte, quindi può creare biforcazioni nel flood fill
- Il flood fill illumina TUTTE le tessere raggiungibili (entrambi i rami)
- Il path tracing (BFS nido→mare) trova il percorso più breve
- Se entrambi i rami portano al mare: la tartaruga prende il più corto
- Se solo un ramo porta al mare: la tartaruga prende quello
- I collezionabili su rami non percorsi dalla tartaruga NON vengono raccolti
  → il giocatore deve riconfiguare le tessere per creare un percorso che li includa

**Level design guidelines (livelli 5-15)**
- Principio: ogni livello introduce UNA meccanica nuova, poi il successivo la consolida
- Ogni livello DEVE avere almeno 2 configurazioni valide che portano nido→mare
- Per le 3 stelle: almeno 1 configurazione deve passare per tutti i collezionabili
- Test: per ogni livello, verificare a mano che:
  1. Esiste almeno una soluzione (path nido→mare)
  2. Esiste una soluzione "3 stelle" (path che raccoglie tutto)
  3. La soluzione non è banale (richiede almeno 3-4 rotazioni)

### Grafica
- Stessi asset di M2 per tessere e tartaruga
- Sprite placeholder per roccia (quadrato grigio scuro) e buco (quadrato nero con bordo tratteggiato)
- Pannello inventario con sfondo chiaro e tessere trascinabili
- Riferimento art: [art-direction.md - Sezione 5, M3](art-direction.md#5-mapping-asset--milestones)

### Struttura progetto (aggiunte a M2)
```
Assets/
  Scripts/
    Tiles/
      TileInventory.cs         # Gestione inventario tessere
      TileInventoryUI.cs       # Rendering barra inventario
      TileDragHandler.cs       # Logica drag & drop
    Grid/
      GridScaler.cs            # Adatta dimensione griglia alla finestra
    UI/
      PauseMenuUI.cs           # Menu pausa overlay
  Data/
    Levels/
      level_05.json ... level_15.json   # Aggiunge livelli 5-15 (1-4 già da M2)
```

### Criteri di accettazione

- [ ] Tutti e 15 i livelli sono giocabili dall'inizio alla fine
- [ ] Livelli 1-4: griglia 4x4, solo Dritto e Curva, nessun ostacolo (già da M2)
- [ ] Livello 5-6: griglia 5x5, tessera T presente e funzionante (nuovi in M3)
- [ ] Livello 7-8: rocce presenti, non cliccabili, il percorso le aggira
- [ ] Livello 9-10: buchi presenti, nessuna tessera piazzabile sopra
- [ ] Livello 11-15: inventario visibile in basso con 2-3 tessere
- [ ] Drag & drop dall'inventario alla griglia funziona (mouse down → drag → release)
- [ ] Tessera rilasciata fuori dalla griglia torna all'inventario
- [ ] Tessera piazzata dall'inventario e ruotabile con click
- [ ] Ogni livello ha almeno 2 soluzioni (verificare che il percorso "bonus completo" funzioni)
- [ ] Il menu pausa si apre/chiude correttamente
- [ ] "Ricomincia" dal menu pausa resetta il livello
- [ ] La progressione 1→15 è fluida, la difficoltà cresce gradualmente
- [ ] Nessun livello è impossibile da completare
- [ ] La griglia 5x5 e 6x6 restano centrate e leggibili nella finestra 450x800
- [ ] Il gioco gira a 60 fps anche sulla griglia 6x6

### Come testare
1. Avviare `Builds/M3/TurtlePath.exe`
2. Giocare tutti i 15 livelli in sequenza
3. Per ogni livello, verificare:
   - È completabile?
   - Si possono ottenere 3 stelle (percorso bonus)?
   - I nuovi elementi (T, rocce, buchi, inventario) funzionano?
4. Testare il drag & drop: trascinare tessere dall'inventario, rilasciarle fuori, riprovarle
5. Testare il menu pausa: aprire, riprendere, ricominciare, tornare al menu
6. Completare tutti i 15 livelli e verificare il contatore tartarughe

---

## M4 - Polish

**Obiettivo:** Grafica finale, animazioni fluide, audio, effetti particellari. Il gioco deve sentirsi "cozy" e completo visivamente.

**Durata stimata:** 7-10 giorni

**Prerequisito:** M3 completata e tutti i criteri di accettazione soddisfatti. **IMPORTANTE:** Completare la [checklist pre-implementazione art](art-direction.md#8-checklist-pre-implementazione-per-milestone) PRIMA di iniziare M4.

### Cosa implementare

1. **Grafica tessere finale**
   - Sprite tessere reskinati da Pipes Tileset → sentieri di sabbia
   - 3 tipi × 4 rotazioni = 12 sprite (o 3 sprite ruotati via codice)
   - 4 stati visivi: default, connessa, fissa, in drag
   - Riferimento art: [art-direction.md - Sezione 3.1](art-direction.md#31-tessere--percorsi) e [Stati Visivi](art-direction.md#2b-celle-speciali-e-stati-visivi)

2. **Grafica celle speciali**
   - Sprite nido (buca con frammenti di guscio)
   - Sprite mare (transizione sabbia→acqua con onde animate 2-3 frame)
   - Sprite roccia e buco
   - Riferimento art: [art-direction.md - Celle Speciali](art-direction.md#2b-celle-speciali-e-stati-visivi)

3. **Tartaruga animata**
   - Sprite tartaruga principale con walk cycle (Idle, Walk)
   - Baby turtle con walk cycle a scala 0.6x e tinta Baby Pink
   - Riferimento art: [art-direction.md - Sezione 3.3](art-direction.md#33-tartaruga)

4. **Collezionabili animati**
   - Conchiglie con bobbing (2px, 1s loop)
   - Baby turtles con bobbing + rotazione lenta
   - Animazione raccolta (scale-down + particelle per conchiglie, jump + aggancio per baby)
   - Riferimento art: [art-direction.md - Collezionabili sulla tessera](art-direction.md#2b-celle-speciali-e-stati-visivi)

5. **Sfondo spiaggia**
   - Sfondo completo con sabbia, acqua animata, palme decorative
   - Riferimento art: [art-direction.md - Sezione 3.2](art-direction.md#32-ambiente-spiaggia)

6. **UI finale**
   - Tutti i bottoni, pannelli, icone da Kenney UI ricolorati alla palette
   - Stelle animate nella schermata risultato (sequenza scale+rimbalzo)
   - Selezione livelli con aspetto locked/unlocked/corrente
   - Riferimento art: [art-direction.md - Sezione 3.4](art-direction.md#34-ui) e [Schermate](art-direction.md#45-schermate-dettagli-visivi)

7. **Effetti particellari**
   - Raccolta conchiglia: particelle Coral Pink
   - Raccolta baby turtle: cuoricini Baby Pink + Star Gold
   - Splash mare: gocce Ocean Teal + Shell White
   - Percorso completato: onda luminosa nido→mare
   - Stelle risultato: scintille Star Gold
   - Riferimento art: [art-direction.md - Effetti Particellari](art-direction.md#44-effetti-particellari)

8. **Outline shader**
   - Shader Graph URP: contorno 2px Deep Brown su tutti gli sprite di gameplay
   - Riferimento art: [art-direction.md - Sezione 4.2](art-direction.md#42-outline-shader-unity)

9. **Post-processing**
   - Color Grading (temperatura calda)
   - Bloom leggero (percorso illuminato, stelle)
   - Vignette sottile
   - Riferimento art: [art-direction.md - Sezione 4.3](art-direction.md#43-post-processing-unity-urp)

10. **Audio**
    - Musica ambient loop (ukulele, marimba, pad oceanici)
    - Effetti sonori per ogni azione (rotazione, connessione, raccolta, splash)
    - Toggle audio funzionante nel menu pausa
    - Riferimento specs: [specs.md - Sezione 10](specs.md#10-audio-e-atmosfera)

11. **Sprite Atlas**
    - 4 atlas configurati (gameplay, tartaruga, UI, ambiente)
    - Riferimento art: [art-direction.md - Sezione 4.6](art-direction.md#46-sprite-atlas-unity)

### Note tecniche di implementazione

**Strategia di transizione da placeholder a sprite finali**
- NON creare un sistema parallelo. Sostituire direttamente:
  1. TileView.cs: il SpriteRenderer punta allo sprite placeholder → cambiare il riferimento allo sprite finale
  2. Usare un ScriptableObject "TileSkin" che mappa tipo+stato → sprite
     - TileSkin contiene: defaultSprite, connectedSprite, fixedSprite, dragSprite
  3. Per ogni tipo tessera: creare un TileSkin asset con i 4 sprite
  4. TileView legge il TileSkin e cambia sprite in base allo stato corrente

**Animazioni: DOTween vs Animator**
- Walk cycle tartaruga: Animator Controller con 2 stati (Idle, Walk)
  - Frame animation tramite sprite swap (4-6 frame per ciclo)
  - Trigger "Walk" per avviare, "Idle" per fermare
- Bobbing collezionabili: DOTween (DOMoveY loop PingPong)
- Rotazione tessere: DOTween (DORotate 90°)
- Animazioni UI (stelle, bottoni): DOTween (DOScale, DOFade)
- Regola: Animator solo per sprite frame animation, DOTween per tutto il resto

**Audio manager**
- AudioManager singleton (DontDestroyOnLoad)
- 2 AudioSource: uno per musica (loop), uno per SFX (one-shot)
- Metodi pubblici: PlaySFX(AudioClip), PlayMusic(AudioClip), SetMute(bool)
- Volume e mute salvati in PlayerPrefs ("audio_muted": 0/1)
- SFX serviti tramite AudioClip references su ScriptableObject o enum→clip dictionary

**Sostituzione sprite: ordine di lavoro**
1. Prima: tessere (impatto visivo maggiore, core gameplay)
2. Poi: tartaruga + collezionabili
3. Poi: celle speciali (nido, mare, roccia, buco)
4. Poi: UI (bottoni, pannelli, stelle)
5. Infine: sfondo, decorazioni, particelle, post-processing

### Criteri di accettazione

- [ ] Tutte le tessere usano sprite finali (sentieri di sabbia, non quadrati colorati)
- [ ] I 4 stati visivi delle tessere sono distinguibili (default/connessa/fissa/in drag)
- [ ] Il nido ha sprite con frammenti di guscio, il mare ha onde animate
- [ ] La tartaruga ha animazione walk visibile mentre si muove
- [ ] Le baby turtles raccolte seguono con walk cycle a scala ridotta e tinta rosa
- [ ] I collezionabili hanno bobbing animation
- [ ] Le animazioni di raccolta (scale-down, particelle, jump) funzionano
- [ ] Lo sfondo spiaggia è visibile con sabbia, acqua e decorazioni
- [ ] Tutti i bottoni UI usano sprite Kenney ricolorati alla palette
- [ ] Le stelle nella schermata risultato hanno animazione sequenziale
- [ ] Tutti e 5 gli effetti particellari funzionano (conchiglia, baby, splash, onda, stelle)
- [ ] L'outline shader 2px è visibile su tessere, tartaruga e collezionabili
- [ ] Il post-processing è attivo (color grading caldo, bloom leggero, vignette)
- [ ] La musica ambient gira in loop senza tagli udibili
- [ ] Ogni azione ha il suo effetto sonoro
- [ ] Il toggle audio muta/riattiva musica e sfx
- [ ] TUTTI i colori a schermo appartengono alla palette master (nessun colore fuori palette)
- [ ] Nessun asset ha uno stile visivo incoerente con gli altri
- [ ] Il gioco gira a 60 fps con tutti gli effetti attivi
- [ ] Il gioco "si sente" cozy: rilassante, nessuna pressione, visivamente armonioso

### Come testare
1. Avviare `Builds/M4/TurtlePath.exe`
2. Verificare che il menu principale abbia grafica finale (non placeholder)
3. Giocare 3-4 livelli prestando attenzione a:
   - Fluidita animazioni (nessun scatto)
   - Suoni corretti per ogni azione
   - Particelle visibili e nel colore giusto
   - Outline visibile su tutti gli sprite
4. Mettere in pausa, verificare overlay e toggle audio
5. Alla schermata risultato, verificare animazione stelle
6. Fare screenshot e confrontare con la palette master
7. Chiedere a 2-3 persone di provarlo per feedback "prima impressione"

---

## M5 - Release

**Obiettivo:** Build mobile (iOS + Android) funzionanti, testate su dispositivi reali, pronte per soft launch.

**Durata stimata:** 5-7 giorni

**Prerequisito:** M4 completata e tutti i criteri di accettazione soddisfatti.

### Cosa implementare

1. **Build mobile**
   - Configurazione build per iOS (Xcode project) e Android (APK/AAB)
   - Adattamento aspect ratio (16:9, 18:9, 19.5:9, 20:9)
   - Safe area handling (notch, barra navigazione)
   - Riferimento specs: [specs.md - Sezione 13](specs.md#13-target-tecnici)

2. **Input mobile**
   - Verificare che tap e drag funzionino su touchscreen
   - Feedback aptico (vibrazione leggera) su tap tessera
   - Testare thumb zone (raggiungibilita con una mano)
   - Riferimento specs: [specs.md - Sezione 11](specs.md#11-controlli)

3. **Performance mobile**
   - Profiling su dispositivi target (iPhone 8+, Android 8.0+ / 2GB RAM)
   - 60 fps stabili
   - Tempo caricamento livello < 1 secondo
   - Dimensione build < 100 MB
   - Riferimento specs: [specs.md - Sezione 13](specs.md#13-target-tecnici)

4. **Splash screen e icona**
   - Splash screen con logo Turtle Path
   - Icona app (1024x1024 per iOS, 512x512 per Android)
   - Riferimento art: [art-direction.md - Sezione 5, M5](art-direction.md#5-mapping-asset--milestones)

5. **Bug fixing e QA**
   - Fix bug emersi dal testing su dispositivi reali
   - Verificare salvataggio/caricamento progressione su mobile
   - Testare interruzioni (chiamata in arrivo, app in background, notifiche)

6. **Verifica licenze**
   - Tutte le licenze degli asset verificate
   - Schermata Credits con attribuzioni obbligatorie (CC-BY-SA per ZRPG Beach)
   - Riferimento art: [art-direction.md - Sezione 7](art-direction.md#7-licenze-e-crediti)

### Note tecniche di implementazione

**Safe area**
- Usare Screen.safeArea per ottenere il rettangolo sicuro
- Applicare come RectTransform offsets al Canvas root o a un SafeAreaPanel
- Tutti gli elementi UI interattivi devono stare dentro la safe area
- La griglia di gioco può estendersi fuori (è decorativa ai bordi)

**Input mobile**
- Unity legacy Input funziona sia con mouse che touch (Input.GetMouseButtonDown(0) = primo tocco)
- Per drag & drop: stessa implementazione di M3 (IBeginDragHandler ecc.), funziona con touch
- Feedback aptico: Handheld.Vibrate() per vibrazione di base, oppure
  iOS: Core Haptics via plugin, Android: Vibrator API
  Per MVP: Handheld.Vibrate() è sufficiente (breve vibrazione)

**Aspect ratio e scaling**
- Canvas Scaler: mode = "Scale With Screen Size", reference = 450x800, match = 0.5
- La griglia usa coordinate world (non UI), quindi il GridScaler gestisce il fit
- Testare con Game view a: 450x800, 450x900, 450x975, 414x896 (iPhone 11)

**Performance checklist**
- Sprite Atlas: verificare che tutti gli sprite siano in atlas (4 atlas da art-direction)
- Draw calls: target < 30 per la scena gameplay
- Particle system: max 50 particelle attive simultaneamente
- Audio: comprimere in Vorbis le clip > 1s, PCM per SFX brevi (< 0.5s)
- Build size: usare compressione ASTC per Android, PVRTC per iOS
- Profiling: usare Unity Profiler su device reale, target frame time < 16.6ms

### Criteri di accettazione

- [ ] La build Android (APK) si installa e avvia su un dispositivo reale
- [ ] La build iOS si avvia su un dispositivo reale (o simulatore Xcode)
- [ ] Il gioco gira a 60 fps su dispositivo minimo target
- [ ] Il tempo di caricamento di qualsiasi livello e < 1 secondo
- [ ] La dimensione della build e < 100 MB
- [ ] Tap e drag funzionano correttamente su touchscreen
- [ ] Il feedback aptico funziona (vibrazione leggera su tap)
- [ ] Il layout si adatta correttamente a aspect ratio diversi (testare almeno 2)
- [ ] La safe area e rispettata (nessun elemento sotto il notch)
- [ ] Il gioco gestisce correttamente le interruzioni (app in background → ripresa)
- [ ] I progressi vengono salvati e ripristinati dopo chiusura/riapertura app
- [ ] La schermata Credits è presente con tutte le attribuzioni obbligatorie
- [ ] Tutti i 15 livelli sono completabili su mobile senza problemi di input
- [ ] Il gioco è stato testato da almeno 3 persone su dispositivi diversi
- [ ] Nessun crash o bug bloccante nei 15 livelli

### Come testare
1. Installare la build su un telefono Android e un iPhone (o simulatore)
2. Giocare tutti i 15 livelli dall'inizio alla fine su mobile
3. Verificare che i controlli touch siano precisi e comodi
4. Minimizzare l'app durante il gameplay, riaprirla, verificare che lo stato sia preservato
5. Verificare la schermata Credits
6. Misurare la dimensione della build installata
7. Far provare il gioco a 3 persone che non lo hanno mai visto: raccogliere feedback

---

## Riepilogo tempi stimati

| Milestone | Durata stimata | Cumulativo |
|---|---|---|
| M1 - Prototype | 3-5 giorni | 3-5 giorni |
| M2 - Core Loop | 5-7 giorni | 8-12 giorni |
| M3 - Content | 5-7 giorni | 13-19 giorni |
| M4 - Polish | 7-10 giorni | 20-29 giorni |
| M5 - Release | 5-7 giorni | 25-36 giorni |

**Stima totale: 5-7 settimane** per MVP completo.

Le stime non includono il tempo per il lavoro grafico (reskin asset, palette remapping). Vedi [art-direction.md - Sezione 5](art-direction.md#5-mapping-asset--milestones) per la stima separata (~28 ore).
