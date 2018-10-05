using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fifa_World_Cup_Simulator
{
    public class Goal : IComparable<Goal>
    {
        public Player goalScorer;
        public Player assister;
        public int time;
        public bool penalty;

        public Goal(Player gs, Player ast, int t, bool pen = false)
        {
            this.goalScorer = gs;
            this.assister = ast;
            this.time = t;
            this.penalty = pen;
            DataContainer.goalList.Add(this);
        }

        public string GetGoalText()
        {
            string text = time.ToString();
            if (time > 90 && time < 96) text = "90 + " + (time - 90).ToString();
            text += "'  " + goalScorer.name;
            if (penalty) text += " (P)";
            return text;
        }

        public int CompareTo (Goal other)
        {
            return this.time - other.time;
        }
    }
}
