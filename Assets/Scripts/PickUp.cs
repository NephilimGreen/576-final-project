using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// Got this from the Unity forums.
public class PickUp : MonoBehaviour
{
    public TextMeshPro text;
    private bool added;

    // Use this for initialization 
    void Start()
    {
        text = gameObject.GetComponent<TextMeshPro>();
        added = false;
    }

    // Update is called once per frame 
    void LateUpdate()
    {
        gameObject.transform.LookAt(Camera.main.transform);
        text.transform.LookAt(Camera.main.transform);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals(MazeRenderer.PLAYER_NAME))
        {
            if(!collision.gameObject.GetComponent<Inventory>().IsHotBarFull() && !added)  // Prevents double-adding when Unity is finnicky with collisions.
            {
                collision.gameObject.GetComponent<Inventory>().add(text.text);
                added = true;
                Destroy(text);
                Destroy(gameObject);
            }
        }
    }
}