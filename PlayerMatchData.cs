using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLMMX
{
    public class PlayerMatchData
    {
        private string player_id;

        private float AttackValue = 0;

        private float TKValue = 0;
        private float TKTimes = 0;
        private float TKPlayerNumbers = 0;

        private int AttackHorse = 0;
        private int TKHorse = 0;

        private int KillNum = 0;
        private int DeadNum = 0;
        public PlayerMatchData(string PlayerId)
        {
            player_id = PlayerId;
        }

        public void RefreshAll()
        {
            AttackValue = 0;

            TKValue = 0;
            TKTimes = 0;
            TKPlayerNumbers = 0;

            AttackHorse = 0;
            TKHorse = 0;

        }

        public void AddAttack(float AttackValue1)
        {
            AttackValue += AttackValue1;
        }

        public void AddTK(float TKValue1) { TKValue += TKValue1; }

        public void AddTKPlayerNumber(float TKPlayerNumbers1)
        {
            TKPlayerNumbers += TKPlayerNumbers1;
            TKTimes++;
        }

        public void AddAttackHorse(int AttackHorseValue)
        {
            AttackHorse += AttackHorseValue;
        }

        public void AddTKHorse(int TKHorseValue)
        {
            TKHorse += TKHorseValue;
        }
    }
}
