
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

/*
 * Very experimental, NOT ready for use yet
*/

/* Layla removed SceneCapture.Create
 * Remove bullshit SceneCapture, replaced with Render.DrawScene and UI.Scene control
 * TODO: find out if a proper renderscope can be made now!
*/

namespace SWB_Base
{

	public class SniperScopeRT:Panel
	{
		protected Image ScopeRT { get; set; }
		protected Texture RTTexture { get; set; }
		protected Texture ColorTexture { get; set; }
		protected Texture DepthTexture { get; set; }
		protected ScenePanel scene { get; set; }

		public SniperScopeRT( string lensTexture, string scopeTexture )
		{
			this.StyleSheet.Load( "/swb_base/ui/SniperScopeRT.scss" );

			SceneWorld.SetCurrent( SceneWorld.Current );
			//sceneCapture = SceneCapture.Create( "worldTestScene", 500, 500 );
			this.ScopeRT = this.Add.Image( "scene:worldTestScene" );

			// TESTING
			this.ColorTexture = Texture.CreateRenderTarget().WithSize( 500, 500 ).WithScreenFormat()
									.WithScreenMultiSample()
									.Create();

			this.DepthTexture = Texture.CreateRenderTarget().WithSize( 500, 500 ).WithDepthFormat()
									.WithScreenMultiSample()
									.Create();
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			//sceneCapture?.Delete();
			//sceneCapture = null;
		}

		[Event( "frame" )]
		public void OnFrame()
		{

		}

		public override void Tick()
		{
			base.Tick();

			Entity player = Local.Pawn;
			if ( player == null )
			{
				return;
			}

			if ( player.ActiveChild is not WeaponBaseSniper weapon )
			{
				return;
			}

			// Update render camera
			Vector3 targetPos = CurrentView.Position;
			Angles targetAng = CurrentView.Rotation.Angles();

			// sceneCapture.SetCamera( TargetPos, TargetAng, weapon.ZoomAmount );

			// RenderTarget on a panel
			Transform scopeBone = weapon.ViewModelEntity.GetBoneTransform( "v_weapon_awm_bolt_action" );
			Vector3 screenpos = scopeBone.Position.ToScreen();

			if ( screenpos.z < 0 )
			{
				return;
			}

			this.Style.Left = Length.Fraction( screenpos.x );
			this.Style.Top = Length.Fraction( screenpos.y );
			this.Style.Dirty();

			// RenderTarget inside a material
			SceneObject sceneObject = weapon.ViewModelEntity.SceneObject;
			//RTTexture = Texture.Load("scene:worldTestScene", false);

			// TESTING
			//Render.SetRenderTarget(RTTexture);
			//Render.DrawScene(Texture.White, DepthTexture, new Vector2(500, 500), SceneWorld.Current, targetPos, targetAng, weapon.ZoomAmount);

			//sceneObject.SetValue("ScopeRT", RTTexture);
		}

		public override void DrawBackground( ref RenderState state )
		{
			Entity player = Local.Pawn;
			if ( player == null )
			{
				return;
			}

			if ( player.ActiveChild is not WeaponBaseSniper weapon )
			{
				return;
			}

			Vector3 targetPos = CurrentView.Position;
			Angles targetAng = CurrentView.Rotation.Angles();
			SceneObject sceneObject = weapon.ViewModelEntity.SceneObject;

			Render.SetRenderTarget( this.RTTexture );
			Render.DrawScene( this.ColorTexture, this.DepthTexture, new Vector2( 500, 500 ), SceneWorld.Current, targetPos, targetAng, weapon.ZoomAmount );

			sceneObject.SetValue( "ScopeRT", this.RTTexture );
		}
	}
}
