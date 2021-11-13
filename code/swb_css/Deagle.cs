using Sandbox;

using SWB_Base;

namespace SWB_CSS
{
	[Library( "swb_css_deagle", Title = "Desert Eagle" )]
	public class Deagle : WeaponBase
	{
		public Deagle()
		{
			Bucket = 1;
			HoldType = HoldType.Pistol;
			ViewModel = "weapons/swb/css/deagle/css_v_pist_deagle.vmdl";
			WorldModelPath = "weapons/swb/css/deagle/css_w_pist_deagle.vmdl";
			Icon = "/swb_css/textures/ui/css_icon_deagle.png";
			FOV = 75;
			ZoomFOV = 60;

			this.Primary = new ClipInfo
			{
				Ammo = 7,
				AmmoType = AmmoType.Revolver,
				ClipSize = 7,
				ReloadTime = 2.17f,

				BulletSize = 6f,
				Damage = 50f,
				Force = 5f,
				Spread = 0.06f,
				Recoil = 1f,
				RPM = 300,
				FiringType = FiringType.SemiAutomatic,
				ScreenShake = new ScreenShake
				{
					Length = 0.5f,
					Speed = 4.0f,
					Size = 1.0f,
					Rotation = 0.5f
				},

				DryFireSound = "swb_pistol.empty",
				ShootSound = "css_deagle.fire",

				BulletEjectParticle = "particles/pistol_ejectbrass.vpcf",
				MuzzleFlashParticle = "particles/swb/muzzle/flash_medium.vpcf",

				InfiniteAmmo = InfiniteAmmoType.Reserve
			};

			this.ZoomAnimData = new AngPos
			{
				Angle = new Angles( 0, -0.1f, 0 ),
				Pos = new Vector3( -5.125f, 0, 2.67f )
			};

			this.RunAnimData = new AngPos
			{
				Angle = new Angles( -40, 0, 0 ),
				Pos = new Vector3( 0, -3, -8 )
			};
		}
	}
}
