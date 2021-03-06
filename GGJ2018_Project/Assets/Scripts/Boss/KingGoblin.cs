﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingGoblin : MonoBehaviour
{
	[SerializeField]
	private ObjectEntity entity;
	[SerializeField]
	private GameObject toSpawn;
	[SerializeField]
	private float intervalSpawn;
	[SerializeField]
	private Animator myAnimator;
	private Rigidbody2D myRigidBody;
	private float gravity;
	private MapColumn map;
	private int column;
	private float currentTime;

	[SerializeField]
	private AudioSource deathBossGoblin;

	[SerializeField]
	private GameObject fx_death;
	[SerializeField]
	private GameObject fx_hurth;
	[SerializeField]
	private GameObject fx_apparition;
	[SerializeField]
	private GameObject fx_tp;
	[SerializeField]
	private List<GameObject> hearts;

	private void Start()
	{
		map = FindObjectOfType<MapColumn>();
		myRigidBody = GetComponent<Rigidbody2D>();
		NameSelector nameSelector = FindObjectOfType<NameSelector>();
		entity.SetName(nameSelector.GetRandom("BOSS"));

		gravity = 9.81f;

		Vector3 pos = transform.position;
		pos.x = map.transform.position.x;
		transform.position = pos;
		JumpTo(map.GetNbrColumn() - 1);
		currentTime = intervalSpawn;
	}

	private void HitBodyColor()
	{
		GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, Color.white, Time.deltaTime * 5.0f);
	}

	private void Update()
	{
		HitBodyColor();

		Vector3 dir = GameManager.Instance.Player.transform.position - transform.position;
		GetComponent<SpriteRenderer>().flipX = dir.x < 0.0f;

		if (GameManager.Instance.Player.GetComponent<ObjectEntity>().GetValue("ALIVE") == "TRUE")
			currentTime -= Time.deltaTime;
		else
			currentTime = intervalSpawn;
		if (currentTime < 0.0f)
		{
			currentTime = intervalSpawn;
			SpawnGoblin();
		}

		Gravity();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.transform.tag != "Attack")
			return;
		string life = entity.GetValue("LIFE");
		int l = 0;
		if (int.TryParse(life, out l))
		{
			l -= other.transform.parent.GetComponent<PlayerController>().GetComponentInChildren<PlayerWeapon>().GetCurrentWeapon().damage;


			AudioSource source = GetComponent<AudioSource>();
			if (source == null)
			{
				source = gameObject.AddComponent<AudioSource>();
			}
			source.clip = other.transform.parent.GetComponent<PlayerController>().GetComponentInChildren<PlayerWeapon>().GetCurrentWeapon().touch;
			source.loop = false;
			source.volume = 0.1f;
			if (source.clip != null)
				source.Play();


			int i = 0;
			foreach (GameObject heart in hearts)
			{
				if (i < l)
				{
					heart.SetActive(true);
				}
				else
				{
					heart.SetActive(false);
				}
				i++;
			}

			entity.SetValue("LIFE", l.ToString());
			if (l < 1)
			{
				if (enabled)
					myAnimator.SetTrigger("Dead");
				enabled = false;
				myRigidBody.velocity = new Vector2(0, myRigidBody.velocity.y);
				Instantiate(fx_death, transform.position, Quaternion.identity);
				Instantiate(deathBossGoblin, transform.position, Quaternion.identity);
			}
			else
				Instantiate(fx_hurth, transform.position, Quaternion.identity);
		}
		if (l > 0)
			JumpTo(column == 0 ? map.GetNbrColumn() - 1 : 0);
	}

	public void Gravity()
	{
		Vector3 moveDirection = myRigidBody.velocity;

		moveDirection.y -= gravity * Time.deltaTime;
		myRigidBody.velocity = moveDirection;
	}

	private void JumpTo(int column)
	{
		Vector3 pos = map.PositionColumn(column);
		pos.y = GameManager.Instance.Player.transform.position.y;
		transform.position = pos;
		this.column = column;
		//myAnimator.SetTrigger("Jump");
		Instantiate(fx_tp, transform.position, Quaternion.identity);
	}

	public void SpawnGoblin()
	{
		int columnSpawn = column == 0 ? 4 : 7;
		Vector3 direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;

		Instantiate(toSpawn, transform.position + direction.normalized * 2.0f, Quaternion.identity).GetComponent<GoblinController>().distanceFollow = 30;
		Instantiate(fx_apparition, transform.position + direction.normalized * 2.0f, Quaternion.identity);
		myAnimator.SetTrigger("Spawn");
	}
}
