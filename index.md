---
# vim: tw=80
layout: base
---

# TrueCraft

The good old days of Minecraft, brought back in all their simplistic glory.

## About TrueCraft

A completely [clean-room](https://en.wikipedia.org/wiki/Clean_room_design)
implementation of Minecraft beta 1.7.3 (circa September 2011). No decompiled
code has been used in the development of this software.

I miss the old days of Minecraft, when it was a simple game. It was nearly
perfect. Most of what Mojang has added since beta 1.7.3 is fluff, life support
for a game that was "done" years ago. This is my attempt to get back to the
original spirit of Minecraft, before there were things like the End, or
all-in-one redstone devices, or village gift shops. A simple sandbox where you
can build and explore and fight with your friends. I miss that.

[**Roadmap »**](/roadmap.html)

<a class="rss" href="/rss.xml"><img src="/images/rss.png" /></a>

# Updates

So far, the only work done on TrueCraft has been towards the server component.
All of these screenshots were taken with the official Minecraft beta 1.7.3
client.

{% for post in site.posts %}
<h3>
    <a href="{{ post.url }}">{{ post.title }}</a>
    <small>{{ post.date | date_to_string }}</small>
</h3>
{% if post.blurb %}
{{ post.blurb }}
{% endif %}
{{ post.excerpt }}
<p class="text-right">
    <a href="{{ post.url }}">Read the full article »</a>
</p>
{% endfor %}
