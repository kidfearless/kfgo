
using Sandbox;

using System;

namespace KFGO
{
	public delegate void HotReload( DateTime dateTime );

	public partial class KFGame : Sandbox.Game
	{
		private static KFGame _instance;

		public static new KFGame Current => _instance;

		public static event HotReload HotReload;
		private static DateTime LastHotReloadTime;

		public static bool TryInvokeHotReload( DateTime time )
		{
			//if(!Host.IsServer)
			//{
			//	return false;
			//}
			if ( time - LastHotReloadTime > TimeSpan.FromSeconds( 0.1 ) )
			{
				HotReload?.Invoke( time );
				if ( !Host.IsServer )
				{
					Admin_HotLoad();
				}

				LastHotReloadTime = time;
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
			_instance = this;
			Global.TickRate = 128;
			KFGame.HotReload += this.KFGame_HotReload;
			this.InitializeGame();
		}

		private void KFGame_HotReload( DateTime dateTime )
		{
			//InitializeGame();
			if ( !this.IsServer )
			{
				return;
			}

			foreach ( Client client in Client.All )
			{

				this.ClientJoined( client );
			}
		}

		protected virtual void InitializeGame()
		{
			if ( this.IsServer )
			{
				Log.Info( "My Gamemode Has Created Serverside!" );
				_ = new DeathmatchHud();
			}

			if ( this.IsClient )
			{
				Log.Info( "My Gamemode Has Created Clientside!" );
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
