using TaleWorlds.MountAndBlade;

namespace BLMMX.ChatCommands
{
    public abstract class ChatCommand
    {
        public abstract string CommandText { get; }
        
        public abstract string Description { get; }
        
        /// <summary>
        /// Whether the player attempting to execute the command is allowed to do so - should check for things like
        /// admin status and maybe other factors (game mode etc.) depending on whats needed for the individual command.
        /// To be called BEFORE Execute(). 
        /// </summary>
        /// <param name="executor">The player attempting to execute the command.</param>
        /// <returns>true if player is allowed to execute the command</returns>
        public abstract bool CanExecute(NetworkCommunicator executor);

        /// <summary>
        /// Actual execution of the command with the arguments provided to it.
        /// </summary>
        /// <param name="executor">The player executing the command.</param>
        /// <param name="args">Arguments passed into the command by the user.</param>
        /// <returns>true if the command was executed successfully, false if not</returns>
        public abstract bool Execute(NetworkCommunicator executor, string args);
    }
}