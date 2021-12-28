using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rigidBody;
    private Animator animator;

    public Transform attackPoint;
    public float attackRange = 0.5f;

    public LayerMask enemyLayers;

    public float runningSpeed = 2f;
    public float jumpForce = 8f;


    private const string STATE_ON_THE_GROUND = "isOnTheGround";
    private const string STATE_JUMP = "isJump";
    private const string STATE_RUNNING = "isRunning";

    public LayerMask groundMask;

    public float jumpRaycastDistance = 1.0f;

    public float dashForce;
    private float currentDashTimer;
    public float startDashTimer;
    private int dashDirection;
    private bool isDashing;

    public float dashDistance = 5f;
    private bool isLeft = false;
    private bool isRight = true;
    

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentDashTimer = startDashTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Pasa x aqui");
            animator.SetBool(STATE_JUMP, true);
            Jump();
        }

        animator.SetBool(STATE_ON_THE_GROUND, IsTouchingTheGround());

        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }

        Debug.DrawRay(this.transform.position, Vector2.down * jumpRaycastDistance, Color.red);

        checkDirection();


    }

    bool IsTouchingTheGround()
    {
        if (Physics2D.Raycast(this.transform.position,
                            Vector2.down,
                            jumpRaycastDistance,
                            groundMask))
        {

            return true;
        }

        return false;
    }

    void Jump()
    {
        //float jumpForceFactor = jumpForce;
        //if (superjump && manaPoints > SUPERJUMP_COST)
        //{
        //    manaPoints -= SUPERJUMP_COST;
        //    jumpForceFactor *= SUPERJUMP_FORCE;
        //}

        if (IsTouchingTheGround())
        {
            //GetComponent<AudioSource>().Play();
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if(isDashing)
        {
            rigidBody.velocity = transform.right * dashDirection * dashForce;

            //currentDashTimer =
        }
    }

    void FixedUpdate()
    {
        //if (GameManager.sharedInstance.currentGameState == GameState.inGame)
        //{
        //    Move();
        //}
        //else
        //{
        //    //Time.timeScale = 0f;
        //    //rigidBody.Sleep();
        //    rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        //}



        Move();

        if(rigidBody.velocity.x != 0)
        {
            animator.SetBool(STATE_RUNNING, true);
        } else
        {
            animator.SetBool(STATE_RUNNING, false);
        }

        

    }

    void Move()
    {
        Vector3 localScale = transform.localScale;


        if (Input.GetKey(KeyCode.A))
        {
            rigidBody.velocity = new Vector2(-runningSpeed, rigidBody.velocity.y);
            dashDirection = 1;
            if (localScale.x > 0)
            {
                localScale.x *= -1;
                transform.localScale = localScale;
                isLeft = true;
                isRight = false;
                
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            rigidBody.velocity = new Vector2(runningSpeed, rigidBody.velocity.y);
            dashDirection = 2;
            if (localScale.x < 0)
            {
                localScale.x *= -1;
                transform.localScale = localScale;

                isLeft = false;
                isRight = true;

            }
        }



        
    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        Collider2D [] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //Damage Them
        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void checkDirection()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            float direction;
            if(isLeft)
            {
                direction = -1f;
            } else
            {
                direction = 1f;
            }
            StartCoroutine(makeDash(direction));
        }
    }

    IEnumerator makeDash(float direction)
    {
        isDashing = true;
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);
        rigidBody.AddForce(new Vector2(dashDistance * direction, 0f), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.4f);
        isDashing = false;


        Vector3 localScale = transform.localScale;


        if(localScale.x < 0)
        {
            rigidBody.velocity = new Vector2(-runningSpeed, rigidBody.velocity.y);
        } else
        {
            rigidBody.velocity = new Vector2(runningSpeed, rigidBody.velocity.y);
        }



        
    }
}