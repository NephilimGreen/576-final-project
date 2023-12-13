using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// Got this from the Unity forums.
public class PickUp : MonoBehaviour
{
    public TextMeshPro text;
    private bool added;
    public GameObject thingToFace;
    public GameObject backGroundBox;
    public MazeRenderer renderer;

    // Use this for initialization 
    void Start()
    {
        text = gameObject.GetComponent<TextMeshPro>();
        added = false;
        text.gameObject.transform.localScale = Vector3.Scale(text.gameObject.transform.localScale, new Vector3 (-1, 1, 1));
    }

    // Update is called once per frame 
    void LateUpdate()
    {
        gameObject.transform.LookAt(thingToFace.transform);
        backGroundBox.transform.LookAt(thingToFace.transform);
        text.transform.position = backGroundBox.transform.TransformPoint(backGroundBox.GetComponent<BoxCollider>().center) + (backGroundBox.transform.forward * backGroundBox.GetComponent<BoxCollider>().transform.localScale.z);
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name.Equals(MazeRenderer.PLAYER_NAME))
        {
            if(!added)  // Prevents double-adding when Unity is finnicky with collisions.
            {
                added = true;
                if (text.text.Equals(MazeGenerator.HEALTH_BOOST))
                {
                    renderer.playerHealth += 1;
                }
                else if(text.text.Equals(MazeGenerator.SPEED_BOST))
                {
                    renderer.playerSpeedModifier += MazeRenderer.SPEED_BOOST_MODIFIER;
                    renderer.speedBoostTimer += MazeRenderer.SPEED_BOOST_TIME;
                }
                else { collider.gameObject.GetComponent<Inventory>().add(text.text); }
                Destroy(text);
                Destroy(gameObject);
                Destroy(backGroundBox);
            }
        }
    }
}