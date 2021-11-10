
using Sandbox;

using SWB_CSS;

namespace KFGO
{
	public partial class KFPlayer : SWB_Base.PlayerBase
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

		public override void SetDefaultController() => this.Controller = new Controller();

		public override void Spawn()
		{
			Log.Info( "Spawn" );
			base.Spawn();
		}

		public override void Respawn()
		{
			Log.Info( "Respawn" );
			Log.Info( IsServer );
			base.Respawn();

			this.Armor = 0;

			//this.Inventory.Add( new SWB_CSS.Knife(), true );

			this.Inventory.Add(MAC10.Create(), true);
			// this.Inventory.Add( KFGame.Current.WeaponData.CreateWeaponByName<AK47>(nameof(AK47)), true );
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
