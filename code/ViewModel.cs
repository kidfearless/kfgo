using Sandbox;

namespace KFGO
{
	public class ViewModel : BaseViewModel
	{
		protected float SwingInfluence => 0.025f;
		protected float ReturnSpeed => 5.0f;
		protected float MaxOffsetLength => 10.0f;
		protected float BobCycleTime => 14;
		protected Vector3 BobDirection => new( 0.0f, 0.1f, 0.2f );

		private Vector3 _SwingOffset;
		private float _LastPitch;
		private float _LastYaw;
		private float _BobAnim;

		private bool _Activated;

		private Vector3 _ViewmodelOffset => new( -15f, 10f, 10f );

		private Vector3 _ShootOffset { get; set; }

		public void OnFire()
		{
			using System.IDisposable _ = Prediction.Off();
			this._ShootOffset -= Vector3.Backward * 35;
		}

		public override void PostCameraSetup( ref CameraSetup camSetup )
		{
			base.PostCameraSetup( ref camSetup );

			this.EnableDrawing = true;

			if ( !Local.Pawn.IsValid() )
			{
				return;
			}

			if ( !this._Activated )
			{
				this._LastPitch = camSetup.Rotation.Pitch();
				this._LastYaw = camSetup.Rotation.Yaw();

				this._Activated = true;
			}

			this.Position = camSetup.Position;
			this.Rotation = camSetup.Rotation;

			camSetup.ViewModel.FieldOfView = 65;

			float newPitch = this.Rotation.Pitch();
			float newYaw = this.Rotation.Yaw();

			float pitchDelta = Angles.NormalizeAngle( newPitch - this._LastPitch );
			float yawDelta = Angles.NormalizeAngle( this._LastYaw - newYaw );

			Vector3 playerVelocity = Local.Pawn.Velocity;
			float verticalDelta = playerVelocity.z * Time.Delta;
			Vector3 viewDown = Rotation.FromPitch( newPitch ).Up * -1.0f;
			verticalDelta *= 1.0f - System.MathF.Abs( viewDown.Cross( Vector3.Down ).y );
			pitchDelta -= verticalDelta * 1;

			Vector3 offset = this.CalcSwingOffset( pitchDelta, yawDelta );
			offset += this.CalcBobbingOffset( playerVelocity );

			offset -= this._ViewmodelOffset + this._ShootOffset;
			offset = offset.WithY( offset.y );

			this.Position += this.Rotation * offset;

			this._LastPitch = newPitch;
			this._LastYaw = newYaw;

			this._ShootOffset = this._ShootOffset.LerpTo( Vector3.Zero, 5 * Time.Delta );
		}

		protected Vector3 CalcSwingOffset( float pitchDelta, float yawDelta )
		{
			Vector3 swingVelocity = new Vector3( 0, yawDelta, pitchDelta );

			this._SwingOffset -= this._SwingOffset * this.ReturnSpeed * Time.Delta;
			this._SwingOffset += swingVelocity * this.SwingInfluence;

			if ( this._SwingOffset.Length > this.MaxOffsetLength )
			{
				this._SwingOffset = this._SwingOffset.Normal * this.MaxOffsetLength;
			}

			return this._SwingOffset;
		}

		protected Vector3 CalcBobbingOffset( Vector3 velocity )
		{
			float halfPI = System.MathF.PI * 0.5f;
			float twoPI = System.MathF.PI * 2.0f;

			if ( this.Owner.GroundEntity != null )
			{
				this._BobAnim += Time.Delta * this.BobCycleTime;
			}
			else
			{
				// In air - return to center
				if ( this._BobAnim > halfPI + 0.1f )
				{
					this._BobAnim -= Time.Delta * this.BobCycleTime * 0.05f;
				}
				else if ( this._BobAnim < halfPI + 0.1f )
				{
					this._BobAnim += Time.Delta * this.BobCycleTime * 0.05f;
				}
				else
				{
					this._BobAnim = halfPI;
				}
			}

			if ( this._BobAnim > twoPI )
			{
				this._BobAnim -= twoPI;
			}

			float speed = new Vector2( velocity.x, velocity.y ).Length;
			speed = speed > 10.0 ? speed : 0.0f;
			Vector3 offset = this.BobDirection * (speed * 0.005f) * System.MathF.Cos( this._BobAnim );
			offset = offset.WithZ( -System.MathF.Abs( offset.z ) );

			return offset;
		}
	}
}
