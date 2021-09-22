using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GoTo : MonoBehaviour
{
    public float Speed;
    private float SpeedRate = 1f;
    private int Right;

    public Transform Target;
    private float Distance = 0.5f;

    private Rigidbody2D rig;
    private Seeker seeker;
    private Path path;
    private Vector2 force;
    private Vector2 Direction;

    private float nextWaypointdistance = 5f;
    private int currentWaypoint = 0;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rig = GetComponent<Rigidbody2D>();
    }

    void GetTarget(Transform x)
    {
        Target = x;
    }

    void FindPath()
    {
        if (Target != null && seeker.IsDone())
        {
            seeker.StartPath(rig.position, Target.position, PathComplete);
        }
    }
    IEnumerator PathFinding()
    {
        while(Target != null)
        {
            if (Target.position.x - rig.position.x < 0)
                Right = 1;
            else
                Right = -1;
            rig.AddTorque(100f * Right);
            SpeedRate += 0.1f;
            FindPath();
            yield return new WaitForSeconds(0.1f);
        }
        yield break;
    }

    void PathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    void Go()
    {
        if (path == null || Target == null)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }
        if (Vector2.Distance(transform.position, Target.position) > Distance)
        {
            Direction = ((Vector2)path.vectorPath[currentWaypoint] - rig.position).normalized;
            force = Direction * Speed * 1000f * Time.deltaTime;
            rig.AddForce(force);
        }
        else
        {
            Target = null;
            return;
        }
        float distance = Vector2.Distance(rig.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointdistance)
        {
            currentWaypoint++;
        }
    }

    void Action(Transform target)
    {
        Target = target;
        StartCoroutine("PathFinding");
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (rig.bodyType != RigidbodyType2D.Dynamic)
            return;
        if(collision.gameObject.layer == 12 || collision.gameObject.layer == 14)
        {
            if(rig.velocity.magnitude < 5f)
                Target = null;
        }
    }

    void FixedUpdate()
    {
        Go();

        
    }
}
