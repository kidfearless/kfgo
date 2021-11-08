
using Sandbox;
using Sandbox.UI;

using SWB_Base;

using System;

public partial class KillFeed : Sandbox.UI.KillFeed
{
	public KillFeed()
	{
		this.StyleSheet.Load( "swb_base/deathmatch_dep/ui/scss/KillFeed.scss" );
	}

	public override Panel AddEntry( ulong lsteamid, string left, ulong rsteamid, string right, string method )
	{
		Log.Info( $"{left} killed {right} using {method}" );

		KillFeedEntry e = Current.AddChild<KillFeedEntry>();

		e.AddClass( method );

		e.Left.Text = left;
		e.Left.SetClass( "me", lsteamid == Local.SteamId );

		try
		{
			// Temp solution ( get reference to kill weapon icon )
			if ( method.StartsWith( "swb_" ) )
			{
				WeaponBase killWeapon = Library.Create<WeaponBase>( method ); // throws error when not found

				if ( !string.IsNullOrEmpty( killWeapon.Icon ) )
				{
					e.Icon.Style.BackgroundImage = Texture.Load( killWeapon.Icon );
					killWeapon.Delete();
				}
			}
		}
		catch ( Exception exception ) { }

		e.Right.Text = right;
		e.Right.SetClass( "me", rsteamid == Local.SteamId );

		return e;
	}
}
