# ScreenCapturer

Library for fast screenshot make and screen capture based on [SharpDX](https://www.nuget.org/packages/SharpDX/) package. Available as NuGet package.

## Usage

`ScreenCapturer.MakeScreenshot()` — returning `Bitmap`* with next screen snapshot.

`ScreenCapturer.StartCapturing(Action<Bitmap>)` — starting screen capture thread with callback function. `MakeScreenshot` can't be used while active.

`ScreenCapturer.StopCapturing()` — stops capturing thread.

`ScreenCapturer.DisposeVariables()` — disposes some memoized static variables, should be used before program is terminated.

<sub>\* `MakeScreenshot` uses same `Bitmap` variable for all calls, so do not change its state, use only for read and note probably it's not thread safe.</sub>