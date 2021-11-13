
using Sandbox;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace KFGO
{
	// https://youtu.be/bTRArPt8PHo?t=6
	public static class KLog
	{
		private static string Location => Host.IsClient ? "[Client] " : "[Server] ";
		public static void JSON<T>( T obj )
		{
			var serializer = new JsonSerializerOptions()
			{
				WriteIndented = true,
				IgnoreReadOnlyProperties = true,
				IncludeFields = true,
				ReferenceHandler = ReferenceHandler.Preserve,
			};
			var json = JsonSerializer.Serialize( obj, serializer );
			Log.Info( Location + json );
		}

		public static void Info()
		{
			Log.Info("\n");
		}

		public static void Info( object message )
		{
			Log.Info( Location + message.ToString() );
		}

		public static void Info<T>( T message )
		{
			Log.Info( Location + message.ToString() );
		}

	}
}
