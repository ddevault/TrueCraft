# TrueCraft

[![Build Status](https://travis-ci.org/SirCmpwn/TrueCraft.svg?branch=master)](https://travis-ci.org/SirCmpwn/TrueCraft)

http://truecraft.io - blog and such

A completely [clean-room](https://en.wikipedia.org/wiki/Clean_room_design) implementation of Minecraft beta 1.7.3 (circa September 2011). No decompiled code has been used in the development of this software. This is an **implementation** - not a clone. TrueCraft is compatible with Minecraft beta 1.7.3 clients and servers.

![](https://sr.ht/b25c.png)

I miss the old days of Minecraft, when it was a simple game. It was nearly perfect. Most of what Mojang has added since beta 1.7.3 is fluff, life support for a game that was "done" years ago. This is my attempt to get back to the original spirit of Minecraft, before there were things like the End, or all-in-one redstone devices, or village gift shops. A simple sandbox where you can build and explore and fight with your friends. I miss that.

The goal of this project is effectively to fork Minecraft. Your contribution is welcome, but keep in mind that I will mercilessly reject changes that aren't in line with the vision. If you like the new Minecraft, please feel free to keep playing it. If you miss the old Minecraft, join me.

### "What about Craft.Net?"

Craft.Net aims to support the latest version of Minecraft. That means I'm aiming for a moving target every time Mojang updates the game, adding more features that I don't like. I'm tired of Craft.Net. Let's do this instead.

## Compiling

**Use a recursive git clone.**

    git clone --recursive git://github.com/SirCmpwn/TrueCraft.git

You need to restore Nuget packages. The easiest way is to open the solution up in monodevelop or visual studio or the like and build from there. You can alternatively acquire Nuget yourself and run this:

    mono path/to/nuget.exe restore

From the root directory of the git repository. Then run:

    xbuild

To compile it and you'll receive binaries in `TrueCraft/bin/Debug/`.

Note: if you have a problem with nuget connecting, run `mozroots --import --sync`.

Note: TrueCraft requires mono 3.10 or newer.

## Get Involved

If you are not a developer, you can keep up with TrueCraft updates and participate in the community on [/r/truecraft](https://reddit.com/r/truecraft), or by joining us to chat in [#truecraft on irc.esper.net](http://webchat.esper.net/?nick=&channels=truecraft).

If you are a developer, you have two paths. If you *have not* read the Minecraft source code, you are what we call a "clean dev", and you should stay that way. If you *have* read the source code, you are what we call a "dirty dev", and the way you can contribute is different. If you are a clean dev, you're welcome to contribute to this repository by adding features and functionality from Minecraft Beta 1.7.3, fixing bugs, refactoring, etc - the usual. [Send pull requests](https://help.github.com/articles/using-pull-requests/) with your work.

If you are a dirty dev, you are more limited in how you can help. You can work on projects that are related to TrueCraft, but not on TrueCraft itself. Direct contributions that you can participate in includes [the website](https://github.com/SirCmpwn/truecraft.io) and the [artwork](https://github.com/SirCmpwn/TrueCraft/tree/master/TrueCraft.Client/Content). You can also work on things like helping to build a community by spreading the word, participating in IRC or the subreddit, etc. You may also work on reverse engineering Minecraft to provide documentation for clean devs to use - see [reverse engineering guidelines](https://github.com/SirCmpwn/TrueCraft/wiki/Reverse-engineering-guidelines) on the wiki for details on how you can do this. **Under no circumstances may you ever share any code with a clean dev, decompiled or otherwise**.

## Assets

TrueCraft cannot use the official Minecraft assets, such as the texture pack and some of the sound effects. We maintain our own texture pack:

![](https://raw.githubusercontent.com/SirCmpwn/TrueCraft/master/TrueCraft.Client/Content/terrain.png)

This is based on [Programmer Art](https://github.com/deathcap/ProgrammerArt), and we are slowly replacing each texture with something that is as close to the Minecraft look & feel as possible. There is also a [Github issue](https://github.com/SirCmpwn/TrueCraft/tree/master/TrueCraft.Client/Content) around finding more assets for our uses.

We have kept compatability with Minecraft (beta 1.7.3) texture packs, so if you want to use the official assets privately you are welcome to. See [the relevant page](https://github.com/SirCmpwn/TrueCraft/wiki/Using-official-assets) on the wiki.

## Blah blah blah

TrueCraft is not associated with Mojang or Minecraft in any sort of official capacity.
