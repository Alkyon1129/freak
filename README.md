# Pierrot Talk (Fair)

This repository builds a SMAPI mod that spawns an interactive **Pierrot** (The Freak Circus) at the Stardew Valley Fair (Fall 16) in Town (festival map).

## What it does
- Spawns Pierrot at tile **(22,77)** during the festival.
- Dialogue flow:
  1) Base dialogue with portrait `$0`
  2) Choice: Cheer / Boo
  3) Cheer -> dialogue with portrait `$1`
  4) Boo   -> dialogue with portrait `$2`
- Portrait sheet is a 2x2 64x64 grid.

## Build without installing tools
Use GitHub Actions:
1. Create a new repo and upload these files.
2. Go to **Actions** tab -> run **build** workflow (or push to `main`).
3. Download the artifact `PierrotTalk_Fair_Mod.zip`.
4. Unzip and copy the `PierrotTalk_Fair` folder into `Stardew Valley/Mods/`.

## Notes
If ModBuildConfig can't locate the game folder on CI, you'll need to add a step to download Stardew Valley assemblies; however, many projects compile on `windows-latest` with ModBuildConfig as-is.
