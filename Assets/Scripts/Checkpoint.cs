using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Vector3 pos = transform.position;
        Debug.Log($"[Checkpoint] Player entrou em '{name}', setando checkpoint em {pos}");

        if (GerenciaJogo.Instance == null)
        {
            Debug.LogError("[Checkpoint] GerenciaJogo.Instance é NULL!");
            return;
        }

        GerenciaJogo.Instance.SetCheckpoint(pos);
    }
}
