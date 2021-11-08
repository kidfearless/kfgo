using Sandbox;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWB_Base
{
	public class AnimatedAction
	{
		public List<InputButton> ActionButtons { get; set; }
		public string OnAnimation { get; set; }
		public float OnAnimationDuration { get; set; } = 2f;
		public string OffAnimation { get; set; }
		public float OffAnimationDuration { get; set; } = 2f;
		public string AnimationStatus { get; set; }

		public string NewViewModel { get; set; }
		public string NewWorldModel { get; set; }
		public string NewShootSound { get; set; }

		private ClipInfo _ClipInfo = new ClipInfo();
		private float _CanNextHandle = 0f;
		public bool IsToggled { get; set; } = false;

		private void HandleChanges( WeaponBase weaponBase )
		{
			if ( !string.IsNullOrEmpty( this.NewShootSound ) )
			{
				if ( this.IsToggled )
				{
					this._ClipInfo.ShootSound = weaponBase.Primary.ShootSound;
				}

				weaponBase.Primary.ShootSound = this.IsToggled ? this.NewShootSound : this._ClipInfo.ShootSound;
			}

			if ( !string.IsNullOrEmpty( this.NewViewModel ) )
			{
				weaponBase?.ViewModelEntity?.SetModel( this.IsToggled ? this.NewViewModel : weaponBase.ViewModelPath );
			}

			if ( !string.IsNullOrEmpty( this.NewWorldModel ) )
			{
				weaponBase.SetModel( this.IsToggled ? this.NewWorldModel : weaponBase.WorldModelPath );
			}
		}

		public void HandleOnDeploy( WeaponBase weaponBase )
		{
			weaponBase.SendWeaponAnim( this.AnimationStatus );
		}

		async private Task ResetAnimating( WeaponBase weaponBase, float delay )
		{
			await GameTask.DelaySeconds( delay );
			weaponBase.IsAnimating = false;
		}

		public bool Handle( Client owner, WeaponBase weaponBase )
		{
			if ( RealTime.Now < this._CanNextHandle )
			{
				return false;
			}

			// Check if animated keys are down
			for ( int i = 0; i < this.ActionButtons.Count; i++ )
			{
				if ( !Input.Down( this.ActionButtons[i] ) )
				{
					return false;
				}

				// Reload will fuck with animations, IsReload is still false here
				if ( this.ActionButtons[i] == InputButton.Reload && weaponBase.Primary.Ammo < weaponBase.Primary.ClipSize )
				{
					return false;
				}
			}

			this.IsToggled = !this.IsToggled;

			float canNextAnimateDelay = this.IsToggled ? this.OnAnimationDuration : this.OffAnimationDuration;
			this._CanNextHandle = RealTime.Now + canNextAnimateDelay;

			// Reset animating after the delay
			weaponBase.IsAnimating = true;
			_ = this.ResetAnimating( weaponBase, canNextAnimateDelay );

			// Handle shared changes
			this.HandleChanges( weaponBase );

			if ( weaponBase.IsClient )
			{
				BaseViewModel viewModelEntity = weaponBase.ViewModelEntity;

				if ( this.IsToggled )
				{
					viewModelEntity?.SetAnimBool( this.OnAnimation, true );
				}
				else
				{
					viewModelEntity?.SetAnimBool( this.OffAnimation, true );
				}

				viewModelEntity?.SetAnimBool( this.AnimationStatus, this.IsToggled );
			}

			return true;
		}
	}
}
