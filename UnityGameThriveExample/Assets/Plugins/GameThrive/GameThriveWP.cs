/**
 * Modified MIT License
 * 
 * Copyright 2015 GameThrive
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * 1. The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * 2. All copies of substantial portions of the Software may only be used in connection
 * with services provided by GameThrive.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

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

	public void SendTags(IDictionary<string> tags) {
		GameThriveSDK.GameThrive.SendTags(tags);
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

	public void DeleteTags(IList<string> key) {
		GameThriveSDK.GameThrive.DeleteTags(key);
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