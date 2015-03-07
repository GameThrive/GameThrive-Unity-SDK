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

#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameThrivePush.MiniJSON;

public class GameThriveAndroid : GameThrivePlatform {
	private static AndroidJavaObject mGameThrive = null;

	public GameThriveAndroid(string gameObjectName, string googleProjectNumber, string appId) {
		Debug.Log("GameThriveAndroid");

		mGameThrive = new AndroidJavaObject("com.gamethrive.GameThriveUnityProxy", gameObjectName, googleProjectNumber, appId);
	}

	public void SendTag(string tagName, string tagValue) {
		mGameThrive.Call("sendTag", tagName, tagValue);
	}

	public void SendTags(IDictionary<string, string> tags) {
		mGameThrive.Call("sendTags", Json.Serialize(tags));
	}

	public void SendPurchase(double amount) {
		mGameThrive.Call("sendPurchase", amount);
	}

	public void GetTags() {
		mGameThrive.Call("getTags");
	}

	public void DeleteTag(string key) {
		mGameThrive.Call("deleteTag", key);
	}

	public void DeleteTags(IList<string> keys) {
		mGameThrive.Call("deleteTags", Json.Serialize(keys));
	}

	public void IdsAvailable() {
		mGameThrive.Call("idsAvailable");
	}
	
	public void FireNotificationReceivedEvent(string jsonString, GameThrive.NotificationReceived notificationReceived) {
		var dict = Json.Deserialize(jsonString) as Dictionary<string, object>;
		Dictionary<string, object> additionalData = null;
		if (dict.ContainsKey("custom"))
			additionalData = dict["custom"] as Dictionary<string, object>;

		notificationReceived((string)(dict["alert"]), additionalData, (bool)dict["isActive"]);
	}
	
	public void OnApplicationPause(bool paused) {
		if (paused)
			mGameThrive.Call("onPause");
		else
			mGameThrive.Call("onResume");
	}

	public void RegisterForPushNotifications() { } // Doesn't apply to Android as the Native SDK always registers with GCM.

	public void EnableVibrate(bool enable) {
		mGameThrive.Call("enableVibrate", enable);
	}

	public void EnableSound(bool enable) {
		mGameThrive.Call("enableSound", enable);
	}
}
#endif