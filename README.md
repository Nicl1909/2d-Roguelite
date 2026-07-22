# 2D Roguelite

> Unity 2D Archero-style roguelite. Phases 0–4 implemented (state machine, combat core, enemies, dungeon/waves/boss, loot & items); 5–6 still open.

## Status

| Phase | Topic | Status |
|-------|-------|-----|
| 0 | State machine & run lifecycle | done |
| 1 | Combat core (damage / projectiles) | done |
| 2 | Enemies (AI archetypes) | done |
| 3 | Dungeon / waves / boss | done |
| 4 | Loot & items | done |
| 5 | Progression / meta upgrades | stub only |
| 6 | UI / HUD | not started |

Full checklist: see [`ROADMAP.md`](ROADMAP.md).
Agent guidance: see [`AGENTS.md`](AGENTS.md).

## Stack

- Unity **6000.5.4f1**
- URP 2D (`com.unity.render-pipelines.universal`)
- Input System **1.19.0** (new Input System — `Input.GetKeyDown` will not fire)
- JetBrains Rider (recommended) via `com.unity.ide.rider`
- C# 9.0
- Save/load: `System.Text.Json` → `Application.persistentDataPath/characterData.json`

## Opening the project

1. Unity Hub → Editor **6000.5.4f1** → open this folder.
2. Or open `2d-Roguelite.slnx` in Rider (auto-detected Unity project).
3. To play: open `Assets/Scenes/Boot.unity` → ▶ Play.
   Or from VS Code use the **"Attach to Unity"** debug config.

There is no CLI build or test runner — verification is in the Editor.

## Scope of this repository

This codebase is **game logic only**. It does **not** include:

- Movement code (input, dash, knockback, AI navigation)
- Scene wiring in the Editor (GameObject composition, prefab setup, UI canvas wiring)
- Art, animation, VFX

Movement / scene wiring / art are owned by other contributors. The scripts here describe **what** the system does, not the per-frame input or render side.

### What *is* here

```
Assets/
  Scripts/
    Core/         GameManager, GameState, GameEvents, RunContext, RunRng,
                  GameBootManager, SceneLoader
    Character/    CharacterData (POCO stats)
    Combat/       Damage, DamageType, Health, Projectile
    Enemies/      EnemyController, EnemyFactory, EnemyContactDamage,
                  IEnemyBehavior, EnemyArchetype, TargetingService,
                  ChargerBehavior, ShooterBehavior, SummonerBehavior, TurretBehavior
    Dungeon/      Room, RoomSet, BossData, DifficultyCurve, WaveRunner,
                  BossController, RoomDirector, DungeonEvents
    Items/        Rarity, WeaponType, ArmorSlot, ItemEnums, ItemInstance,
                  ModifierData, ModifierGenerator, DropTable
    Inventory/    LootService, Inventory, LootInventoryBridge
    Progression/  XPHandler                  (forwards XP → GameManager)
    SaveSystem/   SaveManager                (System.Text.Json inside)
    Player/       Player, PlayerController, Weapon/Attack   ← OUT OF SCOPE
  ScriptableObjects/
    EnemyData.cs
    Items/        WeaponData.cs, ArmorData.cs, Weapon.asset
    DifficultyCurve.cs, RoomSet.cs (created via CreateAssetMenu)
  Scenes/         Boot, MainMenu, Dungeon, SampleScene
  Settings/       URP renderer, input actions, volume profile
```

## Architecture

```
            ┌─────────────────────────────────────┐
            │           GameBootManager           │   (Boot.unity)
            └────────────────┬────────────────────┘
                             │ spawns if missing
                             ▼
            ┌─────────────────────────────────────┐
            │            GameManager              │   (singleton, DontDestroyOnLoad)
            │   state · run · persist · events    │
            └─────┬───────────────────────┬────┬───┘
                  │ events                 │    │   reads/writes
                  ▼                       ▼    ▼
            ┌────────────┐         ┌──────────┐ ┌────────────┐
            │SceneLoader │         │RoomDir.  │ │ SaveManager│ ──▶ .json
            └─────┬──────┘         │ WaveRunner, BossCtl
                  │                └────┬─────┘
                  │                     │ spawns via EnemyFactory
                  ▼                     ▼
              MainMenu / Dungeon    EnemyController / Projectile
```

State transitions:

```
Boot ─▶ MainMenu ─▶ Dungeon ─▶ Reward ─▶ Dungeon (next room)
                 ▲                          │
                 └──────── Death ◀──────────┘
```

Each `→ Reward`, `→ Death`, `→ MainMenu` triggers a save flush.

## Defensive design

Every script handles missing services / null data without crashing:

- `SaveManager.*` returns `bool` and swallows `UnauthorizedAccessException` / `IOException`
- `EnemyController.Initialize` returns `bool`, refuses null `EnemyData`
- `XPHandler.Instance`, `LootService.Instance`, `GameManager.Instance` are always null-checked
- `Object.Destroy(gameObject)` is wrapped in try/catch to survive double-destroy during teardown

Runs continue even if a single system fails — errors are logged via `Debug.LogError`.

## How to extend

1. Add an entry to `GameState` if a new top-level state is needed.
2. Drive state via `GameManager.Instance.SetState(GameState.X)` — never poke the field.
3. Subscribe to `GameEvents.OnStateChanged` if you need to react (e.g. to load a scene).
4. Spawn enemies with `EnemyFactory.Spawn(data, position, runRng)` — pass the run's `RunRng` so spawns stay deterministic.
5. Trigger a room with `RoomDirector.StartRoomForCurrentRun()` via `GameManager.SetState(GameState.Dungeon)`.
6. Persist data through `SaveManager.SaveCharacterData(...)`; don't write to `Application.persistentDataPath` directly.
7. Generate loot with `LootService.RollInstance(dropTable, runRng)`; subscribe to `LootService.OnItemDropped` to handle new `ItemInstance` rolls.
8. Equip / recycle via `Inventory.Instance.Equip(item)` / `Inventory.Instance.Recycle(item)`.

## Don'ts

- **Don't** edit `Assembly-CSharp.csproj` — regenerated by Unity on every script edit.
- **Don't** add `using NUnit.Framework;` to runtime code.
- **Don't** use `Input.GetKeyDown` / `Input.GetAxisRaw` — the new Input System is enabled.
- **Don't** hand-write JSON with `JsonUtility`; this repo uses `System.Text.Json`.
- **Don't** touch `Assets/Scripts/Player/` — owned by another contributor.
- **Don't** auto-commit. There is no CI; commits happen on explicit request.
