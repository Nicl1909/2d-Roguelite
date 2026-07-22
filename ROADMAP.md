# Roadmap

Game-logic-only build order for the Archero-style roguelite. Movement is out of scope; this is systems work. Phases are ordered so each can be played in isolation.

Legend: `[ ]` not started · `[x]` done · `[/]` in progress

---

## Phase 0 — State machine & run lifecycle

- [x] `GameState` enum (Boot / MainMenu / Dungeon / Reward / Death)
- [x] `GameManager` owns and broadcasts current state (event-based, not polling)
- [x] Run start: seed enemy pool + loot table for the run  
- [x] Run end: death → reward screen → back to MainMenu
- [x] Save/load hooks: persist `currentRunSeed`, `characterLevel`, `metaXp` after each phase transition
- [x] `SceneLoader` wires state enum → Unity scene load
- [x] `RunRng` seeded RNG so runs are reproducible
- [x] Save logic consolidated into `SaveManager`; `SaveData.cs` removed

## Phase 1 — Combat core

- [x] `Damage` struct (amount, type, crit, source, target tags)
- [x] `DamageType` enum (Physical / Magic / True)
- [x] `IDamageable` interface → implemented by Player + Enemy
- [x] `Health` MonoBehaviour: current, max, invuln frames, link to controller
- [x] `Projectile` MonoBehaviour: straight launch + lifetime, contacts `IDamageable` on overlap
- [x] Contact damage via `EnemyContactDamage` + `Health.OnTriggerEnter2D`
- [ ] Hit filtering polish (layer mask, projectile immune-on-self) — pending scene work
- [ ] Archero signature: **charge delay then auto-fire toward nearest enemy** — pending PlayerController rewrite (out of scope)
- [x] Death routing: enemies raise `OnKilled` → XP/Loot services

## Phase 2 — Enemies

- [x] `EnemyData` ScriptableObject: hp, dmg, speed, attackInterval, projectilePrefab, aggroRadius
- [x] `EnemyController` MonoBehaviour: state machine + behaviour delegate, ReadOnly intent output
- [x] Behavior archetypes (small strategy classes, not config bloat):
  - [x] Charger (walks at player, melee)
  - [x] Shooter (strafes, fires projectile)
  - [x] Summoner (spawns weaker add, seedable)
  - [x] Turret (rooted, high-range burst)
- [x] Targeting: nearest / furthest / lowestHealth via `TargetingService`
- [x] Aggro-leash distance per behavior
- [x] Contact damage (`EnemyContactDamage`)
- [x] `EnemyFactory.Spawn(data, pos, rng)` — deterministic spawn

## Phase 3 — Dungeon / waves / boss

- [x] `Room` data structure (id, kind, waves, boss, spawn points)
- [x] `Room` + `Wave` + `SpawnPoint` + `BossData` value objects in `Assets/Scripts/Dungeon/`
- [x] `RoomSet` ScriptableObject holding a list of rooms + boss catalog
- [x] `DifficultyCurve` ScriptableObject (hp / dmg / spawn multipliers by room index)
- [x] `WaveRunner` MonoBehaviour: spawn N enemies → wait for clear → next wave
- [x] `RoomDirector` orchestrator: `Room → Room → Boss → Reward` flow
- [x] `BossController` MonoBehaviour with HP-threshold phase transitions
- [x] `DungeonEvents` static bus (`OnRoomStarted`, `OnRoomCleared`, `OnBossKilled`, `OnBossPhaseChanged`)
- [x] Per-room deterministic spawn via run's `RunRng`
- [x] Drop table per enemy / per room — wired into Phase 4 (`EnemyData.dropTable`)

## Phase 4 — Loot & items

- [x] `Rarity` enum (Common / Uncommon / Rare / Epic / Legendary / Mythic) + `RarityUtility` (color, stat budget, sell value)
- [x] `WeaponType` and `ArmorSlot` enums lifted out of `WeaponData` / `ArmorData` (fixes nested-enum collision with Combat's `DamageType`)
- [x] `ModifierData` SO (stat / min / max / weight / slot)
- [x] `ModifierGenerator.Roll(pool, budget, rng)` weighted affix roller
- [x] `DropTable` SO with weighted `(item, rarity)` entries + nothing-weight
- [x] `ItemInstance` runtime record (kind / rarity / modifiers / source reference)
- [x] `LootService.TryDrop(...)` consumes `EnemyData.dropTable`, deterministic via run's RNG
- [x] `Inventory` runtime store + `EquipSlots` per-slot equip model
- [x] `LootInventoryBridge` subscribes to `OnItemDropped` and pushes to inventory
- [x] Recycle / sell flow: `Inventory.Recycle(item)` → `GameManager.AwardMetaXp(value)`

## Phase 5 — Progression

- [x] `LevelCurve` SO (`baseXpForLevel`, `xpGrowth`)
- [x] `XPHandler.Award` splits XP → `characterExperience` (level curve) + half meta-XP
- [x] `GameManager.AwardExperience(...)` and `SpendMetaXp(...)` with `dirty` flag → persist on Reward/Death/MainMenu
- [x] Permanent meta upgrades: `UpgradeData` SO + `MetaUpgradeService` (purchase + cost curve)
- [x] Apply meta upgrades at run start via `RunModifierApplier.ResetForRun()` called from `StartNewRun`/`ContinueOrStartNewRun`
- [x] Persist: `CharacterData.characterName/Level/Health/Mana/Experience/metaXp`, `upgrades[]`
- [x] Recycle flow: `Inventory.Recycle` → `GameManager.AwardMetaXp` (uses `RunModifierApplier.GetStat` lookup for equipped items)

## Phase 6 — UI / HUD

- [x] `HudBootstrap` runtime canvas (uGUI), survives scenes via `RuntimeInitializeOnLoadMethod`
- [x] HUD: HP bar, mana bar, XP bar, level readout, room counter, meta-XP readout
- [x] Death screen: summary + Retry / Quit buttons
- [x] Main menu: title + START RUN button
- [x] Reward screen: bonus text + Continue button
- [x] All UI built in code, no Editor wiring required

---

## Out of scope (for now)

- Hand-authored art, animations, polished VFX
- Audio, music, SFX
- Localization
- Build / CI / test harness (see `AGENTS.md`)
- Editor-side scene wiring (Boot/Dungeon scenes are intentionally empty; the runtime scene bootstrap builds the world)

## Cross-cutting — robustness (added mid-Phase-3)

- [x] `SaveManager` try/catch around all IO (`File.WriteAllText`, `File.Delete`, `File.Exists`); ignores `UnauthorizedAccessException` / `IOException` instead of crashing
- [x] `SaveManager.SaveCharacterData(...)/HasSave()/DeleteSave()` now return `bool` so callers can react
- [x] `SaveManager.LoadCharacterData` swallows deserialize errors, returns `null`
- [x] `EnemyController.Initialize` returns `bool`, refuses null `EnemyData`
- [x] All service calls (`XPHandler`, `LootService`, `GameManager.Instance`) null-checked
- [x] `Tag comparison` wrapped in try/catch (Unity throws on undeclared tags)
- [x] `TargetingService.FindTarget` catches `UnityException` if "Player" tag doesn't exist
- [x] `SpawnOne` falls back to seeded random offsets if `SpawnPoint`s are absent
- [x] `TagManager.asset` seeded with `Player` and `Enemy` tags so `RuntimeInitializeOnLoadMethod` paths don't crash on missing tags

## Player / Scenes — runtime-built (added in final pass)

- [x] `PlayerController` rewritten: dash/move using new InputSystem (`Move`/`Sprint`), Archero-style charge-then-fire auto-aim via `Attack`
- [x] `Attack` rewrite: hold input → target nearest enemy → burst-shot respecting `WeaponData.attackTime/cooldown/damage/type/range`
- [x] `CameraFollow` smooth-follows player in Dungeon
- [x] `DungeonSceneBootstrap`: builds Camera + Tilemap grid + Services (XPHandler, LootService, Inventory, MetaUpgradeService, RoomDirector, HudBootstrap) + Player with default WeaponData + 3 charger enemies around the spawn
- [x] `RuntimeSpriteFactory`: zero-asset `Sprite` and `Circle` sprites built from `Texture2D.whiteTexture` so no art needed to test
- [x] `EditorBuildSettings.asset` extended to include `Boot`, `MainMenu`, `Dungeon`, `SampleScene` so `SceneManager.LoadScene("...")` works at runtime

## Open questions for the user

- [ ] Single permanent character, or roster of playable classes?
- [ ] Are rooms hand-authored or procedurally generated?
