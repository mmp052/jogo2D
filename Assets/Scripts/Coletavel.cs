using UnityEngine;

public class Coletavel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var pm = other.GetComponent<PlayerMovement>();
            if (pm != null)
                pm.EnableTransformation();

            // destrói o coletável
            Destroy(gameObject);
        }
    }
}
