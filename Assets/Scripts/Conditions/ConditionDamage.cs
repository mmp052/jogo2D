using FSM;
using UnityEngine;

public class ConditionDamage : Condition
{
    private Enemy _enemy;

    public ConditionDamage(Enemy enemy)
    {
        _enemy = enemy;
    }

    public override bool Test()
    {
        // quando detectar dano, dispara a transição
        return _enemy.Damaged;
    }
}
