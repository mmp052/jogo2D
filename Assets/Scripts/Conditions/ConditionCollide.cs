using UnityEngine;
<<<<<<< HEAD

namespace FSM
{
    public class ConditionCollide: Condition
    {
        Enemy agent;

        public ConditionCollide(Enemy ag) 
=======
using FSM;  // opcional, mas mantém a consistência

namespace FSM
{
    public class ConditionCollide : Condition
    {
        private Enemy agent;

        public ConditionCollide(Enemy ag)
>>>>>>> 07ec0b5fcdb43dd9f2f3417bf5dd1e1c2f8c276a
        {
            agent = ag;
        }

        public override bool Test()
        {
<<<<<<< HEAD
            return agent.flagEnterAttack;
        }

    }
}
=======
            // usa o nome da propriedade tal como está em Enemy.cs
            return agent.FlagEnterAttack;
        }
    }
}
>>>>>>> 07ec0b5fcdb43dd9f2f3417bf5dd1e1c2f8c276a
