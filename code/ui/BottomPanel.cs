using Sandbox;
using Sandbox.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace KFGO.UI
{
	internal partial class HealthPanel : ReloadPanel
	{
		public const string ICON_PATH = "/icons/health.png";
		public Image Icon { get; private set; }
		public Label Health { get; private set; }

		protected override void InitializeComponents()
		{
			Image icon = this.AddChild<Image>( "healthicon" );
			icon.SetTexture( ICON_PATH );
			this.Icon = icon;

			Label text = this.AddChild<Label>( "healthtext" );
			text.Text = "100";
			this.Health = text;
		}

		protected override void OnValidTick( KFPlayer player )
		{
			this.Health.Text = player.Health.CeilToInt().ToString();
		}
	}

	internal partial class BottomPanel : ReloadPanel
	{
		public HealthPanel HealthBar { get; private set; }
		public ArmorPanel ArmorBar { get; private set; }

		protected override void InitializeComponents()
		{
			this.HealthBar = this.AddChild<HealthPanel>( nameof( HealthPanel ) );
			this.ArmorBar = this.AddChild<ArmorPanel>( nameof( ArmorPanel ) );
		}
	}

	internal partial class ArmorPanel : ReloadPanel
	{
		public const string ICON_PATH = "/icons/armor.png";

		public Image Icon { get; private set; }
		public Label Armor { get; private set; }

		protected override void InitializeComponents()
		{
			Image icon = this.AddChild<Image>( "armoricon" );
			icon.SetTexture( ICON_PATH );
			this.Icon = icon;

			Label text = this.AddChild<Label>( "healthtext" );
			text.Text = "100";
			this.Armor = text;
		}

		protected override void OnValidTick( KFPlayer player )
		{
			this.Armor.Text = player.Armor.CeilToInt().ToString();
		}
	}
}
