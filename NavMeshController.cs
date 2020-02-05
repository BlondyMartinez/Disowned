using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Sets destination of one of the enemies.
public class NavMeshController : MonoBehaviour {
    public GameObject Nine;
    private NavMeshAgent agent;
    Vector3 destination;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
		Nine = GameObject.FindGameObjectWithTag("Nine");
        destination = agent.destination;
    }
    void Update()
	{
        Destination();
	}

    void Destination()  {
        destination = Nine.transform.position;
        agent.destination = destination;
    }
}