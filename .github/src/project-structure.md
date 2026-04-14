# Project Structure

```
Assets/SlotMachine/
├── Content/                         # Textures, Materials, Sprite Atlases
│   ├── Atlas/                       # SA_Reel, SA_Slot sprite atlases
│   ├── Materials/                   # UI materials (CylindricalProjection)
│   └── Textures/                    # Symbol sprites (normal + blur), slot frame, coins
│
├── Data/                            # ScriptableObject assets
│   ├── SpinTimingData               # Spin speed, ramp up, snap, durations
│   ├── SymbolViewData               # Symbol sprite mappings + blur fade settings
│   └── CoinVFXData                  # Coin particle counts, motion, animation
│
├── Prefab/                          # UI prefabs
│   ├── P_ReelView                   # Base reel (RectMask2D + 5 SymbolViews)
│   ├── PV_ReelView_Left             # Left reel variant (reelIndex, reelSide)
│   ├── PV_ReelView_Center           # Center reel variant
│   ├── PV_ReelView_Right            # Right reel variant
│   ├── P_SymbolView                 # Symbol cell (normal + blur Images)
│   └── P_CoinView                   # Coin particle (Image + CoinView)
│
├── Scenes/
│   └── SC_Game                      # Main game scene
│
├── Shaders/
│   └── UI_CylindricalProjection     # Custom URP UI shader for 3D drum effect
│
├── Scripts/
│   ├── DI/Installers/               # Top-level DI installers
│   │   ├── GameSceneInstaller       # Scene-scoped registrations
│   │   └── ProjectInstaller         # Project-scoped registrations
│   │
│   └── Slot/
│       ├── Core/                    # Enums and value types
│       │   ├── Symbol               # A, Bonus, Seven, Wild, Jackpot
│       │   ├── StopMode             # Fast, Normal, Slow
│       │   ├── SpinResult           # 10 predefined outcomes + None
│       │   ├── SpinResultEntry      # Result-count pair
│       │   └── ResolvedSpinResult   # Concrete Left/Middle/Right symbols
│       │
│       ├── Data/                    # Model layer
│       │   ├── SpinResultTable      # Static probability table (10 entries, 100 pool)
│       │   ├── SpinResultProvider   # Deterministic pool-based result generator
│       │   ├── SpinResultPersistence # Binary save/load (seed + index)
│       │   ├── SpinTimingData       # Spin timing configuration (SO)
│       │   ├── SymbolViewData       # Symbol sprite mappings (SO)
│       │   ├── CoinVFXData          # Coin VFX configuration (SO)
│       │   ├── ReelReference        # Scene references to 3 ReelViews
│       │   └── CoinVFXReference     # Scene references for coin spawn/pool
│       │
│       ├── View/                    # View layer
│       │   ├── SlotView             # ISlotView — orchestrates 3 reels, staggered start
│       │   ├── ReelView             # Single reel — 2-phase spin (blur + snap tween)
│       │   ├── SymbolView           # Single symbol cell — normal/blur fade
│       │   ├── SpinButtonView       # Spin button — GamePipe publish/subscribe
│       │   ├── CoinView             # Single coin — trajectory, scale, rotation, animation
│       │   ├── CoinVFXController    # Coin burst manager — pool, spawn, lifecycle
│       │   └── CylindricalMeshEffect # BaseMeshEffect for shader UV data
│       │
│       ├── Controller/              # Controller layer
│       │   └── SpinController       # Spin orchestration, stop mode, persistence
│       │
│       ├── Messages/                # Event bus messages
│       │   ├── SpinRequestedMessage # Button click signal
│       │   ├── ReelStoppedMessage   # Per-reel completion (reelIndex)
│       │   └── SpinCompletedMessage # All reels done (result + isWin)
│       │
│       ├── Utils/                   # Extension methods
│       │   └── SpinResultExtensions # SpinResult.Resolve() → ResolvedSpinResult
│       │
│       └── Installers/
│           └── SlotInstaller        # Slot-specific DI registrations
│
├── Resources/
│   └── DI/ProjectLifetimeScope     # Runtime-loaded DI bootstrap prefab
│
├── Settings/                        # Unity project settings
│
└── Systems/                         # Reusable framework modules
    ├── ConfigManagement/            # Editor window for SO configs (IVisibleConfig)
    ├── Core/                        # GlobalEnvironmentVariables, NonDrawingGraphic
    ├── DI/                          # DIBootStrapper, MonoInstaller, Scopes
    ├── MessagePipe/                 # GenericEventBus, GamePipe, ProjectPipe, ObjectPool
    └── Messages/                    # IMessage marker interface
```

## Architecture

The project follows MVC separation with event-driven communication. Each layer has a clear responsibility:

| Layer | Responsibility |
|-------|---------------|
| **Core** | Value types, enums, readonly structs — no dependencies |
| **Data** | ScriptableObject configs, providers, persistence — pure data |
| **View** | MonoBehaviour UI components — animation, visuals, user interaction |
| **Controller** | Business logic orchestration — connects data and view via messages |
| **Messages** | Lightweight structs (`IMessage`) for decoupled communication |
| **Installers** | VContainer DI registrations |

Communication between layers is handled via **MessagePipe** — a custom event bus with typed messages. Dependency injection is managed by **VContainer**.
