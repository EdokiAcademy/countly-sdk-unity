using iBoxDB.LocalServer;
using Plugins.CountlySDK.Persistance;
using Plugins.CountlySDK.Persistance.Entities;
using UnityEngine;

namespace Plugins.CountlySDK.Helpers
{
    public class CountlyBoxDbHelper
    {
        public static DB BuildDatabase(long dbNumber)
        {
            DB.Root(Application.persistentDataPath);
            var db = new DB(dbNumber);

            db.GetConfig().EnsureTable<RequestEntity>(EntityType.Requests.ToString(), "Id");
            db.GetConfig().EnsureTable<EventEntity>(EntityType.ViewEvents.ToString(), "Id");
            db.GetConfig().EnsureTable<EventEntity>(EntityType.NonViewEvents.ToString(), "Id");
            db.GetConfig().EnsureTable<SegmentEntity>(EntityType.ViewEventSegments.ToString(), "Id");
            db.GetConfig().EnsureTable<SegmentEntity>(EntityType.NonViewEventSegments.ToString(), "Id");
            db.GetConfig().EnsureTable<SegmentEntity>(EntityType.Configs.ToString(), "Id");
            db.GetConfig().EnsureTable<EventNumberInSameSessionEntity>(EntityType.EventNumberInSameSessions.ToString(), "Id");

            return db;
        }
    }
}