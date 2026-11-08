using System.Collections;
using UnityEngine;

namespace Tanks.Complete
{
    public class PowerUpDetector : MonoBehaviour
    {
        // Variable that indicates if the tank has a PowerUp right now
        public bool m_HasActivePowerUp = false;
        // References to the tank's components
        private TankShooting m_TankShooting;
        private TankMovement m_TankMovement;
        private TankHealth m_TankHealth;
        private PowerUpHUD m_PowerUpHUD;

        // What each timed power up has currently applied to the tank. A coroutine running on a GameObject is killed
        // when that object is deactivated (tank death, round reset) *before* its "revert" code runs, so the amounts
        // are stored here and reverted in OnDisable if the coroutine never got the chance to do it.
        private bool m_SpeedBoostActive;
        private float m_AppliedSpeedBoost;
        private float m_AppliedTurnSpeedBoost;
        private bool m_ShootingBoostActive;
        private float m_AppliedCooldownReduction;
        private bool m_ShieldActive;
        private float m_AppliedShieldAmount;
        private bool m_InvincibilityActive;

        private void Awake()
        {
            // Get references to the tank's movement, shooting, and health components
            m_TankShooting = GetComponent<TankShooting>();
            m_TankMovement = GetComponent<TankMovement>();
            m_TankHealth = GetComponent<TankHealth>();
            m_PowerUpHUD = GetComponentInChildren<PowerUpHUD>();
        }

        private void OnDisable()
        {
            // The tank is deactivated on death and on every round reset: drop every active power up so the tank
            // never keeps modified stats or a stale HUD icon into the next round.
            ResetPowerUps();
        }

        /// <summary>
        /// Immediately ends every active power up, reverting any stat it changed. Safe to call multiple times.
        /// </summary>
        public void ResetPowerUps()
        {
            StopAllCoroutines();

            RevertSpeed();
            RevertShootingRate();
            RevertShield();
            RevertInvincibility();

            // The special shell flag lives in TankShooting, which clears it in its own OnEnable.
            m_HasActivePowerUp = false;

            if (m_PowerUpHUD != null)
                m_PowerUpHUD.ResetHUD();
        }

        // Applies a temporary speed boost to the tank
        public void PowerUpSpeed(float speedBoost, float turnSpeedBoost, float duration)
        {
            StartCoroutine(IncreaseSpeed(speedBoost, turnSpeedBoost, duration));
        }

        // Coroutine to temporarily increase the tank's movement speed and turn speed
        private IEnumerator IncreaseSpeed(float speedBoost, float TurnSpeedBoost, float duration)
        {
            // Apply the speed boost
            m_HasActivePowerUp = true;
            m_PowerUpHUD.SetActivePowerUp(PowerUp.PowerUpType.Speed, duration);
            m_TankMovement.m_Speed += speedBoost;
            m_TankMovement.m_TurnSpeed += TurnSpeedBoost;
            m_AppliedSpeedBoost = speedBoost;
            m_AppliedTurnSpeedBoost = TurnSpeedBoost;
            m_SpeedBoostActive = true;
            // Wait for the duration of the power up
            yield return new WaitForSeconds(duration);
            // Revert the speed boost
            RevertSpeed();
            m_HasActivePowerUp = false;
        }

        private void RevertSpeed()
        {
            if (!m_SpeedBoostActive)
                return;
            m_SpeedBoostActive = false;

            if (m_TankMovement != null)
            {
                m_TankMovement.m_Speed -= m_AppliedSpeedBoost;
                m_TankMovement.m_TurnSpeed -= m_AppliedTurnSpeedBoost;
            }
            m_AppliedSpeedBoost = 0f;
            m_AppliedTurnSpeedBoost = 0f;
        }

        // Applies a temporary shooting rate boost to the tank
        public void PowerUpShoootingRate(float cooldownReduction, float duration)
        {
            StartCoroutine(IncreaseShootingRate(cooldownReduction, duration));
        }

        // Coroutine to temporarily enhance the tank's shooting rate
        private IEnumerator IncreaseShootingRate(float cooldownReduction, float duration)
        {
            // Apply the shooting cooldown reduction if it is greater than zero
            if(cooldownReduction > 0)
            {
                m_HasActivePowerUp = true;
                m_PowerUpHUD.SetActivePowerUp(PowerUp.PowerUpType.ShootingBonus, duration);
                m_TankShooting.m_ShotCooldown *= cooldownReduction;
                m_AppliedCooldownReduction = cooldownReduction;
                m_ShootingBoostActive = true;
                // Wait for the duration of the power up
                yield return new WaitForSeconds(duration);
                // Revert the shooting boost after the duration ends
                RevertShootingRate();
                m_HasActivePowerUp = false;
            }
        }

        private void RevertShootingRate()
        {
            if (!m_ShootingBoostActive)
                return;
            m_ShootingBoostActive = false;

            if (m_TankShooting != null && m_AppliedCooldownReduction > 0)
                m_TankShooting.m_ShotCooldown /= m_AppliedCooldownReduction;
            m_AppliedCooldownReduction = 0f;
        }

        // Grants the tank a temporary shield if it does not already have one
        public void PickUpShield(float shieldAmount, float duration)
        {
            if (!m_TankHealth.m_HasShield)
                StartCoroutine(ActivateShield(shieldAmount, duration));
        }

        // Grants the tank a temporary shield if it does not already have one
        private IEnumerator ActivateShield(float shieldAmount, float duration)
        {
            // Activate the shield
            m_HasActivePowerUp = true;
            m_PowerUpHUD.SetActivePowerUp(PowerUp.PowerUpType.DamageReduction, duration);
            m_TankHealth.ToggleShield(shieldAmount);
            m_AppliedShieldAmount = shieldAmount;
            m_ShieldActive = true;
            // Wait for the duration of the power up
            yield return new WaitForSeconds(duration);
            // Deactivate the shield
            RevertShield();
            m_HasActivePowerUp = false;
        }

        private void RevertShield()
        {
            if (!m_ShieldActive)
                return;
            m_ShieldActive = false;

            // ToggleShield flips the flag, so only toggle if the shield is still on (TankHealth.OnEnable also clears it)
            if (m_TankHealth != null && m_TankHealth.m_HasShield)
                m_TankHealth.ToggleShield(m_AppliedShieldAmount);
        }

        // Increases the health of the tank
        public void PowerUpHealing(float healAmount)
        {
            m_TankHealth.IncreaseHealth(healAmount);
            m_PowerUpHUD.SetActivePowerUp(PowerUp.PowerUpType.Healing, 1.0f);
        }

        // Makes the tank invulnerable for an amount of time
        public void PowerUpInvincibility(float duration)
        {
            StartCoroutine(ActivateInvincibility(duration));
        }

        private IEnumerator ActivateInvincibility(float duration)
        {
            m_HasActivePowerUp = true;
            m_PowerUpHUD.SetActivePowerUp(PowerUp.PowerUpType.Invincibility, duration);
            m_TankHealth.ToggleInvincibility();
            m_InvincibilityActive = true;
            yield return new WaitForSeconds(duration);
            m_HasActivePowerUp = false;
            RevertInvincibility();
        }

        private void RevertInvincibility()
        {
            if (!m_InvincibilityActive)
                return;
            m_InvincibilityActive = false;

            // ToggleInvincibility flips the flag; this runs before TankHealth.OnEnable clears it on the next activation
            if (m_TankHealth != null)
                m_TankHealth.ToggleInvincibility();
        }

        // Equips the tank with a special shell that increases damage
        public void PowerUpSpecialShell(float damageMultiplier)
        {
            m_HasActivePowerUp = true;
            m_PowerUpHUD.SetActivePowerUp(PowerUp.PowerUpType.DamageMultiplier, 0f);
            m_TankShooting.EquipSpecialShell(damageMultiplier);
        }
    }
}
