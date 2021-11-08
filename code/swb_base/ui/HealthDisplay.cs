using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using SWB_Base;

using System;

public class HealthDisplay : Panel
{
	protected Panel HealthWrapper { get; set; }
	protected Image HealthIcon { get; set; }
	protected Label HealthLabel { get; set; }

	public HealthDisplay( UISettings uiSettings )
	{
		this.StyleSheet.Load( "/swb_base/ui/HealthDisplay.scss" );

		this.HealthWrapper = this.Add.Panel( "healthWrapper" );

		if ( uiSettings.ShowHealthIcon )
		{
			this.HealthIcon = this.HealthWrapper.Add.Image( "/materials/swb/hud/health.png", "healthIcon" );
		}

		if ( uiSettings.ShowHealthCount )
		{
			this.HealthLabel = this.HealthWrapper.Add.Label( "", "healthLabel" );
		}
	}


	public override void Tick()
	{
		if ( Local.Pawn is not PlayerBase player )
		{
			return;
		}

		bool isAlive = player.Alive();
		this.SetClass( "hideHealthDisplay", !isAlive );

		if ( !isAlive )
		{
			return;
		}

		double health = Math.Round( player.Health );
		float healthPer = ((float)health) / 100f;

		if ( this.HealthIcon != null )
		{
			this.HealthIcon.Style.Opacity = 1; // healthPer
		}

		if ( this.HealthLabel != null )
		{
			this.HealthLabel.SetText( health.ToString() );
			this.HealthLabel.Style.FontColor = new Color( 1, 1 * healthPer, 1 * healthPer );
		}
	}
}
