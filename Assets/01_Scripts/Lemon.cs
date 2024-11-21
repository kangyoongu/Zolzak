using UnityEngine;

public class Lemon : MonoBehaviour
{
    public float rotateSpeed;
    
    void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * rotateSpeed);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.root.GetComponent<Player>().EatLemon();
            Destroy(gameObject);
        }
    }
}
