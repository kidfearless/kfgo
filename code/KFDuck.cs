using Sandbox;

namespace KFGO
{
	[Library]
	public class KFDuck : BaseNetworkable
	{
		public BasePlayerController Controller;

		public bool IsActive; // replicate

		// Uck, saving off the bbox kind of sucks
		// and we should probably be changing the bbox size in PreTick
		Vector3 originalMins;
		Vector3 originalMaxs;

		public KFDuck( BasePlayerController controller )
		{
			this.Controller = controller;
		}

		public virtual void PreTick()
		{
			bool wants = Input.Down( InputButton.Duck );

			if ( wants != this.IsActive )
			{
				if ( wants )
				{
					this.IsActive = true;
				}
				else
				{
					TraceResult pm = this.Controller.TraceBBox( this.Controller.Position, this.Controller.Position, this.originalMins, this.originalMaxs );
					if ( !pm.StartedSolid )
					{
						this.IsActive = false;
					}
				}
			}

			if ( this.IsActive )
			{
				this.Controller.SetTag( "ducked" );
				this.Controller.EyePosLocal *= 0.5f;
			}
		}

		internal void UpdateBBox( ref Vector3 mins, ref Vector3 maxs )
		{
			this.originalMins = mins;
			this.originalMaxs = maxs;

			if ( this.IsActive )
			{
				maxs = maxs.WithZ( 36 );
			}
		}

		//
		// Coudl we do this in a generic callback too?
		//
		public float GetWishSpeed()
		{
			if ( !this.IsActive )
			{
				return -1;
			}

			return 64.0f;
		}
	}
}
