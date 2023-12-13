using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

// the PATROLLER enemy travels between three different patrol points at a constant speed
public class EnemyPatrollerController : MonoBehaviour
{
    // used for determining if player is on current floor
    public int floor;
    public float storey_height;

    NavMeshAgent agent;

    // properties of PATROLLER enemy
    private Vector3[] patrolPoints = new Vector3[3];
    private int nextPatrolPoint = 0;
    private float walkSpeed = 4.0f; // can be adjusted
    private float minimumPatrolDistance = 100.0f; // can be adjusted
    private float searchRadius = 150.0f; // can be adjusted
    // Start is called before the first frame update
    void Start()
    {
        agent = transform.GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        agent.acceleration = 12;
        agent.angularSpeed = 240;

        // set three patrol points, one is the starting position and the other two are randomly assigned
        // NOTE: this function call causes some lag at instantiation, can decrease minimumPatrolDistance and searchRadius if needed
        patrolPoints = EnemyUtility.GeneratePatrolPoints(agent, minimumPatrolDistance, searchRadius);
        // foreach (Vector3 p in patrolPoints) Debug.Log(p);
        StartCoroutine(MoveToPatrolPoints());
    }

    IEnumerator MoveToPatrolPoints()
    {
        // Renderer renderer = transform.GetComponent<Renderer>();
        while (true)
        {
            // cycle through patrol points
            nextPatrolPoint = (nextPatrolPoint + 1) % patrolPoints.Length;
            Vector3 destination = patrolPoints[nextPatrolPoint];
            // renderer.material.color = colors[nextPatrolPoint];
            agent.SetDestination(destination);
            // Debug.Log("patroller set new dest");
            yield return new WaitUntil(() => agent.hasPath && agent.remainingDistance <= 0.5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals(MazeRenderer.PLAYER_NAME))
        {
            // temporarily just teleport back to start on collision
            transform.GetComponent<Animator>().SetBool("run", false);
            transform.GetComponent<Animator>().SetBool("walk", false);
            transform.GetComponent<Animator>().SetBool("punch", true);
            // collision.gameObject.transform.position = new Vector3(0, storey_height, 0);
        }
    }
}