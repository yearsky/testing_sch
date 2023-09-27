using System;
using Hangfire.Server;
using ExcelDataReader;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Net.WebRequestMethods;
using System.Security.Cryptography.Xml;
using Hangfire.Console;
using Hangfire;

namespace Cron_Testings.Jobs
{
	public class TestingJobs
	{
        private readonly HttpClient _httpClient;

        public TestingJobs()
		{
            _httpClient = new HttpClient();
        }

        
        public async Task RunAsync(PerformContext context)
        {
            List<string> columnNamesList = new List<string>();
            List<string> namesList = new List<string>(); // List to store values in the "name" column

            using (var stream = System.IO.File.Open("dbSiswa1.xls", FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // 1. Read the column names (headers) first
                    reader.Read();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetString(i);
                        columnNamesList.Add(columnName);
                    }

                    // Find the index of the "name" column
                    int nameColumnIndex = columnNamesList.IndexOf("NIK");

                    // 2. Loop through the rows and retrieve the values in the "name" column
                    while (reader.Read())
                    {
                        string nameValue = reader.GetString(nameColumnIndex);

                        // Check if the "name" column is empty, and skip the row if it is
                        if (!string.IsNullOrWhiteSpace(nameValue))
                        {
                            namesList.Add(nameValue);
                        }
                    }

                    // 3. Use the AsDataSet extension method if needed
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration() { UseHeaderRow = true, FilterRow = rowReader => rowReader.Depth < 1 }
                    }).Tables[0];
                }
            }

            // Now you can use the namesList to access the non-empty values in the "name" column.


            foreach (var name in namesList)
            {
                await PostNameDataAsync(name,context);
            }
        }

        private async Task PostNameDataAsync(string name,PerformContext context)
        {
            using (HttpClient httpClient = new HttpClient())
            {

                // Send the POST request
                var apiUrl = "http://localhost:8900/api/va/create";

                RequestBody requestBody = new RequestBody();

                requestBody.NIK = name;
                requestBody.Nama = "Yersky";

                string content = JsonConvert.SerializeObject(requestBody);

                StringContent httpContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, httpContent);

                Console.WriteLine(response);

                if (response.IsSuccessStatusCode)
                {
                    // Successfully posted the data
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Successfully posted data:");
                    Console.WriteLine($"Response: {responseContent}");
                    context.WriteLine($"Response: {responseContent}");
                }
                else
                {
                    // Handle the error if the POST request was not successful
                    Console.WriteLine($"Error posting data:");
                    Console.WriteLine($"Status code: {response.StatusCode}");
                }
            }

        }

        private class RequestBody
        {

            public string NIK { get; set; }
            public string Nama { get; set; }
        }

    }
}

