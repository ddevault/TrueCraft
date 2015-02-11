---
# vim: tw=80
layout: post
title: Completed fluid dynamics
blurb: |
    After yesterday's work on fluid dynamics was able to produce a working
    simultion of Minecraft's water dynamics, today was spent refactoring and
    improving this code, and applying it to lava.
---

![](http://a.pomf.se/ldjncs.png)

After yesterday's work on fluid dynamics was able to produce a working simultion
of Minecraft's water dynamics, today was spent refactoring and improving this
code, and applying it to lava.

The code was refactored to move the fluid dynamics into a shared simulation
provider, and then applied to both water and lava. This should allow modders in
the future to create new kinds of fulids (I believe rivers of milk have been
suggested). On top of that, yesterday's implementation came with a handful of
bugs that were tracked down and resolved.

Additionally, some work was started towards supporting falling sand, but the
physics engine wasn't cooperating and work on that will have to resume some
other time. However, at to dextar0's suggestion, I did take the time to
implement stair placement properly.
