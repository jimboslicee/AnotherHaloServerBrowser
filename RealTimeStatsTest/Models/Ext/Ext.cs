using RealTimeStatsTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RealTimeStatsTest.Models.Ext
{
    public static class Ext
    {
        public static int DiffCompare(List<string> oldList, List<string> newList)
        {
            int oldCount = oldList.Count;
            int newCount = newList.Count;
            if (oldCount > newCount)
            {
                //was removed
                return -1;
            }
            if (newCount > oldCount)
            {
                //something added
                return 1;
            }
             return 0;
            
        }

        public static List<string> GetCompleteDiff(List<string> lList, List<string> rList)
        {
            List<string> diffList = new List<string>();
            diffList = diffList.Concat(lList.Except(rList).ToList()).ToList();

            diffList = diffList.Concat(rList.Except(lList).ToList()).ToList();
            return diffList;
        }

        public static string CreateReadableID(string server)
        {
            string[] split = server.Split(':');
            string[] splitIP = split[0].Split('.');
            string mergedIP = "";
            foreach (string octet in splitIP)
            {
                mergedIP += octet;
            }
            return "server"+mergedIP;
        }
        public static string GetScript(string path)
        {
            path = CheckLeadingSlash(path);
            return VirtualPathUtility.ToAbsolute("~/scripts" + path);
        }

        public static string GetContent(string path)
        {
            path = CheckLeadingSlash(path);
            return VirtualPathUtility.ToAbsolute("~/Content" + path);
        }

        /// <summary>
        /// Checks for leading slash.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Returns path with leading / in front (if it's not empty and existing)</returns>
        private static string CheckLeadingSlash(string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                if (!path.StartsWith("/"))
                {
                    path = "/" + path;
                }
            }
            return path;
        }


    }
    
}

