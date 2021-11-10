using KFGO;

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

	public partial class ClipInfo : BaseNetworkable, IClipInfo
	{
		public ClipInfo()
		{

		}

		public ClipInfo( IClipInfo clip )
		{
			this.Ammo = clip.Ammo;
			this.AmmoType = clip.AmmoType;
			this.ClipSize = clip.ClipSize;
			this.ReloadTime = clip.ReloadTime;
			this.ReloadEmptyTime = clip.ReloadEmptyTime;
			this.Bullets = clip.Bullets;
			this.BulletSize = clip.BulletSize;
			this.Damage = clip.Damage;
			this.Force = clip.Force;
			this.Spread = clip.Spread;
			this.Recoil = clip.Recoil;
			this.RPM = clip.RPM;
			this.FiringType = clip.FiringType;
			this.ScreenShake = clip.ScreenShake;
			this.ShootAnim = clip.ShootAnim;
			this.ReloadAnim = clip.ReloadAnim;
			this.ReloadEmptyAnim = clip.ReloadEmptyAnim;
			this.DrawAnim = clip.DrawAnim;
			this.DrawEmptyAnim = clip.DrawEmptyAnim;
			this.DryFireSound = clip.DryFireSound;
			this.ShootSound = clip.ShootSound;
			this.BulletEjectParticle = clip.BulletEjectParticle;
			this.MuzzleFlashParticle = clip.MuzzleFlashParticle;
			this.BarrelSmokeParticle = clip.BarrelSmokeParticle;
			this.BulletTracerParticle = clip.BulletTracerParticle;
			this.InfiniteAmmo = clip.InfiniteAmmo;

		}

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
