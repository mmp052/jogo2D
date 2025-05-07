using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool flagEnterAttack = false;
    [SerializeField]
    private int _vida = 20;
    private int _defesa = 2;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            flagEnterAttack = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            flagEnterAttack = false;
        }
    }

    public void TakeDamage(Vector2 playerPosition, int damage)
    {
        // Implementar lógica de dano aqui
        damage -= _defesa; // Aplica defesa
        _vida -= damage;
        if (_vida <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // Implementar lógica de morte aqui
        Debug.Log("Inimigo morreu!");
        Destroy(gameObject);
    }
}
