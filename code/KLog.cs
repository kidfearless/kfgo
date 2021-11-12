
using Sandbox;

namespace KFGO
{
	// https://youtu.be/bTRArPt8PHo?t=6
	public static class KLog
	{
		public static void Info( object message )
		{
			if( Host.IsClient )
			{
				Log.Info( $"[Client] " + message.ToString() );
			}
			else
			{
				Log.Info("[Server] " + message.ToString() );
			}
		}

		public static void Info<T>( T message )
		{
			if ( Host.IsClient )
			{
				Log.Info( $"[Client] " + message.ToString() );
			}
			else
			{
				Log.Info( "[Server] " + message.ToString() );
			}
		}

	}
}
