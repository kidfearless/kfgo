
using Sandbox;

using SWB_CSS;

using System;

namespace KFGO
{


	public delegate void HotReload( DateTime dateTime );

	public partial class KFGame : Sandbox.Game
	{
		private static DateTime _LastHotReloadTime;
		private static KFGame _Instance;

		public static new KFGame Current => _Instance;
		public static event HotReload HotReload;
		public WeaponData WeaponData { get; private set; }



		public static bool TryInvokeHotReload( DateTime time )
		{
			//if(!Host.IsServer)
			//{
			//	return false;
			//}
			if ( time - _LastHotReloadTime > TimeSpan.FromSeconds( 0.1 ) )
			{
				HotReload?.Invoke( time );
				if ( !Host.IsServer )
				{
					Admin_HotLoad();
				}

				_LastHotReloadTime = time;
				return true;
			}

			return false;
		}

#if DEBUG
		[ServerCmd( "__hotreload_server" )]
#else
        [AdminCmd("__hotreload_server")]
#endif
		public static void Admin_HotLoad()
		{
			if ( Host.IsServer )
			{
				Log.Info( "Server" );
				TryInvokeHotReload( DateTime.Now );
			}
		}

		public KFGame()
		{
			_Instance = this;
			Global.TickRate = 128;
			//KFGame.HotReload += this.KFGame_HotReload;
			this.InitializeGame();		
		}

		protected virtual void InitializeGame()
		{
			WeaponData.Parse();
			for ( int i = 0; i < 10; i++ ) KLog.Info();

			KLog.JSON( new MAC10() );
			KLog.JSON( new SWB_CSS.M4A1() );
			KLog.JSON( new SWB_CSS.DeagleDual() );
			KLog.JSON( new SWB_CSS.M249() );
			KLog.JSON( new SWB_CSS.Deagle() );



			if ( this.IsServer )
			{
				if ( Local.Hud == null )
				{
					_ = new KFHud();
				}
			}
		}

		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );
			Log.Info( "ClientJoined" );

			client.Pawn?.Delete();

			KFPlayer player = new KFPlayer();
			client.Pawn = player;

			player.Respawn();
		}
	}
}
