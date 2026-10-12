using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionEffect;
    public AudioClip explosionSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;

            if (explosionEffect != null)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            if (explosionSound != null)
                AudioSource.PlayClipAtPoint(explosionSound, transform.position);

            GameController.Instance.LoseLife();
        }
    }
}