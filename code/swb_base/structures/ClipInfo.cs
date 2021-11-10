using Sandbox;
using Sandbox.UI;

using SWB_Base;

using System.Runtime.InteropServices;

namespace SWB_Base
{
	public enum FiringType
	{
		SemiAutomatic,
		Automatic,
		BurstFire
	}


	public struct ValueClipInfo
	{
		public int Ammo { get; set; }
		public AmmoType AmmoType { get; set; }
		public int ClipSize { get; set; }
		public float ReloadTime { get; set; }
		public float ReloadEmptyTime { get; set; }
		public int Bullets { get; set; }
		public float BulletSize { get; set; }
		public float Damage { get; set; }
		public float Force { get; set; }
		public float Spread { get; set; }
		public float Recoil { get; set; }
		public int RPM { get; set; }
		public FiringType FiringType { get; set; }
		public ScreenShake ScreenShake { get; set; }
		//public string ShootAnim { get; set; }
		//public string ReloadAnim { get; set; }
		//public string ReloadEmptyAnim { get; set; }
		//public string DrawAnim { get; set; }
		//public string DrawEmptyAnim { get; set; }
		//public string DryFireSound { get; set; }
		//public string ShootSound { get; set; }
		//public string BulletEjectParticle { get; set; }
		//public string MuzzleFlashParticle { get; set; }
		//public string BarrelSmokeParticle { get; set; }
		//public string BulletTracerParticle { get; set; }
		public InfiniteAmmoType InfiniteAmmo { get; set; }

		public static ValueClipInfo Create() => new()
		{
			Ammo = 10, // Amount of ammo in the clip
			AmmoType = AmmoType.Pistol, // Type of ammo
			ClipSize = 10, // Size of the clip
			ReloadTime = 1f, // Duration of the reload animation
			ReloadEmptyTime = -1f, // Duration of the empty reload animation
			Bullets = 1, // Amount of bullets per shot
			BulletSize = 0.1f, // Bullet size
			Damage = 5, // Bullet damage
			Force = 0.1f, // Bullet force
			Spread = 0.1f, // Weapon spread
			Recoil = 0.1f, // Weapon recoil
			RPM = 200, // Firing speed ( higher is faster )
			FiringType = FiringType.SemiAutomatic, // Firing type
			ScreenShake = default, // Screenshake per shot
			//ShootAnim = "fire", // Shooting animation
			//ReloadAnim = "reload", // Reloading animation
			//ReloadEmptyAnim = "reload_empty", // Reloading animation when clip is empty
			//DrawAnim = "deploy", // Draw animation
			//DrawEmptyAnim = "", // Draw animation when there is no ammo
			//DryFireSound = default, // Firing sound when clip is empty
			//ShootSound = default, // Firing sound
			//BulletEjectParticle = default,  // Particle that should be used for bullet ejection
			//MuzzleFlashParticle = default,  // Particle that should be used for the muzzle flash
			//BarrelSmokeParticle = "particles/swb/muzzle/barrel_smoke.vpcf", // Particle that should be used for the barrel smoke
			//BulletTracerParticle = "particles/swb/tracer/tracer_medium.vpcf", // Particle that should be used for the barrel smoke
			InfiniteAmmo = InfiniteAmmoType.Normal, // If the weapon should have infinite ammo
		};
	}

	public partial class ClipInfo : BaseNetworkable
	{
		public ClipInfo()
		{

		}

		public ClipInfo(ValueClipInfo value)
		{
			this.ClipInfoValue = value;
		}

		public ValueClipInfo Info;

		[Net]
		public ValueClipInfo ClipInfoValue { get => Info; set => Info = value; }



		[Net]
		public int Ammo { get; set; } = 10; // Amount of ammo in the clip
		[Net]
		public AmmoType AmmoType { get; set; } = AmmoType.Pistol; // Type of ammo
		[Net]
		public int ClipSize { get; set; } = 10; // Size of the clip
		[Net]
		public float ReloadTime { get; set; } = 1f; // Duration of the reload animation
		[Net]
		public float ReloadEmptyTime { get; set; } = -1f; // Duration of the empty reload animation

		// Shooting
		[Net]
		public int Bullets { get; set; } = 1; // Amount of bullets per shot
		[Net]
		public float BulletSize { get; set; } = 0.1f; // Bullet size
		[Net]
		public float Damage { get; set; } = 5; // Bullet damage
		[Net]
		public float Force { get; set; } = 0.1f; // Bullet force
		[Net]
		public float Spread { get; set; } = 0.1f; // Weapon spread
		[Net]
		public float Recoil { get; set; } = 0.1f; // Weapon recoil

		[Net]
		public int RPM { get; set; } = 200; // Firing speed ( higher is faster )
		[Net]
		public FiringType FiringType { get; set; } = FiringType.SemiAutomatic; // Firing type

		private ScreenShake _ScreenShake;
		[Net]
		public ScreenShake ScreenShake { get => _ScreenShake; set => _ScreenShake = value; } // Screenshake per shot

		public ref ScreenShake GetScreenShake() => ref _ScreenShake;

		// Strings
		[Net]
		public string ShootAnim { get; set; } = "fire"; // Shooting animation
		[Net]
		public string ReloadAnim { get; set; } = "reload"; // Reloading animation
		[Net]
		public string ReloadEmptyAnim { get; set; } = "reload_empty"; // Reloading animation when clip is empty
		[Net]
		public string DrawAnim { get; set; } = "deploy"; // Draw animation
		[Net]
		public string DrawEmptyAnim { get; set; } = ""; // Draw animation when there is no ammo
		[Net]
		public string DryFireSound { get; set; } // Firing sound when clip is empty
		[Net]
		public string ShootSound { get; set; } // Firing sound
		[Net]
		public string BulletEjectParticle { get; set; } // Particle that should be used for bullet ejection
		[Net]
		public string MuzzleFlashParticle { get; set; } // Particle that should be used for the muzzle flash
		[Net]
		public string BarrelSmokeParticle { get; set; } = "particles/swb/muzzle/barrel_smoke.vpcf"; // Particle that should be used for the barrel smoke
		[Net]
		public string BulletTracerParticle { get; set; } = "particles/swb/tracer/tracer_medium.vpcf"; // Particle that should be used for the barrel smoke

		// Extra
		[Net]
		public InfiniteAmmoType InfiniteAmmo { get; set; } = InfiniteAmmoType.Normal; // If the weapon should have infinite ammo
	}
}
