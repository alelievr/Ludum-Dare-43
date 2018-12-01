using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;



[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

    [Header("Basic setting")]
    public Text lifeText = null; 
    public bool facingLeft = false;
    public int life = 5;
    public float invulnTime = 1f;
    public string ouchtag = "ouch";
    public float timestunouch = 0.2f;
    public bool stunStopMove = true;

    [Header("Mobility and groundaison")]
    public float maxSpeed = 1f;
    float minSlideVelocity = 3f;
    float slimeVelocityIgnore = .5f;
    public float jumpPower = 10f;
    public float jumpIdle = .3f;
    public Vector3 groundPosition;
    public Vector2 groundSize;
    public float maxYVelocity = 8f;
    public float minYVelocity = -6f;
    public bool flying = false;

    [Header("Dash")]
	public float timedashinsc = 0.1f;
	public float distanceofdash = 10f;
	public bool isinvuindash = true;
	public float dashcd = 0.5f;

    [Header("Sound")]
    public AudioClip ouchClip;
    [Range(0, 1)] public float ouchVolume = 0.8f;
    public AudioClip jumpClip;
    [Range(0, 1)] public float jumpingVolume = 0.5f;
    public AudioClip TappingClip;
    [Range(0, 1)] public float tappingVolume = 0.5f;
    public AudioClip run;
    [Range(0, 1)] public float runVolume = 0.5f;    
    public AudioClip DieClip;
    [Range(0, 1)] public float DieVolume = 0.5f;

    [Header("Ouch")]
    public float ouchJumpMultPushX = 2;
    public float ouchJumpMultPushY = 4;

    [HideInInspector] public bool canJump = true;
    [HideInInspector] public bool istapping = false;
    [HideInInspector] public bool grounded;

    [HideInInspector] public float move = 0;
    [HideInInspector] public float movey = 0;

    [Header("Events")]
    public UnityEvent   onTakeDamage;

    Vector2 impacto = Vector2.zero;
    CinemachineVirtualCamera vcam;
    CinemachineBasicMultiChannelPerlin vcamperlin;
    Material spriteMaterial;
    bool isPlayer = false;
    bool canOuch = true;
    bool sliding = false;
    new Rigidbody2D rigidbody2D;
    protected SpriteRenderer spriteRenderer;
    protected Animator anim;
    [HideInInspector] public AudioSource audiosource;
    [HideInInspector] public AudioSource audiosource2;
    [HideInInspector] public bool TakingDamage = false;
    [HideInInspector] public bool IsOuchstun = false;
    float baseGravityScale;

    protected Collider2D  col;
    [HideInInspector] public bool isDead;

	[HideInInspector] public Vector2 lastCheckpoint = Vector2.negativeInfinity;
    int baseLayer;
    [HideInInspector] public Rigidbody2D rbparent = null;

    protected LayerMask groundLayer;
    [HideInInspector]public bool cannotmove = false;


    /*****************************************************************************************************************
                                                        INITIALISATION
    *****************************************************************************************************************/

    IEnumerator WaitForCam()
    {
        while (!vcam)
        {
            vcam = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
            yield return new WaitForEndOfFrame();
        }
        if (isPlayer)
        {
            vcam.Follow = (transform.parent) ?? transform;
            vcam.LookAt = (transform.parent) ?? transform;
        }
    }

    protected void reinit()
    {
        isdashing = false;
        anim.SetBool("Grounded", grounded);
		candash = true;
        rigidbody2D.gravityScale = baseGravityScale;
        gameObject.layer = baseLayer;
        if (lifeText)
            lifeText.text = life.ToString();
        IsOuchstun = false;
        isDead = false;

        StartCoroutine(WaitForCam());
        // CaBouge cbtmp = GetComponentInParent<CaBouge>();
        // if (cbtmp)
        //     rbparent = cbtmp.GetComponent<Rigidbody2D>();
    }

    protected void init()
    {
        groundLayer = 1 << (LayerMask.NameToLayer("Ground")) | 1 << (LayerMask.NameToLayer("GroundOneWay"));
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteMaterial = spriteRenderer.material;
        rigidbody2D = GetComponent<Rigidbody2D>();
        baseLayer = gameObject.layer;
        rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        anim = GetComponent<Animator>();
        audiosource = Camera.main.GetComponent<AudioSource>();
        audiosource2 = GetComponent<AudioSource>();
        if (tag == "Player")
            isPlayer = true;
        col = GetComponents<Collider2D>().Where(c => !c.isTrigger).FirstOrDefault();
        baseGravityScale = rigidbody2D.gravityScale;
        reinit();
    }

    // protected void OnEnable()
    // {
    //     reinit();
    // }

    void Start()
    {
        init();
    }


    /*****************************************************************************************************************
                                                        PAS RANGÉ
    *****************************************************************************************************************/

    protected void allCheck()
    {
         GroundCheck();
         SlideCheck();
    }

    public void DoingDamage(Vector2 pos)
    {
        if (tag == "player")
        {
            rigidbody2D.velocity = new Vector2(Mathf.Sign(transform.position.x - pos.x) * 10, rigidbody2D.velocity.y);
        }
    }
    IEnumerator Tapping()
    {
        if (istapping == false && isdashing == false)
        {
            float y = transform.localScale.y;
            if (flying && movey < 0)
                transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            if (TappingClip)
                audiosource.PlayOneShot(TappingClip, tappingVolume);
            istapping = true;
            anim.SetBool("Attack", true);

            yield return new WaitForSeconds(0.3f);
            anim.SetBool("Attack", false);
            yield return new WaitForSeconds(0.05f);
            StopTapping();
            if (flying)
                transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
        }
    }

    void    StopTapping()
    {
        StopCoroutine(Tapping());
        anim.SetBool("Attack", false);
        istapping = false;
    }

	bool isdashing = false;
    Vector2 oldV = Vector2.zero;
    Vector2 currentV = Vector2.zero;


    public void Move(float move, float movey = 0)
    {
        Vector2 newVCible = Vector2.zero;
		if (isdashing || rigidbody2D.bodyType == RigidbodyType2D.Static)
			return ;

        if (audiosource2 && run)
        {
            if (grounded && audiosource2.isPlaying == false && move != 0 && grounded)
            {
                audiosource2.loop = true;
                audiosource2.clip = run;
                audiosource2.volume = runVolume;
                audiosource2.Play();
            }
            else if ((move == 0 || !grounded) && audiosource2.clip == run)
            {
                audiosource2.clip = null;
                audiosource2.Stop();
            }
        }

        newVCible.x = Mathf.Clamp(move * maxSpeed, -maxSpeed, maxSpeed);
        newVCible.y = Mathf.Clamp((IsOnLadder) ? movey * maxSpeed : rigidbody2D.velocity.y, minYVelocity, maxYVelocity);

        if ((!istapping || isPlayer) && move > 0 && facingLeft)
            Flip();
        else if ((!istapping || isPlayer) && move < 0 && !facingLeft)
            Flip();
        
        rigidbody2D.velocity = (flying) ? Vector2.SmoothDamp(oldV, newVCible, ref currentV, 0.2f) : newVCible;
        oldV = rigidbody2D.velocity;

        anim.SetBool("ismoving", move != 0 || (IsOnLadder && movey != 0));
        anim.SetFloat("VelocityX", rigidbody2D.velocity.x);
        anim.SetFloat("VelocityY", rigidbody2D.velocity.y);

        if (rbparent)
            rigidbody2D.velocity += new Vector2(rbparent.velocity.x, (IsOnLadder) ? rbparent.velocity.y : 0);
        rigidbody2D.velocity += impacto;
    }

    Collider2D actualGround;
    protected virtual void GroundCheck()
    {
        if (flying)
        {
            IsOnLadder = true;
            return ;
        }
        if (groundPosition == Vector3.zero && groundSize == Vector2.zero)
        {
            
            grounded = true;
            return ;
        }
        RaycastHit2D[] results = new RaycastHit2D[10];
        int collisionNumber = Physics2D.BoxCastNonAlloc(transform.position + groundPosition, groundSize, 0, Vector2.down, results, .0f, groundLayer);

        grounded = collisionNumber != 0;
        if (collisionNumber != 0)
            actualGround = results.First().collider;
        collisionNumber = Physics2D.BoxCastNonAlloc(transform.position + groundPosition, groundSize, 0, Vector2.down, results, .0f, 1 << LayerMask.NameToLayer("Ladder"));
        grounded = grounded || collisionNumber != 0;
        if (IsOnLadder || move == 0 || movey != 0)
            IsOnLadder = collisionNumber != 0;
        rigidbody2D.gravityScale = (IsOnLadder) ? 0 : baseGravityScale;

        anim.SetBool("Grounded", grounded);
    }

    void SlideCheck()
    {
        // float arx = Mathf.Abs(rigidbody2D.velocity.x);
        // if (arx > minSlideVelocity && Input.GetKeyDown(KeyCode.DownArrow))
        //     sliding = true;
        // if (arx < minSlideVelocity || !grounded)
        //     sliding = false;

        // anim.SetBool("sliding", sliding);
    }

    protected void Flip()
    {
        facingLeft = !facingLeft;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }


    /*****************************************************************************************************************
                                                        OUCH & DIE
    *****************************************************************************************************************/

    int coroutineisplayingcount = 0;

    IEnumerator    waitbefordying()
    {
        yield return new WaitForSeconds(1);
        while (coroutineisplayingcount > 0)
            yield return new WaitForEndOfFrame();
        if (tag == "Player")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            // SaveLoad.Save();
        }
        GameObject.Destroy(gameObject);
    }

    void Die()
    {
        if (audiosource && DieClip)
            audiosource.PlayOneShot(DieClip, DieVolume);
        isDead = true;
        IsOuchstun = true;
        StopTapping();
        gameObject.layer = LayerMask.NameToLayer("TouchNothing");
        rigidbody2D.gravityScale = 0;
        cannotmove = true;
        anim.SetTrigger("death");
        StartCoroutine(waitbefordying());
    }

    void eventOnDie()
    {

    }

    public void ouch(Vector2 impact2)
    {
        onTakeDamage.Invoke();
        if (stunStopMove)
            rigidbody2D.velocity = Vector2.zero;
        canOuch = false;
        life--;
        if (lifeText)
            lifeText.text = life.ToString();
        StopTapping();
        if (impact2.x > 0 && !facingLeft)
            Flip();
        else if (impact2.x < 0 && facingLeft)
            Flip();
        if (audiosource && ouchClip)
            audiosource.PlayOneShot(ouchClip, ouchVolume);
        if (life < 1)
        {
            eventOnDie();
            Die();
        }
        else
        {

            StartCoroutine(ResetCanOuch());
            StartCoroutine(stunouch(impact2));
            StartCoroutine(impactoEffect());
        }
    }

    IEnumerator impactoEffect()
    {
		coroutineisplayingcount++;
        TakingDamage = true;
        vcamperlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        vcamperlin.m_AmplitudeGain = 0.25f;
        vcamperlin.m_FrequencyGain = 30;
        yield return new WaitForSeconds(0.1f);
        TakingDamage = false;
        yield return new WaitForSeconds(0.2f);
        vcamperlin.m_AmplitudeGain = 0;
        cannotmove = false;
        coroutineisplayingcount--;
    }

    IEnumerator stunouch(Vector2 impact)
    {
		coroutineisplayingcount++;
        anim.SetTrigger("Damage");
        IsOuchstun = true;
        if (stunStopMove)
            cannotmove = true;
        rigidbody2D.velocity = impact;
        spriteMaterial.SetFloat("_isflashing", 1);
        yield return new WaitForSeconds(timestunouch);
        spriteMaterial.SetFloat("_isflashing", 0);
        cannotmove = false;
        IsOuchstun = false;
        coroutineisplayingcount--;
    }

    IEnumerator ResetCanOuch()
    {
		coroutineisplayingcount++;
        yield return new WaitForSeconds(invulnTime);
        canOuch = true;
        coroutineisplayingcount--;
    }

    /*****************************************************************************************************************
                                                        JUMP
    *******************************************************************************************************************/

    IEnumerator JumpDelay()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpIdle);
        canJump = true;
    }

    public void tryjump(float jumppower)
    {
        if (grounded && canJump)
        {
            if (audiosource && jumpClip)
                audiosource.PlayOneShot(jumpClip, jumpingVolume);
            anim.SetTrigger("Jump");
            rigidbody2D.AddForce(new Vector2(0, jumppower), ForceMode2D.Impulse);
            StartCoroutine(JumpDelay());
            rigidbody2D.velocity = new Vector2(move * maxSpeed, Mathf.Clamp(rigidbody2D.velocity.y, minYVelocity, maxYVelocity));
        }
    }

    public void tryjump()
    {
        tryjump(jumpPower);
    }

    IEnumerator tryGoUnderCo()
    {
        Collider2D col2 = actualGround;

        Physics2D.IgnoreCollision(col, col2, true);
        yield return new WaitForSeconds(1);
        Physics2D.IgnoreCollision(col, col2, false);
    }

    public void tryGoUnder()
    {
        if (actualGround.usedByEffector == true && actualGround.GetComponent<PlatformEffector2D>() != null)
            StartCoroutine(tryGoUnderCo());
    }

    /*****************************************************************************************************************
														DASH
    *******************************************************************************************************************/

	bool candash = true;

	IEnumerator dash()
	{
		coroutineisplayingcount++;
		if (move == 0 && (!flying || movey == 0))
			Debug.Log(gameObject.name + " trying to dodge without moving");
		else if (candash == true && IsOuchstun == false)
		{
			candash = false;
			anim.SetBool("Dash", true);
            if (isinvuindash == false)
                gameObject.layer = 12;
            else
                gameObject.layer = 13;
               
			float timer = 0;
			float movebysecond = distanceofdash / timedashinsc;
			Vector2 sign = new Vector2(Utils.SignOr0(move), (flying) ? Utils.SignOr0(movey) : 0).normalized;
            isdashing = true;
			while (timer < timedashinsc)
			{
				// rigidbody2D.MovePosition(transform.position + new Vector3(sign * movebysecond * Time.deltaTime, 0, 0));
                rigidbody2D.velocity = sign * movebysecond * Time.fixedDeltaTime;
				timer += Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}
            isdashing = false;
            rigidbody2D.velocity = new Vector2(0, 0);
            if (isinvuindash == false)
                gameObject.layer = baseLayer;
            else
                gameObject.layer = baseLayer;
            anim.SetBool("Dash", false);
			yield return new WaitForSeconds(dashcd);
			candash = true;
		}
		coroutineisplayingcount--;
	}


    /*****************************************************************************************************************
                                                        UPDATE
    *****************************************************************************************************************/

    public enum Key{Left, Right, Up, Down, Jump, Dash, Attack};
    public bool[] key = new bool[10];

    public void DestroyKey(Key k)
    {
        key[(int)k] = false;
    }
    public void AddKey(Key k)
    {
        key[(int)k] = true;
    }


    void Update()
    {
        if (life < 0)
            return;
        if (cannotmove == true)
            return;
        move =  Mathf.Clamp(Input.GetAxisRaw("Horizontal"), (key[(int)Key.Left]) ? -Mathf.Infinity : 0,(key[(int)Key.Right]) ?  Mathf.Infinity : 0);
        movey = Input.GetAxisRaw("Vertical");
       bool iscrouching = false;
       // move = (istapping) ? move / 2 : move;


        if (key[(int)Key.Jump] && (Input.GetKey(KeyCode.Space)) && movey < -0.6f)
            tryGoUnder();
        else if (key[(int)Key.Jump] && Input.GetKey(KeyCode.Space))
            tryjump();
        else if(movey < -0.6f && grounded)
        {
            move = 0;
            iscrouching = true;
        }
        anim.SetBool("iscrouching", iscrouching);

        if (key[(int)Key.Attack] && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            StartCoroutine(Tapping());

		if (key[(int)Key.Dash] && Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightShift))
            StartCoroutine(dash());
    }

    protected virtual void FixedUpdate()
    {
        PCFixedUpdate();
    }
        Collider2D[] ctmp = new Collider2D[52];

    protected void PCFixedUpdate()
    {
        impacto = Vector2.zero;
        if (Physics2D.OverlapCircleNonAlloc(transform.position, 1.4f, ctmp, 1 << gameObject.layer) > 1)
        {
            Collider2D col;
            if (col = ctmp.FirstOrDefault(c => c && !c.transform.IsChildOf(transform) && c.gameObject != gameObject && !c.isTrigger))
            {
                impacto = (transform.position - col.transform.position ) * 2f;
                if (!flying)
                    impacto.y = 0;
            }
            
        }
        if (life <= 0)
            return;
        allCheck();

        if (cannotmove == true)
            return;

        Move(move, movey);
    }

    /*****************************************************************************************************************
                                                        COLLISION
    *****************************************************************************************************************/
    protected bool IsOnLadder = false;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Slime")
        {
            if (rigidbody2D.velocity.y < slimeVelocityIgnore)
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
        }
        OnTriggerStay2D(other.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnTriggerStay2D(collision.collider);
    }

    virtual protected void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "OS")
        {
            life = 0;
            Die();
            return;
        }

        if (canOuch && (other.tag == ouchtag || other.tag == "ouchbam"))
        {
            PlayerController oponnent = other.GetComponentInParent<PlayerController>();
            if (oponnent)
                oponnent.DoingDamage(transform.position);
            ouch(new Vector2(Mathf.Sign(transform.position.x - other.transform.position.x) * ouchJumpMultPushX, ouchJumpMultPushY));
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + groundPosition, groundSize);
    }
}
