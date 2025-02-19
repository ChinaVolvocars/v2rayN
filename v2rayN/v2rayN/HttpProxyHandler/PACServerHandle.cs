﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using v2rayN.Mode;
using v2rayN.Properties;
using v2rayN.Tool;
using v2rayN.Base;

namespace v2rayN.HttpProxyHandler
{
    /// <summary>
    /// 提供PAC功能支持
    /// </summary>
    class PACServerHandle
    {
        private static int pacPort = 0;
        private static HttpWebServer server;
        private static HttpWebServerB serverB;

        public static bool IsRunning
        {
            get
            {
                return (pacPort > 0);
            }
        }

        public static void Init(Config config)
        {
            //if (InitServer("*"))
            //{
            //    pacPort = Global.pacPort;
            //}
            if (InitServer(Global.Loopback))
            {
                pacPort = Global.pacPort;
            }
            else if (InitServerB(Global.Loopback))
            {
                pacPort = Global.pacPort;
            }
            else
            {
                Utils.SaveLog("Webserver init failed ");
                pacPort = 0;
            }
        }

        private static bool InitServer(string address)
        {
            try
            {
                if (pacPort != Global.pacPort)
                {
                    if (server != null)
                    {
                        server.Stop();
                        server = null;
                    }

                    if (server == null)
                    {
                        string prefixes = string.Format("http://{0}:{1}/pac/", address, Global.pacPort);
                        Utils.SaveLog("Webserver prefixes " + prefixes);

                        server = new HttpWebServer(SendResponse, prefixes);
                        server.Run();

                        //pacPort = Global.pacPort;
                    }
                }
                Utils.SaveLog("Webserver at " + address);
            }
            catch (Exception ex)
            {
                Utils.SaveLog("Webserver InitServer " + ex.Message);
                return false;
            }
            return true;
        }

        public static bool InitServerB(string address)
        {
            try
            {
                if (pacPort != Global.pacPort)
                {
                    if (serverB != null)
                    {
                        serverB.Stop();
                        serverB = null;
                    }

                    if (serverB == null)
                    {
                        serverB = new HttpWebServerB(Global.pacPort, SendResponse);
                        //pacPort = Global.pacPort;
                    }
                }
                Utils.SaveLog("WebserverB at " + address);
            }
            catch (Exception ex)
            {
                Utils.SaveLog("WebserverB InitServer " + ex.Message);
                return false;
            }
            return true;
        }

        public static string SendResponse(string address)
        {
            try
            {
                var pac = GetPacList(address);
                return pac;
            }
            catch (Exception ex)
            {
                Utils.SaveLog("Webserver SendResponse " + ex.Message);
                return ex.Message;
            }
        }

        public static void Stop()
        {
            //try
            //{
            //    if (server != null)
            //    {
            //        server.Stop();
            //        server = null;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Utils.SaveLog("Webserver Stop " + ex.Message);
            //}

            //try
            //{
            //    if (httpWebServer == null)
            //    {
            //        return;
            //    }
            //    foreach (var key in httpWebServer.Keys)
            //    {
            //        Utils.SaveLog("Webserver Stop " + key.ToString());
            //        ((HttpWebServer)httpWebServer[key]).Stop();
            //    }
            //    httpWebServer.Clear();
            //}
            //catch (Exception ex)
            //{
            //    Utils.SaveLog("Webserver Stop " + ex.Message);
            //}
        }


        private static string GetPacList(string address)
        {
            var port = Global.sysAgentPort;
            if (port <= 0)
            {
                return "No port";
            }
            try
            {
                List<string> lstProxy = new List<string>();
                lstProxy.Add(string.Format("PROXY {0}:{1};", address, port));
                var proxy = string.Join("", lstProxy.ToArray());

                string strPacfile = Utils.GetPath(Global.pacFILE);
                if (!File.Exists(strPacfile))
                {
                    FileManager.UncompressFile(strPacfile, Resources.pac_txt);
                }
                var pac = File.ReadAllText(strPacfile, Encoding.UTF8);
                pac = pac.Replace("__PROXY__", proxy);
                return pac;
            }
            catch
            {
            }
            return "No pac content";
        }

    }
}
