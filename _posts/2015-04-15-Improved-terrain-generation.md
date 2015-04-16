---
# vim: tw=80
layout: post
title: Improved terrain generation
blurb: |
    Thanks to the work of creatorfromhell, we now have a deterministic terrain
    generator, complete with biomes and decorations like trees and ores!
---

![](http://a.pomf.se/iusjvx.png)

Thanks to the work of creatorfromhell, we now have a deterministic terrain
generator, complete with biomes and decorations like trees and ores!

This new generator has a lot of really nifty stuff:

- Biomes! Desert, tundra, forest, and more
- Ore generation
- Tree generation (all varieties)
- Dungeons
- And so on

This new generator is super slick. Thanks a lot, creatorfromhell!

Once that was merged in, I also added support for better world persistence. Now
the seed, spawn point, etc are saved with the world. Coming up soon - the
ability to save player info like your inventory, HP, and coordinates.

As a side note, I opted to go for Anvil regions and a custom (NBT-based) format
for the world manifest. A similar custom format will be used for player
inventories. The vanilla formats just don't work for what I want out of
TrueCraft. This choice does reduce compatability with beta 1.7.3, but I believe
it is still in line with the spirit of TrueCraft.
