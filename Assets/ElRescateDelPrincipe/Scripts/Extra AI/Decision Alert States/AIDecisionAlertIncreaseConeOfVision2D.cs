
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This Decision will return true if its MMConeOfVision has detected at least one target, and will set it as the Brain's target
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionAlertIncreaseConeOfVision2D")]
    [RequireComponent(typeof(MMConeOfVision2D))]
    public class AIDecisionAlertIncreaseConeOfVision2D : AIDecision
    {
        protected MMConeOfVision2D _coneOfVision;
        private AlertStateController _alertStateController;

        /// <summary>
        /// On Init we grab our MMConeOfVision
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();
            _coneOfVision = this.gameObject.GetComponent<MMConeOfVision2D>();
            _alertStateController = this.gameObject.GetComponent<AlertStateController>();
        }

        /// <summary>
        /// On Decide we look for a target
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return DetectTarget();
        }

        /// <summary>
        /// If the MMConeOfVision has at least one target, it becomes our new brain target and our Alert Status will rise until true, otherwise it's false.
        /// </summary>
        /// <returns></returns>
        protected virtual bool DetectTarget()
        {
            Debug.Log("On Increase:"+ _brain.Target.name);
            if (_coneOfVision.VisibleTargets.Count == 0)
            {
                return false;
            }
            else
            {
                _alertStateController.IncreaseStatus();
                _brain.Target = _coneOfVision.VisibleTargets[0];
                if (_alertStateController.IsReady()){
                    _alertStateController.DecreaseStatus();
                    return true;
                }
                else return false;
            }
        }
    }
}
