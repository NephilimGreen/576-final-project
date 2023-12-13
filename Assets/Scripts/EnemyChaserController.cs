using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

// the CHASER enemy normally walks slowly through the maze, randomly
// if you get too close, they will chase you with very fast speed
public class EnemyChaserController : MonoBehaviour
{
    // used for determining if player is on current floor
    public int floor;
    public float storey_height;

    GameObject player;
    NavMeshAgent agent;

    // properties of CHASER enemy
    private bool isChasing = false;
    private float chaseRadiusSqr = 225.0f; // can be adjusted, using 15^2 for faster computation
    private float walkSpeed = 2.5f; // can be adjusted
    private float runSpeed = 7.5f; // can be adjusted
    private float searchRadius = 50.0f; // can be adjusted
    private float timer = 0;
    private float update = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PLAYER");
        agent = transform.GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        StartCoroutine(MoveToRandomPoint());
    }

    // Update is called once per frame
    void Update()
    {
        // to significantly improve performance, only update roughly every 0.5s instead of every frame
        timer += Time.deltaTime;
        if (timer > update)
        {
            SlowUpdate();
        }
    }

    void SlowUpdate()
    {
        // if can reach player, chase them
        // checking IsDestOnFloor() -> IsDestInSearchDistance() -> DoesPathToDestExist() greatly saves computation due to short-circuiting
        if (EnemyUtility.IsDestOnFloor(player.transform.position, floor, storey_height) &&
            EnemyUtility.IsDestInChaseRadius(agent, player.transform.position, chaseRadiusSqr) &&
            EnemyUtility.DoesPathToDestExist(agent, player.transform.position))
        {
            isChasing = true;
            agent.speed = runSpeed;
            transform.GetComponent<Renderer>().material.color = Color.white;
            // Debug.Log("chasing " + floor);
            agent.SetDestination(player.transform.position);
        }
        // otherwise, continue the random movement coroutine
        else
        {
            isChasing = false;
            agent.speed = walkSpeed;
            transform.GetComponent<Renderer>().material.color = Color.black;
        }
    }

    IEnumerator MoveToRandomPoint()
    {
        while (true)
        {
            if (!isChasing)
            {
                Vector3 destination = EnemyUtility.GenerateRandomDest(agent, searchRadius);
                agent.SetDestination(destination);
                // Debug.Log("chaser set new dest");
                yield return new WaitUntil(() => (isChasing || agent.hasPath && agent.remainingDistance <= 0.5f));
            }
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals(MazeRenderer.PLAYER_NAME))
        {
            // temporarily just teleport back to start on collision
            collision.gameObject.transform.position = new Vector3(0, storey_height, 0);
        }
    }
}

