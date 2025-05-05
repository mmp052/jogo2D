using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Arcaedion.DevDasGalaxias;
using System;
using System.Linq;

public class IAEnemy : MonoBehaviour
{
    [SerializeField] 
    private float _velocidade = 10f; // Velocidade do inimigo
    private float _movimentoHorizontal;
    private Rigidbody2D _rb;
    private Controle2D _controle; // Referência ao script Controle2D
    private int _andandoParaDireita; // Variável para armazenar a direção do movimento
    [SerializeField]
    private LayerMask _layersPermitidas;
    [SerializeField]
    private Vector2 _raycastOffset; // Offset do raycast
    private Animator _animator; // Referência ao Animator do inimigo
    [SerializeField]
    private float _rangeDeDeteccao = 3f; // Distância de detecção do inimigo
    [SerializeField]
    private bool _modoPerseguicao = false;
    [SerializeField]
    private Player _player;
    private bool _pulando; // Variável para armazenar se o jogador está pulando
    private bool _seguindoPlayer; // Variável para armazenar se o inimigo está seguindo o jogador
    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody2D>();
        _controle = GetComponent<Controle2D>();
        _andandoParaDireita = 1;
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        aplicaMovimento(); // Aplica o movimento ao inimigo
        detectaPlayer(); // Detecta o jogador
        detectaParede(); // Detecta se o inimigo está perto de uma parede
        detectaBeirada(); // Detecta se o inimigo está perto de uma beirada
    }

    void FixedUpdate()
    {
        _controle.Movimento(_movimentoHorizontal * Time.fixedDeltaTime, _pulando); // Aplica o movimento ao inimigo
        if(_pulando) // Se o inimigo estiver pulando
        {
            _pulando = false; // Reseta a variável de pulo para evitar múltiplos pulos
        }
    }
    private void aplicaMovimento()
    {
        _movimentoHorizontal = _andandoParaDireita * _velocidade; // Define a direção do movimento
        _animator.SetFloat("Speed", Math.Abs(_movimentoHorizontal)); // Atualiza o parâmetro de movimento no Animator
    }
    private void detectaPlayer()
    {
        var diferencaParaPlayer = _player.transform.position.x - transform.position.x; // Calcula a diferença de posição entre o inimigo e o jogador
        _seguindoPlayer = Math.Abs(diferencaParaPlayer) < _rangeDeDeteccao; // Verifica se o jogador está dentro do alcance de detecção
        if(_modoPerseguicao && _seguindoPlayer) // Se o modo de perseguição estiver ativado e o jogador estiver dentro do alcance
        {
            if(diferencaParaPlayer > 0) // Se o jogador estiver à direita do inimigo
            {
                _andandoParaDireita = 1; // Move para a direita
            }
            else if(diferencaParaPlayer < 0) // Se o jogador estiver à esquerda do inimigo
            {
                _andandoParaDireita = -1; // Move para a esquerda
            }
        }
    }
    private void detectaParede()
    {
        var OrigemX = transform.position.x + _raycastOffset.x;
        var OrigemY = transform.position.y + _raycastOffset.y;
        var raycastParedeDireita = Physics2D.Raycast(new Vector2(OrigemX, OrigemY), Vector2.right, 1f, _layersPermitidas);
        Debug.DrawRay(new Vector2(OrigemX, OrigemY), Vector2.right * 1f, Color.blue); // Desenha o raycast na cena para depuração
        if(raycastParedeDireita.collider != null)
        {
            if(!_seguindoPlayer) // Se o inimigo não estiver seguindo o jogador
            {
                _andandoParaDireita = -1; // Inverte a direção do movimento
            }
            else // Se o inimigo estiver seguindo o jogador
            {
                Pula(); // Faz o inimigo pular
            }

        }

        var raycastParedeEsquerda = Physics2D.Raycast(new Vector2(transform.position.x - _raycastOffset.x, OrigemY), Vector2.left, 1f, _layersPermitidas);
        Debug.DrawRay(new Vector2(transform.position.x - _raycastOffset.x, OrigemY), Vector2.left * 1f, Color.blue); // Desenha o raycast na cena para depuração
        if(raycastParedeEsquerda.collider != null)
        {
            if(!_seguindoPlayer) // Se o inimigo não estiver seguindo o jogador
            {
                _andandoParaDireita = 1; // Inverte a direção do movimento
            }
            else // Se o inimigo estiver seguindo o jogador
            {
                Pula(); // Faz o inimigo pular
            }
        }
    }
    private void detectaBeirada()
    {
        var raycastChaoDireita = Physics2D.Raycast(new Vector2(transform.position.x + _raycastOffset.x, transform.position.y), Vector2.down, 0.5f, _layersPermitidas);
        Debug.DrawRay(new Vector2(transform.position.x + _raycastOffset.x, transform.position.y), Vector2.down * 0.5f, Color.blue); // Desenha o raycast na cena para depuração
        if(raycastChaoDireita.collider == null) // Se não houver chão à direita do inimigo
        {
            if(!_seguindoPlayer) // Se o inimigo não estiver seguindo o jogador
            {
                _andandoParaDireita = -1; // Inverte a direção do movimento
            }
            else // Se o inimigo estiver seguindo o jogador
            {
                Pula(); // Faz o inimigo pular
            }
        }

        var raycastChaoEsquerda = Physics2D.Raycast(new Vector2(transform.position.x - _raycastOffset.x, transform.position.y), Vector2.down, 0.5f, _layersPermitidas);
        Debug.DrawRay(new Vector2(transform.position.x - _raycastOffset.x, transform.position.y), Vector2.down * 0.5f, Color.blue); // Desenha o raycast na cena para depuração
        if(raycastChaoEsquerda.collider == null) // Se não houver chão à esquerda do inimigo
        {
            if(!_seguindoPlayer) // Se o inimigo não estiver seguindo o jogador
            {
                _andandoParaDireita = 1; // Inverte a direção do movimento
            }
            else // Se o inimigo estiver seguindo o jogador
            {
                Pula(); // Faz o inimigo pular
            }
        }
    }
    private void Pula()
    {
        if(_controle.EstaNoChao())
        {
            _pulando = true; // Define a variável de pulo como verdadeira
        }
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Define a cor do gizmo
        // Desenha uma esfera um pouco acima da posição do inimigo com o raio de detecção
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y), _rangeDeDeteccao);
        
    }
}
