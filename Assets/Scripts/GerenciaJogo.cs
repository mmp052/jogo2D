// GerenciaJogo.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GerenciaJogo : MonoBehaviour
{
    [Tooltip("Nomes EXATOS dos clips de morte para cada personagem")]
    public string[] deathAnimationStates;

    [Tooltip("Fallback de delay (s) caso o death clip não seja encontrado")]
    public float defaultDeathDelay = 1f;

    void OnEnable()
    {
        PlayerMovement.OnPlayerDeath += HandlePlayerDeath;
    }

    void OnDisable()
    {
        PlayerMovement.OnPlayerDeath -= HandlePlayerDeath;
    }

    void Update()
    {
        // reload manual com R
        if (Input.GetKeyDown(KeyCode.R))
            ReloadScene();
    }

    private void HandlePlayerDeath(PlayerMovement deadPlayer)
    {
        var anim = deadPlayer.GetComponent<Animator>();
        StartCoroutine(RestartAfterDeath(anim));
    }

    private IEnumerator RestartAfterDeath(Animator anim)
    {
        // 1) espera um frame para garantir que a transição pro estado de morte já aconteceu
        yield return null;

        // 2) pega info e clips atuais
        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        var currentClips = anim.GetCurrentAnimatorClipInfo(0);

        // 3) tenta achar o clip de morte (por nome) na lista do controller
        AnimationClip deathClip = null;
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            foreach (var deathState in deathAnimationStates)
            {
                if (clip.name == deathState)
                {
                    deathClip = clip;
                    break;
                }
            }
            if (deathClip != null) break;
        }

        // 4) calcula tempo a esperar
        float waitTime = defaultDeathDelay;
        if (deathClip != null && stateInfo.IsName(deathClip.name))
        {
            // tempo restante = comprimento total * (1 - normalizedTime)
            waitTime = deathClip.length * (1f - stateInfo.normalizedTime);
            if (waitTime < 0) 
                waitTime = 0f;
        }

        // 5) espera esse tempo
        yield return new WaitForSeconds(waitTime);

        // 6) reinicia a cena
        ReloadScene();
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
