using UnityEngine;

public class Extremidade : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 1) Só o Player
        if (!collision.CompareTag("Player")) 
            return;

        // 2) Ignora colliders que são 'trigger' (o seu hitbox)
        if (collision.isTrigger) 
            return;

        // 3) Aqui sim: o corpo do player saiu, respawna
        GerenciaJogo.Instance.HandlePlayerFall();
    }
}
