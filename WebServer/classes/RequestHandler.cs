﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.classes
{
    public class RequestHandler
    {
        public string serverPath;
        public string message404Path;
        public string message403Path;

        bool ssl;

        X509Certificate2 certificate;

        Configuration config;


        GETHandler GET;

        public RequestHandler(Configuration config)
        {
            this.config = config;
            this.serverPath = config.GetValue("root-path");
            this.message404Path = config.GetValue("404-path");
            this.message403Path = config.GetValue("403-path");

            GET = new GETHandler(serverPath, message404Path, message403Path);

            ssl = !string.IsNullOrEmpty(config.GetValue("certificate-path"));

            if(ssl)
            {
                certificate = new X509Certificate2(config.GetValue("certificate-path"), config.GetValue("certificate-passphrase"));
            }
        }

        public async Task HandleRequest(TcpClient client)
        {
            DateTime startHandle = DateTime.Now;

            Stream stream = client.GetStream();

            if(ssl)
            {
                SslStream sslStream = new SslStream(stream, false);

                bool authSucces = await SslAuth(sslStream);

                stream = sslStream;

                if (!authSucces)
                {
                    EndAllComunication(stream, client);
                }
            }

            try
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                bool readRequest = true;


                // later change for detecting cookies and etc...
                string httpRequest = "";
                while (readRequest)
                {
                    var read = reader.ReadLine();
                    httpRequest += read + "\n";

                    if (read.Contains("Accept:"))
                    {
                        readRequest = false;
                    }
                }


                string[] request = httpRequest.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (request[0] == "GET")
                {
                    await GET.HandleRequest(stream, request[1]);
                }

                EndAllComunication(stream, client);

                Console.WriteLine($"Handling took: {DateTime.Now - startHandle} For request: {request[1]}");
            }
            catch (Exception e) 
            {
                Console.WriteLine("Processing request failed: " + e.Message);
            }
        }

        void EndAllComunication(Stream stream, TcpClient client)
        {
            stream.Close();
            client.Close();
            stream.Dispose();
            client.Dispose();
        }

        async Task<bool> SslAuth(SslStream sslStream) 
        {
            try
            {
                await sslStream.AuthenticateAsServerAsync(certificate, false, SslProtocols.Tls12, false).ConfigureAwait(false);

                if(!sslStream.IsEncrypted || !sslStream.IsAuthenticated)
                {
                    return false;
                }

                return true; //success
            }
            catch (Exception e)
            {
                Console.WriteLine("SSL AUTH FAILED!!! exception: " + e.Message);
                return false;
            }
        }
    }
}
