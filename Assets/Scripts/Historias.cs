using UnityEngine;
using UnityEngine.SceneManagement;

public class Historias : MonoBehaviour
{
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

}