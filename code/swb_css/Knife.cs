//using Sandbox;

//using SWB_Base;

//namespace SWB_CSS
//{
//	[Library( "swb_css_knife", Title = "Knife" )]
//	public class Knife : WeaponBaseMelee
//	{
//		public override string SwingAnimationHit => "swing";
//		public override string SwingAnimationMiss => "swing_miss";
//		public override string StabAnimationHit => "stab";
//		public override string StabAnimationMiss => "stab_miss";
//		public override string SwingSound => "css_knife.hit";
//		public override string StabSound => "css_knife.stab";
//		public override string MissSound => "css_knife.slash";
//		public override string HitWorldSound => "css_knife.hitwall";
//		public override float SwingSpeed => 0.6f;
//		public override float StabSpeed => 1.29f;
//		public override float SwingDamage => 25f;
//		public override float StabDamage => 100f;
//		public override float SwingForce => 25f;
//		public override float StabForce => 50f;
//		public override float DamageDistance => 35f;
//		public override float ImpactSize => 10f;

//		public Knife()
//		{
//			Bucket = 0;
//			HoldType = HoldType.Pistol;
//			ViewModel = "weapons/swb/css/knife/css_v_knife.vmdl";
//			WorldModelPath = "weapons/swb/css/knife/css_w_knife.vmdl";
//			Icon = "/swb_css/textures/ui/css_icon_knife.png";
//			FOV = 75;

//			this.UISettings = new UISettings
//			{
//				ShowCrosshairLines = false,
//				ShowHitmarker = false,
//				ShowAmmoCount = false,
//				ShowFireMode = false
//			};
//		}
//	}
//}
