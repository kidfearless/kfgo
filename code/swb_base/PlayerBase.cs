using Sandbox;

using System.Linq;

namespace SWB_Base
{
	public partial class PlayerBase : Player
	{
		TimeSince _TimeSinceDropped;
		Rotation _LastCameraRot = Rotation.Identity;
		DamageInfo _LastDamage;

		public bool SupressPickupNotices { get; set; }
		public virtual string PlayerModel { get; set; } = "models/citizen/citizen.vmdl";
		public virtual float DefaultHealth { get; set; } = 100;

		public PlayerBase()
		{
			this.Inventory = new InventoryBase( this );
		}

		public PlayerBase( IBaseInventory inventory )
		{
			this.Inventory = inventory;
		}

		public override void Respawn()
		{
			base.Respawn();

			this.SetDefaultModel();
			this.SetDefaultController();
			this.SetDefaultAnimator();
			this.SetDefaultCamera();


			this.EnableAllCollisions = true;
			this.EnableDrawing = true;
			this.EnableHideInFirstPerson = true;
			this.EnableShadowInFirstPerson = true;

			this.Health = this.DefaultHealth;

			this.ClearAmmo();
		}

		public virtual void SetDefaultModel() => this.SetModel( this.PlayerModel );
		public virtual void SetDefaultController() => this.Controller = new WalkController();
		public virtual void SetDefaultAnimator() => this.Animator = new StandardPlayerAnimator();
		public virtual void SetDefaultCamera() => this.Camera = new FirstPersonCamera();

		public override void OnKilled()
		{
			base.OnKilled();

			this.Inventory.DropActive();
			this.Inventory.DeleteContents();

			this.BecomeRagdollOnClient( this._LastDamage.Force, this.GetHitboxBone( this._LastDamage.HitboxIndex ) );

			this.Controller = null;
			this.Camera = new SpectateRagdollCamera();

			this.EnableAllCollisions = false;
			this.EnableDrawing = false;
		}

		public override void Simulate( Client cl )
		{
			//if ( cl.NetworkIdent == 1 )
			//  return;

			base.Simulate( cl );



			// Input requested a weapon switch
			if ( Input.ActiveChild != null )
			{
				this.ActiveChild = Input.ActiveChild;
			}

			if ( this.LifeState != LifeState.Alive )
			{
				return;
			}

			this.TickPlayerUse();

			if ( Input.Pressed( InputButton.View ) )
			{
				if ( this.Camera is ThirdPersonCamera )
				{
					this.Camera = new FirstPersonCamera();
				}
				else
				{
					this.Camera = new ThirdPersonCamera();
				}
			}

			if ( Input.Pressed( InputButton.Drop ) )
			{
				Entity dropped = this.Inventory.DropActive();
				if ( dropped != null )
				{
					if ( dropped.PhysicsGroup != null )
					{
						dropped.PhysicsGroup.Velocity = this.Velocity + ((this.EyeRot.Forward + this.EyeRot.Up) * 300);
					}

					this._TimeSinceDropped = 0;
					this.SwitchToBestWeapon();
				}
			}

			this.SimulateActiveChild( cl, this.ActiveChild );

			//
			// If the current weapon is out of ammo and we last fired it over half a second ago
			// lets try to switch to a better wepaon
			//
			if ( this.ActiveChild is WeaponBase weapon && !weapon.IsUsable() && weapon.TimeSincePrimaryAttack > 0.5f && weapon.TimeSinceSecondaryAttack > 0.5f )
			{
				//this.SwitchToBestWeapon();
			}
		}

		public void SwitchToBestWeapon()
		{
			WeaponBase best = this.Children.Select( x => x as WeaponBase )
				 .Where( x => x.IsValid() && x.IsUsable() )
				 .OrderByDescending( x => x.BucketWeight )
				 .FirstOrDefault();

			if ( best == null )
			{
				return;
			}

			this.ActiveChild = best;
		}

		public override void StartTouch( Entity other )
		{
			if ( this._TimeSinceDropped < 1 )
			{
				return;
			}

			base.StartTouch( other );
		}

		public override void PostCameraSetup( ref CameraSetup setup )
		{
			base.PostCameraSetup( ref setup );

			if ( this._LastCameraRot == Rotation.Identity )
			{
				this._LastCameraRot = setup.Rotation;
			}

			Rotation angleDiff = Rotation.Difference( this._LastCameraRot, setup.Rotation );
			float angleDiffDegrees = angleDiff.Angle();
			float allowance = 20.0f;

			if ( angleDiffDegrees > allowance )
			{
				// We could have a function that clamps a rotation to within x degrees of another rotation?
				this._LastCameraRot = Rotation.Lerp( this._LastCameraRot, setup.Rotation, 1.0f - (allowance / angleDiffDegrees) );
			}
			else
			{
				//lastCameraRot = Rotation.Lerp( lastCameraRot, Camera.Rotation, Time.Delta * 0.2f * angleDiffDegrees );
			}
		}

		public override void TakeDamage( DamageInfo info )
		{
			this._LastDamage = info;

			if ( this.GetHitboxGroup( info.HitboxIndex ) == 1 )
			{
				info.Damage *= 2.0f;
			}

			base.TakeDamage( info );

			if ( info.Attacker is PlayerBase attacker && attacker != this )
			{
				// Note - sending this only to the attacker!
				attacker.DidDamage( To.Single( attacker ), info.Position, info.Damage, this.Health, ((float)this.Health).LerpInverse( 100, 0 ) );

				// Hitmarker
				if ( info.Weapon is WeaponBase weapon && weapon.UISettings.ShowHitmarker )
				{
					attacker.ShowHitmarker( To.Single( attacker ), !this.Alive(), weapon.UISettings.PlayHitmarkerSound );
				}

				this.TookDamage( To.Single( this ), info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position );
			}
		}

		public virtual bool Alive()
		{
			return this.Health > 0;
		}

		[ClientRpc]
		public void DidDamage( Vector3 pos, float amount, float health, float healthinv )
		{
			Sound.FromScreen( "dm.ui_attacker" )
				 .SetPitch( 1 + (healthinv * 1) );

		}

		[ClientRpc]
		public void TookDamage( Vector3 pos )
		{
			//DebugOverlay.Sphere( pos, 5.0f, Color.Red, false, 50.0f );
			// TODO: REIMPLIMENT
			//DamageIndicator.Current?.OnHit( pos );
		}

		[ClientRpc]
		public void ShowHitmarker( bool isKill, bool playSound )
		{
			Hitmarker.Current?.Create( isKill );

			if ( playSound )
			{
				this.PlaySound( "swb_hitmarker" );
			}
		}
	}
}
