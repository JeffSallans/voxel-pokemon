using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The player will fall until their feet colide with something
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class FallToGround : MonoBehaviour
{
    /// <summary>
    /// The player's movement
    /// </summary>
    public CharacterController controller;

    /// <summary>
    /// The speed the player falls
    /// </summary>
    public float speed = 6f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        controller.Move(Vector3.down * speed * Time.deltaTime);
    }

}
