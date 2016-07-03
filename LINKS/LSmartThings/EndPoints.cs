using MSXML;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSmartThings
{
    class EndPoints
    {
        private SmartThingsDevice[] _stDevices;
        protected string _access_token;
        protected string _baseurl;

        internal SmartThingsDevice[] StDevices
        {
            get
            {
                return _stDevices;
            }

            set
            {
                _stDevices = value;
            }
        }

        internal EndPoints(string access_token)
        {
            _access_token = access_token;
            GetEndPoints();
        }

        /// <summary>
        /// Gets the installations path, 
        /// from there you get the endpoints with /switches, /energies, /thermostats, et. etc.
        /// </summary>
        private void GetEndPoints()
        {
            XMLHTTPRequest HTTPRequest = new XMLHTTPRequest(); // u cant reinitialize the var again but not define it again.

            string request = "https://graph.api.smartthings.com/api/smartapps/endpoints";
            string responseText = "";


            HTTPRequest.open("GET", request, false); // get module status
            HTTPRequest.setRequestHeader("Authorization", "Bearer " + _access_token);
            HTTPRequest.send();
            responseText = HTTPRequest.responseText;

            EndPoint[] stEndPoint = JsonConvert.DeserializeObject<EndPoint[]>(responseText); //argh! lol oo it gets multiple  end points.. 
            Console.WriteLine(stEndPoint[0].uri.ToString()); // here is ur uri hehe
            //Debugger.Break();

            GetAllthings(stEndPoint[0].uri);

        }

        private void GetAllthings(string uri)
        {
            XMLHTTPRequest HTTPRequest = new XMLHTTPRequest(); // u can reinitialize the var again but not define it again.

            string request = uri + @"/switches";
            string responseText = "";
            
            HTTPRequest.open("Get", request, false); // get module status
            HTTPRequest.setRequestHeader("Authorization", "Bearer " + _access_token);
            HTTPRequest.setRequestHeader("If-None-Match", "\"doesnt-match-anything\"");
            //HTTPRequest.setRequestHeader("cache-control", "private");
            HTTPRequest.send();

            responseText = HTTPRequest.responseText;

            try
            {
                StDevices = JsonConvert.DeserializeObject<SmartThingsDevice[]>(responseText); //argh! lol oo it gets multiple  end points.. 
            }
            catch
            {
                ErrorObject err = JsonConvert.DeserializeObject<ErrorObject>(responseText); //argh! lol oo it gets multiple  end points.. 
                Console.WriteLine(err.message);
                //Debugger.Break();
            }
            //Console.WriteLine(_stDevices[0].id); // here is ur uri hehe

            _baseurl = uri;
            
            //Debugger.Break();
        }


        //ChangeDevStat("Living Room Kitchen Light", "on");
        internal string ChangeDevState(string SwitchName, string state)
        {
            string retVal = "";
            SmartThingsDevice stDevice = StDevices.First(d => d.name == SwitchName);
            string uri = _baseurl + @"/switchesz/" + stDevice.id + @"/" + state;
            //ChangeDevStat(uri + @"/switchesz/" + stDevice.id + @"/on");
            //ChangeDevStat(uri + @"/switchesz/" + stDevice.id + @"/off");            
            
            XMLHTTPRequest HTTPRequest = new XMLHTTPRequest(); // u cant reinitialize the var again but not define it again.

            string request = uri;

            HTTPRequest.open("GET", request, false); // get module status
            HTTPRequest.setRequestHeader("Authorization", "Bearer " + _access_token);
            HTTPRequest.setRequestHeader("If-None-Match", "\"doesnt-match-anything\"");
            HTTPRequest.send();

            retVal = HTTPRequest.responseText;
            //Console.WriteLine("retVal " + retVal);
            try
            {
                //JArray jResponse = (JArray)JsonConvert.DeserializeObject(retVal);
                JObject jResponse = (JObject)JsonConvert.DeserializeObject(retVal);
                //Console.WriteLine("jResponse " + jResponse);
                retVal = jResponse["response"].ToString();
            }
            catch(Exception e)
            {
                retVal = "Error: " + e.Message;
            }
            //stDevices = JsonConvert.DeserializeObject<SmartThingsDevice[]>(responseText); //argh! lol oo it gets multiple  end points.. 
            //Console.WriteLine(stDevices[0].id); // here is ur uri hehe

            // This error means requested POST/GET/PUT/DELETE is not allowed
            //"{\"error\":true,\"type\":\"SmartAppException\",\"message\":\"Method Not Allowed\"}"
            //Debugger.Break();

            return retVal;
        }

        async Task<string> MakeRequest(string tokenUrl, string qString)
        {
            var client = new System.Net.Http.HttpClient();
            var queryString = System.Web.HttpUtility.ParseQueryString(qString);

            string contentType = "application/x-www-form-urlencoded";

            
            System.Net.Http.HttpContent requestContent =
                new System.Net.Http.StringContent(qString, Encoding.UTF8, contentType);

            requestContent.Headers.Add("Authorization", "Bearer " + _access_token);
            requestContent.Headers.Add("If-None-Match", "\"doesnt-match-anything\"");



            var response = await client.PostAsync(tokenUrl, requestContent);

            if (response.IsSuccessStatusCode)
            {
                string retVal = await response.Content.ReadAsStringAsync();

                //JObject jResponse = (JObject)JsonConvert.DeserializeObject(retVal);
                //if (jResponse.Children().Any(t => t.Path == "access_token"))
                //{
                //    jResponse.Add("refresh_token", jResponse["access_token"]);
                //}
                //_oAuth2.replaceMapping(ref _jObject, jResponse);

                //retVal = "\"root\":" + retVal;
                //retVal = @"{" +
                //          "'?xml': {" +
                //          "'@version': '1.0'," +
                //          "'@standalone': 'no'" +
                //          "}," +
                //          retVal +
                //          "}";

                //XmlDocument doc = JsonConvert.DeserializeXmlNode(retVal);
                //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(doc.GetType());

                //using (StringWriter textWriter = new StringWriter())
                //{
                //    x.Serialize(textWriter, doc);
                //    retVal = textWriter.ToString();

                //    string refresh_token = "";
                //    refresh_token = "";


                //    doc.LoadXml(retVal);
                //    XPathNavigator nav = doc.CreateNavigator();

                //    // Compile a standard XPath expression
                //    XPathExpression expr;
                //    expr = nav.Compile("/root/" + "refresh_token");
                //    XPathNodeIterator iterator = nav.Select(expr);

                //    // Iterate on the node set
                //    while (iterator.MoveNext())
                //    {
                //        refresh_token = iterator.Current.Value;
                //    }
                //}

                return retVal;
            }
            else
            {
                return null;
            }

        }

        //get status of device by id
        string getDeviceStatus(string id)
        {
            string retVal = "-1";

            //retVal = "retrieved device statis"

            return retVal;
        }

        //get id of the device by its name
        string getDeviceID(string name)
        {
            string retVal = "-1";

            foreach (SmartThingsDevice stDev in StDevices)
            {
                if (stDev.name == name)
                {
                    retVal = stDev.id;
                    break;
                }
            }
            //retVal = "retrieved device id"

            return retVal;
        }

        //[TheSmartHouseGuy.SmartThings.changeDeiceStatus("some light","on")]
        //change device status by name to sepecified status
        void changeDeiceStatus(string name, string status)
        {
            string devId = getDeviceID(name);
            string devStatus = getDeviceStatus(devId);

            //Change status...

        }

        public class State
        {
            public string id { get; set; }
            public string hubId { get; set; }
            public bool isVirtualHub { get; set; }
            public string description { get; set; }
            public string rawDescription { get; set; }
            public bool displayed { get; set; }
            public bool isStateChange { get; set; }
            public string linkText { get; set; }
            public string date { get; set; }
            public object unixTime { get; set; }
            public string value { get; set; }
            public bool viewed { get; set; }
            public bool translatable { get; set; }
            public bool archivable { get; set; }
            public string deviceId { get; set; }
            public string name { get; set; }
            public string locationId { get; set; }
            public string groupId { get; set; }
            public string eventSource { get; set; }
            public string deviceTypeId { get; set; }
            public string data { get; set; }
        }

        public class SmartThingsDevice
        {
            public string id { get; set; }
            public string label { get; set; }
            public string name { get; set; }
            public State state { get; set; }
        }


        public class ErrorObject
        {
            public bool error { get; set; }
            public string type { get; set; }
            public string message { get; set; }
        }

        public class OauthClient
        {
            public string clientId { get; set; }
        }

        public class Location
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class EndPoint
        {
            public OauthClient oauthClient { get; set; }
            public Location location { get; set; }
            public string uri { get; set; }
            public string base_url { get; set; }
            public string url { get; set; }
        }
    }
}
