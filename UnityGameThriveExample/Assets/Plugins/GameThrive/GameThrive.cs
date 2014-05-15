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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameThrive : MonoBehaviour {

	// NotificationReceived - Delegate is called when a push notification is opend or one is received when the user is in your game.
	// message        = The message text the use seen in the push notification.
	// additionalData = Dictionary of key value pairs sent with the push notification.
	// isActive       = True when the user is currentlying in your game when a notification was received.
	public delegate void NotificationReceived(string message, Dictionary<string, object> additionalData, bool isActive);
	private static NotificationReceived notificationDelegate = null;

	private static GameThrivePlatform gameThrivePlatform = null;
#if !UNITY_EDITOR // Removes editor warning
	private static bool initialized = false;
#endif

	// Name of the GameObject to put into your game scene.
	private const string gameObjectName = "GameThrive";

	// Init - Only required method you call to setup GameThrive to recieve push notifications.
	//        Call this on the first scene that is loaded.
	// appId                  = Your GameThrive app id from gamethrive.com
	// googleProjectNumber    = Your google project number that is only required for Android GCM pushes.
	// inNotificationDelegate = Calls this delegate when a notification is opened or one is received when the user is in your game.
	// autoRegister           = Delays registering for iOS push notifications so you can RegisterForPushNotifications at a better point in your game.
	public static void Init(string appId, string googleProjectNumber, NotificationReceived inNotificationDelegate, bool autoRegister) {
		#if !UNITY_EDITOR
			if (initialized) return;
			#if UNITY_ANDROID
				gameThrivePlatform = new GameThriveAndroid(gameObjectName, googleProjectNumber, appId);
			#elif UNITY_IPHONE
				gameThrivePlatform = new GameThriveIOS(gameObjectName, appId, autoRegister);
			#endif
			notificationDelegate = inNotificationDelegate;
			GameObject go = new GameObject(gameObjectName);
			go.AddComponent<GameThrive>();
			DontDestroyOnLoad(go);
			initialized = true;
		#else
			print("Please run GameThrive on a device to see push notifications.");
		#endif
	}

	// Parameter defaulting split out into different methods so they are compatible with UnityScript (AKA Unity Javascript).
	public static void Init(string appId, string googleProjectNumber, NotificationReceived inNotificationDelegate) {
		Init(appId, googleProjectNumber, inNotificationDelegate, true);
	}
	public static void Init(string appId, string googleProjectNumber) {
		Init(appId, googleProjectNumber, null, true);
	}
	public static void Init(string appId) {
		Init(appId, null, null, true);
	}



	// Tag player with a key value pair to later create segments on them at the gamethrive site.
	public static void SendTag(string tagName, string tagValue) {
		#if !UNITY_EDITOR
			gameThrivePlatform.SendTag(tagName, tagValue);
		#endif
	}

	// Call when the player has made an IAP purchase in your game so you can later send push notifications based on free or paid users.
	public static void SendPurchase(double amount) {
		#if !UNITY_EDITOR
			gameThrivePlatform.SendPurchase(amount);
		#endif
	}

	// Call this when you would like to prompt an iOS user accept push notifications with the default system prompt.
	// Only use if you passed false to autoRegister when calling Init.
	public static void RegisterForPushNotifications() {
		#if !UNITY_EDITOR
			gameThrivePlatform.RegisterForPushNotifications();
		#endif
	}



	/*** protect and private methods ****/

	// Called from the navtive SDK
	private void onPushNotificationReceived(string jsonString) {
		if (notificationDelegate != null)
			gameThrivePlatform.FireNotificationReceivedEvent(jsonString, notificationDelegate);
	}

	// Called automatically by Unity
	void OnApplicationPause(bool paused) {
		gameThrivePlatform.OnApplicationPause(paused);
	}
}