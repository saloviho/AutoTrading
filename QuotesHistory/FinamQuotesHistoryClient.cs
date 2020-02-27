using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using QuotesHistory.Interfaces;
using QuotesHistory.Models;

namespace QuotesHistory
{
    public class FinamQuotesHistoryClient : IQuotesHistoryClient
    {
        private readonly IHistoryConfiguration configuration;

        public FinamQuotesHistoryClient(IHistoryConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        public async Task<string> GetTicksHistory(
            string ticker, 
            DateTime dateFrom, 
            DateTime dateTo, 
            CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(
                $"{configuration.FinamExportUrl}file.txt?market=1&em=19737&code={ticker}&apply=0&" +
                $"df={dateFrom.Day}&mf={dateFrom.Month - 1}&yf={dateFrom.Year}&from={dateFrom:dd.MM.yy}&" +
                $"dt={dateTo.Day}&mt={dateFrom.Month - 1}&yt={dateTo.Year}&to=2{dateTo:dd.MM.yy}&" +
                $"p=1&f=file&e=.txt&cn={ticker}&dtf=1&tmf=1&MSOR=1&mstime=on&mstimever=1&sep=1&sep2=1&datf=9", 
                cancellationToken);
            return await response.Content.ReadAsStringAsync();
        }
        
        public async Task<string> GetCandlesHistory(
            string ticker, 
            TimeInterval timeInterval, 
            DateTime dateFrom, 
            DateTime dateTo,
            CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(
                $"{configuration.FinamExportUrl}file.txt?market=1&em=16842&code={ticker}&apply=0&" +
                $"df={dateFrom.Day}&mf={dateFrom.Month - 1}&yf={dateFrom.Year}&from={dateFrom:dd.MM.yy}&" +
                $"dt={dateTo.Day}&mt={dateFrom.Month - 1}&yt={dateTo.Year}&to=2{dateTo:dd.MM.yy}&" +
                $"p={(int)timeInterval}&f=file&e=.txt&cn={ticker}&dtf=1&tmf=1&MSOR=1&mstime=on&mstimever=1&sep=1&sep2=1&datf=5", 
                cancellationToken);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
