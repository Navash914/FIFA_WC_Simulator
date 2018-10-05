using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fifa_World_Cup_Simulator
{
    public class Team : IComparable<Team>
    {
        public string name { get; set; }
        public string initial;
        public int rank;
        public int rating { get; private set; }
        public List<Player> players = new List<Player>();
        public int goals = 0;
        public int conceded = 0;
        public int played = 0;
        public int wins = 0;
        public int losses = 0;
        public int draws = 0;
        public int pts = 0;

        public Team(string name, string initial, int rank, List<Player> players)
        {
            this.name = name;
            this.initial = initial;
            this.rank = rank;
            this.players = players;
            this.rating = TotalRating();
            DataContainer.teamList.Add(this);
        }

        public Team(string name, string initial, int rank, string[] players)
        {
            this.name = name;
            this.initial = initial;
            this.rank = rank;
            AssignPlayers(players);
            this.rating = TotalRating();
            DataContainer.teamList.Add(this);
        }

        public Team(string name, string initial, int rank)
        {
            this.name = name;
            this.initial = initial;
            this.rank = rank;
            DataContainer.teamList.Add(this);
        }

        public int CompareTo(Team other)
        {
            if (this.pts > other.pts) return -1;
            if (this.pts < other.pts) return 1;
            if (this.goalDifference() > other.goalDifference()) return -1;
            if (this.goalDifference() < other.goalDifference()) return 1;
            if (this.goals > other.goals) return -1;
            if (this.goals < other.goals) return 1;
            if (this.rank < other.rank) return -1;
            return 1;
        }

            private void AssignPlayers(string[] players)
        {
            string jsonStr = File.ReadAllText("Tournament_Data\\Players.json");
            dynamic jObj = JsonConvert.DeserializeObject(jsonStr);
            for (int i = 0; i < 11; i++)
            {
                string name = players[i];
                dynamic player = jObj[name];
                int rating = player.rating;
                Position pos = (Position)System.Enum.Parse(typeof(Position), player["position"].ToString());
                Player newPlayer = new Player(name, pos, rating);
                this.players.Add (newPlayer);
            }
        }

        private int TotalRating()
        {
            int sum = 0;
            foreach (Player p in players)
                sum += p.rating;
            return sum;
        }

        public int GoalRating()
        {
            int sum = 0;
            foreach (Player p in players)
                sum += p.rating * (int)p.position;
            return sum;
        }

        public double AverageRating()
        {
            return (double)rating / 11;
        }

        public double AverageAttack()
        {
            double sum = 0.0;
            int fwd = 0, md = 0;
            foreach (Player p in players)
            {
                if (p.position == Position.Defender) continue;
                if (p.position == Position.Gk) continue;
                if (p.position == Position.Midfielder)
                {
                    sum += (double)p.rating / 2.0;
                    md++;
                }
                if(p.position == Position.Forward)
                {
                    sum += (double)p.rating;
                    fwd++;
                }
            }
            double tot = (double)md * 0.5 + (double)fwd;
            return sum / tot;
        }

        public double AverageDefence()
        {
            double sum = 0.0;
            int dfd = 0, md = 0;
            foreach (Player p in players)
            {
                if (p.position == Position.Forward) continue;
                if (p.position == Position.Gk) sum += (double)p.rating * 1.5;
                if (p.position == Position.Midfielder)
                {
                    sum += (double)p.rating / 2.0;
                    md++;
                }
                if (p.position == Position.Defender)
                {
                    sum += (double)p.rating;
                    dfd++;
                }
            }
            double tot = (double)md * 0.5 + (double)dfd + 1.5;
            return sum / tot;
        }

        public int goalDifference()
        {
            return goals - conceded;
        }

    }

    public class Player
    {
        public string name { get; set; }
        public Position position;
        public Team team;
        public int rating;
        public int played = 0;
        public int goals = 0;
        public int p_goals = 0;
        public int assists = 0;

        public Player(string name, Position position, int rating)
        {
            this.name = name;
            this.position = position;
            this.rating = rating;
            DataContainer.playerList.Add(this);
        }

        public Player(string name, Position position, int rating, Team team)
        {
            this.name = name;
            this.position = position;
            this.rating = rating;
            this.team = team;
            team.players.Add(this);
            DataContainer.playerList.Add(this);
        }

        public void AssignTeam(Team team)
        {
            this.team = team;
            team.players.Add(this);
        }
    }
}
