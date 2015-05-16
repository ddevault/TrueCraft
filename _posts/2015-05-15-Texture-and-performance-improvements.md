---
# vim: tw=80
title: Texture and performance improvements
layout: post
blurb: |
    I've spent some time improving the textures from Programmer Art to more closely
    resemble Minecraft, as you can (hopefully) tell from the screenshot here.
---

![](https://a.pomf.se/xipiqz.png)

I've spent some time improving the textures from Programmer Art to more closely
resemble Minecraft, as you can (hopefully) tell from the screenshot here. My
goal here is to make the assets we distribute with TrueCraft look close enough
to the original spirit of Minecraft that users don't feel the need to use the
official assets. Also visible in this screenshot is the improved block rendering
capabilities of TrueCraft, including support for odd models (like snow) and
cubes with different textures on different sides (like crafting benches, TNT,
grass, etc). Here's a similar screenshot with the official assets:

![](https://a.pomf.se/izbwdy.png)

You can probably guess from the new FPS counter that performance is still pretty
hit-and-miss, but I have made some improvements that make it *usually* hang out
at 60 FPS. This includes frustrum culling and only rendering blocks that are
visible into meshes. There are more improvements to come.

Also, RobinKanters has shared with me a screenshot of his port of the client to
Windows:

![](http://a.pomf.se/tbinzl.png)

All exciting things! The client is progressing much more quickly than I
expected.
