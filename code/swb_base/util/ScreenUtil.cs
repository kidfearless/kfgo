using Sandbox;

using System;

namespace SWB_Base
{
	partial class ScreenUtil
	{
		public static void Shake( float length = 0, float speed = 0, float size = 0, float rotation = 0 )
		{
			ShakeRPC( length, speed, size, rotation );
		}

		public static void Shake( To to, float length = 0, float speed = 0, float size = 0, float rotation = 0 )
		{
			ShakeRPC( to, length, speed, size, rotation );
		}

		public static void Shake( To to, ClipInfo clip )
		{
			ref var screenShake = ref clip.GetScreenShake();
				Shake( to, screenShake.Length, screenShake.Speed, screenShake.Size, screenShake.Rotation );
		}

		public static void Shake( ScreenShake screenShake )
		{
				Shake( screenShake.Length, screenShake.Speed, screenShake.Size, screenShake.Rotation );
		}

		public static void ShakeAt( Vector3 origin, float radius = 0, float length = 0, float speed = 0, float size = 0, float rotation = 0 )
		{
			System.Collections.Generic.IEnumerable<Entity> objects = Physics.GetEntitiesInSphere( origin, radius );

			foreach ( Entity obj in objects )
			{
				// Player check
				if ( obj is not Player ply || !ply.IsValid() )
				{
					continue;
				}

				// Distance check
				Vector3 targetPos = ply.PhysicsBody.MassCenter;
				float dist = Vector3.DistanceBetween( origin, targetPos );
				if ( dist > radius )
				{
					continue;
				}

				// Intensity calculation
				float distanceMul = 1.0f - Math.Clamp( dist / radius, 0.0f, 0.75f );
				rotation *= distanceMul;
				size *= distanceMul;

				ShakeRPC( To.Single( ply ), length, speed, size, rotation );
			}
		}

		public static void ShakeAt( Vector3 position, float radius, ScreenShake screenShake )
		{
				ShakeAt( position, radius, screenShake.Length, screenShake.Speed, screenShake.Size, screenShake.Rotation );
		}

		[ClientRpc]
		public static void ShakeRPC( float length = 0, float speed = 0, float size = 0, float rotation = 0 )
		{
			new Sandbox.ScreenShake.Perlin( length, speed, size, rotation );
		}
	}
}
