using UnityEngine;

public class Sliceable : MonoBehaviour
{
    public GameObject whole;
    public GameObject sliced;

    public int points = 1;

    [Header("Звук и эффекты")]
    public AudioClip sliceSound;      // Звук разрезания
    public GameObject scorePopupPrefab; // Всплывающий текст "+1" (опционально)

    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;
    private ParticleSystem juiceEffect;

    private void Awake()
    {
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        juiceEffect = GetComponentInChildren<ParticleSystem>();
    }

    private void Slice(Vector3 direction, Vector3 position, float force)
    {
        GameController.Instance.IncreaseScore(points);

        // Звук разрезания
        if (sliceSound != null)
            AudioSource.PlayClipAtPoint(sliceSound, transform.position);

        fruitCollider.enabled = false;
        whole.SetActive(false);

        sliced.SetActive(true);

        // Частицы сока
        if (juiceEffect != null)
            juiceEffect.Play();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody slice in slices)
        {
            slice.linearVelocity = fruitRigidbody.linearVelocity;
            // ИСПРАВЛЕНО: разлет в сторону свайпа
            slice.AddForce(direction.normalized * force, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Sword blade = other.GetComponent<Sword>();
            Slice(blade.direction, blade.transform.position, blade.sliceForce);
        }
    }
}
