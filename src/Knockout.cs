using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fifa_World_Cup_Simulator
{
    public class Knockout
    {
        public string name;
        public List<Team> teams;
        public List<Team> winners = new List<Team>();
        public List<Team> losers = new List<Team>();
        public List<Game> games = new List<Game>();

        public Knockout(string nm, List<Team> t)
        {
            name = nm;
            teams = t;
            DataContainer.koRounds.Add(this);
            DrawHeader();
        }

        public void PlayMatches()
        {
            Random rand = new Random();
            for (int i=0; i<teams.Count; i += 2)
            {
                Game game = new Game(teams[i], teams[i + 1], new Random(rand.Next() * rand.Next()));
                games.Add(game);
                game.SimulateGame();
                game.DisplayGame();
                winners.Add(game.winner);
                losers.Add(game.loser);
            }
        }

        public void DrawHeader()
        {
            Program.PrintBoxedText(this.name);
        }
    }
}
