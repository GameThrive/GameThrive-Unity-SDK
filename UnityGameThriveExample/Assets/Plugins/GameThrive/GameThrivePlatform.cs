﻿/**
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


// Shared interface so GameThrive.cs can use each mobile platform in a generic way
public interface GameThrivePlatform {
	void RegisterForPushNotifications();
	void SendTag(string tagName, string tagValue);
	void SendPurchase(double amount);
	void OnApplicationPause(bool paused);

	void FireNotificationReceivedEvent(string jsonString, GameThrive.NotificationReceived notificationReceived);
}