using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace LHue
{
    class LREST
    {
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        internal async Task<string> Get(string url)
        {
            return await Get(new Uri(url));
        }

        /// <summary>
        /// Runs a http Get request
        /// </summary>
        /// <param name="uri">Full URL</param>
        /// <returns>Gets response from get request</returns>
        internal async Task<string> Get(Uri uri = null)
        {
            string retVal = "";
            
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(uri);
                    //response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        retVal = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        retVal = "Error: " + response.StatusCode;
                    }
                }

            }
            catch (Exception error)
            {
                retVal = "Error: " + error.Message;
            }
            
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">URL to post at</param>
        /// <param name="contentToPost">Any content as string</param>
        /// <param name="headers">Http header in query format. I.e &headerElement1="blah 123"&headerElement2=1</param>
        /// <param name="expectedReturnedFileExtention"></param>
        /// <returns></returns>
        internal async Task<string> Post(string url, string contentToPost, string headers, string expectedReturnedFileExtention)
        {
            return await Post(new Uri(url), contentToPost, headers, expectedReturnedFileExtention);
        }

        internal async Task<string> Post(Uri uri, string contentToPost, string headers, string expectedReturnedFileExtention)
        {
            string retVal = "";
            try
            {
                MimeSharp.Mime mime = new MimeSharp.Mime();

                using (HttpClient client = new HttpClient())
                {
                    NameValueCollection headerCollection = UriExtensions.ParseQueryString(new Uri(uri.AbsoluteUri + "?" + headers));
                    string contentType = mime.Lookup(expectedReturnedFileExtention);// "application/x-www-form-urlencoded";

                    HttpContent postRequestContent = new StringContent(contentToPost, Encoding.UTF8, contentType);

                    //requestContent.Headers.Add("Authorization", "Bearer " + _access_token);

                    postRequestContent.Headers.TryAddWithoutValidation("If-None-Match", "\"doesnt-match-anything\"");
                    foreach (string h in headerCollection)
                    {
                        postRequestContent.Headers.Add(h, headerCollection[h]);
                        //Console.WriteLine(headerCollection[h]);
                    }
                    
                    HttpResponseMessage response = await client.PostAsync(uri, postRequestContent);
                    //response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        retVal = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("sent");
                    }
                    else
                    {
                        retVal = "Error: " + response.StatusCode;
                    }
                }
            }
            catch (Exception error)
            {
                retVal = "Error: " + error.Message;
            }

            return retVal;
        }

        internal async Task<string> Put(string url, string contentToPost, string headers, string expectedReturnedFileExtention)
        {
            string retVal = await Put(new Uri(url), contentToPost, headers, expectedReturnedFileExtention);
            Console.WriteLine(retVal);
            return retVal;
        }

        internal async Task<string> Put(Uri uri, string contentToPost, string headers, string expectedReturnedFileExtention)
        {
            string retVal = "";
            try
            {
                MimeSharp.Mime mime = new MimeSharp.Mime();

                using (HttpClient client = new HttpClient())
                {
                    NameValueCollection headerCollection = UriExtensions.ParseQueryString(new Uri(uri.AbsoluteUri + "?" + headers));
                    string contentType = mime.Lookup(expectedReturnedFileExtention);// "application/x-www-form-urlencoded";

                    HttpContent postRequestContent = new StringContent(contentToPost, Encoding.UTF8, contentType);

                    //requestContent.Headers.Add("Authorization", "Bearer " + _access_token);

                    postRequestContent.Headers.TryAddWithoutValidation("If-None-Match", "\"doesnt-match-anything\"");
                    foreach (string h in headerCollection)
                    {
                        postRequestContent.Headers.Add(h, headerCollection[h]);
                        //Console.WriteLine(headerCollection[h]);
                    }

                    HttpResponseMessage response = await client.PutAsync(uri, postRequestContent);
                    //response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        retVal = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("sent");
                    }
                    else
                    {
                        retVal = "Error: " + response.StatusCode;
                    }
                }
            }
            catch (Exception error)
            {
                retVal = "Error: " + error.Message;
            }

            return retVal;
        }

        async Task<string> Delete(string tokenUrl, string qString)
        {
            var client = new HttpClient();
            var queryString = UriExtensions.ParseQueryString(new Uri(qString));
            //var queryString = System.Web.HttpUtility.ParseQueryString(qString);
            //System.Net.Http.Formatting.JsonMediaTypeFormatter json = new System.Net.Http.Formatting.JsonMediaTypeFormatter();

            string contentType = "application/x-www-form-urlencoded";

            HttpContent requestContent =
                new StringContent(qString, Encoding.UTF8, contentType);

            //requestContent.Headers.Add("Authorization", "Bearer " + _access_token);
            requestContent.Headers.Add("If-None-Match", "\"doesnt-match-anything\"");

            var response = await client.PostAsync(tokenUrl, requestContent);

            if (response.IsSuccessStatusCode)
            {
                string retVal = await response.Content.ReadAsStringAsync();


                return retVal;
            }
            else
            {
                return null;
            }

        }
    }
}
