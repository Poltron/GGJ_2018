﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	#region isMovingRight
	private bool isMovingRight = false;

	public bool GetIsMovingRight()
	{
		return isMovingRight;
	}
	#endregion
	#region isMovingLeft
	private bool isMovingLeft = false;

	public bool GetIsMovingLeft()
	{
		return isMovingLeft;
	}
	#endregion
	#region isJumping
	private bool isKeyJump = false;

	public bool GetIsJumping()
	{
		return isKeyJump;
	}
	#endregion
	#region isAttacking
	private bool isKeyAttack = false;

	public bool GetIsAttacking()
	{
		return isKeyAttack;
	}
	#endregion
	#region Key
	[Header("Keys")]
	[SerializeField]
	private KeyCode keyRight;
	[SerializeField]
	private KeyCode keyLeft;
	[SerializeField]
	private KeyCode keyJump;
	[SerializeField]
	private KeyCode keyAttack;
	#endregion
	#region Jump
	[Header("Jump")]
	[SerializeField]
	private float jumpHeight;
	[SerializeField]
	private float jumpTime;
	#endregion

	[Header("Variables")]
	[SerializeField]
	private List<Transform> feets;
	[SerializeField]
	private float speed;
	[SerializeField]
	private GameObject attackCollider;

	private float defaulGravity = 9.81f;
	private Transform myTransform;
	private Rigidbody2D myRigidBody;
	private Animator myAnimator;
	private SpriteRenderer mySpriteRenderer;
	private float gravity;
	private bool justGrounded = false;
	private bool isOnGround;

    private bool isInputEnabled = true;

    private void Start()
	{
		myTransform = transform;
		myRigidBody = gameObject.GetComponent<Rigidbody2D>();
		myAnimator = GetComponent<Animator>();
		mySpriteRenderer = GetComponent<SpriteRenderer>();
		gravity = defaulGravity;
	}

	private void Update()
	{
		KeyUpdate();
		Gravity();
		Move();
		Jump();
		Attack();
		Animation();
	}

	public void KeyUpdate()
	{
        if (!isInputEnabled)
            return;

		if (Input.GetKey(keyRight))
			isMovingRight = true;
		else
			isMovingRight = false;

		if (Input.GetKey(keyLeft))
			isMovingLeft = true;
		else
			isMovingLeft = false;

		if (Input.GetKeyDown(keyJump))
			isKeyJump = true;
		else
			isKeyJump = false;

		if (Input.GetKeyDown(keyAttack))
			isKeyAttack = true;
		else
			isKeyAttack = false;
	}

	public void Gravity()
	{
		Vector3 moveDirection = myRigidBody.velocity;

		moveDirection.y -= gravity * Time.deltaTime;
		myRigidBody.velocity = moveDirection;
	}

	public void Move()
	{
		if (isMovingRight)
		{
			Vector3 moveDirection = myRigidBody.velocity;

			moveDirection.x = 1.0f * speed * 100f * Time.deltaTime;

			myRigidBody.velocity = moveDirection;
			return;
		}
		else if (isMovingLeft)
		{
			Vector3 moveDirection = myRigidBody.velocity;

			moveDirection.x = -1.0f * speed * 100f * Time.deltaTime;

			myRigidBody.velocity = moveDirection;
			return;
		}
		else
		{
			myRigidBody.velocity = new Vector2(0.0f, myRigidBody.velocity.y);
		}
	}

	public void Jump()
	{
		if (IsOnGround())
		{
			if (isKeyJump)
			{
				Vector3 moveDirection = myRigidBody.velocity;

				moveDirection.y = JumpForce(jumpHeight, jumpTime);
				myRigidBody.velocity = moveDirection;
			}
		}
	}

	public void Attack()
	{
		if(isKeyAttack)
		{
			attackCollider.SetActive(true);
			StartCoroutine(WaitForFrame());
		}
	}

	IEnumerator WaitForFrame()
	{
		yield return null;
		attackCollider.SetActive(false);
	}

	public float JumpForce(float height, float time)
	{
		gravity = CalculeGravity(height, time);
		return (4.0f * height) / time;
	}

	public bool IsOnGround()
	{
		foreach (var trans in feets)
		{
			RaycastHit2D[] ray = Physics2D.RaycastAll(trans.position, -Vector2.up, 0.1f);

			foreach (RaycastHit2D r in ray)
			{
				if (r.transform.tag == "Platform")
				{
					if (justGrounded == false && isOnGround == false)
						justGrounded = true;
					else
						justGrounded = false;
					isOnGround = true;
					return true;
				}
			}
		}
		isOnGround = false;
		return false;
	}

	private float CalculeGravity(float height, float time)
	{
		return (8.0f * height) / (time * time);
	}

    public void EnableInput()
    {
        isInputEnabled = true;
        Debug.Log("Player Input Enabled");
    }

    public void DisableInput()
    {
        isInputEnabled = false;
    }

	private void Animation()
	{
		if(Input.GetKeyDown(keyJump))
		{
			myAnimator.SetTrigger("Jump");
		}
		if(((Input.GetKeyUp(keyRight) || Input.GetKeyUp(keyLeft)) && IsOnGround() && !isMovingLeft && !isMovingRight)
			|| justGrounded && (!isMovingLeft && !isMovingRight))
		{
			myAnimator.SetTrigger("Idle");
		}
		if((Input.GetKeyDown(keyRight) || Input.GetKeyDown(keyLeft)) && IsOnGround() && (isMovingLeft || isMovingRight)
			|| justGrounded && (isMovingLeft || isMovingRight))
		{
			myAnimator.SetTrigger("Run");
		}

		if(isMovingLeft)
		{
			mySpriteRenderer.flipX = true;
		}
		else if(isMovingRight)
		{
			mySpriteRenderer.flipX = false;
		}
	}
}
