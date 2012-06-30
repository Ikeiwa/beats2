package com.beatsportable.beats;

import java.io.File;

import android.app.*;
import android.os.*;

public class MenuSplash extends Activity {
	
	// Main screen
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.splash);
		Tools.setContext(this);
		updateCheck();
		versionCheck();
		installFiles();
	}
	
	// Startup Warnings
	private void versionCheck() {
		if (Build.VERSION.SDK_INT < Build.VERSION_CODES.ECLAIR_MR1 &&
			!Tools.getBooleanSetting(R.string.ignoreLegacyWarning, R.string.ignoreLegacyWarningDefault)) {
			// Multitouch warning			
			Tools.warning(
					Tools.getString(R.string.MenuHome_legacy_warning),
					Tools.cancel_action, R.string.ignoreLegacyWarning
					);
		}
	}
	
	private void updateCheck() {
		new ToolsUpdateTask().execute(Tools.getString(R.string.Url_version));
	}
	
	private void installFiles() {
		if (!new File(Tools.getNoteSkinsDir()).canRead()) {
			Tools.installGraphics(this);
		}
		
		if (Tools.getBooleanSetting(R.string.installSamples, R.string.installSamplesDefault) ||
			!Tools.getBooleanSetting(R.string.ignoreNewUserNotes, R.string.ignoreNewUserNotesDefault)) {
			// Make folders and install sample songs
			if (Tools.isMediaMounted() && 
				Tools.makeBeatsDir()
				) {
				Tools.installSampleSongs(this);
				Tools.putSetting(R.string.installSamples, "0");
			}
		} else {
			// Always make folders
			if (Tools.isMediaMounted()) Tools.makeBeatsDir();
		}
	}
}
