using Sandbox;

using System;
using System.Collections.Generic;

/* 
 * High explosive grenade
*/

namespace SWB_Base
{
	public class Rocket : FiredEntity
	{
		public float ExplosionDelay { get; set; }
		public float ExplosionRadius { get; set; }
		public float ExplosionDamage { get; set; }
		public float ExplosionForce { get; set; }
		public List<string> ExplosionSounds { get; set; }
		public string ExplosionEffect { get; set; }
		public ScreenShake ExplosionShake { get; set; }
		public string RocketSound { get; set; }
		public List<string> RocketEffects { get; set; } = new List<string>();
		public string RocketSmokeEffect { get; set; }
		public int Inaccuracy { get; set; } = 0; // How Inaccurate is the rocket (higher = less accurate)

		private TimeSince _TimeSince;
		private List<Particles> _RocketParticles = new List<Particles>();
		private Sound _RocketLoopSound;

		public override void Start()
		{
			base.Start();
			this._TimeSince = 0;

			if ( !string.IsNullOrEmpty( this.RocketSound ) )
			{
				this._RocketLoopSound = this.PlaySound( this.RocketSound );
			}

			for ( int i = 0; i < this.RocketEffects.Count; i++ )
			{
				this._RocketParticles.Add( Particles.Create( this.RocketEffects[i], this, null, true ) );
			}
		}

		protected override void OnPhysicsCollision( CollisionEventData eventData )
		{
			if ( eventData.Entity == this.Owner )
			{
				return;
			}

			this.Explode();
		}

		public override void Tick()
		{
			// Rocket flight
			Vector3 downForce = this.Rotation.Down * 4;
			Random random = new Random();
			int timeSinceMod = (int)Math.Max( 0, this.Inaccuracy * this._TimeSince );
			Vector3 sideForce = this.Rotation.Left * ((random.Next( 0, timeSinceMod ) * 2) - timeSinceMod);

			this.Velocity += downForce + sideForce;

			// Update sound
			this._RocketLoopSound.SetPosition( this.Position );
		}

		public virtual void Explode()
		{
			// Stop loop sound
			this._RocketLoopSound.Stop();

			// Stop rocket particles
			for ( int i = 0; i < this._RocketParticles.Count; i++ )
			{
				this._RocketParticles[i].Destroy( false );
			}

			// Explosion sound
			string explosionSound = TableUtil.GetRandom( this.ExplosionSounds );

			if ( !string.IsNullOrEmpty( explosionSound ) )
			{
				this.PlaySound( explosionSound );
			}

			// Explosion effect
			if ( !string.IsNullOrEmpty( this.ExplosionEffect ) )
			{
				Particles.Create( this.ExplosionEffect, this.PhysicsBody.MassCenter );
			}

			// Screenshake
			ScreenUtil.ShakeAt( this.PhysicsBody.MassCenter, this.ExplosionRadius * 2, this.ExplosionShake );

			// Damage
			BlastUtil.Explode( this.PhysicsBody.MassCenter, this.ExplosionRadius, this.ExplosionDamage, this.ExplosionForce, this.Owner, this.Weapon, this );

			// Remove entity
			this.RenderColor = Color.Transparent;
			this.DeleteAsync( 0.01f );
		}
	}
}
