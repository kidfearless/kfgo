using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SWB_Base
{

	public class SniperScope : Panel
	{
		Panel LensWrapper;
		Panel ScopeWrapper;

		Panel LeftBar;
		Panel RightBar;
		Panel TopBar;
		Panel BottomBar;

		Image Lens;
		Image Scope;

		float lensRotation;

		public SniperScope( string lensTexture, string scopeTexture )
		{
			this.StyleSheet.Load( "/swb_base/ui/SniperScope.scss" );

			this.LensWrapper = this.Add.Panel( "lensWrapper" );
			this.Lens = this.LensWrapper.Add.Image( lensTexture, "lens" );

			if ( scopeTexture != null )
			{
				this.Scope = this.LensWrapper.Add.Image( scopeTexture, "scope" );

				this.LeftBar = this.Add.Panel( "leftBar" );
				this.RightBar = this.Add.Panel( "rightBar" );
				this.TopBar = this.Add.Panel( "topBar" );
				this.BottomBar = this.Add.Panel( "bottomBar" );
			}
		}

		public override void Tick()
		{
			base.Tick();

			Entity player = Local.Pawn;
			if ( player == null )
			{
				return;
			}

			// Show when zooming
			this.Style.Opacity = (player.ActiveChild is not WeaponBase weapon || !weapon.IsScoped) ? 0 : 1;

			/*
			// Movement impact
			var velocity = player.Velocity;
			var velocityMove = (velocity.y + velocity.x) / 2;
			var lensBob = 0f;

			if (velocityMove != 0)
			{
				 lensBob += MathF.Sin(RealTime.Now * 20f) * 2f;
			}

			this.Style.MarginTop = Length.Percent((velocity.z * 0.05f) + lensBob);

			var targetRotation = 0f;

			if (Input.Left != 0)
			{
				 targetRotation = Input.Left * -5f;
			}

			var rotateTransform = new PanelTransform();
			lensRotation = MathUtil.FILerp(lensRotation, targetRotation, 5);
			rotateTransform.AddRotation(0, 0, lensRotation);

			this.Style.Transform = rotateTransform;
			*/

			this.Style.Dirty();
		}
	}
}
