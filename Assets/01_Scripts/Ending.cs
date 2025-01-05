using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    [SerializeField] CanvasGroup alpha;
    [SerializeField] int sceneIdx = 0;
    private void Start()
    {
        StartCoroutine(EndingProgress());
    }
    IEnumerator EndingProgress()
    {
        alpha.DOFade(0f, 2f);
        yield return new WaitForSeconds(5f);
        alpha.DOFade(1f, 1f);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneIdx);
    }
}
