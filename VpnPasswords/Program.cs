using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VpnPasswords
{
    class Program
    {
        [STAThread()]
        static void Main(string[] args)
        {
            var controller = new Controller();
            controller.Run();
        }

    }

    public class SiteLoader
    {
        public SiteLoader()
        {

        }

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

    public enum Check
    {
        Ok, 
        InvalidInput,
        Exit    
    }

    public class Controller
    {
        private readonly SiteLoader _loader = new SiteLoader();

        public  void Run()
        {
            var sites = _loader.Load();
            while (true)
            {
                CiscoSite currentSite;
                try
                {
                    currentSite = PromptForSiteSelection(sites);
                }
                catch (ExitException)
                {
                    return;
                }

                try
                {
                    while (true)
                    {
                        OutputToClipboard(currentSite);
                    }
                
                }
                catch (ExitException)
                {
                
                }
            }
        }

        public CiscoSite PromptForSiteSelection(IList<CiscoSite> sites)
        {
            int index = 0;
            foreach (var ciscoSite in sites)
            {
                Console.WriteLine($"{index++}: {ciscoSite.Customer}");
            }
            
            Console.Write("Vælg site (afslut med enter)");
            var site = Console.ReadLine();

            Tuple<CiscoSite,Check> result;
            do
            {
                result = TrySelectSite(site, sites);

            } while (result.Item2 != Check.Ok);

            return result.Item1;

        }

        private void OutputToClipboard(CiscoSite resultItem1)
        {
            while (true)
            {
                Clipboard.SetText($"{resultItem1.CiscoUrl}");
                Console.WriteLine($"CiscoUrl {resultItem1.CiscoUrl} Copied to clipboard");
                if (Console.ReadLine() != string.Empty)
                    return;

                Clipboard.SetText($"{resultItem1.CiscoUserName}");
                Console.WriteLine($"Username {resultItem1.CiscoUserName} Copied to clipboard");
                if (Console.ReadLine() != string.Empty)
                    return;

                Clipboard.SetText($"{resultItem1.CiscoPassword}");
                Console.WriteLine($"Password {resultItem1.CiscoPassword} Copied to clipboard");
                if (Console.ReadLine() != string.Empty)
                    return;

            }
        }
        
        

        private Tuple<CiscoSite, Check> TrySelectSite(string input, IList<CiscoSite> sites)
        {
            if (input.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ExitException();
            }

            uint index;
            if (!uint.TryParse(input, out index) && sites.Count <= index) {
                Console.WriteLine("Invalid input prøv igen");
                return new Tuple<CiscoSite, Check>(null, Check.InvalidInput);
            }

            return Tuple.Create(sites[(int) index], Check.Ok);
        }

        public class ExitException : Exception
        {
        }
    }

  




    

    public class CiscoSite
    {
        public string Customer { get; set; }
        public string CiscoUrl { get; set; }

        public string CiscoUserName { get; set; }

        public string CiscoPassword { get; set; }
    }
}
