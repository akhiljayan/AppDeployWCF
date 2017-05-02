using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ApplicationDeployToolsWCF
{
    public class Service1 : IService1
    {
        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public string CreateAppVir(string site, string pool, string name, string virtualpath, string appvir)
        {
            ServerManager serverManager = new ServerManager();
            Site s = null;
            ApplicationPool ap = null;
            foreach (Site mainsite in serverManager.Sites)
            {
                if (mainsite.Name.ToString() == site)
                {
                    s = mainsite;
                }
            }
            foreach (ApplicationPool appPool in serverManager.ApplicationPools)
            {
                if (appPool.Name.ToString() == pool)
                {
                    ap = appPool;
                }
            }
            Application rootApp = s.Applications.First(a => a.Path == "/");
            rootApp.VirtualDirectories.Add("/" + name, @"" + virtualpath);
            if (appvir == "app")
            {
                rootApp.ApplicationPoolName = ap.Name.ToString();
            }
            serverManager.CommitChanges();

            string path = @"" + virtualpath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return "OK";
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public string CreateNewPool(string name, string netclr, string pipeline, bool startflag)
        {
            ManagedPipelineMode mode = ManagedPipelineMode.Classic; ;
            ServerManager serverManager = new ServerManager();
            ApplicationPool newPool = serverManager.ApplicationPools.Add(name);
            newPool.ManagedRuntimeVersion = netclr;
            if (pipeline == "Integrated")
            {
                mode = ManagedPipelineMode.Integrated;
            }
            else if (pipeline == "Clasic")
            {
                mode = ManagedPipelineMode.Classic;
            }
            newPool.ManagedPipelineMode = mode;
            newPool.Enable32BitAppOnWin64 = startflag;
            serverManager.CommitChanges();
            return "OK";
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public List<string> GetAllPools()
        {
            ServerManager serverManager = new ServerManager();
            List<string> listPools = new List<string>();
            foreach (ApplicationPool appPool in serverManager.ApplicationPools)
            {
                string poolName = appPool.Name.ToString();
                string identityType = appPool.ProcessModel.IdentityType.ToString();
                listPools.Add(poolName);
            }

            return listPools;
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public List<string> GetAllSites()
        {
            ServerManager serverManager = new ServerManager();
            List<string> listSites = new List<string>();
            foreach (Site site in serverManager.Sites)
            {
                string siteName = site.Name.ToString();
                listSites.Add(siteName);
            }
            return listSites;
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public List<string> GetAppBySite(string site)
        {
            ServerManager serverManager = new ServerManager();
            List<string> listApps = new List<string>();
            foreach (Site mainsite in serverManager.Sites)
            {
                if (mainsite.Name.ToString() == site)
                {
                    foreach (var item in mainsite.Applications)
                    {
                        string appName = item.Path.Substring(1);
                        listApps.Add(appName);
                    }
                }
            }
            return listApps;
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public string GetAppVirtualPath(string app, string site)
        {
            string path = "";
            ServerManager serverManager = new ServerManager();
            foreach (Site insite in serverManager.Sites)
            {
                if (insite.Name.ToString() == site)
                {
                    foreach (var item in insite.Applications)
                    {
                        if (item.Path.Substring(1) == app)
                        {
                            path = item.VirtualDirectories["/"].PhysicalPath;
                        }
                    }
                    serverManager.CommitChanges();
                }
            }
            return path;
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public string MoveChangeAppPath(string app, string path, string site)
        {
            //string rootdir = ConfigurationManager.AppSettings["rootDirectory"];
            //string sourceFile = @""+rootdir+"\\"+ app;
            string src = this.GetAppVirtualPath(app, site);
            string sourceFile = @"" + src.Replace("%SystemDrive%", "C:");

            string destinationFile = @"" + path;
            Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(sourceFile, destinationFile, true);


            ServerManager serverManager = new ServerManager();
            foreach (Site insite in serverManager.Sites)
            {
                if (insite.Name.ToString() == site)
                {
                    foreach (var item in insite.Applications)
                    {
                        if (item.Path.Substring(1) == app)
                        {
                            item.VirtualDirectories["/"].PhysicalPath = @"" + path;
                        }
                    }
                    serverManager.CommitChanges();
                }
            }

            return "OK";
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public string SetAppPool(string appPool, string app, string site)
        {
            ServerManager serverManager = new ServerManager();
            ApplicationPool resultpool = null;

            foreach (ApplicationPool pool in serverManager.ApplicationPools)
            {
                if (pool.Name.ToString() == appPool)
                {
                    resultpool = pool;
                }
            }

            foreach (Site insite in serverManager.Sites)
            {
                if (insite.Name.ToString() == site)
                {
                    //insite.Stop();
                    foreach (var item in insite.Applications)
                    {
                        if (item.Path.Substring(1) == app)
                        {
                            item.ApplicationPoolName = resultpool.Name;
                        }
                    }
                    serverManager.CommitChanges();
                    //insite.Start();
                    resultpool.Recycle();
                }
            }

            //foreach (Site mainsite in serverManager.Sites)
            //{
            //    foreach (var item in mainsite.Applications)
            //    {
            //        if (item.Path.Substring(1) == app)
            //        {
            //            item.ApplicationPoolName = resultpool.Name;
            //        }
            //    }
            //}

            return "OK";

        }
    }
}
