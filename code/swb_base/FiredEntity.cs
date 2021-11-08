using Sandbox;

/* 
 * Weapon base for weapons firing entities
*/

namespace SWB_Base
{
    public class FiredEntity : ModelEntity
	{
		public virtual WeaponBase Weapon { get; set; } // The parent weapon
		public virtual Vector3 StartVelocity { get; set; }
		public virtual float RemoveDelay { get; set; }
		public virtual float Damage { get; set; }
		public virtual float Force { get; set; }
		public virtual float Speed { get; set; }
		public virtual bool UseGravity { get; set; }
		public virtual bool IsSticky { get; set; }

		public virtual bool CanThink { get; set; }
		public virtual bool IsStuck { get; set; }

		public override void Spawn()
		{
			base.Spawn();
		}

		public virtual void Start()
		{
			// Initialize physics
			this.MoveType = MoveType.Physics;
			this.PhysicsEnabled = true;
			this.UsePhysicsCollision = true;
			this.SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
			this.PhysicsGroup.AddVelocity( this.StartVelocity * this.Speed );
			this.PhysicsBody.GravityEnabled = this.UseGravity;

			// Delete entity
			_ = this.DeleteAsync( this.RemoveDelay );
		}

		protected override void OnPhysicsCollision(CollisionEventData eventData)
		{
			base.OnPhysicsCollision(eventData);

			if ( this.IsSticky && eventData.Entity.IsValid())
			{
				this.Velocity = Vector3.Zero;
				this.Parent = eventData.Entity;
			}
		}

		[Event.Tick.Server]
		public virtual void Tick()
		{
		}
	}
}
