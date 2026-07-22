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

- [ ] `Room` data: size, enemy roster, spawn points
- [ ] Wave manager: spawn N enemies → wait for clear → next wave
- [ ] `DungeonDirector`: `Room → Room → Boss room` flow, branches or fixed
- [ ] `Boss` MonoBehaviour: phase transitions at HP thresholds
- [ ] Difficulty scaling per room index (HP×, DMG×, spawn count+)
- [ ] Drop table per enemy + per room

## Phase 4 — Loot & items

- [ ] `ItemData` ScriptableObject (base stats + modifier slots)
- [ ] `Rarity` enum: Common / Rare / Epic / Legendary (color + stat budget)
- [ ] Modifier generator: pool of affixes, roll N from pool
- [ ] Drop resolver: enemy/room → weighted item roll
- [ ] `Inventory` runtime store; equip at most N items per slot
- [ ] Recycle / sell flow between runs

## Phase 5 — Progression

- [ ] XP / level curve table (`LevelCurve` SO)
- [ ] Level-up pickups drop from enemies
- [ ] Permanent (meta) upgrades: +HP, +DMG, +Crit, etc. — purchased with `metaXp`
- [ ] Apply meta upgrades at run start
- [ ] Persist: `characterLevel`, `currentXP`, `metaXp`, owned upgrades
- [ ] Wire to existing `SaveData` / `CharacterData`

## Phase 6 — UI / HUD

- [ ] HUD: HP bar, mana bar, XP bar, wave counter
- [ ] Reward screen between rooms (3 picks from a generated pool)
- [ ] Death screen: summary → retry / abandon
- [ ] Pause / settings overlay
- [ ] Minimal UI Toolkit or uGUI — pick one and stay consistent

---

## Out of scope (for now)

- Movement code, dash, dodge — already scaffolded in `PlayerController` / `Attack`
- Audio, music, SFX
- Art, animation, VFX beyond placeholders
- Localization
- Build / CI / test harness (see `AGENTS.md`)

## Open questions for the user

- [ ] Single permanent character, or roster of playable classes?
- [ ] Are rooms hand-authored or procedurally generated?
