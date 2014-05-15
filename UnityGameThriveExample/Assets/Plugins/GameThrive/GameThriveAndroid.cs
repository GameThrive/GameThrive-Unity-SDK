/**
 * Copyright 2014 GameThrive
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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

	public void SendPurchase(double amount) {
		mGameThrive.Call("sendPurchase", amount);
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
}
#endif