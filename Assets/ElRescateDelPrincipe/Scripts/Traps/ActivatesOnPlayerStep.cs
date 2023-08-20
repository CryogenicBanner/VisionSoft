using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using System;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Add this component to trigger an animation's object in it. 
    /// </summary>
    public class ActivatesOnPlayerStep : MonoBehaviour
    {
        /// the possible ways to add knockback : noKnockback, which won't do nothing, set force, or add force
        public enum KnockbackStyles { NoKnockback, AddForce }
        /// the possible knockback directions
        public enum KnockbackDirections { BasedOnOwnerPosition, BasedOnSpeed, BasedOnDirection, BasedOnScriptDirection }

        [Header("Targets")]

        [MMInformation("This component will make your object cause damage to objects that collide with it. Here you can define what layers will be affected by the damage (for a standard enemy, choose Player), how much damage to give, and how much force should be applied to the object that gets the damage on hit. You can also specify how long the post-hit invincibility should last (in seconds).", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
        // the layers that will be damaged by this object
        [Tooltip("the layers that will be damaged by this object")]
        public LayerMask TargetLayerMask;

        /// the owner of the DamageOnTouch zone
        [MMReadOnly]
        [Tooltip("the owner of the DamageOnTouch zone")]
        public GameObject Owner;

        [Header("Animation Triggers")]
        [Tooltip("Activates an animation upon stanting on it")]
        public Animator _animatorToTrigger;
        public string _animatorParameterVar;
        

        // storage		
        protected Vector3 _lastPosition, _lastDamagePosition, _velocity, _knockbackForce, _damageDirection;
        protected float _startTime = 0f;
        protected Health _colliderHealth;
        protected TopDownController _topDownController;
        protected TopDownController _colliderTopDownController;
        protected Rigidbody _colliderRigidBody;
        protected Health _health;
        protected List<GameObject> _ignoredGameObjects;
        protected Vector3 _knockbackForceApplied;
        protected CircleCollider2D _circleCollider2D;
        protected BoxCollider2D _boxCollider2D;
        protected SphereCollider _sphereCollider;
        protected BoxCollider _boxCollider;
        protected Color _gizmosColor;
        protected Vector3 _gizmoSize;
        protected Vector3 _gizmoOffset;
        protected Transform _gizmoTransform;
        protected bool _twoD = false;
        protected bool _initializedFeedbacks = false;
        protected Vector3 _positionLastFrame;
        protected Vector3 _scriptDirection;

        /// <summary>
        /// Initialization
        /// </summary>
        protected virtual void Awake()
        {
            InitializeIgnoreList();
            
            _health = GetComponent<Health>();
            _topDownController = GetComponent<TopDownController>();
            _boxCollider = GetComponent<BoxCollider>();
            _sphereCollider = GetComponent<SphereCollider>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _circleCollider2D = GetComponent<CircleCollider2D>();
            _lastDamagePosition = this.transform.position;

            _twoD = ((_boxCollider2D != null) || (_circleCollider2D != null));

            _gizmosColor = Color.red;
            _gizmosColor.a = 0.25f;
            if (_boxCollider2D != null) { SetGizmoOffset(_boxCollider2D.offset); _boxCollider2D.isTrigger = true; }
            if (_boxCollider != null) { SetGizmoOffset(_boxCollider.center); _boxCollider.isTrigger = true; }
            if (_sphereCollider != null) { SetGizmoOffset(_sphereCollider.center); _sphereCollider.isTrigger = true; }
            if (_circleCollider2D != null) { SetGizmoOffset(_circleCollider2D.offset); _circleCollider2D.isTrigger = true; }
        }


        public virtual void SetGizmoSize(Vector3 newGizmoSize)
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _boxCollider = GetComponent<BoxCollider>();
            _sphereCollider = GetComponent<SphereCollider>();
            _circleCollider2D = GetComponent<CircleCollider2D>();
            _gizmoSize = newGizmoSize;
        }

        public virtual void SetGizmoOffset(Vector3 newOffset)
        {
            _gizmoOffset = newOffset;
        }

        public virtual void SetScriptDirection(Vector3 newDirection)
        {
            _scriptDirection = newDirection;
        }

        /// <summary>
        /// OnEnable we set the start time to the current timestamp
        /// </summary>
        protected virtual void OnEnable()
        {
            _startTime = Time.time;
            _lastPosition = this.transform.position;
            _lastDamagePosition = this.transform.position;
        }

        /// <summary>
        /// During last update, we store the position and velocity of the object
        /// </summary>
        protected virtual void Update()
        {
            ComputeVelocity();
        }

        protected void LateUpdate()
        {
            _positionLastFrame = this.transform.position;
        }

        /// <summary>
        /// Initializes the _ignoredGameObjects list if needed
        /// </summary>
        protected virtual void InitializeIgnoreList()
        {
            if (_ignoredGameObjects == null)
            {
                _ignoredGameObjects = new List<GameObject>();
            }
        }

        /// <summary>
        /// Adds the gameobject set in parameters to the ignore list
        /// </summary>
        /// <param name="newIgnoredGameObject">New ignored game object.</param>
        public virtual void IgnoreGameObject(GameObject newIgnoredGameObject)
        {
            InitializeIgnoreList();
            _ignoredGameObjects.Add(newIgnoredGameObject);
        }

        /// <summary>
        /// Removes the object set in parameters from the ignore list
        /// </summary>
        /// <param name="ignoredGameObject">Ignored game object.</param>
        public virtual void StopIgnoringObject(GameObject ignoredGameObject)
        {
            if (_ignoredGameObjects != null)
            {
                _ignoredGameObjects.Remove(ignoredGameObject);    
            }
        }

        /// <summary>
        /// Clears the ignore list.
        /// </summary>
        public virtual void ClearIgnoreList()
        {
            InitializeIgnoreList();
            _ignoredGameObjects.Clear();
        }

        /// <summary>
        /// Computes the velocity based on the object's last position
        /// </summary>
        protected virtual void ComputeVelocity()
        {
            if (Time.deltaTime != 0f)
            {
                _velocity = (_lastPosition - (Vector3)transform.position) / Time.deltaTime;

                if (Vector3.Distance(_lastDamagePosition, this.transform.position) > 0.5f)
                {
                    _damageDirection = this.transform.position - _lastDamagePosition;
                    _lastDamagePosition = this.transform.position;
                }                

                _lastPosition = this.transform.position;
            }            
        }

        /// <summary>
        /// When a collision with the player is triggered, we give damage to the player and knock it back
        /// </summary>
        /// <param name="collider">what's colliding with the object.</param>
        public virtual void OnTriggerStay2D(Collider2D collider)
        {
            
        }

        /// <summary>
        /// On trigger enter 2D, we call our colliding endpoint
        /// </summary>
        /// <param name="collider"></param>S
        public virtual void OnTriggerEnter2D(Collider2D collider)
        {
            Colliding(collider.gameObject);
        }

        /// <summary>
        /// On trigger stay, we call our colliding endpoint
        /// </summary>
        /// <param name="collider"></param>
        public virtual void OnTriggerStay(Collider collider)
        {
            Colliding(collider.gameObject);
        }

        /// <summary>
        /// On trigger enter, we call our colliding endpoint
        /// </summary>
        /// <param name="collider"></param>
        public virtual void OnTriggerEnter(Collider collider)
        {
            Colliding(collider.gameObject);
        }

        /// <summary>
        /// When colliding, we apply damage
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void Colliding(GameObject collider)
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            // if the object we're colliding with is part of our ignore list, we do nothing and exit
            if (_ignoredGameObjects.Contains(collider))
            {
                return;
            }

            // if what we're colliding with isn't part of the target layers, we do nothing and exit
            if (!MMLayers.LayerInLayerMask(collider.layer, TargetLayerMask))
            {

                return;
            }

            // if we're on our first frame, we don't apply damage
            if (Time.time == 0f)
            {
                return;
            }
            _colliderHealth = collider.gameObject.MMGetComponentNoAlloc<Health>();

            // if what we're colliding with is damageable
            if (_colliderHealth != null)
            {
                if (_colliderHealth.CurrentHealth > 0)
                {
                    OnCollideWithDamageable(_colliderHealth);
                }
            }

            // if what we're colliding with can't be damaged
            else {}
        }

        /// <summary>
        /// Describes what happens when colliding with a damageable object
        /// </summary>
        /// <param name="health">Health.</param>
        protected virtual void OnCollideWithDamageable(Health health)
        {
            // if what we're colliding with is a TopDownController, we apply a knockback force
            _colliderTopDownController = health.gameObject.MMGetComponentNoAlloc<TopDownController>();
            _colliderRigidBody = health.gameObject.MMGetComponentNoAlloc<Rigidbody>();

            if ((_colliderTopDownController != null) && (!_colliderHealth.Invulnerable) && (!_colliderHealth.ImmuneToKnockback))
            {
                //Debug.Log("Disque voy a llamar la animación jaja :(");
                StartCoroutine(TriggerAnim(_animatorParameterVar));
            }
        }

        private IEnumerator TriggerAnim(string s)
        {
            _animatorToTrigger.SetBool(s, true);
            yield return new WaitForSeconds(1);
            _animatorToTrigger.SetBool(s, false);
        }

    }
}
