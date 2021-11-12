using Sandbox;

using System;
using System.Threading.Tasks;

/* 
 * Weapon base for weapons firing entities
*/

namespace SWB_Base
{
	public partial class WeaponBaseEntity : WeaponBase
	{
		public virtual Func<ClipInfo, bool, FiredEntity> CreateEntity => null; // Function that creates an entity and returns it ( to use custom entities in the base )
		public virtual string EntityModel => ""; // Path to the model of the entity
		public virtual Vector3 EntityVelocity => new Vector3( 0, 0, 100 ); // Velocity ( right, up, forward )
		public virtual Angles EntityAngles => new Angles( 0, 0, 0 ); // Spawn angles
		public virtual Vector3 EntitySpawnOffset => Vector3.Zero; // Spawn offset ( right, up, forward )
		public virtual float PrimaryEntitySpeed => 100f; // Primary velocity speed
		public virtual float SecondaryEntitySpeed => 50f; // Secondary velocity Speed
		public virtual float RemoveDelay => -1; // Delay that the entity should be removed after
		public virtual bool UseGravity => true; // Should gravity affect the entity
		public virtual bool IsSticky => false; // Should the entity stick to the surface it hits

		public virtual void FireEntity( ClipInfo clipInfo, bool isPrimary )
		{
			FiredEntity firedEntity;

			if ( this.CreateEntity == null )
			{
				firedEntity = new FiredEntity();
			}
			else
			{
				firedEntity = this.CreateEntity( clipInfo, isPrimary );
			}

			if ( !string.IsNullOrEmpty( this.EntityModel ) )
			{
				firedEntity.SetModel( this.EntityModel );
			}

			firedEntity.Owner = this.Owner;
			firedEntity.Position = MathUtil.RelativeAdd( this.Position, this.EntitySpawnOffset, this.Owner.EyeRot );
			firedEntity.Rotation = this.Owner.EyeRot * Rotation.From( this.EntityAngles );
			firedEntity.RemoveDelay = this.RemoveDelay;
			firedEntity.UseGravity = this.UseGravity;
			firedEntity.Speed = isPrimary ? this.PrimaryEntitySpeed : this.SecondaryEntitySpeed;
			firedEntity.IsSticky = this.IsSticky;
			firedEntity.Damage = clipInfo.Damage;
			firedEntity.Force = clipInfo.Force;
			firedEntity.StartVelocity = MathUtil.RelativeAdd( Vector3.Zero, this.EntityVelocity, this.Owner.EyeRot );
			firedEntity.Start();
		}

		public override void Attack( ClipInfo clipInfo, bool isPrimary )
		{
			if ( (this.IsRunning && this.RunAnimData != null) || this.ShouldTuck() )
			{
				return;
			}

			this.TimeSincePrimaryAttack = 0;
			this.TimeSinceSecondaryAttack = 0;

			if ( !this.TakeAmmo( 1 ) )
			{
				this.DryFire( clipInfo.DryFireSound );
				return;
			}

			// Player anim
			(this.Owner as AnimEntity).SetAnimBool( "b_attack", true );

			// Weapon anim
			ScreenUtil.Shake( To.Single( this.Owner ), clipInfo );
			this.ShootEffects( clipInfo.MuzzleFlashParticle, clipInfo.BulletEjectParticle, clipInfo.ShootAnim );

			if ( !string.IsNullOrEmpty( clipInfo.ShootSound ) )
			{
				this.PlaySound( clipInfo.ShootSound );
			}

			if ( Host.IsServer )
			{
				using ( Prediction.Off() )
				{
					this.FireEntity( clipInfo, isPrimary );
				}
			}
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
			ScreenUtil.Shake( To.Single( owner ), clipInfo );
			this.ShootEffects( clipInfo.MuzzleFlashParticle, clipInfo.BulletEjectParticle, null );

			if ( clipInfo.ShootSound != null )
			{
				this.PlaySound( clipInfo.ShootSound );
			}

			if ( this.IsServer )
			{
				using ( Prediction.Off() )
				{
					this.FireEntity( clipInfo, isPrimary );
				}
			}
		}

		public override void DelayedAttack( ClipInfo clipInfo, bool isPrimary, float delay )
		{
			_ = this.AsyncAttack( this.Primary, isPrimary, this.PrimaryDelay );
		}
	}
}
