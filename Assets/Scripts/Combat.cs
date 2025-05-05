using UnityEngine;

public class Combat : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Implementar lógica de combate aqui
            Debug.Log("Inimigo atingido!");
        }
    }
}
