using UnityEngine;

namespace FSM
{
    public class ConditionCollide: Condition
    {
        Enemy agent;

        public ConditionCollide(Enemy ag) 
        {
            agent = ag;
        }

        public override bool Test()
        {
            return agent.flagEnterAttack;
        }

    }
}