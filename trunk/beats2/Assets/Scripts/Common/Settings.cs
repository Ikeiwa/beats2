using System;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;

/*
 * TODO:
 * - keep updated
 */
namespace Beats2.Common {
	
	/// <summary>
	/// Settings enum. Reference these as indicies with SettingsManager
	/// </summary>
	public enum Settings {
		// This should never appear, but just in case...
		ERROR,
		// Common
		INPUT_SWIPE_TIME_MAX,
		INPUT_SWIPE_DIST_MIN,
		RANDOM_SEED,
		SCORE_PERCENT_AAA,
		SCORE_PERCENT_AA,
		SCORE_PERCENT_A,
		SCORE_PERCENT_B,
		SCORE_PERCENT_C,
		SCORE_TIMING_FLAWLESS,
		SCORE_TIMING_PERFECT,
		SCORE_TIMING_GREAT,
		SCORE_TIMING_GOOD,
		SCORE_TIMING_ALMOST,
		SCORE_TIMING_OK,
		SYSTEM_PHONE_SCREEN_WIDTH,
		SYSTEM_ENABLE_VIBRATIONS,
		FPS_COUNTER_UPDATE_INTERVAL
	};
	
	/// <summary>
	/// Default setting values. I could separate these as public constants, but not worth the effort
	/// </summary>
	public static class Defaults {
		
		public static Dictionary<Settings, string> GetDefaultSettings() {
			Dictionary<Settings, string> settings = new Dictionary<Settings, string>(Enum.GetValues(typeof(Settings)).Length);
			
			// Common
			settings.Add(Settings.INPUT_SWIPE_TIME_MAX, 		"0.250000"		); // TODO: Play around with this
			settings.Add(Settings.INPUT_SWIPE_DIST_MIN, 		"0.15"			); // TODO: Play around with this
			settings.Add(Settings.RANDOM_SEED,					"0"				);
			// Score percents based off DDR Extreme (8th Mix): http://aaronin.jp/taren/scoring/ss8.html
			settings.Add(Settings.SCORE_PERCENT_AAA,			"1.00"			);
			settings.Add(Settings.SCORE_PERCENT_AA,				"0.93"			);
			settings.Add(Settings.SCORE_PERCENT_A,				"0.80"			);
			settings.Add(Settings.SCORE_PERCENT_B,				"0.65"			);
			settings.Add(Settings.SCORE_PERCENT_C,				"0.45"			);
			// Timing windows based off DDR Extreme (8th Mix): http://www.ddrfreak.com/phpBB2/viewtopic.php?t=95076
			settings.Add(Settings.SCORE_TIMING_FLAWLESS,		"0.016667"		);
			settings.Add(Settings.SCORE_TIMING_PERFECT,			"0.033333"		);
			settings.Add(Settings.SCORE_TIMING_GREAT,			"0.066667"		);
			settings.Add(Settings.SCORE_TIMING_GOOD,			"0.083333"		);
			settings.Add(Settings.SCORE_TIMING_ALMOST,			"0.133333"		);
			settings.Add(Settings.SCORE_TIMING_OK,				"0.250000"		);
			settings.Add(Settings.SYSTEM_PHONE_SCREEN_WIDTH,	"3"				); // Dell Streak is 3.1" width
			settings.Add(Settings.SYSTEM_ENABLE_VIBRATIONS,		"True"			);
			settings.Add(Settings.FPS_COUNTER_UPDATE_INTERVAL,	"0.5"			);
			
			return settings;
		}
		
	}
	
	/// <summary>
	/// Settings manager. This should be Init() before all other Common classes (this is the reason why we don't use Singleton)
	/// </summary>
	public static class SettingsManager {
		private const string TAG = "SettingsManager";
		
		private static Dictionary<Settings, string> _settings;
		
		public static void Init() {
			_settings = Defaults.GetDefaultSettings();
			Reset();
			Logger.Debug(TAG, "Initialized...");
		}
		
		public static void Reset() {
			ReloadSettings();
			Logger.Debug(TAG, "Reset...");
		}
		
		public static void ReloadSettings() {
			// TODO	
		}
		
		public static void SaveSettings() {
			// TODO
		}
		
		// Returns a null if the settingName could not be found
		private static Settings GetSetting(string settingName) {
			object setting = Enum.Parse(typeof(Settings), settingName, true);
			if (setting != null) {
				return (Settings)setting;
			} else {
				Logger.Error(TAG, String.Format("Unable to find settingName \"{0}\"", settingName));
				return Settings.ERROR;
			}
		}
		
		public static string GetValue(Settings setting) {
			return _settings[setting];
		}
		
		public static bool GetValueBool(Settings setting) {
			bool val;
			if (bool.TryParse(_settings[setting], out val)) {
				return val;
			} else {
				Logger.Error(TAG, String.Format("Unable to parse bool \"{0}\" for setting \"{1}\"", _settings[setting], setting));
				return false;
			}
		}
		public static int GetValueInt(Settings setting) {
			int val;
			if (int.TryParse(_settings[setting], out val)) {
				return val;
			} else {
				Logger.Error(TAG, String.Format("Unable to parse int \"{0}\" for setting \"{1}\"", _settings[setting], setting));
				return -1;
			}
		}
		
		public static float GetValueFloat(Settings setting) {
			float val;
			if (float.TryParse(_settings[setting], out val)) {
				return val;
			} else {
				Logger.Error(TAG, String.Format("Unable to parse float \"{0}\" for setting \"{1}\"", _settings[setting], setting));
				return -1f;
			}
		}
		
		private static void SetValue(string settingName, string newValue) {
			Settings setting = (Settings)Enum.Parse(typeof(Settings), settingName, true);
			_settings[setting] = newValue;
		}
		
		public static void SetValue(Settings setting, string newValue) {
			_settings[setting] = newValue;
		}
		
	}
}
