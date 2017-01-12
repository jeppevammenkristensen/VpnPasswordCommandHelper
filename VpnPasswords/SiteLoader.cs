using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VpnPasswords
{
    public class SiteLoader
    {
        public IList<CiscoSite> Load()
        {
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            try
            {
                return JsonConvert.DeserializeObject<List<CiscoSite>>(File.ReadAllText("configuration.json"), jsonSerializerSettings);
            }
            catch (Exception)
            {
                return GenerateDummyConfigurationDataAndOutput(jsonSerializerSettings);
            }
            
        }

        private static List<CiscoSite> GenerateDummyConfigurationDataAndOutput(JsonSerializerSettings jsonSerializerSettings)
        {
            var result = new List<CiscoSite>
            {
                new CiscoSite()
                {
                    Customer = "DummyCustomer",
                    CiscoUrl = "DummyUrl",
                    CiscoUserName = "DummyUserName",
                    CiscoPassword = "DummyPassword"
                }
            };

            var text = JsonConvert.SerializeObject(result, jsonSerializerSettings);

            File.WriteAllText("configuration.json", text);

            Console.WriteLine("Configurationfile was not ready but generated it for you (in the same place as the exe file)");
            return result;
        }
    }
}