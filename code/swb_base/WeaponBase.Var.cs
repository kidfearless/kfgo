using Sandbox;

using System;
using System.Collections.Generic;

namespace SWB_Base
{
	public enum HoldType
	{
		Pistol = 1,
		Rifle = 2,
		Shotgun = 3
	}
	public partial class WeaponBase
	{
		// Virtual
		[Net] public int Bucket { get; set; } = 1; // Inventory slot position
		[Net]	public int BucketWeight { get; set; } = 100; // Inventory slot position weight ( higher = more important )
		[Net]	public bool CanDrop { get; set; } = true; // Can manually drop weapon

		[Net] public bool DropWeaponOnDeath { get; set; } = true; // Drop the weapon on death
		[Net] public bool BulletCocking { get; set; } = true; // Can bullets be cocked in the barrel? ( clip ammo + 1 )
		[Net] public bool BarrelSmoking { get; set; } = true; // Should the barrel smoke after heavy weapon usage?
		[Net] public string FreezeViewModelOnZoom { get; set; } = null; // Some weapons have looping idle animations -> force spam another animation to "freeze" it
		[Net] public int FOV { get; set; } = 65; // Default FOV
		[Net] public int ZoomFOV { get; set; } = 65; // FOV while zooming
		[Net] public float TuckRange { get; set; } = 30; // Range that tucking should be enabled (set to -1 to disable tucking)
		[Net] public HoldType HoldType { get; set; } = HoldType.Pistol; // Thirdperson holdtype

		[Net] public string ViewModel { get; set; }
		public override string ViewModelPath => ViewModel;


		[Net] public string WorldModelPath { get; set; } = "weapons/rust_pistol/rust_pistol.vmdl"; // Path to the world model
		[Net] public string Icon { get; set; } = ""; // Path to an image that represent the weapon on the HUD
		[Net] public float WalkAnimationSpeedMod { get; set; } = 1; // Procedural animation speed ( lower is slower )
		[Net] public float AimSensitivity { get; set; } = 0.85f; // Aim sensitivity while zooming ( lower is slower )
		[Net] public bool DualWield { get; set; } = false; // If the weapon should be dual wielded
		[Net] public float PrimaryDelay { get; set; } = -1; // Delay before firing when the primary attack button is pressed
		[Net] public float SecondaryDelay { get; set; } = -1; // Delay before firing when the secondary attack button is pressed

		/*[Net]*/ public AngPos ZoomAnimData { get; set; } // Data used for setting the weapon to its zoom position
		/*[Net]*/ public AngPos RunAnimData { get; set; } // Data used for setting the weapon to its run position

		//public override string ViewModelPath => this.ViewModelPath;

		// Properties
		public string PrintName { get { return this.ClassInfo.Title; } }

		public List<AnimatedAction> AnimatedActions { get; set; } // Extra actions that use certain key combinations to trigger animations

		public UISettings UISettings { get; set; } = new UISettings();

		[Net]
		public ClipInfo Primary { get; set; } = new ClipInfo(); // Primary attack data

		[Net]
		public ClipInfo Secondary { get; set; } = null; // Secondary attack data ( setting this will disable weapon zooming )

		public TimeSince TimeSincePrimaryAttack { get; set; }

		public TimeSince TimeSinceSecondaryAttack { get; set; }

		public TimeSince TimeSinceReload { get; set; }

		public TimeSince TimeSinceDeployed { get; set; }

		public bool IsReloading { get; set; }

		public bool IsZooming { get; set; }

		public bool IsScoped { get; set; }

		public bool IsRunning { get; set; }

		public bool IsAnimating { get; set; }

		public PickupTrigger PickupTrigger { get; protected set; }

		protected bool DoRecoil { get; set; } = false;
		protected int BarrelHeat { get; set; } = 0;
		protected TimeSince TimeSinceFired { get; set; }
		protected BaseViewModel DualWieldViewModel { get; set; }
		protected bool IsDualWieldConverted { get; set; } = false;
		protected bool DualWieldLeftFire { get; set; } = false;
		protected bool DualWieldShouldReload { get; set; } = false;
	}
}
