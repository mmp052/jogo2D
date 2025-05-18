using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GerenciaJogo : MonoBehaviour
{
    public static GerenciaJogo Instance { get; private set; }

    [Tooltip("Nomes EXATOS dos clips de morte para cada personagem")]
    public string[] deathAnimationStates;
    [Tooltip("Fallback de delay (s) caso o death clip não seja encontrado")]
    public float defaultDeathDelay = 1f;

    // **novo**: última posição de respawn
    private Vector3 _checkpointPosition;

    void Awake()
    {
        // singleton    
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        PlayerMovement.OnPlayerDeath += HandlePlayerDeath;
    }

    void OnDisable()
    {
        PlayerMovement.OnPlayerDeath -= HandlePlayerDeath;
    }

    // exposto para o Checkpoint chamar
    public void SetCheckpoint(Vector3 pos)
    {
        _checkpointPosition = pos;
        // opcional: feedback visual ou de som
    }

    // chamado pela extremidade (queda)
    public void HandlePlayerFall()
    {
        RespawnPlayer();
    }

    private void HandlePlayerDeath(PlayerMovement deadPlayer)
    {
        var anim = deadPlayer.GetComponent<Animator>();
        StartCoroutine(RestartAfterDeath(anim));
    }

    private IEnumerator RestartAfterDeath(Animator anim)
    {
        yield return null; // espera frame
        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // busca clip de morte
        AnimationClip deathClip = null;
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            foreach (var deathState in deathAnimationStates)
                if (clip.name == deathState)
                {
                    deathClip = clip;
                    break;
                }
            if (deathClip != null) break;
        }

        // calcula delay
        float waitTime = defaultDeathDelay;
        if (deathClip != null && stateInfo.IsName(deathClip.name))
        {
            waitTime = deathClip.length * (1f - stateInfo.normalizedTime);
            waitTime = Mathf.Max(0f, waitTime);
        }
        yield return new WaitForSeconds(waitTime);

        // **em vez de recarregar**, reposiciona o jogador
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        // encontra o jogador na cena
        var player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            player.RespawnAt(_checkpointPosition);
    }
}
