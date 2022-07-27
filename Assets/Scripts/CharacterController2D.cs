using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 20f;                          // Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
	[SerializeField] private int jumpCount = 2;                                 // number of jumps a player has

	//special moves
	[SerializeField] private float dashSpeed;
	private float dashTime;
	[SerializeField] private float dashDuration;
	[SerializeField] private float dashCooldown;
	private float dashCDTimer = 0f;
	private bool currDashing = false;

	const float k_GroundedRadius = .2f;  // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;             // Whether or not the player is grounded.
	private int m_JumpsLeft; // number of jumps currently available to the player
	const float k_CeilingRadius = .2f;   // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	public bool m_FacingRight = true;    // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private bool m_UncrouchPermitted = true;

	// healthbar to ensure it is not inverted when player switches directions
	public HealthBar healthBar;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	public UnityEvent OnDoubleJumpEvent;

	public UnityEvent OnJumpEvent;

	public UnityEvent OnDashEnterEvent;
	public UnityEvent OnDashExitEvent;

	private void Awake()
	{
		dashTime = dashDuration;

		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_Rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();

		if (OnDoubleJumpEvent == null)
			OnDoubleJumpEvent = new UnityEvent();

		if (OnJumpEvent == null)
			OnJumpEvent = new UnityEvent();

		if (OnDashEnterEvent == null)
			OnDashEnterEvent = new UnityEvent();

		if (OnDashExitEvent == null)
			OnDashExitEvent = new UnityEvent();
	}

	public void Move(float move, bool crouch, bool jump)
	{

		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
				{
					OnLandEvent.Invoke();
				}
			}
		}

		// If attempting to uncrouch, check to see if the character can stand up
		if (!crouch && m_JumpsLeft == jumpCount)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			m_UncrouchPermitted = true;
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				m_UncrouchPermitted = false;
            }
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch || !m_UncrouchPermitted)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
			else if(m_UncrouchPermitted)
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
			// m_Rigidbody2D.AddForce(new Vector2(targetVelocity.x/10f, 0), ForceMode2D.Impulse);
			

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if ((m_Grounded || m_JumpsLeft > 0) && jump)
		{
			m_wasCrouching = false;
			OnCrouchEvent.Invoke(false);
			if (m_CrouchDisableCollider != null)
				m_CrouchDisableCollider.enabled = true;
			m_Grounded = false;
			//set player velocity to be vertical
			m_Rigidbody2D.velocity = Vector2.up * m_JumpForce;
			OnJumpEvent.Invoke();
			if(m_JumpsLeft < jumpCount)
            {
				OnDoubleJumpEvent.Invoke();
            }

			m_JumpsLeft--;
		}

        if (m_Grounded)
        {
			m_JumpsLeft = jumpCount;
        }
	}

    public void useAbilities(bool dashInput)
    {
		if(dashCDTimer > 0)
        {
			dashCDTimer -= Time.fixedDeltaTime;
        }
        if (dashInput && dashCDTimer <= 0)
			currDashing = true;

        if (currDashing)
        {
			if(dashTime <= 0)
            {
				OnDashExitEvent.Invoke();
				dashTime = dashDuration;
				m_Rigidbody2D.velocity = Vector2.zero;
				currDashing = false;
				dashCDTimer = dashCooldown;
            }
            else
            {
				OnDashEnterEvent.Invoke();
				dashTime -= Time.deltaTime;
				if (m_FacingRight)
				{
					m_Rigidbody2D.velocity = Vector2.right * dashSpeed;
				}
				else
				{
					m_Rigidbody2D.velocity = Vector2.left * dashSpeed;
				}
			}
			
		}
        
    }

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		healthBar.transform.SetParent(null);
		transform.Rotate(0f, 180f, 0f);
		healthBar.transform.SetParent(gameObject.transform);
	}

	// function to reset player movement after a stun
	public void ResetPlayerMovement(float delay)
    {
		Invoke("RPM", delay);
    }

	// timing function to reset player movement
	private void RPM()
    {
		gameObject.GetComponent<PlayerMovement>().enabled = true;
    }
}