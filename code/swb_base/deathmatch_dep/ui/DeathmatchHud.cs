
using Sandbox;
using Sandbox.UI;

[Library]
public partial class DeathmatchHud : HudEntity<RootPanel>
{
	public DeathmatchHud()
	{
		if ( !this.IsClient )
		{
			return;
		}

		this.RootPanel.StyleSheet.Load( "swb_base/deathmatch_dep/ui/scss/DeathmatchHud.scss" );

		//this.RootPanel.AddChild<NameTags>();
		//this.RootPanel.AddChild<DamageIndicator>();

		this.RootPanel.AddChild<InventoryBar>();
		//this.RootPanel.AddChild<PickupFeed>();

		this.RootPanel.AddChild<ChatBox>();
		this.RootPanel.AddChild<KillFeed>();
		this.RootPanel.AddChild<Scoreboard>();
		this.RootPanel.AddChild<VoiceList>();
	}

	[ClientRpc]
	public void OnPlayerDied( string victim, string attacker = null )
	{
		Host.AssertClient();
	}

	[ClientRpc]
	public void ShowDeathScreen( string attackerName )
	{
		Host.AssertClient();
	}
}
