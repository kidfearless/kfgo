using System;

namespace SWB_Base
{
	public interface IWeaponData
	{
		int Bucket { get; set; }
		int BucketWeight { get; set; }
		bool CanDrop { get; set; }
		bool DropWeaponOnDeath { get; set; }
		bool BulletCocking { get; set; }
		bool BarrelSmoking { get; set; }
		string FreezeViewModelOnZoom { get; set; }
		int FOV { get; set; }
		int ZoomFOV { get; set; }
		float TuckRange { get; set; }
		HoldType HoldType { get; set; }
		string ViewModel { get; set; }
		string WorldModelPath { get; set; }
		string Icon { get; set; }
		float WalkAnimationSpeedMod { get; set; }
		float AimSensitivity { get; set; }
		bool DualWield { get; set; }
		float PrimaryDelay { get; set; }
		float SecondaryDelay { get; set; }
		AngPos ZoomAnimData { get; set; }
		AngPos RunAnimData { get; set; }
	}
}
