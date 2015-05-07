---
layout: base
---

Some features that are in-progress will give more details when hovered over.

# Server

The server is coming along nicely. The design of TrueCraft places most of the
logic into the server.

<table class="table">
    <thead>
        <tr>
            <th>Feature</th>
            <th>Progress</th>
        </tr>
    </thead>
    <tbody>
        {% include feature.html name="Beta 1.7.3 networking" progress="done" %}
        {% include feature.html name="MCRegion worlds" progress="done" %}
            {% include feature.html name="Tile entities" progress="done" indent=true %}
            {% include feature.html name="Multiworld" progress="not-started" indent=true %}
            {% include feature.html name="Player metadata" progress="done" indent=true %}
        {% include feature.html name="Entities" progress="meta" %}
            {% include feature.html name="Physics" progress="done" indent=true %}
            {% include feature.html name="Item entities" progress="done" indent=true %}
            {% include feature.html name="Falling blocks" progress="done" indent=true %}
            {% include feature.html name="Players" progress="done" indent=true %}
            {% include feature.html name="Mobs" progress="not-started" indent=true %}
            {% include feature.html name="Minecarts" progress="not-started" indent=true %}
            {% include feature.html name="Boats" progress="not-started" indent=true %}
        {% include feature.html name="AI" progress="not-started" %}
        {% include feature.html name="Terrain generation" progress="done" %}
            {% include feature.html name="Perlin terrain" progress="done" indent=true %}
            {% include feature.html name="Biomes" progress="done" indent=true %}
            {% include feature.html name="Caves" progress="done" indent=true %}
            {% include feature.html name="Decorators" progress="done" indent=true %}
        {% include feature.html name="Window management" progress="meta" %}
            {% include feature.html name="Inventory" progress="done" indent=true %}
            {% include feature.html name="Crafting" progress="done" indent=true %}
            {% include feature.html name="Furnaces" progress="not-started" indent=true %}
            {% include feature.html name="Chests" progress="not-started" indent=true %}
        {% include feature.html name="Modding API" progress="not-started" %}
        {% include feature.html name="Game logic" progress="meta" %}
            {% include feature.html name="Farming" progress="in-progress" indent=true comments="Wheat and sugarcane are done" %}
            {% include feature.html name="Doors" progress="done" indent=true %}
            {% include feature.html name="Unusual item drops" progress="in-progress" indent=true comments="For example, snow drops snowballs instead of snow blocks" %}
            {% include feature.html name="Block updates" progress="done" indent=true %}
            {% include feature.html name="Fluid simulation" progress="done" indent=true %}
            {% include feature.html name="Portals" progress="not-started" indent=true %}
            {% include feature.html name="Signs" progress="done" indent=true %}
            {% include feature.html name="Trees" progress="in-progress" indent=true comments="Generation is done but not hooked up to saplings" %}
            {% include feature.html name="TNT" progress="not-started" indent=true %}
            {% include feature.html name="Redstone" progress="not-started" indent=true %}
            {% include feature.html name="Bonemeal" progress="not-started" indent=true %}
            {% include feature.html name="Fishing" progress="not-started" indent=true %}
            {% include feature.html name="Fire" progress="not-started" indent=true %}
            {% include feature.html name="Jukeboxes" progress="not-started" indent=true %}
            {% include feature.html name="Beds" progress="in-progress" indent=true comments="Placement is done, sleeping is not" %}
            {% include feature.html name="Directional blocks" progress="in-progress" indent=true comments="Refers to things like ladders and dispensers, that face a certain direction" %}
        {% include feature.html name="Lighting" progress="not-started" %}
        {% include feature.html name="Weather" progress="not-started" %}
        {% include feature.html name="Combat" progress="not-started" %}
        {% include feature.html name="Food" progress="not-started" %}
    </tbody>
</table>

# Client

Work on the client has not begun.

# Auth Servers

Work on the auth servers has not begun.
