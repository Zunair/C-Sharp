using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LHue
{
    public class Response
    {
        public JObject Lights = null;
        public JObject Groups = null;
        public JObject Config = null;
        public JObject Schedules = null;
        public JObject Scenes = null;
        public JObject Sensors = null;
        public JObject Rules = null;        
    }

    [XmlType("Bridge")]
    [XmlInclude(typeof(SwitchSetting))]
    public class Bridge
    {
        private int id = 1;
        private List<SwitchSetting> switchSettings;
        private string ip = string.Empty;
        private string name = string.Empty;
        private string userId = string.Empty;
        private bool isDefault = false;
        private Response webResponse = null;

        [XmlIgnore]
        public ILocalHueClient Client = null;


        // This has to be defined to serialize this object
        public Bridge()
        {
        }

        public Bridge(string name, string ip, string userid, int id)
        {
            this.Name = name;
            this.Ip = ip;
            this.UserId = userid;
            this.Id = id;            
        }
        
        [XmlElement("IP")]
        public string Ip
        {
            get
            {
                return ip;
            }

            set
            {
                ip = value;
                if (ip != string.Empty)
                {
                    Client = new LocalHueClient(ip);
                }
            }
        }

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

        [XmlElement("UserID")]
        public string UserId
        {
            get
            {
                return userId;                
            }

            set
            {
                userId = value;
                if (userId != string.Empty)
                {
                    Client.Initialize(userId);
                }
            }
        }

        [XmlAttribute("Default", DataType = "boolean")]
        public bool IsDefault
        {
            get
            {
                return isDefault;
            }

            set
            {
                isDefault = value;
            }
        }

        [XmlAttribute("Uid", DataType = "int")]
        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        [XmlArray("SwitchSettings")]
        [XmlArrayItem("Switch")]
        public List<SwitchSetting> SwitchSettings
        {
            get
            {
                return switchSettings;
            }

            set
            {
                switchSettings = value;
            }
        }
        
        public async Task<bool> LinkBridge(string name, string ip)
        {
            bool retVal = false;

            UserId = await TryCreateUserID();

            if (UserId != string.Empty)
            {
                retVal = true;
            }            

            return retVal;
        }

        private async Task<string> TryCreateUserID()
        {
            string retVal = string.Empty;
            DateTime started = DateTime.Now;

            // Loops while retVal is empty or loops for 30 secs
            do
            {
                retVal = await CreateUserID();
                if (retVal == string.Empty)
                {
                    Thread.Sleep(5000);
                }
            } while (retVal == string.Empty && started.AddSeconds(30) > DateTime.Now);


            return retVal;
        }

        private async Task<string> CreateUserID()
        {
            string retVal = string.Empty;

            string createUser = "{\"devicetype\":\"LINKS#PC-" + Environment.UserName.ToUpper() + "\"}";
            if (Debugger.IsAttached)
            {
                // Emulate response while debugging
                createUser = "[{\"success\":{\"username\":\"7ngKxvpo5fuLWLmCZfb42KzvKSYlA6gGgX4CYFtN\"}}]";
            }
            else
            {
                //"http://73.251.151.39/api/createUser
                createUser = await LREST.Post("http://" + ip + "/api", createUser, "", ".json");
            }

            try
            {
                try
                {
                    JArray jResponseO = (JArray)JsonConvert.DeserializeObject(createUser);
                    retVal = jResponseO[0]["success"]["username"].ToString();
                }
                catch
                {
                    JArray jResponse = (JArray)JsonConvert.DeserializeObject(createUser);
                    if (jResponse.Count > 0)
                    {
                        //MessageBox.Show(jResponse[0]["error"]["description"] + "\r\n Please press the button on the hue bridge and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        //return;
                    }
                }
            }
            catch(Exception error)
            {
                retVal = "Error: " + error.Message;                
            }

            return retVal;
        }

        public async Task<string> TryGetIP()
        {
            string retVal = string.Empty;
            string webResponse = string.Empty;

            try
            {

                if (Debugger.IsAttached)
                {
                    // Emulate response while debugging
                    webResponse = "[{\"id\":\"test\",\"internalipaddress\":\"73.251.151.39\"}]";
                    //webResponse = "[{\"id\":\"test\",\"internalipaddress\":\"wrightsew.ddns.net:6543\"}]";
                }
                else
                {
                    webResponse = await LREST.Get("https://www.meethue.com/api/nupnp");
                }

                if (!webResponse.StartsWith("Error:"))
                {
                    JArray jResponse = (JArray)JsonConvert.DeserializeObject(webResponse);
                    if (jResponse.Count > 0)
                    {
                        retVal = jResponse[0]["internalipaddress"].ToString();
                    }
                }
                //else
                //{
                //    MessageBox.Show("Can not find Philips Hue SmartHub.\r\nPlease plugin your hue hub and click connect again or type the IP manually.", "Failed to Find Hub", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //    ip = "127.0.0.1";
                //}
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show("Error 01: " + error.Message);
            }

            return retVal;
        }

        public string getBaseUrl (string baseUrl)
        {
            if (!baseUrl.ToUpper().StartsWith("HTTP"))
            {
                baseUrl = "http://" + baseUrl + "/api";
            }
            return baseUrl;
        }

        public async Task<bool> DeleteUser(string userId)
        {
            bool retVal = false;
            string webResponse = string.Empty;

            try
            {

                if (Debugger.IsAttached)
                {
                    // Emulate response while debugging
                    //webResponse = "[{\"id\":\"test\",\"internalipaddress\":\"73.251.151.39\"}]";
                    //webResponse = "[{\"id\":\"test\",\"internalipaddress\":\"wrightsew.ddns.net:6543\"}]";
                }
                else
                {
                    ///api/jbaNdiVb8TUJeyOPrS3SlKFgQmP62nMOOyPpTgAU/config/whitelist/jbaNdiVb8TUJeyOPrS3SlKFgQmP62nMOOyPpTgAU
                    string url = string.Format("{0}/{1}/config/whitelist/{1}", getBaseUrl(ip), userId);
                    webResponse = await LREST.Delete(url);
                }

                if (!webResponse.StartsWith("Error:"))
                {
                    JArray jResponse = (JArray)JsonConvert.DeserializeObject(webResponse);
                    if (jResponse.Count > 0)
                    {
                        if (jResponse[0]["success"].ToString().ToUpper().Contains(userId.ToUpper() + " DELETED"))
                        {
                            retVal = true;
                        }
                        //retVal = jResponse[0]["internalipaddress"].ToString();
                    }
                }
                //else
                //{
                //    MessageBox.Show("Can not find Philips Hue SmartHub.\r\nPlease plugin your hue hub and click connect again or type the IP manually.", "Failed to Find Hub", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //    ip = "127.0.0.1";
                //}
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show("Error 01: " + error.Message);
            }

            return retVal;
        }

        public async Task<Response> GetResponse(bool refresh = false)
        {
            if (webResponse == null)
            {
                webResponse = new Response();
                refresh = true;
            }

            if (refresh)
            {
                string base_url = "http://" + Ip + "/api/" + UserId + "/";
                webResponse.Lights = (JObject)JsonConvert.DeserializeObject(await LREST.Get(base_url + "lights"));
                webResponse.Groups = (JObject)JsonConvert.DeserializeObject(await LREST.Get(base_url + "groups"));
                //await lRest.Put("http://73.251.151.39/api/lz3nFIcGOelQlsxsDJYqrXGuxivjFmWswZ9fIGcw/lights/1/state", "{\"bri\":" + bri + "}", "", ".json");
                //System.Threading.Thread.Sleep(50);
            }

            return webResponse;
        }

        internal async Task<bool> UnLink(string userID)
        {
            return await DeleteUser(userId);
        }
    }

    public class SwitchSetting
    {
        private double top = 0;
        private double left = 0;
        private double height = 120;
        private double width = 160;
        private string displayName = string.Empty;
        private bool visemeAttached = false;
        private float min = 0;
        private float max = 254;
        private string id = string.Empty;

        public SwitchSetting()
        { }

        public SwitchSetting(SwitchView sView, Light lSwitch)
        {
            this.DisplayName = lSwitch.Name;
            this.Id = lSwitch.Id;
            this.Left = sView.Left;
            this.Top = sView.Top;
            this.width = sView.Width;
            this.height = sView.Height;
        }

        public SwitchSetting(string title, double left, double top, string id)
        {
            this.DisplayName = title;
            this.Left = left;
            this.Top = top;
            this.Id = id;
        }

        [XmlAttribute("Id", DataType = "string")]
        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        [XmlAttribute("Name", DataType = "string")]
        public string DisplayName
        {
            get
            {
                return displayName;
            }

            set
            {
                displayName = value;
            }
        }

        [XmlAttribute("Left", DataType = "double")]
        public double Left
        {
            get
            {
                return left;
            }

            set
            {
                left = value;
            }
        }

        [XmlAttribute("Top", DataType = "double")]
        public double Top
        {
            get
            {
                return top;
            }

            set
            {
                top = value;
            }
        }

        [XmlAttribute("VisemeAttached", DataType = "boolean")]
        public bool VisemeAttached
        {
            get
            {
                return visemeAttached;
            }

            set
            {
                visemeAttached = value;
            }
        }

        [XmlAttribute("Min", DataType = "float")]
        public float Min
        {
            get
            {
                return min;
            }

            set
            {
                min = value;
            }
        }

        [XmlAttribute("Max", DataType = "float")]
        public float Max
        {
            get
            {
                return max;
            }

            set
            {
                max = value;
            }
        }

        [XmlAttribute("Height", DataType = "double")]
        public double Height
        {
            get
            {
                return height;
            }

            set
            {
                height = value;
            }
        }

        [XmlAttribute("Width", DataType = "double")]
        public double Width
        {
            get
            {
                return width;
            }

            set
            {
                width = value;
            }
        }
    }

    [XmlRoot("Config")]
    [XmlInclude(typeof(Bridge))]
    public class Configuration
    {
        //private XmlDataProvider config_DataProvider = null;

        [XmlIgnore]
        private string config_xml = Global.AppVariables.CurrentPluginDirectory + "Config.xml";

        [XmlIgnore]
        public bool IsLoaded = false;

        //public XmlDataProvider Config_DataProvider
        //{
        //    get
        //    {
        //        if (config_DataProvider == null)
        //        {
        //            config_DataProvider = new XmlDataProvider();
        //            config_DataProvider.Source = new Uri(config_xml);
        //        }
        //        return config_DataProvider;
        //    }
        //}
        //private System.Net.IPAddress ip;

        private List<Bridge> bridges = null;

        [XmlArray("Bridges")]
        [XmlArrayItem("Bridge")]
        public List<Bridge> Bridges
        {
            get
            {
                return bridges;
            }

            set
            {
                bridges = value;
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
                Bridges = new List<Bridge>();
            }
        }

        private void LoadConfig()
        {
            // Construct an instance of the XmlSerializer with the type of object that is being deserialized.
            XmlSerializer serializer = new XmlSerializer(this.GetType());

            // Read the file, create a FileStream.
            using (FileStream fileStream = new FileStream(config_xml, FileMode.Open))
            {
                Bridges = ((Configuration)serializer.Deserialize(fileStream)).Bridges;
            }
            
            IsLoaded = true;
        }

        public void Save()
        {
            // Construct an instance of the XmlSerializer with the type of object that is being deserialized.
            XmlSerializer serializer = new XmlSerializer(this.GetType());

            // Write to a file, create a StreamWriter object.
            using (StreamWriter streamWriter = new StreamWriter(config_xml))
            {
                serializer.Serialize(streamWriter, this);
            }
        }

        public void AddBridge(string name, string ip, string userid)
        {
            Bridges.Add(new Bridge(name, ip, userid, bridges.Count + 1));
        }

        public void AddBridge(Bridge bridge)
        {
            Bridges.Add(bridge);

            if (Bridges.Count == 1)
            {
                SetAsDefault(bridge.Name);
            }
        }

        public void SetAsDefault (string name)
        {
            Bridge defaultBridge = Bridges.Find(b => b.IsDefault == true);
            if (defaultBridge != null)
            {
                defaultBridge.IsDefault = false;
            }

            Bridges.Find(b => b.Name == name).IsDefault = true;
        }

        public void RemoveBridge(string name)
        {
            Bridges.Remove(Bridges.Find(b => b.Name == name));
        }

        public bool BridgeExists(string name)
        {
            return Bridges.Find(b => b.Name == name) == null ? false : true;
        }

        public string GetBridgeID(string name)
        {
            return Bridges.Find(b => b.Name == name).UserId;
        }

        public Bridge SelectBridge(string name)
        {
            return Bridges.Find(b => b.Name == name);
        }

        [XmlIgnore]
        public Bridge DefaultBridge
        {
            get
            {
                return Bridges.Find(b => b.IsDefault == true);
            }
        }
    }
    
}
