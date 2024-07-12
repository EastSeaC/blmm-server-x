using PlayerMessageAll = NetworkMessages.FromClient.PlayerMessageAll;
using PlayerMessageTeam = NetworkMessages.FromClient.PlayerMessageTeam;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using BLMMX.ChatCommands;
using BLMMX.Helpers;

namespace BLMMX.Handler
{
    internal class ESHandler : GameHandler
    {
        private DoFConfigOptions _configOptions = DoFConfigOptions.Instance;
        public override void OnAfterSave()
        {

        }

        public override void OnBeforeSave()
        {

        }

        private bool HandleClientEventPlayerMessageAll(NetworkCommunicator peer, PlayerMessageAll message)
        {
            return HandleChatMessage(peer, message.Message);
        }

        private bool HandleChatMessage(NetworkCommunicator sender, string message)
        {
            if (!message.StartsWith(_configOptions.CommandPrefix))
                return true; // don't hide, show in chat

            ChatCommandHandler.Instance.ExecuteCommand(sender, message);

            return false; // "hide" message from other MessageHandlers, making it not show up in chat for players
        }

        private bool HandleClientEventPlayerMessageTeam(NetworkCommunicator peer, PlayerMessageTeam message)
        {
            return HandleChatMessage(peer, message.Message);
        }

        protected override void OnGameNetworkBegin()
        {
            base.OnGameNetworkBegin();
            Helper.Print("Registering Chat handlers...");
            AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
        }

        protected override void OnGameNetworkEnd()
        {
            base.OnGameNetworkEnd();
            Helper.Print("Unregistering Chat handlers...");
            AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
        }

        private void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
        {
            if (!GameNetwork.IsServer)
                return;
            GameNetwork.NetworkMessageHandlerRegistererContainer container = new();
            //GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new(mode);
            container.Register<PlayerMessageAll>(HandleClientEventPlayerMessageAll);
            container.Register<PlayerMessageTeam>(HandleClientEventPlayerMessageTeam);

            container.RegisterMessages();
        }
    }
}
