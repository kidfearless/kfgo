using Sandbox;

using System;
using System.Collections.Generic;

namespace SWB_Base
{
	public enum AmmoType
	{
		Pistol,
		Revolver,
		Shotgun,
		SMG,
		Rifle,
		Sniper,
		LMG,
		Crossbow,
		RPG,
		Explosive,
		Grenade
	}

	public enum InfiniteAmmoType
	{
		Normal,
		Clip,
		Reserve
	}

	partial class PlayerBase
	{
		[Net]
		public List<int> Ammo { get; set; } = new();

		public virtual void ClearAmmo()
		{
			this.Ammo.Clear();
		}

		public virtual int AmmoCount( AmmoType type )
		{
			int iType = (int)type;
			if ( this.Ammo == null )
			{
				return 0;
			}

			if ( this.Ammo.Count <= iType )
			{
				return 0;
			}

			return this.Ammo[(int)type];
		}

		public virtual bool SetAmmo( AmmoType type, int amount )
		{
			int iType = (int)type;
			if ( !Host.IsServer )
			{
				return false;
			}

			if ( this.Ammo == null )
			{
				return false;
			}

			while ( this.Ammo.Count <= iType )
			{
				this.Ammo.Add( 0 );
			}

			this.Ammo[(int)type] = amount;
			return true;
		}

		public virtual bool GiveAmmo( AmmoType type, int amount )
		{
			if ( !Host.IsServer )
			{
				return false;
			}

			if ( this.Ammo == null )
			{
				return false;
			}

			this.SetAmmo( type, this.AmmoCount( type ) + amount );
			return true;
		}

		public virtual int TakeAmmo( AmmoType type, int amount )
		{
			if ( this.Ammo == null )
			{
				return 0;
			}

			int available = this.AmmoCount( type );
			amount = Math.Min( available, amount );

			this.SetAmmo( type, available - amount );

			return amount;
		}

		public virtual bool HasAmmo( AmmoType type )
		{
			return this.AmmoCount( type ) > 0;
		}
	}
}
