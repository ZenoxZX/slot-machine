# Slot Machine

A classic 3x3 slot machine built with Unity. Players spin the reels to match symbols on the middle row and collect coin rewards.

## Gameplay

- Press the spin button to start the reels spinning from left to right with a slight random delay between each column.
- Symbols on the middle row determine the outcome — matching all three wins coins.
- Winning triggers a coin particle fountain from the center of the machine with a perspective projectile effect.
- Coin reward amount scales with symbol rarity.

## Symbols

| Symbol | Reward Rank |
|--------|-------------|
| Jackpot | 1 (Highest) |
| Wild | 2 |
| Seven | 3 |
| Bonus | 4 |
| A | 5 (Lowest) |

Each symbol has a normal and blurred (motion) visual variant. Blurred versions are shown during spin, normal versions are revealed as reels stop.

## Probability System

Results are not random — they follow a predefined probability table enforced over 100-spin periods. Each result is distributed evenly across its own sub-period blocks for near-perfect distribution.

| Result | Rate |
|--------|------|
| A, Wild, Bonus | 13% |
| Wild, Wild, Seven | 13% |
| Jackpot, Jackpot, A | 13% |
| Wild, Bonus, A | 13% |
| Bonus, A, Jackpot | 13% |
| A, A, A | 9% |
| Bonus, Bonus, Bonus | 8% |
| Seven, Seven, Seven | 7% |
| Wild, Wild, Wild | 6% |
| Jackpot, Jackpot, Jackpot | 5% |

Spin progress persists across sessions — closing and reopening the game continues from where you left off.

## Stop Animations

| Type | Duration | Applied To |
|------|----------|------------|
| Fast | < 100 ms | Left and middle columns (always) |
| Normal | 1 sec | Right column (when left + middle match) |
| Slow | 2.25 sec | Right column (when all three will match) |

## Tech Stack

- **Engine:** Unity 6.3
- **Language:** C#
- **DI:** VContainer
- **Animation:** LitMotion
- **Async:** UniTask
- **Architecture:** SOLID principles, modular design

## How to Run

1. Clone the repository
   ```
   git clone https://github.com/ZenoxZX/slot-machine.git
   ```
2. Open the project with **Unity 6.3**
3. Press Play

## License

This project is for demonstration purposes.
