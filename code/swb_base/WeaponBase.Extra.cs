using Sandbox;

/* 
 * Weapon base for weapons using magazine based reloading 
*/

namespace SWB_Base
{
	public partial class WeaponBase
	{
		// Tucking
		public virtual float GetTuckDist()
		{
			if ( this.TuckRange == -1 )
			{
				return -1;
			}

			if ( this.Owner is not Player player )
			{
				return -1;
			}

			Vector3 pos = player.EyePos;
			Vector3 forward = this.Owner.EyeRot.Forward;
			TraceResult trace = Trace.Ray( pos, pos + (forward * this.TuckRange) )
					 .Ignore( this )
					 .Ignore( player )
					 .Run();

			if ( trace.Entity == null )
			{
				return -1;
			}

			return trace.Distance;
		}

		public bool ShouldTuck( float dist )
		{
			return dist != -1;
		}

		public bool ShouldTuck()
		{
			return this.GetTuckDist() != -1;
		}

		public bool ShouldTuck( out float dist )
		{
			dist = this.GetTuckDist();
			return dist != -1;
		}

		// Barrel heat
		public void AddBarrelHeat()
		{
			this.BarrelHeat += 1;
		}

		[Event.Tick.Server]
		public void BarrelHeatCheck()
		{
			if ( this.TimeSinceFired > 3 )
			{
				this.BarrelHeat = 0;
			}
		}
	}
}
