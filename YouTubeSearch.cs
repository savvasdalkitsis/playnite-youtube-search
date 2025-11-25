using Playnite.SDK;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace YouTubeSearch
{
    public class YouTubeSearch : GenericPlugin
    {
        public override Guid Id { get; } = Guid.Parse("E3C64309-3175-4740-8802-18118001602B");

        public YouTubeSearch(IPlayniteAPI api) : base(api)
        {
            Properties = new GenericPluginProperties
            {
                HasSettings = false
            };
        }

        public override IEnumerable<GameMenuItem> GetGameMenuItems(GetGameMenuItemsArgs args)
        {
            string descGameplay = GetDescription("YouTubeSearchSearchYouTubeGameplay", "Search YouTube for Gameplay");
            string descTrailer = GetDescription("YouTubeSearchSearchYouTubeTrailer", "Search YouTube for Trailer");
            
            yield return GetMenuItem(descTrailer, "trailer");
            yield return GetMenuItem(descGameplay, "gameplay");
        }

        private string GetDescription(string key, string fallback)
        {
            string desc = null;
            try
            {
                desc = PlayniteApi.Resources.GetString(key);
            }
            catch
            {
                // ignore and fallback
            }
            if (string.IsNullOrEmpty(desc) || (desc.StartsWith("<!") && desc.EndsWith("!>")))
            {
                desc = fallback;
            }
            return desc;
        }

        private GameMenuItem GetMenuItem(string desc, string keyword)
        { 
            string pluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return new GameMenuItem
            {
                Description = desc,
                Icon = Path.Combine(pluginFolder, "icon_32.png"), 
                Action = (actionArgs) =>
                {
                    foreach (var game in actionArgs.Games)
                    {
                        SearchYouTube(game.Name, keyword);
                    }
                }
            };
        }


        private void SearchYouTube(string gameName, string keyword)
        {
            try
            {
                var query = Uri.EscapeDataString(gameName + " " + keyword);
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