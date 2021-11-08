using Sandbox.UI;

using System.Threading.Tasks;

namespace SWB_Base
{

	public class Hitmarker : Panel
	{
		public static Hitmarker Current;
		private Marker activeMarker;

		public Hitmarker()
		{
			this.StyleSheet.Load( "/swb_base/ui/Hitmarker.scss" );
			Current = this;
		}

		public override void Tick()
		{
			base.Tick();
			this.PositionAtCrosshair();
		}

		public void Create( bool isKill )
		{
			if ( this.activeMarker != null )
			{
				this.activeMarker.Delete();
			}

			this.activeMarker = new Marker( this, isKill );
		}

		public class Marker : Panel
		{
			public Marker( Panel parent, bool isKill )
			{
				this.Parent = parent;

				Panel leftTopBar = this.Add.Panel( "leftTopBar" );
				Panel lefBottomBar = this.Add.Panel( "leftBottomBar" );
				Panel rightTopBar = this.Add.Panel( "rightTopBar" );
				Panel rightBottomBar = this.Add.Panel( "rightBottomBar" );

				string sharedStyling = isKill ? "sharedBarStylingKill" : "sharedBarStyling";
				leftTopBar.AddClass( sharedStyling );
				lefBottomBar.AddClass( sharedStyling );
				rightTopBar.AddClass( sharedStyling );
				rightBottomBar.AddClass( sharedStyling );

				_ = this.Lifetime();
			}

			async Task Lifetime()
			{
				await this.Task.Delay( 100 );
				this.AddClass( "fadeOut" );
				await this.Task.Delay( 300 );
				this.Delete();
			}
		}
	}
}
