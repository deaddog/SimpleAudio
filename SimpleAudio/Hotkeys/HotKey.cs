using System;
using System.Collections.Generic;
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
            private List<Action> actions;

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

            public void AddAction(Action action)
            {
                actions.Add(action);
            }
            public bool RemoveAction(Action action)
            {
                return actions.Remove(action);
            }

            public int Actions
            {
                get { return actions.Count; }
            }
            public void Execute()
            {
                Action[] ac = actions.ToArray();
                foreach (Action a in ac)
                    a();
            }

            public HotKey(int id, Key key, ModifierKeys modifiers)
            {
                this.id = id;
                this.key = key;
                this.modifiers = modifiers;
                this.actions = new List<Action>();
            }
        }
    }
}
