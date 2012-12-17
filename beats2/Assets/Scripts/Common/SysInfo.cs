using UnityEngine; // Keep for SystemLanguage, may try to abstract away later
using System;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;

/*
 * DONE
 */
namespace Beats2.Common {
	
	/// <summary>
	/// System info. Wraps Unity's SystemInfo class.
	/// </summary>
	public static class SysInfo {
		private const string TAG = "SysInfo";
		
		public enum DeviceTypes {
			DESKTOP,
			CONSOLE,
			WEB,
			TABLET,
			PHONE
		}

		public enum Platforms {
			DESKTOP_WINDOWS,
			DESKTOP_OSX,
			DESKTOP_LINUX,
			CONSOLE_WII,
			CONSOLE_XBOX,
			CONSOLE_PS3,
			WEB_BROWSER,
			WEB_GOOGLE,
			WEB_FLASH,
			DEVICE_ANDROID,
			DEVICE_IOS,
			DEVICE_WINPHONE
		}

		public static string appName			{ get; private set; }
		public static string appVersionNum		{ get; private set; }
		public static string appVersionName		{ get; private set; }

		public static string deviceName			{ get; private set; }
		public static string deviceModel		{ get; private set; }
		public static string deviceId			{ get; private set; }

		public static Platforms platform		{ get; private set; }
		public static DeviceTypes deviceType	{ get; private set; }
		public static string operatingSystem	{ get; private set; }
		public static string unityVersion		{ get; private set; }

		public static bool vibrationSupport		{ get; private set; }
		public static bool touchSupport			{ get; private set; }

		public static SystemLanguage language	{ get; private set; }
		
		public static void Init() {
			appName = Constants.APP_NAME;
			appVersionNum = Constants.APP_VERSION_NUM;
			appVersionName = Constants.APP_VERSION_NAME;

			deviceName = UnityEngine.SystemInfo.deviceName;
			deviceModel = UnityEngine.SystemInfo.deviceModel;
			// FIXME: deviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;

			SetPlatformAndDeviceType();
			operatingSystem = UnityEngine.SystemInfo.operatingSystem;
			unityVersion = UnityEngine.Application.unityVersion;

			vibrationSupport = UnityEngine.SystemInfo.supportsVibration;
			touchSupport = UnityEngine.Input.multiTouchEnabled;

			Reset();
			Logger.Debug(TAG, "Initialized...");
			Logger.Debug(TAG, PrintInfo());
		}
		
		public static void Reset() {
			language = UnityEngine.Application.systemLanguage;
			Logger.Debug(TAG, "Reset...");
		}

		private static void SetPlatformAndDeviceType() {
			switch (UnityEngine.Application.platform) {
				case UnityEngine.RuntimePlatform.OSXEditor:
				case UnityEngine.RuntimePlatform.OSXPlayer:
				case UnityEngine.RuntimePlatform.OSXDashboardPlayer:
					platform = Platforms.DESKTOP_OSX;
					deviceType = DeviceTypes.DESKTOP;
					break;
				case UnityEngine.RuntimePlatform.WindowsEditor:
				case UnityEngine.RuntimePlatform.WindowsPlayer:
					platform = Platforms.DESKTOP_WINDOWS;
					deviceType = DeviceTypes.DESKTOP;
					break;
				case UnityEngine.RuntimePlatform.LinuxPlayer:
					platform = Platforms.DESKTOP_LINUX;
					deviceType = DeviceTypes.DESKTOP;
					break;
				case UnityEngine.RuntimePlatform.OSXWebPlayer:
				case UnityEngine.RuntimePlatform.WindowsWebPlayer:
					platform = Platforms.WEB_BROWSER;
					deviceType = DeviceTypes.WEB;
					break;
				case UnityEngine.RuntimePlatform.FlashPlayer:
					platform = Platforms.WEB_FLASH;
					deviceType = DeviceTypes.WEB;
					break;
				case UnityEngine.RuntimePlatform.NaCl:
					platform = Platforms.WEB_GOOGLE;
					deviceType = DeviceTypes.WEB;
					break;
				case UnityEngine.RuntimePlatform.WiiPlayer:
					platform = Platforms.CONSOLE_WII;
					deviceType = DeviceTypes.CONSOLE;
					break;
				case UnityEngine.RuntimePlatform.XBOX360:
					platform = Platforms.CONSOLE_XBOX;
					deviceType = DeviceTypes.CONSOLE;
					break;
				case UnityEngine.RuntimePlatform.PS3:
					platform = Platforms.CONSOLE_PS3;
					deviceType = DeviceTypes.CONSOLE;
					break;
				case UnityEngine.RuntimePlatform.IPhonePlayer:
					platform = Platforms.DEVICE_IOS;
					deviceType = (IsPhone()) ? DeviceTypes.PHONE : DeviceTypes.TABLET;
					break;
				case UnityEngine.RuntimePlatform.Android:
					platform = Platforms.DEVICE_ANDROID;
					deviceType = (IsPhone()) ? DeviceTypes.PHONE : DeviceTypes.TABLET;
					break;
				// Not implemented yet
				/*
				case UnityEngine.RuntimePlatform.WindowsPhone:
					platform = Platforms.DEVICE_WINPHONE;
					deviceType = DeviceTypes.PHONE;
					break;
				*/
				default:
					platform = Platforms.DESKTOP_WINDOWS;
					deviceType = DeviceTypes.DESKTOP;
					break;
			}
		}
		
		private static bool IsPhone() {
			float phoneScreenWidth = SettingsManager.GetValueFloat(Settings.SYSTEM_PHONE_SCREEN_WIDTH);
			return Screens.minPhysical < phoneScreenWidth;
		}

		public static string PrintInfo() {
			return
				"appName: " + appName +
				"\nappVersionNum: " + appVersionNum +
				"\nappVersionName: " + appVersionName +
				"\ndeviceName: " + deviceName +
				"\ndeviceModel: " + deviceModel +
				"\ndeviceId: " + deviceModel +
				"\nplatform: " + platform +
				"\ndeviceType: " + deviceType +
				"\noperatingSystem: " + operatingSystem +
				"\nunityVersion: " + unityVersion +
				"\nvibrationSupport: " + vibrationSupport +
				"\ntouchSupport: " + touchSupport +
				"\nsystemLanguage: " + language
			;
		}
	}
}
