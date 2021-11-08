using Sandbox;
using Sandbox.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace KFGO
{
	/// <summary>
	/// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
	/// via RootPanel on this entity, or Local.Hud.
	/// </summary>
	public partial class KFHud : HudEntity<RootPanel>
	{
		public KFHud() : base()
		{
			if ( !this.IsClient )
			{
				return;
			}

			this.RootPanel.StyleSheet.Load( "/styles/KFHud.scss" );
			this.RootPanel.AddChild<UI.BottomPanel>();
		}
	}
}
