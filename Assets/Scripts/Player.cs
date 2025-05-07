using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int _vida = 100;
    private int _ataque = 10;
    private int _defesa = 1;
    private Animator _animator;

    public void TakeDamage(int damage)
    {
        // Implementar lógica de dano aqui
        Debug.Log("Dano recebido: " + damage);
        _vida -= damage - _defesa;


    }
}