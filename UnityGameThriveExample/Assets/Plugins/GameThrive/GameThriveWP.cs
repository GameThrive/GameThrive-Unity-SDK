#if UNITY_WP8 && !UNITY_EDITOR
using UnityEngine;
using System.Linq;

class GameThriveWP : GameThrivePlatform {

    public GameThriveWP(string appId) {
        GameThriveSDK.GameThrive.Init(appId, (additionalData, isActive) => {
            if (GameThrive.notificationDelegate != null)
                GameThrive.notificationDelegate("", additionalData.ToDictionary(pair => pair.Key, pair=>(object)pair.Value), isActive);
        });
    }

    public void SendTag(string tagName, string tagValue) {
        GameThriveSDK.GameThrive.SendTag(tagName, tagValue);
    }
    
    public void SendPurchase(double amount) {
        GameThriveSDK.GameThrive.SendPurchase(amount);
    }
    
    public void GetTags() {
        GameThriveSDK.GameThrive.GetTags((tags) => {
           GameThrive.tagsReceivedDelegate(tags.ToDictionary(pair => pair.Key, pair=>(object)pair.Value));
        });
    }

    public void DeleteTag(string key) {
        GameThriveSDK.GameThrive.DeleteTag(key);
    }

    public void IdsAvailable() {
        GameThriveSDK.GameThrive.GetIdsAvailable((playerId, channelUri) => {
            GameThrive.idsAvailableDelegate(playerId, channelUri);
        });
    }

    // Doesn't apply to Windows Phone: The Callback is setup in the constructor so this is never called.
    public void FireNotificationReceivedEvent(string jsonString, GameThrive.NotificationReceived notificationReceived) {}

    public void OnApplicationPause(bool paused) { } // Doesn't apply to Windows Phone: The Native SDK auto handles this.
    public void RegisterForPushNotifications() { } // Doesn't apply to Windows Phone: The Native SDK always registers.
}
#endif