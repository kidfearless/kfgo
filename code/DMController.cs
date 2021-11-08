
using Sandbox;
namespace KFGO
{
	partial class Controller : BasePlayerController
	{
		[Net] public float SprintSpeed { get; set; } = 250.0f;
		[Net] public float WalkSpeed { get; set; } = 250.0F;
		[Net] public float DefaultSpeed { get; set; } = 250.0f;
		[Net] public float Acceleration { get; set; } = 10.0f;
		[Net] public float AirAcceleration { get; set; } = 1000.0f;
		[Net] public float FallSoundZ { get; set; } = -30.0f;
		[Net] public float GroundFriction { get; set; } = 4.0f;
		[Net] public float StopSpeed { get; set; } = 100.0f;
		[Net] public float Size { get; set; } = 20.0f;
		[Net] public float DistEpsilon { get; set; } = 0.03125f;
		[Net] public float GroundAngle { get; set; } = 46.0f;
		[Net] public float Bounce { get; set; } = 0.0f;
		[Net] public float MoveFriction { get; set; } = 1.0f;
		[Net] public float StepSize { get; set; } = 18.0f;
		[Net] public float MaxNonJumpVelocity { get; set; } = 140000.0f;
		[Net] public float BodyGirth { get; set; } = 32.0f;
		[Net] public float BodyHeight { get; set; } = 72.0f;
		[Net] public float EyeHeight { get; set; } = 64.0f;
		[Net] public float Gravity { get; set; } = 800.0f;
		[Net] public float AirControl { get; set; } = 30.0f;
		public bool Swimming { get; set; } = false;
		[Net] public bool AutoJump { get; set; } = true;

		public Duck Duck;
		public Unstuck Unstuck;

		public Controller()
		{
			this.Duck = new Duck( this );
			this.Unstuck = new Unstuck( this );
		}

		/// <summary>
		/// This is temporary, get the hull size for the player's collision
		/// </summary>
		public override BBox GetHull()
		{
			float girth = this.BodyGirth * 0.5f;
			Vector3 mins = new Vector3( -girth, -girth, 0 );
			Vector3 maxs = new Vector3( +girth, +girth, this.BodyHeight );

			return new BBox( mins, maxs );
		}

		// Duck body height 32
		// Eye Height 64
		// Duck Eye Height 28

		protected Vector3 mins;
		protected Vector3 maxs;

		public virtual void SetBBox( Vector3 mins, Vector3 maxs )
		{
			if ( this.mins == mins && this.maxs == maxs )
			{
				return;
			}

			this.mins = mins;
			this.maxs = maxs;
		}

		/// <summary>
		/// Update the size of the bbox. We should really trigger some shit if this changes.
		/// </summary>
		public virtual void UpdateBBox()
		{
			float girth = this.BodyGirth * 0.5f;

			Vector3 mins = new Vector3( -girth, -girth, 0 ) * this.Pawn.Scale;
			Vector3 maxs = new Vector3( +girth, +girth, this.BodyHeight ) * this.Pawn.Scale;

			this.Duck.UpdateBBox( ref mins, ref maxs, this.Pawn.Scale );

			this.SetBBox( mins, maxs );
		}

		protected float SurfaceFriction;

		public override void FrameSimulate()
		{
			this.EyePosLocal = Vector3.Up * (this.EyeHeight * this.Pawn.Scale);

			base.FrameSimulate();
		}

		public override void Simulate()
		{
			this.EyePosLocal = Vector3.Up * (this.EyeHeight * this.Pawn.Scale);
			this.UpdateBBox();

			this.EyePosLocal += this.TraceOffset;
			this.EyeRot = Input.Rotation;

			this.RestoreGroundPos();

			if ( this.Unstuck.TestAndFix() )
			{
				return;
			}

			// RunLadderMode

			this.CheckLadder();
			this.Swimming = this.Pawn.WaterLevel.Fraction > 0.6f;

			//
			// Start Gravity
			//
			if ( !this.Swimming && !this.IsTouchingLadder )
			{
				this.Velocity -= new Vector3( 0, 0, this.Gravity * 0.5f ) * Time.Delta;
				this.Velocity += new Vector3( 0, 0, this.BaseVelocity.z ) * Time.Delta;

				this.BaseVelocity = this.BaseVelocity.WithZ( 0 );
			}

			if ( this.AutoJump ? Input.Down( InputButton.Jump ) : Input.Pressed( InputButton.Jump ) )
			{
				this.CheckJumpButton();
			}

			// Fricion is handled before we add in any base velocity. That way, if we are on a conveyor,
			//  we don't slow when standing still, relative to the conveyor.
			bool bStartOnGround = this.GroundEntity != null;
			//bool bDropSound = false;
			if ( bStartOnGround )
			{
				//if ( Velocity.z < FallSoundZ ) bDropSound = true;

				this.Velocity = this.Velocity.WithZ( 0 );
				//player->m_Local.m_flFallVelocity = 0.0f;

				if ( this.GroundEntity != null )
				{
					this.ApplyFriction( this.GroundFriction * this.SurfaceFriction );
				}
			}

			//
			// Work out wish velocity.. just take input, rotate it to view, clamp to -1, 1
			//

			this.WishVelocity = new Vector3( Input.Forward, Input.Left, 0 );
			float inSpeed = this.WishVelocity.Length.Clamp( 0, 1 );
			this.WishVelocity *= Input.Rotation;

			if ( !this.Swimming && !this.IsTouchingLadder )
			{
				this.WishVelocity = this.WishVelocity.WithZ( 0 );
			}

			this.WishVelocity = this.WishVelocity.Normal * inSpeed;
			this.WishVelocity *= this.GetWishSpeed();

			this.Duck.PreTick();

			this.ApplyBoosters();

			bool bStayOnGround = false;
			if ( this.Swimming )
			{
				this.ApplyFriction( 1 );
				this.WaterMove();
			}
			else if ( this.IsTouchingLadder )
			{
				this.LadderMove();
			}
			else if ( this.GroundEntity != null )
			{
				bStayOnGround = true;
				this.WalkMove();
			}
			else
			{
				this.AirMove();
			}

			this.CategorizePosition( bStayOnGround );

			// FinishGravity
			if ( !this.Swimming && !this.IsTouchingLadder )
			{
				this.Velocity -= new Vector3( 0, 0, this.Gravity * 0.5f ) * Time.Delta;
			}

			if ( this.GroundEntity != null )
			{
				this.Velocity = this.Velocity.WithZ( 0 );
			}

			// CheckFalling(); // fall damage etc

			// Land Sound
			// Swim Sounds

			this.SaveGroundPos();

			if ( Debug )
			{
				DebugOverlay.Box( this.Position + this.TraceOffset, this.mins, this.maxs, Color.Red );
				DebugOverlay.Box( this.Position, this.mins, this.maxs, Color.Blue );

				int lineOffset = 0;
				if ( Host.IsServer )
				{
					lineOffset = 10;
				}

				DebugOverlay.ScreenText( lineOffset + 0, $"        Position: {this.Position}" );
				DebugOverlay.ScreenText( lineOffset + 1, $"        Velocity: {this.Velocity}" );
				DebugOverlay.ScreenText( lineOffset + 2, $"    BaseVelocity: {this.BaseVelocity}" );
				DebugOverlay.ScreenText( lineOffset + 3, $"    GroundEntity: {this.GroundEntity} [{this.GroundEntity?.Velocity}]" );
				DebugOverlay.ScreenText( lineOffset + 4, $" SurfaceFriction: {this.SurfaceFriction}" );
				DebugOverlay.ScreenText( lineOffset + 5, $"    WishVelocity: {this.WishVelocity}" );
			}
		}

		private void ApplyBoosters()
		{
			//KFPlayer pawn = this.Pawn as KFPlayer;
			//if(pawn.Triggers.Count > 0)
			//{
			//	ClearGroundEntity();
			//}

			//foreach ( var item in pawn.Triggers )
			//{
			//	Velocity += item.pushdir * Time.Delta;
			//}
		}

		public virtual float GetWishSpeed()
		{
			float ws = this.Duck.GetWishSpeed();
			if ( ws >= 0 )
			{
				return ws;
			}

			if ( Input.Down( InputButton.Run ) )
			{
				return this.SprintSpeed;
			}

			if ( Input.Down( InputButton.Walk ) )
			{
				return this.WalkSpeed;
			}

			return this.DefaultSpeed;
		}

		public virtual void WalkMove()
		{
			Vector3 wishdir = this.WishVelocity.Normal;
			float wishspeed = this.WishVelocity.Length;

			this.WishVelocity = this.WishVelocity.WithZ( 0 );
			this.WishVelocity = this.WishVelocity.Normal * wishspeed;

			this.Velocity = this.Velocity.WithZ( 0 );
			this.Accelerate( wishdir, wishspeed, 0, this.Acceleration );
			this.Velocity = this.Velocity.WithZ( 0 );

			//   Player.SetAnimParam( "forward", Input.Forward );
			//   Player.SetAnimParam( "sideward", Input.Right );
			//   Player.SetAnimParam( "wishspeed", wishspeed );
			//    Player.SetAnimParam( "walkspeed_scale", 2.0f / 190.0f );
			//   Player.SetAnimParam( "runspeed_scale", 2.0f / 320.0f );

			//  DebugOverlay.Text( 0, Pos + Vector3.Up * 100, $"forward: {Input.Forward}\nsideward: {Input.Right}" );

			// Add in any base velocity to the current velocity.
			this.Velocity += this.BaseVelocity;

			try
			{
				if ( this.Velocity.Length < 1.0f )
				{
					this.Velocity = Vector3.Zero;
					return;
				}

				// first try just moving to the destination
				Vector3 dest = (this.Position + (this.Velocity * Time.Delta)).WithZ( this.Position.z );

				TraceResult pm = this.TraceBBox( this.Position, dest );

				if ( pm.Fraction == 1 )
				{
					this.Position = pm.EndPos;
					this.StayOnGround();
					return;
				}

				this.StepMove();
			}
			finally
			{

				// Now pull the base velocity back out.   Base velocity is set if you are on a moving object, like a conveyor (or maybe another monster?)
				this.Velocity -= this.BaseVelocity;
			}

			this.StayOnGround();
		}

		public virtual void StepMove()
		{
			MoveHelper mover = new MoveHelper( this.Position, this.Velocity );
			mover.Trace = mover.Trace.Size( this.mins, this.maxs ).Ignore( this.Pawn );
			mover.MaxStandableAngle = this.GroundAngle;

			mover.TryMoveWithStep( Time.Delta, this.StepSize );

			this.Position = mover.Position;
			this.Velocity = mover.Velocity;
		}

		public virtual void Move()
		{
			MoveHelper mover = new MoveHelper( this.Position, this.Velocity );
			mover.Trace = mover.Trace.Size( this.mins, this.maxs ).Ignore( this.Pawn );
			mover.MaxStandableAngle = this.GroundAngle;

			mover.TryMove( Time.Delta );

			this.Position = mover.Position;
			this.Velocity = mover.Velocity;
		}

		/// <summary>
		/// Add our wish direction and speed onto our velocity
		/// </summary>
		public virtual void Accelerate( Vector3 wishdir, float wishspeed, float speedLimit, float acceleration )
		{
			// This gets overridden because some games (CSPort) want to allow dead (observer) players
			// to be able to move around.
			// if ( !CanAccelerate() )
			//     return;

			if ( speedLimit > 0 && wishspeed > speedLimit )
			{
				wishspeed = speedLimit;
			}

			// See if we are changing direction a bit
			float currentspeed = this.Velocity.Dot( wishdir );

			// Reduce wishspeed by the amount of veer.
			float addspeed = wishspeed - currentspeed;

			// If not going to add any speed, done.
			if ( addspeed <= 0 )
			{
				return;
			}

			// Determine amount of acceleration.
			float accelspeed = acceleration * Time.Delta * wishspeed;

			// Cap at addspeed
			if ( accelspeed > addspeed )
			{
				accelspeed = addspeed;
			}

			accelspeed *= 1.5f;

			this.Velocity += wishdir * accelspeed;
		}

		/// <summary>
		/// Remove ground friction from velocity
		/// </summary>
		public virtual void ApplyFriction( float frictionAmount = 1.0f )
		{
			// Calculate speed
			float speed = this.Velocity.Length;
			if ( speed < 0.1f )
			{
				return;
			}

			// Bleed off some speed, but if we have less than the bleed
			//  threshold, bleed the threshold amount.
			float control = (speed < this.StopSpeed) ? this.StopSpeed : speed;

			// Add the amount to the drop amount.
			float drop = control * Time.Delta * frictionAmount;

			// scale the velocity
			float newspeed = speed - drop;
			if ( newspeed < 0 )
			{
				newspeed = 0;
			}

			if ( newspeed != speed )
			{
				newspeed /= speed;
				this.Velocity *= newspeed;
			}
		}

		public virtual void CheckJumpButton()
		{
			// If we are in the water most of the way...
			if ( this.Swimming )
			{
				// swimming, not jumping
				this.ClearGroundEntity();

				this.Velocity = this.Velocity.WithZ( 100 );

				return;
			}

			if ( this.GroundEntity == null )
			{
				return;
			}

			this.ClearGroundEntity();

			float flGroundFactor = 1.0f;
			float flMul = 268.3281572999747f * 1.2f;

			float startz = this.Velocity.z;

			if ( this.Duck.IsActive )
			{
				flMul *= 0.8f;
			}

			this.Velocity = this.Velocity.WithZ( startz + (flMul * flGroundFactor) );

			this.Velocity -= new Vector3( 0, 0, this.Gravity * 0.5f ) * Time.Delta;

			this.AddEvent( "jump" );

		}

		public override void OnEvent( string name )
		{
			base.OnEvent( name );
			if ( Game.Current.IsServer )
			{
				return;
			}


			if ( name == "jump" )
			{
				Vector3 horizontalvelocity = this.Velocity;
				horizontalvelocity.z = 0;
				Log.Info( horizontalvelocity.Length );
			}
		}

		public virtual void AirMove()
		{
			Vector3 wishdir = this.WishVelocity.Normal;
			float wishspeed = this.WishVelocity.Length;

			this.Accelerate( wishdir, wishspeed, this.AirControl, this.AirAcceleration );

			this.Velocity += this.BaseVelocity;

			this.Move();

			this.Velocity -= this.BaseVelocity;
		}

		public virtual void WaterMove()
		{
			Vector3 wishdir = this.WishVelocity.Normal;
			float wishspeed = this.WishVelocity.Length;

			wishspeed *= 0.8f;

			this.Accelerate( wishdir, wishspeed, 100, this.Acceleration );

			this.Velocity += this.BaseVelocity;

			this.Move();

			this.Velocity -= this.BaseVelocity;
		}

		bool IsTouchingLadder = false;
		Vector3 LadderNormal;

		public virtual void CheckLadder()
		{
			if ( this.IsTouchingLadder && Input.Pressed( InputButton.Jump ) )
			{
				this.Velocity = this.LadderNormal * 100.0f;
				this.IsTouchingLadder = false;

				return;
			}

			const float ladderDistance = 1.0f;
			Vector3 start = this.Position;
			Vector3 end = start + ((this.IsTouchingLadder ? (this.LadderNormal * -1.0f) : this.WishVelocity.Normal) * ladderDistance);

			TraceResult pm = Trace.Ray( start, end )
								.Size( this.mins, this.maxs )
								.HitLayer( CollisionLayer.All, false )
								.HitLayer( CollisionLayer.LADDER, true )
								.Ignore( this.Pawn )
								.Run();

			this.IsTouchingLadder = false;

			if ( pm.Hit )
			{
				this.IsTouchingLadder = true;
				this.LadderNormal = pm.Normal;
			}
		}

		public virtual void LadderMove()
		{
			Vector3 velocity = this.WishVelocity;
			float normalDot = velocity.Dot( this.LadderNormal );
			Vector3 cross = this.LadderNormal * normalDot;
			this.Velocity = velocity - cross + (-normalDot * this.LadderNormal.Cross( Vector3.Up.Cross( this.LadderNormal ).Normal ));

			this.Move();
		}

		public virtual void CategorizePosition( bool bStayOnGround )
		{
			this.SurfaceFriction = 1.0f;

			// Doing this before we move may introduce a potential latency in water detection, but
			// doing it after can get us stuck on the bottom in water if the amount we move up
			// is less than the 1 pixel 'threshold' we're about to snap to.	Also, we'll call
			// this several times per frame, so we really need to avoid sticking to the bottom of
			// water on each call, and the converse case will correct itself if called twice.
			//CheckWater();

			Vector3 point = this.Position - (Vector3.Up * 2);
			Vector3 vBumpOrigin = this.Position;

			//
			//  Shooting up really fast.  Definitely not on ground trimed until ladder shit
			//
			bool bMovingUpRapidly = this.Velocity.z > this.MaxNonJumpVelocity;
			bool bMovingUp = this.Velocity.z > 0;

			bool bMoveToEndPos = false;

			if ( this.GroundEntity != null ) // and not underwater
			{
				bMoveToEndPos = true;
				point.z -= this.StepSize;
			}
			else if ( bStayOnGround )
			{
				bMoveToEndPos = true;
				point.z -= this.StepSize;
			}

			if ( bMovingUpRapidly || this.Swimming ) // or ladder and moving up
			{
				this.ClearGroundEntity();
				return;
			}

			TraceResult pm = this.TraceBBox( vBumpOrigin, point, 4.0f );

			if ( pm.Entity == null || Vector3.GetAngle( Vector3.Up, pm.Normal ) > this.GroundAngle )
			{
				this.ClearGroundEntity();
				bMoveToEndPos = false;

				if ( this.Velocity.z > 0 )
				{
					this.SurfaceFriction = 0.25f;
				}
			}
			else
			{
				this.UpdateGroundEntity( pm );
			}

			if ( bMoveToEndPos && !pm.StartedSolid && pm.Fraction > 0.0f && pm.Fraction < 1.0f )
			{
				this.Position = pm.EndPos;
			}
		}

		/// <summary>
		/// We have a new ground entity
		/// </summary>
		public virtual void UpdateGroundEntity( TraceResult tr )
		{
			this.GroundNormal = tr.Normal;

			// VALVE HACKHACK: Scale this to fudge the relationship between vphysics friction values and player friction values.
			// A value of 0.8f feels pretty normal for vphysics, whereas 1.0f is normal for players.
			// This scaling trivially makes them equivalent.  REVISIT if this affects low friction surfaces too much.
			this.SurfaceFriction = tr.Surface.Friction * 1.25f;
			if ( this.SurfaceFriction > 1 )
			{
				this.SurfaceFriction = 1;
			}

			//if ( tr.Entity == GroundEntity ) return;

			Vector3 oldGroundVelocity = default;
			if ( this.GroundEntity != null )
			{
				oldGroundVelocity = this.GroundEntity.Velocity;
			}

			bool wasOffGround = this.GroundEntity == null;

			this.GroundEntity = tr.Entity;

			if ( this.GroundEntity != null )
			{
				this.BaseVelocity = this.GroundEntity.Velocity;
			}

			/*
					  m_vecGroundUp = pm.m_vHitNormal;
					  player->m_surfaceProps = pm.m_pSurfaceProperties->GetNameHash();
					  player->m_pSurfaceData = pm.m_pSurfaceProperties;
					  const CPhysSurfaceProperties *pProp = pm.m_pSurfaceProperties;

					  const CGameSurfaceProperties *pGameProps = g_pPhysicsQuery->GetGameSurfaceproperties( pProp );
					  player->m_chTextureType = (int8)pGameProps->m_nLegacyGameMaterial;
				 */
		}

		/// <summary>
		/// We're no longer on the ground, remove it
		/// </summary>
		public virtual void ClearGroundEntity()
		{
			if ( this.GroundEntity == null )
			{
				return;
			}

			this.GroundEntity = null;
			this.GroundNormal = Vector3.Up;
			this.SurfaceFriction = 1.0f;
		}

		/// <summary>
		/// Traces the current bbox and returns the result.
		/// liftFeet will move the start position up by this amount, while keeping the top of the bbox at the same
		/// position. This is good when tracing down because you won't be tracing through the ceiling above.
		/// </summary>
		public override TraceResult TraceBBox( Vector3 start, Vector3 end, float liftFeet = 0.0f )
		{
			return this.TraceBBox( start, end, this.mins, this.maxs, liftFeet );
		}

		/// <summary>
		/// Try to keep a walking player on the ground when running down slopes etc
		/// </summary>
		public virtual void StayOnGround()
		{
			Vector3 start = this.Position + (Vector3.Up * 2);
			Vector3 end = this.Position + (Vector3.Down * this.StepSize);

			// See how far up we can go without getting stuck
			TraceResult trace = this.TraceBBox( this.Position, start );
			start = trace.EndPos;

			// Now trace down from a known safe position
			trace = this.TraceBBox( start, end );

			if ( trace.Fraction <= 0 )
			{
				return;
			}

			if ( trace.Fraction >= 1 )
			{
				return;
			}

			if ( trace.StartedSolid )
			{
				return;
			}

			if ( Vector3.GetAngle( Vector3.Up, trace.Normal ) > this.GroundAngle )
			{
				return;
			}

			// This is incredibly hacky. The real problem is that trace returning that strange value we can't network over.
			// float flDelta = fabs( mv->GetAbsOrigin().z - trace.m_vEndPos.z );
			// if ( flDelta > 0.5f * DIST_EPSILON )

			this.Position = trace.EndPos;
		}

		void RestoreGroundPos()
		{
			if ( this.GroundEntity == null || this.GroundEntity.IsWorld )
			{
				return;
			}

			//var Position = GroundEntity.Transform.ToWorld( GroundTransform );
			//Pos = Position.Position;
		}

		void SaveGroundPos()
		{
			if ( this.GroundEntity == null || this.GroundEntity.IsWorld )
			{
				return;
			}

			//GroundTransform = GroundEntity.Transform.ToLocal( new Transform( Pos, Rot ) );
		}
	}
}
