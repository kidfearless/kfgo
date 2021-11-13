// using Sandbox;

// using SWB_Base;

// namespace SWB_CSS
// {
// 	[Library( "swb_css_super90", Title = "M3 Super 90" )]
// 	public class Super90 : WeaponBaseShotty
// 	{


// 		public Super90()
// 		{
// 			Bucket = 2;
// 			HoldType = HoldType.Shotgun;
// 			ViewModelPath = "weapons/swb/css/super90/css_v_shot_m3super90.vmdl";
// 			WorldModelPath = "weapons/swb/css/super90/css_w_shot_m3super90.vmdl";
// 			Icon = "/swb_css/textures/ui/css_icon_super90.png";
// 			FOV = 75;
// 			ZoomFOV = 45;
// 			WalkAnimationSpeedMod = 0.9f;
// 			ShellReloadTimeStart = 0.38f;
// 			ShellReloadTimeInsert = 0.49f;
// 			this.Primary = new ClipInfo
// 			{
// 				Ammo = 8,
// 				AmmoType = AmmoType.Shotgun,
// 				ClipSize = 8,

// 				Bullets = 8,
// 				BulletSize = 2f,
// 				Damage = 15f,
// 				Force = 5f,
// 				Spread = 0.3f,
// 				Recoil = 2f,
// 				RPM = 80,
// 				FiringType = FiringType.SemiAutomatic,
// 				ScreenShake = new ScreenShake
// 				{
// 					Length = 0.5f,
// 					Speed = 4.0f,
// 					Size = 1.0f,
// 					Rotation = 0.5f
// 				},

// 				DryFireSound = "swb_shotty.empty",
// 				ShootSound = "css_super90.fire",

// 				BulletEjectParticle = "particles/pistol_ejectbrass.vpcf",
// 				MuzzleFlashParticle = "particles/swb/muzzle/flash_medium.vpcf",

// 				InfiniteAmmo = InfiniteAmmoType.Reserve
// 			};

// 			this.ZoomAnimData = new AngPos
// 			{
// 				Angle = new Angles( 0.1f, -0.07f, -0.5f ),
// 				Pos = new Vector3( -5.76f, 6, 3.3f )
// 			};

// 			this.RunAnimData = new AngPos
// 			{
// 				Angle = new Angles( 10, 50, 0 ),
// 				Pos = new Vector3( 5, 2, 0 )
// 			};

// 		}
// 	}
// }
