using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
using TMPro;
using DG.Tweening;

using Hashtable = ExitGames.Client.Photon.Hashtable;


/*
 * switch to variable refs :
 *      - walk speed
 *      - dash speed
 *      - fire cd
 * 
 * nouvelle architecture : 
 *  - Player
 *      - PlayerController
 *      - Player CameraController
 * 
 * 
 * */

namespace Arashmup
{
    public class PlayerController : MonoBehaviour/*, IPunObservable*/
    {
        public float walkSpeed;
        [SerializeField] float smoothTime;

        GameManager gameManager;

        Rigidbody2D rigidBody;
        PhotonView PV;

        [HideInInspector] public Camera followCamera;

        bool isDead = false;
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


        void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            currentWalkSpeed = walkSpeed;
            currentDashRate = dashRate;

            gameManager = FindObjectOfType<GameManager>();


            PV = GetComponent<PhotonView>();

            weaponController = GetComponent<WeaponController>();
            boosterController = GetComponent<BoosterController>();

            GetComponentInChildren<TMP_Text>().text = PV.Owner.NickName;

            if (PV.IsMine)
            {
                dashProgressBar = gameManager.uiManager.dashProgressBar;
                fireProgressBar = gameManager.uiManager.fireProgressBar;
            }
            else
            {
                //Destroy(rigidBody);
            }
        }

        void Update()
        {
            if (PV.IsMine)
            {
                Move();
                Fire();
                UpdateProgressBars();
            }
            else
            {
                //UpdateNetworkedPosition();
            }
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
            if (!PV.IsMine)
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
                rigidBody.velocity = moveDir * currentWalkSpeed;
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

                PV.RPC(RPC_Fire_Name, RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, position, direction);
            }
        }


        static string RPC_Fire_Name = "RPC_Fire";
        [PunRPC]
        void RPC_Fire(int actorNumber, Vector3 position, Vector2 direction)
        {
            Collider2D[] toIgnore = new Collider2D[]
            {
            GetComponent<Collider2D>(),
            };

            weaponController.Fire(actorNumber, position, direction, toIgnore);
        }

        public void ReceiveDamage(int actorNumber, int damage)
        {
            if (PV.IsMine && !isDead)
            {
                if (boosterController.IsDamageDone(damage))
                {
                    IncreaseKillFeed(actorNumber);
                    PV.RPC(RPC_SetDead_Name, RpcTarget.All, true);
                }
            }
        }

        static string RPC_SetDead_Name = "RPC_SetDead";
        [PunRPC]
        void RPC_SetDead(bool value)
        {
            isDead = true;
            SpriteRenderer visual = GetComponentInChildren<SpriteRenderer>();
            visual.color = new Color(visual.color.r, visual.color.g, visual.color.b, 0.1f);
            Destroy(GetComponent<CircleCollider2D>());

            gameManager.IncreaseDead(PV.IsMine);
            if (gameManager.CheckEnd())
            {
                foreach (PlayerController player in FindObjectsOfType<PlayerController>())
                {
                    if (player.PV.IsMine)
                    {
                        player.fireAllowed = false;
                        player.moveAllowed = false;
                    }
                }
            }
        }

        void IncreaseKillFeed(int actorNumber)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Count(); ++i)
            {
                if (PhotonNetwork.PlayerList[i].ActorNumber == actorNumber)
                {
                    int killCount = 0;
                    if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey(CustomPropertiesKeys.KillCount))
                    {
                        killCount = (int)PhotonNetwork.PlayerList[i].CustomProperties[CustomPropertiesKeys.KillCount];
                    }
                    Hashtable hash = new Hashtable();
                    hash.Add(CustomPropertiesKeys.KillCount, ++killCount);
                    PhotonNetwork.PlayerList[i].SetCustomProperties(hash);
                    break;
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
//        Debug.Log("sending " + rigidBody.position + "/" + rigidBody.velocity);
//        stream.SendNext(rigidBody.position);
//        stream.SendNext(rigidBody.velocity);
//        stream.SendNext(currentSpeed);
//    }
//    else
//    {

//        networkPosition = (Vector2)stream.ReceiveNext();
//        networkVelocity = (Vector2)stream.ReceiveNext();
//        Debug.Log("reading " + networkPosition + "/" + networkVelocity);
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
//    float totalTimePassed = pingInSeconds + timeSinceLastUpdate;

//    networkPosition += rigidBody.velocity * totalTimePassed;
//    networkVelocity += (networkVelocity - rigidBody.velocity) * Time.deltaTime * totalTimePassed * 50;

//    rigidBody.MovePosition(Vector3.MoveTowards(rigidBody.position, networkPosition, Time.deltaTime * currentSpeed));
//}

//Vector2 networkPosition;
//Vector2 networkVelocity;
//double lastNetworkDataReceivedTime;
//public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//{
//    if (stream.IsWriting)
//    {
//        stream.SendNext(rigidBody.position);
//        stream.SendNext(rigidBody.velocity);
//        stream.SendNext(currentWalkSpeed);
//    }
//    else
//    {

//        networkPosition = (Vector2)stream.ReceiveNext();
//        networkVelocity = (Vector2)stream.ReceiveNext();
//        currentWalkSpeed = (float)stream.ReceiveNext();
//        lastNetworkDataReceivedTime = info.SentServerTime; //timestamp

//        // float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
//        networkPosition += networkVelocity * Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime)); ;
//    }
//}

//private void UpdateNetworkedPosition()
//{
//    float pingInSeconds = PhotonNetwork.GetPing() * 0.001f;
//    float timeSinceLastUpdate = (float)(PhotonNetwork.Time - lastNetworkDataReceivedTime);
//    float totalTimePassed = pingInSeconds + timeSinceLastUpdate;

//    networkPosition += rigidBody.velocity * totalTimePassed;
//    networkVelocity += (networkVelocity - rigidBody.velocity) * Time.deltaTime * totalTimePassed * 50;

//    rigidBody.MovePosition(Vector3.MoveTowards(rigidBody.position, networkPosition, Time.deltaTime * 50));
//}