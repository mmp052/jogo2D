using UnityEngine;
using UnityEngine.SceneManagement;

public class Instrucoes : MonoBehaviour
{
    public void Voltar()
    {
        SceneManager.LoadScene(0);
    }

}