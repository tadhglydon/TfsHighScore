using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace SourceControlHighScore
{
    public partial class HighScore : System.Web.UI.Page
    {
        private const string TFS_URI = "tfsUri";
        private const string TFS_DOMAIN = "tfsDomain";
        private const string TFS_USER = "tfsUsername";
        private const string TFS_PASS = "tfsPassword";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            //Move to config
            var tfsUri = new Uri(ConfigurationManager.AppSettings[TFS_URI]);
            var tfsCred = new NetworkCredential(
                ConfigurationManager.AppSettings[TFS_USER],
                ConfigurationManager.AppSettings[TFS_PASS],
                ConfigurationManager.AppSettings[TFS_DOMAIN]
                );

            var configurationServer = new TfsConfigurationServer(tfsUri,tfsCred);

            ReadOnlyCollection<CatalogNode> collectionNodes = configurationServer.CatalogNode.QueryChildren(
                new[] { CatalogResourceTypes.ProjectCollection },
                false, CatalogQueryOptions.None);

            var players = new List<Player>();

            // List the team project collections
            foreach (CatalogNode collectionNode in collectionNodes)
            {
                Guid tpcId = new Guid(collectionNode.Resource.Properties["InstanceId"]);
                TfsTeamProjectCollection tpc = configurationServer.GetTeamProjectCollection(tpcId);
                var vcs = tpc.GetService<VersionControlServer>();
                var srcRoot = vcs.GetItem("$/");

                IEnumerable<PendingSet> pendings = vcs.QueryPendingSets(new[] { srcRoot.ServerItem }, RecursionType.Full, null, null).AsEnumerable();

                foreach (var pending in pendings)
                {
                    Player player;
                    if (players.Exists(x => x.Name == pending.OwnerDisplayName))
                    {
                        player = players.Find(x => x.Name == pending.OwnerDisplayName);
                    }
                    else
                    {
                        player = new Player(pending.OwnerDisplayName);
                        players.Add(player);
                    }
                    player.Score += pending.PendingChanges.Count();
                }
            }

            NamesList.Text = GetNamesHtml(players);
        }

        private static string GetNamesHtml(IEnumerable<Player> players)
        {
            var table = new StringBuilder();
            int i = 0;
            var colours = new Colours();
            foreach (var player in players.OrderByDescending(p => p.Score).ToList())
            {
                string colour = colours.GetNextColour();
                i++;
                table.AppendLine(string.Format(
                    "<p><span title=\"{0}\"><font color=\"{1}\">{2} {3} {4}</font></span></p>",
                    player.Name,
                    colour,
                    i,
                    player.Nickname,
                    player.GetScore()));
            }
            return table.ToString();
        }
    }

    internal class Player : IEquatable<Player>
    {
        private readonly string _name;

        public bool Equals(Player other)
        {
            if (other == null) return false;
            return (Name.Equals(other.Name));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var objAsPart = obj as Player;
            if (objAsPart == null) return false;
            return Equals(objAsPart);
        }

        public Player(string name)
        {
            _name = name;
            Score = 0;
        }
        public string Name
        {
            get { return _name; }
        }

        public string Nickname
        {
            get
            {
                string nickname;
                var name = _name.Replace("(Williams Lea GB)", "").Trim();
                var space = name.Split(' ');
                if (space.Length == 3)
                {
                    nickname = space[0].Substring(0, 1) + space[1].Substring(0, 1) + space[2].Substring(0, 1);
                }
                else if (space.Length == 2)
                {
                    if (space[1].Contains("-"))
                    {
                        var dash = space[1].Split('-');
                        nickname = space[0].Substring(0, 1) + space[1].Substring(0, 1) + dash[1].Substring(0, 1);
                    }
                    else
                        nickname = space[0].Substring(0, 1) + space[1].Substring(0, 2);
                }
                else
                {
                    nickname = name.Substring(0, 3);
                }
                return nickname;
            }
        }

        public string GetScore()
        {
            return Score.ToString(CultureInfo.InvariantCulture);
        }

        public int Score
        {
            set; get;
        }
    }

    internal class Colours
    {
        private int _currentColour;

        public Colours()
        {
            _currentColour = 0;
        }

        public string GetNextColour()
        {
            var returnColour = _colourNames[_currentColour];
            _currentColour = _currentColour == _colourNames.Length - 1 ? 0 : _currentColour + 1;
            return returnColour;
        }

        private readonly string[] _colourNames = { "red", "orange", "yellow", "lime", "cyan", "blue", "purple" };

    }
}