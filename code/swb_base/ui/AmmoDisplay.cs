using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using SWB_Base;

using System;

public class AmmoDisplay : Panel
{
	Panel _AmmoWrapper;
	Panel _IconWrapper;
	Panel _FireModeWrapper;

	Image _WeaponIcon;
	Image _SemiFireIcon;
	Image _AutoFireIcon;

	Label _ClipLabel;
	Label _ReserveLabel;

	Color _ReserveColor = new Color32( 200, 200, 200 ).ToColor();
	Color _EmptyColor = new Color32( 175, 175, 175, 200 ).ToColor();

	public AmmoDisplay( UISettings uiSettings )
	{
		this.StyleSheet.Load( "/swb_base/ui/AmmoDisplay.scss" );

		if ( uiSettings.ShowAmmoCount )
		{
			this._AmmoWrapper = this.Add.Panel( "ammoWrapper" );
			this._ClipLabel = this._AmmoWrapper.Add.Label( "", "clipLabel" );
			this._ReserveLabel = this._AmmoWrapper.Add.Label( "", "reserveLabel" );
		}

		if ( uiSettings.ShowWeaponIcon )
		{
			this._IconWrapper = this.Add.Panel( "iconWrapper" );
			this._WeaponIcon = this._IconWrapper.Add.Image( "", "weaponIcon" );
		}

		if ( uiSettings.ShowFireMode )
		{
			this._FireModeWrapper = this._IconWrapper.Add.Panel( "fireModeWrapper" );
			this._SemiFireIcon = this._FireModeWrapper.Add.Image( "/materials/swb/bullets/bullets_1.png", "fireModeIcon" );
			this._AutoFireIcon = this._FireModeWrapper.Add.Image( "/materials/swb/bullets/bullets_3.png", "fireModeIcon" );
		}
	}

	public override void Tick()
	{
		if ( Local.Pawn is not PlayerBase player )
		{
			return;
		}

		WeaponBase weapon = player.ActiveChild as WeaponBase;
		bool isValidWeapon = weapon != null;

		this.SetClass( "hideAmmoDisplay", !isValidWeapon );

		if ( !isValidWeapon )
		{
			return;
		}

		if ( this._AmmoWrapper != null )
		{
			bool hasClipSize = weapon.Primary.ClipSize > 0;
			int reserveAmmo = Math.Min( player.AmmoCount( weapon.Primary.AmmoType ), 999 );

			if ( weapon.Primary.InfiniteAmmo != InfiniteAmmoType.Clip )
			{
				int clipAmmo = hasClipSize ? weapon.Primary.Ammo : reserveAmmo;
				clipAmmo = Math.Min( clipAmmo, 999 );

				this._ClipLabel.SetText( clipAmmo.ToString() );
				this._ClipLabel.Style.FontColor = clipAmmo == 0 ? this._EmptyColor : Color.White;
			}
			else
			{
				this._ClipLabel.SetText( "∞" );
				this._ClipLabel.Style.FontColor = Color.White;
			}

			if ( hasClipSize )
			{
				if ( weapon.Primary.InfiniteAmmo == InfiniteAmmoType.Normal )
				{
					this._ReserveLabel.SetText( "|" + reserveAmmo );
					this._ReserveLabel.Style.FontColor = reserveAmmo == 0 ? this._EmptyColor : this._ReserveColor;
				}
				else
				{
					this._ReserveLabel.SetText( "|∞" );
					this._ReserveLabel.Style.FontColor = this._ReserveColor;
				}
			}
		}

		if ( this._IconWrapper != null )
		{
			this._WeaponIcon.SetTexture( weapon.Icon );
		}

		if ( this._FireModeWrapper != null )
		{
			bool isSemiFire = weapon.Primary.FiringType == FiringType.semi;

			this._SemiFireIcon.Style.Opacity = isSemiFire ? 1 : 0.25f;
			this._AutoFireIcon.Style.Opacity = isSemiFire ? 0.25f : 1;
		}
	}
}
