using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SWB_Base
{

	public class SniperScope:Panel
	{
		protected Panel LensWrapper { get; set; }
		protected Panel ScopeWrapper { get; set; }

		protected Panel LeftBar { get; set; }
		protected Panel RightBar { get; set; }
		protected Panel TopBar { get; set; }
		protected Panel BottomBar { get; set; }

		protected Image Lens { get; set; }
		protected Image Scope { get; set; }


		public SniperScope( string lensTexture, string scopeTexture )
		{
			this.StyleSheet.Load( "/swb_base/ui/SniperScope.scss" );

			this.LensWrapper = this.Add.Panel( "lensWrapper" );
			this.Lens = this.LensWrapper.Add.Image( lensTexture, "lens" );

			if ( scopeTexture != null )
			{
				this.Scope = this.LensWrapper.Add.Image( scopeTexture, "scope" );

				this.LeftBar = this.Add.Panel( "leftBar" );
				this.RightBar = this.Add.Panel( "rightBar" );
				this.TopBar = this.Add.Panel( "topBar" );
				this.BottomBar = this.Add.Panel( "bottomBar" );
			}
		}

		public override void Tick()
		{
			base.Tick();

			Entity player = Local.Pawn;
			if ( player == null )
			{
				return;
			}

			// Show when zooming
			this.Style.Opacity = (player.ActiveChild is not WeaponBase weapon || !weapon.IsScoped) ? 0 : 1;

			this.Style.Dirty();
		}
	}
}
