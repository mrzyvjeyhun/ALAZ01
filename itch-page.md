# Guardians of the Flame

*(working title — rename to whatever you like)*

> A sacred fire burns in the dark, and the dark wants it gone. Keep the flame alive: swing your archer companion to the right side of the field, and when the demon comes for the embers, parry it back into the night.

---

## What is this game?

**Guardians of the Flame** is a top-down survival / defense game. At the center of the map burns a single **sacred fire** — that fire is your life. Waves of creatures pour out of the doors around the arena and march straight toward it. Every one that reaches the flame chips away at it. Let the fire go out and it's over.

You don't win by attacking head-on. You win by **managing two things at once**:

1. **A companion archer** that you teleport around the fire to shoot down the common horde.
2. **A perfectly-timed parry** that you save for the one enemy the arrows can't stop — the **Incubus**.
3. **A gate-sealing potion** you earn through clean parries and spend to slam a door shut when the pressure spikes.

It's part reflex test, part plate-spinning. The horde keeps you moving the archer; the Incubus makes you hold your nerve.

---

## How to Play (the core loop)

1. **Watch the doors.** Enemies spawn from doorways around the edge of the arena and walk toward the fire in the center.
2. **Aim your companion.** Your archer guards **one direction at a time**. Use the arrow keys to teleport it to the side the enemies are coming from — it automatically shoots the nearest target in that direction.
3. **Keep your parry charged.** Stand in the **fire zone** to fill your *Sword Fire* meter. A full meter is what lets you parry.
4. **Parry the Incubus.** When the Incubus appears (announced by a flickering door light), the arrows won't save you. Get in position, line up the timing bar, and press **Parry** at the right instant to destroy it.
5. **Survive.** Protect the flame for as long as you can.

---

## Controls

| Action | Keyboard | Gamepad |
|---|---|---|
| Move your character | **W A S D** | Left stick / D-pad |
| Move companion **Up** | **↑ Up Arrow** | — |
| Move companion **Down** | **↓ Down Arrow** | — |
| Move companion **Right** | **→ Right Arrow** | — |
| Move companion **Left** | **← Left Arrow** | — |
| **Parry** | **Space** or **Enter** | West button (X / □) |
| **Seal a gate** (needs a full potion) | **Left Mouse Button** on a gate | — |

> Your character starts each round positioned near the flame, and the companion begins guarding the **left** side.

---

## Mechanics in Detail

### The Sacred Fire (your health)
The fire in the middle is the only thing you're protecting. It has a limited amount of life. Common enemies that touch it deal light damage; the Incubus hits **far harder**. A heads-up display shows the flame's current strength — when it runs out, the fire dies and the run ends.

### The Companion Archer
Your archer is an auto-firing turret that you *position* rather than aim:

- It covers **one of four directions** (up / down / left / right) at a time. Arrow keys teleport it to that side in a flash of light.
- It automatically locks onto the **nearest enemy** in its active zone and looses arrows at it.
- There's a short **aim delay** when it first spots a target and a brief **reload** between shots — so it isn't instant. If two doors get busy at once, you'll have to choose which side to cover and rotate quickly.

This is the heart of moment-to-moment play: reading which doors are active and swinging the archer to meet the threat before it reaches the fire.

### The Incubus & the Parry
The **Incubus** is the special enemy — slower, deadlier, and immune to your usual defense. Only one stalks the field at a time, and its arrival is telegraphed by a **blinking door light** before it steps out. When it advances on the flame, arrows won't cut it. You have to parry.

How the parry works:

- **Charge first.** You can only parry when your **Sword Fire** meter is full. Refill it by standing in the **fire zone**. Each parry spends the whole charge, so you re-charge between attempts.
- **Get in the doorway.** The parry window only opens while you're standing in the Incubus's lane (its doorway zone).
- **Read the timing bar.** A parry bar appears with a moving **marker** tracking the Incubus's advance and a highlighted **target zone** set by where *you're* standing. Press **Parry** when the marker meets the zone:
  - **Perfect parry** — clean strike. The Incubus is destroyed, the flame flares **green**, and you bank a parry toward your tally.
  - **Glancing parry (early)** — you still destroy the Incubus, but you struck too soon: the flame flares **yellow** and takes **partial damage** anyway.
  - **Miss (late)** — the flame flares **red**, the window slams shut, and the Incubus keeps coming. You've lost the chance to stop it this time.
- On a successful parry your character swings the fire-sword in the direction you're facing.

So a parry is a small puzzle every time: charge up, plant yourself in the right lane to set the target zone, and hit the button on the beat.

### The Potion (gate-sealing ability)
Every **perfect parry** adds a charge to your **potion gauge** on the HUD. Glancing (yellow) parries don't count — only clean ones fill it, so the potion is a direct reward for nailing the timing.

Once the gauge is **full**, you can spend it: **left-click a gate** to seal that door shut for a few seconds. While it's sealed, nothing spawns from it — the gateway bursts into flame and any enemy trying to come through is shut out (it'll even cut off an Incubus that's just starting to appear). Using it **empties the potion**, so you have to earn another full gauge with more perfect parries before you can seal a gate again.

Use it to buy yourself breathing room when two doors light up at once, or to deny the Incubus its entrance entirely.

---

## Tips & Strategy

- **Don't camp one side.** The archer only guards one direction. Keep your eyes on every door and pre-swing the archer toward whichever is heating up.
- **Stay topped up.** Drift back into the fire zone whenever it's quiet so your Sword Fire is ready the instant the Incubus shows.
- **Respect the door light.** A flickering doorway means the Incubus is about to spawn there. Stop fussing with the horde and get into that lane to set up your parry.
- **Early beats late.** A glancing (yellow) parry still kills the Incubus and singes the flame; a late parry lets it through completely. If you're unsure, lean early.
- **But only perfect parries build the potion.** Yellow parries keep you alive without filling the gauge — go for clean hits when you can afford to, since the potion is what powers your gate seal.
- **Position = timing.** Where you stand sets the parry's target zone. Settle into your spot *before* you press, don't scramble at the last second.
- **Save the seal for the spike.** A full potion is precious — left-click a gate to shut it when a door floods or when an Incubus is mid-spawn, then go re-earn the charge.

---

## Game Over

When the sacred fire runs out of life, it goes dark and a **Restart** button appears. Hit it to try again and see how long you can keep the flame burning.

---

*Made in Unity. Built around a single idea: one fire, one archer, and the nerve to parry the dark.*
