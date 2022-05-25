# AlphaDilate

**Note**: This approach has various limitations and should probably only used very carefully or not at all. If premultiplied alpha compositing is used (e.g. via the OBS Spout2 plugin), rendering with a black clear color should also solve this issue.

The "Game Capture" function of [OBS](https://obsproject.com/) allows you to directly capture transparency rendered to the screen by applications. This can be useful when rendering an animated avatar without having to use chroma or color keying, which can lead to errors, when the avatar's colors coincide with the keyed color. Alpha transparency also allows for the inclusion of partially transparent objects in the capture.

One possible way to output such objects with transparent backgrounds in Unity is to set up a camera with a background color that has the alpha value set to 0 and "Clear Flags" set to "Solid Color".

However, when anti aliasing is used, the transparent background color can still bleed into the edges of the object.

AlphaDilate allows you to avoid such background color seams around objects by setting the colors of partially transparent pixels to those of fully solid, neighboring pixels. This makes the object appear thicker on screen, however, once captured in OBS, it will look as intended.

To use it, simply add the AlphaDilate component to the camera in Unity. If other post processing effects are used, the AlphaDilateCommandBuffer component should be added instead.

# Demonstration

To illustrate the issue, here is an avatar in front of a grey 100% transparent background, captured in OBS and overlayed on a light blue background. Without AlphaDilute, the grey leaks into the edge of the avatar. With AlphaDilute, the avatar looks thicker outside of OBS.

## Without AlphaDilute
![Without AlphaDilute](https://github.com/emilianavt/AlphaDilate/raw/master/Plugins/AlphaDilate/Screenshots/WithoutAlphaDilute.png)

## With AlphaDilute enabled
![With AlphaDilute enabled](https://github.com/emilianavt/AlphaDilate/raw/master/Plugins/AlphaDilate/Screenshots/WithAlphaDilute.png)

## Without AlphaDilute, outside of OBS
![Without AlphaDilute, outside of OBS](https://github.com/emilianavt/AlphaDilate/raw/master/Plugins/AlphaDilate/Screenshots/WithoutAlphaDiluteNoOBS.png)

## With AlphaDilute enabled, outside of OBS
![With AlphaDilute enabled, outside of OBS](https://github.com/emilianavt/AlphaDilate/raw/master/Plugins/AlphaDilate/Screenshots/WithAlphaDiluteNoOBS.png)
