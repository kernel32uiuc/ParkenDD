﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ParkenDD.Api.Converters;
using ParkenDD.Api.Interfaces;
using ParkenDD.Api.Models;
using ParkenDD.Api.Models.Exceptions;

namespace ParkenDD.Api
{
    public class ParkenDdClient : IParkenDdClient
    {
        private const string BaseUri = "http://api.parkendd.de/";
        //private const string BaseUri = "http://park-api.higgsboson.tk/";
        //private const string BaseUri = "http://jkliemann.de/parkendd/park-api/";

        protected readonly HttpClient Client;

        public ParkenDdClient()
        {
            Client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            })
            {
                BaseAddress = new Uri(BaseUri),
                Timeout = TimeSpan.FromSeconds(30)
            };
            Client.DefaultRequestHeaders.Add("User-Agent", "ParkenDD for Windows"); //TODO: add version using variable
        }

        /// <summary>
        ///     Fetches a typed response
        /// </summary>
        /// <typeparam name="T">type of expected response</typeparam>
        /// <param name="method">HTTP method to use</param>
        /// <param name="requestUri">URI to request (will be appended to base uri)</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>parsed typed response</returns>
        /// <exception cref="ApiException">
        ///     Error retrieving response
        /// </exception>
        protected async Task<T> Request<T>(
            HttpMethod method = null,
            string requestUri = null,
            CancellationToken? cancellationToken = null)
                where T : class, new()
        {
            if (method == null)
            {
                method = HttpMethod.Get;
            }
            if (requestUri == null)
            {
                requestUri = string.Empty;
            }

            var request = new HttpRequestMessage(method, requestUri);
            HttpResponseMessage response = null;
            string responseString = null;

            // make async request
            try
            {
                if (cancellationToken.HasValue)
                {
                    response = await Client.SendAsync(request, cancellationToken.Value);
                }
                else
                {
                    response = await Client.SendAsync(request);
                }

                ValidateResponse(response);

                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException exc)
            {
                throw new ApiException(exc.Message, exc);
            }
            catch (TaskCanceledException e)
            {
                if (!e.CancellationToken.IsCancellationRequested)
                {
                    throw new ApiException("The server took too long to response :(", e);
                }
                return null;
            }
            finally
            {
                request.Dispose();
                response?.Dispose();
            }

            return DeserializeJson<T>(responseString);
        }

        /// <summary>
        ///     Validate the server response
        /// </summary>
        /// <param name="response">server response</param>
        /// <exception cref="ApiException">
        ///     Error retrieving response
        /// </exception>
        protected void ValidateResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }
            throw new ApiException($"Request error: {response.ReasonPhrase} ({(int)response.StatusCode})");
        }


        protected T DeserializeJson<T>(string json) where T : class, new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(
                    json,
                    new JsonSerializerSettings
                    {
                        /*Error = delegate(object sender, ErrorEventArgs args)
                        {
                            args.ErrorContext.Handled = true;
                            throw new ApiException($"Parse error: {args.ErrorContext.Error.Message}", args.ErrorContext.Error);
                        },
                        */Converters =
                        {
                            new JsonMetaDataCitiesConverter(),
                            new JsonForecastConverter(),
                            new JsonUriConverter(),
                            new JsonUtcDateTimeConverter()
                        }
                    }
                    );
            }
            catch (Exception e)
            {
                throw new ApiException($"Parse error: {e.Message}", e);
            }
        }

        /// <summary>
        ///     Get meta data from server
        /// </summary>
        /// <param name="ct">cancellation token</param>
        /// <returns>parsed server response</returns>
        public Task<MetaData> GetMetaDataAsync(CancellationToken? ct = null)
        {
            return Request<MetaData>(
                cancellationToken: ct
            );
        }

        /// <summary>
        ///     Get city details from server
        /// </summary>
        /// <param name="cityId">ID of city</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>parsed server response</returns>
        public Task<City> GetCityAsync(string cityId, CancellationToken? ct = null)
        {
            return Request<City>(
                requestUri: cityId,
                cancellationToken: ct
            );
        }

        /// <summary>
        ///     Get forecast from server
        /// </summary>
        /// <param name="cityId">ID of city</param>
        /// <param name="parkingLotId">ID of parking lot</param>
        /// <param name="from">start time (inclusive)</param>
        /// <param name="to">end time (inclusive)</param>
        /// <param name="ct">cancellation token</param>
        /// <returns></returns>
        public Task<Forecast> GetForecastAsync(string cityId, string parkingLotId, DateTime from, DateTime to, CancellationToken? ct = null)
        {
            if (from > to)
            {
                throw new ArgumentOutOfRangeException(nameof(to), "End date must be after or equal to start date.");
            }
            return Request<Forecast>(
                requestUri:
                    $"{cityId}/{parkingLotId}/timespan?from={IsoDateConverter.ToIsoString(@from)}&to={IsoDateConverter.ToIsoString(to)}",
                cancellationToken: ct
            );
        }
    }
}
