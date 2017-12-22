﻿using NBi.Core.PowerBiDesktop;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Query.Session
{
    class PowerBiDesktopSessionFactory : ISessionFactory
    {
        private readonly PowerBiDesktopConnectionStringBuilder connectionStringBuilder = new PowerBiDesktopConnectionStringBuilder();

        public PowerBiDesktopSessionFactory()
        { }

        public PowerBiDesktopSessionFactory(PowerBiDesktopConnectionStringBuilder connectionStringBuilder)
        {
            this.connectionStringBuilder = connectionStringBuilder;
        }

        public bool CanHandle(string connectionString)
        {
            return !string.IsNullOrEmpty(ParseConnectionString(connectionString));
        }

        public ISession Instantiate(string connectionString)
        {
            if (!CanHandle(connectionString))
                throw new ArgumentException();

            var csb = new DbConnectionStringBuilder() { ConnectionString = connectionString };
            connectionStringBuilder.Build(csb["pbix"].ToString());
            connectionString = connectionStringBuilder.GetConnectionString();

            return new PowerBiDesktopSession(connectionString);
        }

        private string ParseConnectionString(string connectionString)
        {
            var providerName = ExtractProviderName(connectionString);
            return providerName;
        }

        private string ExtractProviderName(string connectionString)
        {
            try
            {
                var csb = new DbConnectionStringBuilder() { ConnectionString = connectionString };

                if (csb.ContainsKey("pbix"))
                    return "Microsoft.AnalysisServices.AdomdClient";
            }
            catch (Exception) { return null; }
            return null;
        }

    }
}
