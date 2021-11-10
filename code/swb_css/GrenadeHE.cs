using Sandbox;

using SWB_Base;

using System;
using System.Collections.Generic;

namespace SWB_CSS
{
	[Library( "swb_css_grenade_he", Title = "HE Grenade" )]
	public class GrenadeHE : WeaponBaseEntity
	{
		public override int Bucket => 0;
		public override HoldType HoldType => HoldType.Pistol;
		public override string ViewModelPath => "weapons/swb/css/grenade_he/css_v_grenade_he.vmdl";
		public override string WorldModelPath => "weapons/swb/css/grenade_he/css_w_grenade_he.vmdl";
		public override string Icon => "/swb_css/textures/ui/css_icon_grenade.png";
		public override int FOV => 75;

		public override Func<ClipInfo, bool, FiredEntity> CreateEntity => this.CreateGrenadeEntity;
		public override string EntityModel => "weapons/swb/css/grenade_he/css_w_grenade_he_thrown.vmdl";
		public override Vector3 EntityVelocity => new Vector3( 0, 25, 50 );
		public override Angles EntityAngles => new Angles( 0, 0, -45 );
		public override bool IsSticky => true;
		public override float PrimaryEntitySpeed => 17;
		public override float SecondaryEntitySpeed => 10;
		public override float PrimaryDelay => 1.27f;
		public override float SecondaryDelay => 1.27f;

		public GrenadeHE()
		{
			this.UISettings = new UISettings
			{
				ShowCrosshairLines = false,
				ShowFireMode = false,
			};

			this.Primary = new ClipInfo
			{
				Ammo = -1,
				ClipSize = -1,
				AmmoType = AmmoType.Grenade,
				FiringType = FiringType.SemiAutomatic,
				RPM = 50,
			};
			this.Secondary = this.Primary;
		}

		private FiredEntity CreateGrenadeEntity( ClipInfo clipInfo, bool isPrimary )
		{
			Grenade grenade = new Grenade
			{
				Weapon = this,
				ExplosionDelay = 3f,
				ExplosionRadius = 300f,
				ExplosionDamage = 200f,
				ExplosionForce = 350f,
				BounceSound = "css_grenade_he.bounce",
				ExplosionSounds = new List<string>
				{
					 "css_grenade_he.explode"
				},
				ExplosionEffect = "weapons/swb/css/grenade_he/particles/grenade_he_explosion.vpcf",
				ExplosionShake = new ScreenShake
				{
					Length = 1f,
					Speed = 5f,
					Size = 5f,
					Rotation = 2f,
				}
			};

			return grenade;
		}
	}
}
