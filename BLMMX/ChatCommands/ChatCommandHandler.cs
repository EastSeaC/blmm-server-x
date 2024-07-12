using BLMMX;
using BLMMX.Helpers;
using TaleWorlds.MountAndBlade;

namespace BLMMX.ChatCommands;

public class ChatCommandHandler
{
    public static ChatCommandHandler Instance { get; } = new ChatCommandHandler();

    private readonly Dictionary<string, ChatCommand> _registeredCommands = new();

    private DoFConfigOptions _configOptions = DoFConfigOptions.Instance;

    public List<ChatCommand> Commands => _registeredCommands.Values.ToList();

    public bool RegisterCommand(ChatCommand command)
    {
        if (_registeredCommands.ContainsKey(command.CommandText))
            return false;

        _registeredCommands.Add(command.CommandText, command);

        Helper.Print($"Registered Command {command.CommandText}");
        return true;
    }

    public bool ExecuteCommand(NetworkCommunicator executor, string command)
    {
        if (executor == null || string.IsNullOrWhiteSpace(command))
            return false;

        int prefixLength = _configOptions.CommandPrefix.Length;

        // ignore if it's just the prefix
        if (command.Length == prefixLength)
            return false;

        // cut off arguments to find command name only
        int firstWhiteSpaceIndex = command.IndexOf(' ');
        string commandName = firstWhiteSpaceIndex == -1
            ? command.Substring(prefixLength)
            : command.Substring(prefixLength, firstWhiteSpaceIndex - 1);

        if (!_registeredCommands.TryGetValue(commandName, out ChatCommand chatCommand)
            || !chatCommand.CanExecute(executor))
        {
            Helper.SendMessageToPeer(executor,
                "That command does not exist or you are not allowed to use it right now.");
            return false;
        }

        string args = command.Substring(prefixLength + commandName.Length).Trim();
        return chatCommand.Execute(executor, args);
    }
}