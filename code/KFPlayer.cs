
using Sandbox;

namespace KFGO
{
	partial class KFPlayer : SWB_Base.PlayerBase
	{
		public float Armor { get; set; }

		public override float Scale
		{
			get => base.Scale;

			set
			{
				base.Scale = value;
				//this.Controller?.UpdateScale(value);
			}
		}

		public virtual bool IsDead => this.Health <= 0 || this.IsDormant || this.LifeState == LifeState.Dead;

		public KFPlayer() : base()
		{
			Log.Info( "Init" );
		}

		public override void Spawn()
		{
			Log.Info( "Spawn" );
			base.Spawn();
		}

		public override void SetDefaultController() => this.Controller = new Controller();
		public override void Respawn()
		{
			Log.Info( "Respawn" );
			base.Respawn();

			this.Armor = 0;

			this.Inventory.Add( new SWB_CSS.Knife(), true );
			this.Inventory.Add( new SWB_CSS.AK47() );
		}


		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
		}
	}
}
