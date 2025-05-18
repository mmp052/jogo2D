using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool flagEnterAttack = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            flagEnterAttack = true;
        }
    }
}
