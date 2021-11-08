using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

public partial class DamageIndicator : Panel
{
    public static DamageIndicator Current;

    public DamageIndicator()
    {
        Current = this;
		this.StyleSheet.Load( "swb_base/deathmatch_dep/ui/scss/DamageIndicator.scss" );
    }

    public void OnHit( Vector3 pos )
    {
		HitPoint p = new HitPoint( pos );
        p.Parent = this;
    }

    public class HitPoint : Panel
    {
        public Vector3 Position;

        public HitPoint( Vector3 pos )
        {
			this.Position = pos;

            _ = this.Lifetime();
        }

        public override void Tick()
        {
            base.Tick();

			Vector3 wpos = CurrentView.Rotation.Inverse * (this.Position.WithZ( 0 ) - CurrentView.Position.WithZ( 0 )).Normal;
            wpos = wpos.WithZ( 0 ).Normal;

			float angle = MathF.Atan2( wpos.y, -1.0f * wpos.x );

			PanelTransform pt = new PanelTransform();

            pt.AddTranslateX( Length.Percent( -50.0f ) );
            pt.AddTranslateY( Length.Percent( -50.0f ) );
            pt.AddRotation( 0, 0, angle.RadianToDegree() );

			this.Style.Transform = pt;
			this.Style.Dirty();

        }

        async Task Lifetime()
        {
            await this.Task.Delay( 200 );
			this.AddClass( "dying" );
            await this.Task.Delay( 500 );
			this.Delete();
        }
    }
}

