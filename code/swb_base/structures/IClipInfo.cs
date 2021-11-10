using Sandbox;

namespace SWB_Base
{
	public interface IClipInfo
	{

		int Ammo { get; set; }
		AmmoType AmmoType { get; set; }
		int ClipSize { get; set; }
		float ReloadTime { get; set; }
		float ReloadEmptyTime { get; set; }
		// Shooting

		int Bullets { get; set; }
		float BulletSize { get; set; }
		float Damage { get; set; }
		float Force { get; set; }
		float Spread { get; set; }
		float Recoil { get; set; }

		int RPM { get; set; }
		FiringType FiringType { get; set; }

		ScreenShake ScreenShake { get; set; }
		// Strings
		string ShootAnim { get; set; }
		string ReloadAnim { get; set; }
		string ReloadEmptyAnim { get; set; }
		string DrawAnim { get; set; }
		string DrawEmptyAnim { get; set; }
		string DryFireSound { get; set; } // Firing sound when clip is empty

		string ShootSound { get; set; } // Firing sound

		string BulletEjectParticle { get; set; } // Particle that should be used for bullet ejection

		string MuzzleFlashParticle { get; set; } // Particle that should be used for the muzzle flash

		string BarrelSmokeParticle { get; set; }
		string BulletTracerParticle { get; set; }
		// Extra
		InfiniteAmmoType InfiniteAmmo { get; set; }
	}
}
