using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;


// FSM for dashing / walking

namespace Arashmup
{
    public class CharacterMovement : MonoBehaviour
    {
        public Vector3Variable Position;
        public Vector2Variable Direction;
        public BoolVariable MoveAllowed;
        public GameInputs Inputs;

        [Header("Walking")]
        public GenericReference<float> WalkSpeedStandard;
        public FloatVariable WalkSpeed;

        [Header("Dashing")]
        public GenericReference<float> DashRateStandard;
        public GenericReference<float> DashForce;
        public FloatVariable DashRate;
        public FloatVariable DashElaspedTime;
        public GameObject DashParticule;
        public float dashTime;

        Animator characterAnimator;
        SpriteRenderer weaponSprite;
        SpriteRenderer sprite;

        //Vector2 moveDir;
        Rigidbody2D rigidBody;
        //bool mustDash;

        void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();

            //mustDash = false;

            WalkSpeed.SetValue(WalkSpeedStandard);
            DashRate.SetValue(DashRateStandard);

            sprite = GetComponentInChildren<SpriteRenderer>();

            foreach (Transform t in transform)
            {
                if (t.name == "Body")
                {
                    characterAnimator = t.GetComponent<Animator>();
                }
            }
        }

        public void OnGameInitialized()
        {
            MoveAllowed.SetValue(true);
        }

        public void OnGameEnd()
        {
            MoveAllowed.SetValue(false);
        }

        void Update()
        {
            if (!MoveAllowed.Value)
            {
                return;
            }

            DashElaspedTime.ApplyChange(Time.deltaTime);

            if (DashElaspedTime.Value < dashTime)
            {
                return;
            }

            Direction.SetValue(Vector2.zero);

            if (Inputs.Actions.Gameplay.Dash.ReadValue<float>() > 0.0f && DashElaspedTime.Value > DashRate.Value)
            {
                //mustDash = true;
                DashElaspedTime.SetValue(0);
            }

            Direction.SetValue(Inputs.Actions.Gameplay.Move.ReadValue<Vector2>());
        }

        void FixedUpdate()
        {
            if (DashElaspedTime.Value < dashTime)
            {
                //animator.SetTrigger("Dash");
                //mustDash = false;
                //rigidBody.position += moveDir;
                rigidBody.velocity = Direction.Value * DashForce;

                ParticleSystemRenderer particuleRenderer = Instantiate(DashParticule, transform.position, transform.rotation).GetComponent<ParticleSystemRenderer>();
                if (sprite.flipX)
                {
                    particuleRenderer.flip = new Vector3(1, 0, 0);
                }
            }
            else
            {
                characterAnimator.SetBool("IsRunning", Direction.Value != Vector2.zero);
                rigidBody.velocity = Direction.Value * WalkSpeed.Value;
            }

            if (Direction.Value.x != 0.0f)
            {
                sprite.flipX = Direction.Value.x < 0;

                //if (weaponSprite == null)
                //{
                //    weaponSprite = GetComponent<CharacterFire>().weaponAnimator.GetComponent<SpriteRenderer>();
                //}
                //weaponSprite.flipX = moveDir.x < 0;
            }

            Position.SetValue(transform.position);
        }
    }
}



//Vector2 networkPosition;
//Vector2 networkVelocity;
//float currentSpeed;
//double lastNetworkDataReceivedTime;
//public void   (PhotonStream stream, PhotonMessageInfo info)
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