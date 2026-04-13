# Slot Machine

A 3x3 slot machine built with Unity 6.3. Players spin the reels to match symbols on the middle row and collect coin rewards.

## Gameplay

- Press the spin button to start the reels spinning from left to right with a slight random delay between each column.
- Symbols on the middle row determine the outcome — matching all three wins coins.
- Symbols switch to blurred variants during motion and fade back to normal as reels decelerate.

## Symbols

| Symbol | Reward Rank |
|--------|-------------|
| Jackpot | 1 (Highest) |
| Wild | 2 |
| Seven | 3 |
| Bonus | 4 |
| A | 5 (Lowest) |

Each symbol has a normal and blurred visual variant. A custom cylindrical projection shader warps the flat UI sprites to simulate a 3D drum surface.

## Stop Animations

Three distinct stop behaviors create tension based on the incoming result:

| Type | Duration | Easing | Applied To |
|------|----------|--------|------------|
| Fast | < 100 ms | OutQuad | Left and middle columns (always) |
| Normal | 1 sec | OutCubic | Right column (when left + middle match but right differs) |
| Slow | 2.25 sec | OutQuart | Right column (when all three will match — maximum suspense) |

## Probability System

Results are **not random**. They follow a predefined probability table enforced over 100-spin pool cycles.

| Result | % of Spin |
|--------|-----------|
| A, Wild, Bonus | 13 |
| Wild, Wild, Seven | 13 |
| Jackpot, Jackpot, A | 13 |
| Wild, Bonus, A | 13 |
| Bonus, A, Jackpot | 13 |
| A, A, A | 9 |
| Bonus, Bonus, Bonus | 8 |
| Seven, Seven, Seven | 7 |
| Wild, Wild, Wild | 6 |
| Jackpot, Jackpot, Jackpot | 5 |

### Periodic Block Distribution

Each result type is distributed across its own sub-period blocks within the 100-spin pool, preventing clustering. For example, Jackpot x3 at 5% creates 5 blocks of 20 spins — exactly one Jackpot x3 lands in each block. The position within each block is randomized using an urgency-based greedy algorithm: candidates with the fewest remaining slots in their block window get priority, with ties broken randomly.

### Session Persistence

Spin state (seed + current pool index) is saved to a binary file on every spin. Closing and reopening the game resumes from the exact same position in the pool, preserving the distribution guarantee across sessions.

## Architecture

MVC pattern with VContainer for dependency injection and a custom event bus (GamePipe) for decoupled messaging.

```
SpinButtonView (click)
  -> GamePipe: SpinRequestedMessage
    -> SpinController
      -> SpinResultProvider.GetNext()    -- deterministic pool draw
      -> Resolve() -> ResolvedSpinResult -- concrete Left/Middle/Right symbols
      -> DetermineStopMode()             -- Fast / Normal / Slow
      -> Persistence.Save()              -- binary file write
      -> ISlotView.StartSpin()           -- kick reel animations
        -> ReelView x3                   -- per-reel scroll + deceleration
        -> GamePipe: ReelStoppedMessage  -- per reel
    -> SpinController (3 reels stopped)
      -> GamePipe: SpinCompletedMessage
        -> SpinButtonView (re-enable)
```

### Folder Structure

```
Assets/SlotMachine/
├── Content/             -- Textures, Materials, Sprite Atlases
├── Prefab/              -- Reel and Symbol prefabs (base + variants)
├── Scripts/
│   ├── DI/              -- VContainer installers
│   └── Slot/
│       ├── Core/        -- Enums, structs (Symbol, SpinResult, StopMode)
│       ├── Data/        -- Model layer (provider, persistence, config)
│       ├── View/        -- View layer (reel visuals, animations, shader effects)
│       ├── Controller/  -- SpinController orchestration
│       ├── Messages/    -- Event bus message types
│       └── Utils/       -- Extension methods
├── Resources/           -- DI bootstrap prefab (runtime loaded)
├── Scenes/              -- SC_Game.unity
└── Systems/             -- Reusable framework modules (DI, MessagePipe, Core)
```

## Tests

34 unit tests covering the probability engine:

- **Distribution** — Every 100-spin pool contains exactly the right count for each result
- **Periodicity** — Each result appears exactly once per its block period
- **Determinism** — Same seed produces the same sequence
- **Resume** — Mid-pool resume via seed+index reconstructs the identical remaining sequence
- **Multi-seed** — Distribution holds across multiple seed values

## Tech Stack

| | |
|---|---|
| Engine | Unity 6.3 |
| DI | VContainer |
| Tween | LitMotion |
| Async | UniTask |
| Pattern | MVC |

## Planned

- [ ] Coin particle VFX — projectile motion from slot center with perspective depth, coin count scaled by symbol rarity (Jackpot > Wild > Seven > Bonus > A)

## How to Run

1. Clone the repository
   ```
   git clone https://github.com/ZenoxZX/slot-machine.git
   ```
2. Open the project with **Unity 6.3**
3. Press Play

## License

This project is for demonstration purposes.
