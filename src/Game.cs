using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fifa_World_Cup_Simulator
{
    public class Game
    {
        const int AVG_INCIDENT_TIME = 12;

        public string id;
        public Team winner;
        public Team loser;
        public List<Goal> goals = new List<Goal>();
        public List<Goal> t1goals = new List<Goal>();
        public List<Goal> t2goals = new List<Goal>();
        public int[] score = new int[2];
        
        public Team teamA { get; private set; }
        public Team teamB { get; private set; }
        private Random random;
        private bool groupGame = false;
        private bool simulated = false;

        public Game(Team t1, Team t2, Random random, bool isGroupGame = false)
        {
            this.teamA = t1;
            this.teamB = t2;
            this.id = "#" + t1.initial + t2.initial;
            this.random = random;
            this.groupGame = isGroupGame;
            DataContainer.gameList.Add(this);
        }

        public void SimulateGame()
        {
            if (teamA == null || teamB == null) return;
            simulated = true;
            int gameTime = 0;
            while (true)
            {
                int n = random.Next(1, AVG_INCIDENT_TIME) + random.Next(1, AVG_INCIDENT_TIME);
                gameTime += n;
                if (gameTime > 95) break;
                SimulateIncident(gameTime);
            }
            if (groupGame)
            {
                teamA.played++;
                teamB.played++;
            }
            for (int i=0; i<teamA.players.Count; i++)
            {
                teamA.players[i].played++;
                teamB.players[i].played++;
            }
            EndGame();
        }

        private void SimulateIncident(int gameTime)
        {
            double tAChance = 50.0, tBChance = 50.0;
            double rankBias = GetRankBias();
            double ratingBias = GetRatingBias();
            if (teamA.rank < teamB.rank)
            {
                tAChance += rankBias;
                tBChance -= rankBias;
            }
            else
            {
                tBChance += rankBias;
                tAChance -= rankBias;
            }
            tAChance += ratingBias;
            int rand = random.Next(0, 100);
            Team atk = rand < tAChance ? teamA : teamB;
            Team def = atk == teamA ? teamB : teamA;
            double atkBonus = GetAttackBonus(atk, def);
            double goalChance = 30.0 + atkBonus;
            rand = random.Next(0, 100);
            if (rand < goalChance) ScoreGoal(atk, def, gameTime);
        }

        private void EndGame()
        {
            if (score[0] > score[1])
            {
                winner = teamA;
                loser = teamB;
                teamA.wins++;
                teamB.losses++;
                if (groupGame) teamA.pts += 3;
            }
            else if (score[1] > score[0])
            {
                winner = teamB;
                teamB.wins++;
                loser = teamA;
                teamA.losses++;
                if (groupGame) teamB.pts += 3;
            }
            else
            {
                if (groupGame)
                {
                    winner = null;
                    loser = null;
                    teamA.draws++;
                    teamB.draws++;
                    teamA.pts++;
                    teamB.pts++;
                }
                else
                {
                    int rand = random.Next(0, 100);
                    if (rand < 50) ScoreGoal(teamA, teamB, random.Next(1, 95));
                    else ScoreGoal(teamA, teamB, random.Next(1, 95));
                    EndGame();
                    return;
                }
                
            }
            goals.Sort();
            ArrangeGoals();
        }

        private void ScoreGoal(Team atk, Team def, int time)
        {
            Player scorer = GetGoalScorer(atk);
            Player assister = null;
            int penChance = 10;
            bool pen = random.Next(100) < penChance;
            if (!pen) assister = GetAssistGiver(atk, scorer);
            goals.Add(new Goal(scorer, assister, time, pen));
            scorer.goals++;
            if (pen) scorer.p_goals++;
            if (!DataContainer.goalScorers.ContainsKey(scorer.name)) DataContainer.goalScorers.Add(scorer.name, 0);
            DataContainer.goalScorers[scorer.name]++;
            if (assister != null)
            {
                assister.assists++;
                if (!DataContainer.assistGivers.ContainsKey(assister.name)) DataContainer.assistGivers.Add(assister.name, 0);
                DataContainer.assistGivers[assister.name]++;
            }
            atk.goals++;
            def.conceded++;
            int indx = atk == teamA ? 0 : 1;
            score[indx]++;
        }

        private void ArrangeGoals()
        {
            if (goals.Count <= 0) return;
            foreach(Goal goal in goals)
            {
                Team scorer = goal.goalScorer.team;
                if (scorer == teamA) t1goals.Add(goal);
                else t2goals.Add(goal);
            }
        }

        private double GetRankBias()
        {
            double diff = Math.Abs(teamA.rank - teamB.rank);
            return diff;
        }

        private double GetRatingBias()
        {
            double diff = teamA.AverageRating() - teamB.AverageRating();
            return diff;
        }

        private double GetAttackBonus(Team atk, Team def)
        {
            double fwd = atk.AverageAttack();
            double dfd = def.AverageDefence();
            return fwd - dfd;
        }

        private Player GetGoalScorer(Team team)
        {
            int rand = random.Next(0, team.GoalRating());
            foreach(Player p in team.players)
            {
                int n = p.rating * (int)p.position;
                if (n > rand) return p;
                rand -= n;
            }
            return team.players[0];
        }

        private Player GetAssistGiver(Team team, Player gs)
        {
            int rand = random.Next(22);
            if (rand > 10) return null;
            Player pl = team.players[rand];
            if (pl.name == gs.name || pl.position == Position.Gk) return null;
            return pl;
        }

        public void DisplayGame(int spaces = 14)
        {
            string space = new string(' ', spaces);
            if (!simulated)
            {
                Console.WriteLine("Game not simulated yet!");
                return;
            }
            string horzLine = space + new string (' ', 6) + new string('-', 79);
            Console.WriteLine(horzLine);
            string text = string.Format("{0,35}", teamA.name) + "  " + score[0] + " - " + score[1] + "  " + teamB.name;
            text = space + "     |" + text;
            text += new string(' ', horzLine.Length - text.Length) + "|";
            Console.WriteLine(text);
            int toT1End = 35;
            int middleSpace = ("  " + score[0] + " - " + score[1] + "  ").Length;
            int toT2Start = toT1End + middleSpace;
            text =  space + "     |" + new string(' ', toT1End + ("  " + score[0]).Length) + "FT";
            text += new string(' ', horzLine.Length - text.Length) + "|";
            Console.WriteLine(text);
            DisplayGoals(toT1End, middleSpace, toT2Start, spaces);
            Console.WriteLine(horzLine + "\n");
        }

        private void DisplayGoals(int toT1End, int middleSpace, int toT2Start, int spaces)
        {
            string space = new string(' ', spaces);
            string text;
            string t1goalText = $"{{0,{toT1End}}}";
            int count = Math.Max(t1goals.Count, t2goals.Count);
            for (int i = 0; i < count; i++)
            {
                text = space + "     |";
                if (i < t1goals.Count)
                {
                    text += string.Format(t1goalText, t1goals[i].GetGoalText());
                    if (i < t2goals.Count)
                    {
                        text += new string(' ', middleSpace);
                        text += t2goals[i].GetGoalText();
                    }
                }
                else
                {
                    text += new string(' ', toT2Start);
                    text += t2goals[i].GetGoalText();
                }
                text += new string(' ', 85 + spaces - text.Length) + "|";
                Console.WriteLine(text);
            }
        }
    }
}
