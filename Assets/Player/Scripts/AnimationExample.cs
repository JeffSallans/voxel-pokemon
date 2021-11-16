using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to demo the walking animation for the character.  Hold W to see it work.
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimationExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print("Press w to trigger walk animation");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            gameObject.GetComponent<Animator>().SetBool("isWalking", true);
        }
        else
        {
            gameObject.GetComponent<Animator>().SetBool("isWalking", false);
        }
    }
}
