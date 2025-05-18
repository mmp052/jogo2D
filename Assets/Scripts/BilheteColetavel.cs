using UnityEngine;
using TMPro;

using UnityEngine;
using TMPro;

public class BilheteColetavel : MonoBehaviour
{
    public GameObject painelHistoria;
    public TextMeshProUGUI textoHistoria;
    [TextArea(3, 10)]
    public string conteudoDoBilhete;

    private void OnMouseDown()
    {
        painelHistoria.SetActive(true);
        textoHistoria.text = conteudoDoBilhete;
        Time.timeScale = 0f; // pausa o jogo
    }
}

