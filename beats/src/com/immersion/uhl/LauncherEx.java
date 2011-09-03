/*
** =============================================================================
** Copyright (c) 2010-2011  Immersion Corporation. All rights reserved.
**                          Immersion Corporation Confidential and Proprietary
**
** File:
**     LauncherEx.java
**
** Description:
**     Simple extension to UHL Launcher class
**
** =============================================================================
*/

package com.immersion.uhl;

import com.immersion.uhl.Launcher;

import android.content.Context;

/**
 * Small extension class to get duration of Launcher effects
 */
public class LauncherEx extends Launcher
{
    /**
     * Creates an instance of LauncherEx.
     * @param context               Context of calling application
     * @throws RuntimeException     if Launcher fails to initialize
     */
    public LauncherEx(Context context) throws RuntimeException
    {
        super(context);
    }

    /**
     * Get the duration of the indexed Launcher effect.
     * @param index    Launcher effect index (use constants)
     * @return         duration of effect in milliseconds
     */
    public int getDuration(int index)
    {
        try
        {
            return effectSet.getEffectDuration(index);
        }
        catch (RuntimeException re)
        {
            return 0;
        }
    }
    
    /**
     * Get handle of last played effect.
     * @return         last played effect
     */
    public EffectHandle getEffectHandle()
    {
        return effect;
    }
}
