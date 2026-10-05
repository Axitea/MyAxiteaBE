using Microsoft.Extensions.Options;
using MYA.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MYA.Business.Mvs
{
    internal class Utility
    {
        private readonly MvsOptions _options;

        public Utility(MvsOptions options)
        {
            _options = options;
        }

        public string GetCurrentMvsJsonUrl(string soc)
        {
            string url = string.Empty;

            switch (soc.ToUpper())
            {
                case "MI":
                    url = _options.UrlServerNordJson;
                    break;
                case "AN":
                    url = _options.UrlServerCentroJson;
                    break;
                default:
                    url = _options.UrlServerNordJson;
                    break;
            }

            return url;
        }

        public string GetCurrentMvsJsonApiKey(string soc)
        {
            string apyKey = string.Empty;

            switch (soc.ToUpper())
            {
                case "MI":
                    apyKey = _options.ApiKeyNord;
                    break;
                case "AN":
                    apyKey = _options.ApiKeyCentro;
                    break;
                default:
                    apyKey = _options.ApiKeyNord;
                    break;
            }

            return apyKey;
        }
    }
}
