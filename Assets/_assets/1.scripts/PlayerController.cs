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
        
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));

        photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            Destroy(rigidBody);
        }

        bulletPoll = Instantiate(Resources.Load<GameObject>(Path.Combine("Prefabs", "ObjectPool")), Vector3.zero, Quaternion.identity).GetComponent<ObjectPool>();
        bulletPoll.Setup(Resources.Load<GameObject>(Path.Combine("Prefabs", "Bullet")), 20);
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
        if (Input.GetMouseButtonDown(0) && fireAllowed)
        {
            GameObject bullet = bulletPoll.GetNext();

            bullet.transform.position = transform.position;

            Vector2 worldPosition = followCamera.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector2 direction = (worldPosition - new Vector2(transform.position.x, transform.position.y)).normalized;

            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x * 10, direction.y * 10);

            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());

            photonView.RPC("RemoteFire", RpcTarget.Others, transform.position, direction);
        }
    }

    [PunRPC]
    void RemoteFire(Vector3 position, Vector2 velocity)
    {
        GameObject bullet = bulletPoll.GetNext();

        bullet.transform.position = position;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * 6, velocity.y * 6);

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

        gameObject.SetActive(false);

        dead = dead ? dead : photonView.IsMine;
        gameManager.UpdateUI(dead);

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
