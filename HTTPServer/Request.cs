using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string [] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {

            //TODO: parse the receivedRequest using the \r\n delimeter   
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            // Parse Request line
            // Validate blank line exists
            // Load header lines into HeaderLines dictionary
            var req = this.loadLines() & this.ParseRequestLine() & this.ValidateBlankLine() & this.LoadHeaderLines();

            return (this.method == RequestMethod.POST) ? req & readContent() : req;
        }

        private bool loadLines()
        {
            string[] stringSeparators = new string[] { "\r\n" };
            this.requestLines = this.requestString.Split(stringSeparators, StringSplitOptions.None);
            return (this.requestLines.Length >= 3);
        }

        private bool ParseRequestLine()
        {
            try
            {
                string[] parts = this.requestLines[0].Split(' ');
                /*================= METHOD =================*/
                this.method = (parts[0].ToUpper() == "GET") ? RequestMethod.GET :
                                (parts[0].ToUpper() == "POST") ? RequestMethod.POST : RequestMethod.HEAD;
                /*=================  URI  ==================*/
                this.relativeURI = (this.ValidateIsURI(parts[1])) ? parts[1] : "/";

                /*================= HTTP VER ===============*/
                this.httpVersion = (parts[2] == "HTTP/1.0") ? HTTPVersion.HTTP10 :
                                   (parts[2] == "HTTP/1.1") ? HTTPVersion.HTTP11 : HTTPVersion.HTTP09;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error while parsing request line please check the log file.");
                Logger.LogException(ex);
                return false;
            }
            return true;
        }



        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            try
            {
                this.headerLines = new Dictionary<string, string>();
                foreach(var line in this.requestLines.Skip(1))
                {
                    if (line == "" || line.Length < 3 || line == "/") break;
                    string[] stringSeparators = new string[] { ": " };
                    string[] headerLine = line.Split(stringSeparators,StringSplitOptions.None);
                    this.headerLines.Add(headerLine[0], headerLine[1]);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error while reading header lines please check the log file.");
                Logger.LogException(ex);
                return false;
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            return (this.requestLines[this.requestLines.Length - 2] == "");
        }

        private bool readContent()
        {
            try
            {
                this.contentLines = this.requestLines[this.requestLines.Length - 1].Split();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error while reading header content please check the log file.");
                Logger.LogException(ex);
                return false;
            }
        }



    }
}
