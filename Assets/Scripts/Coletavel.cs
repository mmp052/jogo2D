using UnityEngine;

public class Coletavel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Aqui você pode adicionar pontos, efeitos, etc.

            // Destrói o coletável
            Destroy(gameObject);
        }
    }
}
