using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static string Time;
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
            // 1) Make server object on port 1000
            // 2) Start Server
            /*===  DONE ===*/
            Server server = new Server(1000, "redirectionRules.txt");
            server.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
            /*===  DONE ===*/
            string[] rules = {
                "aboutus.html,aboutus2.html",
                
                
            };
            string path = "redirectionRules.txt";
            Console.WriteLine("Loading redirection rules...\n");
            try
            {
                using (StreamWriter sw = (File.Exists(path)) ? new StreamWriter(path, false) : File.CreateText(path))
                {
                    foreach(string rule in rules)
                    {
                        Program.Time = DateTime.Now.ToString();
                        sw.WriteLine(rule);
                        Console.WriteLine($"[ {Time} ] Added a new Rule : {rule}");
                    }
                }
                Console.WriteLine("\nRules loaded succesfully.\n");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"\nError while loading the rules please check the log file.");
                Logger.LogException(ex);
            }
        }
         
    }
}
