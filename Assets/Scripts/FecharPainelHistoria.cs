using UnityEngine;

public class FecharPainelHistoria : MonoBehaviour
{
    public GameObject painelHistoria;

    public void Fechar()
    {
        painelHistoria.SetActive(false);
        Time.timeScale = 1f; // volta ao tempo normal do jogo
    }
}
