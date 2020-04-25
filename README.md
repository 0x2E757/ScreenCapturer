# ScreenCapturer

Library for fast screenshot make and screen capture based on [SharpDX](https://www.nuget.org/packages/SharpDX/) package. Available as [NuGet package](https://www.nuget.org/packages/ScreenCapturer/).

## Usage

`ScreenCapturer.MakeScreenshot()` — returning `Bitmap`\* with <b>next screen snapshot</b>.

`ScreenCapturer.StartCapturing(Action<Bitmap>)` — starting screen capture thread with callback function that recieves `Bitmap`\*\* as argument. `MakeScreenshot` can't be used while active.

`ScreenCapturer.StopCapturing()` — stops capturing thread.

`ScreenCapturer.DisableCallback()` — will disable callback without stopping capture.

`ScreenCapturer.EnableCallback()` — will enable callback if it was disabled.

`ScreenCapturer.DisposeVariables()` — disposes some memoized static variables, should be used before program is terminated.

<sub>\* `MakeScreenshot` uses same `Bitmap` variable for all calls, so do not change its state, use only for read and note probably it's not thread safe.</sub>

<sub>\*\* You must manually dispose `Bitmap` that is passed as argument to capture callback function, becase GC likes to leave those objects alive (unless you call `GC.Collect()`, but you probably shouldn't) and you will run into out of memory exception.</sub>
