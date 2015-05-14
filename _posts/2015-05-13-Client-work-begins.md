---
# vim: tw=80
title: Client work begins
layout: post
blurb: |
    I've started work on the client, and I have a *very* early screenshot to share
    with you.
---

![](https://a.pomf.se/zkhhmx.png)

I've started work on the client, and I have a *very* early screenshot to share
with you.

I'm writing the client with MonoGame (so I can share a lot of the TrueCraft core
code with it). Currently I'm only worried about supporting Linux but expanding
to more platforms should be very easy (including platforms like Android and
iOS!).

Thanks to being able to leverage the work that's already been put into the
server, the client already supports quite a bit:

* Connecting to servers
* Receiving and displaying chat messages
* Physics and simple movement
* Chunk to mesh conversion and rendering

The screenshot should tell you that we have a long ways to go, but this is a
start!
