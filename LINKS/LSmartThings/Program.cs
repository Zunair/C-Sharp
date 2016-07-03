using MSXML;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uAuth2;

namespace LSmartThings
{
    class Program
    {
        static void Main(string[] args)
        {
            //Put("{\"on\":true}", "");

            //System.Diagnostics.Debugger.Launch();
            //System.Diagnostics.Debugger.Break();

            //LSmartThings.EndPoints stDevices = new EndPoints(It.InitAuthWindowAsync("SmartThings_MVC_LINKS"));
            //stDevices.ChangeDevStat("Living Room Kitchen Light", "on");
            //LSmartThings AppName="SmartThings_MVC_LINKS" DeviceName="Living Room Kitchen Light" DeviceStatus="on"
            //string appName = args[2];
            //string deviceName = args[0];
            //string deviceStatus = args[1];

            var parsedArgs = args
                .Select(s => s.Split(new[] { '=' }))
                .ToDictionary(s => s[0], s => s[1]);

            

            EndPoints stDevices = new EndPoints(It.InitAuthWindowAsync(parsedArgs["AppName"]));
            Console.WriteLine(stDevices.ChangeDevState(parsedArgs["DeviceName"], parsedArgs["DeviceStatus"]));

            //"Living Room Kitchen Light" "on" "SmartThings_MVC_LINKS"
        }

        //[Test]
        public static string Put(string str, string uri)
        {
            string retVal = "";

            XMLHTTPRequest HTTPRequest = new XMLHTTPRequest(); // u cant reinitialize the var again but not define it again.

            string request = uri;

            HTTPRequest.open("PUT", request, false); // get module status
            HTTPRequest.send(str);
            //HTTPRequest.setRequestHeader("Authorization", "Bearer " + _access_token);
            //HTTPRequest.setRequestHeader("If-None-Match", "\"doesnt-match-anything\"");
            //HTTPRequest.send();

            retVal = HTTPRequest.responseText;
            //Console.WriteLine("retVal " + retVal);
            try
            {
                //JArray jResponse = (JArray)JsonConvert.DeserializeObject(retVal);
                retVal = "";
            }
            catch (Exception e)
            {
                retVal = "Error: " + e.Message;
            }

            return retVal;
        }
    }
}
