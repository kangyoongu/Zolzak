using UnityEngine;
using UnityEngine.Events;

public class WindowEventGet : MonoBehaviour
{
    public UnityEvent OnEvent;
    public bool once;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WindowEvent"))
        {
            OnEvent?.Invoke();
            if (once)
                gameObject.SetActive(false);
        }
    }
}
