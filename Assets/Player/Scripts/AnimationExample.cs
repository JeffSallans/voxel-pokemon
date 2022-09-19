using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Plays walking animations and sounds for the character.
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimationExample : MonoBehaviour
{

    public List<AudioSource> audioSourceList;
    private int lastPlayed = -1;

    public List<KeyCode> movementKeys = new List<KeyCode>
    {
        KeyCode.W,
        KeyCode.A,
        KeyCode.S,
        KeyCode.D,
    };
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    private Animator _animator;
    private Animator animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            return _animator;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Start (or continue) walking animation when any movement key starts being pressed.
        if (movementKeys.Any(Input.GetKeyDown))
        {
            animator.SetBool(IsWalking, true);
        }
        // Stop walking animation when any movement key is released and now none are pressed.
        else if (movementKeys.Any(Input.GetKeyUp) && !movementKeys.Any(Input.GetKey))
        {
            animator.SetBool(IsWalking, false);
        }

        // Play a random walking sounds (without consecutive repeats) continuously while walking.
        if (!animator.GetBool(IsWalking)) return;
        if (audioSourceList.Any(a => a.isPlaying) || audioSourceList.Count <= 0) return;

        // Pick from a list that doesn't include the unwanted index to avoid
        // technically-inconsistent/unbounded runtime by re-randomizing when
        // the unwanted index is randomly picked.
        var indexes = new List<int>();
        indexes.AddRange(Enumerable.Range(0, audioSourceList.Count));
        indexes.Remove(lastPlayed);
        var randomIndex = indexes[Random.Range(0, indexes.Count)];

        lastPlayed = randomIndex;
        audioSourceList[randomIndex].Play();
    }
}
