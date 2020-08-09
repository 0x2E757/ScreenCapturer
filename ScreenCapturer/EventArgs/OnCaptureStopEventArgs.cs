using System;

namespace ScreenCapturerNS {

    public class OnCaptureStopEventArgs : EventArgs {

        public Exception Exception { get; set; }

        internal OnCaptureStopEventArgs(Exception exception) {
            this.Exception = exception;
        }

    }

}
