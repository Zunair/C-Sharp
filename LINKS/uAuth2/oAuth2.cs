using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;
using System.Xml.XPath;

namespace uAuth2
{
    public class oAuth2
    {
        string _appdata = null;
        internal string _app_name = null;
        internal string _error = null;
        internal bool _reAuthorize = false;
        string _file_path = null;
        string _xmlFilePath = null;
        string _jsonFilePath = null;
        protected string _client_id = null;
        protected string _client_secret = null;
        protected string _refresh_token = null;
        internal JObject _jObject = null;
        WPFWindow _window;

        public oAuth2(WPFWindow window, string AppName, string ClientID = null, string ClientSecret = null)
        {
            _window = window;
            _appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _file_path = _appdata + @"\LINKS\Customization\Plugins\oAuth2\";
            _app_name = AppName;
            _client_id = ClientID;
            _client_secret = ClientSecret;
            _xmlFilePath = _file_path + _app_name + ".xml";
            _jsonFilePath = _file_path + _app_name + ".json";
            _refresh_token = getElementFromXML("refresh_token");

            if (!Directory.Exists(_file_path))
            {
                Directory.CreateDirectory(_file_path);
            }
        }

        internal string getElementFromXML(string elementName)
        {
            string retVal = null;

            // Check if XML exists with same AppName
            if (File.Exists(_xmlFilePath))
            {
                try
                {
                    // Open XML file
                    XPathDocument doc = new XPathDocument(_xmlFilePath);
                    XPathNavigator nav = doc.CreateNavigator();

                    // Compile a standard XPath expression
                    XPathExpression expr = nav.Compile("/root/" + elementName);
                    XPathNodeIterator iterator = nav.Select(expr);

                    // Iterate on the node set
                    while (iterator.MoveNext())
                    {
                        retVal = iterator.Current.Value;
                    }
                }
                catch (Exception e)
                {
                    //writeLog(e);
                    Console.WriteLine(e.Message);
                }
            }

            return retVal;
        }

        public string getAccessToken(ref string elementName)
        {
            string retVal = null;
            string reAuthorize = null;
            bool refreshAccessToken = false;

            if (elementName == null || elementName == "")
            {
                elementName = "access_token";
            }

            reAuthorize = getElementFromXML("reauthorize");
            if (reAuthorize == null || reAuthorize == "false")
            {
                string jsonData = File.ReadAllText(_jsonFilePath);
                _jObject = (JObject)JsonConvert.DeserializeObject(jsonData);
                reAuthorize = _jObject["reauthorize"].ToString();
                
            }

            if (reAuthorize == null || reAuthorize.ToLower() == "false")
            {
                _reAuthorize = false;

                // Look for app json file
                if (File.Exists(_jsonFilePath))
                {
                    // Get access_token from XML if it's not expired
                    if (elementName.ToUpper() == "access_token".ToUpper())
                    {
                        int seconds = -1;
                        DateTime dt = DateTime.Now;

                        if (getElementFromXML("access_token_retrived_time") != null)
                        {
                            dt = DateTime.Parse(getElementFromXML("access_token_retrived_time"));
                            int.TryParse(getElementFromXML("access_token_expires_in"), out seconds);
                        }

                        if (seconds == -1 || dt.AddSeconds(seconds) >= DateTime.Now.AddSeconds(-60))
                        {
                            // TODO: Double check
                            //https://www.googleapis.com/oauth2/v1/tokeninfo?access_token=
                            retVal = getElementFromXML(elementName);
                        }
                        else
                        {
                            refreshAccessToken = true;
                        }
                    }
                    else // Get element from XML
                    {
                        retVal = getElementFromXML(elementName);
                    }
                }
                else
                {
                    _error = "App setup file was not found.\r\nPath: " + _jsonFilePath;
                }
            }
            else
            {
                _reAuthorize = true;
            }


            if (retVal == null)
            {
                // Authorize and create XML
                if (authUser(refreshAccessToken))
                {
                    // Get element from XML
                    retVal = getElementFromXML(elementName);
                }
            }

            return retVal;
        }

        private bool authUser(bool refreshAccessToken = false)
        {
            bool retVal = false;

            try
            {
                string jsonData = File.ReadAllText(_jsonFilePath);
                _jObject = (JObject)JsonConvert.DeserializeObject(jsonData);

                if (_refresh_token != null && !_reAuthorize)
                {
                    _jObject["refresh_token"] = _jObject["refresh_token"].ToString().Replace("{refresh_token}", _refresh_token);
                }

                // Assign client_Id & client_secret to JObject
                if (_client_id != null && _client_secret != null)
                {
                    _jObject["client_Id"] = _client_id;
                    _jObject["client_secret"] = _client_secret;                    
                }
                else
                {
                    _client_id = (string)_jObject["client_Id"];
                    _client_secret = (string)_jObject["client_secret"];
                }

                replaceMapping(ref _jObject, _jObject);

                //AuthWindow authWindow = new AuthWindow(this);
                //_window = new WPFWindow();
                _window._oAuth2 = this;
                _window._windowTitle = _app_name.Replace("_", " ");
                _window._jObject = _jObject;
                //_window.Show();
                if (refreshAccessToken)
                {
                    _window.loadAuthUrl(refreshAccessToken).Wait();
                    _window.Close();
                }
                else
                {
                    _window.loadAuthUrl(false).Wait(-1);
                    _window.Closed += AuthWindow_Closed;
                    _window.Closed += (s, e) => Dispatcher.ExitAllFrames();
                    Dispatcher.Run();
                }
                

                if (_jObject != null)
                {
                    _jObject.Remove("client_Id");
                    _jObject.Remove("client_secret");
                    _jObject.Remove("redirect_uri");
                    _jObject.Remove("auth_code");
                    _jObject.Remove("authorize_url");
                    _jObject.Remove("authorize_query");
                    _jObject.Remove("reauthorize_url");
                    _jObject.Remove("reauthorize_query");
                    _jObject.Remove("access_token_url");
                    _jObject.Remove("access_token_query");
                    _jObject.Remove("refresh_token_url");
                    _jObject.Remove("refresh_token_query");
                    _jObject["reauthorize"] = false;

                    string jString = _jObject.ToString();

                    jString = "\"root\":" + jString;
                    jString = @"{" +
                          "'?xml': {" +
                          "'@version': '1.0'," +
                          "'@standalone': 'no'" +
                          "}," +
                          jString +
                          "}";
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jString);
                    doc.Save(_xmlFilePath);

                    //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(doc.GetType());
                    //using (StringWriter textWriter = new StringWriter())
                    //{
                    //    x.Serialize(textWriter, doc);

                    //    if (_xmlFilePath != string.Empty)
                    //    {
                    //        XmlDocument doc = new XmlDocument();
                    //        doc.LoadXml(content);
                    //        doc.Save(SaveAs);
                    //    }
                    //}

                    retVal = true;
                }
            }
            catch (Exception e)
            {
                retVal = false;
                //writeLog(e);
                Console.WriteLine(e.Message);
            }
            return retVal;
        }

        private void AuthWindow_Closed(object sender, EventArgs e)
        {
            _jObject = ((WPFWindow)sender)._jObject; //GetJResponse();
        }

        public void replaceMapping(ref JObject jObjectFixed, JObject jObjectQuery)
        {
            // Replace key values with other key values
            for (int i = 0; i < jObjectFixed.Count; i++)
            {
                string path = jObjectFixed.Children().ElementAt(i).Path;

                string pattern = @"(((?'Open'\{)[^\{\}]*)+((?'Close-Open'\})[^\{\}]*)+)*(?(Open)(?!))$"; // Finds {*}
                Match m = Regex.Match(jObjectFixed[path].ToString(), pattern);
                Group grp = m.Groups[m.Groups.Count - 1];
                foreach (Capture cap in grp.Captures)
                {
                    if (jObjectQuery.Children().Any(t => t.Path == cap.Value))
                    {
                        jObjectFixed[path] = jObjectFixed[path].ToString().Replace("{" + cap.Value + "}", jObjectQuery[cap.Value].ToString());
                        if (!jObjectFixed[path].ToString().Contains("{" + cap.Value + "}") && jObjectFixed.Children().Any(t => t.Path == cap.Value + "_retrived_time"))
                        {
                            jObjectFixed[path + "_retrived_time"] = jObjectFixed[path + "_retrived_time"].ToString().Replace("{ret_time()}", DateTime.Now.ToString());
                        }
                    }
                }
            }
        }
    }
}
