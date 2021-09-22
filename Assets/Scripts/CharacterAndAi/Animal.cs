using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Animal : MonoBehaviour
{
    public enum TapeType { Wild, CutePet, AttackPet };
    public TapeType Tape;
    public enum AnimalType {Predator, Neutral, Victim};
    public AnimalType PredatorType;
    public enum State {Idle, Walk, CatchMan, Catch, RunAway, RunFromMan, Follow, Wait, Play};
    public State AnimalState = State.Idle;

    public int HP;
    public int MaxHp;
    public int Satiety;
    public int LVL;
    public GameObject Food;
    private GameObject PrevFood;
    private float Speed;
    public float WalkSpeed;
    public float WarSpeed;
    public float Sense;
    public int Damage;
    public float AttackSpeed;
    private float AttackTime;
    private float Range = 20f;
    public float DistanceToStop;
    public bool AfraidPeople;
    public bool Agressive;
    private bool Dead;
    private GameObject AnimalEnemy;
    private GameObject ManEnemy;

    public GameObject Owner;
    public Vector2 Home;

    public GameObject[] Drop;
    public GameObject[] RareDrop;

    private SpriteRenderer sprite;
    private Rigidbody2D rig;
    private Animator anim;
    private Seeker seeker;
    public UI_Ai UI;
    public LevelSystem levelSystem;

    private Vector2 Target;
    
    private Path path;
    private Vector2 Direction;
    private Vector2 RandomDirection;
    private float nextWaypointdistance = 1f;
    private int currentWaypoint = 0;

    public Vector2 Direct(Vector2 obj)
    {
        if (obj == null)
            return (new Vector2(1, 0));
        return new Vector2(obj.x - transform.position.x, obj.y - transform.position.y).normalized;
    }

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        seeker = GetComponent<Seeker>();

        UI.GetComponent<UI_Ai>().GetName("Rabbit");
        UI.GetComponent<UI_Ai>().GetLVL(LVL.ToString());
        HP = HP * (1 + LVL / 10);
        MaxHp = MaxHp * (1 + LVL / 10);
        UI.GetComponent<UI_Ai>().GetMaxHP(MaxHp);
        UI.GetComponent<UI_Ai>().GetHP(HP);
        UI.GetComponent<UI_Ai>().HpTurn(false);

        InvokeRepeating("FindPath", 0f, 0.5f);
        InvokeRepeating("RandomWay", 0f, 1f);
    }
    private void RandomWay()
    {
        RandomDirection = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
    }
    private void FindPath()
    {
        if (Target != null && seeker.IsDone())
        {
            seeker.StartPath(rig.position, Target, PathComplete);
        }
    }
    void PathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void GoTo()
    {
        if (path == null)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
            return;
        float CurrentSpeed = Speed;
        if (Vector2.Distance(transform.position,Target) > DistanceToStop)
        {
            Direction = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
            Vector2 force = (Direction + RandomDirection * 0.1f) * CurrentSpeed * 100f * Time.deltaTime;
            rig.AddRelativeForce(force);
        }
        else
        {
            rig.velocity *= 0.9f;
        }
        float distance = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointdistance)
        {
            currentWaypoint++;
        }
    }
    public void RunTo()
    {
        Vector2 Direction = (Target - (Vector2)transform.position).normalized;
        Vector2 force = Direction * Speed;
        if ((Target - (Vector2)transform.position).magnitude > 0.25f)
        {
            rig.AddRelativeForce(force);
        }
    }
    public void RunAway()
    {
        Vector2 Direction = (Target - (Vector2)transform.position).normalized;
        Vector2 force = Direction * -Speed;
        rig.AddRelativeForce(force);
    }

    private void Animation()
    {
        anim.SetFloat("Velocity", rig.velocity.magnitude);
        
    }
    public void FlipTo(float x)
    {
        if(x > 0)
        {
            sprite.flipX = false;
        }
        else
        {
            sprite.flipX = true;
        }
    }

    public void GetHit(int Damage, Transform obj)
    {
        HP -= Mathf.RoundToInt(Damage * (100 - LVL) / 50f);
        levelSystem.ParticleSystem.CreateParticle(Particle.ParticlesType.Blood, transform.position, obj.transform.up);
        if (obj.root.tag == "Player")
        {
            if (UI != null)
            {
                UI.HpTurn(true);
                UI.GetHP(HP);
                
            }
        }
    }
    public void GetDistanceHit(int Damage, Transform obj)
    {
        GetHit(Damage, obj);
    }
    public void GetKick(Vector2 Direction)
    {
        if (HP > 0)
        {
            rig.AddForce(Direction);
        }

    }

    public void Defend()
    {

    }
    public IEnumerator LooseMan()
    {
        yield return new WaitForSeconds(5f);
        ManEnemy = null;
        UI.HpTurn(false);
    }
    public IEnumerator LoosePredator()
    {
        yield return new WaitForSeconds(5f);
        AnimalEnemy = null;
    }

    private IEnumerator Walk()
    {
        if(((Vector2)transform.position - Home).magnitude > 20f)
        {
            Target = Home;
        }
        else
        {
            Target = Home + new Vector2(Random.Range(-Range, Range), Random.Range(-Range, Range));
        }
        int Frames = 0;
        while(((Vector2)transform.position - Target).magnitude > 1f)
        {
            GoTo();
            Frames++;
            if (Frames >= 10 && rig.velocity.magnitude < 0.5f)
            {
                AnimalState = State.Idle;
                break;
            }
            if(ManEnemy != null || AnimalEnemy != null)
            {
                AnimalState = State.Idle;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        AnimalState = State.Idle;
        yield break;
    }
    private IEnumerator Wait(float Time)
    {
        anim.SetBool("Sit", true);
        yield return new WaitForSeconds(Time);
        AnimalState = State.Idle;
        anim.SetBool("Sit", false);
        yield break;
    }
    private IEnumerator Play()
    {
        anim.SetBool("Play", true);
        yield return new WaitForSeconds(3f);
        AnimalState = State.Idle;
        anim.SetBool("Play", false);
        yield break;
    }

    private IEnumerator Die()
    {
        
        for(int i = 0; i < Drop.Length; i++)
        {
            Instantiate(Drop[i], transform.position, transform.rotation, null);
        }
        for (int i = 0; i < RareDrop.Length; i++)
        {
            Instantiate(RareDrop[i], transform.position, transform.rotation, null);
        }
        anim.Play("Die");
        yield break;
    }

    private void CheckFood()
    {
        Collider2D Info = Physics2D.OverlapCircle(transform.position, 3f, 1 << 9);
        if (Info != null && Food != null && Info.gameObject != PrevFood)
        { 
            GameObject item = Info.gameObject;
            PrevFood = item;
            int itemIndex = Info.transform.GetComponent<Item>().Index;
            int FoodIndex = Food.GetComponent<Item>().Index;
            if (itemIndex == FoodIndex)
            {
                Satiety += item.GetComponent<ItemFood>().Satiety;
                item.GetComponent<Item>().Destroy();
            }
        }
    }
    private void StateLogic()
    {
        switch(Tape)
        {
            case TapeType.CutePet:
                {
                    if (Owner != null)
                    {

                    }
                    else
                    {
                        Tape = TapeType.Wild;
                    }
                }
                break;
            case TapeType.Wild:
                {
                    if(ManEnemy != null)
                    {
                        Speed = WarSpeed;
                        Target = ManEnemy.transform.position;
                        if (Agressive)
                        {
                            AnimalState = State.CatchMan;
                        }
                        else
                        {
                            AnimalState = State.RunAway;
                        }
                        anim.SetBool("Sit", false);
                        anim.SetBool("Play", false);
                    }
                    else if (AnimalEnemy != null)
                    {
                        Speed = WarSpeed;
                        if (PredatorType == AnimalType.Predator)
                        {
                            Target = AnimalEnemy.transform.position;
                            AnimalState = State.Catch;
                        }
                        else if (PredatorType == AnimalType.Victim)
                        {
                            Target = AnimalEnemy.transform.position;
                            AnimalState = State.RunAway;
                        }
                        anim.SetBool("Sit", false);
                        anim.SetBool("Play", false);
                    }
                    else if (AnimalState == State.Idle)
                    {
                        int Rand = Random.Range(0, 4);
                        if (Rand <= 1)
                        {
                            Speed = WalkSpeed;
                            AnimalState = State.Walk;
                            StartCoroutine(Walk());
                        }
                        else if (Rand == 2)
                        {
                            rig.velocity = rig.velocity / 5f;
                            AnimalState = State.Wait;
                            StartCoroutine(Wait(3f));
                        }
                        else if (Rand == 3)
                        {
                            rig.velocity = rig.velocity / 5f;
                            AnimalState = State.Play;
                            StartCoroutine(Play());
                        }
                    }
                }
                break;
        }
    }
    private void Main()
    {
        switch (Tape)
        {
            case TapeType.Wild:
                {
                    Collider2D[] Info = Physics2D.OverlapCircleAll(transform.position, Sense, 1 << 13);
                    if (ManEnemy != null && Info.Length == 1)
                    {
                        StartCoroutine(LooseMan());
                    }
                    if (AnimalEnemy != null && Info.Length == 1)
                    {
                        StartCoroutine(LoosePredator());
                    }
                    for (int i = 1; i < Info.Length; i++)
                    {
                        Transform animal = Info[i].transform.root;
                        if (PredatorType != AnimalType.Neutral && animal != transform)
                        {
                            if (animal.tag == "Animal" && animal.GetComponent<Animal>().PredatorType != AnimalType.Neutral)
                            {
                                AnimalEnemy = animal.gameObject;
                                break;
                            }
                        }
                        if (AfraidPeople && (animal.tag == "Player" || animal.tag == "Enemy"))
                        {
                            if (ManEnemy == null || ((transform.position - ManEnemy.transform.position).magnitude >
                                                      (transform.position - animal.transform.position).magnitude))
                            {
                                ManEnemy = animal.gameObject;
                                StopCoroutine("Wait");
                                StopCoroutine("Play");
                                StopCoroutine("Walk");
                            }
                        }
                    }

                    if (AnimalState == State.Idle)
                    {
                        
                    }
                    else if(AnimalState == State.Catch)
                    {
                        FlipTo(rig.velocity.x);
                        RunTo();
                        if (AnimalEnemy != null)
                        {
                            if ((transform.position - AnimalEnemy.transform.position).magnitude < 2f)
                            {
                                if (Time.time > AttackTime)
                                {
                                    AnimalEnemy.GetComponent<Animal>().GetHit(Damage, transform);
                                    AnimalEnemy.GetComponent<Rigidbody2D>().velocity /= 2f;
                                    rig.velocity /= 2f;
                                    AttackTime = Time.time + AttackSpeed;
                                }
                            }
                            
                        }
                        
                    }
                    else if(AnimalState == State.CatchMan)
                    {
                        FlipTo(rig.velocity.x);
                        RunTo();
                        if ((transform.position - ManEnemy.transform.position).magnitude < 1f)
                        {
                            if (ManEnemy.tag == "Player")
                            {
                                ManEnemy.GetComponent<Character>().GetHit(Damage, transform);
                                ManEnemy.GetComponent<Character>().GetKick(-Direct(ManEnemy.transform.position) * 100f);
                            }
                            else if(ManEnemy.tag == "Enemy")
                            {
                                ManEnemy.GetComponent<Ai>().GetHit(Damage, transform);
                                ManEnemy.GetComponent<Ai>().GetKick(-Direct(ManEnemy.transform.position) * 100f);
                            }
                        }
                       
                    }
                    else if(AnimalState == State.RunAway)
                    {
                        FlipTo(rig.velocity.x);
                        if (((Vector2)transform.position - Target).magnitude < Sense * 1.5f)
                        {
                            RunAway();
                        }
                    }
                    else if (AnimalState == State.Walk)
                    {
                        FlipTo(rig.velocity.x);
                    }
                    else if (AnimalState == State.Wait)
                    {
                        rig.velocity = rig.velocity * 0.9f;
                    }
                    else if (AnimalState == State.Play)
                    {
                        rig.velocity = rig.velocity * 0.9f;
                    }
                    break;
                }
        }

    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
    private void FixedUpdate()
    {

        if (HP <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            CheckFood();
            Animation();
            StateLogic();
            Main();
        }
    }
}
