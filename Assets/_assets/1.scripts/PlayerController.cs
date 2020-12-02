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
        fireAllowed = true;

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

    [SerializeField] float fireRate;
    float timeSinceFire;
    void Fire()
    {
        timeSinceFire += Time.deltaTime;

        if (Input.GetMouseButton(0) && fireAllowed && !dead && timeSinceFire > fireRate)
        {
            timeSinceFire = 0.0f;
            Vector2 worldPosition = followCamera.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector2 direction = (worldPosition - new Vector2(transform.position.x, transform.position.y)).normalized;

            Vector3 position = transform.position + new Vector3(direction.x, direction.y, transform.position.z) * 0.8f;

            photonView.RPC("RPC_RemoteFire", RpcTarget.All, position, direction);
        }
    }

    [PunRPC]
    void RPC_RemoteFire(Vector3 position, Vector2 velocity)
    {
        GameObject bullet = bulletPoll.GetNext();

        bullet.transform.position = position;
        bullet.GetComponent<Rigidbody2D>().velocity = /*velocity * 10;*/new Vector2(velocity.x * 10, velocity.y * 10);

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
    }
}


//Vector2 networkPosition;
//Vector2 networkVelocity;
//float currentSpeed;
//double lastNetworkDataReceivedTime;
//public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//{
//    if (stream.IsWriting)
//    {
//        stream.SendNext(rigidBody.position);
//        stream.SendNext(rigidBody.velocity);
//        stream.SendNext(currentSpeed);
//    }
//    else
//    {
//        networkPosition = (Vector2)stream.ReceiveNext();
//        networkVelocity = (Vector2)stream.ReceiveNext();
//        currentSpeed = (float)stream.ReceiveNext();
//        lastNetworkDataReceivedTime = info.SentServerTime; //timestamp

//        // float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
//        networkPosition += networkVelocity * Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime)); ;
//    }
//}

//private void UpdateNetworkedPosition()
//{
//    float pingInSeconds = PhotonNetwork.GetPing() * 0.001f;
//    float timeSinceLastUpdate = (float)(PhotonNetwork.Time - lastNetworkDataReceivedTime);
//    float totalTimePassed = pingInSeconds + timeSinceLastUpdate; // lag

//    networkPosition += rigidBody.velocity * totalTimePassed;
//    networkVelocity += (networkVelocity - rigidBody.velocity) * Time.deltaTime * totalTimePassed * 50;

//    rigidBody.MovePosition(Vector3.MoveTowards(rigidBody.position, networkPosition, Time.deltaTime * currentSpeed));
//}