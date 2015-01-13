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

// GameThrive SDK v1.6.6

#import <Foundation/Foundation.h>
#import <objc/runtime.h>

typedef void (^GTResultSuccessBlock)(NSDictionary* result);
typedef void (^GTFailureBlock)(NSError* error);
typedef void (^GTIdsAvailableBlock)(NSString* playerId, NSString* pushToken);
typedef void (^GTHandleNotificationBlock)(NSString* message, NSDictionary* additionalData, BOOL isActive);

/**
 `GameThrive` provides a high level interface to interacting with GameThrive's push service.
 
 `GameThrive` exposes a defaultClient for applications which use a globally available client to share configuration settings.
 
 Include `#import "GameThrive/GameThrive.h"` in your application files to access GameThrive's methods.
 
 ### Setting up the SDK ###
 
 Follow the documentation from http://documentation.gamethrive.com/v1.0/docs/installing-the-gamethrive-ios-sdk to setup with your game.
 
 */
@interface GameThrive : NSObject

@property(nonatomic, readonly, copy) NSString* app_id;

///--------------------
/// @name Initialize
///--------------------

/**
 Initialize GameThrive. Sends push token to GameThrive so you can later send notifications.
 
 */

- (id)initWithLaunchOptions:(NSDictionary*)launchOptions;

- (id)initWithLaunchOptions:(NSDictionary*)launchOptions autoRegister:(BOOL)autoRegister;

- (id)initWithLaunchOptions:(NSDictionary*)launchOptions handleNotification:(GTHandleNotificationBlock)callback;

- (id)initWithLaunchOptions:(NSDictionary*)launchOptions appId:(NSString*)appId handleNotification:(GTHandleNotificationBlock)callback;

- (id)initWithLaunchOptions:(NSDictionary*)launchOptions appId:(NSString*)appId handleNotification:(GTHandleNotificationBlock)callback autoRegister:(BOOL)autoRegister;

// Only use if you passed FALSE to autoRegister
- (void)registerForPushNotifications;


+ (void)setDefaultClient:(GameThrive*)client;
+ (GameThrive*)defaultClient;

- (void)sendTag:(NSString*)key value:(NSString*)value onSuccess:(GTResultSuccessBlock)successBlock onFailure:(GTFailureBlock)failureBlock;
- (void)sendTag:(NSString*)key value:(NSString*)value;

- (void)sendTags:(NSDictionary*)keyValuePair onSuccess:(GTResultSuccessBlock)successBlock onFailure:(GTFailureBlock)failureBlock;
- (void)sendTags:(NSDictionary*)keyValuePair;

- (void)getTags:(GTResultSuccessBlock)successBlock onFailure:(GTFailureBlock)failureBlock;
- (void)getTags:(GTResultSuccessBlock)successBlock;

- (void)deleteTag:(NSString*)key onSuccess:(GTResultSuccessBlock)successBlock onFailure:(GTFailureBlock)failureBlock;
- (void)deleteTag:(NSString*)key;

- (void)deleteTags:(NSArray*)keys onSuccess:(GTResultSuccessBlock)successBlock onFailure:(GTFailureBlock)failureBlock;
- (void)deleteTags:(NSArray*)keys;

- (void)sendPurchase:(NSNumber*)amount onSuccess:(GTResultSuccessBlock)successBlock onFailure:(GTFailureBlock)failureBlock DEPRECATED_ATTRIBUTE;
- (void)sendPurchase:(NSNumber*)amount DEPRECATED_ATTRIBUTE;


- (void)IdsAvailable:(GTIdsAvailableBlock)idsAvailableBlock;

@end

