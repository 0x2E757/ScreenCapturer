# ScreenCapturer

Library for easy screen capture based on [SharpDX](https://github.com/sharpdx/SharpDX/) [package](https://www.nuget.org/packages/SharpDX/). Available as [NuGet package](https://www.nuget.org/packages/ScreenCapturer/).

Compatible with .NET Standard 2.0 and higher.

## Usage

All methods, events and properties can be accessed using static `ScreenCapturer` class.

### Methods

`StartCapture` — starts capture and callback threads.

`StopCapturing` — asynchronously stops capture and callback threads.

### Events

`OnScreenUpdated` — dispatched when screen snapshot was made, additional info in notes.

`OnCaptureStop` — dispatched when capture is stopped manually or because of exception.

### Flags and options

`SkipFirstFrame` — if flag is set to `true` (default) then first captured screen snapshot gets skipped and no callbacks called (for some unknown reason it is a black screen, apparently [SharpDX](https://github.com/sharpdx/SharpDX/) related issue).

`SkipFrames` — if flag is set to `true` (default) and snapshot queue size gets over than 2 bitmaps then oldest bitmaps get removed.

`PreserveBitmap` — if flag is set to `false` (default) then after callbacks were executed bitmap gets disposed automatically.

`IsActive` — read-only property that indicates that capture is active (it can be starting, in progress or going to be stopped).

`IsNotActive` — read-only property that indicated that capture is not active (opposite to `IsActive`).

### Usage examples

Minimal:
```C#
using System.Drawing;
using ScreenCapturerNS;

ScreenCapturer.StartCapture((Bitmap bitmap) => {
    // Process image (bitmap) here
});
```

Typical C# events way:
```C#
using System.Drawing;
using ScreenCapturerNS;

void OnScreenUpdated(Object? sender, OnScreenUpdatedEventArgs e) {
    // Process image (e.Bitmap) here
}

ScreenCapturer.OnScreenUpdated += OnScreenUpdated;
ScreenCapturer.StartCapture();
```

### Notes

You can set callback by passing action as argument to `StartCapture` or set it as standard event callback using `OnScreenUpdated`. `StopCapturing` will stop capture as fast as possible, however method will return immediately to minimize deadlock possibility. 

Use `OnCaptureStop` event if you need to perform actions after capture was really stopped. If capture process gets interrupted by exception (SharpDX exception in capture thread or exception in any of capture callbacks in callback thread) and `OnCaptureStop` event has assigned callback — it will be called with exception as argument (might be useful if resolution gets changed and etc.), otherwise regular exception will be thrown.

## Possible issues

If capture gets often interrupted by SharpDX exceptions small memory leaks may occur, though it's should be unnoticable in most cases and can be ignored.
