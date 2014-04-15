using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace SimpleAudio.Hotkeys
{
    public partial class HotKeyManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeyManager"/> class.
        /// </summary>
        /// <param name="hwndSource">The handle of the window. Must not be null.</param>
        public HotKeyManager(HwndSource hwndSource)
        {
            if (hwndSource == null)
                throw new ArgumentNullException("hwndSource");

            this.hook = new HwndSourceHook(WndProc);
            this.hwndSource = hwndSource;
            hwndSource.AddHook(hook);

            this.hotkeys = new Dictionary<int, HotKey>();
        }

        #region HotKey Interop

        private const int WM_HotKey = 786;

        [DllImport("user32", CharSet = CharSet.Ansi,
                   SetLastError = true, ExactSpelling = true)]
        private static extern int RegisterHotKey(IntPtr hwnd,
                int id, int modifiers, int key);

        [DllImport("user32", CharSet = CharSet.Ansi,
                   SetLastError = true, ExactSpelling = true)]
        private static extern int UnregisterHotKey(IntPtr hwnd, int id);

        #endregion

        #region Interop-Encapsulation

        private HwndSourceHook hook;
        private HwndSource hwndSource;

        private void RegisterHotKey(int id, Key key, ModifierKeys modifiers)
        {
            if ((int)hwndSource.Handle != 0)
            {
                RegisterHotKey(hwndSource.Handle, id, (int)modifiers, KeyInterop.VirtualKeyFromKey(key));
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    Exception e = new Win32Exception(error);

                    if (error == 1409)
                        throw new HotKeyAlreadyRegisteredException(e.Message, key, modifiers, e);
                    else
                        throw e;
                }
            }
            else
                throw new InvalidOperationException("Handle is invalid");
        }

        private void UnregisterHotKey(int id)
        {
            if ((int)hwndSource.Handle != 0)
            {
                UnregisterHotKey(hwndSource.Handle, id);
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                    throw new Win32Exception(error);
            }
        }

        #endregion

        private class IDGenerator
        {
            private int current = 1;

            public int Next()
            {
                return current++;
            }
        }
        private static readonly IDGenerator idgen = new IDGenerator();

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HotKey && hotkeys.ContainsKey((int)wParam))
                hotkeys[(int)wParam].Execute();

            return new IntPtr(0);
        }

        private Dictionary<int, HotKey> hotkeys;

        public void AddHotKey(Key key, ModifierKeys modifiers, Action action)
        {
            if (key == Key.None)
                throw new ArgumentException("A key was not specified for this hotkey.", "key");
            if (modifiers == ModifierKeys.None)
                throw new ArgumentException("A modifier was not specified for this hotkey.", "modifiers");
            if (action == null)
                throw new ArgumentNullException("An action is required when adding a HotKey.", "action");

            HotKey hk = hotkeys.Values.Where(x => x.Key == key && x.Modifiers == modifiers).FirstOrDefault();
            if (hk == null)
            {
                int id = idgen.Next();
                hk = new HotKey(id, key, modifiers);
                hotkeys.Add(id, hk);
                RegisterHotKey(id, key, modifiers);
            }
            hk.AddAction(action);
        }

        public bool RemoveHotKey(Key key, ModifierKeys modifier, Action action)
        {
            if (key == Key.None)
                throw new ArgumentException("A key was not specified for this hotkey.", "key");
            if (modifier == ModifierKeys.None)
                throw new ArgumentException("A modifier was not specified for this hotkey.", "modifiers");
            if (action == null)
                throw new ArgumentNullException("An action is required when adding a HotKey.", "action");

            HotKey hk = hotkeys.Values.Where(x => x.Key == key && x.Modifiers == modifier).FirstOrDefault();
            if (hk != null && hk.RemoveAction(action))
            {
                if (hk.Actions == 0)
                    ClearHotKey(key, modifier);
                return true;
            }
            else
                return false;
        }

        public void ClearHotKey(Key key, ModifierKeys modifier)
        {
            if (key == Key.None)
                throw new ArgumentException("A key was not specified for this hotkey.", "key");
            if (modifier == ModifierKeys.None)
                throw new ArgumentException("A modifier was not specified for this hotkey.", "modifiers");

            HotKey hk = hotkeys.Values.Where(x => x.Key == key && x.Modifiers == modifier).FirstOrDefault();
            if (hk != null)
            {
                hotkeys.Remove(hk.Id);
                UnregisterHotKey(hk.Id);
            }
        }


        private bool disposed;
        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                hwndSource.RemoveHook(hook);

            while (hotkeys.Count > 0)
            {
                HotKey hk = hotkeys[0];
                ClearHotKey(hk.Key, hk.Modifiers);
            }

            disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HotKeyManager()
        {
            this.Dispose(false);
        }
    }
}
