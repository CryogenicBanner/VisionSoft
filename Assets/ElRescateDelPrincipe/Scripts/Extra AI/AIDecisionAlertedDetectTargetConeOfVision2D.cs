using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This Decision will return true if its MMConeOfVision has detected at least one target, and will set it as the Brain's target
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionAlertedDetectTargetConeOfVision2D")]
    [RequireComponent(typeof(MMConeOfVision2D))]
    public class AIDecisionAlertedDetectTargetConeOfVision2D : AIDecision
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
            if(_alertStateController == null)
            {
                Debug.Log("Enemy lacks Alert State Controller");
            }
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
        /// If the MMConeOfVision has at least one target, it becomes our new brain target and this decision is true, otherwise it's false.
        /// </summary>
        /// <returns></returns>
        protected virtual bool DetectTarget()
        {
            if (_coneOfVision.VisibleTargets.Count == 0)
            {
                _brain.Target = null;
                _alertStateController.DecreaseStatus();
            }
            else
            {
                _brain.Target = _coneOfVision.VisibleTargets[0];
                _alertStateController.IncreaseStatus();
            }

            return _alertStateController.IsReady();
        }
    }
}
