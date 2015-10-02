using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace RealTimeStatsTest.Models.Ext
{
    class ResponseHandler
    {
        public readonly static string[] URL_LIST =
        {
            //this may change, so moving this to a file might be a good idea
            //or just pull from all these and merge the server list results =D
            "eldewrito.red-m.net/list",
            "samantha-master.halo.click/list"

            

        };

        public const int timeout = 30000; //30 sec timeout, why not
        public const string HTTP_PROTOCOL = "http://";
        public const string USER_AGENT = @"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
        public static string getJSONResponse(string url)
        {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(HTTP_PROTOCOL+url);
            req.UserAgent = USER_AGENT;
            req.Timeout = timeout;
            req.ContentType = "application/json; charset=utf-8";
            //needed to explicitly send Connection: keep-alive on every request rather than just the first! 
            req.ProtocolVersion = HttpVersion.Version10;
            //req.Connection = "keep-alive";

            string textBuffer = "";

            try
            {
                var response = (HttpWebResponse) req.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    textBuffer = sr.ReadToEnd();
                }
                return textBuffer;
            }
            catch (WebException we)
            {
                Debug.WriteLine("Error from Master List {0} Message: ", url,we.Message);
                return "";
            }
            catch (AggregateException ae)
            {
                ae.Flatten();
                
                    //Debug.WriteLine("{0}: {1}", url, e.Message);
                
                //Debug.WriteLine(url + ":" + se.Message);
                return "";
            }
        }

        public static string GetMasterList()
        {
            string res = "";
            foreach (string s in URL_LIST)
            {
                res = getJSONResponse(s);
                if (!String.IsNullOrEmpty(res))
                {
                    //return first one to get a response
                    //ideally we should get a mash of all servers in cases of differing results
                    return res;

                }
            }
            return res;
        }

        public static AnnouncedListObj GetALOWithoutServerInfo(double timeoutInMS)
        {
            string jsonRes = GetMasterList();
            return DeserializeResp(jsonRes);
        }

        public static ServerInfo GetServerInfoResponse(string server)
        {
            string jsonRes = getJSONResponse(server);
            return DeserializeServerInfo(jsonRes);
        }
        public static AnnouncedListObj GetAnnounceObjectAuto(double timeoutInMS)
        {
            string jsonRes = GetMasterList();
            AnnouncedListObj alo = DeserializeResp(jsonRes);
            if (alo != null)
            {
                alo.result.serverInfos = GetResAndDesAsync(alo.result.servers, timeoutInMS);
            }
            else
            {
                Debug.WriteLine("Timed out or no response");
            }
            return alo;
        }
        public static Dictionary<string, ServerInfo> GetResAndDesAsync(IEnumerable<string> urls, double timeoutInMS)
        {
            ConcurrentDictionary<string, ServerInfo> serverInfosD = new ConcurrentDictionary<string, ServerInfo>();
            if (urls != null)
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMilliseconds(timeoutInMS);
                    //do it all in parallel so we dont have to wait on eachother
                    //wait time becomes max specified timeout, typically 1000 - 3000 ms
                    Task.WaitAll(urls.Select(url =>
                    {
                        return client.GetAsync(HTTP_PROTOCOL + url).ContinueWith(response =>
                        {

                            var resp = "";
                            try
                            {
                                resp = response.Result.Content.ReadAsStringAsync().Result;
                            }
                            catch (AggregateException ae)
                            {
                                //?
                            }
                    

                            var si = new ServerInfo();
                            if (!String.IsNullOrEmpty(resp))
                            {
                                si = JsonConvert.DeserializeObject<ServerInfo>(resp);
                               // Debug.WriteLine("{0} -> {1}", url, resp);
                            }

                            //var content = JsonConvert.DeserializeObject<IEnumerable<ServerInfo>>(resp
                            serverInfosD.AddOrUpdate(url, si, (key, existingValue) => si);
                        });
                    }).ToArray());
                }
            }
            return serverInfosD.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static AnnouncedListObj DeserializeResp(string json)
        {
            AnnouncedListObj alo = new AnnouncedListObj();
            alo = JsonConvert.DeserializeObject<AnnouncedListObj>(json);
            return alo;
        }

        public static ServerInfo DeserializeServerInfo(string json)
        {
            ServerInfo si = new ServerInfo();
            si = JsonConvert.DeserializeObject<ServerInfo>(json);
            return si;
        }
    }
}
