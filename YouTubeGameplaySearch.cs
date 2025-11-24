using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace YouTubeGameplaySearch
{
    public class YouTubeGameplaySearch : GenericPlugin
    {
        public override Guid Id { get; } = Guid.Parse("E3C64309-3175-4740-8802-18118001602B");

        public YouTubeGameplaySearch(IPlayniteAPI api) : base(api)
        {
            Properties = new GenericPluginProperties
            {
                HasSettings = false
            };
        }

        public override IEnumerable<GameMenuItem> GetGameMenuItems(GetGameMenuItemsArgs args)
        {
            string desc = null;
            try
            {
                desc = PlayniteApi.Resources.GetString("YouTubeGameplaySearch_SearchYouTube");
            }
            catch
            {
                // ignore and fallback
            }

            if (string.IsNullOrEmpty(desc))
            {
                desc = "Search YouTube for Gameplay";
            }
            
            string pluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iconPath = Path.Combine(pluginFolder, "icon_32.png");
            yield return new GameMenuItem
            {
                Description = desc,
                Icon = Path.Combine(pluginFolder, "icon_32.png"), 
                Action = (actionArgs) =>
                {
                    foreach (var game in actionArgs.Games)
                    {
                        SearchYouTube(game.Name);
                    }
                }
            };
        }

        private void SearchYouTube(string gameName)
        {
            try
            {
                var query = Uri.EscapeDataString(gameName + " gameplay");
                var url = $"https://www.youtube.com/results?search_query={query}";

                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception e)
            {
                PlayniteApi.Dialogs.ShowErrorMessage(
                    $"Failed to open browser: {e.Message}", 
                    "Extension Error"
                );
            }
        }
    }
}