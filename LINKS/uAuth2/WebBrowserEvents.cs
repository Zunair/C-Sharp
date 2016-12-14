using mshtml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.XPath;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


namespace uAuth2
{
    public partial class WPFWindow : Window
    {
        internal oAuth2 _oAuth2;
        internal JObject _jObject = null;
        bool _retrieved = false;

        private void webBrowser_Initialized(object sender, EventArgs e)
        {
            //loadAuthUrl();
            //System.Windows.Interop.ComponentDispatcher.PopModal();            
            ((WebBrowser)sender).InvalidateVisual();
        }

        public async Task<bool> loadAuthUrl(bool refreshAccessToken)
        {
            Uri authUrl = null;
            System.Threading.Thread.Sleep(500);

            if (this._oAuth2._reAuthorize)
            {
                authUrl = new Uri(
                    _jObject["reauthorize_url"].ToString() + "?" +
                    _jObject["reauthorize_query"].ToString());
            }
            else if (refreshAccessToken)
            {
                //authUrl = new Uri(
                //    _jObject["refresh_token_url"].ToString() + "?" +
                //    _jObject["refresh_token_query"].ToString());
                string reply = await MakeRequest(
                        _jObject["refresh_token_url"].ToString(),
                        _jObject["refresh_token_query"].ToString());
                if (reply == null)
                {
                    _jObject = null;
                }
                else
                {
                    _retrieved = true;
                }
                
                //Fix this!
                //this.Close();
            }
            else
            {
                authUrl = new Uri(
                   _jObject["authorize_url"].ToString() + "?" +
                   _jObject["authorize_query"].ToString());
            }

            if (!refreshAccessToken)
            {
                if (authUrl.AbsoluteUri.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    useChrome(authUrl);
                }
                else
                {
                    webBrowser.Source = authUrl;
                }
            }

            return _retrieved;
        }

        private void useChrome(Uri uri)
        {
            try
            {
                ChromeDriverService cDs = ChromeDriverService.CreateDefaultService();
                cDs.HideCommandPromptWindow = true;
                
                ChromeDriver cD = new ChromeDriver(cDs);
                
                //cD.Url = "https://ai-dot.net/getAuth?code=blah";
                cD.Navigate().GoToUrl(uri);

                // Google's search is rendered dynamically with JavaScript.
                // Wait for the page to load, timeout after 10 seconds
                //Console.Clear();
                var wait = new WebDriverWait(cD, TimeSpan.FromSeconds(160));

                try
                {
                    wait.Until(d => d.Title.StartsWith("oAuth2"));
                    webBrowser.Source = new Uri(cD.Url);
                    cD.Quit();
                    cD = null;
                    cDs = null;                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message + " - Auth Failed.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + " - Auth Failed.");
            }
        }

        private void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            WebBrowser wb = (WebBrowser)sender;
            HTMLDocument hDoc = (HTMLDocument)wb.Document;

            if (hDoc.url != null)
            {
                //<script src="http://code.jquery.com/jquery-1.9.1.js"></script>

                //<meta http-equiv="X-UA-Compatible" content="IE=edge">

                //document.getElementsByTagName('head')[0]
                //HTMLNoShowElement n = new HTMLNoShowElement();
                //n.innerText = "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">";
                //HTMLDOMAttribute a = new HTMLDOMAttribute();
                //a.value = "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">";


                //HTMLNoShowElement meta = (HTMLNoShowElement)hDoc.createElement("meta");
                //meta.attributes
                //document.getElementsByTagName('head')[0].appendChild( ... );
                //string script = "document.body.style.overflow ='hidden'";
                //wb.InvokeScript("execScript", new Object[] { script, "JavaScript" });

                //this.AllowsTransparency = false;
                //this.Opacity = 1;
                //this.Hide();

//this.WindowStyle = WindowStyle.ToolWindow;
//this.ResizeMode = ResizeMode.NoResize;

                if (hDoc.url != null && hDoc.url.StartsWith(_jObject["redirect_uri"].ToString()))
                {
                    this.Hide();
                }
                else
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                    this.Width = 600;
                    this.Height = 820;
                    this.Center();
                }

            }
        }

        private async void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            this.Hide();

            WebBrowser wb = (WebBrowser)sender;
            HTMLDocument hDoc = (HTMLDocument)wb.Document;
            //Console.WriteLine(hDoc.url);
            Title = _windowTitle + " - " + hDoc.title;

                        
            

            string auth_code = null;
            if (hDoc.url != null && hDoc.url.StartsWith(_jObject["redirect_uri"].ToString()))
            {
                if (hDoc.body != null && hDoc.body.innerText.Trim() != string.Empty)
                {
                    //Console.WriteLine(hDoc.body.innerText);
                    auth_code = hDoc.body.innerText;

                    

                    JObject responseJObject = new JObject();
                    responseJObject.Add("auth_code", auth_code.Trim());

                    _oAuth2.replaceMapping(ref _jObject, responseJObject);

                    string reply = await MakeRequest(
                        _jObject["access_token_url"].ToString(),
                        _jObject["access_token_query"].ToString());
                    if (reply == null)
                    {
                        _jObject = null;
                    }
                    else
                    {
                        _retrieved = true;
                    }

                    this.Close();
                }
            }
            else if (hDoc.url != null)
            {
                // Get Head
                HTMLHeadElement eL = (HTMLHeadElement)hDoc.getElementsByTagName("head").item();
                
                // Add IE-edge compatibility
                HTMLMetaElement m = (HTMLMetaElement)hDoc.createElement("meta");
                m.setAttribute("http-equiv", "X-UA-Compatible");
                m.setAttribute("content", "IE=edge");

                //<script src="https://code.jquery.com/jquery-1.9.1.js"></script>
                HTMLScriptElement sL = (HTMLScriptElement)hDoc.createElement("script");
                sL.setAttribute("src", "https://code.jquery.com/jquery-1.9.1.js");


                //eL.appendChild((IHTMLDOMNode)m);
                //eL.appendChild((IHTMLDOMNode)sL);

                //foreach (HTMLHeadElement eL in  hDoc.getElementsByTagName("head"))
                //{
                //    Console.WriteLine(eL.set);
                //}


                //hDoc.appendChild(m.childNodes);


                //string script = "document.getElementsByTagName('head')[0].appendChild('<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">');";
                //wb.InvokeScript("execScript", new Object[] { script, "JavaScript" });

                //this.Show();
                //if (this.Opacity == .01)
                //{
                //    this.Opacity = 1;
                //}
                //Console.WriteLine(hDoc.body.innerText);
            }
        }

        public async Task<string> MakeRequest(string tokenUrl, string qString)
        {
            var client = new System.Net.Http.HttpClient();
            var queryString = System.Web.HttpUtility.ParseQueryString(qString);

            string contentType = "application/x-www-form-urlencoded";

            System.Net.Http.HttpContent requestContent =
                new System.Net.Http.StringContent(qString, Encoding.UTF8, contentType);

            var response = await client.PostAsync(tokenUrl, requestContent);

            if (response.IsSuccessStatusCode)
            {
                string retVal = await response.Content.ReadAsStringAsync();

                JObject jResponse = (JObject)JsonConvert.DeserializeObject(retVal);
                //if (jResponse.Children().Any(t => t.Path == "access_token"))
                //{
                //    jResponse.Add("refresh_token", jResponse["access_token"]);
                //}
                _oAuth2.replaceMapping(ref _jObject, jResponse);

                retVal = "\"root\":" + retVal;
                retVal = @"{" +
                          "'?xml': {" +
                          "'@version': '1.0'," +
                          "'@standalone': 'no'" +
                          "}," +
                          retVal +
                          "}";

                XmlDocument doc = JsonConvert.DeserializeXmlNode(retVal);
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(doc.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    x.Serialize(textWriter, doc);
                    retVal = textWriter.ToString();

                    string refresh_token = "";
                    refresh_token = "";


                    doc.LoadXml(retVal);
                    XPathNavigator nav = doc.CreateNavigator();

                    // Compile a standard XPath expression
                    XPathExpression expr;
                    expr = nav.Compile("/root/" + "refresh_token");
                    XPathNodeIterator iterator = nav.Select(expr);

                    // Iterate on the node set
                    while (iterator.MoveNext())
                    {
                        refresh_token = iterator.Current.Value;
                    }
                    if (refresh_token == "")
                    {
                        if (queryString["refresh_token"] != null)
                        {
                            refresh_token = queryString["refresh_token"];
                        }
                    }
                }

                return retVal;
            }
            else
            {
                return null;
            }

        }
    }
    
}
