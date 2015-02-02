---
# vim: tw=80
layout: post
title: Armor and Item Entities
blurb: |
    When you're wearing armor, your friends will know about it. I also added
    item drops so that the things you're mining no longer magically appear in
    your inventory.
---

![](http://a.pomf.se/gdhypb.png)

When you're wearing armor, your friends will know about it. I also added item
drops so that the things you're mining no longer magically appear in your
inventory.

I also mostly finished entity management, so entities are despawned when they
get too far away, and spawned as they get closer. Today, we were also able to
play for well over an hour without a server crash, marking a significant
improvement in server stability.

Additionally, much of the groundwork has been laid to support world interaction
logic. Beds have been implemented (though sleeping has not) and the block
placement logic refactored to use the same system. Grass blocks drop dirt items
and leaves have the vanilla 5% chance of dropping saplings. Today was a very
productive day!
