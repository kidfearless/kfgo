using System;
using Sandbox;

namespace SWB_Base
{
	class ViewModelBase : BaseViewModel
	{
		private WeaponBase _Weapon;

		private bool _IsDualWieldVM = false;

		private float _AnimSpeed;

		// Target animation values
		private Vector3 _TargetVectorPos;
		private Vector3 _TargetVectorRot;
		private float _TargetFOV;

		// Finalized animation values
		private Vector3 _FinalVectorPos;
		private Vector3 _FinalVectorRot;
		private float _FinalFOV;

		// Sway
		private Rotation _LastEyeRot;

		// Jumping Animation
		private float _JumpTime;
		private float _LandTime;

		// Helpful values
		private Vector3 _LocalVel;

		// Enable this to help calculate weapon vector postions & angles
		// Be sure to switch weapons after changing this value
		private bool _LiveEditing = false;

		public ViewModelBase(WeaponBase weapon, bool isDualWieldVM = false)
		{
			this._Weapon = weapon;
			this._IsDualWieldVM = isDualWieldVM;
		}

		public override void PostCameraSetup(ref CameraSetup camSetup)
		{
			base.PostCameraSetup(ref camSetup);
			this.FieldOfView = this._Weapon.FOV;
			this.Rotation = camSetup.Rotation;
			this.Position = camSetup.Position;
			if ( this._Weapon.IsDormant)
			{
				return;
			}

			if ( this.Owner != null && this.Owner.Health <= 0)
			{
				this.EnableDrawing = false;
				return;
			}

			// Smoothly transition the vectors with the target values
			this._FinalVectorPos = this._FinalVectorPos.LerpTo( this._TargetVectorPos, this._AnimSpeed * RealTime.Delta);
			this._FinalVectorRot = this._FinalVectorRot.LerpTo( this._TargetVectorRot, this._AnimSpeed * RealTime.Delta);
			this._FinalFOV = MathX.LerpTo( this._FinalFOV, this._TargetFOV, this._AnimSpeed * RealTime.Delta);
			this._AnimSpeed = 10 * this._Weapon.WalkAnimationSpeedMod;

			// Change the angles and positions of the viewmodel with the new vectors
			this.Rotation *= Rotation.From( this._FinalVectorRot.x, this._FinalVectorRot.y, this._FinalVectorRot.z);
			this.Position += (this._FinalVectorPos.z * this.Rotation.Up) + (this._FinalVectorPos.y * this.Rotation.Forward) + (this._FinalVectorPos.x * this.Rotation.Right);
			this.FieldOfView = this._FinalFOV;

			// I'm sure there's something already that does this for me, but I spend an hour
			// searching through the wiki and a bunch of other garbage and couldn't find anything...
			// So I'm doing it manually. Problem solved.
			this._LocalVel = new Vector3( this.Owner.Rotation.Right.Dot( this.Owner.Velocity), this.Owner.Rotation.Forward.Dot( this.Owner.Velocity), this.Owner.Velocity.z);

			// Initialize the target vectors for this frame
			this._TargetVectorPos = new Vector3(0.0f, 0.0f, 0.0f);
			this._TargetVectorRot = new Vector3(0.0f, 0.0f, 0.0f);
			this._TargetFOV = this._Weapon.FOV;

			// Live editing
			if ( this._LiveEditing && this._Weapon.RunAnimData != null)
			{
				// Zooming
				this._TargetVectorRot = MathUtil.ToVector3(new Angles(-0.21f, -0.05f, 0));
				this._TargetVectorPos = new Vector3(-2.317f, -3f, 1.56f);
				this._TargetFOV = this._Weapon.ZoomFOV;
				return;

				// Running
				//weapon.RunAnimData.Angle = new Angles(10, 40, 0);
				//weapon.RunAnimData.Pos = new Vector3(5, 0, 0);
			}

			// Flip the Viewmodel
			if ( this._IsDualWieldVM )
			{
				this.FlipViewModel(ref camSetup);
			}

			// Tucking
			if ( this._Weapon.RunAnimData != null && this._Weapon.ShouldTuck(out float tuckDist))
			{
				float animationCompletion = Math.Min(1, ((this._Weapon.TuckRange - tuckDist) / this._Weapon.TuckRange) + 0.5f);
				this._TargetVectorPos = this._Weapon.RunAnimData.Pos * animationCompletion;
				this._TargetVectorRot = MathUtil.ToVector3( this._Weapon.RunAnimData.Angle * animationCompletion);
				return;
			}

			// Handle different animations
			this.HandleIdleAnimation(ref camSetup);
			this.HandleWalkAnimation(ref camSetup);
			this.HandleSwayAnimation(ref camSetup);
			this.HandleIronAnimation(ref camSetup);
			this.HandleSprintAnimation(ref camSetup);
			this.HandleJumpAnimation(ref camSetup);
		}

		private void FlipViewModel(ref CameraSetup camSetup)
		{
			// Waiting for https://github.com/Facepunch/sbox-issues/issues/324

			// Temp solution: 
			Vector3 posOffset = Vector3.Zero;
			posOffset -= camSetup.Rotation.Right * 10.0f;
			this.Position += posOffset;
		}

		private void HandleIdleAnimation(ref CameraSetup camSetup)
		{
			// No swaying if aiming
			if ( this._Weapon.IsZooming)
			{
				return;
			}

			// Perform a "breathing" animation
			float breatheTime = RealTime.Now * 2.0f;
			this._TargetVectorPos -= new Vector3(MathF.Cos(breatheTime / 4.0f) / 8.0f, 0.0f, -MathF.Cos(breatheTime / 4.0f) / 32.0f);
			this._TargetVectorRot -= new Vector3(MathF.Cos(breatheTime / 5.0f), MathF.Cos(breatheTime / 4.0f), MathF.Cos(breatheTime / 7.0f));

			// Crouching animation
			if (Input.Down(InputButton.Duck))
			{
				this._TargetVectorPos += new Vector3(-1.0f, -1.0f, 0.5f);
			}
		}

		private void HandleWalkAnimation(ref CameraSetup camSetup)
		{
			float breatheTime = RealTime.Now * 16.0f;
			float walkSpeed = new Vector3( this.Owner.Velocity.x, this.Owner.Velocity.y, 0.0f).Length;
			float maxWalkSpeed = 200.0f;
			float roll = 0.0f;
			float yaw = 0.0f;

			// Check if on the ground
			if ( this.Owner.GroundEntity == null)
			{
				return;
			}

			// Check if sprinting
			if ( this._Weapon.IsRunning)
			{
				breatheTime = RealTime.Now * 18.0f;
				maxWalkSpeed = 100.0f;
			}

			// Check for sideways velocity to sway the gun slightly
			if ( this._Weapon.IsZooming || this._LocalVel.x > 0.0f)
			{
				roll = -7.0f * (this._LocalVel.x / maxWalkSpeed);
			}
			else if ( this._LocalVel.x < 0.0f)
			{
				yaw = 3.0f * (this._LocalVel.x / maxWalkSpeed);
			}

			// Perform walk cycle
			this._TargetVectorPos -= new Vector3((-MathF.Cos(breatheTime / 2.0f) / 5.0f * walkSpeed / maxWalkSpeed) - (yaw / 4.0f), 0.0f, 0.0f);
			this._TargetVectorRot -= new Vector3(Math.Clamp(MathF.Cos(breatheTime), -0.3f, 0.3f) * 2.0f * walkSpeed / maxWalkSpeed, (-MathF.Cos(breatheTime / 2.0f) * 1.2f * walkSpeed / maxWalkSpeed) - (yaw * 1.5f), roll);
		}

		private void HandleSwayAnimation(ref CameraSetup camSetup)
		{
			int swayspeed = 5;

			// Fix the sway faster if we're ironsighting
			if ( this._Weapon.IsZooming && this._Weapon.ZoomAnimData != null)
			{
				swayspeed = 20;
			}

			// Lerp the eye position
			this._LastEyeRot = Rotation.Lerp( this._LastEyeRot, this.Owner.EyeRot, swayspeed * RealTime.Delta);

			// Calculate the difference between our current eye angles and old (lerped) eye angles
			Angles angDif = this.Owner.EyeRot.Angles() - this._LastEyeRot.Angles();
			angDif = new Angles(angDif.pitch, MathX.RadianToDegree(MathF.Atan2(MathF.Sin(MathX.DegreeToRadian(angDif.yaw)), MathF.Cos(MathX.DegreeToRadian(angDif.yaw)))), 0);

			// Perform sway
			this._TargetVectorPos += new Vector3(Math.Clamp(angDif.yaw * 0.04f, -1.5f, 1.5f), 0.0f, Math.Clamp(angDif.pitch * 0.04f, -1.5f, 1.5f));
			this._TargetVectorRot += new Vector3(Math.Clamp(angDif.pitch * 0.2f, -4.0f, 4.0f), Math.Clamp(angDif.yaw * 0.2f, -4.0f, 4.0f), 0.0f);
		}

		private void HandleIronAnimation(ref CameraSetup camSetup)
		{
			if ( this._Weapon.IsZooming && this._Weapon.ZoomAnimData != null)
			{
				this._AnimSpeed = 10 * this._Weapon.WalkAnimationSpeedMod;
				this._TargetVectorPos += this._Weapon.ZoomAnimData.Pos;
				this._TargetVectorRot += new Vector3( this._Weapon.ZoomAnimData.Angle.pitch, this._Weapon.ZoomAnimData.Angle.yaw, this._Weapon.ZoomAnimData.Angle.roll);
				this._TargetFOV = this._Weapon.ZoomFOV;
			}
		}

		private void HandleSprintAnimation(ref CameraSetup camSetup)
		{
			if ( this._Weapon.IsRunning && this._Weapon.RunAnimData != null)
			{
				this._TargetVectorPos += this._Weapon.RunAnimData.Pos;
				this._TargetVectorRot += new Vector3( this._Weapon.RunAnimData.Angle.pitch, this._Weapon.RunAnimData.Angle.yaw, this._Weapon.RunAnimData.Angle.roll);
			}
		}

		private void HandleJumpAnimation(ref CameraSetup camSetup)
		{
			// If we're not on the ground, reset the landing animation time
			if ( this.Owner.GroundEntity == null)
			{
				this._LandTime = RealTime.Now + 0.31f;
			}

			// Reset the timers once they elapse
			if ( this._LandTime < RealTime.Now && this._LandTime != 0.0f)
			{
				this._LandTime = 0.0f;
				this._JumpTime = 0.0f;
			}

			// If we jumped, start the animation
			if (Input.Down(InputButton.Jump) && this._JumpTime == 0.0f)
			{
				this._JumpTime = RealTime.Now + 0.31f;
				this._LandTime = 0.0f;
			}

			// If we're not ironsighting, do a fancy jump animation
			if (!this._Weapon.IsZooming)
			{
				if ( this._JumpTime > RealTime.Now)
				{
					// fuck you for naming them this way
					// If we jumped, do a curve upwards
					float f = 0.31f - (this._JumpTime - RealTime.Now);
					float xx = MathUtil.BezierY(f, 0.0f, -4.0f, 0.0f);
					float yy = 0.0f;
					float zz = MathUtil.BezierY(f, 0.0f, -2.0f, -5.0f);
					float pt = MathUtil.BezierY(f, 0.0f, -4.36f, 10.0f);
					float yw = xx;
					float rl = MathUtil.BezierY(f, 0.0f, -10.82f, -5.0f);
					this._TargetVectorPos += new Vector3(xx, yy, zz) / 4.0f;
					this._TargetVectorRot += new Vector3(pt, yw, rl) / 4.0f;
					this._AnimSpeed = 20.0f;
				}
				else if ( this.Owner.GroundEntity == null)
				{
					// Shaking while falling
					float breatheTime = RealTime.Now * 30.0f;
					this._TargetVectorPos += new Vector3(MathF.Cos(breatheTime / 2.0f) / 16.0f, 0.0f, -5.0f + (MathF.Sin(breatheTime / 3.0f) / 16.0f)) / 4.0f;
					this._TargetVectorRot += new Vector3(10.0f - (MathF.Sin(breatheTime / 3.0f) / 4.0f), MathF.Cos(breatheTime / 2.0f) / 4.0f, -5.0f) / 4.0f;
					this._AnimSpeed = 20.0f;
				}
				else if ( this._LandTime > RealTime.Now)
				{
					// fuck you for naming them this way
					// If we landed, do a fancy curve downwards
					float f = this._LandTime - RealTime.Now;
					float xx = MathUtil.BezierY(f, 0.0f, -4.0f, 0.0f);
					float yy = 0.0f;
					float zz = MathUtil.BezierY(f, 0.0f, -2.0f, -5.0f);
					float pt = MathUtil.BezierY(f, 0.0f, -4.36f, 10.0f);
					float yw = xx;
					float rl = MathUtil.BezierY(f, 0.0f, -10.82f, -5.0f);
					this._TargetVectorPos += new Vector3(xx, yy, zz) / 2.0f;
					this._TargetVectorRot += new Vector3(pt, yw, rl) / 2.0f;
					this._AnimSpeed = 20.0f;
				}
			}
			else
			{
				this._TargetVectorPos += new Vector3(0.0f, 0.0f, Math.Clamp( this._LocalVel.z / 1000.0f, -1.0f, 1.0f));
			}
		}
	}
}
