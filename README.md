﻿# OMEGA Game Engine [WIP]

 This is an amalgamation of all my game engines that I made in the span of 10 years. Currently work in progress;

 **This is using**:

 - C# .NET 5.0
 - SDL2/[SDL2-CS][10] for window creation, graphics context, mouse, keyboard and gamepad input;
 - [BGFX][1] for low level graphics API abstraction;
 - [StbImageSharp][9] for image loading/writing;
 - Code fragments from [SharpBGFX][2] and [bgfx-cs][3] to help better abstract low level bindings;
 - Game loop code from https://github.com/TylerGlaiel/FrameTimingControl;
 - [FontBM][12] for generating bitmap fonts; 
 - Binary parsing code from [Cyotek BitmapFont][13] for parsing binary .FNT file generated by FontBM;
 - [ImGui.NET](https://github.com/mellinoe/ImGui.NET) and [ImGui](https://github.com/ocornut/imgui/blob/master/imgui.h);

 **This project has taken inspiration or is using code directly from:**

 - [Sokol][4]
 - [Vortex2D (RIP: First game engine I studied and used)][5]
 - [HGE Game Engine (RIP: One of the first game engines I've studied)][6]
 - [LibGDX (Learned low level (OpenGL level) graphics code from it)][11]
 - [MonoGame (Using input code from it)][7]
 - [FNA][8]

[1]: https://github.com/bkaradzic/bgfx
[2]: https://github.com/MikePopoloski/SharpBgfx
[3]: https://github.com/msmshazan/bgfx-cs
[4]: https://github.com/floooh/sokol
[5]: https://archive.codeplex.com/?p=vortex2d
[6]: https://kvakvs.github.io/hge/
[7]: https://github.com/MonoGame/MonoGame
[8]: https://github.com/FNA-XNA/FNA
[9]: https://github.com/StbSharp/StbImageSharp
[10]: https://github.com/flibitijibibo/SDL2-CS
[11]: https://github.com/libgdx/libgdx
[12]: https://github.com/vladimirgamalyan/fontbm
[13]: https://github.com/cyotek/Cyotek.Drawing.BitmapFont/blob/master/src/BitmapFont.cs
