using Sandbox;
using Sandbox.UI;

namespace KFGO.UI
{
	/// <summary>
	/// A panel that provides better reloading experience.
	/// Build your components in the InitializeComponents method.
	/// There is no need to call the base class' InitializeComponents method.
	/// </summary>
	internal partial class ReloadPanel : Panel
	{
		public ReloadPanel() : base()
		{
			this.InitializeComponents();
		}

		/// <summary>
		/// Method to instantiate your UI components.
		/// Elements created inside this method will be deleted and recreated when the gamemode is (re)loaded.
		/// </summary>
		protected virtual void InitializeComponents()
		{

		}

		public override void Tick()
		{
			base.Tick();
			if ( Local.Pawn is not KFPlayer player )
			{
				return;
			}

			this.OnValidTick( player );
		}

		protected virtual void OnValidTick( KFPlayer player )
		{
		}

		public override void OnHotloaded()
		{

			KFGame.TryInvokeHotReload( System.DateTime.Now );
			Log.Info( "OnHotLoad" );
			base.OnHotloaded();
			this.DeleteChildren( true );
			this.InitializeComponents();
		}
	}
}
