using FSM;
using UnityEngine;

namespace FSM
{
    public class ConditionDeath : Condition
    {
        private Enemy _enemy;

        public ConditionDeath(Enemy enemy)
        {
            _enemy = enemy;
        }

        public override bool Test()
        {
            return _enemy.Vida <= 0;
        }
    }
}
