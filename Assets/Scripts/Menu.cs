using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Jogar()
    {
        SceneManager.LoadScene(2);
    }

    public void Instrucoes()
    {
        SceneManager.LoadScene(1);
    }

}
