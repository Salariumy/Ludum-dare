using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerControl : MonoBehaviour
{

    Rigidbody2D rigidbody2D;
    [SerializeField] Collider2D land;
    [SerializeField] float attackCD = 0.2f;
    [SerializeField] GameObject attackWave;
    float attackTimer = 0f;
    // Update is called once per frame\
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
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
            Instantiate(attackWave,transform.position,transform.rotation);
            attackTimer = 0f;
        }
    }
    void TryChangeFreequncy()
    {

    }
}
