using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavigation : MonoBehaviour
{
    public int navInterval;
    public Vector2 maxDistanceFromOrigin;

    
    private Vector3 startingPoint;
    private NavMeshAgent agent;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        startingPoint = transform.position;

        StartCoroutine(testNav());
    }

    private IEnumerator testNav()
    {
        while (true)
        {
            float x = Random.Range(startingPoint.x - maxDistanceFromOrigin.x, startingPoint.x + maxDistanceFromOrigin.x);
            float y = Random.Range(startingPoint.y - maxDistanceFromOrigin.y, startingPoint.y + maxDistanceFromOrigin.y);

            Vector3 nextPoint = new Vector3(x, 0, y);

            agent.SetDestination(nextPoint);

            Debug.Log("Next destination set");

            yield return new WaitForSeconds(navInterval);
        }
    }
}
