# Turtle Path - MVP Technical Specifications

**Versione:** 1.0
**Data:** 2026-02-22
**Stato:** MVP - Validazione Core Loop
**Art Direction:** [art-direction.md](art-direction.md)
**Milestones:** [milestones.md](milestones.md)

---

## 1. Overview

**Nome:** Turtle Path
**Genere:** Puzzle Cozy
**Piattaforma:** Mobile (iOS / Android)
**Engine:** Unity + C#
**Target:** 5-55 anni
**Sessione media:** 1-3 minuti

**Obiettivo MVP:** Validare che il core loop (ruota tessere, crea percorso, guida la tartaruga al mare) sia divertente e soddisfacente. Nessuna monetizzazione, nessun social, nessuna personalizzazione.

---

## 2. Core Loop

```
Osserva griglia → Ruota tessere (tap) → Percorso si illumina in tempo reale →
Percorso completo → Tartaruga parte automaticamente → Raccolta bonus →
Valutazione stelle → Livello successivo
```

---

## 3. Griglia

### 3.1 Tipo
- Quadrata (coordinate x, y)
- Bordi visivi morbidi/organici per estetica cozy (la griglia sottostante resta quadrata)
- Stile visivo e regole grafiche: vedi [art-direction.md - Stile Visivo](art-direction.md#1-stile-visivo)

### 3.2 Dimensioni per livello
| Livelli | Dimensione griglia | Note |
|---|---|---|
| 1-4 | 4x4 | Introduzione meccaniche base (Dritto, Curva) |
| 5-10 | 5x5 | Introduzione tessera T (livello 5) e primo ostacolo (livello 7) |
| 11-15 | 6x6 | Tutti gli elementi, tessere mancanti da inventario |

### 3.3 Orientamento
- **Start (nido):** sempre in alto (riga 0, colonna centrale o variabile)
- **End (mare):** sempre in basso (ultima riga, colonna centrale o variabile)
- Il nido e il mare sono celle speciali, non ruotabili
- **Porte nido:** il nido ha sempre una porta "south" (connessione verso il basso)
- **Porte mare:** il mare ha sempre una porta "north" (connessione verso l'alto)

> **Aspetto visivo nido e mare:** vedi [art-direction.md - Celle Speciali](art-direction.md#2b-celle-speciali-e-stati-visivi). Il nido ha sprite con frammenti di guscio, il mare ha transizione sabbia→acqua con onde animate.

### 3.4 Sistema di coordinate

- Origine (0,0): angolo in ALTO A SINISTRA della griglia
- Asse X: cresce verso destra (0 = colonna sinistra)
- Asse Y: cresce verso il BASSO (0 = riga superiore = riga del nido)
- Mapping a Unity world space: la cella (x, y) ha centro a
  worldX = gridOriginX + x * cellSize
  worldY = gridOriginY - y * cellSize  (Y invertito rispetto a Unity)
- cellSize: calcolato dinamicamente per riempire la finestra (vedi GridScaler)

Adiacenze:
| Direzione | Offset     |
|-----------|------------|
| North     | (0, -1)    |
| South     | (0, +1)    |
| East      | (+1, 0)    |
| West      | (-1, 0)    |

---

## 4. Tessere

### 4.1 Tipi

| Tipo | Connessioni | Icona | Introduzione |
|---|---|---|---|
| **Dritto** | 2 lati opposti (N-S oppure E-W) | `│` o `─` | Livello 1 |
| **Curva** | 2 lati adiacenti (N-E, E-S, S-W, W-N) | `╮` `╯` `╰` `╭` | Livello 1 |
| **T** | 3 lati (tutte le combinazioni) | `┬` `┤` `┴` `├` | Livello 5 |

> **Asset e lavorazione grafica tessere:** vedi [art-direction.md - Tessere/Percorsi](art-direction.md#31-tessere--percorsi). Include asset source, pipeline di reskin (tubi → sentieri di sabbia), palette remapping e dimensioni (64x64 px).

### 4.2 Comportamento
- Ogni tessera ha delle **porte** (lati aperti) che definiscono da dove il percorso entra/esce
- Una tessera si connette alla tessera adiacente se entrambe hanno una porta sul lato condiviso
- Tap su una tessera → ruota di 90 gradi in senso orario
- Le tessere sul nido e sul mare sono fisse e non ruotabili

> **4 stati visivi delle tessere** (default, connessa, fissa, in drag) definiti in [art-direction.md - Stati Visivi](art-direction.md#2b-celle-speciali-e-stati-visivi) con colori e comportamento per ciascuno.

### 4.3 Layout griglia

| Livelli | Layout |
|---|---|
| 1-10 | **Pre-riempita:** tutte le tessere sono sulla griglia, il giocatore le ruota |
| 11-15 | **Tessere mancanti:** alcune caselle sono vuote. Un inventario laterale mostra 2-3 tessere da trascinare (drag & drop) nelle caselle vuote, poi ruotare |

> **Aspetto pannello inventario:** barra orizzontale in basso (80px), sfondo BG Off-White, tessere a 64x64 con gap 16px. Dettagli in [art-direction.md - Pannello Inventario](art-direction.md#2b-celle-speciali-e-stati-visivi).

### 4.4 Struttura dati tessera
```
Tile {
    type: "straight" | "curve" | "t"
    rotation: 0 | 90 | 180 | 270    // gradi, senso orario
    position: { x: int, y: int }
    isFixed: bool                     // true per nido, mare, e tessere bloccate
    ports: [Direction]                // calcolato da type + rotation
}

Direction: "north" | "east" | "south" | "west"
```

### 4.5 Porte per tipo e rotazione

Le porte base (a rotazione 0°) sono:

| Tipo    | Rotazione 0° | 90°      | 180°     | 270°     |
|---------|-------------|----------|----------|----------|
| Dritto  | N, S        | E, W     | N, S     | E, W    |
| Curva   | N, E        | E, S     | S, W     | W, N    |
| T       | N, E, S     | E, S, W  | S, W, N  | W, N, E |

Formula: per ogni porta a rotazione 0°, applicare la rotazione in senso orario:
- 90°: N→E, E→S, S→W, W→N
- 180°: N→S, E→W, S→N, W→E
- 270°: N→W, E→N, S→E, W→S

---

## 5. Ostacoli

| Ostacolo | Comportamento | Aspetto | Introduzione |
|---|---|---|---|
| **Roccia** | Casella occupata e non attraversabile. Nessuna tessera ci va sopra. Il percorso deve aggirarla. | Roccia/corallo sulla sabbia | Livello 7 |
| **Buco** | Casella vuota permanente. Non si possono piazzare tessere (neanche dall'inventario). | Buca nella sabbia | Livello 9 |

> **Asset grafici ostacoli:** custom pixel art (roccia e buco, da creare in M3). Colore roccia: Rock Grey `#A0937D` da [palette master](art-direction.md#2-palette-colori-master). Vedi [art-direction.md - Mapping Milestones](art-direction.md#5-mapping-asset--milestones).

### Struttura dati cella griglia
```
Cell {
    position: { x: int, y: int }
    type: "normal" | "rock" | "hole" | "nest" | "sea"
    tile: Tile | null               // null se rock, hole, o casella vuota da riempire
}
```

---

## 6. Feedback in Tempo Reale e Percorso

### 6.1 Validazione continua
- Ad ogni rotazione/piazzamento di tessera, il sistema ricalcola il percorso valido partendo dal nido
- Le tessere connesse correttamente al nido si **illuminano** progressivamente (scia luminosa / colore)
- Le tessere non ancora connesse restano in stato neutro (nessun feedback negativo)
- Colore illuminazione percorso valido: Ocean Teal `#96CEB4` da [palette master](art-direction.md#2-palette-colori-master)

### 6.2 Completamento percorso
- Quando il percorso raggiunge la cella mare → **la tartaruga parte automaticamente**
- Nessun bottone "Avvia"
- Breve delay (0.5s) tra completamento e partenza per dare soddisfazione visiva

### 6.3 Animazione percorso
- La tartaruga cammina lungo il percorso con animazione fluida
- Velocità costante, proporzionale alla lunghezza del percorso (durata totale: 2-4 secondi)
- Raccoglie i bonus sul percorso al passaggio
- Animazione finale: la tartaruga entra in acqua con splash + particelle

> **Asset grafici per feedback e animazioni:** Sprite glow percorso e particelle splash/stelle in [art-direction.md - Mapping Milestones](art-direction.md#5-mapping-asset--milestones) (M2 e M4). Effetto bloom sul percorso illuminato via [post-processing URP](art-direction.md#43-post-processing-unity-urp). Sprite tartaruga e walk cycle in [art-direction.md - Tartaruga](art-direction.md#33-tartaruga). Dettaglio effetti particellari (colori, quantita, durata) in [art-direction.md - Effetti Particellari](art-direction.md#44-effetti-particellari).

### 6.4 Algoritmo pathfinding

Due operazioni distinte, entrambe ricalcolate ad ogni rotazione/piazzamento:

**1. Flood fill (BFS) dal nido** — per illuminare le tessere connesse
- Partire dalla cella nido
- Per ogni cella visitata, controllare le porte della tessera
- Per ogni porta, controllare la cella adiacente:
  se la cella adiacente ha una tessera con una porta opposta → connessa
- Risultato: set di celle connesse al nido (per feedback visivo)

**2. Path tracing (BFS shortest path) nido → mare** — per il movimento tartaruga
- Eseguire solo se la cella mare è nel set delle celle connesse
- BFS dal nido al mare, seguendo solo le connessioni valide
- Risultato: lista ordinata di celle [nido, ..., mare]
- La tartaruga segue QUESTO percorso, raccogliendo i collezionabili sulle celle attraversate
- Se esistono più percorsi di uguale lunghezza, il BFS restituisce il primo trovato (ordine: N, E, S, W)

Nota: la scelta del percorso non è del giocatore. Il giocatore crea il percorso
ruotando le tessere. La tartaruga segue sempre il percorso più breve.
Per raccogliere tutti i bonus, il giocatore deve configurare le tessere
in modo che il percorso passi per le caselle con collezionabili.

### 6.5 Stati del gioco

| Stato       | Input consentito        | Cosa succede                                  |
|-------------|------------------------|-----------------------------------------------|
| Editing     | Tap tessere, drag inv. | Il giocatore ruota/piazza tessere. Path validation attiva. |
| Completed   | Nessuno (0.5s)         | Percorso completato, delay prima della partenza tartaruga. |
| Animating   | Nessuno                | La tartaruga si muove lungo il percorso. Nessun input accettato. |
| Result      | Tap bottoni UI         | Schermata risultato con stelle e bottoni.     |
| Paused      | Tap bottoni menu pausa | Overlay pausa sopra il gameplay.              |

Transizioni:
Editing → Completed (quando flood fill include la cella mare)
Completed → Animating (dopo 0.5s delay)
Animating → Result (tartaruga raggiunge il mare)
Editing ↔ Paused (bottone pausa)
Result → Editing (Rigioca) | LevelSelect (Menu) | Editing next level (Prossimo)

### 6.6 Movimento tartaruga

- La tartaruga si muove da centro-cella a centro-cella lungo il path calcolato
- Velocità: il percorso completo dura tra 2 e 4 secondi (specs 6.3)
  - Formula: speed = pathLength / clamp(pathLength * 0.4, 2.0, 4.0) celle/secondo
- Interpolazione: DOTween, ease InOutSine per movimento fluido
- A ogni arrivo su una nuova cella:
  1. Controlla se la cella ha un collezionabile
  2. Se sì: trigger animazione raccolta, rimuovi collezionabile, aggiorna contatore
  3. Se baby turtle: aggiungi alla coda (segue con delay 0.3s e scala 0.6x)
- Rotazione sprite: la tartaruga guarda nella direzione del movimento
- Alla cella mare: trigger animazione splash + transizione a stato Result

---

## 7. Collezionabili e Punteggio

### 7.1 Oggetti sul percorso
| Oggetto | Posizione | Effetto |
|---|---|---|
| **Conchiglia** | Su alcune tessere del percorso | Raccolta automatica al passaggio della tartaruga. Contribuisce al punteggio stelle. |
| **Baby Turtle** | Su alcune tessere del percorso | Raccolta automatica. La baby turtle segue la tartaruga principale fino al mare. Contribuisce al punteggio stelle. |

> **Asset grafici collezionabili:** Sprite conchiglia custom pixel art 32x32, colore Coral Pink `#FF6F69`. Sprite baby turtle custom pixel art 32x32, colore Baby Pink `#FFB6C1`. Aspetto sulla tessera (32x32 centrato, bobbing), animazione raccolta e coda baby turtle in [art-direction.md - Collezionabili sulla tessera](art-direction.md#2b-celle-speciali-e-stati-visivi).

### 7.2 Sistema stelle (per livello)
| Stelle | Condizione |
|---|---|
| 1 | Completa il percorso (la tartaruga raggiunge il mare) |
| 2 | Completa il percorso + raccogli tutte le conchiglie |
| 3 | Completa il percorso + raccogli tutte le conchiglie + salva tutte le baby turtles |

> **Asset stelle:** Icona stella da Kenney Game Icons, colore Star Gold `#FFD700`. Vedi [art-direction.md - UI](art-direction.md#34-ui).

### 7.3 Implicazione di design
- Per ottenere 3 stelle, il giocatore potrebbe dover trovare un **percorso alternativo** che passi per tutte le caselle con bonus
- Non tutti i bonus sono raggiungibili con il percorso più breve → incentiva replay e sperimentazione
- I livelli devono essere progettati con almeno 2 percorsi validi: uno diretto e uno che raccoglie tutti i bonus

### 7.4 Contatore globale
- Schermata principale mostra: "Tartarughe salvate: X"
- Somma di tutte le baby turtles salvate in tutti i livelli
- Nessun rifugio/isola nell'MVP, solo il contatore numerico

---

## 8. Progressione e Struttura Livelli

### 8.1 Curva di apprendimento (15 livelli)

| Livello | Griglia | Tessere | Ostacoli | Inventario | Bonus | Concetto insegnato |
|---|---|---|---|---|---|---|
| 1 | 4x4 | Dritto, Curva | - | No | 1 conchiglia | Tap per ruotare |
| 2 | 4x4 | Dritto, Curva | - | No | 2 conchiglie | Percorsi curvi |
| 3 | 4x4 | Dritto, Curva | - | No | 1 conchiglia, 1 baby | Raccolta baby turtle |
| 4 | 4x4 | Dritto, Curva | - | No | 2 conchiglie, 1 baby | Percorso alternativo per 3 stelle |
| 5 | 5x5 | Dritto, Curva, T | - | No | 2 conchiglie | Introduzione tessera T |
| 6 | 5x5 | Dritto, Curva, T | - | No | 2 conchiglie, 1 baby | T con bivi |
| 7 | 5x5 | Dritto, Curva, T | Roccia | No | 2 conchiglie, 1 baby | Aggirare le rocce |
| 8 | 5x5 | Dritto, Curva, T | Roccia | No | 3 conchiglie, 1 baby | Rocce multiple |
| 9 | 5x5 | Dritto, Curva, T | Roccia, Buco | No | 2 conchiglie, 2 baby | Introduzione buchi |
| 10 | 5x5 | Dritto, Curva, T | Roccia, Buco | No | 3 conchiglie, 2 baby | Combinazione ostacoli |
| 11 | 6x6 | Dritto, Curva, T | Roccia | Si (2 tessere) | 3 conchiglie, 2 baby | Introduzione inventario |
| 12 | 6x6 | Dritto, Curva, T | Roccia, Buco | Si (2 tessere) | 3 conchiglie, 2 baby | Inventario + ostacoli |
| 13 | 6x6 | Dritto, Curva, T | Roccia, Buco | Si (3 tessere) | 4 conchiglie, 2 baby | Griglia complessa |
| 14 | 6x6 | Dritto, Curva, T | Roccia, Buco | Si (3 tessere) | 4 conchiglie, 3 baby | Sfida di ottimizzazione |
| 15 | 6x6 | Dritto, Curva, T | Roccia, Buco | Si (3 tessere) | 5 conchiglie, 3 baby | Livello finale, massima sfida |

### 8.2 Formato dati livello
```json
{
    "id": 1,
    "gridSize": { "width": 4, "height": 4 },
    "nest": { "x": 1, "y": 0 },
    "sea": { "x": 2, "y": 3 },
    "tiles": [
        { "type": "straight", "position": { "x": 1, "y": 1 }, "rotation": 0, "isFixed": false },
        { "type": "curve", "position": { "x": 2, "y": 1 }, "rotation": 90, "isFixed": false }
    ],
    "obstacles": [
        { "type": "rock", "position": { "x": 0, "y": 2 } }
    ],
    "inventory": [],
    "collectibles": [
        { "type": "shell", "position": { "x": 1, "y": 2 } },
        { "type": "baby_turtle", "position": { "x": 3, "y": 1 } }
    ],
    "starConditions": {
        "one": "complete",
        "two": "all_shells",
        "three": "all_shells_and_babies"
    }
}
```

---

## 9. Schermate e Navigazione

### 9.1 Flusso
```
Splash Screen → Menu Principale → Selezione Livello → Gameplay → Risultato Livello
                      ↓                                    ↓              ↓
                   Credits                            Menu Pausa    Prossimo livello / Menu
                                                      (overlay)
```

### 9.2 Schermate

> **Asset UI per tutte le schermate:** Bottoni, pannelli, icone da Kenney UI Pack (CC0). Colori UI dalla [palette master](art-direction.md#2-palette-colori-master): sfondo pannelli BG Off-White `#EBE5E1`, testi UI Dark `#2C3E50`, accenti Coral Pink `#FF6F69`. Pipeline di lavorazione UI in [art-direction.md - UI](art-direction.md#34-ui).

**Menu Principale**
- Titolo "Turtle Path" + tartaruga animata
- Bottone "Gioca"
- Contatore tartarughe salvate
- Bottone impostazioni (audio on/off)
- Bottone "Credits" (attribuzioni licenze, vedi [art-direction.md - Licenze](art-direction.md#7-licenze-e-crediti))
- Sfondo: Sky Blue `#87CEEB` + decorazioni spiaggia da [art-direction.md - Ambiente](art-direction.md#32-ambiente-spiaggia)

**Selezione Livello**
- Griglia 3x5 centrata di livelli
- Ogni livello mostra: numero, stelle ottenute (0-3, icona Star Gold `#FFD700`), stato (bloccato/sbloccato)
- I livelli si sbloccano in sequenza (completare il livello N sblocca N+1)
- Aspetto locked/unlocked/corrente: vedi [art-direction.md - Schermate](art-direction.md#45-schermate-dettagli-visivi)

**Gameplay**
- Griglia di gioco al centro, sfondo Sand Light `#FFEEAD`
- Inventario tessere in basso (se presente)
- Contatore conchiglie e baby turtles in alto
- Bottone pausa in alto a destra
- Nessun bottone "Avvia" (partenza automatica)

**Risultato Livello**
- Stelle ottenute (animazione sequenziale con scale+rimbalzo, Star Gold `#FFD700`)
- Baby turtles salvate (animazione delle tartarughine che entrano in acqua)
- Bottone "Prossimo livello" (Ocean Teal)
- Bottone "Rigioca" (Sand Warm)
- Bottone "Menu"
- Animazioni dettagliate: vedi [art-direction.md - Schermate](art-direction.md#45-schermate-dettagli-visivi)

**Credits**
- Accessibile dal menu principale
- Lista delle attribuzioni obbligatorie (CC-BY-SA per ZRPG Beach, crediti CC0)
- Scrollabile se necessario
- Bottone "Indietro" per tornare al menu principale
- Aspetto visivo: sfondo BG Off-White `#EBE5E1`, testi UI Dark `#2C3E50`

**Menu Pausa**
- Overlay scuro (nero 50%) sopra il gameplay
- Pannello centrato con 3 bottoni: Riprendi, Ricomincia, Menu
- Toggle audio on/off
- Aspetto visivo: vedi [art-direction.md - Schermate](art-direction.md#45-schermate-dettagli-visivi)

---

## 10. Audio e Atmosfera

### 10.1 Ambiente: Spiaggia Tropicale
- Palette colori, stile visivo, asset e pipeline grafica: vedi [art-direction.md](art-direction.md)
- Animazioni: onde sullo sfondo, granelli di sabbia, foglie che si muovono

### 10.2 Effetti sonori
| Azione | Suono |
|---|---|
| Tap su tessera (rotazione) | Click morbido / suono di conchiglia |
| Connessione tessera valida | Tono armonico crescente |
| Percorso completo | Melodia breve celebrativa |
| Tartaruga che cammina | Passi leggeri sulla sabbia |
| Raccolta conchiglia | Tintinnio |
| Raccolta baby turtle | Squeak dolce |
| Splash finale (mare) | Splash acqua + applauso soft |

### 10.3 Musica
- Loop ambient rilassante
- Strumenti: ukulele leggero, marimba, pad oceanici
- Volume basso, non invasiva
- Fade in/out tra schermate

---

## 11. Controlli

| Gesto | Azione |
|---|---|
| Tap su tessera | Ruota di 90 gradi in senso orario |
| Drag da inventario | Trascina tessera su casella vuota |
| Tap su tessera inventario piazzata | Ruota (come le altre) |
| Tap bottone pausa | Apre menu pausa |

### 11.1 Vincoli UX
- Giocabile con una mano (thumb zone)
- Nessun gesto complesso (no pinch, no long press, no swipe)
- Feedback visivo immediato su ogni tap (tessera ruota con animazione 0.15s via DOTween, vedi [art-direction.md - Setup Unity](art-direction.md#6-setup-unity))
- Feedback aptico leggero su tap (vibrazione soft, se supportata dal dispositivo)
- Contorno outline 2px su tutti gli elementi interattivi per distinguerli dallo sfondo, vedi [art-direction.md - Outline Shader](art-direction.md#42-outline-shader-unity)

---

## 12. Scope MVP - Riepilogo

### Incluso
- 15 livelli fatti a mano con difficoltà crescente
- 1 ambiente (Spiaggia tropicale)
- 3 tipi di tessera (Dritto, Curva, T)
- 2 ostacoli (Roccia, Buco)
- Sistema inventario tessere (livelli 11-15)
- Feedback percorso in tempo reale + partenza automatica tartaruga
- Punteggio 1-3 stelle basato su collezionabili
- Contatore globale tartarughe salvate
- Audio ambient + effetti sonori
- Menu principale, selezione livelli, schermata risultato

### Escluso (post-validazione)
- Monetizzazione (IAP, ads)
- Personalizzazione tartaruga (skin, cappellini)
- Isola rifugio / sistema decorazione
- Ambienti aggiuntivi (dune, foresta, paludi, barriera corallina, oceano notturno)
- Social (replay condivisibili, leaderboard)
- Puzzle giornaliero
- Eventi stagionali
- Ostacoli avanzati (granchi mobili, onde, predatori, maree)
- Generazione procedurale livelli
- Backend / cloud save
- Localizzazione multilingua

---

## 13. Target tecnici

| Parametro | Target |
|---|---|
| FPS | 60 fps stabili |
| Tempo di caricamento livello | < 1 secondo |
| Dimensione build | < 100 MB |
| Dispositivo minimo iOS | iPhone 8 / iOS 15+ |
| Dispositivo minimo Android | Android 8.0+, 2 GB RAM |
| Aspect ratio supportati | 16:9, 18:9, 19.5:9, 20:9 |
| Orientamento | Portrait only |

---

## 14. Milestones di sviluppo

Lo sviluppo è organizzato in 5 milestones progressive (M1→M5). Ogni milestone produce una **build PC giocabile** per validazione e ha criteri di accettazione espliciti.

> **Documento unico di riferimento:** [milestones.md](milestones.md) contiene il piano completo: cosa implementare, struttura progetto Unity, criteri di accettazione e istruzioni di test per ogni milestone.
>
> **Asset grafici per milestone:** [art-direction.md - Sezione 5](art-direction.md#5-mapping-asset--milestones).
>
> **IMPORTANTE:** Prima di iniziare qualsiasi milestone, completare la [checklist pre-implementazione](art-direction.md#8-checklist-pre-implementazione-per-milestone) in art-direction.md.
