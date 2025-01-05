using UnityEngine;

public class Lemon : MonoBehaviour
{
    public float rotateSpeed;
    private void Start()
    {
        StartCoroutine(Core.DelayFrame(() => GameManager.Instance.lemons.Add(gameObject)));
    }
    private void OnDestroy()
    {
        if (GameManager.Instance.lemons != null && GameManager.Instance != null)
            GameManager.Instance.lemons.Remove(gameObject);
    }
    void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * rotateSpeed);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.root.GetComponent<Player>().EatLemon();
            gameObject.SetActive(false);
        }
    }
}
