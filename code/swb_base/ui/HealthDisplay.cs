using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using SWB_Base;

public class HealthDisplay : Panel
{
    Panel healthWrapper;

    Image healthIcon;
    Label healthLabel;

    public HealthDisplay(UISettings uiSettings)
    {
		this.StyleSheet.Load("/swb_base/ui/HealthDisplay.scss");

		this.healthWrapper = this.Add.Panel("healthWrapper");

        if (uiSettings.ShowHealthIcon)
        {
			this.healthIcon = this.healthWrapper.Add.Image("/materials/swb/hud/health.png", "healthIcon");
        }

        if (uiSettings.ShowHealthCount)
        {
			this.healthLabel = this.healthWrapper.Add.Label("", "healthLabel");
        }
    }

    public override void Tick()
    {
        if (Local.Pawn is not PlayerBase player)
        {
            return;
        }

		bool isAlive = player.Alive();
		this.SetClass("hideHealthDisplay", !isAlive);

        if (!isAlive)
        {
            return;
        }

		double health = Math.Round(player.Health);
		float healthPer = ((float)health) / 100f;

        if ( this.healthIcon != null)
        {
			this.healthIcon.Style.Opacity = 1; // healthPer
        }

        if ( this.healthLabel != null)
        {
			this.healthLabel.SetText(health.ToString());
			this.healthLabel.Style.FontColor = new Color(1, 1 * healthPer, 1 * healthPer);
        }
    }
}
