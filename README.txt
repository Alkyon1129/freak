Pierrot Talk (Fair) - Source Package

What this mod does
- On Fall 16 (Stardew Valley Fair) during the festival, spawns an interactive character named "Pierrot"
  at tile (22, 77) in Town (the festival temporary map).
- Talking to Pierrot shows:
    1) Base dialogue (portrait $0)
    2) Choices: Cheer / Boo
    3) Cheer => dialogue + portrait $1
    4) Boo   => dialogue + portrait $2
- Portrait sheet is a 2x2 (64x64 each). Only $0/$1/$2 are used.

Assets
- assets/pierrot_sprite.png      : 32x64 frames, 48 frames total (0..47), played in order in a loop
- assets/pierrot_portrait.png    : 2x2 portrait sheet (64x64 each). $0=$top-left, $1=$top-right, $2=$bottom-left

i18n
- i18n/default.json : English
- i18n/ko.json      : Korean

Build
- Use the official SMAPI C# mod template, then replace/add files from this package.
