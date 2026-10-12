using UnityEngine;

public class Sliceable : MonoBehaviour
{
    public GameObject whole;
    public GameObject sliced;

    public int points = 1;

    [Header("Звук и эффекты")]
    public AudioClip sliceSound;
    public GameObject scorePopupPrefab;

    [Header("Сила разлета")]
    public float sliceForce = 5f; // Сила, с которой разлетаются половинки

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

        // Звук
        if (sliceSound != null)
            AudioSource.PlayClipAtPoint(sliceSound, transform.position);

        // Попап "+1"
        if (scorePopupPrefab != null)
        {
            GameObject popup = Instantiate(scorePopupPrefab, transform.position, Quaternion.identity);
            TMPro.TextMeshPro tmp = popup.GetComponent<TMPro.TextMeshPro>();
            if (tmp != null) tmp.text = "+" + points;
        }

        fruitCollider.enabled = false;
        whole.SetActive(false);
        sliced.SetActive(true);

        // Частицы сока
        if (juiceEffect != null)
            juiceEffect.Play();

        // Поворот половинок вдоль линии разреза
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // === РАЗЛЕТ ПОЛОВИНОК ===
        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody slice in slices)
        {
            // Сохраняем скорость целого фрукта
            slice.linearVelocity = fruitRigidbody.linearVelocity;

            // Направление разлета: перпендикулярно линии разреза
            Vector3 perpendicular = Vector3.Cross(direction.normalized, Vector3.forward).normalized;

            // Добавляем случайности, чтобы половинки не летели идеально ровно
            perpendicular += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0f);

            // Применяем силу
            slice.AddForce(perpendicular * sliceForce, ForceMode.Impulse);

            // Добавляем вращение для реалистичности
            slice.AddTorque(new Vector3(0f, 0f, Random.Range(-5f, 5f)), ForceMode.Impulse);
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