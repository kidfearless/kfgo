
using Sandbox;
using Sandbox.UI;

using SWB_Base;

class InventoryIcon : Panel
{
	public WeaponBase Weapon;
	public Image Icon;

	public InventoryIcon( WeaponBase weapon )
	{
		this.Weapon = weapon;
		this.AddChild( out this.Icon, "icon" );
		this.Icon.SetTexture( this.Weapon.Icon );
	}

	internal void TickSelection( WeaponBase selectedWeapon )
	{
		this.SetClass( "active", selectedWeapon == this.Weapon );
		this.SetClass( "empty", !this.Weapon?.IsUsable() ?? true );
	}

	public override void Tick()
	{
		base.Tick();

		if ( !this.Weapon.IsValid() || this.Weapon.Owner != Local.Pawn )
		{
			this.Delete();
		}
	}
}
