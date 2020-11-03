# ElementEngine
A simple C#/Veldrid based 2D game engine.

Still very much an early stage work in progress with an initial focus on optimised 2D game development.

### Features
* Simply inherit from the base game class and overload the load, update and draw methods, and you'll get a fully working game loop out of the box
* Game states are built into the core game loop so you can change states through a single easy method call
* 2D rendering with sprite batching, primitive batching and highly optimised tile maps
* A TTF font renderer that can be batched through the sprite batch
* Sprites, animated sprites and layered animated sprites
* A simple file format for importing and playing sprite animations
* Asset manager to support easy modding and resource loading
* Simple 2D collision handling for rectangles and circles
* A simple state machine and GOAP controller for entity AI
* An easy to use 2D camera
* Keyboard and mouse input management
* A game controls system that transforms keyboard and mouse input combinations into meaningful control names like "Move Up", "Attack" etc.
* Import maps created in Tiled with a built in optimised renderer for them that makes use of custom properties to load assets and control layer ordering
