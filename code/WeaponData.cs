using KFGO;

using Sandbox;
using Sandbox.UI;

using SWB_Base;


using System;
using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace KFGO
{

	public struct GameItems
	{
		public BaseWeapon[] BaseWeapons;
		public Clip[] Clips;
		public WeaponAttribute[] WeaponAttributes;
	}

	public struct BaseWeapon
	{
		public string Name;
		public string PrimaryClip;
		public string SecondaryClip;
		public string Attributes;
	}

	public struct Clip
	{
		public string Name;
		public string Inherits;
		public int? Ammo;
		public AmmoType? AmmoType;
		public int? ClipSize;
		public float? ReloadTime;
		public float? ReloadEmptyTime;
		public int? Bullets;
		public float? BulletSize;
		public float? Damage;
		public float? Force;
		public float? Spread;
		public float? Recoil;
		public int? RPM;
		public FiringType? FiringType;
		public ScreenShake? ScreenShake;
		public string ShootAnim;
		public string ReloadAnim;
		public string ReloadEmptyAnim;
		public string DrawAnim;
		public string DrawEmptyAnim;
		public string DryFireSound;
		public string ShootSound;
		public string BulletEjectParticle;
		public string MuzzleFlashParticle;
		public string BarrelSmokeParticle;
		public string BulletTracerParticle;
		public InfiniteAmmoType? InfiniteAmmo;

		public void CopyTo( ref Clip to )
		{
			if ( this.Name != null ) to.Name = this.Name;
			if ( this.Inherits != null ) to.Inherits = this.Inherits;
			if ( this.Ammo != null ) to.Ammo = this.Ammo.Value;
			if ( this.AmmoType != null ) to.AmmoType = this.AmmoType.Value;
			if ( this.ClipSize != null ) to.ClipSize = this.ClipSize.Value;
			if ( this.ReloadTime != null ) to.ReloadTime = this.ReloadTime.Value;
			if ( this.ReloadEmptyTime != null ) to.ReloadEmptyTime = this.ReloadEmptyTime.Value;
			if ( this.Bullets != null ) to.Bullets = this.Bullets.Value;
			if ( this.BulletSize != null ) to.BulletSize = this.BulletSize.Value;
			if ( this.Damage != null ) to.Damage = this.Damage.Value;
			if ( this.Force != null ) to.Force = this.Force.Value;
			if ( this.Spread != null ) to.Spread = this.Spread.Value;
			if ( this.Recoil != null ) to.Recoil = this.Recoil.Value;
			if ( this.RPM != null ) to.RPM = this.RPM.Value;
			if ( this.FiringType != null ) to.FiringType = this.FiringType.Value;
			if ( this.ScreenShake != null ) to.ScreenShake = this.ScreenShake.Value;
			if ( this.ShootAnim != null ) to.ShootAnim = this.ShootAnim;
			if ( this.ReloadAnim != null ) to.ReloadAnim = this.ReloadAnim;
			if ( this.ReloadEmptyAnim != null ) to.ReloadEmptyAnim = this.ReloadEmptyAnim;
			if ( this.DrawAnim != null ) to.DrawAnim = this.DrawAnim;
			if ( this.DrawEmptyAnim != null ) to.DrawEmptyAnim = this.DrawEmptyAnim;
			if ( this.DryFireSound != null ) to.DryFireSound = this.DryFireSound;
			if ( this.ShootSound != null ) to.ShootSound = this.ShootSound;
			if ( this.BulletEjectParticle != null ) to.BulletEjectParticle = this.BulletEjectParticle;
			if ( this.MuzzleFlashParticle != null ) to.MuzzleFlashParticle = this.MuzzleFlashParticle;
			if ( this.BarrelSmokeParticle != null ) to.BarrelSmokeParticle = this.BarrelSmokeParticle;
			if ( this.BulletTracerParticle != null ) to.BulletTracerParticle = this.BulletTracerParticle;
			if ( this.InfiniteAmmo != null ) to.InfiniteAmmo = this.InfiniteAmmo.Value;

		}
	}

	public struct WeaponAttribute
	{
		public string Name;
		public string Inherits;
		public int? Bucket;
		public int? BucketWeight;
		public bool? CanDrop;
		public bool? DropWeaponOnDeath;
		public bool? BulletCocking;
		public bool? BarrelSmoking;
		public string FreezeViewModelOnZoom;
		public int? FOV;
		public int? ZoomFOV;
		public float? TuckRange;
		public HoldType? HoldType;
		public string ViewModel;
		public string WorldModelPath;
		public string Icon;
		public float? WalkAnimationSpeedMod;
		public float? AimSensitivity;
		public bool? DualWield;
		public float? PrimaryDelay;
		public float? SecondaryDelay;
		public AngPos ZoomAnimData;
		public AngPos RunAnimData;

		public void CopyTo( ref WeaponAttribute to )
		{
			if ( this.Name != null ) to.Name = this.Name;
			if ( this.Inherits != null ) to.Inherits = this.Inherits;
			if ( this.Bucket != null ) to.Bucket = this.Bucket.Value;
			if ( this.BucketWeight != null ) to.BucketWeight = this.BucketWeight.Value;
			if ( this.CanDrop != null ) to.CanDrop = this.CanDrop.Value;
			if ( this.DropWeaponOnDeath != null ) to.DropWeaponOnDeath = this.DropWeaponOnDeath.Value;
			if ( this.BulletCocking != null ) to.BulletCocking = this.BulletCocking.Value;
			if ( this.BarrelSmoking != null ) to.BarrelSmoking = this.BarrelSmoking.Value;
			if ( this.FreezeViewModelOnZoom != null ) to.FreezeViewModelOnZoom = this.FreezeViewModelOnZoom;
			if ( this.FOV != null ) to.FOV = this.FOV.Value;
			if ( this.ZoomFOV != null ) to.ZoomFOV = this.ZoomFOV.Value;
			if ( this.TuckRange != null ) to.TuckRange = this.TuckRange.Value;
			if ( this.HoldType != null ) to.HoldType = this.HoldType.Value;
			if ( this.ViewModel != null ) to.ViewModel = this.ViewModel;
			if ( this.WorldModelPath != null ) to.WorldModelPath = this.WorldModelPath;
			if ( this.Icon != null ) to.Icon = this.Icon;
			if ( this.WalkAnimationSpeedMod != null ) to.WalkAnimationSpeedMod = this.WalkAnimationSpeedMod.Value;
			if ( this.AimSensitivity != null ) to.AimSensitivity = this.AimSensitivity.Value;
			if ( this.DualWield != null ) to.DualWield = this.DualWield.Value;
			if ( this.PrimaryDelay != null ) to.PrimaryDelay = this.PrimaryDelay.Value;
			if ( this.SecondaryDelay != null ) to.SecondaryDelay = this.SecondaryDelay.Value;
			if ( this.ZoomAnimData != null ) to.ZoomAnimData = this.ZoomAnimData;
			if ( this.RunAnimData != null ) to.RunAnimData = this.RunAnimData;
		}
	}

	public class EnumConverter<T> : JsonConverter<T> where T : notnull, Enum
	{
		public T Default { get; set; } = default( T );


		public override bool CanConvert( Type typeToConvert ) => typeToConvert == typeof( T );

		public override T Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
		{
			string value = reader.GetString();
			if ( String.IsNullOrEmpty( value ) )
			{
				return default( T );
			}

			// generic freaks out and says it's nullable when it isn't
			if ( Enum.TryParse( typeof( T ), value, out object result ) )
			{
				return (T)result;
			}

			return default( T );
		}

		public override void Write( Utf8JsonWriter writer, T value, JsonSerializerOptions options ) => throw new NotImplementedException();
	}

	public struct WeaponData
	{
		private const string PATH = "config/items_game.json";
		private static GameItems Entries { get; set; }
		private static Dictionary<string, Clip> Clips { get; set; }
		private static Dictionary<string, WeaponAttribute> WeaponAttributes { get; set; }
		private static Dictionary<string, BaseWeapon> BaseWeapons { get; set; }


		public string Name;
		public Clip Primary;
		public Clip? Secondary;
		public WeaponAttribute Attributes;

		static WeaponData() => Init();

		public static void Init()
		{
			if ( Clips is null )
			{
				WeaponAttributes = new( 32 );
				BaseWeapons = new( 32 );
				Clips = new( 32 );
				return;
			}

			Clips.Clear();
			BaseWeapons.Clear();
			WeaponAttributes.Clear();
		}

		private static GameItems ParseFile()
		{
			string text = FileSystem.Mounted.ReadAllText( PATH );
			if ( string.IsNullOrWhiteSpace( text ) )
			{
				return default( GameItems );
			}

			var options = new JsonSerializerOptions()
			{
				ReadCommentHandling = JsonCommentHandling.Skip,
				PropertyNameCaseInsensitive = true,
				AllowTrailingCommas = true,
				IncludeFields = true,
				Converters =
				{
					new EnumConverter<AmmoType>(){ Default = AmmoType.Rifle },
					new EnumConverter<FiringType>(){ Default = FiringType.Automatic},
					new EnumConverter<InfiniteAmmoType>(){ Default = InfiniteAmmoType.Clip},
					new EnumConverter<HoldType>(){ Default = HoldType.Rifle},
				}
			};

			return JsonSerializer.Deserialize<GameItems>( text, options );
		}

		public static void Parse()
		{
			KLog.Info( "Parsing weapon data" );
			if ( !FileSystem.Mounted.FileExists( PATH ) )
			{
				throw new InvalidOperationException( $"Could not find items_game.json in kfgo/config/" );
			}

			var json = ParseFile();


			foreach ( var item in json.Clips )
			{
				Clips[item.Name] = item;
			}

			foreach ( var item in json.WeaponAttributes )
			{
				WeaponAttributes[item.Name] = item;
			}

			foreach ( var item in json.BaseWeapons )
			{
				BaseWeapons[item.Name] = item;
			}
		}

		public static WeaponData GetByName( string weaponname )
		{
			KLog.Info( "Creating weapon" );

			if ( !BaseWeapons.TryGetValue( weaponname, out var weapon ) )
			{
				Log.Trace( $"Tried to create unknown weapon: {weaponname}" );
				Log.Warning( $"Tried to create unknown weapon: {weaponname}" );
				return new();
			}

			WeaponData result = new()
			{
				Name = weaponname,
				Primary = GetClip( weapon.Name, weapon.PrimaryClip ),
				Attributes = GetAttributes( weapon ),
				Secondary = null
			};

			if ( weapon.SecondaryClip != null )
			{
				result.Secondary = GetClip( weapon.Name, weapon.SecondaryClip );
			}


			return result;
		}

		private static WeaponAttribute GetAttributes( BaseWeapon weapon )
		{
			if ( string.IsNullOrEmpty( weapon.Attributes ) || !WeaponAttributes.TryGetValue( weapon.Attributes, out var primary ) )
			{
				Log.Trace( $"Could not find attribute '{weapon.Attributes}' for weapon '{weapon.Name}'" );
				Log.Warning( $"Could not find attribute '{weapon.Attributes}' for weapon '{weapon.Name}'" );
				return new WeaponAttribute();
			}

			Stack<WeaponAttribute> attributes = new();
			attributes.Push( primary );

			while(WeaponAttributes.TryGetValue(primary.Inherits??"", out var next))
			{
				attributes.Push( next );
				primary = next;
			}

			WeaponAttribute result = new WeaponAttribute();
			while (attributes.TryPop(out var attr))
			{
				attr.CopyTo(ref result);
			}

			return result;
		}

		private static Clip GetClip( string weaponname, string clipname )
		{
			if ( string.IsNullOrEmpty( clipname ) )
			{
				return new Clip();
			}

			if ( !Clips.TryGetValue( clipname ?? "", out var primary ) )
			{
				Log.Trace( $"Could not find clip '{clipname}' for weapon '{weaponname}'" );
				Log.Warning( $"Could not find clip '{clipname}' for weapon '{weaponname}'" );
				return new Clip();
			}

			Stack<Clip> clips = new();
			clips.Push( primary );

			while ( Clips.TryGetValue( primary.Inherits ?? "", out var baseclip ) )
			{
				clips.Push( baseclip );
				primary = baseclip;
			}

			Clip result = new();
			while ( clips.TryPop(out var clip))
			{
				clip.CopyTo( ref result );
			}

			return result;
		}


		[ServerCmd]
		public static void KFGO_SV_PrintWeaponData()
		{
			Log.Info( Host.IsClient ? "Client" : "Server" );
			Log.Info( JsonSerializer.Serialize( Entries, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true } ) );
		}

		[ClientCmd]
		public static void KFGO_CL_PrintWeaponData()
		{
			Log.Info( Host.IsClient ? "Client" : "Server" );
			Log.Info( JsonSerializer.Serialize( Entries, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true } ) );
		}
	}
}
