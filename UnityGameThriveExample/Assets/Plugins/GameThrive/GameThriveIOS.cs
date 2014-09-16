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

#if UNITY_IPHONE
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using GameThrivePush.MiniJSON;

public class GameThriveIOS : GameThrivePlatform {

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _init(string listenerName, string appId, bool autoRegister);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _registerForPushNotifications();
	
	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _sendTag(string tagName, string tagValue);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _getTags();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _deleteTag(string key);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _sendPurchase(double amount);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _idsAvailable();


	public GameThriveIOS(string gameObjectName, string appId, bool autoRegister) {
		_init(gameObjectName, appId, autoRegister);
	}

	public void RegisterForPushNotifications() {
		_registerForPushNotifications();
	}

	public void SendTag(string tagName, string tagValue) {
		_sendTag(tagName, tagValue);
	}

	public void GetTags() {
		_getTags();
	}

	public void DeleteTag(string key) {
		_deleteTag(key);
	}

	public void SendPurchase(double amount) {
		_sendPurchase(amount);
	}

	public void IdsAvailable() {
		_idsAvailable();
	}

	public void FireNotificationReceivedEvent(string jsonString, GameThrive.NotificationReceived notificationReceived) {
		var dict = Json.Deserialize(jsonString) as Dictionary<string, object>;

		string message = (string)(dict["alertMessage"]);
		dict.Remove("alertMessage");

		bool isActive = (bool)dict["isActive"];
		dict.Remove("isActive");

		notificationReceived(message, dict, isActive);
	}

	public void OnApplicationPause(bool paused) {} // Handled by the Native Plugin
}
#endif