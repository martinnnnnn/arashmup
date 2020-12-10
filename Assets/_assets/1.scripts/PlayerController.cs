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

        public StringVariable Name;

        [Header("Camera")]
        public CameraController FollowCamera;
        public Vector3Variable PlayerPosition;

        [Header("Walking")]
        public FloatReference WalkSpeedStandard;
        public FloatVariable WalkSpeed;

        [Header("Dashing")]
        public FloatReference DashRateStandard;
        public FloatReference DashForce;
        public FloatVariable DashRate;
        public FloatVariable DashElaspedTime;

        Vector2 moveDir;

        [Header("Firing")]
        public FloatVariable FireElaspedTime;
        public FloatReference FireRate;


        GameManager gameManager;

        Rigidbody2D rigidBody;
        PhotonView PV;

        WeaponController weaponController;
        BoosterController boosterController;

        bool isDead;
        bool mustDash;
        bool moveAllowed;
        bool fireAllowed;

        void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();

            isDead = false;
            mustDash = false;
            moveAllowed = false;
            fireAllowed = false;
        }

        void Start()
        {
            WalkSpeed.SetValue(WalkSpeedStandard);
            DashRate.SetValue(DashRateStandard);

            gameManager = FindObjectOfType<GameManager>();


            PV = GetComponent<PhotonView>();

            weaponController = GetComponent<WeaponController>();
            boosterController = GetComponent<BoosterController>();

            Name.Value = PV.Owner.NickName;

            if (!PV.IsMine)
            {
                Destroy(rigidBody);
                Destroy(FollowCamera.gameObject);
                Name = ScriptableObject.CreateInstance<StringVariable>();
                Name.Value = PV.Owner.NickName;
            }
        }

        public void OnGameInitialized()
        {
            moveAllowed = true;
        }

        public void OnCountdownOver()
        {
            fireAllowed = true;
        }

        void Update()
        {
            if (PV.IsMine)
            {
                Move();
                Fire();
            }
            else
            {
                //UpdateNetworkedPosition();
            }
        }

        void Move()
        {
            moveDir = Vector2.zero;

            if (!moveAllowed)
            {
                return;
            }


            DashElaspedTime.ApplyChange(Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Space) && DashElaspedTime.Value > DashRate.Value)
            {
                mustDash = true;
                DashElaspedTime.SetValue(0);
                
            }

            moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        void FixedUpdate()
        {
            if (!PV.IsMine)
            {
                return;
            }

            if (mustDash)
            {
                mustDash = false;
                rigidBody.velocity = moveDir * DashForce;
            }
            else
            {
                rigidBody.velocity = moveDir * WalkSpeed.Value;
            }

            PlayerPosition.SetValue(transform.position);
        }


        void Fire()
        {
            FireElaspedTime.ApplyChange(Time.deltaTime);

            if (Input.GetMouseButton(0) && fireAllowed && !isDead && FireElaspedTime.Value > FireRate)
            {
                FireElaspedTime.Value = 0.0f;

                Vector2 direction = (FollowCamera.GetWorldPoint() - new Vector2(transform.position.x, transform.position.y)).normalized;
                Vector3 position = transform.position + new Vector3(direction.x, direction.y, transform.position.z) * 0.9f;

                PV.RPC(RPC_Functions.Fire, RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, position, direction);
            }
        }

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
                    PV.RPC(RPC_Functions.SetDead, RpcTarget.All, true);
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