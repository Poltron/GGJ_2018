﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinSpawner : MonoBehaviour {

    bool hasBeenUsed;

    [SerializeField]
    GameObject goblin;

    [SerializeField]
    List<Transform> spawns;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name == "Player" && !hasBeenUsed)
        {
            hasBeenUsed = true;
            
            foreach(Transform spawn in spawns)
            {
                GameObject g = Instantiate(goblin, spawn.position, Quaternion.identity);
                g.GetComponent<GoblinController>().distanceFollow = 100;
            }
        }
    }
}
