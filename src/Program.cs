using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fifa_World_Cup_Simulator
{
    class Program
    {
        public static bool useColor = false;
        public static Team[] finalPositions;
        public static string sp = new string(' ', 10);

        static void Main(string[] args)
        {
            Console.Title = "Fifa World Cup 2018 Simulator";
            int maxSims;
            while (true)
            {
                Console.Write("Use text color variety? ('y' for yes, 'n' for no) ");
                string response = Console.ReadLine().ToUpper();
                if (response == "Y" || response == "T" ||
                    response == "YES" || response == "TRUE")
                {
                    useColor = true;
                    break;
                } else if (response == "N" || response == "F" ||
                    response == "NO" || response == "FALSE")
                {
                    useColor = false;
                    break;
                } else
                {
                    Console.WriteLine("Please Enter A Valid Response");
                }
            }
            while (true)
            {
                Console.Write("How many simulations? ");
                int num_of_sims;
                string input = Console.ReadLine();
                if (int.TryParse(input, out num_of_sims) && num_of_sims > 0)
                {
                    maxSims = num_of_sims;
                    break;
                }
                else Console.WriteLine("Please Input A Valid Number");
            }

            int sim_num = 0;
            while (sim_num++ < maxSims)
            {
                PrepareTournament(sim_num);
                GroupStage();
                KnockoutStage();
                EndTournament(maxSims);
            }
            EndSimulation(maxSims);

            Console.WriteLine("\nPress Enter to Continue...");
            Console.ReadLine();
            return;

            if (true)
            {
                Player[] brazilPlayers = CreateBrazilTeam();
                Player[] belgiumPlayers = CreateBelgiumTeam();

                Team brazil = new Team("Brazil", "BRA", 2, brazilPlayers.ToList());
                Team belgium = new Team("Belgium", "BEL", 3, belgiumPlayers.ToList());

                int numOfSims = 1000;
                int goals = 0, t1goals = 0, t2goals = 0;
                Random random = new Random();

                for (int i = 0; i < numOfSims; i++)
                {

                    Game game = new Game(brazil, belgium, random);
                    game.SimulateGame();

                    goals += game.goals.Count;
                    t1goals += game.score[0];
                    t2goals += game.score[1];

                    PrintGameDetails(game);

                    Console.WriteLine("\nSimulation Results: \n");
                    Console.WriteLine("Games Played: " + numOfSims);
                    Console.WriteLine(brazil.name + " wins: " + brazil.wins);
                    Console.WriteLine(belgium.name + " wins: " + belgium.wins);
                    Console.WriteLine(brazil.draws + " draw games.");
                    Console.WriteLine();
                    Console.WriteLine("Total Goals Scored: " + goals);
                    Console.WriteLine("Goals Scored By Brazil: " + t1goals);
                    Console.WriteLine("Goals Scored By Belgium: " + t2goals);

                    Console.WriteLine("\nPress Enter to Continue...");
                    Console.ReadLine();
                }
            }

        }

        public static void ChangeTextColor(ConsoleColor color) {
            if (!useColor) return;
            Console.ForegroundColor = color;
        }

        static void PrintGameDetails(Game game)
        {
            string div = "\n" + new string('=', 30) + "\n";
            Console.WriteLine(div);
            string t1 = game.teamA.name;
            string t2 = game.teamB.name;
            Console.WriteLine(t1 + " vs " + t2);
            Console.WriteLine("Score was " + t1 + " " + game.score[0] + " - " + game.score[1] + " " + t2);
            if (game.winner == null)
            {
                Console.WriteLine("Match ended in a draw!");
            }
            else
            {
                Console.WriteLine(game.winner.name + " won the game!");
            }
            if (game.goals.Count > 0)
            {
                Console.WriteLine("\nGoalscorers:\n");
                foreach (Goal goal in game.goals)
                {
                    int t = goal.time;
                    string time;
                    if (t <= 90)
                        time = t.ToString();
                    else
                        time = "90 + " + (t - 90).ToString();
                    if (goal.assister != null)
                        Console.WriteLine(time + "' - " + goal.goalScorer.name + " assisted by " + goal.assister.name);
                    else
                        Console.WriteLine(time + "' - " + goal.goalScorer.name);
                }
            }
            Console.WriteLine(div);
        }

        static Player[] CreateBrazilTeam()
        {
            Player[] team = new Player[11];
            team[0] = new Player("Neymar", Position.Forward, 93);
            team[1] = new Player("Gabriel Jesus", Position.Forward, 85);
            team[2] = new Player("Willian", Position.Forward, 85);
            team[3] = new Player("Philippe Coutinho", Position.Midfielder, 88);
            team[4] = new Player("Paulinho", Position.Midfielder, 87);
            team[5] = new Player("Casemiro", Position.Defender, 86);
            team[6] = new Player("Marcelo", Position.Defender, 87);
            team[7] = new Player("Thiago Silva", Position.Defender, 86);
            team[8] = new Player("Miranda", Position.Defender, 87);
            team[9] = new Player("Fagner", Position.Defender, 77);
            team[10] = new Player("Alisson", Position.Gk, 84);
            return team;
        }

        static Player[] CreateBelgiumTeam()
        {
            Player[] team = new Player[11];
            team[0] = new Player("Eden Hazard", Position.Forward, 90);
            team[1] = new Player("Romelu Lukaku", Position.Forward, 86);
            team[2] = new Player("Kevin De Bruyne", Position.Midfielder, 90);
            team[3] = new Player("Axel Witsel", Position.Midfielder, 83);
            team[4] = new Player("Mertens", Position.Midfielder, 86);
            team[5] = new Player("Moussa Dembele", Position.Midfielder, 82);
            team[6] = new Player("Carrasco", Position.Midfielder, 82);
            team[7] = new Player("Alderweireld", Position.Defender, 84);
            team[8] = new Player("Vincent Kompany", Position.Defender, 84);
            team[9] = new Player("Vertonghen", Position.Defender, 84);
            team[10] = new Player("Thibaut Courtois", Position.Gk, 88);

            /*team[0] = new Player("Eden Hazard", Position.Forward, 20);
            team[1] = new Player("Romelu Lukaku", Position.Forward, 18);
            team[2] = new Player("Kevin De Bruyne", Position.Midfielder, 5);
            team[3] = new Player("Axel Witsel", Position.Midfielder, 12);
            team[4] = new Player("Mertens", Position.Midfielder, 25);
            team[5] = new Player("Moussa Dembele", Position.Midfielder, 13);
            team[6] = new Player("Carrasco", Position.Midfielder, 21);
            team[7] = new Player("Alderweireld", Position.Defender, 18);
            team[8] = new Player("Vincent Kompany", Position.Defender, 16);
            team[9] = new Player("Vertonghen", Position.Defender, 17);
            team[10] = new Player("Thibaut Courtois", Position.Gk, 19);*/
            return team;
        }

        public static void PrepareTournament(int sim_num)
        {
            ChangeTextColor(ConsoleColor.Red);
            PrintBoxedText("!!! FIFA WORLD CUP RUSSIA 2018 !!!", 110, 2, 4);
            ChangeTextColor(ConsoleColor.White);
            PrintBoxedText("Simulation #" + sim_num, blankLines: 0);
            string jsonStr = File.ReadAllText("Tournament_Data\\Groups.json");
            dynamic jObj = JsonConvert.DeserializeObject(jsonStr);
            dynamic group = JsonConvert.DeserializeObject(jObj.groups.ToString());
            Team[] teams = new Team[4];
            for (int i = 0; i < 8; i++)
            {
                string name = group[i].name;
                string[] t = JsonConvert.DeserializeObject<string[]>(group[i].teams.ToString());
                new Group(name, t);
            }
            GeneratePlayers();
        }

        public static void GroupStage()
        {
            ChangeTextColor(ConsoleColor.Yellow);
            PrintBoxedText("Group Stage");
            foreach (Group g in DataContainer.groupList)
            {
                g.PlayMatches();
                g.DrawGroupTable();
            }
        }

        public static void KnockoutStage()
        {
            List<Team> ro16Teams = GetRO16Teams();
            ChangeTextColor(ConsoleColor.Cyan);
            Knockout ro16 = new Knockout("Round of 16", ro16Teams);
            ro16.PlayMatches();
            List<Team> qfTeams = GetQFTeams(ro16.winners);
            ChangeTextColor(ConsoleColor.Blue);
            Knockout qf = new Knockout("Quarter Finals", qfTeams);
            qf.PlayMatches();
            List<Team> sfTeams = GetSFTeams(qf.winners);
            ChangeTextColor(ConsoleColor.Yellow);
            Knockout sf = new Knockout("Semi Finals", sfTeams);
            sf.PlayMatches();
            List<Team> finalTeams = GetFinalTeams(sf.winners);
            List<Team> tpTeams = GetTPTeams(sf.losers);
            Knockout tp = new Knockout("Third Place Playoff", tpTeams);
            tp.PlayMatches();
            ChangeTextColor(ConsoleColor.Red);
            Knockout finals = new Knockout("!!! FINALS !!!", finalTeams);
            finals.PlayMatches();

            finalPositions = new Team[] { finals.winners[0], finals.losers[0], tp.winners[0], tp.losers[0] };
        }

        public static List<Team> GetRO16Teams()
        {
            List<Team> teams = new List<Team>();

            teams.Add(GroupChampion('C'));
            teams.Add(GroupRunnerUp('D'));

            teams.Add(GroupChampion('A'));
            teams.Add(GroupRunnerUp('B'));

            teams.Add(GroupChampion('B'));
            teams.Add(GroupRunnerUp('A'));

            teams.Add(GroupChampion('D'));
            teams.Add(GroupRunnerUp('C'));

            teams.Add(GroupChampion('E'));
            teams.Add(GroupRunnerUp('F'));

            teams.Add(GroupChampion('G')); 
            teams.Add(GroupRunnerUp('H'));

            teams.Add(GroupChampion('F'));
            teams.Add(GroupRunnerUp('E'));

            teams.Add(GroupChampion('H'));
            teams.Add(GroupRunnerUp('G'));

            return teams;
        }

        public static List<Team> GetQFTeams(List<Team> ro16Wins)
        {
            return ro16Wins;
        }

        public static List<Team> GetSFTeams(List<Team> qfWins)
        {
            List<Team> res = new List<Team>();
            res.Add(qfWins[0]);
            res.Add(qfWins[2]);
            res.Add(qfWins[1]);
            res.Add(qfWins[3]);
            return res;
        }

        public static List<Team> GetTPTeams(List<Team> teams)
        {
            return teams;
        }

        public static List<Team> GetFinalTeams(List<Team> teams)
        {
            return teams;
        }

        public static Team GroupChampion(char nm)
        {
            int n = (int)nm - (int)'A';
            return DataContainer.groupList[n].teams[0];
        }

        public static Team GroupRunnerUp(char nm)
        {
            int n = (int)nm - (int)'A';
            return DataContainer.groupList[n].teams[1];
        }

        public static void GeneratePlayers()
        {
            string jsonStr = File.ReadAllText("Tournament_Data\\Players.json");
            dynamic jObj = JsonConvert.DeserializeObject(jsonStr);
            foreach (JProperty prop in jObj.Properties())
            {
                dynamic obj = prop.Value;
                string t = obj.team;
                Team team = NameSearch<Team>(DataContainer.teamList, t);
                Position pos = (Position)System.Enum.Parse(typeof(Position), obj["position"].ToString());
                int rating = obj.rating;
                new Player(prop.Name, pos, rating, team);
            }
        }

        public static T NameSearch<T>(List<T> lst, string name)
        {
            foreach (T ele in lst)
            {
                string eleName = (string)ele.GetType().GetProperty("name").GetValue(ele);
                if (eleName == name) return ele;
            }
            return default(T);
        }

        public static void PrintBoxedText(string txt, int maxLen = 90, int blankLines = 1, int spaces = 14)
        {
            string horzLine = " " + new string('-', maxLen - 1);
            string blankLine = "/" + new string(' ', maxLen - 1) + "\\";
            string blankLine2 = "\\" + new string(' ', maxLen - 1) + "/";
            if (spaces > 0)
            {
                horzLine = new string(' ', spaces) + horzLine;
                blankLine = new string(' ', spaces) + blankLine;
                blankLine2 = new string(' ', spaces) + blankLine2;
            }
            int len = txt.Length;
            int start = (maxLen - len) / 2;
            string text = "<" + new string(' ', start - 1) + txt + new string(' ', maxLen - (start + len)) + "  >";
            if (spaces > 0) text = new string(' ', spaces - 1) + text;
            Console.WriteLine();
            Console.WriteLine(horzLine);
            for (int i = 0; i < blankLines; i++) Console.WriteLine(blankLine);
            Console.WriteLine(text);
            for (int i = 0; i < blankLines; i++) Console.WriteLine(blankLine2);
            Console.WriteLine(horzLine);
            Console.WriteLine();
        }

        public static void EndTournament(int maxSims)
        {
            if (maxSims < 6) DrawBrackets();
            ChangeTextColor(ConsoleColor.Green);
            Console.WriteLine(new string('=', 120));
            Console.WriteLine("\n" + sp + "Tournament Stats:\n");
            DisplayTop4();
            Console.WriteLine();
            DisplayTopGoals();
            Console.WriteLine();
            DisplayTopAssists();
            Console.WriteLine();
            DisplayStats();
            Console.WriteLine();
            Console.WriteLine(new string('=', 120));
            string name = finalPositions[0].name;
            if (!DataContainer.tournamentWinners.ContainsKey(name)) DataContainer.tournamentWinners.Add(name, 0);
            DataContainer.tournamentWinners[name]++;
            DataContainer.ClearData();
        }

        public static void EndSimulation(int maxSims)
        {
            const int showTopNum = 5;
            string border = new string('=', 120);
            ChangeTextColor(ConsoleColor.Magenta);
            PrintBoxedText("End Of Simulation", blankLines: 0);
            ChangeTextColor(ConsoleColor.White);
            Console.WriteLine(border);
            Console.WriteLine(border);
            Console.WriteLine("\n" + sp + "Simulation Stats:\n");
            Console.WriteLine(sp + "Number of Simulations Done: " + maxSims);
            Console.WriteLine();
            DisplayTopStats(showTopNum, DataContainer.tournamentWinners, "Top Performing Teams", "Win");
            Console.WriteLine();
            DisplayTopStats(showTopNum, DataContainer.goalScorers, "Top Goal Scorers", "Goal", maxSims);
            Console.WriteLine();
            DisplayTopStats(showTopNum, DataContainer.assistGivers, "Top Assist Givers", "Assist", maxSims);
            Console.WriteLine();
            Console.WriteLine(border);
            Console.WriteLine(border);
        }

        public static void DisplayTopStats(int n, Dictionary<string, int> dic, string header, string tail, int max = 0)
        {
            Console.WriteLine(sp + header + ":");
            var items = from pair in dic
                        orderby pair.Value descending
                        select pair;
            int count = 0;
            foreach (KeyValuePair<string, int> kvp in items)
            {
                if (count >= n) break;
                string name = kvp.Key;
                int val = kvp.Value;
                string text = sp + "  " + name + ": " + val + " " + tail;
                if (val > 1) text += "s";
                if (max > 0)
                {
                    double perc = (double)val / (double)max;
                    perc = Math.Round(perc, 2);
                    text += " ( " + perc + " " + tail;
                    if (perc != 1.00) text += "s";
                    text += " per tournament )";
                }
                Console.WriteLine(text);
                count++;
            }
        }

        public static void DrawBrackets()
        {
            string text = "\n" +
" -----------                                                                                                                            ----------- \n" +
" | {0}  | {1} | -----                                                                                                              ----- | {9} |  {8} | \n" +
" -----------       |                                                                                                            |       ----------- \n" +
"                   |                                                                                                            | \n" +
"                 -----------                                                                                            ----------- \n" +
"                 | {32}  | {33} | -----                                                                              ----- | {37} |  {36} | \n" +
"                 -----------       |                                                                            |       ----------- \n" +
"                   |               |                                                                            |               | \n" +
" -----------       |               |                                                                            |               |       ----------- \n" +
" | {2}  | {3} | -----                |                                                                            |                ----- | {11} |  {10} | \n" +
" -----------                       |                              !!! CHAMPIONS !!!                             |                       ----------- \n" +
"                                 -----------                          =========                         ----------- \n" +
"                                 | {48}  | {49} | -----                                              ----- | {53} |  {52} | \n" +
"                                 -----------       |                     {65}                    |       ----------- \n" +
" -----------                       |               |                                            |               |                       ----------- \n" +
" | {4}  | {5} | -----                |               |                                            |               |                ----- | {13} |  {12} | \n" +
" -----------       |               |               |                                            |               |               |       ----------- \n" +
"                   |               |               |                                            |               |               | \n" +
"                 -----------       |               |                                            |               |       ----------- \n" +
"                 | {34}  | {35} | -----                |                                            |                ----- | {39} |  {38} | \n" +
"                 -----------                       |                                            |                       ----------- \n" +
"                   |                               |                                            |                               | \n" +
" -----------       |                               |                                            |                               |       ----------- \n" +
" | {6}  | {7} | -----                                |                                            |                                ----- | {15} |  {14} | \n" +
" -----------                                       |                                            |                                       ----------- \n" +
"                                                 -----------                            ----------- \n" +
"                                                 | {60}  | {61} |          FINALS          | {63} |  {62} | \n" +
"                                                 -----------                            ----------- \n" +
" -----------                                       |                                            |                                       ----------- \n" +
" | {16}  | {17} | -----                                |                                            |                                ----- | {25} |  {24} | \n" +
" -----------       |                               |                                            |                               |       ----------- \n" +
"                   |                               |                                            |                               | \n" +
"                 -----------                       |                                            |                       ----------- \n" +
"                 | {40}  | {41} | -----                |                                            |                ----- | {45} |  {44} | \n" +
"                 -----------       |               |            Third Place Play-0ff            |               |       ----------- \n" +
"                   |               |               |       -----------        -----------       |               |               | \n" +
" -----------       |               |               |       | {56}  | {57} |      | {59} |  {58} |       |               |               |       ----------- \n" +
" | {18}  | {19} | -----                |               |       -----------        -----------       |               |                ----- | {27} |  {26} | \n" +
" -----------                       |               |                                            |               |                       ----------- \n" +
"                                 -----------       |                   Third                    |       ----------- \n" +
"                                 | {50}  | {51} | -----                      {64}                     ----- | {55} |  {54} | \n" +
"                                 -----------                                                            ----------- \n" +
" -----------                       |                                                                            |                       ----------- \n" +
" | {20}  | {21} | -----                |                                                                            |                ----- | {29} |  {28} | \n" +
" -----------       |               |                                                                            |               |       ----------- \n" +
"                   |               |                                                                            |               |  \n" +
"                 -----------       |                                                                            |       ----------- \n" +
"                 | {42}  | {43} | -----                                                                              ----- | {47} |  {46} | \n" +
"                 -----------                                                                                            ----------- \n" +
"                   |                                                                                                            | \n" +
" -----------       |                                                                                                            |       ----------- \n" +
" | {22}  | {23} | -----                                                                                                              ----- | {31} |  {30} | \n" +
" -----------                                                                                                                            ----------- \n" +
"\n";
            Knockout ro16 = DataContainer.koRounds[0];
            Knockout qf = DataContainer.koRounds[1];
            Knockout sf = DataContainer.koRounds[2];
            Knockout tp = DataContainer.koRounds[3];
            Knockout finals = DataContainer.koRounds[4];
            text = string.Format(text, ro16.teams[0].initial, ro16.games[0].score[0], ro16.teams[1].initial, ro16.games[0].score[1], 
                ro16.teams[2].initial, ro16.games[1].score[0], ro16.teams[3].initial, ro16.games[1].score[1],
                ro16.teams[4].initial, ro16.games[2].score[0], ro16.teams[5].initial, ro16.games[2].score[1], 
                ro16.teams[6].initial, ro16.games[3].score[0], ro16.teams[7].initial, ro16.games[3].score[1], 
                ro16.teams[8].initial, ro16.games[4].score[0], ro16.teams[9].initial, ro16.games[4].score[1], 
                ro16.teams[10].initial, ro16.games[5].score[0], ro16.teams[11].initial, ro16.games[5].score[1], 
                ro16.teams[12].initial, ro16.games[6].score[0], ro16.teams[13].initial, ro16.games[6].score[1],
                ro16.teams[14].initial, ro16.games[7].score[0], ro16.teams[15].initial, ro16.games[7].score[1],
                qf.teams[0].initial, qf.games[0].score[0], qf.teams[1].initial, qf.games[0].score[1],
                qf.teams[2].initial, qf.games[1].score[0], qf.teams[3].initial, qf.games[1].score[1], 
                qf.teams[4].initial, qf.games[2].score[0], qf.teams[5].initial, qf.games[2].score[1],
                qf.teams[6].initial, qf.games[3].score[0], qf.teams[7].initial, qf.games[3].score[1],
                sf.teams[0].initial, sf.games[0].score[0], sf.teams[1].initial, sf.games[0].score[1],
                sf.teams[2].initial, sf.games[1].score[0], sf.teams[3].initial, sf.games[1].score[1],
                tp.teams[0].initial, tp.games[0].score[0], tp.teams[1].initial, tp.games[0].score[1],
                finals.teams[0].initial, finals.games[0].score[0], finals.teams[1].initial, finals.games[0].score[1],
                tp.winners[0].initial, finals.winners[0].initial
                );
            ChangeTextColor(ConsoleColor.Magenta);
            Console.Write(text);
        }

        public string Spaces(int n)
        {
            return new string(' ', n);
        }

        public static void DisplayTop4()
        {
            Console.WriteLine(sp + "Top Teams:");
            string[] pos = new string[] { "Champions", "First Runner Up", "Second Runner Up", "Fourth Place" };
            int n = 40;
            for (int i = 0; i < finalPositions.Length; i++)
            {
                string text = sp + "  " + pos[i] + ":";
                text += new string(' ', n - text.Length);
                text += finalPositions[i].name;
                Console.WriteLine(text);
            }
        }

        public static void DisplayTopGoals()
        {
            DataContainer.playerList.Sort(new GoalSort());
            Console.WriteLine(sp + "Top Goalscorers:");
            int n = 50;
            for (int i = 0; i < 5; i++)
            {
                Player p = DataContainer.playerList[i];
                string nm = p.name;
                string text = sp + "  " + nm;
                if (i == 0) text += " (G.B. Winner)";
                text += " - ";
                text += new string(' ', n - text.Length);
                text += p.goals.ToString();
                string penText = p.p_goals == 1 ? " penalty" : " penalties";
                string astText = p.assists == 1 ? " assist" : " assists";
                text += " ( " + p.p_goals.ToString() + penText + " ) w/ " + p.assists.ToString() + astText;
                Console.WriteLine(text);
            }
        }

        public static void DisplayTopAssists()
        {
            DataContainer.playerList.Sort(new AssistSort());
            Console.WriteLine(sp + "Top Assist Givers:");
            int n = 50;
            for (int i = 0; i < 5; i++)
            {
                Player p = DataContainer.playerList[i];
                string nm = p.name;
                string text = sp + "  " + nm + " - ";
                text += new string(' ', n - text.Length);
                text += p.assists.ToString();
                Console.WriteLine(text);
            }
        }

        public static void DisplayStats()
        {
            string text;
            Console.WriteLine(sp + "Tournament Statistics:");
            int gamesPlayed = 64;
            int goalCount = GetTotalGoalCount();
            Team[] topTeamGoals = GetTopScoringTeams();
            text = sp + "  Total Goal Count: " + goalCount;
            double avg = (double) goalCount / (double) gamesPlayed;
            avg = Math.Round(avg, 2);
            text += " ( " + avg + " goals per game )";
            Console.WriteLine(text);
            Console.WriteLine(sp + "  Top Scoring Nations:");
            int n = 40;
            foreach (Team team in topTeamGoals)
            {
                text = sp + "    " + team.name + " - ";
                text += new string(' ', n - text.Length);
                text += team.goals;
                Console.WriteLine(text);
            }
        }

        public static int GetTotalGoalCount()
        {
            int total = 0;
            foreach (Team t in DataContainer.teamList)
                total += t.goals;
            return total;
        }

        public static Team[] GetTopScoringTeams()
        {
            DataContainer.teamList.Sort(new TeamGoalSort());
            return DataContainer.teamList.Take(5).ToArray();
        }

        public class GoalSort : IComparer<Player>
        {
            public int Compare(Player p1, Player p2)
            {
                if (p1.goals > p2.goals) return -1;
                if (p1.goals < p2.goals) return 1;
                if (p1.assists > p2.assists) return -1;
                if (p1.assists < p2.assists) return 1;
                if (p1.played > p2.played) return 1;
                if (p1.played < p2.played) return -1;
                if (p1.p_goals > p2.p_goals) return 1;
                if (p1.p_goals < p2.p_goals) return -1;
                if (p1.rating > p2.rating) return -1;
                if (p1.rating < p2.rating) return 1;
                return 0;
            }
        }

        public class TeamGoalSort : IComparer<Team>
        {
            public int Compare(Team p1, Team p2)
            {
                if (p1.goals > p2.goals) return -1;
                if (p1.goals < p2.goals) return 1;
                if (p1.conceded > p2.conceded) return 1;
                if (p1.conceded < p2.conceded) return -1;
                if (p1.played > p2.played) return 1;
                if (p1.played < p2.played) return -1;
                if (p1.rank > p2.rank) return 1;
                return -1;
            }
        }

        public class AssistSort : IComparer<Player>
        {
            public int Compare(Player p1, Player p2)
            {
                if (p1.assists > p2.assists) return -1;
                if (p1.assists < p2.assists) return 1;
                if (p1.played > p2.played) return 1;
                if (p1.played < p2.played) return -1;
                if (p1.rating > p2.rating) return -1;
                if (p1.rating < p2.rating) return 1;
                return 0;
            }
        }
    }
}
