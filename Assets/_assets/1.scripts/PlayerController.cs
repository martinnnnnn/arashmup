using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
using TMPro;
using DG.Tweening;




public class PlayerController : MonoBehaviour/*, IPunObservable*/
{
    public float walkSpeed;
    [SerializeField] float smoothTime;

    Vector2 smoothMoveVelocity;
    Vector2 moveAmount;

    GameManager gameManager;

    Rigidbody2D rigidBody;
    SpriteRenderer spriteRenderer;
    PhotonView photonView;

    [HideInInspector] public Camera followCamera;

    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool moveAllowed = false;
    [HideInInspector] public bool fireAllowed = false;

    public float dashForce;
    public float dashRate;
    float timeSinceDash;
    Vector2 moveDir;
    bool dash;

    float timeSinceFire;

    ProgressBar dashProgressBar;
    ProgressBar fireProgressBar;

    WeaponController weaponController;
    BoosterController boosterController;

    [HideInInspector] public float currentWalkSpeed;
    [HideInInspector] public float currentDashRate;

    void Start()
    {
        currentWalkSpeed = walkSpeed;
        currentDashRate = dashRate;

        gameManager = GameObject.FindObjectOfType<GameManager>();

        rigidBody = GetComponent<Rigidbody2D>();

        photonView = GetComponent<PhotonView>();

        weaponController = GetComponent<WeaponController>();
        boosterController = GetComponent<BoosterController>();

        if (photonView.IsMine)
        {
            photonView.RPC("RPC_SetColor", RpcTarget.All, Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            dashProgressBar = gameManager.uiManager.dashProgressBar;
            fireProgressBar = gameManager.uiManager.fireProgressBar;
        }
        else
        {
            Destroy(rigidBody);
        }
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
        UpdateProgressBars();
    }

    void UpdateProgressBars()
    {
        if (dashProgressBar == null)
        {
            return;
        }

        dashProgressBar.current = Mathf.Min(timeSinceDash / currentDashRate * dashProgressBar.maximum, dashProgressBar.maximum);

        if (fireProgressBar == null)
        {
            return;
        }

        fireProgressBar.current = Mathf.Min(timeSinceFire / weaponController.GetFireRate() * fireProgressBar.maximum, fireProgressBar.maximum);
    }

    void Move()
    {
        moveDir = Vector2.zero;

        if (!moveAllowed)
        {
            return;
        }

        timeSinceDash += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && timeSinceDash > currentDashRate)
        {
            dash = true;
            timeSinceDash = 0f;
            dashProgressBar.current = 0;
        }

        moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (dash)
        {
            dash = false;
            rigidBody.velocity = moveDir * dashForce;
        }
        else
        {
            moveAmount = Vector2.SmoothDamp(moveAmount, moveDir * currentWalkSpeed, ref smoothMoveVelocity, smoothTime);
            rigidBody.velocity = moveAmount;
        }
    }


    void Fire()
    {
        timeSinceFire += Time.deltaTime;

        if (Input.GetMouseButton(0) && fireAllowed && !isDead && timeSinceFire > weaponController.GetFireRate())
        {
            timeSinceFire = 0.0f;
            Vector2 worldPosition = followCamera.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector2 direction = (worldPosition - new Vector2(transform.position.x, transform.position.y)).normalized;

            Vector3 position = transform.position + new Vector3(direction.x, direction.y, transform.position.z) * 0.9f;

            photonView.RPC("RPC_Fire", RpcTarget.All, position, direction);
        }
    }

    [PunRPC]
    void RPC_Fire(Vector3 position, Vector2 direction)
    {
        Collider2D[] toIgnore = new Collider2D[]
        {
            GetComponent<Collider2D>(),
        };

        weaponController.Fire(position, direction, toIgnore);
    }

    public void ReceiveDamage(int damage)
    {
        if (photonView.IsMine && !isDead)
        {
            if (boosterController.IsDamageDone(damage))
            {
                photonView.RPC("RPC_SetDead", RpcTarget.All, true);
            }
        }
    }

    [PunRPC]
    void RPC_SetDead(bool value)
    {
        isDead = true;
        SpriteRenderer visual = GetComponentInChildren<SpriteRenderer>();
        visual.color = new Color(visual.color.r, visual.color.g, visual.color.b, 0.1f);
        Destroy(GetComponent<CircleCollider2D>());

        gameManager.IncreaseDead(photonView.IsMine);
        if (gameManager.CheckEnd())
        {
            foreach (PlayerController player in FindObjectsOfType<PlayerController>())
            {
                if (player.photonView.IsMine)
                {
                    player.fireAllowed = false;
                    player.moveAllowed = false;
                }
            }
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