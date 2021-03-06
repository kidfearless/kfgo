using KFGO;

using Sandbox;

using System;
using System.Linq;
using System.Text;

/* 
 * Weapon base for weapons using magazine based reloading 
*/

namespace SWB_Base
{

	public partial class Weapon : WeaponBase
	{
		public Weapon()
		{

		}
		public Weapon( string weaponname ) : base( weaponname ) { }

		public static Weapon CreateWeapon(string name)
		{
			Weapon weapon = new Weapon( name );
			return weapon;
		}
	}

	public partial class WeaponBase
	{
		public void OnWorldModelPathChanged( string oldvalue, string newvalue )
		{

		}

		public void OnViewModelChanged(string oldvalue, string newvalue)
		{
			ViewModel = newvalue;
			ActiveEnd( Local.Pawn, false );
			ActiveStart( Local.Pawn );
		}
	}

	public partial class WeaponBase : CarriableBase
	{
		// this is forcefully called no matter what on the client!!!
		public WeaponBase() { }


		public WeaponBase( string name ) : base()
		{
			var data = WeaponData.GetByName( name );

			this.Primary = new();
			if ( data.Primary.Ammo != null ) this.Primary.Ammo = data.Primary.Ammo.Value;
			if ( data.Primary.AmmoType != null ) this.Primary.AmmoType = data.Primary.AmmoType.Value;
			if ( data.Primary.ClipSize != null ) this.Primary.ClipSize = data.Primary.ClipSize.Value;
			if ( data.Primary.ReloadTime != null ) this.Primary.ReloadTime = data.Primary.ReloadTime.Value;
			if ( data.Primary.ReloadEmptyTime != null ) this.Primary.ReloadEmptyTime = data.Primary.ReloadEmptyTime.Value;
			if ( data.Primary.Bullets != null ) this.Primary.Bullets = data.Primary.Bullets.Value;
			if ( data.Primary.BulletSize != null ) this.Primary.BulletSize = data.Primary.BulletSize.Value;
			if ( data.Primary.Damage != null ) this.Primary.Damage = data.Primary.Damage.Value;
			if ( data.Primary.Force != null ) this.Primary.Force = data.Primary.Force.Value;
			if ( data.Primary.Spread != null ) this.Primary.Spread = data.Primary.Spread.Value;
			if ( data.Primary.Recoil != null ) this.Primary.Recoil = data.Primary.Recoil.Value;
			if ( data.Primary.RPM != null ) this.Primary.RPM = data.Primary.RPM.Value;
			if ( data.Primary.FiringType != null ) this.Primary.FiringType = data.Primary.FiringType.Value;
			if ( data.Primary.ScreenShake != null ) this.Primary.ScreenShake = data.Primary.ScreenShake.Value;
			if ( data.Primary.ShootAnim != null ) this.Primary.ShootAnim = data.Primary.ShootAnim;
			if ( data.Primary.ReloadAnim != null ) this.Primary.ReloadAnim = data.Primary.ReloadAnim;
			if ( data.Primary.ReloadEmptyAnim != null ) this.Primary.ReloadEmptyAnim = data.Primary.ReloadEmptyAnim;
			if ( data.Primary.DrawAnim != null ) this.Primary.DrawAnim = data.Primary.DrawAnim;
			if ( data.Primary.DrawEmptyAnim != null ) this.Primary.DrawEmptyAnim = data.Primary.DrawEmptyAnim;
			if ( data.Primary.DryFireSound != null ) this.Primary.DryFireSound = data.Primary.DryFireSound;
			if ( data.Primary.ShootSound != null ) this.Primary.ShootSound = data.Primary.ShootSound;
			if ( data.Primary.BulletEjectParticle != null ) this.Primary.BulletEjectParticle = data.Primary.BulletEjectParticle;
			if ( data.Primary.MuzzleFlashParticle != null ) this.Primary.MuzzleFlashParticle = data.Primary.MuzzleFlashParticle;
			if ( data.Primary.BarrelSmokeParticle != null ) this.Primary.BarrelSmokeParticle = data.Primary.BarrelSmokeParticle;
			if ( data.Primary.BulletTracerParticle != null ) this.Primary.BulletTracerParticle = data.Primary.BulletTracerParticle;
			if ( data.Primary.InfiniteAmmo != null ) this.Primary.InfiniteAmmo = data.Primary.InfiniteAmmo.Value;

			if ( data.Secondary != null )
			{
				//this.Secondary = new( data.Secondary );
			}

			// gross
			if ( data.Attributes.Bucket != null ) this.Bucket = data.Attributes.Bucket.Value;
			if ( data.Attributes.BucketWeight != null ) this.BucketWeight = data.Attributes.BucketWeight.Value;
			if ( data.Attributes.CanDrop != null ) this.CanDrop = data.Attributes.CanDrop.Value;
			if ( data.Attributes.DropWeaponOnDeath != null ) this.DropWeaponOnDeath = data.Attributes.DropWeaponOnDeath.Value;
			if ( data.Attributes.BulletCocking != null ) this.BulletCocking = data.Attributes.BulletCocking.Value;
			if ( data.Attributes.BarrelSmoking != null ) this.BarrelSmoking = data.Attributes.BarrelSmoking.Value;
			if ( data.Attributes.FreezeViewModelOnZoom != null ) this.FreezeViewModelOnZoom = data.Attributes.FreezeViewModelOnZoom;
			if ( data.Attributes.FOV != null ) this.FOV = data.Attributes.FOV.Value;
			if ( data.Attributes.ZoomFOV != null ) this.ZoomFOV = data.Attributes.ZoomFOV.Value;
			if ( data.Attributes.TuckRange != null ) this.TuckRange = data.Attributes.TuckRange.Value;
			if ( data.Attributes.HoldType != null ) this.HoldType = data.Attributes.HoldType.Value;
			if ( data.Attributes.ViewModel != null ) this.ViewModel = data.Attributes.ViewModel;
			if ( data.Attributes.WorldModelPath != null ) this.WorldModelPath = data.Attributes.WorldModelPath;
			if ( data.Attributes.Icon != null ) this.Icon = data.Attributes.Icon;
			if ( data.Attributes.WalkAnimationSpeedMod != null ) this.WalkAnimationSpeedMod = data.Attributes.WalkAnimationSpeedMod.Value;
			if ( data.Attributes.AimSensitivity != null ) this.AimSensitivity = data.Attributes.AimSensitivity.Value;
			if ( data.Attributes.DualWield != null ) this.DualWield = data.Attributes.DualWield.Value;
			if ( data.Attributes.PrimaryDelay != null ) this.PrimaryDelay = data.Attributes.PrimaryDelay.Value;
			if ( data.Attributes.SecondaryDelay != null ) this.SecondaryDelay = data.Attributes.SecondaryDelay.Value;
			if ( data.Attributes.ZoomAnimData != null ) this.ZoomAnimData = data.Attributes.ZoomAnimData;
			if ( data.Attributes.RunAnimData != null ) this.RunAnimData = data.Attributes.RunAnimData;

		}


		public override void Spawn()
		{
			base.Spawn();

			this.CollisionGroup = CollisionGroup.Weapon;
			this.SetInteractsAs( CollisionLayer.Debris );

			this.SetModel( this.WorldModelPath );

			this.PickupTrigger = new PickupTrigger();
			this.PickupTrigger.Parent = this;
			this.PickupTrigger.Position = this.Position;
		}

		public override void ActiveStart( Entity ent )
		{
			base.ActiveStart( ent );

			this.TimeSinceDeployed = 0;
			this.IsReloading = false;

			// Draw animation
			if ( this.IsLocalPawn )
			{
				BaseViewModel activeViewModel = !this.DualWield ? this.ViewModelEntity : this.DualWieldViewModel;

				if ( this.Primary.Ammo == 0 && !string.IsNullOrEmpty( this.Primary.DrawEmptyAnim ) )
				{
					activeViewModel?.SetAnimBool( this.Primary.DrawEmptyAnim, true );
				}
				else if ( !string.IsNullOrEmpty( this.Primary.DrawAnim ) )
				{
					activeViewModel?.SetAnimBool( this.Primary.DrawAnim, true );
				}
			}

			// Animated activity status will reset when weapon is switched out
			if ( this.AnimatedActions != null )
			{
				for ( int i = 0; i < this.AnimatedActions.Count; i++ )
				{
					if ( this.AnimatedActions[i].IsToggled )
					{
						this.AnimatedActions[i].HandleOnDeploy( this );
					}
				}
			}

			// Dualwield setup
			if ( this.DualWield )
			{
				if ( !this.IsDualWieldConverted )
				{
					this.IsDualWieldConverted = true;
					this.Primary.Ammo *= 2;
					this.Primary.ClipSize *= 2;
					this.Primary.RPM = (int)(this.Primary.RPM * 2.0);
					this.ZoomAnimData = null;
					this.RunAnimData = null;
				}
			}
		}

		public override void ActiveEnd( Entity ent, bool dropped )
		{
			base.ActiveEnd( ent, dropped );

			if ( this.DualWield && this.DualWieldViewModel != null )
			{
				this.DualWieldViewModel.Delete();
			}
		}

		// BaseSimulate
		public void BaseSimulate( Client player )
		{
			if ( Input.Down( InputButton.Reload ) )
			{
				this.Reload();
			}

			// Reload could have deleted us
			if ( !this.IsValid() )
			{
				return;
			}

			if ( this.CanPrimaryAttack() )
			{
				this.TimeSincePrimaryAttack = 0;
				this.AttackPrimary();
			}

			// AttackPrimary could have deleted us
			if ( !player.IsValid() )
			{
				return;
			}

			if ( this.CanSecondaryAttack() )
			{
				this.TimeSinceSecondaryAttack = 0;
				this.AttackSecondary();
			}
		}

		public override void Simulate( Client owner )
		{

			if ( this.IsAnimating )
			{
				return;
			}

			// Handle custom animation actions
			if ( this.AnimatedActions != null && !this.IsReloading )
			{
				for ( int i = 0; i < this.AnimatedActions.Count; i++ )
				{
					if ( this.AnimatedActions[i].Handle( owner, this ) )
					{
						return;
					}
				}
			}

			this.IsRunning = Input.Down( InputButton.Run ) && this.RunAnimData != null && this.Owner.Velocity.Length >= 200;

			if ( this.Secondary == null && this.ZoomAnimData != null && !(this is WeaponBaseMelee) )
			{
				this.IsZooming = Input.Down( InputButton.Attack2 ) && !this.IsRunning && !this.IsReloading;
			}

			if ( this.TimeSinceDeployed < 0.6f )
			{
				return;
			}

			if ( !this.IsReloading || this is WeaponBaseShotty )
			{
				this.BaseSimulate( owner );
			}

			if ( this.IsReloading && this.TimeSinceReload >= 0 )
			{
				this.OnReloadFinish();
			}
		}

		public virtual void Reload()
		{
			if ( this.IsReloading || this.IsAnimating )
			{
				return;
			}

			int maxClipSize = this.BulletCocking ? this.Primary.ClipSize + 1 : this.Primary.ClipSize;

			if ( this.Primary.Ammo >= maxClipSize || this.Primary.ClipSize == -1 )
			{
				return;
			}

			// TODO: Make these properties
			bool isEmptyReload = this.Primary.ReloadEmptyTime > 0 && this.Primary.Ammo == 0;
			this.TimeSinceReload = -(isEmptyReload ? this.Primary.ReloadEmptyTime : this.Primary.ReloadTime);

			if ( this.Owner is PlayerBase player )
			{
				if ( player.AmmoCount( this.Primary.AmmoType ) <= 0 && this.Primary.InfiniteAmmo != InfiniteAmmoType.Reserve )
				{
					return;
				}
			}

			this.IsReloading = true;

			// Player anim
			(this.Owner as AnimEntity).SetAnimBool( "b_reload", true );

			this.StartReloadEffects( isEmptyReload );
		}

		public virtual void OnReloadFinish()
		{
			this.IsReloading = false;

			// Dual wield
			if ( this.DualWield && !this.DualWieldShouldReload )
			{
				this.DualWieldShouldReload = true;
				this.Reload();
				return;
			}

			this.DualWieldShouldReload = false;

			if ( this.Primary.InfiniteAmmo == InfiniteAmmoType.Reserve )
			{
				int newAmmo = this.Primary.ClipSize;

				if ( this.BulletCocking && this.Primary.Ammo > 0 )
				{
					newAmmo += 1;
				}

				this.Primary.Ammo = newAmmo;
				return;
			}

			if ( this.Owner is PlayerBase player )
			{
				int ammo = player.TakeAmmo( this.Primary.AmmoType, this.Primary.ClipSize - this.Primary.Ammo );
				if ( ammo == 0 )
				{
					return;
				}

				this.Primary.Ammo += ammo;
			}
		}

		[ClientRpc]
		public virtual void StartReloadEffects( bool isEmpty, string reloadAnim = null )
		{
			BaseViewModel reloadingViewModel = this.DualWield && this.DualWieldShouldReload ? this.DualWieldViewModel : this.ViewModelEntity;

			if ( reloadAnim != null )
			{
				reloadingViewModel?.SetAnimBool( reloadAnim, true );
			}
			else if ( isEmpty && this.Primary.ReloadEmptyAnim != null )
			{
				reloadingViewModel?.SetAnimBool( this.Primary.ReloadEmptyAnim, true );
			}
			else if ( this.Primary.ReloadAnim != null )
			{
				reloadingViewModel?.SetAnimBool( this.Primary.ReloadAnim, true );
			}

			// TODO - player third person model reload
		}

		public override void BuildInput( InputBuilder input )
		{
			// Mouse sensitivity
			if ( this.IsZooming )
			{
				input.ViewAngles = MathUtil.FILerp( input.OriginalViewAngles, input.ViewAngles, this.AimSensitivity * 90 );
			}

			// Recoil
			if ( this.DoRecoil )
			{
				this.DoRecoil = false;
				Angles recoilAngles = new Angles( this.IsZooming ? -this.Primary.Recoil * 0.4f : -this.Primary.Recoil, 0, 0 );
				input.ViewAngles += recoilAngles;
			}
		}

		[ClientRpc]
		public void CreateView()
		{
			this.CreateViewModel();
		}
		public override void CreateViewModel()
		{
			Host.AssertClient();

			if ( string.IsNullOrEmpty( this.ViewModelPath ) )
			{
				return;
			}

			this.ViewModelEntity?.Delete();

			this.ViewModelEntity = new ViewModelBase( this )
			{
				Position = Position, // --> Does not seem to do anything
				Owner = Owner,
				EnableViewmodelRendering = true
			};
			this.ViewModelEntity.SetModel( this.ViewModelPath );

			if ( this.DualWield )
			{
				this.DualWieldViewModel = new ViewModelBase( this, true )
				{
					Owner = Owner,
					EnableViewmodelRendering = true
				};
				this.DualWieldViewModel.SetModel( this.ViewModelPath );
			}
		}

		public virtual ModelEntity GetEffectModel()
		{
			BaseViewModel animatingViewModel = this.DualWield && this.DualWieldLeftFire ? this.DualWieldViewModel : this.ViewModelEntity;
			ModelEntity effectModel = animatingViewModel;

			// We don't want to change the world effect origin if we or others can see it
			if ( (this.IsLocalPawn && !this.Owner.IsFirstPersonMode) || !this.IsLocalPawn )
			{
				effectModel = this.EffectEntity;
			}

			return effectModel;
		}

		public virtual float GetRealSpread( float baseSpread = -1 )
		{
			if ( !this.Owner.IsValid() )
			{
				return 0;
			}

			float spread = baseSpread != -1 ? baseSpread : this.Primary.Spread;
			float floatMod = 1f;

			// Ducking
			if ( Input.Down( InputButton.Duck ) && !this.IsZooming )
			{
				floatMod -= 0.25f;
			}

			// Aiming
			if ( this.IsZooming && this is not WeaponBaseShotty )
			{
				floatMod /= 4;
			}

			// Jumping
			if ( this.Owner.GroundEntity == null )
			{
				floatMod += 0.5f;
			}

			return spread * floatMod;
		}

		public bool TakeAmmo( int amount )
		{
			if ( this.Primary.InfiniteAmmo == InfiniteAmmoType.Clip )
			{
				return true;
			}

			if ( this.Primary.ClipSize == -1 )
			{
				if ( this.Owner is PlayerBase player )
				{
					return player.TakeAmmo( this.Primary.AmmoType, amount ) > 0;
				}

				return true;
			}

			if ( this.Primary.Ammo < amount )
			{
				return false;
			}

			this.Primary.Ammo -= amount;
			return true;
		}

		public int AvailableAmmo()
		{
			if ( this.Owner is not PlayerBase owner )
			{
				return 0;
			}

			// Show clipsize as the available ammo
			if ( this.Primary.InfiniteAmmo == InfiniteAmmoType.Reserve )
			{
				return this.Primary.ClipSize;
			}

			return owner.AmmoCount( this.Primary.AmmoType );
		}

		public virtual bool IsUsable()
		{
			if ( this.Primary.Ammo > 0 )
			{
				return true;
			}

			return this.AvailableAmmo() > 0;
		}

		public override void OnCarryStart( Entity carrier )
		{
			base.OnCarryStart( carrier );

			if ( this.PickupTrigger.IsValid() )
			{
				this.PickupTrigger.EnableTouch = false;
			}
		}

		public override void OnCarryDrop( Entity dropper )
		{
			if ( !this.DropWeaponOnDeath )
			{
				this.Delete();
				return;
			}

			base.OnCarryDrop( dropper );

			if ( this.PickupTrigger.IsValid() )
			{
				this.PickupTrigger.EnableTouch = true;
			}
		}

		public override void SimulateAnimator( PawnAnimator anim )
		{
			anim.SetParam( "holdtype", (int)this.HoldType );
			anim.SetParam( "aimat_weight", 1.0f );
		}

		[ClientRpc]
		public virtual void SendWeaponAnim( string anim, bool value = true )
		{
			this.ViewModelEntity?.SetAnimBool( anim, value );
		}
	}
}
