---
# vim: tw=80
title: Internal improvements &amp; more terrain gen
layout: post
blurb: |
    There was some progress today on improving some of the server's internal
    mechanics, as well as some improvements to the terrain generator.
---

![](http://a.pomf.se/jksvcq.png)

There was some progress today on improving some of the server's internal
mechanics, as well as some improvements to the terrain generator. I tweaked some
constants from creatorfromhell's work and fixed a number of bugs, resulting in
a gorgeous terrain generator. Thanks a ton to him for all the hard work. I also
spent some time improving how TrueCraft shuts down - all worlds are saved, all
player profiles are saved, and clients are gracefully disconnected when
TrueCraft gets a SIGTERM.

I also fixed a stupid bug with how tall grass drops seeds, but that's not very
interesting so I won't mention it.
