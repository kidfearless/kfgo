using Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;

/* 
 * High explosive grenade
*/

namespace SWB_Base
{
    public class Grenade : FiredEntity
    {
        public float ExplosionDelay { get; set; }
        public float ExplosionRadius { get; set; }
        public float ExplosionDamage { get; set; }
        public float ExplosionForce { get; set; }
        public string BounceSound { get; set; }
        public List<string> ExplosionSounds { get; set; }
        public string ExplosionEffect { get; set; }
        public ScreenShake ExplosionShake { get; set; }

        public override void Start()
        {
            base.Start();
            _ = this.DelayedExplode();
        }

        protected override void OnPhysicsCollision(CollisionEventData eventData)
        {
            if (eventData.Entity is not Player && eventData.Speed > 50 && !string.IsNullOrEmpty( this.BounceSound ))
            {
				this.PlaySound( this.BounceSound );
            }
        }

        async Task DelayedExplode()
        {
            await GameTask.DelaySeconds( this.ExplosionDelay );
			this.Explode();
        }

        public virtual void Explode()
        {
			// Explosion sound
			string explosionSound = TableUtil.GetRandom( this.ExplosionSounds );

            if (!string.IsNullOrEmpty(explosionSound))
            {
				this.PlaySound(explosionSound);
            }

            // Effects
            Particles.Create( this.ExplosionEffect, this.PhysicsBody.MassCenter);

            // Screenshake
            ScreenUtil.ShakeAt( this.PhysicsBody.MassCenter, this.ExplosionRadius * 1.5f, this.ExplosionShake );

            // Damage
            BlastUtil.Explode( this.PhysicsBody.MassCenter, this.ExplosionRadius, this.ExplosionDamage, this.ExplosionForce, this.Owner, this.Weapon, this);

			// Remove entity
			this.Delete();
        }
    }
}
