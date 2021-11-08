using Sandbox;

namespace SWB_Base
{
	/// <summary>
	/// An entity that can be carried in the player's inventory and hands.
	/// </summary>
	public class CarriableBase : BaseCarriable
	{
		public override void ActiveStart( Entity ent )
		{
			//base.ActiveStart( ent );

			this.EnableDrawing = true;

			if ( ent is Player player )
			{
				PawnAnimator animator = player.GetActiveAnimator();
				if ( animator != null )
				{
					this.SimulateAnimator( animator );
				}
			}

			//
			// If we're the local player (clientside) create viewmodel
			// and any HUD elements that this weapon wants
			//
			if ( this.IsLocalPawn )
			{
				this.DestroyViewModel();
				this.DestroyHudElements();

				this.CreateViewModel();
				this.CreateHudElements();
			}
		}
	}
}
