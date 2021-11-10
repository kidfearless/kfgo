/* 
 * Weapon base for weapons using shell based reloading 
*/

namespace SWB_Base
{
	public partial class WeaponBaseShotty : WeaponBase
	{
		public virtual float ShellReloadTimeStart => -1; // Duration of the reload start animation
		public virtual float ShellReloadTimeInsert => -1; // Duration of the reload insert animation
		public virtual string ReloadFinishAnim => "reload_finished"; // Finishing reload animation
		public virtual bool CanShootDuringReload => true; // Can the shotgun shoot while reloading

		// TODO: Fix ME
		//public override bool BulletCocking => false;

		private void CancelReload()
		{
			this.IsReloading = false;
		}

		public override void AttackPrimary()
		{
			if ( this.IsReloading && !this.CanShootDuringReload )
			{
				return;
			}

			this.CancelReload();
			base.AttackPrimary();
		}

		public override void AttackSecondary()
		{
			if ( this.IsReloading && !this.CanShootDuringReload )
			{
				return;
			}

			this.CancelReload();
			base.AttackSecondary();
		}

		public override void Reload()
		{
			this.Primary.ReloadTime = this.ShellReloadTimeStart;
			base.Reload();
		}

		public override void OnReloadFinish()
		{
			this.IsReloading = false;

			if ( !this.CanShootDuringReload )
			{
				this.TimeSincePrimaryAttack = 0;
				this.TimeSinceSecondaryAttack = 0;
			}

			if ( this.Primary.Ammo >= this.Primary.ClipSize )
			{
				return;
			}

			if ( this.Owner is PlayerBase player )
			{
				bool hasInfiniteReserve = this.Primary.InfiniteAmmo == InfiniteAmmoType.Reserve;
				int ammo = hasInfiniteReserve ? 1 : player.TakeAmmo( this.Primary.AmmoType, 1 );

				if ( ammo != 0 )
				{
					this.Primary.Ammo += 1;
				}

				if ( ammo != 0 && this.Primary.Ammo < this.Primary.ClipSize )
				{
					this.Primary.ReloadTime = this.ShellReloadTimeInsert;
					base.Reload();
				}
				else
				{
					this.StartReloadEffects( false, this.ReloadFinishAnim );
				}
			}
		}
	}
}
