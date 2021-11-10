using KFGO;

using Sandbox;

using SWB_Base;

using SWB_CSS;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KFGO
{

	public partial class GameItems
	{
		public BaseWeapon[] BaseWeapons { get; set; }
		public Clip[] Clips { get; set; }
		public WeaponAttribute[] WeaponAttributes { get; set; }
	}

	public partial class BaseWeapon
	{
		public string Name { get; set; }
		public string PrimaryClip { get; set; }
		public string SecondaryClip { get; set; }
		public string Attributes { get; set; }
	}

	public struct Clip : IClipInfo
	{
		public string Name { get; set; }
		public string Inherits { get; set; }
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
		public string ShootAnim { get; set; }
		public string ReloadAnim { get; set; }
		public string ReloadEmptyAnim { get; set; }
		public string DrawAnim { get; set; }
		public string DrawEmptyAnim { get; set; }
		public string DryFireSound { get; set; }
		public string ShootSound { get; set; }
		public string BulletEjectParticle { get; set; }
		public string MuzzleFlashParticle { get; set; }
		public string BarrelSmokeParticle { get; set; }
		public string BulletTracerParticle { get; set; }
		public InfiniteAmmoType InfiniteAmmo { get; set; }
	}

	public partial class WeaponAttribute
	{
		public string Name { get; set; }
		public string Inherits { get; set; }
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

		public override void Write( Utf8JsonWriter writer, T value, JsonSerializerOptions options )
		{
			throw new NotImplementedException();
		}
	}




	public class WeaponDataParser
	{
		protected const string PATH = "config/items_game.json";
		protected GameItems Entries { get; set; }

		protected Dictionary<string, Clip> Clips { get; set; } = new();
		protected Dictionary<string, WeaponAttribute> WeaponAttributes { get; set; } = new();
		protected Dictionary<string, BaseWeapon> BaseWeapons { get; set; } = new();

		//public Dictionary<string, ItemEntry> WeaponData { get; set; }


		public WeaponDataParser()
		{
			//WeaponData = new Dictionary<string, ItemEntry>(32);
		}

		private static GameItems ParseFile()
		{
			string text = FileSystem.Mounted.ReadAllText( PATH );
			if ( string.IsNullOrWhiteSpace( text ) )
			{
				return default( GameItems );
			}

			JsonSerializerOptions options = new JsonSerializerOptions()
			{
				ReadCommentHandling = JsonCommentHandling.Skip,
				PropertyNameCaseInsensitive = true,
				AllowTrailingCommas = true,
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

		public void Parse()
		{
			Log.Info( "Parsing weapon data" );
			if ( !FileSystem.Mounted.FileExists( PATH ) )
			{
				throw new InvalidOperationException( $"Could not find items_game.json in kfgo/config/" );
			}

			var json = ParseFile();
			this.Entries = json;


			foreach ( var item in json.Clips )
			{
				this.Clips[item.Name] = item;
			}

			foreach ( var item in json.WeaponAttributes )
			{
				this.WeaponAttributes[item.Name] = item;
			}

			foreach ( var item in json.BaseWeapons )
			{
				this.BaseWeapons[item.Name] = item;
			}
		}


		protected static T Clone<T>( T input ) where T : class, new()
		{
			if ( input is null )
			{
				return new T();
			}
			T result = new();
			var proper = (typeof( T )).GetProperties();

			foreach ( var prop in proper )
			{
				try
				{
					var val = prop.GetValue( input );
					prop.SetValue( result, val );

				}
				catch ( Exception e )
				{
				}
			}

			return result;
		}

		public SWB_Base.WeaponBase CreateWeaponByName( string weaponname )
		{
			return CreateWeaponByName<WeaponBase>( weaponname );
		}

		/// <summary>
		/// Don't use this as typeof() is fairly slow. Just use nameof() instead. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="weaponname"></param>
		/// <returns></returns>
		//public T CreateWeaponByName<T>() where T : WeaponBase, new()
		//{
		//	return CreateWeaponByName<T>( typeof(T).Name );
		//}

		public T CreateWeaponByName<T>( string weaponname ) where T : WeaponBase, new()
		{
			if ( !this.BaseWeapons.TryGetValue( weaponname, out var weapon ) )
			{
				Log.Trace( $"Tried to create unknown weapon: {weaponname}" );
				Log.Warning( $"Tried to create unknown weapon: {weaponname}" );
				return new T();
			}

			var primary = GetClip( weapon.Name, weapon.PrimaryClip );
			//var attributes = GetAttributes( weapon );

			var result = new T()
			{
				Primary = new( primary )
			};

			var clip = new Clip()
			{
				Name = "AK47_Primary_Clip",
				Ammo = 30,
				AmmoType = AmmoType.Rifle,
				ClipSize = 30,
				ReloadTime = 2.17f,
				BulletSize = 4,
				Damage = 15,
				Force = 3,
				Spread = 0.1f,
				Recoil = 0.5f,
				RPM = 600,
				FiringType = FiringType.Automatic,
				ScreenShake = new()
				{
					Length = 0.5f,
					Speed = 4.0f,
					Size = 0.5f,
					Rotation = 0.5f
				},
				DryFireSound = "swb_rifle.empty",
				ShootSound = "css_ak47.fire",
				BulletEjectParticle = "particles/pistol_ejectbrass.vpcf",
				MuzzleFlashParticle = "particles/swb/muzzle/flash_medium.vpcf",
				InfiniteAmmo = InfiniteAmmoType.Reserve
			};

			Assert.AreEqual( primary, clip );


			return result;
		}



		private WeaponAttribute GetAttributes( BaseWeapon weapon )
		{
			if ( string.IsNullOrEmpty( weapon.Attributes ) )
			{
				return new WeaponAttribute();
			}

			if ( this.WeaponAttributes.TryGetValue( weapon.Attributes, out var primary ) )
			{
				return primary;
			}

			Log.Trace( $"Could not find attribute '{weapon.Attributes}' for weapon '{weapon.Name}'" );
			Log.Warning( $"Could not find attribute '{weapon.Attributes}' for weapon '{weapon.Name}'" );
			return new WeaponAttribute();

		}

		private Clip GetClip( string weaponname, string clipname )
		{
			if ( string.IsNullOrEmpty( clipname ) )
			{
				return new Clip();
			}

			if ( this.Clips.TryGetValue( clipname, out var primary ) )
			{
				return primary;
			}

			Log.Trace( $"Could not find clip '{clipname}' for weapon '{weaponname}'" );
			Log.Warning( $"Could not find clip '{clipname}' for weapon '{weaponname}'" );

			return new Clip();
		}
	}
}
