using System.Collections.Generic;
using Plugins.Countly.Models;
using UnityEngine;

namespace Notifications
{
    public class NotificationsCallbackService 
    {
        CountlyConfigModel _config;
        private List<INotificationListener> _listeners;
        internal NotificationsCallbackService(CountlyConfigModel config)
        {
            _config = config;
            _listeners = new List<INotificationListener>();
        }

        public void AddListener(INotificationListener listener)
        {
            if (_listeners.Contains(listener)) {
                return;
            }

            _listeners.Add(listener);

            if (_config.EnableConsoleErrorLogging)
            {
                Debug.Log("[Countly NotificationsCallbackServcie] AddListener: " + listener);
            }
        }

        public void RemoveListener(INotificationListener listener)
        {
            _listeners.Remove(listener);

            if (_config.EnableConsoleErrorLogging)
            {
                Debug.Log("[Countly NotificationsCallbackServcie] RemoveListener: " + listener);
            }
        }

        public void SendMessageToListeners(string data)
        {
            foreach (INotificationListener listener in _listeners)
            {
                if (listener != null)
                {
                    listener.OnReceive(data);
                }
            }

            if (_config.EnableConsoleErrorLogging)
            {
                Debug.Log("[Countly NotificationsCallbackServcie] SendMessageToListeners: " + data);
            }
        }
    }

    public interface INotificationListener
    {
        void OnReceive(string message);
    }
}