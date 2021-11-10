//using KFGO.UI;

//using Sandbox.UI;
//using Sandbox.UI.Construct;

//using SWB_Base;

//using System.Collections.Generic;
//using System.Linq;

//public class InventoryColumn:Panel
//{
//	public int Column;
//	public bool IsSelected;
//	public Label Header;
//	public int SelectedIndex;

//	internal List<InventoryIcon> _Icons = new();

//	public InventoryColumn( int i, Panel parent )
//	{
//		this.Parent = parent;
//		this.Column = i;
//		this.Header = this.Add.Label( $"{i + 1}", "slot-number" );
//	}

//	internal void UpdateWeapon( WeaponBase weapon )
//	{
//		InventoryIcon icon = this.ChildrenOfType<InventoryIcon>().FirstOrDefault( x => x.Weapon == weapon );
//		if ( icon == null )
//		{
//			icon = new InventoryIcon( weapon );
//			icon.Parent = this;
//			this._Icons.Add( icon );
//		}
//	}

//	internal void TickSelection( WeaponBase selectedWeapon )
//	{
//		this.SetClass( "active", selectedWeapon?.Bucket == this.Column );

//		for ( int i = 0; i < this._Icons.Count; i++ )
//		{
//			this._Icons[i].TickSelection( selectedWeapon );
//		}
//	}
//}
