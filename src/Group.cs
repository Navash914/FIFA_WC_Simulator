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
    public class Group
    {
        public string name { get; private set; }
        public Team[] teams { get; private set; }
        Game[] matches = new Game[6];

        public Group(string name, Team[] teams)
        {
            this.name = name;
            this.teams = teams;
            DataContainer.groupList.Add(this);
            PrepareMatches();
        }

        public Group (string name, string[] teams)
        {
            this.name = name;
            AssignTeams(teams);
            DataContainer.groupList.Add(this);
            PrepareMatches();
        }

        private void AssignTeams(string[] t)
        {
            this.teams = new Team[4];
            string jsonStr = File.ReadAllText("Tournament_Data\\Teams.json");
            dynamic jObj = JsonConvert.DeserializeObject(jsonStr);
            for (int i = 0; i < t.Length; i++)
            {
                dynamic team = jObj[t[i]];
                if (team == null) break;
                string initial = team.initial;
                int rank = team.rank;
                Team newTeam = new Team(t[i], initial, rank);
                this.teams[i] = newTeam;
            }
        }

        private void PrepareMatches()
        {
            Random random = new Random();
            matches[0] = new Game(teams[0], teams[1], new Random(random.Next() * random.Next()), true);
            matches[1] = new Game(teams[2], teams[3], new Random(random.Next() * random.Next()), true);
            matches[2] = new Game(teams[0], teams[2], new Random(random.Next() * random.Next()), true);
            matches[3] = new Game(teams[1], teams[3], new Random(random.Next() * random.Next()), true);
            matches[4] = new Game(teams[0], teams[3], new Random(random.Next() * random.Next()), true);
            matches[5] = new Game(teams[1], teams[2], new Random(random.Next() * random.Next()), true);
        }

        public void PlayMatches()
        {
            foreach(Game game in matches)
            {
                game.SimulateGame();
            }

            List<Team> temp = teams.ToList();
            temp.Sort();
            teams = temp.ToArray();
        }

        public void DrawGroupTable(int spaces = 17)
        {
            Program.ChangeTextColor((ConsoleColor.Yellow));
            string space = new string(' ', spaces);
            int nameWidth = 19;
            int playWidth = 10;
            int wdlWidth = 10;
            int goalsWidth = 5;
            int ptsWidth = 10;
            int maxWidth = nameWidth + playWidth + wdlWidth * 3 + goalsWidth * 3 + ptsWidth;
            string horzLine = space + " " + new string('-', maxWidth-1);
            Console.WriteLine(horzLine);
            Console.Write(space);
            DrawTextWithBorder("Group " + this.name + ":", maxWidth, true);
            Console.WriteLine(horzLine);
            Console.Write(space);
            DrawTextWithBorder("Name", nameWidth);
            DrawTextWithBorder("Played", playWidth);
            DrawTextWithBorder("Wins", wdlWidth);
            DrawTextWithBorder("Draws", wdlWidth);
            DrawTextWithBorder("Losses", wdlWidth);
            DrawTextWithBorder("GF", goalsWidth);
            DrawTextWithBorder("GA", goalsWidth);
            DrawTextWithBorder("GD", goalsWidth);
            DrawTextWithBorder("Points", ptsWidth, true);
            Console.WriteLine(horzLine);
            int pos = 0;
            foreach(Team team in teams)
            {
                if (pos < 2) Program.ChangeTextColor((ConsoleColor.Green));
                else Program.ChangeTextColor((ConsoleColor.Red));
                Console.Write(space);
                DrawTextWithBorder(team.name, nameWidth);
                DrawTextWithBorder(team.played.ToString(), playWidth);
                DrawTextWithBorder(team.wins.ToString(), wdlWidth);
                DrawTextWithBorder(team.draws.ToString(), wdlWidth);
                DrawTextWithBorder(team.losses.ToString(), wdlWidth);
                DrawTextWithBorder(team.goals.ToString(), goalsWidth);
                DrawTextWithBorder(team.conceded.ToString(), goalsWidth);
                DrawTextWithBorder(team.goalDifference().ToString(), goalsWidth);
                DrawTextWithBorder(team.pts.ToString(), ptsWidth, true);
                pos++;
            }
            Program.ChangeTextColor((ConsoleColor.Yellow));
            Console.WriteLine(horzLine);
        }

        public void DrawTextWithBorder(string text, int width, bool newLine = false)
        {
            text = "| " + text;
            Console.Write(text + new string(' ', width - text.Length));
            if (newLine) Console.Write("|\n");
        }
    }
}
