//using Sandbox.UI;
//using Sandbox.UI.Construct;

//using System.Threading.Tasks;

//public partial class KillFeedEntry:Panel
//{
//	public Label Left { get; internal set; }
//	public Label Right { get; internal set; }
//	public Panel Icon { get; internal set; }

//	public KillFeedEntry()
//	{
//		this.Left = this.Add.Label( "", "left" );
//		this.Icon = this.Add.Panel( "icon" );
//		this.Right = this.Add.Label( "", "right" );

//		_ = this.RunAsync();
//	}

//	async Task RunAsync()
//	{
//		await this.Task.Delay( 4000 );
//		this.Delete();
//	}
//}
