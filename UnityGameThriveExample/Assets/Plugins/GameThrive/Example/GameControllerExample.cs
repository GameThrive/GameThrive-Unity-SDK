/**
 * Modified MIT License
 * 
 * Copyright 2014 GameThrive
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameControllerExample : MonoBehaviour {

	private static string extraMessage;

	void Start () {
		extraMessage = null;

		// Call before using any other methods on GameThrive.
		// Should only be called once when your game is loaded.
		GameThrive.Init("b49e69ca-d0b8-11e3-97bf-c3d1433e8bc1", "703322744261", HandleNotification);
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
			} else if (additionalData.ContainsKey("actionSelected")) {
				// actionSelected equals the id on the button the user pressed.
				// actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present.
				extraMessage = "Pressed ButtonId: " + additionalData["actionSelected"];
			}
		}
	}

	// Test buttons to SendTags and SendPurchase to test segments on gamethrive.com
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
