---
# vim: tw=80
layout: post
title: Multiplayer entity management
blurb: |
    Multiplayer entity management is working now, which means that you can
    see and interact with your friends on TrueCraft servers now.
---

![](http://a.pomf.se/ouhdfw.png)

Playing TrueCraft with your friends is possible, thanks to the new entity
management system. It keeps track of which entities should be aware of which
other entities and spawns/despawns them as appropriate. With this change also
came the very first time any group of TrueCraft enthusiasts played together
online. Thanks to that test run, some oversights in the networking code were
brought forth and the performance of that code was dramatically improved.

Item entities are coming soon, but will require the integration of the Craft.Net
physics engine. I don't expect this integration to be too difficult, and then
we'll be able to mine without having items magically appear in our inventories.

Additionally, we have partially completed the block/item logic subsystem and it
should soon become usable, which means that it'll be easy to add block and item
logic to the game (i.e. using a bed item places a bed). Big thanks to new
contributor "creatorfromhell" who was kind enough to do a lot of the work
involved in that.
