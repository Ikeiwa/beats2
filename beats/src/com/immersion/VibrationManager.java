/*
** =========================================================================
** Copyright (c) 2010  Immersion Corporation.  All rights reserved.
**                     Immersion Corporation Confidential and Proprietary
**
** File:
**     VibeSystem.java
**
** Description: 
**     ImmVibe sample application for Android.
**
** =========================================================================
*/

package com.immersion;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;
import java.util.Vector;

import javax.xml.parsers.FactoryConfigurationError;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.parsers.SAXParserFactory;

import org.xml.sax.Attributes;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;
import org.xml.sax.XMLReader;
import org.xml.sax.helpers.DefaultHandler;

import com.immersion.uhl.Device;
import com.immersion.uhl.EffectHandle;
import com.immersion.uhl.IVTBuffer;
import com.immersion.uhl.ImmVibe;
import com.immersion.uhl.LauncherEx;
import com.immersion.uhl.MagSweepEffectDefinition;
import com.immersion.uhl.PeriodicEffectDefinition;

import android.content.Context;
import android.os.Environment;
import android.util.Log;

/**
 * <p>This is a sample effect manager class for advanced haptic
 * feedback developers who aim to get the best experience out of
 * a variety of devices. This class is also useful for developers
 * who want to get started quickly using the built-in effects (Launcher)
 * but may be interested in later exploring advanced effect creation
 * for one or more specific devices.</p>
 *
 * <p>This class somewhat decouples effect design from effect programming.
 * The XML file and IVT files should reside in the assets folder, however
 * this class looks on the sdcard first allowing an effect designer to
 * override the effects in the assets folder thus working independent
 * of the programmer to get the feeling "just right." Once complete,
 * the programmer can update the assets folder with final effects.</p>
 */
public class VibrationManager
{
    private Device m_device = null;
    private IVTBuffer m_ivtBuffer = null;   // the active IVT data file
    
    // dynamic effects per actuator
    private Device[] m_actuators = null;
    private EffectHandle m_hEffectPeriodic = null;
    private EffectHandle m_hEffectMagSweep = null;
    private PeriodicEffectDefinition m_defPeriodic = new PeriodicEffectDefinition(0, 0, 0, 0, 0, 0, 0, 0, 0);
    private MagSweepEffectDefinition m_defMagSweep = new MagSweepEffectDefinition(0, 0, 0, 0, 0, 0, 0, 0);
    
    // per-effect priority mechanism
    private Map<String,Integer> m_mapPriority = new HashMap<String,Integer>();
    private int m_nLastPriority = 0;
    private long m_timeNext = 0;
    
    private LauncherEx m_launcher;
    private Map<String,Integer> m_mapLauncher = new HashMap<String,Integer>();
    
    /**
     * <p>Loads an XML file from assets (or sdcard) which contains effect
     * mappings from name to launcher index. An effect designer may
     * optionally override the launcher effects with a custom IVT
     * file which is chosen based on modelname/actuator-type.</p>
     * 
     * <p>For more information on the XML file format, see the
     * example sample XML files in the assets folder of this project.</p>
     * 
     * @param context Calling application context
     * @param strXmlSettings Name of XML file in assets folder. This file
     * may also be located on the sdcard with the package name of the
     * application prepended ("com.immersion.samples." + strXmlSettings)
     */
    public VibrationManager(Context context, String strXmlSettings)
    {
        try
        {
            // useful for dynamic effect code and actuator queries
            if (m_actuators == null)
            {
                m_actuators = new Device[ImmVibe.getInstance(context).getDeviceCount()];
                for (int i = 0; i < m_actuators.length; i++)
                    m_actuators[i] = Device.newDevice(context, i);
            }
            
            // get vibration settings
            XMLHandler xmlHandler = new XMLHandler(context, strXmlSettings);
            m_mapLauncher = xmlHandler.mapLauncher;
            m_mapPriority = xmlHandler.mapPriority;
            
            try
            {
                // load IVT file specific to actuator configuration
                XMLHandler.Collection.Item item = xmlHandler.specList.find(android.os.Build.MANUFACTURER, android.os.Build.MODEL, m_actuators);
                byte[] data = getOverrideableData(context, item.filename);
                m_ivtBuffer = new IVTBuffer(data);
                
                // initialize a multiactuator device if supported
                // otherwise fallback to a single actuator device
                try
                {
                    m_device = Device.newDevice(context);
                }
                catch (RuntimeException e)
                {
                    m_device = Device.newDevice(context, 0);
                }
                
                // remove launcher definitions for effects that exist in IVT data
                Vector<String> deleteme = new Vector<String>();
                for (Iterator<String> iterator = m_mapLauncher.keySet().iterator(); iterator.hasNext(); )
                {
                    String effectName = iterator.next();
                    
                    try
                    {
                        m_ivtBuffer.getEffectIndexFromName(effectName);
                        deleteme.add(effectName);
                    }
                    catch (RuntimeException re) {}
                }
                for (Iterator<String> iterator = deleteme.iterator(); iterator.hasNext(); )
                {
                    String effectName = iterator.next();
                    m_mapLauncher.remove(effectName);
                }
            }
            catch (Throwable t) {}
            
            // initialize Launcher
            try
            {
                if (m_mapLauncher.size() > 0)
                    m_launcher = new LauncherEx(context);
            }
            catch (Throwable t) {}
        }
        catch (Throwable t) {}
    }
    
    /**
     * Call in Activity onDestroy or when VibrationManager is
     * no longer needed.
     */
    public void release()
    {
        try
        {
            if (m_launcher != null)
            {
                m_launcher.stop();
                m_launcher = null;
            }
            
            if (m_device != null)
            {
                m_device.close();
                m_device = null;
            }
            
            if (m_actuators != null)
            {
                for (int i = 0; i < m_actuators.length; i++)
                {
                    m_actuators[i].close();
                    m_actuators[i] = null;
                }
                m_actuators = null;
            }
        }
        catch (Throwable t)
        {
            
        }
    }
    
    /**
     * Plays a simple effect. If the effect does not exist or fails for any
     * reason, this function fails silently.
     * 
     * @param name Name of effect stored in effect data. The effect may exist
     *             in any number of files depending on which device is active.
     * @return Playing effect handle
     */
    public EffectHandle playEffect(String name)
    {
        try
        {
            // check effect priority
            Integer priority = m_mapPriority.get(name);
            int nPriority = priority != null ? priority.intValue() : 0;
            long time = System.currentTimeMillis();
            if (m_nLastPriority > nPriority && time < m_timeNext)
            {
                return null;
            }
            m_nLastPriority = nPriority;
            
            // play launcher effect
            Integer launcher = m_mapLauncher.get(name);
            if (launcher != null)
            {
                if (m_launcher != null)
                {
                    m_launcher.play(launcher.intValue());
                    m_timeNext = time + m_launcher.getDuration(launcher.intValue());
                    return m_launcher.getEffectHandle();
                }
                return null;
            }
            
            // play advanced effect
            if (m_device == null || m_ivtBuffer == null)
                return null;
            int nIndex = m_ivtBuffer.getEffectIndexFromName(name);
            EffectHandle effectHandle = m_device.playIVTEffect(m_ivtBuffer, nIndex);
            m_timeNext = time + m_ivtBuffer.getEffectDuration(nIndex);
            return effectHandle;
        }
        catch (Throwable t)
        {
            log(t.toString());
            return null;
        }
    }
    
    /**
     * Stop all playing vibration effects. This should be called at application
     * pause or exit. This can also be called to terminate a repeating effect
     * if the application guarantees that only one effect plays at a time.
     * The underlying system may support more than one simultaneous effect.
     */
    public void stopEffects()
    {
        try
        {
            if (m_device != null)
            {
                m_device.stopAllPlayingEffects();
            }
            
            for (int i = 0; i < m_actuators.length; i++)
            {
                m_actuators[i].stopAllPlayingEffects();
            }
            
            m_hEffectPeriodic = null;
            m_hEffectMagSweep = null;
        }
        catch (Throwable t)
        {
            
        }
    }
    
    /**
     * Get the names of all effects in the effect data.
     */
    public String[] getEffectNames()
    {
        try
        {
            if (m_ivtBuffer == null)
                return new String[0];
            
            // get number of effects in IVT data
            int effectCount = m_ivtBuffer.getEffectCount();
            
            // allocate the buffer
            String[] effects = new String[effectCount];
            
            // copy the names to the buffer
            for (int i = 0; i < effectCount; i++)
            {
                effects[i] = m_ivtBuffer.getEffectName(i);
            }
            
            return effects;
        }
        catch (Throwable t)
        {
            return new String[0];
        }
    }
    
    /**
     * Play or update a MagSweep effect type.
     * 
     * @param   magnitude   [0,1] normalized magnitude. 0 = minimum magnitude, 1 = maximum magnitude
     * @param   duration    the length of time to play the effect or -1 to play continuously.
     */
    public EffectHandle playMagSweep(float magnitude, int duration)
    {
        if (m_actuators[0] == null)
            return null;
        
        m_defMagSweep.setDuration(duration == -1 ? ImmVibe.VIBE_TIME_INFINITE : duration);
        m_defMagSweep.setMagnitude((int) (ImmVibe.VIBE_MAX_MAGNITUDE * magnitude));
        m_defMagSweep.setStyle(ImmVibe.VIBE_STYLE_STRONG);
        
        if (m_hEffectMagSweep == null || duration != -1)
        {
            try { m_hEffectMagSweep = m_actuators[0].playMagSweepEffect(m_defMagSweep); }
            catch (RuntimeException re) {}
        }
        else
        {
            try { m_hEffectMagSweep.modifyPlayingMagSweepEffect(m_defMagSweep); }
            catch (RuntimeException re) {}
        }
        
        return m_hEffectMagSweep;
    }
    
    /**
     * Play or update a Periodic effect type.
     * 
     * @param   magnitude   [0,1] normalized magnitude. 0 = minimum magnitude, 1 = maximum magnitude
     * @param   period      milliseconds of the period of the pulsing waveform
     * @param   duration    the length of time to play the effect or -1 to play continuously.
     * @return  a handle to the playing effect which can be passed to other functions
     *          like isPlaying. 
     */
    public EffectHandle playPeriodic(float magnitude, int period, int duration)
    {
        if (m_actuators[0] == null)
            return null;
        
        m_defPeriodic.setDuration(duration == -1 ? ImmVibe.VIBE_TIME_INFINITE : duration);
        m_defPeriodic.setMagnitude((int) (ImmVibe.VIBE_MAX_MAGNITUDE * magnitude));
        m_defPeriodic.setPeriod(period);
        
        if (m_hEffectPeriodic == null || duration != -1)
        {
            try { m_hEffectPeriodic = m_actuators[0].playPeriodicEffect(m_defPeriodic); }
            catch (RuntimeException re) {}
        }
        else
        {
            try { m_hEffectPeriodic.modifyPlayingPeriodicEffect(m_defPeriodic); }
            catch (RuntimeException re) {}
        }
        
        return m_hEffectPeriodic;
    }
    
    /**
     * Play a dynamic effect encoded in IVT envelope parameters.
     * The Magnitude parameter is the minimum strength of the effect.
     * The AttackLevel parameter is the maximum strength of the effect.
     * 
     * @param effectName    name of effect in IVT data
     * @param strength      [0-1] where 1 is maximum strength 
     * @return playing effect handle or null
     */
    public EffectHandle playMagSweepIVT(String effectName, float strength)
    {
        try
        {
            if (m_ivtBuffer == null)
                return null;
            int effectIndex = m_ivtBuffer.getEffectIndexFromName(effectName);
            MagSweepEffectDefinition definition = m_ivtBuffer.getMagSweepEffectDefinitionAtIndex(effectIndex);
            int duration = definition.getDuration();
            int lowMagnitude = definition.getMagnitude();
            int highMagnitude = definition.getAttackLevel();
            float magnitude = (lowMagnitude + strength * (highMagnitude - lowMagnitude)) / ImmVibe.VIBE_MAX_MAGNITUDE;
            return playMagSweep(magnitude, duration);
        }
        catch (RuntimeException re)
        {
            return null;
        }
    }

    /**
     * Play a dynamic effect encoded in IVT envelope parameters.
     * The Magnitude parameter is the minimum strength of the effect.
     * The AttackLevel parameter is the maximum strength of the effect.
     * 
     * @param effectName    name of effect in IVT data
     * @param strength      [0-1] where 1 is maximum strength 
     * @return playing effect handle or null
     */
    public EffectHandle playPeriodicIVT(String effectName, float strength)
    {
        try
        {
            if (m_ivtBuffer == null)
                return null;
            int effectIndex = m_ivtBuffer.getEffectIndexFromName(effectName);
            PeriodicEffectDefinition definition = m_ivtBuffer.getPeriodicEffectDefinitionAtIndex(effectIndex);
            int duration = definition.getDuration();
            int lowMagnitude = definition.getMagnitude();
            int highMagnitude = definition.getAttackLevel();
            int period = definition.getPeriod();
            float magnitude = (lowMagnitude + strength * (highMagnitude - lowMagnitude)) / ImmVibe.VIBE_MAX_MAGNITUDE;
            return playPeriodic(magnitude, period, duration);
        }
        catch (RuntimeException re)
        {
            return null;
        }
    }
    
    /**
     * Get the duration of the indexed effect in the effect set
     * 
     * @param index 0-based index of the effect within the current effect set
     * @return the duration of the indexed effect
     */
    public int getDuration(int index)
    {
        int duration = 0;
        
        if (m_ivtBuffer != null)
        {
            try { duration = m_ivtBuffer.getEffectDuration(index); }
            catch (RuntimeException re) {}
        }
        
        return duration;
    }
    
    /**
     * Get the status of a previously-played effect
     * 
     * @param hPlayingEffect the handle returned by playEffect
     * @return true if the effect is currently playing
     */
    public boolean isPlaying(EffectHandle hPlayingEffect)
    {
        try { return hPlayingEffect != null && hPlayingEffect.isPlaying(); }
        catch (RuntimeException re) { return false; }
    }
        
    /**
     * 
     * @param strength Value between [0,1] where 1 is maximum strength
     */
    public void setStrength(float strength)
    {
        if (m_device == null)
            return;
        
        int nStrength = (int) (strength * ImmVibe.VIBE_MAX_MAGNITUDE);
        if (nStrength < ImmVibe.VIBE_MIN_MAGNITUDE)
            nStrength = ImmVibe.VIBE_MIN_MAGNITUDE;
        else if (nStrength > ImmVibe.VIBE_MAX_MAGNITUDE)
            nStrength = ImmVibe.VIBE_MAX_MAGNITUDE;
        
        try { m_device.setPropertyInt32(ImmVibe.VIBE_DEVPROPTYPE_STRENGTH, nStrength); }
        catch (RuntimeException re) { log("failed to set strength"); }
    }
    
    // ------------------------------------------------------------
    // The following functions are convenience functions for
    // loading IVT data from various points in the file system.
    // This allows effects to be overridden by the effect designer
    // for the purpose of development without recompiling the app.
    // ------------------------------------------------------------
    
    /**
     * Get the named file from the file system
     */
    private static File getFile(String filename)
    {
        File f = new File(filename);
        if (f.exists() && f.canRead()) {
            log("Found file " + f.getAbsolutePath());
            return f;
        }
        
        return null;
    }
    
    /**
     * Try finding the named file in a few different places so an
     * effect designer can tweak the effects after the application
     * is compiled. It first looks on the sd-card if present using
     * an application-unique filename by prepending the package
     * name of the application, then looks in the application
     * data directory, then tries the name exactly as specified.
     * @param context Application context of current application
     * @param filename Name of file (can be a relative path)
     * @return
     */
    private static File getOverrideableFile(Context context, String filename)
    {
        File f;
        
        // try sd-card
        f = getFile(Environment.getExternalStorageDirectory().getAbsolutePath() + File.separator + context.getPackageName() + "." + filename);
        if (f != null && f.exists())
            return f;
        
        // try application data directory
        //f = getFile(context.getFilesDir() + File.separator + filename);
        //if (f != null && f.exists())
        //    return f;
        
        // try verbatim
        f = getFile(filename);
        if (f != null && f.exists())
            return f;
        
        return null;
    }
    
    /**
     * Get an InputStream to the named file searching in a few different
     * places on the file system. The differs slightly from getOverrideableFile
     * in that it looks in the application assets directory if the named
     * file cannot be found anywhere else on the file system.
     * @param context
     * @param filename
     * @return
     * @throws FileNotFoundException
     * @throws IOException
     */
    private static InputStream getOverrideableInputStream(Context context, String filename) throws FileNotFoundException, IOException
    {
        File f = getOverrideableFile(context, filename);
        if (f != null)
        {
            return new FileInputStream(f);
        }
        
        try
        {
            InputStream inputStream = context.getAssets().open(filename);
            log("Found asset " + filename);
            return inputStream;
        }
        catch (IOException ioe)
        {
            return null;
        }
    }
    
    /**
     * Get a byte array with the contents of the named file searching in
     * a few different places on the file system. See getOverrideableInputStream
     * for more information.
     * @param context
     * @param filename
     * @return
     * @throws FileNotFoundException
     * @throws IOException
     */
    private static byte[] getOverrideableData(Context context, String filename) throws FileNotFoundException, IOException
    {
        InputStream is = getOverrideableInputStream(context, filename);
        byte[] buffer = new byte[is.available()];
        is.read(buffer);
        return buffer;
    }
    
    /**
     * While inner classes are overkill, at least it keeps
     * all the logic in one place.
     */
    private class XMLHandler extends DefaultHandler
    {
        private class Collection
        {
            private class Item
            {
                String filename;
                String modelname;
                String manufacturer;
                Vector<Integer> actuators = new Vector<Integer>();
            }
            
            Vector<Item> specifications = new Vector<Item>();
            
            Item newItem()
            {
                Item item = new Item();
                specifications.add(item);
                return item;
            }
            
            Item find(String manufacturer, String modelname, Device[] actuators)
            {
                // all elements in specification must match
                for (int i = 0, size = specifications.size(); i < size; i++)
                {
                    Item spec = specifications.get(i);
                    
                    // check for specific device name
                    if (spec.modelname != null && !spec.modelname.equalsIgnoreCase(modelname))
                        continue;
                    
                    if (spec.manufacturer != null && !spec.manufacturer.equalsIgnoreCase(manufacturer))
                        continue;
                    
                    // no actuator specification
                    if (spec.actuators.size() == 0)
                        return spec;
                    
                    // actuator spec does not match
                    if (spec.actuators.size() != actuators.length)
                        continue;
                    
                    boolean matches = true;
                    for (int j = 0, count = actuators.length; matches && j < count; j++)
                    {
                        // assume actuators that fail this are ERM 
                        int nActuatorType = actuators[j].getCapabilityInt32(ImmVibe.VIBE_DEVCAPTYPE_ACTUATOR_TYPE);
                        matches = nActuatorType == spec.actuators.get(j).intValue();
                    }
                    if (matches)
                        return spec;
                }
                
                return null;
            }
        }
        
        public Map<String,Integer> mapLauncher;
        public Map<String,Integer> mapPriority;
        public Collection specList = null;
        private Collection.Item currentSpec = null;
        
        public XMLHandler(Context context, String xmlFilename) throws SAXException, ParserConfigurationException, FactoryConfigurationError, FileNotFoundException, IOException
        {
            mapPriority = new HashMap<String,Integer>();
            mapLauncher = new HashMap<String,Integer>();
            
            XMLReader xr = SAXParserFactory.newInstance().newSAXParser().getXMLReader();
            xr.setContentHandler(this);
            xr.parse(new InputSource(getOverrideableInputStream(context, xmlFilename)));
        }
        
        public void startElement(String uri, String localName, String qName,
                Attributes attributes) throws SAXException {
            
            if (localName.equals("effectset"))
            {
                specList = new Collection();
            }
            else if (localName.equalsIgnoreCase("file"))
            {
                currentSpec = specList.newItem();
                currentSpec.filename = attributes.getValue("filename");
                currentSpec.modelname = attributes.getValue("modelname");
                currentSpec.manufacturer = attributes.getValue("manufacturer");
            }
            else if (localName.equals("actuator"))
            {
                /** set actuator - order matters */
                String szDevActuatorType = attributes.getValue("devactuatortype");
                int nActuatorType = szDevActuatorType.equalsIgnoreCase("VIBE_DEVACTUATORTYPE_BLDC") ? ImmVibe.VIBE_DEVACTUATORTYPE_BLDC
                        : szDevActuatorType.equalsIgnoreCase("VIBE_DEVACTUATORTYPE_ERM") ? ImmVibe.VIBE_DEVACTUATORTYPE_ERM
                        : szDevActuatorType.equalsIgnoreCase("VIBE_DEVACTUATORTYPE_LRA") ? ImmVibe.VIBE_DEVACTUATORTYPE_LRA
                        : szDevActuatorType.equalsIgnoreCase("VIBE_DEVACTUATORTYPE_PIEZO") ? ImmVibe.VIBE_DEVACTUATORTYPE_PIEZO
                        : -1;
                
                if (nActuatorType != -1)
                    currentSpec.actuators.add(nActuatorType);
                else
                    warn("invalid actuator type \"" + szDevActuatorType + "\"");
            }
            else if (localName.equals("effect"))
            {
                String strName = attributes.getValue("name");
                String strPriority = attributes.getValue("priority");
                String strLauncher = attributes.getValue("launcher");
                
                if (strName != null)
                {
                    if (strPriority != null)
                    {
                        try { mapPriority.put(strName, Integer.parseInt(strPriority)); }
                        catch (NumberFormatException nfe)
                        {
                            warn("XML parsing error: effect priority should be integer");
                        }
                    }
                    
                    if (strLauncher != null)
                    {
                        try { mapLauncher.put(strName, Integer.parseInt(strLauncher)); }
                        catch (NumberFormatException nfe)
                        {
                            warn("XML parsing error: effect launcher should be integer");
                        }
                    }
                }
            }
        }
        
        public void endElement(String uri, String localName, String qName)
                throws SAXException {
            
            if (localName.equalsIgnoreCase("effectset"))
            {
                currentSpec = null;
            }
        }
        
        public void characters(char[] ch, int start, int length)
                throws SAXException {
        }
    }
    
    /**
     * Display a log message for debugging
     * @param msg String to display in the log
     */
    private static void log(String msg)
    {
        Log.i("VibrationManager", msg);
    }
    
    private static void warn(String msg)
    {
        Log.w("VibrationManager", msg);
    }
}
