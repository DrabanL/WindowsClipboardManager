using RabanSoft.ClipboardExtension.Core.Helpers;
using RabanSoft.ClipboardExtension.Core.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Forms;

namespace RabanSoft.ClipboardExtension.Core {
    /// <summary>
    /// Manages and implements logics to extend Windows Clipboard
    /// </summary>
    public class ClipboardManager {

        /// <summary>
        /// Eanble to re-add object to Clipboard queue right after it had been inserted to Windows Clipboard 
        /// </summary>
        public static bool IsLoopPaste;
        /// <summary>
        /// When setting object to Windows Clipboard, it would cause Clipboard Hook to be invoked for the very same object.
        /// So to avoid that, we ignore next 2 Invokes.
        /// TODO: find a better way to handle that, maybe with custom Clipboard object...
        /// </summary>
        private static int _ignoreSetClipboardEventCount;

        private static Queue<ClipboardObject> _dataQueue = new Queue<ClipboardObject>();

        public static void TryCopy() {
            var bestFormat = ClipboardHelper.GetManagedFormat(out var copyObj);
            if (bestFormat == null)
                return;

            if (_ignoreSetClipboardEventCount > 0) {
                _ignoreSetClipboardEventCount--;
                return;
            }

            Debug.WriteLine($"ClipboardManager.TryCopy {bestFormat}, {copyObj}");

            _dataQueue.Enqueue(new ClipboardObject(bestFormat, copyObj));
        }

        private static ClipboardObject getFromQueue() {
            if (_dataQueue.Count == 0)
                return default;

            var obj = _dataQueue.Dequeue();
            if (IsLoopPaste)
                _dataQueue.Enqueue(obj);

            Debug.WriteLine($"ClipboardManager.getFromQueue ({_dataQueue.Count}), {obj}");

            return obj;
        }

        public static void TryPaste() {
            var pasteObj = getFromQueue();
            if (pasteObj == null)
                return;

            Debug.WriteLine($"paste {pasteObj.Format}: {pasteObj.Data}");

            _ignoreSetClipboardEventCount = 2;
            switch (pasteObj.Format) {
                case "Text":
                    Clipboard.SetDataObject(pasteObj.Data, true);
                    break;
                case "FileDrop":
                    var list = new StringCollection();
                    list.AddRange((string[])pasteObj.Data);
                    Clipboard.SetFileDropList(list);
                    break;
                case "Bitmap":
                    Clipboard.SetDataObject(pasteObj.Data, true);
                    break;
            }
        }
    }
}
