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

public class GameControllerExample : MonoBehaviour {

	private static string extraMessage;

	void Start () {
		extraMessage = null;

		// Call before using any other methods on GameThrive.
		// Should only be called once when your game is loaded.
		GameThrive.Init("5eb5a37e-b458-11e3-ac11-000c2940e62c", "703322744261", HandleNotification);
	}

	// Gets called when the player opens the notification or gets one while in your game.
	// The name of the method can be anything as long as the signature matches.
	// Method must be static or this object should be marked as DontDestroyOnLoad
	private static void HandleNotification(string message, Dictionary<string, object> additionalData, bool isActive) {
		print("GameControllerExample:HandleNotification");
		print(message);

		// When isActive is true this means the player is currently in your game.
		// Use isActive and your own game logic so you don't interupt the player with a popup or menu when they are in the middle of playing your game.
		if (additionalData != null) {
			if (additionalData.ContainsKey("discount")) {
                extraMessage = (string)additionalData["discount"];
				// Take player to your store.
			}
			else if (additionalData.ContainsKey("bonusCredits")) {
				extraMessage = "BONUS CREDITS!";
				// Take player to your store.
			}
		}
	}

	// Test buttons to SendTags and SendPurchase to test segements on gamethrive.com
	void OnGUI () {
		GUIStyle customTextSize = new GUIStyle("button");
		customTextSize.fontSize = 30;

		GUIStyle customBoxSize = new GUIStyle("box");
		customBoxSize.fontSize = 30;

		GUI.Box(new Rect(10, 10, 390, 250), "Test Menu", customBoxSize);

		if (GUI.Button(new Rect(60, 80, 300, 60), "SendTags", customTextSize))
			GameThrive.SendTag("UnityTestKey", "TestValue");

		if (GUI.Button(new Rect(60, 170, 300, 60), "SendPurchase", customTextSize))
			GameThrive.SendPurchase(2.57d);

		if (extraMessage != null)
			GUI.Box(new Rect(60, 300, 400, 60), extraMessage, customBoxSize);
	}
}
