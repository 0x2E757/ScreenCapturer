using System;
using System.Drawing;

namespace ScreenCapturerNS {

    public class OnScreenUpdatedEventArgs : EventArgs {

        public Bitmap Bitmap { get; set; }

        internal OnScreenUpdatedEventArgs(Bitmap bitmap) {
            this.Bitmap = bitmap;
        }

    }

}
