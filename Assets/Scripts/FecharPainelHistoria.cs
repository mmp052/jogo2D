using UnityEngine;

public class FecharPainelHistoria : MonoBehaviour
{
    public GameObject PainelHistoria;

    public void Fechar()
    {
        PainelHistoria.SetActive(false);
        Time.timeScale = 1f; // volta ao tempo normal do jogo
    }
}
