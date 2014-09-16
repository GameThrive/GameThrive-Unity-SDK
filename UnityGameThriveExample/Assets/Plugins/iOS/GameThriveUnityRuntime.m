/**
 * Copyright 2014 GameThrive
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

#import "GameThrive.h"
#import <objc/runtime.h>

@implementation UIApplication(GameThriveUnityPush)

NSString* CreateNSString(const char* string) {
    return [NSString stringWithUTF8String: string ? string : ""];
}

GameThrive* gameThrive;
char* unityListener = nil;
char* appId;
NSMutableDictionary* launchDict;

static void switchMethods(Class class, SEL oldSel, SEL newSel, IMP impl, const char* sig) {
    class_addMethod(class, newSel, impl, sig);
    method_exchangeImplementations(class_getInstanceMethod(class, oldSel), class_getInstanceMethod(class, newSel));
}

const char* dictionaryToJsonChar(NSDictionary* dictionaryToConvert) {
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:dictionaryToConvert options:0 error:nil];
    NSString* jsonRequestData = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    
    return [jsonRequestData UTF8String];
}


+ (void)load {
    method_exchangeImplementations(class_getInstanceMethod(self, @selector(setDelegate:)), class_getInstanceMethod(self, @selector(setGameThriveUnityDelegate:)));
}

- (void) setGameThriveUnityDelegate:(id<UIApplicationDelegate>)delegate {
    switchMethods([delegate class], @selector(application:didFinishLaunchingWithOptions:),
                  @selector(application:selectorDidFinishLaunchingWithOptions:), (IMP)didFinishLaunchingWithOptions_GTLocal, "v@:::");
    [self setGameThriveUnityDelegate:delegate];
}

BOOL didFinishLaunchingWithOptions_GTLocal(id self, SEL _cmd, id application, id launchOptions) {
    BOOL result = YES;
    
    if ([self respondsToSelector:@selector(application:selectorDidFinishLaunchingWithOptions:)]) {
        BOOL openedFromNotification = ([launchOptions objectForKey:UIApplicationLaunchOptionsRemoteNotificationKey] != nil);
        if (openedFromNotification)
            initGameThriveObject(launchOptions, nil, true);
        result = (BOOL) [self application:application selectorDidFinishLaunchingWithOptions:launchOptions];
    }
    else {
        [self applicationDidFinishLaunching:application];
        result = YES;
    }
    
    return result;
}


void processNotificationOpened(NSDictionary* resultDictionary) {
    UnitySendMessage(unityListener, "onPushNotificationReceived", dictionaryToJsonChar(resultDictionary));
}

void initGameThriveObject(NSDictionary* launchOptions, const char* appId, BOOL autoRegister) {
    if (gameThrive == nil) {
        NSString* appIdStr = (appId ? [NSString stringWithUTF8String: appId] : nil);
        
        gameThrive = [[GameThrive alloc] initWithLaunchOptions:launchOptions appId:appIdStr handleNotification:^(NSString* message, NSDictionary* additionalData, BOOL isActive) {
            launchDict = [[NSMutableDictionary alloc] initWithDictionary:additionalData];
            launchDict[@"isActive"] = [NSNumber numberWithBool:isActive];
            launchDict[@"alertMessage"] = message;
            
            if (unityListener)
                processNotificationOpened(launchDict);
        } autoRegister:autoRegister];
    }
}

void _init(const char* listenerName, const char* appId, BOOL autoRegister) {
    unsigned long len = strlen(listenerName);
	unityListener = malloc(len + 1);
	strcpy(unityListener, listenerName);
    
    initGameThriveObject(nil, appId, autoRegister);
    
    if (launchDict)
        processNotificationOpened(launchDict);
}


void _registerForPushNotifications() {
    [gameThrive registerForPushNotifications];
}

void _sendTag(const char* tagName, const char* tagValue) {
	[gameThrive sendTag:CreateNSString(tagName) value:CreateNSString(tagValue)];
}

void _sendPurchase(double amount) {
    [gameThrive sendPurchase:[NSNumber numberWithDouble:amount]];
}

void _deleteTag(const char* key) {
    [gameThrive deleteTag:CreateNSString(key)];
}

void _getTags() {
    [gameThrive getTags:^(NSDictionary* result) {
        UnitySendMessage(unityListener, "onTagsReceived", dictionaryToJsonChar(result));
    }];
}

void _idsAvailable() {
    [gameThrive IdsAvailable:^(NSString* playerId, NSString* pushToken) {
        if(pushToken == nil)
            pushToken = @"";
        
        UnitySendMessage(unityListener, "onIdsAvailable",
                         dictionaryToJsonChar(@{@"playerId" : playerId, @"pushToken" : pushToken}));
    }];
}

@end