using System;
using System.Windows.Input;

namespace SimpleAudio.Hotkeys
{
    public partial class HotKeyManager
    {
        private class HotKey
        {
            private int id;
            private Key key;
            private ModifierKeys modifiers;

            public int Id
            {
                get { return id; }
            }
            public Key Key
            {
                get { return key; }
            }
            public ModifierKeys Modifiers
            {
                get { return modifiers; }
            }

            public HotKey(int id, Key key, ModifierKeys modifiers)
            {
                this.id = id;
                this.key = key;
                this.modifiers = modifiers;
            }
        }
    }
}
