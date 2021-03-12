using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace WindowsService
{
    /// <summary>
    /// Class to process the information of the climate
    /// </summary>
    public sealed class process
    {
        /// <summary>
        /// Private variable for create the instance of the class
        /// </summary>
        private readonly static process _instance = new process();

        /// <summary>
        /// Variable for create the instance of the class
        /// </summary>
        public static process Instance { get { return _instance; } }

        /// <summary>
        /// Obtain the information of the climate of dallas texas
        /// </summary>
        /// <returns>An object with the information of the climate</returns>
        public data obtainData()
        {
            data objReturn = new data();

            var url = "https://forecast.weather.gov/MapClick.php?&lat=32.78&lon=-96.8&FcstType=json"; //dallas - texas
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.UserAgent = "api.weather.gov";

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return objReturn;

                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            string responseBody = objReader.ReadToEnd();
                            //var myUser = JsonConvert.DeserializeObject<data>(responseBody);
                            //dynamic varJson = JsonConvert.DeserializeObject(responseBody);
                            var data = JObject.Parse(responseBody);

                            try
                            {
                                objReturn.temperature = Convert.ToString(data["currentobservation"]["Temp"]);
                                objReturn.precipitation = Convert.ToString(data["currentobservation"]["Relh"]);

                            }
                            catch (Exception Ex)
                            {

                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                objReturn = new data();
            }

            return objReturn;
        }

        /// <summary>
        /// Create a CSV file with the data of the climate of dallas texas
        /// </summary>
        /// <param name="pData">Object with the information of the climate to write on the file</param>
        public void writeFile(data pData)
        {
            string strPath = @"C:\Climate";
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }


            string strFileName = "Data" + DateTime.Now.ToString("HHmmss") + ".csv";
            string strSeperator = ",";
            StringBuilder sbOutput = new StringBuilder();

            sbOutput.AppendLine(string.Format("Temperature {0} Units {0} Precipitation", strSeperator));
            sbOutput.AppendLine(string.Format("{1} {0} F {0} {2}", strSeperator, pData.temperature, pData.precipitation));

            File.WriteAllText(strPath + "\\" + strFileName, sbOutput.ToString());
        }
    }
}