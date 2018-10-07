# LGIM_Mono

## Introduction
This is basic 3d engine. It allows loading multiple 3d models in .obj format. Loaded models can be freely rotated, scaled and moved on the surface. For each model can have texture. Engine also allows rotation and moving our point of view.

## Technologies
* Monodevelop
* C#
* Mono
* GtkSharp
* Math.Net

## Setup
This project can be built in Monodevelop. All models and textures can be loaded from any accessible location from disk.

## Controls
| Key | Function |
|-----|----------|
| W   | Move the camera forward |
| S | Move the camera backward |
| A | Move the camera to the left |
| D | Move the camera to the right |
| Space | Move the camera up |
| Left Control | Move the camera down |
| LPM + Mouse move | Rotate the camera in the x-axis and y-axis |
| Left Shift + LPM + Mouse move | Rotate the camera in the z-axis |
| Mouse Scroll | Zoom |
| 1 | Change mode to moving |
| 2 | Change mode to scaling | 
| 3 | Change mode to rotating |

### Moving mode
| Key | Function |
|-----|----------|
| PPM + Horizontal mouse move | Move the object to the left/right |
| PPM + Vertical mouse move | Move the object up/down |
| Left Shift + PPM + Veritcal mouse move | Move the object to yourself/from yourself |

### Scaling mode
| Key | Function |
|-----|----------|
| PPM + Horizontal mouse move | Scale the object horizontally  |
| PPM + Vertical mouse move | Scale the object vertically |
| Left Shift + PPM + Veritcal mouse move | Scale the object horizontally and vertically |

### Rotating mode
| Key | Function |
|-----|----------|
| PPM + Horizontal mouse move | Rotate the object horizontally  |
| PPM + Vertical mouse move | Rotate the object vertically |
| Left Shift + PPM + Veritcal mouse move | Rotate the object in the z-axis |
