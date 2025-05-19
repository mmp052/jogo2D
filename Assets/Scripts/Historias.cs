using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Historias : MonoBehaviour
{
    void Update()
    {
        // Quando o jogador aperta TAB, muda para a próxima cena (pode ajustar a lógica)
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CarregarProximaCena();
        }
    }

    public void Voltar1()
    {
        SceneManager.LoadScene(0);
    }

    public void Voltar2()
    {
        SceneManager.LoadScene(2);
    }

    public void Seguir1()
    {
        SceneManager.LoadScene(3);
    }

    public void Seguir2()
    {
        SceneManager.LoadScene(4);
    }

    private void CarregarProximaCena()
    {
        int cenaAtual = SceneManager.GetActiveScene().buildIndex;
        int totalCenas = SceneManager.sceneCountInBuildSettings;

        int proximaCena = (cenaAtual + 1) % totalCenas; // Volta ao início se for a última
        SceneManager.LoadScene(proximaCena);
    }
}
