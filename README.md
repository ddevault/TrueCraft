<p align="center">
    <img src="https://sr.ht/3O-k.png" width="728" />
</p>

A completely [clean-room](https://en.wikipedia.org/wiki/Clean_room_design) implementation of Minecraft beta 1.7.3 (circa September 2011). No decompiled code has been used in the development of this software. This is an **implementation** - not a clone. TrueCraft is compatible with Minecraft beta 1.7.3 clients and servers.

[![Build Status](https://travis-ci.org/SirCmpwn/TrueCraft.svg?branch=master)](https://travis-ci.org/SirCmpwn/TrueCraft) [![Donate with fosspay](https://drewdevault.com/donate/static/donate-with-fosspay.png)](https://drewdevault.com/donate?project=1)

![](https://sr.ht/87Ov.png)

*Screenshot taken with [Eldpack](http://eldpack.com/)*

I miss the old days of Minecraft, when it was a simple game. It was nearly perfect. Most of what Mojang has added since beta 1.7.3 is fluff, life support for a game that was "done" years ago. This is my attempt to get back to the original spirit of Minecraft, before there were things like the End, or all-in-one redstone devices, or village gift shops. A simple sandbox where you can build and explore and fight with your friends. I miss that.

The goal of this project is effectively to fork Minecraft. Your contribution is welcome, but keep in mind that I will mercilessly reject changes that aren't in line with the vision. If you like the new Minecraft, please feel free to keep playing it. If you miss the old Minecraft, join me.

## Compiling

**Use a recursive git clone.**

    git clone --recursive git://github.com/SirCmpwn/TrueCraft.git

You need to restore Nuget packages. The easiest way is to open the solution up in monodevelop or visual studio or the like and build from there. You can alternatively acquire Nuget yourself and run this:

    mono path/to/nuget.exe restore

From the root directory of the git repository. Then run:

    xbuild

To compile it and you'll receive binaries in `TrueCraft.Launcher/bin/Debug/`. Run `[mono] TrueCraft.Launcher.exe` to run the client and connect to servers and play singleplayer and so on. Run `[mono] TrueCraft.Server.exe` to host a server for others to play on.

Note: if you have a problem with nuget connecting, run `mozroots --import --sync`.

Note: TrueCraft requires mono 4.0 or newer.

## Get Involved

If you are not a developer, you can keep up with TrueCraft updates and participate in the community on [/r/truecraft](https://reddit.com/r/truecraft), or by joining us to chat in [#truecraft on irc.esper.net](http://webchat.esper.net/?nick=&channels=truecraft).

If you are a developer, you have two paths. If you *have not* read the Minecraft source code, you are what we call a "clean dev", and you should stay that way. If you *have* read the source code, you are what we call a "dirty dev", and the way you can contribute is different. If you are a clean dev, you're welcome to contribute to this repository by adding features and functionality from Minecraft Beta 1.7.3, fixing bugs, refactoring, etc - the usual. [Send pull requests](https://help.github.com/articles/using-pull-requests/) with your work.

If you are a dirty dev, you are more limited in how you can help. You can work on projects that are related to TrueCraft, but not on TrueCraft itself. Direct contributions that you can participate in includes [the website](https://github.com/SirCmpwn/truecraft.io) and the [artwork](https://github.com/SirCmpwn/TrueCraft/tree/master/TrueCraft.Client/Content). You can also work on things like helping to build a community by spreading the word, participating in IRC or the subreddit, etc. You may also work on reverse engineering Minecraft to provide documentation for clean devs to use - see [reverse engineering guidelines](https://github.com/SirCmpwn/TrueCraft/wiki/Reverse-engineering-guidelines) on the wiki for details on how you can do this. **Under no circumstances may you ever share any code with a clean dev, decompiled or otherwise**.

## Assets

TrueCraft is compatible with Minecraft beta 1.7.3 texture packs. We ship the Pixeludi Pack (by Wojtek Mroczek) by default. You can install the Mojang assets through the TrueCraft launcher if you wish.

## Blah blah blah

TrueCraft is not associated with Mojang or Minecraft in any sort of official capacity.
