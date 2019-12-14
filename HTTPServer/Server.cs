using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            Console.WriteLine("Initializing the Server...");
            try
            {
                IPAddress host = IPAddress.Any;
                IPEndPoint iPEnd = new IPEndPoint(host, portNumber);
                this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.serverSocket.Bind(iPEnd);
                Console.WriteLine($"Successfuly initialized the server on PORT {portNumber}\nWating for connections...");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while initializing the server please check the log file.");
                Logger.LogException(ex);
            }
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            
            this.serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                Socket client = this.serverSocket.Accept();
                //TODO: accept connections and start thread for each accepted connection.
                Thread thread = new Thread(new ParameterizedThreadStart(this.HandleConnection));
                thread.Start(client);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket client = (Socket)obj;
            client.ReceiveTimeout = 0;
            Console.WriteLine($"Client {client.LocalEndPoint} Connected");

            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            // TODO: receive requests in while true until remote client closes the socket.
            int timeout = 10000;/*10 S*/
            DateTime startTime = System.DateTime.Now;
            DateTime endTime = System.DateTime.Now.AddMilliseconds(timeout);
            /*To discuss*/
            while (true)
            {
                try
                {
                    byte[] data = new byte[2097152];/*TEMP SIZE TILL WE DEFINE BEST SIZE 2MB*/
                    // TODO: Receive request
                    int msgLen = client.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if (msgLen == 0) break;
                    // TODO: Create a Request object using received request string
                    Request req = new Request(Encoding.ASCII.GetString(data));
                    // TODO: Call HandleRequest Method that returns the response
                    Response clientResponse = HandleRequest(req);
                    // TODO: Send Response back to client
                    client.Send(Encoding.ASCII.GetBytes(clientResponse.ResponseString));
                    
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Console.WriteLine("Error while receiving a request please check the log file");
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            /*HOW TO DO THIS??*/
            Console.WriteLine("Client Closed");
            client.Close();
        }

        Response HandleRequest(Request request)
        {
            StatusCode code = StatusCode.OK;
            string content;
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    code = StatusCode.NotFound;
                    content = this.LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, "text/html", content, "");
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string relativePath = request.relativeURI.TrimStart('/');
                if (relativePath == "") relativePath = "main.html";
                string redirectPath = this.GetRedirectionPagePathIFExist(relativePath);
                //TODO: check for redirect
                if (redirectPath != string.Empty) code = StatusCode.Redirect;
                //TODO: check file exists
                //TODO: read the physical file
                content = this.LoadDefaultPage(relativePath);
                if(content == string.Empty)
                {
                    code = StatusCode.NotFound;
                    content = this.LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(code, "text/html", content, "");
                }

                // Create OK response
                return new Response(code, "text/html", content, "aboutus2.html");

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Console.WriteLine("Error while parsing the request please check the log file");
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                code = StatusCode.NotFound;
                content = this.LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                return new Response(StatusCode.InternalServerError, "text/html", content, "");
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RedirectionRules[relativePath];
            }

            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            try
            // else read file and return its content
            {
               return File.ReadAllText(filePath);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error while loading the pages please check the log file.");
                Logger.LogException(ex);
            }
            return string.Empty;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                string[] lines = File.ReadAllLines(filePath);
                Configuration.RedirectionRules = new Dictionary<string, string>();
                foreach (string line in lines)
                {
                    string[] rule = line.Split(',');
                    Configuration.RedirectionRules.Add(rule[0], rule[1]);       
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
