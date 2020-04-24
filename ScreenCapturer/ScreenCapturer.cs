using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.DXGI.Resource;

namespace ScreenCapturerNS {

    public static class ScreenCapturer {

        public static Boolean CaptureActive { get; private set; }
        public static Boolean CallbackEnabled { get; private set; }

        private static Thread Thread;

        private static Factory1 Factory1;
        private static Adapter1 Adapter1;
        private static Device Device;
        private static Output Output;
        private static Output1 Output1;
        private static Int32 Width;
        private static Int32 Height;
        private static Rectangle Bounds;
        private static Texture2DDescription Texture2DDescription;
        private static Texture2D Texture2D;
        private static OutputDuplication OutputDuplication;
        private static Bitmap Bitmap;

        private static Int32 MakeScreenshot_LastDisplayIndexValue = 0;
        private static Int32 MakeScreenshot_LastAdapterIndexValue = 0;

        static ScreenCapturer() {
            CaptureActive = false;
            CallbackEnabled = true;
            Thread = null;
            InitializeStaticVariables(0, 0, true);
        }

        public static void StartCapturing(Action<Bitmap> onCaptured, Int32 minimalDelay = 0, Int32 displayIndex = 0, Int32 adapterIndex = 0, Int32 maxTimeout = 60000) {
            CaptureActive = true;
            Thread = new Thread(() => ThreadMain(onCaptured, minimalDelay, displayIndex, adapterIndex, maxTimeout));
            Thread.IsBackground = true;
            Thread.Priority = ThreadPriority.Lowest;
            Thread.Start();
        }

        public static void StopCapturing() {
            CaptureActive = false;
            Thread.Join();
            Thread = null;
            DisposeVariables(true);
        }

        public static void EnableCallback() {
            CallbackEnabled = true;
        }

        public static void DisableCallback() {
            CallbackEnabled = false;
        }

        private static void ThreadMain(Action<Bitmap> onCaptured, Int32 minimalDelay, Int32 displayIndex, Int32 adapterIndex, Int32 maxTimeout) {
            Stopwatch stopwatch = new Stopwatch();
            while (CaptureActive) {
                stopwatch.Restart();
                Bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppRgb);
                InnerMakeScreenshot(displayIndex, adapterIndex, maxTimeout);
                if (CaptureActive && CallbackEnabled) onCaptured(Bitmap);
                Thread.Sleep((Int32)Math.Max(minimalDelay - stopwatch.ElapsedMilliseconds, 0));
            }
        }

        public static Bitmap MakeScreenshot(Int32 displayIndex = 0, Int32 adapterIndex = 0, Int32 maxTimeout = 60000) {
            if (Thread != null) throw new Exception("Do not call MakeScreenshot while capturing is active!");
            return InnerMakeScreenshot(displayIndex, adapterIndex, maxTimeout);
        }

        private static Bitmap InnerMakeScreenshot(Int32 displayIndex, Int32 adapterIndex, Int32 maxTimeout) {
            InitializeStaticVariables(displayIndex, adapterIndex);
            Resource screenResource;
            OutputDuplication.AcquireNextFrame(maxTimeout, out _, out screenResource);
            Texture2D screenTexture2D = screenResource.QueryInterface<Texture2D>();
            Device.ImmediateContext.CopyResource(screenTexture2D, Texture2D);
            DataBox dataBox = Device.ImmediateContext.MapSubresource(Texture2D, 0, MapMode.Read, MapFlags.None);
            BitmapData bitmapData = Bitmap.LockBits(Bounds, ImageLockMode.WriteOnly, Bitmap.PixelFormat);
            IntPtr dataBoxPointer = dataBox.DataPointer;
            IntPtr bitmapDataPointer = bitmapData.Scan0;
            for (Int32 y = 0; y < Height; y++) {
                Utilities.CopyMemory(bitmapDataPointer, dataBoxPointer, Width * 4);
                dataBoxPointer = IntPtr.Add(dataBoxPointer, dataBox.RowPitch);
                bitmapDataPointer = IntPtr.Add(bitmapDataPointer, bitmapData.Stride);
            }
            Bitmap.UnlockBits(bitmapData);
            Device.ImmediateContext.UnmapSubresource(Texture2D, 0);
            OutputDuplication.ReleaseFrame();
            screenTexture2D.Dispose();
            screenResource.Dispose();
            return Bitmap;
        }

        private static void InitializeStaticVariables(Int32 displayIndex, Int32 adapterIndex, Boolean forcedInitialization = false) {
            Boolean displayIndexChanged = MakeScreenshot_LastDisplayIndexValue != displayIndex;
            Boolean adapterIndexChanged = MakeScreenshot_LastAdapterIndexValue != adapterIndex;
            if (displayIndexChanged || adapterIndexChanged || forcedInitialization) {
                DisposeVariables(true);
                Factory1 = new Factory1();
                Adapter1 = Factory1.GetAdapter1(adapterIndex);
                Device = new Device(Adapter1);
                Output = Adapter1.GetOutput(displayIndex);
                Output1 = Output.QueryInterface<Output1>();
                Width = Output1.Description.DesktopBounds.Right - Output1.Description.DesktopBounds.Left;
                Height = Output1.Description.DesktopBounds.Bottom - Output1.Description.DesktopBounds.Top;
                Bounds = new Rectangle(Point.Empty, new Size(Width, Height));
                Texture2DDescription = new Texture2DDescription {
                    CpuAccessFlags = CpuAccessFlags.Read,
                    BindFlags = BindFlags.None,
                    Format = Format.B8G8R8A8_UNorm,
                    Width = Width,
                    Height = Height,
                    OptionFlags = ResourceOptionFlags.None,
                    MipLevels = 1,
                    ArraySize = 1,
                    SampleDescription = { Count = 1, Quality = 0 },
                    Usage = ResourceUsage.Staging
                };
                Texture2D = new Texture2D(Device, Texture2DDescription);
                OutputDuplication = Output1.DuplicateOutput(Device);
                OutputDuplication.AcquireNextFrame(60000, out _, out _);
                OutputDuplication.ReleaseFrame();
                Bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppRgb);
                MakeScreenshot_LastAdapterIndexValue = adapterIndex;
                MakeScreenshot_LastDisplayIndexValue = displayIndex;
            }
        }

        public static void DisposeVariables() {
            DisposeVariables(false);
        }

        private static void DisposeVariables(Boolean isSafe) {
            if (isSafe != true && Thread != null) throw new Exception("Do not call DisposeVariables while capturing is active!");
            Bitmap?.Dispose();
            OutputDuplication?.Dispose();
            Texture2D?.Dispose();
            Output1?.Dispose();
            Output?.Dispose();
            Device?.Dispose();
            Adapter1?.Dispose();
            Factory1?.Dispose();
            MakeScreenshot_LastAdapterIndexValue = 0;
            MakeScreenshot_LastDisplayIndexValue = 0;
        }

    }

}
