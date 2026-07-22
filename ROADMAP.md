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

## Phase 1 — Combat core

- [ ] `Damage` struct (amount, type, crit, source, target tags)
- [ ] `DamageType` enum (Physical / Magic / True)
- [ ] `IDamageable` interface → implemented by Player + Enemy
- [ ] `Health` MonoBehaviour: current, max, regen, shield, invuln frames
- [ ] `Projectile` (parabolic + straight variants); share `Damage` payload
- [ ] Hit detection: layer/tag filtering, ignore trigger-on-trigger
- [ ] Death routing: enemies raise `OnDeath` → loot/exp handlers
- [ ] Archero signature: **charge delay then auto-fire toward nearest enemy**

## Phase 2 — Enemies

- [ ] `EnemyData` ScriptableObject: hp, dmg, speed, attackInterval, projectilePrefab, sprite
- [ ] `EnemyController` base: idle / move / attack states
- [ ] Behavior archetypes (each is a small class, not config bloat):
  - [ ] Charger (walks at player, melee)
  - [ ] Shooter (strafes, fires projectile)
  - [ ] Summoner (spawns weaker add)
  - [ ] Turret (rooted, high-range burst)
- [ ] Targeting: nearest / furthest/aggro-leash distance
- [ ] Contact damage (player touch = damage, brief i-frames)

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
