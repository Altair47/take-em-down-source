using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieNavMesh : MonoBehaviour
{
    // Start is called before the first frame update
    public Enemy zombie;
    [SerializeField] private Transform moveToPositionTransform;
    [SerializeField] private Transform playerPosition;
    private NavMeshAgent navMeshAgent;
    void Start()
    {
        zombie = GetComponent<Enemy>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
            navMeshAgent.destination = playerPosition.position;
            //rotate towrad player
            //transform.LookAt(playerPosition);
            navMeshAgent.acceleration = 10f;
    }
}
