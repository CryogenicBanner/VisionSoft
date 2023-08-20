using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine // you might want to use your own namespace here
{
    public class PlayerShield : CharacterAbility
    {
        /// This method is only used to display a helpbox text
        /// at the beginning of the ability's inspector
        public override string HelpBoxText() { return "This enables shield"; }
        // Start is called before the first frame update

        [Header("Public Parameters")]
        /// declare your parameters here
        public float randomParameter = 4f;
        public bool randomBool;

        [Header("Shield Parameters")]
        public int maxSlots = 3;
        private int currentSlots;

        public float delayForRecharge = 1;
        private float currentDelayForRecharge;

        public float rechargeTime = 1;
        private float currentRechargeTime;

        protected const string _yourAbilityAnimationParameterName = "YourAnimationParameterName";
        protected int _yourAbilityAnimationParameter;

        private float shieldInput = Input.GetAxis("Shield");

        /// <summary>
        /// Here you should initialize our parameters
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            randomBool = false;
            currentSlots = maxSlots;
            currentDelayForRecharge = 0;
            currentRechargeTime = 0;
        }

        /// <summary>
        /// Every frame, we check if we're crouched and if we still should be
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
        }

        /// <summary>
        /// Called at the start of the ability's cycle, this is where you'll check for input
        /// </summary>
        protected override void HandleInput()
        {
            base.HandleInput();
            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard
            /*
            if (_inputManager.PrimaryMovement.y < -_inputManager.Threshold.y)
            {
                DoSomething();
            }
            */
            if(shieldInput == 1|| Input.GetMouseButtonDown(1))
            {
                DoSomething();
            }
        }

        /// <summary>
        /// If we're pressing down, we check for a few conditions to see if we can perform our action
        /// </summary>
        protected virtual void DoSomething()
        {
            
            // if the ability is not permitted
            if (!AbilityPermitted
                // or if we're not in our normal stance
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                // or if we're grounded
                // || (!_controller.Grounded)
                // or if we're out of shield slots to use
                || (currentSlots == 0)
                )
            {
                // we do nothing and exit
                return;
            }

            // if we're still here, we display a text log in the console
            MMDebug.DebugLogTime("We're using shield yay");


        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_yourAbilityAnimationParameterName, AnimatorControllerParameterType.Bool, out _yourAbilityAnimationParameter);
        }

        /// <summary>
        /// At the end of the ability's cycle,
        /// we send our current crouching and crawling states to the animator
        /// </summary>
        public override void UpdateAnimator()
        {

            bool myCondition = true;
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _yourAbilityAnimationParameter, myCondition, _character._animatorParameters);
        }
    }
}