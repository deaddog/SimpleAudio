using System;

namespace SimpleAudio.Hotkeys
{
    public class HotKeyAlreadyRegisteredException : Exception
    {
        private HotKey hotkey;
        public HotKey HotKey
        {
            get { return hotkey; }
        }
            
        public HotKeyAlreadyRegisteredException(string message, HotKey hotkey)
            : base(message)
        {
            this.hotkey = hotkey;
        }
        public HotKeyAlreadyRegisteredException(string message, HotKey hotkey, Exception inner)
            : base(message, inner)
        {
            this.hotkey = hotkey;
        }
    }
}
