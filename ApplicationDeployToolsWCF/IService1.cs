using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ApplicationDeployToolsWCF
{
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        [WebGet(UriTemplate = "/sites", ResponseFormat = WebMessageFormat.Json)]
        List<string> GetAllSites();

        [OperationContract]
        [WebGet(UriTemplate = "/pools", ResponseFormat = WebMessageFormat.Json)]
        List<string> GetAllPools();

        [OperationContract]
        [WebGet(UriTemplate = "/app/{site}", ResponseFormat = WebMessageFormat.Json)]
        List<string> GetAppBySite(string site);

        [OperationContract]
        [WebGet(UriTemplate = "/addApp/{appPool}/{app}", ResponseFormat = WebMessageFormat.Json)]
        string SetAppPool(string appPool, string app, string site);

        [OperationContract]
        [WebGet(UriTemplate = "/move/change/{app}/{path}/{site}", ResponseFormat = WebMessageFormat.Json)]
        string MoveChangeAppPath(string app, string path, string site);

        [OperationContract]
        [WebGet(UriTemplate = "/getpath/{app}/{site}", ResponseFormat = WebMessageFormat.Json)]
        string GetAppVirtualPath(string app, string site);

        [OperationContract]
        [WebGet(UriTemplate = "/createPool/{name}/{netclr}/{pipeline}/{startflag}", ResponseFormat = WebMessageFormat.Json)]
        string CreateNewPool(string name, string netclr, string pipeline, bool startflag);

        [OperationContract]
        [WebGet(UriTemplate = "/createAppVir/{site}/{pool}/{name}/{virtualpath}/{appvir}", ResponseFormat = WebMessageFormat.Json)]
        string CreateAppVir(string site, string pool, string name, string virtualpath, string appvir);
    }


    [DataContract]
    public class WCFSites
    {
        string siteName;

        public static WCFSites MakeSite(string siteName)
        {
            WCFSites s = new WCFSites();
            s.siteName = siteName;
            return s;
        }

        [DataMember]
        public string SiteName
        {
            get { return siteName; }
            set { siteName = value; }
        }
    }

    [DataContract]
    public class WCFPools
    {
        string poolName;

        public static WCFPools MakePool(string poolName)
        {
            WCFPools p = new WCFPools();
            p.poolName = poolName;
            return p;
        }

        [DataMember]
        public string PoolName
        {
            get { return poolName; }
            set { poolName = value; }
        }
    }

    [DataContract]
    public class WCFSiteApp
    {
        string siteAppName;

        public static WCFSiteApp MakeSiteApp(string siteAppName)
        {
            WCFSiteApp sa = new WCFSiteApp();
            sa.siteAppName = siteAppName;
            return sa;
        }

        [DataMember]
        public string SiteAppName
        {
            get { return siteAppName; }
            set { siteAppName = value; }
        }
    }
}
