using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaedion.DevDasGalaxias;

[RequireComponent(typeof(Controle2D))]
public class Movimento2DPlayer : MonoBehaviour
{
    private Controle2D _controle; // Referência ao script Controle2D
    private float _movimentoHorizontal; // Variável para armazenar o movimento horizontal
    [SerializeField]
    private float _velocidade = 30f;
    private bool _pulando; // Variável para armazenar se o jogador está pulando
    private void Awake()
    {
        // Inicializa o Rigidbody2D e o Collider2D
        _controle = GetComponent<Controle2D>();
    }

    void Update()
    {
        _movimentoHorizontal = Input.GetAxisRaw("Horizontal") * _velocidade; // Captura o movimento horizontal do jogador(A/D ou <-/->)
        if(Input.GetButtonDown("Jump"))
        {
            _pulando = true; // Define a variável de pulo como verdadeira
        }
    }

    void FixedUpdate()
    {
        _controle.Movimento(_movimentoHorizontal * Time.fixedDeltaTime, _pulando); // Aplica o movimento ao jogador
        _pulando = false; // Reseta a variável de pulo para evitar múltiplos pulos
    }
}
