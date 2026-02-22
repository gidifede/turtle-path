# Turtle Path - Contesto Progetto

## Stato attuale
**Fase:** Pre-implementazione completata. Pronto per M1 (Prototype).
**Ultima sessione:** 2026-02-22

## Documentazione
Tutta la documentazione del progetto si trova in `docs/`. Leggere questi file IN ORDINE prima di procedere:

1. **[docs/specs.md](docs/specs.md)** - Specifiche tecniche MVP: regole di gioco, struttura dati, formato livelli, schermate, audio, controlli, target tecnici
2. **[docs/art-direction.md](docs/art-direction.md)** - Direzione artistica: stile visivo, palette colori 16 hex, asset stack con link e licenze, tecniche coerenza, setup Unity, struttura cartelle
3. **[docs/milestones.md](docs/milestones.md)** - Milestones dettagliate: 5 step (M1→M5), ciascuno con implementazione, struttura progetto Unity, criteri di accettazione, istruzioni test PC

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
- **Asset:** Tutti gratuiti (Kenney CC0, OpenGameArt CC0/CC-BY-SA, CraftPix royalty-free)
- **Audio:** Kenney Interface/Impact Sounds (CC0), Freesound.org, Incompetech per musica
- **Unity:** Unity 6 LTS (6000.x) raccomandata
- **Test:** Ogni milestone produce build PC 450x800 portrait

## Diario sessioni
Le sessioni di lavoro sono documentate in `~/.claude/sessioni/`.
