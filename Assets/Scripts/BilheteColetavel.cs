using UnityEngine;
using TMPro;

public class BilheteColetavel : MonoBehaviour
{
    public GameObject painelHistoria;
    public TextMeshProUGUI textoHistoria;
    [TextArea(3, 10)]
    public string conteudoDoBilhete;

    private bool jogadorPerto = false;

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            painelHistoria.SetActive(true);
            textoHistoria.text = conteudoDoBilhete;
            Time.timeScale = 0f; // pausa o jogo
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jogadorPerto = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jogadorPerto = false;
    }
}
