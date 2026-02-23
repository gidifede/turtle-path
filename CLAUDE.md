# Turtle Path - Contesto Progetto

## Stato attuale
**Fase:** M2-Bis completata. Sprite custom, overlay connessione, pathfinding funzionante con grafica.
**Ultima sessione:** 2026-02-23

## Documentazione
Tutta la documentazione del progetto si trova in `docs/`. Leggere questi file IN ORDINE prima di procedere:

1. **[docs/specs.md](docs/specs.md)** - Specifiche tecniche MVP: regole di gioco, struttura dati, formato livelli, schermate, audio, controlli, target tecnici
2. **[docs/art-direction.md](docs/art-direction.md)** - Direzione artistica: stile visivo, palette colori 16 hex, asset stack con link e licenze, tecniche coerenza, setup Unity, struttura cartelle
3. **[docs/milestones.md](docs/milestones.md)** - Milestones dettagliate: 6 step (M1→M2→M2-Bis→M3→M4→M5), ciascuno con implementazione, struttura progetto Unity, criteri di accettazione, istruzioni test PC. Include M2-Bis (Visual Base) per colmare il gap grafico.

## Come riprendere il lavoro
1. Leggere i 3 file docs/ per avere il contesto completo
2. Identificare la milestone corrente (M1 se è la prima volta)
3. Leggere la milestone nel dettaglio in milestones.md
4. Verificare la checklist pre-implementazione in art-direction.md sezione 8
5. Procedere con l'implementazione

## Decisioni chiave già prese
- **Engine:** Unity + C# con URP
- **Stile:** Flat 2D cozy, palette pastello, 64x64 px per tile
- **Meccanica:** Tap per ruotare tessere su griglia quadrata, pathfinding BFS dal nido al mare
- **MVP:** 15 livelli, 1 ambiente, 3 tipi tessera, 2 ostacoli, zero monetizzazione
- **Asset:** Tutti gratuiti (Kenney CC0, OpenGameArt CC0). CraftPix Seabed non disponibile (a pagamento). Schwarnhild baby turtle non esistente.
- **Audio:** Kenney Interface/Impact Sounds (CC0), Freesound.org, Incompetech per musica
- **Unity:** Unity 6 LTS (6000.x) raccomandata
- **Test:** Ogni milestone produce build PC 450x800 portrait
- **Scena unica** con panel overlay (non scene separate)
- **Test automatici:** Edit Mode + Play Mode con assembly definitions
- **Tartaruga:** Turtle Sprite (Oiboo, OpenGameArt, CC0) — top-down 64x64, sprite statico per M2-Bis, walk cycle in M4
- **Tessere:** Pipes Tileset (Stealthix, itch.io, CC0) — 32x32, upscale 2x, palette remap
- **Collezionabili + nido + mare:** custom pixel art (nessun asset esterno adatto trovato)
- **Glow connesso:** child overlay semitrasparente (Ocean Teal alpha 50%) + bloom URP. NON usare SpriteRenderer.color (moltiplicativo, colori sporchi su sprite Sand)
- **Contorno 2px:** rimandato a M4 (outline shader)
- **Sfondo celle normali:** tint Sand Light `#FFEEAD` su quadrato bianco (nessuno sprite dedicato)
- **Port indicators:** nascosti in M2-Bis (flag ShowPortIndicators)
- **Camera background:** Sky Blue `#87CEEB` (ambiente spiaggia completo in M4)
- **Caricamento sprite:** SceneSetup (Editor) + serializzazione scena. Sprite in `Assets/Art/`, NON Resources/
- **Sprite colorati WYSIWYG:** sprite custom contengono il colore finale, codice usa `sr.color = Color.white` (nessun tint moltiplicativo)
- **Baby follower:** scala assoluta 0.72 (60% della tartaruga visiva, ~23px)
- **Tessere Pipes Tileset:** Dritto = colonna 1 (N-S), Curva = colonna 2 (N-E)
- **Bobbing/particelle:** rimandati a M4

## Diario sessioni
Le sessioni di lavoro sono documentate in `~/.claude/sessioni/`.
