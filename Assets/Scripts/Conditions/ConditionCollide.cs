using UnityEngine;
using FSM;  // opcional, mas mantém a consistência

namespace FSM
{
    public class ConditionCollide : Condition
    {
        private Enemy agent;

        public ConditionCollide(Enemy ag)
        {
            agent = ag;
        }

        public override bool Test()
        {
            // usa o nome da propriedade tal como está em Enemy.cs
            return agent.FlagEnterAttack;
        }
    }
}
