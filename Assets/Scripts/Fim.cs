using UnityEngine;
using UnityEngine.SceneManagement;

public class Fim : MonoBehaviour
{
    public void Reiniciar()
    {
        SceneManager.LoadScene(1);
    }

    public void Voltar()
    {
        SceneManager.LoadScene(0);
    }
}
