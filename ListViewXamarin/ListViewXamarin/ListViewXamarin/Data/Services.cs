﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ListViewXamarin
{
    public class DataServices
    {
        #region Fields

        private static HttpClient httpClient;
        #endregion

        #region Constructor
        public DataServices()
        {
            httpClient = new HttpClient();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Fetches data from web server and write it to the local file.
        /// </summary>
        /// <returns>Returns true, if data fetched from webserver and downloaded to the local location.</returns>
        public async Task<bool> DownloadJsonAsync()
        {
            try
            {
                var url = "https://ej2services.syncfusion.com/production/web-services/api/Orders"; //Set your REST API url here
                
                //Sends request to retrieve data from the web service for the specified Uri
                var response = await httpClient.GetAsync(url);
                using (var stream = await response.Content.ReadAsStreamAsync()) //Reads data as stream
                {
                    //Gets the path to the specified folder
                    var localFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);

                    var newpath = Path.Combine(localFolder, "Data.json"); // Combine path with the file name

                    var fileInfo = new FileInfo(newpath);
                    File.WriteAllText(newpath, String.Empty);

                    //Creates a write-only file stream
                    using (var fileStream = fileInfo.OpenWrite()) 
                    {
                        await stream.CopyToAsync(fileStream); //Reads data from the current stream and write to destination stream (fileStream)
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                System.Diagnostics.Debug.WriteLine(@"ERROR {0}", e.Message);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(@"ERROR {0}", e.Message);
                return false;
            }
            return true;
        }
        #endregion
    }
}