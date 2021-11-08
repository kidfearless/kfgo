using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using SWB_Base;

public class AmmoDisplay : Panel
{
    Panel ammoWrapper;
    Panel iconWrapper;
    Panel fireModeWrapper;

    Image weaponIcon;
    Image semiFireIcon;
    Image autoFireIcon;

    Label clipLabel;
    Label reserveLabel;

    Color reserveColor = new Color32(200, 200, 200).ToColor();
    Color emptyColor = new Color32(175, 175, 175, 200).ToColor();

    public AmmoDisplay(UISettings uiSettings)
    {
		this.StyleSheet.Load("/swb_base/ui/AmmoDisplay.scss");

        if (uiSettings.ShowAmmoCount)
        {
			this.ammoWrapper = this.Add.Panel("ammoWrapper");
			this.clipLabel = this.ammoWrapper.Add.Label("", "clipLabel");
			this.reserveLabel = this.ammoWrapper.Add.Label("", "reserveLabel");
        }

        if (uiSettings.ShowWeaponIcon)
        {
			this.iconWrapper = this.Add.Panel("iconWrapper");
			this.weaponIcon = this.iconWrapper.Add.Image("", "weaponIcon");
        }

        if (uiSettings.ShowFireMode)
        {
			this.fireModeWrapper = this.iconWrapper.Add.Panel("fireModeWrapper");
			this.semiFireIcon = this.fireModeWrapper.Add.Image("/materials/swb/bullets/bullets_1.png", "fireModeIcon");
			this.autoFireIcon = this.fireModeWrapper.Add.Image("/materials/swb/bullets/bullets_3.png", "fireModeIcon");
        }
    }

    public override void Tick()
    {
        if (Local.Pawn is not PlayerBase player)
        {
            return;
        }

		WeaponBase weapon = player.ActiveChild as WeaponBase;
        bool isValidWeapon = weapon != null;

		this.SetClass("hideAmmoDisplay", !isValidWeapon);

        if (!isValidWeapon)
        {
            return;
        }

        if ( this.ammoWrapper != null)
        {
			bool hasClipSize = weapon.Primary.ClipSize > 0;
			int reserveAmmo = Math.Min(player.AmmoCount(weapon.Primary.AmmoType), 999);

            if (weapon.Primary.InfiniteAmmo != InfiniteAmmoType.Clip)
            {
				int clipAmmo = hasClipSize ? weapon.Primary.Ammo : reserveAmmo;
                clipAmmo = Math.Min(clipAmmo, 999);

				this.clipLabel.SetText(clipAmmo.ToString());
				this.clipLabel.Style.FontColor = clipAmmo == 0 ? this.emptyColor : Color.White;
            }
            else
            {
				this.clipLabel.SetText("∞");
				this.clipLabel.Style.FontColor = Color.White;
            }

            if (hasClipSize)
            {
                if (weapon.Primary.InfiniteAmmo == InfiniteAmmoType.Normal)
                {
					this.reserveLabel.SetText("|" + reserveAmmo);
					this.reserveLabel.Style.FontColor = reserveAmmo == 0 ? this.emptyColor : this.reserveColor;
                }
                else
                {
					this.reserveLabel.SetText("|∞");
					this.reserveLabel.Style.FontColor = this.reserveColor;
                }
            }
        }

        if ( this.iconWrapper != null)
        {
			this.weaponIcon.SetTexture(weapon.Icon);
        }

        if ( this.fireModeWrapper != null)
        {
			bool isSemiFire = weapon.Primary.FiringType == FiringType.semi;

			this.semiFireIcon.Style.Opacity = isSemiFire ? 1 : 0.25f;
			this.autoFireIcon.Style.Opacity = isSemiFire ? 0.25f : 1;
        }
    }
}
