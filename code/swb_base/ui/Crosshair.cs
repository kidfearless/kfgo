
using Sandbox;
using Sandbox.UI;

using System.Threading.Tasks;

namespace SWB_Base
{

	public class Crosshair : Panel
	{

		Panel CenterDot;
		Panel LeftBar;
		Panel RightBar;
		Panel TopBar;
		Panel BottomBar;

		private int spreadOffset = 400;
		private int sprintOffset = 100;
		private int fireOffset = 50;

		private bool wasZooming = false;

		public Crosshair()
		{
			this.StyleSheet.Load( "/swb_base/ui/Crosshair.scss" );

			this.CenterDot = this.Add.Panel( "centerDot" );
			this.LeftBar = this.Add.Panel( "leftBar" );
			this.RightBar = this.Add.Panel( "rightBar" );
			this.TopBar = this.Add.Panel( "topBar" );
			this.BottomBar = this.Add.Panel( "bottomBar" );

			this.LeftBar.AddClass( "sharedBarStyling" );
			this.RightBar.AddClass( "sharedBarStyling" );
			this.TopBar.AddClass( "sharedBarStyling" );
			this.BottomBar.AddClass( "sharedBarStyling" );
		}

		private void UpdateCrosshair()
		{
			this.CenterDot.Style.Dirty();
			this.LeftBar.Style.Dirty();
			this.RightBar.Style.Dirty();
			this.TopBar.Style.Dirty();
			this.BottomBar.Style.Dirty();
		}

		private void RestoreBarPositions()
		{
			this.LeftBar.Style.Left = -16;
			this.RightBar.Style.Left = 5;
			this.TopBar.Style.Top = -16;
			this.BottomBar.Style.Top = 5;
		}

		private void RestoreCrosshairOpacity()
		{
			this.CenterDot.Style.Opacity = 1;
			this.LeftBar.Style.Opacity = 1;
			this.RightBar.Style.Opacity = 1;
			this.TopBar.Style.Opacity = 1;
			this.BottomBar.Style.Opacity = 1;
		}

		private void HideBarLines()
		{
			this.LeftBar.Style.Opacity = 0;
			this.RightBar.Style.Opacity = 0;
			this.TopBar.Style.Opacity = 0;
			this.BottomBar.Style.Opacity = 0;
		}

		public override void Tick()
		{
			base.Tick();
			this.PositionAtCrosshair();

			Entity player = Local.Pawn;
			if ( player == null )
			{
				return;
			}

			WeaponBase weapon = player.ActiveChild as WeaponBase;
			bool isValidWeapon = weapon != null;

			bool hideCrosshairDot = isValidWeapon ? !weapon.UISettings.ShowCrosshairDot : true;
			this.CenterDot.SetClass( "hideCrosshair", hideCrosshairDot );

			bool hideCrosshairLines = isValidWeapon ? !weapon.UISettings.ShowCrosshairLines : true;
			this.LeftBar.SetClass( "hideCrosshair", hideCrosshairLines );
			this.RightBar.SetClass( "hideCrosshair", hideCrosshairLines );
			this.TopBar.SetClass( "hideCrosshair", hideCrosshairLines );
			this.BottomBar.SetClass( "hideCrosshair", hideCrosshairLines );

			if ( !isValidWeapon )
			{
				return;
			}

			// Crosshair spread offset
			float screenOffset = this.spreadOffset * weapon.GetRealSpread();
			this.LeftBar.Style.MarginLeft = -screenOffset;
			this.RightBar.Style.MarginLeft = screenOffset;
			this.TopBar.Style.MarginTop = -screenOffset;
			this.BottomBar.Style.MarginTop = screenOffset;

			// Sprint spread offsets
			if ( weapon.IsRunning || weapon.ShouldTuck() || weapon.IsReloading )
			{
				this.LeftBar.Style.Left = -this.sprintOffset;
				this.RightBar.Style.Left = this.sprintOffset - 5;
				this.TopBar.Style.Top = -this.sprintOffset;
				this.BottomBar.Style.Top = this.sprintOffset - 5;

				this.HideBarLines();
			}
			else if ( weapon.IsZooming )
			{
				this.wasZooming = true;

				if ( player.IsFirstPersonMode )
				{
					this.CenterDot.Style.Opacity = 0;
					this.HideBarLines();
				}
			}
			else if ( this.LeftBar.Style.Left == -this.sprintOffset || this.wasZooming )
			{
				this.wasZooming = false;
				this.RestoreBarPositions();
				this.RestoreCrosshairOpacity();
			}

			this.UpdateCrosshair();
		}

		[PanelEvent]
		public void FireEvent( float fireDelay )
		{
			// Fire spread offsets
			this.LeftBar.Style.Left = -this.fireOffset;
			this.RightBar.Style.Left = this.fireOffset - 5;
			this.TopBar.Style.Top = -this.fireOffset;
			this.BottomBar.Style.Top = this.fireOffset - 5;

			_ = this.FireDelay( fireDelay / 2 );
		}

		private async Task FireDelay( float delay )
		{
			await GameTask.DelaySeconds( delay );
			this.RestoreBarPositions();
		}
	}
}
