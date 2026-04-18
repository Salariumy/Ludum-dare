using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerControl : MonoBehaviour
{

    Rigidbody2D rigidbody2D;
    enum Freequnce
    {
        attack,
        detect
    }
    [SerializeField] Collider2D land;
    [SerializeField] float attackCD = 0.2f;
    [SerializeField] GameObject attackWave;
    Freequnce current = Freequnce.attack;
    float attackTimer = 0f;
    // Update is called once per frame\
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        transform.position = new Vector3(transform.position.x + Time.deltaTime * PlayerData.playerMoveSpeed, transform.position.y, transform.position.z);
        attackTimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryJump();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryAttack();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryChangeFreequncy();
        }
    }
    void TryJump()
    {
        if (rigidbody2D.IsTouching(land))
        {
            rigidbody2D.AddForce(new Vector2(0f, 300f));
        }
    }
    void TryAttack()
    {
        if (attackTimer > attackCD)
        {
            switch (current)
            {
                case Freequnce.attack:
                    Instantiate(attackWave, transform.position, transform.rotation);
                    break;
                case Freequnce.detect:
                    //Instantiate(detect)
                    break;

                default:
                    break;
            }

            attackTimer = 0f;
        }
    }
    void TryChangeFreequncy()
    {
        current++;
        if(current > Freequnce.detect)
        {
            current = 0;
        }
        Debug.Log(current);
    }
}