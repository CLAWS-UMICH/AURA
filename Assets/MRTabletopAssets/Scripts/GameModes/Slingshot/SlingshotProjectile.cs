using System;
using System.Collections;
using UnityEngine;
using XRMultiplayer;

namespace MRTTT
{
    /// <summary>
    /// Represents a projectile for the slingshot mini-game.
    /// </summary>
    public class SlingshotProjectile : MonoBehaviour
    {
        /// <summary>
        /// Event that is triggered when the local player hits a target with the projectile.
        /// </summary>
        public Action<SlingshotProjectile> localPlayerHitTarget;

        /// <summary>
        /// The collider for the projectile.
        /// </summary>
        [SerializeField] Collider m_Collider;

        /// <summary>
        /// The rigidbody for the projectile.
        /// </summary>
        [SerializeField] Rigidbody m_Rigidbody;

        [SerializeField]
        float m_GravityModifier = 1.0f;

        /// <summary>
        /// The trail renderer for the projectile.
        /// </summary>
        [SerializeField] protected TrailRenderer m_TrailRenderer;

        [SerializeField] protected float m_Lifetime = 10.0f;

        [SerializeField] string m_TargetTag = "Target";

        /// <summary>
        /// The previous position of the projectile.
        /// </summary>
        Vector3 m_PrevPos = Vector3.zero;

        /// <summary>
        /// The raycast hit for the projectile.
        /// </summary>
        RaycastHit m_Hit;

        public bool isLocalPlayerProjectile => m_LocalPlayerProjectile;
        /// <summary>
        /// Indicates whether the projectile belongs to the local player.
        /// </summary>
        bool m_LocalPlayerProjectile;

        Rigidbody m_Rigidybody;

        bool m_Initialized = false;

        void FixedUpdate()
        {
            if (!m_Initialized)
                return;

            m_Rigidbody.useGravity = false;
            m_Rigidbody.AddForce(Physics.gravity * m_GravityModifier, ForceMode.Acceleration);

            // if (!m_LocalPlayerProjectile) return;
            if (Physics.Linecast(m_PrevPos, transform.position, out m_Hit))
            {
                if (m_Hit.transform.CompareTag(m_TargetTag))
                {
                    HitTarget(m_Hit.transform.GetComponentInParent<ITarget>());
                }
            }

            m_PrevPos = transform.position;
        }

        /// <summary>
        /// Sets up the projectile with the specified parameters.
        /// </summary>
        /// <param name="localPlayer">Indicates whether the projectile belongs to the local player.</param>
        /// <param name="playerColor">The color of the player.</param>
        public void Setup(bool localPlayer, Color playerColor)
        {
            m_Initialized = false;
            if (m_Rigidybody == null)
            {
                TryGetComponent(out m_Rigidybody);
            }

            m_LocalPlayerProjectile = localPlayer;
            m_TrailRenderer.startColor = playerColor;
            m_TrailRenderer.endColor = playerColor;
            m_TrailRenderer.Clear();
            m_TrailRenderer.enabled = false;
            m_PrevPos = transform.position;
        }

        IEnumerator ResetProjectileAfterTime()
        {
            yield return new WaitForSeconds(m_Lifetime);
            DestroyProjectile();
        }

        public void DestroyProjectile()
        {
            StopAllCoroutines();
            StartCoroutine(DestroyRoutine());
        }

        IEnumerator DestroyRoutine()
        {
            m_Initialized = false;
            m_Rigidbody.isKinematic = true;
            yield return new WaitForSeconds(m_TrailRenderer.time);
            Destroy(gameObject);
        }

        /// <summary>
        /// Launches the projectile with the specified parameters.
        /// </summary>
        /// <param name="launchForce">The force to launch the projectile with.</param>
        /// <param name="isLocalPlayer">Indicates whether the player launching the projectile is the local player.</param>
        /// <param name="playerColor">The color of the player launching the projectile.</param>
        public void LaunchProjectile(Vector3 launchForce)
        {
            m_Initialized = true;
            m_Rigidbody.isKinematic = false;
            m_Collider.enabled = false;
            m_Rigidbody.linearVelocity = launchForce;
            m_TrailRenderer.enabled = true;
            StartCoroutine(LaunchRoutine());

        }

        /// <summary>
        /// Coroutine that enables the collider after a short delay.
        /// </summary>
        IEnumerator LaunchRoutine()
        {
            yield return new WaitForSeconds(.15f);
            m_Collider.enabled = true;

            StartCoroutine(ResetProjectileAfterTime());
        }

        /// <inheritdoc/>
        void OnTriggerEnter(Collider other)
        {
            // if (!m_LocalPlayerProjectile) return;
            if (other.CompareTag(m_TargetTag))
            {
                HitTarget(other.transform.GetComponentInParent<ITarget>());
            }
        }

        /// <summary>
        /// Called when the projectile hits a target.
        /// </summary>
        /// <param name="target">The target that was hit.</param>
        protected void HitTarget(ITarget target)
        {
            target.OnHit(m_TrailRenderer.startColor);
            // localPlayerHitTarget?.Invoke(this);
        }
    }
}
