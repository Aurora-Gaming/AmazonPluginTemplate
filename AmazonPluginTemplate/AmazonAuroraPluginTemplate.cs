using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace PluginTemplate
{
    [ApiVersion(2, 1)]
    public class AmazonAuroraPluginTemplate : TerrariaPlugin
    {
        public override string Author => "Zaicon";
        public override string Description => "Best coded template plugin in... Aurora.";
        public override string Name => "AmazonAuroraPluginTemplate";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public AmazonAuroraPluginTemplate(Main game) : base(game) { }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args)
        {
            DatabaseManager.Connect();

            Commands.ChatCommands.RemoveAll(e => e.Name == "help");
            Commands.ChatCommands.Add(new Command("permission.here", CustomHelpCmdMethod, "help") { AllowServer = false });
        }

        private void CustomHelpCmdMethod(CommandArgs args)
        {
            args.Player.KillPlayer();

            if (!args.Player.IsLoggedIn)
            {
                TShock.Utils.Broadcast($"{args.Player.Name} is very un-special and they can't count. Their IP is {args.Player.IP}.", new Color(155, 155, 155));
            }
            else
            {
                var cmdCount = DatabaseManager.IncreaseHelpCmdCount(args.Player.Account.ID);
                TShock.Utils.Broadcast($"{args.Player.Name} is logged in as {args.Player.Account.Name} and their IP is {args.Player.IP}.", new Color(155, 155, 155));
                TSPlayer.All.SendInfoMessage($"{args.Player.Name} has died {cmdCount} times now.");
                TSPlayer.All.SendMessageFromPlayer("i hate u all, down with aurora, long live amazon!!11!11!!!!", args.Player.Group.R, args.Player.Group.G, args.Player.Group.B, args.Player.Index);
            }
        }
    }
}
