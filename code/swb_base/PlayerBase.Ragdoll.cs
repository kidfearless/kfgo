using Sandbox;

namespace SWB_Base
{
	partial class PlayerBase
	{
		// TODO - make ragdolls one per entity
		// TODO - make ragdolls dissapear after a load of seconds
		static EntityLimit RagdollLimit = new EntityLimit() { MaxTotal = 20 };

		[ClientRpc]
		protected virtual void BecomeRagdollOnClient( Vector3 force, int forceBone )
		{
			// TODO - lets not make everyone write this shit out all the time
			// maybe a CreateRagdoll<T>() on ModelEntity?
			ModelEntity ent = new ModelEntity
			{
				Position = Position,
				Rotation = Rotation,
				MoveType = MoveType.Physics,
				UsePhysicsCollision = true
			};
			ent.SetInteractsAs( CollisionLayer.Debris );
			ent.SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
			ent.SetInteractsExclude( CollisionLayer.Player | CollisionLayer.Debris );

			ent.SetModel( this.GetModelName() );
			ent.CopyBonesFrom( this );
			ent.TakeDecalsFrom( this );
			ent.SetRagdollVelocityFrom( this );
			ent.DeleteAsync( 20.0f );

			// Copy the clothes over
			foreach ( Entity child in this.Children )
			{
				if ( child is ModelEntity e )
				{
					string model = e.GetModelName();
					if ( model != null && !model.Contains( "clothes" ) ) // Uck we 're better than this, entity tags, entity type or something?
					{
						continue;
					}

					ModelEntity clothing = new ModelEntity();
					clothing.SetModel( model );
					clothing.SetParent( ent, true );
				}
			}

			ent.PhysicsGroup.AddVelocity( force );

			if ( forceBone >= 0 )
			{
				PhysicsBody body = ent.GetBonePhysicsBody( forceBone );
				if ( body != null )
				{
					body.ApplyForce( force * 1000 );
				}
				else
				{
					ent.PhysicsGroup.AddVelocity( force );
				}
			}

			this.Corpse = ent;

			RagdollLimit.Watch( ent );
		}
	}
}
