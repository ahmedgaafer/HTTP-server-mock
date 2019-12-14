using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            // TODO: Create and return the request string
            this.responseString = this.GetStatusLine(code) + this.getHeaderLines(contentType, content.Length, redirectoinPath) + content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;

            int codeNum = (code == StatusCode.BadRequest) ? 400 :
                         (code == StatusCode.InternalServerError) ? 5000 :
                         (code == StatusCode.NotFound) ? 400 :
                         (code == StatusCode.Redirect) ? 301 : 200;
            statusLine = $"{Configuration.ServerHTTPVersion} {codeNum} {code.ToString()}\r\n";

            return statusLine;
        }

        private string getHeaderLines(string contentType,int contentLen, string redirectionPath)
        {
            string HeaderLines = $"Date: {DateTime.Now.ToString()}\r\nServer: {Configuration.ServerType}\r\nContent-Type: {contentType}\r\nContent-Length: {contentLen}\r\n";
            HeaderLines += (redirectionPath.Length > 0) ? $"Location: {"\\" + redirectionPath}\r\n\r\n" : "\r\n";
            
            return HeaderLines;
        }
    }
}
