/*
    FraMMWorks - Multimedia Processing and Diagnostic Framework.
    Copyright (C) 2008  Wilson Waters

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using FraMMWorks.Interfaces;

namespace FraMMWorks.Core
{
   /// <remarks>
   /// Scans, creates and provides instances of plugins for use by the framework.
   /// This is implemlented in a singleton pattern as there should only
   /// ever be a single plugin manager in operation.
   /// 
   /// Wilson Waters W.Waters@curtin.edu.au 20080115
   /// </remarks>
   public class PluginManager
   {
      //----------------------- consts ----------------------------------------
      /// <summary>
      /// Directory where plugins are stored
      /// </summary>
      const String PLUGIN_DIRECTORY = @".\plugins\";

      //----------------------- structs/enums ---------------------------------
      /// <remarks>
      /// Information on a particular plugin.
      /// </remarks>
      public struct PluginInfo
      {
         /// <summary>
         /// The Type of this plugin (as returned by the typeof() operator)
         /// </summary>
         public Type type;

         /// <summary>
         /// The name as provided by this plugin
         /// </summary>
         public String name;

         /// <summary>
         /// The description as provided by this plugin
         /// </summary>
         public String  description;

         /// <summary>
         /// The inputCapabilities handled by this plugin
         /// </summary>
         public Type [] inputCapabilities;

         /// <summary>
         /// The outputCapabilities provided by this plugin
         /// </summary>
         public Type [] outputCapabilities;

         /// <summary>
         /// Compare the actual conents of this pluginInfo for equality
         /// </summary>
         /// <param name="obj"></param>
         /// <returns></returns>
         public bool Equals(PluginInfo p)
         {
            // Check the basic fields
            // ww 20080214 remove type comparison. It's never the same
            //if (!(this.type == p.type && this.name.Equals(p.name) && this.description.Equals(p.description)))
            if (!(this.name.Equals(p.name) && this.description.Equals(p.description)))
               return false;

            // make sure the input capabilities are exactly the same
            if (this.inputCapabilities != null && p.inputCapabilities != null)
            {
               for (int i = 0; i < this.inputCapabilities.Length; i++)
                  if (this.inputCapabilities[i] != p.inputCapabilities[i])
                     return false;
            }
            else if (this.inputCapabilities == null && p.inputCapabilities == null)
            {
            }
            else
            {
               return false;
            }

            // make sure the output capabilities are exactly the same
            if (this.outputCapabilities != null && p.outputCapabilities != null)
            {
               for (int i = 0; i < this.outputCapabilities.Length; i++)
                  if (this.outputCapabilities[i] != p.outputCapabilities[i])
                     return false;
            }
            else if (this.outputCapabilities == null && p.outputCapabilities == null)
            {
            }
            else
            {
               return false;
            }

            // If we made it here, they are "the same"
            return true;

         }

         public override bool Equals(object obj)
         {
            // make sure it's a PluginInfo object
            if (!(obj is PluginInfo))
               return false;

            return Equals((PluginInfo)obj);
         }


         /// <summary>
         /// Format the pluginInfo details nicely
         /// </summary>
         /// <returns></returns>
         public override string ToString()
         {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("FraMMWorks Plugin");
            sb.Append("   Name: "); sb.AppendLine(this.name);
            sb.Append("   Description: "); sb.AppendLine(this.description);
            sb.Append("   Type: "); sb.AppendLine(this.type.Name);
            sb.Append("   InputCapabilities: ");
            bool first = true;
            foreach(Type t in this.inputCapabilities)
            {
               if(first)
               {
                  first = false;
               }
               else
               {
                  sb.Append(", ");
               }
               sb.Append(t.Name);
            }
            sb.AppendLine();
            sb.Append("   OutputCapabilities: ");
            first = true;
            foreach(Type t in this.outputCapabilities)
            {
               if(first)
               {
                  first = false;
               }
               else
               {
                  sb.Append(", ");
               }
               sb.Append(t.Name);
            }
            return sb.ToString();
         }
      }

      //----------------------- private data members --------------------------
      /// <summary>
      /// The actual singleton instance
      /// </summary>
      private static volatile PluginManager instance;

      /// <summary>
      /// Thread lock for multi-threaded access to object.
      /// </summary>
      private static object syncRoot = new Object();

      /// <summary>
      /// List of plugins that we know about. Cal scanAvailablePlugins to refresh
      /// </summary>
      private PluginInfo[] availablePlugins;

      /// <summary>
      /// Internal array corresponding to the AvailablePlugins field containing
      /// the associated plugin assembly for the plugin. use the CreateInstance
      /// field to create a new one.
      /// </summary>
      private Assembly[] availablePluginAssemblies;


      //----------------------- public data members --------------------------
      /// <summary>
      /// List of plugins that we know about.
      /// </summary>
      public PluginInfo[] AvailablePlugins
      {
         get { lock (this) { return availablePlugins; } }
      }

      //----------------------- Constructors ----------------------------------
      /// <summary>
      /// Default constructor
      /// </summary>
      private PluginManager()
      {
         scanForPlugins();
      }

      /// <summary>
      /// Get the PluginManager singleton class. This is thread safe and can
      /// handle two threads calling Instance() at the same time (especially
      /// important when creating the luginManager for the first time).
      /// 
      /// See http://msdn2.microsoft.com/en-us/library/ms998558.aspx for
      /// implementation example.
      /// </summary>
      public static PluginManager Instance
      {
         get 
         {
            if (instance == null) 
            {
               lock (syncRoot) 
               {
                  if (instance == null)
                     instance = new PluginManager();
               }
            }

            return instance;
         }
      }

      //----------------------- public processing members ---------------------
      /// <summary>
      /// Scans the current path/directory for plugin dll files and stores info
      /// to the class.
      /// </summary>
      /// <returns>the number of plugins found</returns>
      public int scanForPlugins()
      {
         // implelmented with help from http://www.thescripts.com/forum/thread231548.html

         // Temp store for plugin details. Saved to class fields later
         List<PluginInfo> pluginInfo = new List<PluginInfo>();
         List<Assembly> pluginAssemblies = new List<Assembly>();

         // scan the same directory as the .exe AND the PLUGIN_DIRECTORY.
         string[] pluginFilesStartupDir = Directory.GetFiles(Application.StartupPath, "*.DLL");
         string[] pluginFilesPluginDir = new string []{};
         if (Directory.Exists(PLUGIN_DIRECTORY))
            pluginFilesPluginDir = Directory.GetFiles(PLUGIN_DIRECTORY, "*.DLL");
         string[] dllFiles = new string[pluginFilesStartupDir.Length + pluginFilesPluginDir.Length];
         pluginFilesStartupDir.CopyTo(dllFiles, 0);
         if (pluginFilesPluginDir != null)
            pluginFilesPluginDir.CopyTo(dllFiles, pluginFilesStartupDir.Length);
         foreach (string file in dllFiles)
         {
            try
            {
               Assembly pluginAssembly = Assembly.LoadFrom(file);
               Type[] typesInAssembly = pluginAssembly.GetTypes();
               foreach (Type type in typesInAssembly)
               {
                  Type pluginType = type.GetInterface("IPlugin");
                  if (pluginType != null)
                  {
                     // Found one!
                     IPlugin plugin = (IPlugin)pluginAssembly.CreateInstance(type.FullName);
                     PluginInfo info = new PluginInfo();
                     info.name = plugin.Name;
                     info.description = plugin.Description;
                     info.type = plugin.GetType();
                     if (typeof(ISink).IsAssignableFrom(info.type))
                     {
                        info.inputCapabilities = ((ISink)plugin).InputCapabilities;
                     }
                     if (typeof(ISource).IsAssignableFrom(info.type))
                     {
                        info.outputCapabilities = ((ISource)plugin).OutputCapabilities;
                     }

                     // Make sure we don't already have this plugin
                     bool alreadyHave = false;
                     foreach (PluginInfo i in pluginInfo)
                     {
                        if (info.Equals(i))
                        {
                           alreadyHave = true;
                           break;
                        }
                     }
                     if (!alreadyHave)
                     {
                        pluginInfo.Add(info);
                        pluginAssemblies.Add(pluginAssembly);
                     }
                  }
               }
            }
            catch (Exception e)
            {
               ControlAPI.Instance.debugMessage(this,"PluginManager: warning - unrecognised plugin: " + file + " (error: " + e.Message + ").");
            }
         }

         // Now save it all to class fields
         lock (this) // Don't let anyone else access it while we're scanning.
         {
            availablePlugins = new PluginInfo[pluginInfo.Count];
            pluginInfo.CopyTo(availablePlugins, 0);
            availablePluginAssemblies = new Assembly[pluginAssemblies.Count];
            pluginAssemblies.CopyTo(availablePluginAssemblies, 0);
         }
         return availablePlugins.Length;
      }

      /// <summary>
      /// Returns a new instance of the plugin as specified in the pluginInfo
      /// parameter.
      /// </summary>
      /// <param name="pluginInfo">the plugin to return an instance of</param>
      /// <returns>a new instance of the requested plugin</returns>
      public IPlugin getNewPluginInstance(PluginInfo pluginInfo)
      {
         IPlugin plugin = null;
         lock (this)
         {
            for (int i = 0 ; i<availablePlugins.Length ; i++)
            {
               if(availablePlugins[i].Equals(pluginInfo))
               {
                  // Found it, stop here and return this one.
                  plugin = (IPlugin)availablePluginAssemblies[i].CreateInstance(availablePlugins[i].type.FullName);
                  break;
               }
            }
         }

         // If we didn't find anything, throw exception.
         if (plugin == null)
            throw new ApplicationException("PluginManager - Couldn't find a plugin matching "+pluginInfo.ToString());

         return plugin;
      }

      //----------------------- private processing members --------------------
   }
}
