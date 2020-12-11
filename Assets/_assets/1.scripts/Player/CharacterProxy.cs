using Photon.Pun;
using UnityEngine;


namespace Arashmup
{
    public class CharacterProxy : MonoBehaviour
    {
        public BoolVariable IsDead;
        public SpriteRenderer Visual;

        PhotonView PV;
        WeaponController weaponController;
        Collider2D collider2d;

        void Start()
        {
            PV = GetComponent<PhotonView>();
            weaponController = GetComponent<WeaponController>();
            collider2d = GetComponent<Collider2D>();
        }


        public void Fire(int actorNumber, Vector3 position, Vector2 direction)
        {
            PV.RPC(RPC_Functions.Fire, RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, position, direction);
        }

        [PunRPC]
        void RPC_Fire(int actorNumber, Vector3 position, Vector2 direction)
        {
            Collider2D[] toIgnore = null;
            if (collider2d != null)
            {
                toIgnore = new Collider2D[]
                {
                    collider2d,
                };
            }

            weaponController.Fire(actorNumber, position, direction, toIgnore);
        }

        public void Kill()
        {
            PV.RPC(RPC_Functions.Kill, RpcTarget.All);
        }

        [PunRPC]
        void RPC_Kill()
        {
            IsDead.SetValue(true);
            Visual.color = new Color(Visual.color.r, Visual.color.g, Visual.color.b, 0.1f);
        }
    }
}