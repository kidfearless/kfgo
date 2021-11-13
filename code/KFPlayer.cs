
using Sandbox;

using SWB_Base;

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
			KLog.Info( "Respawn" );

			this.Armor = 0;

			//this.Inventory.Add( new SWB_CSS.Knife(), true );

			//this.Inventory.Add(MAC10.Create(), true);

			if ( IsClient )
			{
				KLog.Info( "IsClient" );
			}

			this.Inventory.Add( Weapon.CreateWeapon(nameof(MAC10)), true );
			base.Respawn();
		}
	}
}
