using RabanSoft.ClipboardExtension.Core;
using RabanSoft.SysHooks.Core;
using RabanSoft.SysHooks.Implementations;
using System.Threading;

namespace RabanSoft.BasicClipboardExtension {
    internal class Program {
        private static void Main(string[] args) {
            // this basic implemetation of Clipboard manager extends Windows clipboard to be able to sequentially copy&paste clipboard data
            // for example: copy (file1), copy (file2), copy (file3). and then, paste (file1), paste (file2), paste (file3), paste (file1) etc..
            // the program is built as Windows Application executable so it will be able to run siliently as a Background process without User Interaction

            ClipboardManager.IsLoopPaste = false;

            KeyboardListener.OnPaste = ClipboardManager.TryPaste;
            ClipboardListener.OnCopy = ClipboardManager.TryCopy;

            using (var clipboardHook = new WindowHookBase<ClipboardHook>()) {
                ClipboardListener.Start(clipboardHook);
                using (var keyboardHook = new WindowHookBase<KeyboardHook>()) {
                    KeyboardListener.Start(keyboardHook);
                    // waiting forever to halt termination of the program
                    SpinWait.SpinUntil(() => { return false; });
                    KeyboardListener.Stop(keyboardHook);
                }
                ClipboardListener.Stop(clipboardHook);
            }
        }
    }
}
