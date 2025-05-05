using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}