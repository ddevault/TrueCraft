# TrueCraft

http://truecraft.io - blog and such

A completely
[clean-room](https://en.wikipedia.org/wiki/Clean_room_design) implementation of
Minecraft beta 1.7.3 (circa September 2011). No decompiled code has been used in
the development of this software.

I miss the old days of Minecraft, when it was a simple game. It was nearly
perfect. Most of what Mojang has added since beta 1.7.3 is fluff, life support
for a game that was "done" years ago. This is my attempt to get back to the
original spirit of Minecraft, before there were things like the End, or
all-in-one redstone devices, or village gift shops. A simple sandbox where you
can build and explore and fight with your friends. I miss that.

The goal of this project is effectively to fork Minecraft. Your contribution is
welcome, but keep in mind that I will mercilessly reject changes that aren't in
line with the vision. If you like the new Minecraft, please feel free to keep
playing it. If you miss the old Minecraft, join me.

### "What about Craft.Net?"

Craft.Net aims to support the latest version of Minecraft. That means I'm aiming
for a moving target every time Mojang updates the game, adding more features
that I don't like. I'm tired of Craft.Net. Let's do this instead.

## Compiling

**Use a recursive git clone.**

    git clone --recursive git://github.com/SirCmpwn/TrueCraft.git

You need to restore Nuget packages. The easiest way is to open the solution up
in monodevelop or visual studio or the like and build from there. You can
alternatively acquire Nuget yourself and run this:

    mono path/to/nuget.exe restore

From the root directory of the git repository. Then run:

    xbuild

To compile it and you'll receive binaries in `TrueCraft/bin/Debug/`.

Note: if you have a problem with nuget connecting, run `mozroots --import
--sync`.

Note: TrueCraft requires mono 3.10 or newer.

## Get Involved

If you want to keep up with development or contribute, join #truecraft on
irc.esper.net. Pull requests will be rejected from authors who have read any
decompiled official Minecraft code.

To get started, sign the [Contributor License
Agreement](https://www.clahub.com/agreements/SirCmpwn/TrueCraft) to establish
that you have not, in fact, ever read decompiled Minecraft code.

## Roadmap

1. Implement server (in progress)
1. Implement client
1. New authentication
1. Backport the good
1. Modding support

First order of business is building a server, which I've started on. Then we'll
have to build a client, and for that I want community help because I have next
to no experience writing actual games.

Mojang has shut off the old authentication servers, which means that it's no
longer possible to use beta 1.7.3 in online mode. We'll have to build our own.
After that, we'll have effectively reimplemented Minecraft from scratch and can
start (conservatively) adding things.

"Backporting the good" refers to implementing features from newer versions of
Minecraft that aren't bad. This also includes refactoring the internal details
of beta 1.7.3 to not suck so much (like the protocol). Some examples of features
I want to bring backwards are sprinting, more food types (but not hunger), and
smaller changes like lighting improvements and bow usage mechanics.

Finally, if we've got a nice mature project and a good community going, modding
support would be great.

## Blah blah blah

TrueCraft is not associated with Mojang or Minecraft in any sort of official
capacity.
