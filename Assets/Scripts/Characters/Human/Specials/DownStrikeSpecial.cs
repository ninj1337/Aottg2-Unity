using Settings;
using System.Collections;
using UnityEngine;

namespace Characters
{
    class DownStrikeSpecial : BaseHoldAttackSpecial
    {
        protected bool _needActivate;

        public DownStrikeSpecial(BaseCharacter owner): base(owner)
        {
            Cooldown = 5f;
        }

        protected override void Activate()
        {
            _needActivate = true;
            _human.HookLeft.DisableAnyHook();
            _human.HookRight.DisableAnyHook();
            _human.CancelHookLeftKey = true;
            _human.CancelHookRightKey = true;
            _human.CancelHookBothKey = true;
            _human.StartSpecialAttack(HumanAnimations.SpecialMikasa1);
        }

        protected override void ActiveFixedUpdate()
        {
            base.ActiveFixedUpdate();
            bool firstFrame = _needActivate;
            if (_needActivate)
            {
                // Impulse force allows people to use this to quickly stop their momentum
                Vector3 impulseForce = Vector3.up * 10f;
                _human._instantForceAcc += impulseForce / _human.Cache.Rigidbody.mass;
                _human.Cache.Rigidbody.AddForce(impulseForce, ForceMode.Impulse);
                _needActivate = false;
                _human.Cache.Rigidbody.velocity = Vector3.zero;
                _human.ActivateBlades();
                if (SettingsManager.SoundSettings.OldBladeEffect.Value)
                    _human.PlaySound(HumanSounds.OldBladeSwing);
                else
                    _human.PlaySound(HumanSounds.BladeSwing4);
            }

            Vector3 velocityChange = Vector3.down * 3f;
            _human._instantForceAcc += velocityChange;
            _human.Cache.Rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            if ((!firstFrame && _human.Grounded) || _human.HookLeft.IsActive || _human.HookRight.IsActive || _human.State == HumanState.Grab)
            {
                IsActive = false;
                Deactivate();
            }
        }

        public override bool CanUse()
        {
            return base.CanUse() && _human.CanBladeAttack();
        }
    }
}
