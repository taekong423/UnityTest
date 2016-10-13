﻿using UnityEngine;
using System.Collections;

public class Rafflesia : MonoBehaviour {

    public float firstForce;
    public float secondForce;
    public Vector2 pushDirection;
    GameManager gm;
	// Use this for initialization
	void Start () {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {
            Rigidbody2D m_rigidbody = other.gameObject.GetComponent<Rigidbody2D>();
            if (gm.flags["DefeatBossPig"] == true)
            {
                m_rigidbody.velocity = Vector2.zero;
                m_rigidbody.AddForce(Vector2.up * secondForce, ForceMode2D.Impulse);
            }
            else
            {
                m_rigidbody.velocity = Vector2.zero;
                m_rigidbody.AddForce(pushDirection * firstForce, ForceMode2D.Impulse);
            }
            
        }
    }
}
