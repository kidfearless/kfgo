using Sandbox;
using Sandbox.UI;

using SWB_Base;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The main inventory panel, top left of the screen.
/// </summary>
public class InventoryBar : Panel
{
	List<InventoryColumn> _Columns = new();
	List<WeaponBase> _Weapons = new();

	public bool IsOpen { get; set; }
	WeaponBase _SelectedWeapon;

	public InventoryBar()
	{
		this.StyleSheet.Load( "swb_base/deathmatch_dep/ui/scss/InventoryBar.scss" );

		for ( int i = 0; i < 6; i++ )
		{
			InventoryColumn icon = new InventoryColumn( i, this );
			this._Columns.Add( icon );
		}
	}

	public override void Tick()
	{
		base.Tick();

		this.SetClass( "active", this.IsOpen );

		if ( Local.Pawn is not Player player )
		{
			return;
		}

		this._Weapons.Clear();
		this._Weapons.AddRange( player.Children
			.Select( x => x as WeaponBase )
			.Where( x => x.IsValid() && x.IsUsable() ) );

		foreach ( WeaponBase weapon in this._Weapons )
		{
			this._Columns[weapon.Bucket].UpdateWeapon( weapon );
		}
	}

	/// <summary>
	/// IClientInput implementation, calls during the client input build.
	/// You can both read and write to input, to affect what happens down the line.
	/// </summary>
	[Event.BuildInput]
	public void ProcessClientInput( InputBuilder input )
	{
		bool wantOpen = false;

		// If we're not open, maybe this input has something that will 
		// make us want to start being open?
		wantOpen = wantOpen || input.MouseWheel != 0;
		wantOpen = wantOpen || input.Pressed( InputButton.Slot1 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot2 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot3 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot4 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot5 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot6 );

		if ( this._Weapons.Count == 0 )
		{
			this.IsOpen = false;
			return;
		}

		// We're not open, but we want to be
		if ( this.IsOpen != wantOpen )
		{
			this._SelectedWeapon = Local.Pawn.ActiveChild as WeaponBase;
			this.IsOpen = true;
		}

		// Not open fuck it off
		if ( !this.IsOpen )
		{
			return;
		}

		//
		// Fire pressed when we're open - select the weapon and close.
		//
		if ( input.Down( InputButton.Attack1 ) )
		{
			input.SuppressButton( InputButton.Attack1 );
			input.ActiveChild = this._SelectedWeapon;
			this.IsOpen = false;
			Sound.FromScreen( "dm.ui_select" );
			return;
		}

		// get our current index
		WeaponBase oldSelected = this._SelectedWeapon;
		int SelectedIndex = this._Weapons.IndexOf( this._SelectedWeapon );
		SelectedIndex = this.SlotPressInput( input, SelectedIndex );

		// forward if mouse wheel was pressed
		SelectedIndex += input.MouseWheel;
		SelectedIndex = SelectedIndex.UnsignedMod( this._Weapons.Count );

		this._SelectedWeapon = this._Weapons[SelectedIndex];

		for ( int i = 0; i < 6; i++ )
		{
			this._Columns[i].TickSelection( this._SelectedWeapon );
		}

		input.MouseWheel = 0;

		if ( oldSelected != this._SelectedWeapon )
		{
			Sound.FromScreen( "dm.ui_tap" );
		}
	}

	int SlotPressInput( InputBuilder input, int SelectedIndex )
	{
		int columninput = -1;

		if ( input.Pressed( InputButton.Slot1 ) )
		{
			columninput = 0;
		}

		if ( input.Pressed( InputButton.Slot2 ) )
		{
			columninput = 1;
		}

		if ( input.Pressed( InputButton.Slot3 ) )
		{
			columninput = 2;
		}

		if ( input.Pressed( InputButton.Slot4 ) )
		{
			columninput = 3;
		}

		if ( input.Pressed( InputButton.Slot5 ) )
		{
			columninput = 4;
		}

		if ( input.Pressed( InputButton.Slot6 ) )
		{
			columninput = 5;
		}

		if ( columninput == -1 )
		{
			return SelectedIndex;
		}

		if ( this._SelectedWeapon.IsValid() && this._SelectedWeapon.Bucket == columninput )
		{
			return this.NextInBucket();
		}

		// Are we already selecting a weapon with this column?
		WeaponBase firstOfColumn = this._Weapons.Where( x => x.Bucket == columninput ).OrderBy( x => x.BucketWeight ).FirstOrDefault();
		if ( firstOfColumn == null )
		{
			// DOOP sound
			return SelectedIndex;
		}

		return this._Weapons.IndexOf( firstOfColumn );
	}

	int NextInBucket()
	{
		Assert.NotNull( this._SelectedWeapon );

		WeaponBase first = null;
		WeaponBase prev = null;
		foreach ( WeaponBase weapon in this._Weapons.Where( x => x.Bucket == this._SelectedWeapon.Bucket ).OrderBy( x => x.BucketWeight ) )
		{
			if ( first == null )
			{
				first = weapon;
			}

			if ( prev == this._SelectedWeapon )
			{
				return this._Weapons.IndexOf( weapon );
			}

			prev = weapon;
		}

		return this._Weapons.IndexOf( first );
	}
}
