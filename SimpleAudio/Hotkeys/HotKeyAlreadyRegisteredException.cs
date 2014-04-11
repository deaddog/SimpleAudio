using System;
using System.Windows.Input;

namespace SimpleAudio.Hotkeys
{
    public class HotKeyAlreadyRegisteredException : Exception
    {
        private Key key;
        private ModifierKeys modifiers;

        public ModifierKeys Modifiers
        {
            get { return modifiers; }
            set { modifiers = value; }
        }
        public Key Key
        {
            get { return key; }
            set { key = value; }
        }

        public HotKeyAlreadyRegisteredException(string message, Key key, ModifierKeys modifiers)
            : base(message)
        {
            this.key = key;
            this.modifiers = modifiers;
        }
        public HotKeyAlreadyRegisteredException(string message, Key key, ModifierKeys modifiers, Exception inner)
            : base(message, inner)
        {
            this.key = key;
            this.modifiers = modifiers;
        }
    }
}
