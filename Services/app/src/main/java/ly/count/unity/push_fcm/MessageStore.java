package ly.count.unity.push_fcm;

import android.content.Context;
import android.content.SharedPreferences;
import android.util.Log;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;


public class MessageStore {
    private static SharedPreferences messagePreferences;
    private static final String MESSAGE_DATA = "MESSAGE_DATA";
    private static final String MESSAGE_PREFERENCES = "MESSAGE_PREFERENCES";

    private MessageStore() {
    }

    public static void init(final Context context) {
        if (messagePreferences == null) {
            messagePreferences = context.getSharedPreferences(MESSAGE_PREFERENCES, Context.MODE_PRIVATE);
        }
        CountlyPushPlugin.log("MessageStore init", CountlyPushPlugin.LogLevel.DEBUG);
    }

    public static boolean storeMessageData(String messageId, String index) {
        if (!isInitialized()) {
            CountlyPushPlugin.log("MessageStore isn't initialized", CountlyPushPlugin.LogLevel.DEBUG);
            return false;
        }

        String messagesData = getMessagesData();
        try {
            JSONArray jsonArray = null;

            if (messagesData == null) {
                jsonArray = new JSONArray();
            } else {
                jsonArray = new JSONArray(messagesData);
            }

            JSONObject jsonObject = new JSONObject();

            jsonObject.put("action_index", index);
            jsonObject.put("messageId", messageId);

            jsonArray.put(jsonObject);

            messagePreferences.edit().putString(MESSAGE_DATA, jsonArray.toString()).apply();

        } catch (JSONException e) {
            e.printStackTrace();
            return false;
        }

        return true;
    }

    public static void clearMessagesData() {
        if (isInitialized()) {
            messagePreferences.edit().remove(MESSAGE_DATA).apply();
        } else {
            CountlyPushPlugin.log("MessageStore isn't initialized", CountlyPushPlugin.LogLevel.DEBUG);
        }
    }

    public static String getMessagesData() {
        if (isInitialized()) {
            return messagePreferences.getString(MESSAGE_DATA, null);
        }

        CountlyPushPlugin.log("MessageStore isn't initialized", CountlyPushPlugin.LogLevel.DEBUG);
        return null;
    }

    public static boolean isInitialized() {
        return messagePreferences != null;
    }
}
