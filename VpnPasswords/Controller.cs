using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace VpnPasswords
{
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
            var site = GetReadLineThrowOnExit();

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
                Console.WriteLine($"CiscoUrl: {resultItem1.CiscoUrl} Copied to clipboard");
                if (GetReadLineThrowOnExit() != string.Empty)
                    return;

                Clipboard.SetText($"{resultItem1.CiscoUserName}");
                Console.WriteLine($"Username: {resultItem1.CiscoUserName} Copied to clipboard");
                if (GetReadLineThrowOnExit() != string.Empty)
                    return;

                Clipboard.SetText($"{resultItem1.CiscoPassword}");
                Console.WriteLine($"Password: {resultItem1.CiscoPassword} Copied to clipboard");
                if (GetReadLineThrowOnExit() != string.Empty)
                    return;

            }
        }

        private string GetReadLineThrowOnExit()
        {
            var readLine = Console.ReadLine();
            if (readLine?.Equals("exit", StringComparison.CurrentCultureIgnoreCase) == true)
            {
                throw new ExitException();
            }

            return readLine;
        }
        

        private Tuple<CiscoSite, Check> TrySelectSite(string input, IList<CiscoSite> sites)
        {
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
}