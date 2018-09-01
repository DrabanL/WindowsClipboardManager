using System.Windows.Forms;

namespace RabanSoft.ClipboardExtension.Core.Helpers {
    public static class ClipboardHelper {
        /// <summary>
        /// Returns the most probable format of the object currently in Clipboard.
        /// Returns null if could not be determined.
        /// </summary>
        public static string GetManagedFormat(out object obj) {
            obj = null;
            
            var copyObj = Clipboard.GetDataObject();
            string checkClipboardObject(string format, out object resultObj) {
                resultObj = null;

                if (Clipboard.ContainsData(format)) {
                    resultObj = copyObj.GetData(format);
                    return format;
                }

                return default;
            }

            return
                checkClipboardObject(DataFormats.Bitmap, out obj) ??
                checkClipboardObject(DataFormats.FileDrop, out obj) ??
                checkClipboardObject(DataFormats.Text, out obj) ??
                null;
        }
    }
}
