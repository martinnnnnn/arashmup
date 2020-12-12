using UnityEngine;
using UnityEngine.InputSystem;


// FSM for dashing / walking

namespace Arashmup
{
    public class CharacterMovement : MonoBehaviour
    {
        public Vector3Variable Position;
        public BoolVariable MoveAllowed;

        public GameInputs Inputs;

        [Header("Walking")]
        public FloatReference WalkSpeedStandard;
        public FloatVariable WalkSpeed;

        [Header("Dashing")]
        public FloatReference DashRateStandard;
        public FloatReference DashForce;
        public FloatVariable DashRate;
        public FloatVariable DashElaspedTime;

        Vector2 moveDir;
        Rigidbody2D rigidBody;
        bool mustDash;

        void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();

            mustDash = false;

            WalkSpeed.SetValue(WalkSpeedStandard);
            DashRate.SetValue(DashRateStandard);
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
            Move();
        }

        void Move()
        {
            moveDir = Vector2.zero;

            if (!MoveAllowed.Value)
            {
                return;
            }

            DashElaspedTime.ApplyChange(Time.deltaTime);

            if (Inputs.Actions.Gameplay.Dash.ReadValue<float>() > 0.0f && DashElaspedTime.Value > DashRate.Value)
            {
                mustDash = true;
                DashElaspedTime.SetValue(0);
            }

            moveDir = Inputs.Actions.Gameplay.Move.ReadValue<Vector2>();
        }

        void FixedUpdate()
        {
            if (mustDash)
            {
                mustDash = false;
                rigidBody.velocity = moveDir * DashForce;
            }
            else
            {
                rigidBody.velocity = moveDir * WalkSpeed.Value;
            }

            Position.SetValue(transform.position);
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