using LPluginReg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PluginLoader
{

    [XmlRoot("Configuration")]
    [XmlInclude(typeof(Plugin))]
    public class Configuration
    {
        [XmlIgnore]
        private string config_xml = Global.AppVariables.XMLDirectory + "PluginsConfig.xml";

        [XmlIgnore]
        public bool IsLoaded = false;

        private List<Plugin> plugins = null;

        [XmlArray("Plugins")]
        [XmlArrayItem("Plugin")]
        public List<Plugin> Plugins
        {
            get
            {
                return plugins;
            }

            set
            {
                plugins = value;
            }
        }

        // This has to be defined to serialize this object
        public Configuration()
        { }


        public void Load()
        {
            if (File.Exists(config_xml))
            {
                // Load existing configurations
                LoadConfig();
            }
            else
            {
                // Create LHue directory
                Directory.CreateDirectory(Global.AppVariables.CurrentPluginDirectory);
                Plugins = new List<Plugin>();
            }
            IsLoaded = true;
        }

        public void Save()
        {
            // Construct an instance of the XmlSerializer with the type of object that is being deserialized.
            XmlSerializer serializer = new XmlSerializer(this.GetType());

            if (File.Exists(config_xml))
                File.SetAttributes(config_xml, FileAttributes.Normal);

            if (!Directory.Exists(config_xml + "\\..\\"))
                Directory.CreateDirectory(config_xml + "\\..\\");

            // Write to a file, create a StreamWriter object.
            using (StreamWriter streamWriter = new StreamWriter(config_xml))
            {
                serializer.Serialize(streamWriter, this);
            }

            File.SetAttributes(config_xml, FileAttributes.Hidden);
        }

        public Plugin AddPlugin(string FullName, string FullExeOrDLLPath, string Version)
        {
            Plugin p = new Plugin(FullName, FullExeOrDLLPath, Version);
            return AddPlugin(p);
        }

        public void UpdatePlugin(Plugin plugin, Assembly assembly)
        {
            string v = assembly.GetName().Version.ToString();
            plugin.Version = v.Split('.')[0] + "." + v.Split('.')[1];
            plugin.FullName = assembly.FullName;
            Save();
        }

        public void AddPlugin (string FullExeOrDLLPath)
        {
            List<string> Errors = new List<string>();

            try
            {
                Assembly a = Assembly.LoadFrom(FullExeOrDLLPath);
                string v = a.GetName().Version.ToString();
                Plugin p = AddPlugin(a.FullName, a.Location, v.Split('.')[0] + "." + v.Split('.')[1]);

                var test = a.GetTypes();
                test = null;
                p.Enabled = true;
            }
            catch (Exception error)
            {
                if (error is System.Reflection.ReflectionTypeLoadException)
                {
                    ReflectionTypeLoadException typeLoadException = error as ReflectionTypeLoadException;
                    Exception LastException = null;

                    foreach (Exception e in typeLoadException.LoaderExceptions)
                    {
                        if (LastException == null || !LastException.Message.Equals(e.Message))
                        {
                            Errors.Add(e.Message);
                            if (e.InnerException != null)
                                Errors.Add(e.InnerException.Message);
                        }
                        LastException = e;
                    }
                }
                else
                {
                    Errors.Add(error.Message);
                    if (error.InnerException != null)
                        Errors.Add(error.InnerException.Message);
                }
            }

            if (Errors.Count > 0)
                throw new Exception (string.Join("\r\n", Errors.ToArray()));
        }

        public Plugin AddPlugin(Plugin plugin)
        {
            if (!IsLoaded) Load();
            if (Plugins.Exists(p => p.FullName == plugin.FullName))
            {
                Plugins.Remove(Plugins.Find(p => p.FullName == plugin.FullName));
            }
            Plugins.Add(plugin);

            return plugin;
        }

        public void DisablePlugin(string PluginName)
        {
            if (!IsLoaded) Load();
            if (Plugins.Exists(p => p.Name == PluginName))
            {
                Plugin plugin = Plugins.Find(p => p.Name == PluginName);
                plugin.Enabled = false;
            }
        }

        #region Private Methods
        private void LoadConfig()
        {
            // Construct an instance of the XmlSerializer with the type of object that is being deserialized.
            XmlSerializer serializer = new XmlSerializer(this.GetType());

            // Read the file, create a FileStream.
            using (FileStream fileStream = new FileStream(config_xml, FileMode.Open))
            {
                Plugins = ((Configuration)serializer.Deserialize(fileStream)).Plugins;
            }
        }
        #endregion Private Methods
    }

    [XmlType("Plugin")]
    public class Plugin
    {
        /// <summary>
        /// Parameter less constructor for 
        /// </summary>
        public Plugin()
        {
        }

        public Plugin(string Name, string Path, string Version)
        {
            this.FullPath = Path;
            this.FullName = Name;
            this.Enabled = false;
            this.Version = Version;
        }

        private string fullName;
        private string name;
        private string fullPath;
        private bool enabled;
        private string version = null;


        [XmlAttribute("Name", DataType = "string")]
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        [XmlElement]
        public string FullName
        {
            get
            {
                return fullName;
            }

            set
            {
                fullName = value;
            }
        }

        [XmlElement]
        public string FullPath
        {
            get
            {
                return fullPath;
            }

            set
            {
                fullPath = value;
                name = Path.GetFileNameWithoutExtension(fullPath);
            }
        }

        [XmlElement]
        public bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                enabled = value;
            }
        }

        [XmlAttribute("Version", DataType = "string")]
        public string Version
        {
            get
            {
                return version;
            }

            set
            {
                version = value;
            }
        }
    }

}
