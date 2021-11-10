using Sandbox;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/* 
 * Weapon base for weapons using magazine based reloading 
*/

namespace SWB_Base
{
	public partial class WeaponBase
	{
		public virtual bool CanAttack( ClipInfo clipInfo, TimeSince lastAttackTime, InputButton inputButton )
		{
			if ( this.IsAnimating )
			{
				return false;
			}

			if ( clipInfo == null || !this.Owner.IsValid() || !Input.Down( inputButton ) )
			{
				return false;
			}

			if ( clipInfo.FiringType == FiringType.SemiAutomatic && !Input.Pressed( inputButton ) )
			{
				return false;
			}

			if ( clipInfo.RPM <= 0 )
			{
				return true;
			}

			return lastAttackTime > (60f / clipInfo.RPM);
		}

		public virtual bool CanPrimaryAttack()
		{
			return this.CanAttack( this.Primary, this.TimeSincePrimaryAttack, InputButton.Attack1 );
		}

		public virtual bool CanSecondaryAttack()
		{
			return this.CanAttack( this.Secondary, this.TimeSinceSecondaryAttack, InputButton.Attack2 );
		}

		public virtual void Attack( ClipInfo clipInfo, bool isPrimary )
		{
			if ( this.IsRunning || this.ShouldTuck() )
			{
				return;
			}

			this.TimeSincePrimaryAttack = 0;
			this.TimeSinceSecondaryAttack = 0;
			this.TimeSinceFired = 0;

			if ( !this.TakeAmmo( 1 ) )
			{
				this.DryFire( clipInfo.DryFireSound );
				return;
			}

			// Player anim
			(this.Owner as AnimEntity).SetAnimBool( "b_attack", true );

			// Tell the clients to play the shoot effects
			ScreenUtil.Shake( To.Single( this.Owner ), clipInfo.ScreenShake );
			this.ShootEffects( clipInfo.MuzzleFlashParticle, clipInfo.BulletEjectParticle, clipInfo.ShootAnim );

			// Barrel smoke
			if ( this.IsServer && this.BarrelSmoking )
			{
				this.AddBarrelHeat();
				if ( this.BarrelHeat >= clipInfo.ClipSize * 0.75 )
				{
					this.ShootEffects( clipInfo.BarrelSmokeParticle, null, null );
				}
			}

			if ( clipInfo.ShootSound != null )
			{
				this.PlaySound( clipInfo.ShootSound );
			}

			// Shoot the bullets
			float realSpread;

			if ( this is WeaponBaseShotty )
			{
				realSpread = clipInfo.Spread;
			}
			else
			{
				realSpread = this.IsZooming ? clipInfo.Spread / 4 : clipInfo.Spread;
			}

			if ( this.IsServer )
			{
				for ( int i = 0; i < clipInfo.Bullets; i++ )
				{
					this.ShootBullet( realSpread, clipInfo.Force, clipInfo.Damage, clipInfo.BulletSize );
				}
			}

			// Recoil
			this.DoRecoil = true;
		}

		async Task AsyncAttack( ClipInfo clipInfo, bool isPrimary, float delay )
		{
			if ( this.AvailableAmmo() <= 0 )
			{
				return;
			}

			this.TimeSincePrimaryAttack -= delay;
			this.TimeSinceSecondaryAttack -= delay;

			// Player anim
			(this.Owner as AnimEntity).SetAnimBool( "b_attack", true );

			// Play pre-fire animation
			this.ShootEffects( null, null, clipInfo.ShootAnim );

			if ( this.Owner is not PlayerBase owner )
			{
				return;
			}

			Entity activeWeapon = owner.ActiveChild;

			await GameTask.DelaySeconds( delay );

			// Check if owner and weapon are still valid
			if ( owner == null || activeWeapon != owner.ActiveChild )
			{
				return;
			}

			// Take ammo
			this.TakeAmmo( 1 );

			// Play shoot effects
			ScreenUtil.Shake( To.Single( this.Owner ), clipInfo.ScreenShake );
			this.ShootEffects( clipInfo.MuzzleFlashParticle, clipInfo.BulletEjectParticle, null );

			if ( clipInfo.ShootSound != null )
			{
				this.PlaySound( clipInfo.ShootSound );
			}

			// Shoot the bullets
			if ( this.IsServer )
			{
				for ( int i = 0; i < clipInfo.Bullets; i++ )
				{
					this.ShootBullet( this.GetRealSpread( clipInfo.Spread ), clipInfo.Force, clipInfo.Damage, clipInfo.BulletSize );
				}
			}
		}

		public virtual void DelayedAttack( ClipInfo clipInfo, bool isPrimary, float delay )
		{
			_ = this.AsyncAttack( this.Primary, isPrimary, this.PrimaryDelay );
		}

		public virtual void AttackPrimary()
		{
			if ( this.DualWield )
			{
				this.DualWieldLeftFire = !this.DualWieldLeftFire;
			}

			if ( this.PrimaryDelay > 0 )
			{
				this.DelayedAttack( this.Primary, true, this.PrimaryDelay );
			}
			else
			{
				this.Attack( this.Primary, true );
			}
		}

		public virtual void AttackSecondary()
		{
			if ( this.Secondary != null )
			{
				if ( this.SecondaryDelay > 0 )
				{
					this.DelayedAttack( this.Secondary, false, this.SecondaryDelay );
				}
				else
				{
					this.Attack( this.Secondary, false );
				}

				return;
			}
		}

		/// <summary>
		/// Does a trace from start to end, does bullet impact effects. Coded as an IEnumerable so you can return multiple
		/// hits, like if you're going through layers or ricocet'ing or something.
		/// </summary>
		public virtual IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2.0f )
		{
			bool InWater = Physics.TestPointContents( start, CollisionLayer.Water );

			TraceResult tr = Trace.Ray( start, end )
					.UseHitboxes()
					.HitLayer( CollisionLayer.Water, !InWater )
					.Ignore( this.Owner )
					.Ignore( this )
					.Size( radius )
					.Run();

			yield return tr;

			//
			// Another trace, bullet going through thin material, penetrating water surface?
			//
		}

		/// Shoot a single bullet (server)
		public virtual void ShootBullet( float spread, float force, float damage, float bulletSize )
		{
			// Spread
			Vector3 forward = this.Owner.EyeRot.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;
			Vector3 endPos = this.Owner.EyePos + (forward * 999999);

			// Client bullet
			this.ShootClientBullet( this.Owner.EyePos, endPos, bulletSize );

			// Server bullet
			foreach ( TraceResult tr in this.TraceBullet( this.Owner.EyePos, endPos, bulletSize ) )
			{

				if ( !tr.Entity.IsValid() )
				{
					continue;
				}

				// We turn prediction off for this, so any exploding effects don't get culled etc
				using ( Prediction.Off() )
				{
					DamageInfo damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100 * force, damage )
						.UsingTraceResult( tr )
						.WithAttacker( this.Owner )
						.WithWeapon( this );

					tr.Entity.TakeDamage( damageInfo );
				}
			}
		}

		[ClientRpc]
		public virtual void ShootClientBullet( Vector3 startPos, Vector3 endPos, float radius = 2.0f )
		{
			foreach ( TraceResult tr in this.TraceBullet( startPos, endPos, radius ) )
			{
				// Impact
				tr.Surface.DoBulletImpact( tr );

				// Tracer
				if ( !string.IsNullOrEmpty( this.Primary.BulletTracerParticle ) )
				{
					Random random = new Random();
					int randVal = random.Next( 0, 2 );

					if ( randVal == 0 )
					{
						this.TracerEffects( this.Primary.BulletTracerParticle, tr.EndPos );
					}
				}
			}
		}

		[ClientRpc]
		protected virtual void ShootEffects( string muzzleFlashParticle, string bulletEjectParticle, string shootAnim )
		{
			Host.AssertClient();

			BaseViewModel animatingViewModel = this.DualWield && this.DualWieldLeftFire ? this.DualWieldViewModel : this.ViewModelEntity;
			ModelEntity firingViewModel = animatingViewModel;

			// We don't want to change the world effect origin if we or others can see it
			if ( (this.IsLocalPawn && !this.Owner.IsFirstPersonMode) || !this.IsLocalPawn )
			{
				firingViewModel = this.EffectEntity;
			}

			if ( !string.IsNullOrEmpty( muzzleFlashParticle ) )
			{
				Particles.Create( muzzleFlashParticle, firingViewModel, "muzzle" );
			}

			if ( !string.IsNullOrEmpty( bulletEjectParticle ) )
			{
				Particles.Create( bulletEjectParticle, firingViewModel, "ejection_point" );
			}

			if ( !string.IsNullOrEmpty( shootAnim ) )
			{
				animatingViewModel?.SetAnimBool( shootAnim, true );
				this.CrosshairPanel?.CreateEvent( "fire", 60f / this.Primary.RPM );
			}
		}

		protected virtual void TracerEffects( string tracerParticle, Vector3 endPos )
		{
			ModelEntity firingViewModel = this.GetEffectModel();

			Transform? muzzleAttach = firingViewModel.GetAttachment( "muzzle" );
			Particles tracer = Particles.Create( tracerParticle );
			tracer.SetPosition( 1, muzzleAttach.GetValueOrDefault().Position );
			tracer.SetPosition( 2, endPos );
		}

		[ClientRpc]
		public virtual void DryFire( string dryFireSound )
		{
			if ( !string.IsNullOrEmpty( dryFireSound ) )
			{
				this.PlaySound( dryFireSound );
			}
		}
	}
}
