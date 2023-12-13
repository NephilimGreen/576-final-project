using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Placeholder script for the player. Delete or modify as needed.
public class Inventory : MonoBehaviour
{
    private Dictionary<string, int> items;
    public MazeRenderer mazeRenderer;

    // Start is called before the first frame update
    void Start()
    {
        items = new Dictionary<string, int>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void add(string pickup)
    {
        // fixed issue with duplicate keys
        if (items.ContainsKey(pickup))
        {
            items[pickup]++;
        }
        else
        {
            items.Add(pickup, 1);
        }
    }

    public bool use(string pickup)
    {
        if(items.ContainsKey(pickup) && (items[pickup] > 0))
        {
            items[pickup] -= 1;
            return true;
        }
        return false;
    }

    public bool drop(string pickup, Vector3 position, Vector3 size)
    {
        if(use(pickup))
        {
            mazeRenderer.createPickup(pickup, position, size);
            return true;
        }
        return false;
    }
}
