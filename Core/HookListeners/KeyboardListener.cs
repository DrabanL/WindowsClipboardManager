using RabanSoft.SysHooks.Core;
using RabanSoft.SysHooks.Implementations;
using RabanSoft.SysHooks.Interfaces;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RabanSoft.ClipboardExtension.Core {
    /// <summary>
    /// Manages the Keyboard hook callback and events
    /// </summary>
    public class KeyboardListener : IHookCallback {
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x101;

        public static Action OnPaste;
        private static KeyboardListener _instance;

        public static void Start(WindowHookBase<KeyboardHook> hookBase) {
            if (_instance == null)
                _instance = new KeyboardListener();

            hookBase.Subscribe(_instance);
        }

        public static void Stop(WindowHookBase<KeyboardHook> hookBase) {
            if (_instance == null)
                return;

            hookBase.Unsubscribe(_instance);
        }

        private bool _isCtrlDown;

        void IHookCallback.OnError(Exception ex) {
            Debug.WriteLine(ex.Message);
        }

        void IHookCallback.OnHookProc(int nCode, IntPtr wParam, IntPtr lParam) {
            Debug.WriteLine($"KeyboardListener(IHookCallback).OnHookProc {nCode}, {wParam}, {lParam}");
            
            if (nCode < 0)
                // for keyboard events, nCode should not be less than zero
                return;

            var vkCode = (Keys)Marshal.ReadInt32(lParam);
            switch ((int)wParam) {
                case WM_KEYDOWN:
                    if (vkCode == Keys.LControlKey || vkCode == Keys.RControlKey) {
                        _isCtrlDown = true;
                        return;
                    }

                    // we look for a specific keycode combination (CTRL+C)
                    if (vkCode == Keys.V && _isCtrlDown)
                        OnPaste?.Invoke();
                    break;
                case WM_KEYUP:
                    if (vkCode == Keys.LControlKey || vkCode == Keys.RControlKey)
                        _isCtrlDown = false;
                    break;
            }
        }
    }
}
