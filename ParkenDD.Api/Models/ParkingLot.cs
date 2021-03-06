﻿using System;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace ParkenDD.Api.Models
{
    /// <summary>
    ///     Data for single parking lot
    /// </summary>
    public sealed class ParkingLot : ViewModelBase
    {
        /// <summary>
        ///     Address of the parking lot
        /// </summary>
        [JsonProperty("address")]
        public string Address
        {
            get { return _address; }
            set { Set(ref _address, value); }
        }
        private string _address;

        /// <summary>
        ///     Coordinates of the parking lot
        /// </summary>
        [JsonProperty("coords")]
        public Coordinate Coordinates
        {
            get { return _coordinates; }
            set { Set(ref _coordinates, value); }
        }
        private Coordinate _coordinates;

        /// <summary>
        ///     Amount of free parking space
        /// </summary>
        [JsonProperty("free")]
        public int FreeLots
        {
            get { return Math.Min(_freeLots, _totalLots); }
            set { Set(ref _freeLots, value); }
        }
        private int _freeLots;

        /// <summary>
        ///     Total amount of parking space
        /// </summary>
        [JsonProperty("total")] 
        public int TotalLots
        {
            get { return _totalLots; }
            set { Set(ref _totalLots, value); }
        }
        private int _totalLots;

        /// <summary>
        ///     Id of the parking lot
        /// </summary>
        [JsonProperty("id")]
        public string Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }
        private string _id;

        /// <summary>
        ///     Type of the Parking lot
        /// </summary>
        [JsonProperty("lot_type")]
        public string LotType
        {
            get { return _lotType; }
            set { Set(ref _lotType, value); }
        }
        private string _lotType;

        /// <summary>
        ///     State of the parking lot
        /// </summary>
        [JsonProperty("state")]
        public ParkingLotState State
        {
            get { return _state; }
            set { Set(ref _state, value); }
        }
        private ParkingLotState _state;

        /// <summary>
        ///     Name of the Parking lot
        /// </summary>
        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }
        private string _name;

        /// <summary>
        ///     Indicates whether a forecast or tendency is available
        /// </summary>
        [JsonProperty("forecast")]
        public bool HasForecast
        {
            get { return _hasForecast; }
            set { Set(ref _hasForecast, value); }
        }
        private bool _hasForecast;

        /// <summary>
        ///     Short time data for parking lot
        /// </summary>
        [JsonIgnore] //currently not in API
        public ForecastBase Forecast
        {
            get { return _forecast; }
            set { Set(ref _forecast, value); }
        }
        private ForecastBase _forecast;

        /// <summary>
        ///     Shows if long time data is available
        /// </summary>
        [JsonProperty("long_forecast_available")]
        public bool HasLongForecast
        {
            get { return _hasLongForecast; }
            set { Set(ref _hasLongForecast, value); }
        }
        private bool _hasLongForecast;

        /// <summary>
        ///     City region of this parking lot
        /// </summary>
        [JsonProperty("region")]
        public string Region
        {
            get { return _region; }
            set { Set(ref _region, value); }
        }
        private string _region;
    }
}
