using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The configuration for a scene transition
/// </summary>
public class SceneNav : MonoBehaviour
{
    public string sceneName;
    public float delayInSeconds = 1.2f;

    public Animator animator = null;
    public string titleAnimatorTriggerName = "fadeOut";
    public float animationInSeconds = 1.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Triggers the Scene navigation
    /// </summary>
    public void OnNavigate()
    {
        StartCoroutine(NavigateToScene());
    }

    private IEnumerator NavigateToScene()
    {
        yield return new WaitForSeconds(delayInSeconds);

        if (animator != null)
        {
            animator.SetTrigger(titleAnimatorTriggerName);
            yield return new WaitForSeconds(animationInSeconds);
        }

        SceneManager.LoadScene(sceneName);
    }
}
