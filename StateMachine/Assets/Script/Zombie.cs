using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{

    private StateMachine brain;

    private Animator animator;
    
    [SerializeField] private UnityEngine.UI.Text stateNote; 
    private NavMeshAgent agent;
    //private Player player;
    private GameObject player;
    private bool isPlayerNear;
    public int distanceToChase; 
    private bool isWithinAttackRange;
    public int distanceToAttack;

    private float idleTimer; 
    
    // Start is called before the first frame update
    void Start()
    {
        //player = FindObjectOfType<Player>();
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        brain = GetComponent<StateMachine>();

        isPlayerNear = false;
        distanceToChase = 5; 
        isWithinAttackRange = false;
        distanceToAttack = 1; 
        
        brain.PushState(Idle, OnIdleEnter, OnIdleExit); //set our starting state
    }

    // Update is called once per frame
    void Update()
    {
        isPlayerNear = Vector3.Distance(transform.position, player.transform.position) < distanceToChase;
        isWithinAttackRange = Vector3.Distance(transform.position, player.transform.position) < distanceToAttack;
    }

    void OnIdleEnter()
    {
        stateNote.text = "Idle";
        agent.ResetPath();
    }
    
    void Idle()
    {
        if (isPlayerNear)
        {
            brain.PushState(Chase,OnChaseEnter,onChaseExit);
        }
        
        //set up a timer and have our enemy enter wander after X amount of time
        idleTimer -= Time.deltaTime; //time the frame took to execute
        if (idleTimer <= 0)
        {
            brain.PushState(Wander,OnWanderEnter,OnWanderExit);
            idleTimer = Random.Range(3, 10); 
        }
    }

    void OnIdleExit()
    {
        
    }

    void OnWanderEnter()
    {
        stateNote.text = "Wander";
        animator.SetBool("Chase",true);
        float distanceFromCurrentPosition = 10f; 
        Vector3 wanderDirection = (Random.insideUnitSphere * distanceFromCurrentPosition) + transform.position; //current position of enemy offset by unitspehre calc 
        //we need to make sure it will use the navmesh
        //similiar to physics raycast
        NavMeshHit navMeshHit; 
        NavMesh.SamplePosition(wanderDirection, out navMeshHit, 3f,NavMesh.AllAreas);
        Vector3 destination = navMeshHit.position;  //destination for my agent
        agent.SetDestination(destination);
    }
    
    void Wander()
    {
        if (agent.remainingDistance <= 0.25f) //we're close to the destination
        {
            agent.ResetPath();
            brain.PushState(Idle,OnIdleEnter,OnIdleExit);
        }

        if (isPlayerNear)
        {
            brain.PushState(Chase,OnChaseEnter,onChaseExit);
        }
    }

    void OnWanderExit()
    {
        animator.SetBool("Chase",false);
    }

    void OnChaseEnter()
    {
        stateNote.text = "Chase";
        animator.SetBool("Chase",true);
    }
    
    void Chase()
    {
        agent.SetDestination(player.transform.position); //when you are in chase move towards the player's position
        //if the player moves too far from you go back into idle 
        if (Vector3.Distance(transform.position, player.transform.position) >distanceToChase+0.5)
        {
            brain.PushState(Idle,OnIdleEnter,OnIdleExit); 
        }
    }

    void onChaseExit()
    {
        animator.SetBool("Chase",false);
    }
}
