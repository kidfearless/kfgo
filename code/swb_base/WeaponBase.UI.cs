using Sandbox;
using Sandbox.UI;

/* 
 * Weapon base UI
*/

namespace SWB_Base
{

	public partial class WeaponBase
	{
		private Panel healthDisplay;
		private Panel ammoDisplay;

		private Panel hitmarker;

		public override void CreateHudElements()
		{
			if ( Local.Hud == null )
			{
				return;
			}

			if ( this.UISettings.ShowCrosshair )
			{
				this.CrosshairPanel = new Crosshair
				{
					Parent = Local.Hud
				};
			}

			if ( this.UISettings.ShowHitmarker )
			{
				this.hitmarker = new Hitmarker
				{
					Parent = Local.Hud
				};
			}

			if ( this.UISettings.ShowHealthCount || this.UISettings.ShowHealthIcon )
			{
				this.healthDisplay = new HealthDisplay( this.UISettings )
				{
					Parent = Local.Hud
				};
			}

			if ( this.UISettings.ShowAmmoCount || this.UISettings.ShowWeaponIcon || this.UISettings.ShowFireMode )
			{
				this.ammoDisplay = new AmmoDisplay( this.UISettings )
				{
					Parent = Local.Hud
				};
			}
		}

		public override void DestroyHudElements()
		{
			base.DestroyHudElements();

			if ( this.healthDisplay != null )
			{
				this.healthDisplay.Delete( true );
			}

			if ( this.ammoDisplay != null )
			{
				this.ammoDisplay.Delete( true );
			}

			if ( this.hitmarker != null )
			{
				this.hitmarker.Delete( true );
			}
		}
	}
}
