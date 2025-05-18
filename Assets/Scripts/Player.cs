using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public void TakeDamage(int damage)
    {
        // Implementar lógica de dano aqui
        Debug.Log("Dano recebido: " + damage);
    }
    public void IniciaAtaque()
    {
        // Implementar lógica de ataque aqui
        Debug.Log("Ataque iniciado!");
    }
    public void FinalizaAtaque()
    {
        // Implementar lógica de finalização de ataque aqui
        Debug.Log("Ataque finalizado!");
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("fundo")) {
            SceneManager.LoadScene(2);
        }
    }
}