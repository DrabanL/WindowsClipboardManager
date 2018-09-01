using RabanSoft.SysHooks.Core;
using RabanSoft.SysHooks.Implementations;
using RabanSoft.SysHooks.Interfaces;
using System;
using System.Diagnostics;

namespace RabanSoft.ClipboardExtension.Core {
    /// <summary>
    /// Manages the Clipboard hook callback and events
    /// </summary>
    public class ClipboardListener : IHookCallback {

        public static Action OnCopy;
        private static ClipboardListener _instance;

        public static void Start(WindowHookBase<ClipboardHook> hookBase) {
            if (_instance == null)
                _instance = new ClipboardListener();

            hookBase.Subscribe(_instance);
        }

        public static void Stop(WindowHookBase<ClipboardHook> hookBase) {
            if (_instance == null)
                return;

            hookBase.Unsubscribe(_instance);
        }

        void IHookCallback.OnError(Exception ex) {
            Debug.WriteLine($"ClipboardListener(IHookCallback).OnError\n{ex.Message}");
        }

        void IHookCallback.OnHookProc(int nCode, IntPtr wParam, IntPtr lParam) {
            Debug.WriteLine($"ClipboardListener(IHookCallback).OnHookProc {nCode}, {wParam}, {lParam}");

            OnCopy?.Invoke();
        }
    }
}
