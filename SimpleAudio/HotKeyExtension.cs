using SimpleAudio.Hotkeys;
using System;
using System.Windows.Input;

namespace SimpleAudio
{
    public static class HotKeyExtension
    {
        public static void AddHotKey(this HotKeyManager manager, Key key, ModifierKeys modifiers, Func<ICommand> commandRetriever)
        {
            manager.AddHotKey(key, modifiers, () => ExecuteCommand(commandRetriever()));
        }

        private static void ExecuteCommand(ICommand command)
        {
            if (command == null || !command.CanExecute(null))
                return;
            else
                command.Execute(null);
        }
    }
}
