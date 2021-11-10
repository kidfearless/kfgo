using KFGO.UI;

using Sandbox;
using Sandbox.UI;

/* 
 * Weapon base UI
*/

namespace SWB_Base
{

	public partial class WeaponBase
	{
		protected Panel AmmoDisplay { get; set; }
		protected Panel Hitmarker { get; set; }

		public override void CreateHudElements()
		{
			if ( Local.Hud == null )
			{
				return;
			}

			if ( this.UISettings.ShowCrosshair )
			{
				this.CrosshairPanel = new Crosshair()
				{
					Parent = Local.Hud
				};
			}

			if ( this.UISettings.ShowHitmarker )
			{
				this.Hitmarker = new Hitmarker()
				{
					Parent = Local.Hud
				};
			}

			if ( this.UISettings.ShowAmmoCount || this.UISettings.ShowWeaponIcon || this.UISettings.ShowFireMode )
			{
				this.AmmoDisplay = new AmmoDisplay( this.UISettings )
				{
					Parent = Local.Hud
				};
			}
		}

		public override void DestroyHudElements()
		{
			base.DestroyHudElements();

			this.AmmoDisplay?.Delete( true );
			this.Hitmarker?.Delete( true );
		}
	}
}
