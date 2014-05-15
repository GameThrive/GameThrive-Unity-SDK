/**
 * Copyright 2014 GameThrive
 * Portions Copyright 2014 StackMob
 *
 * This file includes portions from the StackMob iOS SDK and distributed under an Apache 2.0 license.
 * StackMob was acquired by PayPal and ceased operation on May 22, 2014.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// GameThrive SDK v1.2.0

#import <Foundation/Foundation.h>

/** SMResultSuccessBlock
 */
typedef void (^GTResultSuccessBlock)(NSDictionary* result);

/** SMFailureBlock
 */
typedef void (^GTFailureBlock)(NSError* error);

/**
 An `GameThrive` provides a high level interface to interacting with GameThrive's push service.
 
 `GameThrive` exposes a defaultClient for applications which use a globally available client to share configuration settings.
 
 Include `#import "GameThrive/GameThrive.h"` in your application files to access GameThrive's methods.
 
 ### Setting up the SDK ###
 
 Follow the documentation from http://docs.gamethrive.com/category/10-ios-game-setup to setup with your game.
 
 */
@interface GameThrive : NSObject

@property(nonatomic, readonly, copy) NSString* app_id;

///--------------------
/// @name Initialize
///--------------------

/**
 Initialize GameThrive. Sends push token to GameThrive so you can later send notifications.
 
 */
- (id)init;

- (id)init:(BOOL)autoRegister;

- (id)init:(NSString*)appId autoRegister:(BOOL)autoRegister;

- (void)registerForPushNotifications;


+ (void)setDefaultClient:(GameThrive*)client;
+ (GameThrive*)defaultClient;

- (void)registerDeviceToken:(id)token onSuccess:(GTResultSuccessBlock)successBlock onFailure:(GTFailureBlock)failureBlock;

- (void)sendTag:(NSString*)key value:(NSString*)value onSuccess:(GTResultSuccessBlock)successBlock onFailure:(GTFailureBlock)failureBlock;
- (void)sendTag:(NSString*)key value:(NSString*)value;

- (void)sendTags:(NSDictionary*)keyValuePair onSuccess:(GTResultSuccessBlock)successBlock onFailure:(GTFailureBlock)failureBlock;
- (void)sendTags:(NSDictionary*)keyValuePair;

- (void)sendPurchase:(NSNumber*)amount onSuccess:(GTResultSuccessBlock)successBlock onFailure:(GTFailureBlock)failureBlock;
- (void)sendPurchase:(NSNumber*)amount;

- (void)notificationOpened:(NSDictionary*)messageDict;

- (NSString*)getPlayerId;
- (NSString*)getDeviceToken;

- (NSDictionary*)getAdditionalData;
- (NSString*)getMessageString;

@end
