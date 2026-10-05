using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MYA.Business.Puzzle;
using MYA.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MYA.Business.Mvs
{
    public sealed class MvsService
    {
        private readonly MvsApiClient _mvsApiClient;
        private readonly Utility _utility;

        private readonly MvsOptions _options;

        public MvsService(MvsApiClient mvsApiClient, IOptions<MvsOptions> options)
        {
            _mvsApiClient = mvsApiClient;
            _options = options.Value;
            _utility = new Utility(_options);
        }

        
        public async Task<string> GetSitesIdAsync(string soc, CancellationToken cancellationToken = default)
        {
            try
            {
                // 
                string currentUrlServerJson = _utility.GetCurrentMvsJsonUrl(soc);
                string currentApiKey = _utility.GetCurrentMvsJsonApiKey(soc);
                string urlApi = $"{currentUrlServerJson}:{_options.ServerPortJson}/db/sites/id";
                // 
                (string responseContent, long elapsedMilliseconds) =
                    await _mvsApiClient.GetAsync(
                        urlApi,
                        currentApiKey,
                        cancellationToken)
                    .ConfigureAwait(false);

                //_puzzleDataLayer.SP_Insert_Pz_Log_ChiamateMvs(
                //    urlApi,
                //    elapsedMilliseconds);

                return responseContent;
            }
            catch (Exception ex)
            {
                //_puzzleDataLayer.SP_InsertLogMvsCall(
                //    nameof(GetSitesIdAsync),
                //    $"soc: {soc}",
                //    ex.Message);

                throw;
            }
        }

        #region Cartelle

        public async Task<string> GetCartelleAsync(string soc, CancellationToken cancellationToken = default)
        {
            try
            {
                // 
                string currentUrlServerJson = _utility.GetCurrentMvsJsonUrl(soc);
                string currentApiKey = _utility.GetCurrentMvsJsonApiKey(soc);
                string urlApi = $"{currentUrlServerJson}:{_options.ServerPortJson}/db/table/lists";
                // 
                (string responseContent, long elapsedMilliseconds) =
                    await _mvsApiClient.GetAsync(
                        urlApi,
                        currentApiKey,
                        cancellationToken)
                    .ConfigureAwait(false);

                //_puzzleDataLayer.SP_Insert_Pz_Log_ChiamateMvs(
                //    urlApi,
                //    elapsedMilliseconds);

                return responseContent;
            }
            catch (Exception ex)
            {
                //_puzzleDataLayer.SP_InsertLogMvsCall(
                //    nameof(GetSitesIdAsync),
                //    $"soc: {soc}",
                //    ex.Message);

                throw;
            }
        }

        #endregion Cartelle

        #region Operatori

        public async Task<string> GetOperatoriAsync(string soc, CancellationToken cancellationToken = default)
        {
            try
            {
                // 
                string currentUrlServerJson = _utility.GetCurrentMvsJsonUrl(soc);
                string currentApiKey = _utility.GetCurrentMvsJsonApiKey(soc);
                string urlApi = $"{currentUrlServerJson}:{_options.ServerPortJson}/db/table/operators";
                // 
                (string responseContent, long elapsedMilliseconds) =
                    await _mvsApiClient.GetAsync(
                        urlApi,
                        currentApiKey,
                        cancellationToken)
                    .ConfigureAwait(false);

                //_puzzleDataLayer.SP_Insert_Pz_Log_ChiamateMvs(
                //    urlApi,
                //    elapsedMilliseconds);

                return responseContent;
            }
            catch (Exception ex)
            {
                //_puzzleDataLayer.SP_InsertLogMvsCall(
                //    nameof(GetSitesIdAsync),
                //    $"soc: {soc}",
                //    ex.Message);

                throw;
            }
        }

        #endregion Operatori
    }
}
