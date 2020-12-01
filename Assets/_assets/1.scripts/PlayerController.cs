using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
using TMPro;
using DG.Tweening;

// add rb

public class PlayerController : MonoBehaviour/*, IPunObservable*/
{
    [SerializeField] float sprintSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float smoothTime;

    Vector2 smoothMoveVelocity;
    Vector2 moveAmount;

    GameManager gameManager;

    Rigidbody2D rigidBody;
    SpriteRenderer spriteRenderer;
    PhotonView photonView;

    [HideInInspector] public Camera followCamera;

    ObjectPool bulletPoll;

    [HideInInspector] public bool moveAllowed = false;
    [HideInInspector] public bool fireAllowed = false;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();

        rigidBody = GetComponent<Rigidbody2D>();

        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            photonView.RPC("RPC_SetColor", RpcTarget.All, Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        }
        else
        {
            Destroy(rigidBody);
        }

        bulletPoll = Instantiate(Resources.Load<GameObject>(Path.Combine("Prefabs", "ObjectPool")), Vector3.zero, Quaternion.identity).GetComponent<ObjectPool>();
        bulletPoll.Setup(Resources.Load<GameObject>(Path.Combine("Prefabs", "Bullet")), 20);
    }

    [PunRPC]
    void RPC_SetColor(float r, float g, float b)
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = new Color(r, g, b, 1);
    }


    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Move();
        Fire();
    }

    void Move()
    {
        if (!moveAllowed)
        {
            return;
        }

        Vector2 moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))/*.normalized*/;
        moveAmount = Vector2.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }
    void Fire()
    {
        if (Input.GetMouseButtonDown(0) && fireAllowed && !dead)
        {
            Vector2 worldPosition = followCamera.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector2 direction = (worldPosition - new Vector2(transform.position.x, transform.position.y)).normalized;

            photonView.RPC("RPC_RemoteFire", RpcTarget.All, transform.position, direction);
        }
    }

    [PunRPC]
    void RPC_RemoteFire(Vector3 position, Vector2 velocity)
    {
        GameObject bullet = bulletPoll.GetNext();

        bullet.transform.position = position;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * 10, velocity.y * 10);

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Vector3 transDir = transform.TransformDirection(moveAmount);
        rigidBody.MovePosition(rigidBody.position + new Vector2(transDir.x, transDir.y) * Time.deltaTime);
    }

    bool dead = false;
    public void ReceiveDamage(int damage)
    {
        if (!dead)
        {
            SpriteRenderer visual = GetComponentInChildren<SpriteRenderer>();
            visual.color = new Color(visual.color.r, visual.color.g, visual.color.b, 0.1f);
            dead = true;
            Destroy(GetComponent<CircleCollider2D>());

            gameManager.IncreaseDead(photonView.IsMine);
            gameManager.CheckEnd();
        }

        //if (photonView.IsMine)
        //{
        //    gameManager.uiManager.youDied.gameObject.SetActive(true);
        //    gameManager.uiManager.leaveRoom.gameObject.SetActive(true);
        //}
        //else
        //{
        //    gameManager.CheckWin();
        //}
    }
}
