# Modeling Toolkit

Made by: Osdanova

A toolkit to load 3D model data and be able to visualize it and export/import it as fbx as well as do some common operations on models.
Other model formats can be made into Modeling Toolking objects. If they are loaded and visualized correctly in a viewport (Hooked to a Viewport Service) it should be exportable to fbx.

This was made in order to make it easier porting models between older games. The main issue being that converting a quaternion rotation to XYZ euler rotation most often than not leads to errors.

![](Showcase.png)