﻿using Sandbox;
using Sandbox.UI;

/* 
 * Weapon base for sniper based zooming
*/

namespace SWB_Base
{
    public partial class WeaponBaseSniper : WeaponBase
    {
        public virtual string LensTexture => "/materials/swb/scopes/swb_lens_hunter.png"; // Path to the lens texture
        public virtual string ScopeTexture => "/materials/swb/scopes/swb_scope_hunter.png"; // Path to the scope texture
        public virtual string ZoomInSound => "swb_sniper.zoom_in"; // Sound to play when zooming in
        public virtual string ZoomOutSound => ""; // Sound to play when zooming out
        public virtual float ZoomAmount => 20f; // The amount to zoom in ( lower is more )
        public virtual bool UseRenderTarget => false; // EXPERIMENTAL - Use a render target instead of a full screen texture zoom

        private Panel SniperScopePanel;
        private bool switchBackToThirdP = false;
        private float lerpZoomAmount = 0;
        private float oldSpread = -1;

        public override void ActiveStart(Entity ent)
        {
            base.ActiveStart(ent);
        }

        public override void ActiveEnd(Entity ent, bool dropped)
        {
            base.ActiveEnd(ent, dropped);

			this.SniperScopePanel?.Delete();
        }

        public virtual void OnScopedStart()
        {
			this.IsScoped = true;

            if ( this.oldSpread == -1)
            {
				this.oldSpread = this.Primary.Spread;
            }

			this.Primary.Spread = 0;

            if ( this.IsServer )
            {
                if ( this.Owner.Camera is ThirdPersonCamera)
                {
					this.switchBackToThirdP = true;
					this.Owner.Camera = new FirstPersonCamera();
                }
            }

            if ( this.IsLocalPawn )
            {
				this.ViewModelEntity.RenderColor = Color.Transparent;

                if (!string.IsNullOrEmpty( this.ZoomInSound ))
                {
					this.PlaySound( this.ZoomInSound );
                }
            }
        }

        public virtual void OnScopedEnd()
        {
			this.IsScoped = false;
			this.Primary.Spread = this.oldSpread;
			this.lerpZoomAmount = 0;

            if ( this.IsServer && this.switchBackToThirdP )
            {
				this.switchBackToThirdP = false;
				this.Owner.Camera = new ThirdPersonCamera();
            }

            if ( this.IsLocalPawn )
            {
				this.ViewModelEntity.RenderColor = Color.White;

                if (!string.IsNullOrEmpty( this.ZoomOutSound ))
                {
					this.PlaySound( this.ZoomOutSound );
                }
            }
        }

        public override void Simulate(Client owner)
        {
            base.Simulate(owner);
			bool shouldTuck = this.ShouldTuck();

            if (((Input.Pressed(InputButton.Attack2) && !this.IsReloading && !this.IsRunning) || (this.IsZooming && !this.IsScoped)) && !shouldTuck)
            {
				this.OnScopedStart();
            }

            if (Input.Released(InputButton.Attack2) || (this.IsScoped && (this.IsRunning || shouldTuck)))
            {
				this.OnScopedEnd();
            }
        }
        public override void CreateHudElements()
        {
            base.CreateHudElements();

            if (Local.Hud == null)
            {
                return;
            }

            if ( this.UseRenderTarget )
            {
				this.SniperScopePanel = new SniperScopeRT( this.LensTexture, this.ScopeTexture );
				this.SniperScopePanel.Parent = Local.Hud;
            }
            else
            {
				this.SniperScopePanel = new SniperScope( this.LensTexture, this.ScopeTexture );
				this.SniperScopePanel.Parent = Local.Hud;
            }
        }

        public override void PostCameraSetup(ref CameraSetup camSetup)
        {
            base.PostCameraSetup(ref camSetup);

            if ( this.IsScoped )
            {
                if ( this.lerpZoomAmount == 0)
                {
					this.lerpZoomAmount = camSetup.FieldOfView;
                }

				this.lerpZoomAmount = MathUtil.FILerp( this.lerpZoomAmount, this.ZoomAmount, 10f);
                camSetup.FieldOfView = this.lerpZoomAmount;
            }
        }
    }
}
