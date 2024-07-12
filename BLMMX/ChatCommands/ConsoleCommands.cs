//using System;
//using System.IO;
//using ZemServer2.Helpers;
//using JetBrains.Annotations;
//using TaleWorlds.MountAndBlade;
//using TaleWorlds.PlayerServices;

//namespace ZemServer2;

//public static class ConsoleCommands
//{
//    private static DoFConfigOptions _configOptions = DoFConfigOptions.Instance;

//    //[UsedImplicitly]
//    //[ConsoleCommandMethod("dat_add_admin",
//    //    "Add the ID of a player to be given admin permissions upon login, without using the admin password")]
//    //private static void AddAdminCommand(string adminId)
//    //{
//    //    Helper.Print("Trying to add admin " + adminId);

//    //    var adminRepo = AdminRepository.Instance;

//    //    // TODO verify the given adminId is an actual playerId
//    //    adminRepo.AddAdmin(adminId);
//    //}
//    //private static readonly Action<string> HandleConsoleCommand =
//    //(Action<string>)Delegate.CreateDelegate(typeof(Action<string>),
//    //    typeof(DedicatedServerConsoleCommandManager).GetStaticMethodInfo("HandleConsoleCommand"));

//    [UsedImplicitly]
//    [ConsoleCommandMethod("等级", "查看玩家等级")]
//    private static void ShowPlayerCommand()
//    {

//    }



//    [UsedImplicitly]
//    [ConsoleCommandMethod("dat_include",
//        "Include another config file to be parsed as well. Useful for data shared between multiple configurations.")]
//    private static void IncludeConfigFileCommand(string configFileName)
//    {
//        Helper.Print($"Trying to include file {configFileName}");
//        string nativeModulePath = SubModule.NativeModulePath;

//        string fullTargetPath = Path.GetFullPath(Path.Combine(nativeModulePath, configFileName));

//        if (!fullTargetPath.StartsWith(nativeModulePath))
//        {
//            Helper.PrintError(
//                $"\tGiven Path ({configFileName}) leads to location ({fullTargetPath}) outside of your Modules/Native/ directory ({nativeModulePath}), therefore it is not included.");
//            return;
//        }

//        if (!File.Exists(fullTargetPath))
//        {
//            Helper.PrintError($"\tGiven File ({fullTargetPath}) does not exist.");
//            return;
//        }

//        Helper.Print("\tReading config file " + fullTargetPath);
        
//        string[] lines = File.ReadAllLines(fullTargetPath);

//        foreach (string currentLine in lines)
//        {
//            if (!currentLine.StartsWith("#") && !string.IsNullOrWhiteSpace(currentLine))
//            {
//                HandleConsoleCommand(currentLine);
//            }
//        }
        
//        Helper.Print("\tDone reading config file " + fullTargetPath);
//    }

//    [UsedImplicitly]
//    [ConsoleCommandMethod("dat_set_command_prefix",
//        "Set the prefix for ingame chat commands. Note that '/' will not work.")]
//    private static void SetCommandPrefixCommand(string prefix)
//    {
//        if (string.IsNullOrWhiteSpace(prefix))
//        {
//            Helper.PrintError("No prefix provided for dat_set_command_prefix");
//            return;
//        }
        
//        prefix = prefix.Trim();

//        if (prefix.StartsWith("/"))
//        {
//            Helper.PrintError("dat_set_command_prefix: Can't set prefix starting with '/'.");
//            return;
//        }

//        _configOptions.CommandPrefix = prefix;
//        Helper.Print($"Set command prefix to {prefix}");
//    }

//    [UsedImplicitly]
//    [ConsoleCommandMethod("dat_set_show_joinleave_messages",
//        "[True/False] Set whether to show messages in chat when someone joins or leaves the server.")]
//    private static void SetShowJoinLeaveMessagesCommand(string show)
//    {
//        if (!bool.TryParse(show, out bool showMessages))
//        {
//            Helper.PrintError($"dat_set_show_joinleave_messages: Could not parse boolean (True/False) from '{show}'");
//            return;
//        }

//        _configOptions.ShowJoinLeaveMessages = showMessages;
        
//        Helper.Print($"Set ShowJoinLeaveMessages to {showMessages}");
//    }

//    [UsedImplicitly]
//    [ConsoleCommandMethod("dat_set_and_load_banlist",
//        "Set the ban list file, then load all bans contained in the file if it exists.")]
//    private static void SetAndLoadBanlistCommand(string banListPath)
//    {
//        Helper.Print($"Trying to load ban list file {banListPath}");
//        string nativeModulePath = DoFSubModule.NativeModulePath;

//        string fullTargetPath = Path.GetFullPath(Path.Combine(nativeModulePath, banListPath));

//        if (!fullTargetPath.StartsWith(nativeModulePath))
//        {
//            Helper.PrintError(
//                $"\tGiven Path ({banListPath}) leads to location ({fullTargetPath}) outside of your Modules/Native/ directory ({nativeModulePath}), therefore it can not be loaded.");
//            return;
//        }

//        _configOptions.BanListFileName = banListPath;
//        Helper.Print($"\tSet BanListFileName to {banListPath}");

//        if (!File.Exists(fullTargetPath))
//        {
//            Helper.PrintWarning($"\tNo ban list file found at path {fullTargetPath}. Path will be used for new bans but no existing bans are loaded right now.");
//            return;
//        }
        
//        Helper.Print("\tLoading ban list from " + fullTargetPath);

//        string[] lines = File.ReadAllLines(fullTargetPath);

//        foreach (string line in lines)
//        {
//            var currentLine = line.Trim();
            
//            if (currentLine.StartsWith("#"))
//                continue;

//            int commentSignIndex = currentLine.IndexOf("#", StringComparison.Ordinal);

//            if (commentSignIndex != -1)
//                currentLine = currentLine.Substring(0, commentSignIndex).TrimEnd();
//            try
//            {
//                PlayerId playerId = PlayerId.FromString(currentLine);
//                CustomGameBannedPlayerManager.AddBannedPlayer(playerId, int.MaxValue);
//            }
//            catch (FormatException ex)
//            {
//                Helper.PrintError($"\tCould not parse {currentLine} as a PlayerId, skipping.");
//            }
//        }
        
//        Helper.Print("\tDone reading ban list");
//    }
//}