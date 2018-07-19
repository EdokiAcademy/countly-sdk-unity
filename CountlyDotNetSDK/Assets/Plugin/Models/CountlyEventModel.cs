﻿using Assets.Plugin.Scripts.Development;
using Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Plugin.Models
{
    /// <summary>
    /// Custom Segmentation for Views related events.
    /// </summary>
    [Serializable]
    struct ViewSegment
    {
        [JsonProperty("name")]
        internal string Name { get; set; }
        [JsonProperty("segment")]
        internal string Segment { get; set; }
        [JsonProperty("visit")]
        internal int? Visit { get; set; }
        [JsonIgnore]
        internal bool HasSessionBegunWithView { get; set; }
        [JsonProperty("start")]
        internal string Start => HasSessionBegunWithView ? "true" : null;
    }

    /// <summary>
    /// Custom Segmentation for Action related events.
    /// </summary>
    [Serializable]
    struct ActionSegment
    {
        [JsonProperty("type")]
        internal string Type { get; set; }
        [JsonProperty("x")]
        internal int PositionX { get; set; }
        [JsonProperty("y")]
        internal int PositionY { get; set; }
        [JsonProperty("width")]
        internal int Width { get; set; }
        [JsonProperty("height")]
        internal int Height { get; set; }
    }

    /// <summary>
    /// Custom Segmentation for Star Rating event.
    /// </summary>
    [Serializable]
    struct StarRatingSegment
    {
        [JsonProperty("platform ")]
        internal string Platform { get; set; }
        [JsonProperty("app_version")]
        internal string AppVersion { get; set; }
        [JsonProperty("rating")]
        internal int Rating { get; set; }
    }

    [Serializable]
    class CountlyEventModel
    {
        #region Reserved Event Names

        [JsonIgnore]
        internal const string ViewEvent = "[CLY]_view";
        [JsonIgnore]
        internal const string ViewActionEvent = "[CLY]_action";
        [JsonIgnore]
        internal const string StarRatingEvent = "[CLY]_star_rating";

        #endregion

        [JsonProperty("key")]
        internal string Key { get; set; }
        [JsonProperty("count")]
        internal int? Count { get; set; }
        [JsonProperty("sum")]
        internal double? Sum { get; set; }
        [JsonProperty("dur")]
        internal double? Duration { get; set; }
        [JsonProperty("segmentation")]
        internal JRaw Segmentation { get; set; }

        /// <summary>
        /// Initializes an instance of event model. It doesn't mark the event as started instead it reports them.
        /// It is used for internal custom events like reporting views, actions, etc.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="segmentation"></param>
        /// <param name="duration"></param>
        internal CountlyEventModel(string key, string segmentation, double? duration)
        {
            Key = key;
            Count = 1;
            Segmentation = new JRaw(segmentation);
            Duration = duration;
        }

        /// <summary>
        /// Initializes an instance of event model. It also marks the event as started. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="segmentation"></param>
        /// <param name="sum"></param>
        /// <param name="count"></param>
        /// <param name="dur"></param>
        internal CountlyEventModel(string key, string segmentation = null, double? sum = null, int? count = 1, double? dur = null)
        {
            Key = key;
            Count = count;
            Segmentation = new JRaw(segmentation);
            Duration = dur;
            Sum = sum;

            Start();
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(Key) && string.IsNullOrWhiteSpace(Key))
                throw new ArgumentNullException(Key, "Key is required.");

            if (Countly.TotalEvents.Any(evnt => Key.Equals(evnt.Key, StringComparison.OrdinalIgnoreCase)))
                throw new Exception($"Event {Key} already started. Please end this event first.");

            Countly.TotalEvents.Add(Key, DateTime.Now);
        }

        /// <summary>
        /// Ends a particular event
        /// </summary>
        /// <returns></returns>
        internal bool End()
        {
            if (string.IsNullOrEmpty(Key))
                throw new ArgumentNullException(Key, "Key is required.");

            var addedEvent = Countly.TotalEvents.Where(evnt => Key.Equals(evnt.Key, StringComparison.OrdinalIgnoreCase))
                                        .FirstOrDefault();
            if (string.IsNullOrEmpty(addedEvent.Key))
                throw new Exception($"Event {Key} doesn't exists. Please add an event first.");

            Duration = Duration ?? (DateTime.Now - addedEvent.Value).TotalMilliseconds;

            var requestParams =
               new Dictionary<string, object>
               {
                    { "events", JsonConvert.SerializeObject(this, Formatting.Indented,
                                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) },
               };
            CountlyHelper.GetResponse(requestParams);

            //Removing the event
            Countly.TotalEvents.Remove(Key);

            return true;
        }

        /// <summary>
        /// Sends mulitple events to the countly server. It expects a set of events as input in the form of a serialized json string.
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        internal static bool StartMultipleEvents(string events)
        {
            var requestParams =
               new Dictionary<string, object>
               {
                    { "events", events },
               };
            CountlyHelper.GetResponse(requestParams);

            return true;
        }

        /// <summary>
        /// Sends multiple events to the countly server. It expects a list of events as input.
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        internal static bool StartMultipleEvents(List<CountlyEventModel> events)
        {
            var requestParams =
               new Dictionary<string, object>
               {
                    { "events", JsonConvert.SerializeObject(events, Formatting.Indented,
                                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) },
               };
            CountlyHelper.GetResponse(requestParams);

            return true;
        }

        /// <summary>
        /// Sends custom events to the Counlty server.
        /// </summary>
        /// <returns></returns>
        internal bool ReportCustomEvent()
        {
            if (string.IsNullOrEmpty(Key))
                throw new ArgumentNullException(Key, "Key is required.");

            var requestParams =
               new Dictionary<string, object>
               {
                    { "events", JsonConvert.SerializeObject(new List<CountlyEventModel>{ this }, Formatting.Indented,
                                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) },
               };
            CountlyHelper.GetResponse(requestParams);

            return true;
        }
    }
}