using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class HumanAI : MonoBehaviour
{


    public float speed = 200f;


    private Rigidbody2D rb;

    private Human human;
    public float moveRange = 4f;
    public float minTimeBetweenMoves = 1f;
    public float maxTimeBetweenMoves = 3f;
    public float nextPointDistanceThreshold = 0.5f;

    public Seeker seeker;
    private Path path;
    private int currentWaypoint;
    //private Animator animator;

    public bool isMoving = false;
    public bool isEscaping = false;
    private AstarPath astar;
    public  bool isRunningAway; //is he is actually running away
    public bool hasTube = false;
    public GameObject fluidPrefab;
    public float fluidGenerateTime = 0.3f;
    private float fluidGenerateTimer;
    public AnimatorOverrideController meleeAnimatorController;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        human = GetComponent<Human>();
        seeker = GetComponent<Seeker>();
        //animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        astar = AstarPath.active;
        if (astar == null)
        {
            Debug.LogError("No active AstarPath found in the scene.");
            return;
        }
        
        switch (GetComponent<Human>().levelDesignInfo.move[0])
        {
            case "random":
                break;
            case "patrol":
                var patrolPoints = GetComponent<Human>().levelDesignInfo.move.ToList();
                patrolPoints.RemoveAt(0);
                
                    var patrolId = patrolPoints[patrolIndex];
                transform.position = HumanSpawner.Instance.GetPoint(patrolId);
                break;
            case "idle":
                break;
        }
        
        FindNextPath();
    }

    private void Update()
    {
        if (path == null)
        {
            isRunningAway = false;
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count) //reach the end of the current path
        {
            isMoving = false;
            FindNextPath();
            isEscaping = false;
            isRunningAway = false;
            return;
        }

        // if (GetComponent<RunAwayFromTarget>().isRunningAway)
        // {
        //     return;
        // }

        

        if (hasTube)
        {
            
            fluidGenerateTimer += Time.deltaTime;
            if (fluidGenerateTimer >= fluidGenerateTime)
            {
                fluidGenerateTimer = 0;
                Instantiate(fluidPrefab, transform.position, Quaternion.identity);
            }
        }
        //actual move code with animation
        Vector3 direction = ((Vector2)(path.vectorPath[currentWaypoint] - transform.position)).normalized;
        // animator.SetTrigger("move");
        // animator.SetFloat("horizontal",direction.x);
        // animator.SetFloat("verticle",direction.y);
        float distanceToWaypoint = Vector2.Distance((Vector2)transform.position, (Vector2)path.vectorPath[currentWaypoint]);

        if (distanceToWaypoint < nextPointDistanceThreshold)
        {
            currentWaypoint++;
        }
        else
        {
            //rb.MovePosition((Vector3)rb.position+(direction * (Time.deltaTime * (human.isSuffering ? speed / 2:speed))));
            transform.position += direction * Time.deltaTime * speed;//(human.isSuffering ? speed / 2:speed);
            GetComponent<CharacterRenderController>().UpdateDir(direction);
        }
    
        
    }


    // public void Escape()
    // {
    //     isEscaping = true;
    //     FindNextRandomPath();
    // }
    private int patrolIndex = 0;
    public void FindNextPath()
    {
        if (GetComponent<Human>().isPausedMoving)
        {
            return;
            
        }
        switch (GetComponent<Human>().levelDesignInfo.move[0])
        {
            case "random":
                FindNextRandomPath();
                break;
            case "patrol":
                var patrolPoints = GetComponent<Human>().levelDesignInfo.move.ToList();
                patrolPoints.RemoveAt(0);
                for (int i = 1; i < patrolPoints.Count; i++)
                {
                    patrolIndex++;
                    if (patrolIndex >= patrolPoints.Count)
                    {
                        patrolIndex = 0;
                    }

                    var patrolId = patrolPoints[patrolIndex];
                    
                    if (MoveToPosition(HumanSpawner.Instance.GetPoint(patrolId)))
                    {
                        break;
                    }
                }
                break;
            case "idle":
                break;
        }
    }
    public void FindNextRandomPath()
    {

        if (!astar)
        {
            return;
        }

        int test = 100;
        while (test-->0)
        {
            Vector3 randomPosition = GetRandomPosition(transform.position, moveRange);
            GraphNode startNode = astar.GetNearest(transform.position).node;
            GraphNode endNode = astar.GetNearest(randomPosition).node;
            bool isPathPossible = PathUtilities.IsPathPossible(startNode, endNode);
            if (isPathPossible)
            {
                seeker.StartPath(transform.position, randomPosition, OnPathComplete);
                break;
            }
        }
        //seeker.StartPath(transform.position, randomPosition, OnPathComplete);
        
        // }
        return;
       
    }

    bool MoveToPosition(Vector3 position)
    {
        
        GraphNode startNode = astar.GetNearest(transform.position).node;
        GraphNode endNode = astar.GetNearest(position).node;
        bool isPathPossible = PathUtilities.IsPathPossible(startNode, endNode);
        if (isPathPossible)
        {
            seeker.StartPath(transform.position, position, OnPathComplete);
            return true;
        }

        return false;
    }

    private float collectDistance = 5;
    private float knifeSearchDistance = 10;

    public void StopSeekPath()
    {
        seeker.CancelCurrentPathRequest();
        path = null;
    }

    public void RestartSeek()
    {
        if (GetComponent<Human>().levelDesignInfo.move[0] == "random")
        {
            
        }
        seeker.CancelCurrentPathRequest();
        FindNextPath();
    }
    
    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            isMoving = true;
        }
        else
        {
            Debug.Log("path error");
        }
    }

    private Vector3 GetRandomPosition(Vector3 center, float range)
    {
        if (human.levelDesignInfo.move.Count > 1)
        {
            
            var area = HumanSpawner.Instance.area;
            return HumanSpawner.GetRandomPointInBoxCollider(area.Find(human.levelDesignInfo.spawn)
                .GetComponent<BoxCollider2D>());
        }
        else
        {
            
            float x = Random.Range(center.x - range, center.x + range);
            float z = Random.Range(center.z - range, center.z + range);
    
            return new Vector3(x, z, 0);
        }
        
    }
    //
    // private Vector3 GetRandomPositionAwayFromTarget(Vector3 targetPosition, float minRange, float maxRange)
    // {
    //     Vector3 randomDirection = Random.insideUnitSphere.normalized;
    //     float randomDistance = Random.Range(minRange, maxRange);
    //
    //     Vector3 randomPosition = targetPosition + randomDirection * randomDistance;
    //     randomPosition.z = targetPosition.z;
    //
    //     return randomPosition;
    // }

}
