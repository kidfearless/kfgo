using Sandbox;

using System;

namespace SWB_Base
{
	partial class BlastUtil
	{
		public static void Explode( Vector3 origin, float radius, float damage, float force, Entity attacker = null, Entity weapon = null, ModelEntity explodingEnt = null )
		{
			System.Collections.Generic.IEnumerable<Entity> objects = Physics.GetEntitiesInSphere( origin, radius );

			foreach ( Entity obj in objects )
			{
				// Entity check
				if ( obj is not ModelEntity ent || !ent.IsValid() )
				{
					continue;
				}

				if ( ent.LifeState != LifeState.Alive )
				{
					continue;
				}

				if ( !ent.PhysicsBody.IsValid() )
				{
					continue;
				}

				if ( ent.IsWorld )
				{
					continue;
				}

				// Dist check
				Vector3 targetPos = ent.PhysicsBody.MassCenter;
				float dist = Vector3.DistanceBetween( origin, targetPos );
				if ( dist > radius )
				{
					continue;
				}

				// Temp solution
				TraceResult tr = Trace.Ray( origin, targetPos )
						  .Ignore( explodingEnt )
						  .WorldOnly()
						  .Run();

				if ( tr.Fraction < 1.0f )
				{
					continue;
				}

				float distanceMul = 1.0f - Math.Clamp( dist / radius, 0.0f, 1.0f );
				float realDamage = damage * distanceMul;
				float realForce = force * distanceMul;
				Vector3 forceDir = (targetPos - origin).Normal;

				ent.TakeDamage( DamageInfo.Explosion( origin, forceDir * realForce, realDamage )
						  .WithAttacker( attacker )
						  .WithWeapon( weapon ) );
			}
		}
	}
}
