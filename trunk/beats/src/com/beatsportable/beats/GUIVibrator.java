package com.beatsportable.beats;

import android.content.Context;
import android.os.Build;
import android.os.Vibrator;

import com.immersion.VibrationManager;
import com.immersion.uhl.EffectHandle;

public class GUIVibrator {
	
	private Vibrator v;
	private VibrationManager vm;
	private int vibrateMiss;
	private boolean vibrateTap;
	private boolean vibrateHold;
	private boolean vibrateTouchSense;
	private int holdsCount;
	private EffectHandle holdHandle;
	
	// This is called in GUIHandler's constructor, which is called by GUIGame's onCreate 
	public GUIVibrator() {
		try {
			vibrateMiss = Integer.valueOf(
					Tools.getSetting(R.string.vibrateMiss, R.string.vibrateMissDefault));
			vibrateTap = Tools.getBooleanSetting(R.string.vibrateTap, R.string.vibrateTapDefault);
			vibrateHold = Tools.getBooleanSetting(R.string.vibrateHold, R.string.vibrateHoldDefault);
			vibrateTouchSense =
				Tools.getBooleanSetting(R.string.vibrateTouchSense, R.string.vibrateTouchSenseDefault)
				&& Integer.parseInt(Build.VERSION.SDK) > Build.VERSION_CODES.CUPCAKE;
			holdsCount = 0;
			holdHandle = null;
			if (vibrateTouchSense) {
				v = null;
				vm = new VibrationManager(Tools.c, "vibration.xml");
			} else {
				v = (Vibrator)Tools.c.getSystemService(Context.VIBRATOR_SERVICE);
				vm = null;
			}
		} catch (Exception e) {
			ToolsTracker.error("GUIVibrator.init", e, Build.VERSION.SDK);
		}
	}
	
	// Call this in GUIHandler's releaseVibrator(), which is called by GUIGame's onDestroy
	public void release() {
		try {
			pause();
			if (vm != null) {
				vm.release();
				vm = null;
			}
		} catch (Exception e) {
			ToolsTracker.error("GUIVibrator.release", e, "");
		}
	}
	
	public void endHold() {
		try {
			holdsCount--;
			if (holdsCount <= 0) {
				holdsCount = 0;
				if (v != null) {
					v.cancel();
				}
				if (holdHandle != null) {
					holdHandle.stop();
					holdHandle = null;
				}
			}
		} catch (Exception e) {
			holdHandle = null;
			ToolsTracker.error("GUIVibrator.endHold", e, "");
		}
	}
	
	public void pause() {
		try {
			holdsCount = 0;
			if (v != null) {
				v.cancel();
			}
			if (holdHandle != null) {
				holdHandle.stop();
				holdHandle = null;
			}
			if (vm != null) {
				vm.stopEffects();
			}
		} catch (Exception e) {
			holdHandle = null;
			ToolsTracker.error("GUIVibrator.pause", e, "");
		}
	}
	
	public void vibrateTap() {
		try {
			if (vibrateTap) {
				if (vibrateTouchSense) {
					if (vm != null) {
						vm.playEffect("tap");
					}
				} else {
					if (v != null) {
						if (holdsCount <= 0) {
							v.vibrate(50);
						}
					}
				}
			}
		} catch (Exception e) {
			ToolsTracker.error("GUIVibrator.vibrateTap", e, "");
		}
	}
	
	public void vibrateHold(boolean hasStartedVibrating) {
		try {
			if (vibrateTap) {
				if (vibrateHold) {
					if (!hasStartedVibrating) {
						holdsCount++;
					}
					if (vibrateTouchSense) {
						if (holdsCount > 1) {
							// Restart a new vibration call since the effect is timeline'd
							if (holdHandle != null) {
								holdHandle.stop();
								holdHandle = null;
							}
						}
						holdHandle = vm.playEffect("hold");
					} else {
						if (v != null) {
							v.vibrate(10000);
						}
					}
				} else {
					vibrateTap();
				}
			}
		} catch (Exception e) {
			ToolsTracker.error("GUIVibrator.vibrateHold", e, "");
		}
	}
	
	public void vibrateMiss() {
		try {
			switch (vibrateMiss) {
				case 0:
					break;
				case 1:
					if (vibrateTouchSense) {
						if (vm != null) {
							vm.playEffect("miss");
						}
					} else {
						if (v != null) {
							v.vibrate(75);
						}
					}
					break;
				case 2:
					if (vibrateTouchSense) {
						if (vm != null) { 
							vm.playEffect("miss2");
						}
					} else {
						if (v != null) v.vibrate(150);
					}
					break;
				default:
					break;
			}
		} catch (Exception e) {
			ToolsTracker.error("GUIVibrator.vibrateMiss", e, "");
		}
	}
}
