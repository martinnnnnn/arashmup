using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arashmup
{
    public class CharacterAnimation : MonoBehaviour
    {
        [Serializable]
        public struct VisualInfo
        {
            public RuntimeAnimatorController animController;
            public GameObject dashParticulePrefab;
        }

        public List<VisualInfo> VisualsInfo;

        SpriteRenderer sprite;
        Animator animator;
        GameObject dashParticule;

        void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        public void SetAnim(string animatorName)
        {
            if (sprite == null)
            { 
                sprite = GetComponent<SpriteRenderer>();
            }

            if (animator == null)
            {
                animator = GetComponent<Animator>(); 
            }

            foreach (VisualInfo visualInfo in VisualsInfo)
            {
                if (visualInfo.animController.name == animatorName)
                {
                    animator.runtimeAnimatorController = visualInfo.animController;
                    dashParticule = visualInfo.dashParticulePrefab;
                    break;
                }
            }
        }

        internal void Animate(Vector2 direction, bool isDashing)
        {

            if (isDashing)
            {
                ParticleSystemRenderer particuleRenderer = Instantiate(dashParticule, transform.position, transform.rotation).GetComponent<ParticleSystemRenderer>();
                particuleRenderer.flip = direction.x < 0 ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
            }
            else
            {
                animator.SetBool("IsRunning", direction != Vector2.zero);
            }

            if (direction.x != 0.0f)
            {
                sprite.flipX = direction.x < 0;
            }
        }
    }
}
