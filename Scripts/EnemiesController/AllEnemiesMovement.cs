using UnityEngine;
using System.Collections;
using System;

public class AllEnemiesMovement : MonoBehaviour
{

    public Transform player;
    Animator anim;
    NavMeshAgent nav;
    NavMeshPath path;
    [SerializeField]
    //Tipo de Emenigo
    public enum EnemyType { Boss, Swashbuckler, Shooter, Kamikaze, Melee };
    public EnemyType enemyType;

    private AllEnemiesSound allEnemiesSound;

    public int health = 100;
    public bool Dead = false; 
    public bool Hurting = false; 
    public bool isAShooter = false;

    //Distancias de Accion
    public float distance = 0f;
    public int startMovingDistance = 15;
    public int throwDistance = 7;
    public int attackDistance = 3;

    //Etados del Enemigo
    private bool isAttacking = false;
    private bool isMoving = false;
    private bool throwing = false;
    private bool looking = false;
    private bool patroling = false;
    Vector3 destiny;
    Vector3 origin;
    Vector3 playerLastPositon;
    Vector3 playerCurrentPositon;

    //Propieades de disparo
    public Transform shotSpawner;
    private float nextShot;
    public float shotRate = 3f;
    public int shots = 1;
    public GameObject projectile;

    //Propiedades de attaque cuerpo a cuerpo
    private float nextAttack;
    public float AttackRate = 2f;
    public GameObject attack;
    public GameObject hitter;

    //Velocidad de movimiento
    private Vector3 Speed;

    public float RunningSpeed = 3.5f;
    public float walkingSpeed = 1.2f;

    //Objetivo
    //public Transform destiny;

    void Awake()
    {
        allEnemiesSound = GetComponent<AllEnemiesSound>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        path = new NavMeshPath();

        switch (enemyType)
        {
            case EnemyType.Boss:
                health = 150;
                break;
            case EnemyType.Kamikaze:
                health = 30;
                break;
            case EnemyType.Melee:
                health = 40;
                break;
            case EnemyType.Shooter:
                health = 30;
                break;
            case EnemyType.Swashbuckler:
                health = 50;
                break;
        }

        origin = transform.position;

        destiny = transform.position - (transform.forward * 3);

        //Se define el enemigo es un attacante a distancia
        if (enemyType != EnemyType.Kamikaze && enemyType != EnemyType.Melee)
        {
            isAShooter = true;
        }

        //Se iguala la distancia de attacke a la de disparo para los enmigos tipo cuerpo a cuerpo para 
        //que corran hasta esta a distancia de attaque
        if (!isAShooter)
        {
            throwDistance = attackDistance;

        }
        if (enemyType == EnemyType.Boss)
        {
            hitter.SetActive(false);
        }
        nav.speed = walkingSpeed;

        StartCoroutine("UpdateEnemy");
    }


    IEnumerator UpdateEnemy()
    {
        while (!Dead)
        {
            //Se calcula la distacia al jugador
            distance = Vector3.Distance(transform.position, player.position);

            playerCurrentPositon = player.transform.position;

            //Se reanuda el movimiento del enemigo
            if (!isAttacking && isMoving && !patroling)
            {
                nav.SetDestination(player.position);
                nav.Resume();
                anim.SetBool("walk", isMoving);
                if (distance > throwDistance && enemyType == EnemyType.Boss)
                {
                    allEnemiesSound.enemiesDetectingSound();
                }

            }
            else if (isAttacking)//Se esper hasta que acabe la animacion del attaque
            {
                transform.LookAt(player.position);
                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            }

            //Se establece la velocidad y la animacion de correr
            if (distance > throwDistance && enemyType == EnemyType.Boss)
            {
                StartCoroutine("IaView");
                nav.CalculatePath(player.position, path);
                //print("path.status" + path.status);

                if (path.status != NavMeshPathStatus.PathComplete)
                {
                    isMoving = false;
                    nav.Stop();
                }
                else
                {
                    //allEnemiesSound.enemiesDetectingSound();

                    isMoving = true;
                    nav.SetDestination(player.position);
                    nav.speed = RunningSpeed;
                }
                anim.SetBool("Running", isMoving);
                anim.SetBool("walk", false);

            }
            else
            {
                StartCoroutine("IaView");
                //Se establece la velocidad y la animacion de caminar
                if (isMoving) { 
                nav.speed = walkingSpeed;
                }
                anim.SetBool("Running", false);
                anim.SetBool("walk", isMoving);

                //ATTACK
                //Se inicia el attaque a la distancia definida
                if (distance < attackDistance && enemyType != EnemyType.Shooter)
                {
                    anim.SetBool("walk", false);

                    //Se decide si el attaque cuerpo a cuerpo es critico o normal
                    if (Time.time > nextAttack)
                    {
                        
                        nextAttack = Time.time + AttackRate;
                        Attack("Attack");
                    }
                    else if (!isAttacking)
                    {
                        Attack("Punch");
                    }

                }
                //THROW
                //Se inician los attaques a distancia en la distancia definida
                else if (distance < throwDistance && isAShooter && looking)
                {
                    //lookTo(player.position);
                    //transform.LookAt(player.position);
                    StartCoroutine("IaView");
                    //Si se ha cumplido el tiempo de espera de la siguiente rafaga attaca
                    if (Time.time > nextShot && looking)
                    {
                        anim.SetBool("walk", false);
                        nextShot = Time.time + shotRate;
                        //Si es de tipo Shooter se queda quieto
                        if (enemyType == EnemyType.Shooter)
                        {
                            isMoving = false;
                        }

                        Attack("Throw");
                    }
                }
                //START MOVING
                //Se comienza a mover el enemigo
                else if (enemyType != EnemyType.Boss && distance < startMovingDistance)
                {

                    
                    StartCoroutine("IaView");


                    if (!isAShooter)
                    {
                        nav.speed = RunningSpeed;
                        anim.SetBool("Running", true);
                        anim.SetBool("walk", false);
                    }
                    //Se calcula estado dela ruta para alcanzar al jugador
                    nav.CalculatePath(player.position, path);

                    //print("looking" + looking);
                    if (path.status != NavMeshPathStatus.PathComplete || !looking)
                    {
                        isMoving = false;
                        nav.Stop();

                        Patrol();

                    }
                    else
                    {
                            allEnemiesSound.enemiesDetectingSound();
                       if (looking)
                        {
                            if(playerLastPositon != playerCurrentPositon)
                            { 
                                playerLastPositon = playerCurrentPositon;
                                //print("playerLastPositon" + playerLastPositon);
                                destiny = playerLastPositon;
                            }
                        }
                        patroling = false;
                        isMoving = true;
                        nav.SetDestination(player.position);
                    }

                }
                else if(enemyType != EnemyType.Boss)
                {
                    isMoving = false;
                    looking = false;
                    nav.Stop();
                    
                    Patrol();
                }

            }
            yield return new WaitForEndOfFrame();
        }

    }

    public void TakeDamage(int damage)
    {
        StartCoroutine("TakingDamage", damage);
    }

    IEnumerator TakingDamage(int damage)
    {
        health -= damage;
        if (health > 0 && !Hurting)
        {
            Hurting = true;
            AttackEnd();
            Attack("Damage");
            allEnemiesSound.enemiesHurtingSound();
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            isAttacking = false;
            startMovingDistance = Convert.ToInt32(distance + 2);
            if (isAShooter && (distance - 2) > throwDistance) { 
                throwDistance = Convert.ToInt32(distance - 2);
            }
            
            Hurting = false;

        }
        else if(!Dead)
        {
            Dead = true;
            Attack("Dead");
            allEnemiesSound.enemiesDyingSound();
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            
            
            Destroy(gameObject, 1.5f);
            CollectibleManager.Instance.spawnCollectible(transform);

        }
    }

    IEnumerator IaView()
    {
        Vector3 playerview = new Vector3(player.position.x, player.position.y + 1, player.position.z);
        Vector3 enemyView = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        Vector3 EtoPDirection = (playerview - enemyView).normalized;

        Ray viewRay = new Ray(enemyView, EtoPDirection);
        RaycastHit viewHit;

        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (Physics.Raycast(viewRay, out viewHit, startMovingDistance))
        {
            Debug.DrawLine(enemyView, viewHit.point, Color.red);
            if (viewHit.collider.tag == "Player" )
            {
                if (angle < 90)
                {

                    looking = true;
                }
            }
            else
            {
                looking = false;
            }
        }
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }

    private void lookTo(Vector3 Point)
    {
        Vector3 direction = Point - transform.position;
        Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation,  15 * Time.deltaTime);
    }


    private void Patrol()
    {
        float destinyDistance = Vector3.Distance(transform.position, destiny);
        NavMeshPath pathdes = new NavMeshPath();
        float timeToMove = 5;
        float originDistance = Vector3.Distance(transform.position, origin);
        float timeToOrigin = 20;
        
        if(destiny == playerLastPositon && destinyDistance > 1)
        {
            patroling = true;
            timeToMove += Time.time + 5;

        } else if (destinyDistance < 2f || !patroling)
        {
            destiny = transform.position - ((transform.right * 2) + transform.forward * 5);
            patroling = true;
        }

        if (originDistance > 10 && timeToOrigin < Time.time  && destiny != playerLastPositon)
        {
            destiny = origin;
            timeToMove += Time.time + 20;
        }
        nav.CalculatePath(destiny, pathdes);
        //print("pathdes.status" + pathdes.status);

        nav.SetDestination(destiny);
        if (pathdes.status != NavMeshPathStatus.PathComplete )
        {
            //isMoving = false;
            //nav.Stop();
            destiny = transform.position + (transform.right /*+ transform.forward*/ * 2);
            //lookTo(destiny);
            nav.SetDestination(destiny);
            timeToMove = Time.time + 5;

        }
        else
        {
            isMoving = true;
            nav.Resume();
        }

    }


    //Efectua el attaque
    private void Attack(string action)
    {
        nav.Stop();
        //nav.speed = 0;
        isAttacking = true;
        //Delimita el tipo de attaque
        if (action == "Throw")
        {
            throwing = true;
            //lookTo(player.position);
            transform.LookAt(player.position);
        }
        else if (action == "Punch")
        {
            //lookTo(player.position);
            transform.LookAt(player.position);
            if (enemyType == EnemyType.Boss)
            {
                hitter.SetActive(true);
            }
            
                allEnemiesSound.enemiesAttackingSound("Punch");
            
            //isAttacking = false;
        }
        anim.SetTrigger(action);
    }

    IEnumerator Attacking()
    {
        //Genera los proyectiles
        if (throwing)
        {
            for (int i = 0; i < shots; i++)
            {
                Vector3 playerHit = new Vector3(player.position.x, player.position.y + 1, player.position.z);
                shotSpawner.LookAt(playerHit);
                
                allEnemiesSound.enemiesAttackingSound("Throw");
                
                Instantiate(projectile, shotSpawner.position, shotSpawner.rotation);
                yield return new WaitForSeconds(0.2f);
            }
        }
        else//Genera los posibles objetos de un attaque critico
        {

            attack.SetActive(true);
            Instantiate(attack, transform.position, transform.rotation);
            if (enemyType == EnemyType.Kamikaze)
            {
                for (int i = 0; i < 3; i++)
                {
                    allEnemiesSound.enemiesAttackingSound("Attack");
                    yield return new WaitForSeconds(0.1f);
                }
                Destroy(gameObject, 0);
            } else
            {
                allEnemiesSound.enemiesAttackingSound("Attack");
            }
          
        }
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }


    //Finaliza el attaque restableciendo los valores
    private void AttackEnd()
    {
        isAttacking = false;
        if (throwing && enemyType != EnemyType.Shooter)
        {
            nav.Resume();
            throwing = false;
        }
        if (enemyType == EnemyType.Boss)
        {
            hitter.SetActive(false);
            attack.SetActive(false);
        }
    }

    //Desplaza al enemigo para corregir desplazaminetos de la animacion
    private void AttackMove()
    {
        nav.speed = RunningSpeed;
        nav.Resume();
        attack.SetActive(true);
    }
}