using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;

namespace WpfApp10
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> routeIdList = new List<string>();
        List<string> plateNoList = new List<string>();
        List<string> stationIdList = new List<string>();
        List<string> nodeInfoList = new List<string>();
        List<string> listArrTimeList = new List<string>();
        List<string> stationNameList = new List<string>();
        List<string> endStationInfoList = new List<string>();
        List<string> rouetNameList = new List<string>();
        List<TotalBusInfo> totalList = new List<TotalBusInfo>();
        internal class TotalBusInfo
        {
            public string RouteName { get; set; }
            public string BusArrTime { get; set; }
            public string EndStationName { get; set; }
            public string ArrStationName { get; set; }
            public string StationCount { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
            string noId = "";
            string busSta = "";
            string busArr = "";
            string busNo = "";
            string busname = "";
            string busstation = "";
            string stationName = "";
            string stationId = "";
            string routeName = "";
            string busendStationName = "";
            
            //버스 도착 정보 api
            string urlArrTime = "http://apis.data.go.kr/6410000/busarrivalservice/getBusArrivalList"; // URL
            urlArrTime += "?ServiceKey=" + "서비스키"; // Service Key
            urlArrTime += "&stationId="+"정류장 아이디"; //ex) 200000070

            var requestArrTime = (HttpWebRequest)WebRequest.Create(urlArrTime);
            requestArrTime.Method = "GET";

            string resultsArrTime = string.Empty;
            HttpWebResponse responseArrTime;
            using (responseArrTime = requestArrTime.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(responseArrTime.GetResponseStream());
                resultsArrTime = reader.ReadToEnd();
            }
            XmlDocument xmlDocArrTime = new XmlDocument();
            xmlDocArrTime.LoadXml(resultsArrTime);
            XmlNodeList nodeListArrTime = xmlDocArrTime.SelectNodes("//response/msgBody/busArrivalList");
            foreach (XmlNode node in nodeListArrTime)
            {
                noId = node.SelectSingleNode("routeId").InnerText;
                busSta = node.SelectSingleNode("locationNo1").InnerText;
                busArr = node.SelectSingleNode("predictTime1").InnerText+"분";
                busNo = node.SelectSingleNode("plateNo1").InnerText;
                routeIdList.Add(noId);
                plateNoList.Add(busNo);

                listArrTimeList.Add(busArr);
                stationNameList.Add(busSta);
            }
            for (int i = 0; i < routeIdList.Count; i++)
            {
                //버스 현재 위치 api
                string urlPosition = "http://apis.data.go.kr/6410000/buslocationservice/getBusLocationList"; // URL
                urlPosition += "?ServiceKey=" + "서비스키"; // Service Key
                urlPosition += "&routeId=" + routeIdList[i];
                var requestPosition = (HttpWebRequest)WebRequest.Create(urlPosition);
                requestPosition.Method = "GET";

                string resultsPosition = string.Empty;
                HttpWebResponse responsePosition;
                using (responsePosition = requestPosition.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(responsePosition.GetResponseStream());
                    resultsPosition = reader.ReadToEnd();
                }
                XmlDocument xmlDocPosition = new XmlDocument();
                xmlDocPosition.LoadXml(resultsPosition);
                XmlNodeList nodeListPosition = xmlDocPosition.SelectNodes("//response/msgBody/busLocationList");
                for (int j = 0; j < nodeListPosition.Count; j++)
                {
                    XmlNode nodePosition = nodeListPosition[j];
                    busname = nodePosition.SelectSingleNode("plateNo").InnerText;
                    busstation = nodePosition.SelectSingleNode("stationId").InnerText;
                    if (busname.Equals(plateNoList[i]))
                    {
                        stationIdList.Add(busstation);
                        break;
                    }
                }

                //버스 노선 정보 api
                //경유정류소목록조회
                string urlStationName = "http://apis.data.go.kr/6410000/busrouteservice/getBusRouteStationList"; // URL
                urlStationName += "?ServiceKey="+"서비스키"; // Service Key
                urlStationName += "&routeId=" + routeIdList[i];

                var requestStationName = (HttpWebRequest)WebRequest.Create(urlStationName);
                requestStationName.Method = "GET";

                string resultsStationName = string.Empty;
                HttpWebResponse responseStationName;
                using (responseStationName = requestStationName.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(responseStationName.GetResponseStream());
                    resultsStationName = reader.ReadToEnd();
                }
                XmlDocument xmlDocStationName = new XmlDocument();
                xmlDocStationName.LoadXml(resultsStationName);
                XmlNodeList nodeListStationName = xmlDocStationName.SelectNodes("//response/msgBody/busRouteStationList");
                for (int j = 0; j < nodeListStationName.Count; j++)
                {
                    XmlNode nodeStationName = nodeListStationName[j];
                    stationName = nodeStationName.SelectSingleNode("stationName").InnerText;
                    stationId = nodeStationName.SelectSingleNode("stationId").InnerText;
                    if (stationId.Equals(stationIdList[i]))
                    {
                        nodeInfoList.Add(stationName);
                        break;
                    }
                }

                //버스 노선 정보 api
                //노선정보항목조회
                string urlEndStation = "http://apis.data.go.kr/6410000/busrouteservice/getBusRouteInfoItem"; // URL
                urlEndStation += "?ServiceKey="+"서비스키"; // Service Key
                urlEndStation += "&routeId=" + routeIdList[i];

                var requestEndStation = (HttpWebRequest)WebRequest.Create(urlEndStation);
                requestEndStation.Method = "GET";

                string resultsEndStation = string.Empty;
                HttpWebResponse responseEndStation;
                using (responseEndStation = requestEndStation.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(responseEndStation.GetResponseStream());
                    resultsEndStation = reader.ReadToEnd();
                }
                XmlDocument xmlDocEndStation = new XmlDocument();
                xmlDocEndStation.LoadXml(resultsEndStation);
                XmlNodeList nodeListEndStation = xmlDocEndStation.SelectNodes("//response/msgBody/busRouteInfoItem");
                for (int j = 0; j < nodeListEndStation.Count; j++)
                {
                    XmlNode nodeEndStation = nodeListEndStation[j];
                    routeName = nodeEndStation.SelectSingleNode("routeName").InnerText;
                    busendStationName = nodeEndStation.SelectSingleNode("endStationName").InnerText;
                    endStationInfoList.Add(busendStationName);
                    rouetNameList.Add(routeName);
                }
            }
            for(int i = 0; i< nodeInfoList.Count; i++)
            {
                TotalBusInfo total = new TotalBusInfo
                {
                    RouteName = rouetNameList[i],
                    BusArrTime = listArrTimeList[i],
                    EndStationName = endStationInfoList[i],
                    ArrStationName = nodeInfoList[i],
                    StationCount = stationNameList[i],
                };
                totalList.Add(total);
            }
            BusInfoList.ItemsSource = totalList;
        }
    }
}