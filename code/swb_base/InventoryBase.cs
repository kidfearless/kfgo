using Sandbox;

using SWB_Base;

using System;
using System.Linq;

partial class InventoryBase : BaseInventory
{
	public InventoryBase( PlayerBase player ) : base( player )
	{
	}

	public override bool Add( Entity ent, bool makeActive = false )
	{
		PlayerBase player = this.Owner as PlayerBase;
		WeaponBase weapon = ent as WeaponBase;

		bool showNotice = !player.SupressPickupNotices;

		if ( weapon != null && this.IsCarryingType( ent.GetType() ) )
		{
			int ammo = weapon.Primary.Ammo;
			AmmoType ammoType = weapon.Primary.AmmoType;

			if ( ammo > 0 )
			{
				player.GiveAmmo( ammoType, ammo );

				if ( showNotice )
				{
					Sound.FromWorld( "dm.pickup_ammo", ent.Position );
				}
			}

			// Despawn it
			ent.Delete();
			return false;
		}

		if ( weapon != null && showNotice )
		{
			Sound.FromWorld( "dm.pickup_weapon", ent.Position );
		}

		return base.Add( ent, makeActive );
	}

	public virtual bool IsCarryingType<T>()
	{
		return this.List.Any( x => x is T );
	}

	public virtual bool IsCarryingType( Type type )
	{
		return this.List.Any( x => x.GetType() == type );
	}

	public override Entity DropActive()
	{
		if ( !Host.IsServer )
		{
			return null;
		}

		Entity ent = this.Owner.ActiveChild;

		if ( ent is WeaponBase weapon && weapon.CanDrop && this.Drop( ent ) )
		{
			this.Owner.ActiveChild = null;
			return ent;
		}

		return null;
	}
}
