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

@implementation UIApplication(GameThrivePush)

NSString* CreateNSString(const char* string) {
    return [NSString stringWithUTF8String: string ? string : ""];
}

GameThrive* gameThrive;
char* unityListener;
char* appId;
NSDictionary* launchDict;

static void switchMethods(Class class, SEL oldSel, SEL newSel, IMP impl, const char* sig) {
    class_addMethod(class, newSel, impl, sig);
    method_exchangeImplementations(class_getInstanceMethod(class, oldSel), class_getInstanceMethod(class, newSel));
}

+(void)load {
    method_exchangeImplementations(class_getInstanceMethod(self, @selector(setDelegate:)), class_getInstanceMethod(self, @selector(setGameThriveDelegate:)));
}

- (void) setGameThriveDelegate:(id<UIApplicationDelegate>)delegate {
    static Class delegateClass = nil;
    
	if(delegateClass == [delegate class]) {
		[self setGameThriveDelegate:delegate];
		return;
	}
    
	delegateClass = [delegate class];
    
	switchMethods(delegateClass, @selector(application:didRegisterForRemoteNotificationsWithDeviceToken:),
                  @selector(application:blankMethod:), (IMP)didRegisterForRemoteNotificationsWithDeviceTokenLocal, "v@:::");
    
    switchMethods(delegateClass, @selector(application:didReceiveRemoteNotification:),
                  @selector(application:blankMethod2:), (IMP)didReceiveRemoteNotificationLocal, "v@:::");
    
    switchMethods(delegateClass, @selector(application:didFinishLaunchingWithOptions:),
                  @selector(application:selectorDidFinishLaunchingWithOptions:), (IMP)didFinishLaunchingWithOptionsLocal, "v@:::");
    
    [self setGameThriveDelegate:delegate];
}


void _init(const char* listenerName, const char* appId, BOOL autoRegister) {
    unsigned long len = strlen(listenerName);
	unityListener = malloc(len + 1);
	strcpy(unityListener, listenerName);
    
    gameThrive = [[GameThrive alloc] init:CreateNSString(appId) autoRegister:autoRegister];
    
    if (launchDict)
        processNotificationOpened(launchDict, false);
}

void _registerForPushNotifications() {
    [gameThrive registerForPushNotifications];
}

void _sendTag(const char* tagName,const char* tagValue) {
	[gameThrive sendTag:CreateNSString(tagName) value:CreateNSString(tagValue)];
}

void _sendPurchase(double amount) {
    [gameThrive sendPurchase:[NSNumber numberWithDouble:amount]];
}

void didRegisterForRemoteNotificationsWithDeviceTokenLocal(id self, SEL _cmd, id application, id deviceToken) {
    NSLog(@"Device Registered with Apple!");
    [gameThrive registerDeviceToken:deviceToken onSuccess:^(NSDictionary* results) {
        NSLog(@"Device Registered with GameThrive.");
    } onFailure:^(NSError* error) {
        NSLog(@"Device Registion Error with GameThrive: %@", error);
    }];
}

BOOL didFinishLaunchingWithOptionsLocal(id self, SEL _cmd, id application, id launchOptions) {
    launchDict = [launchOptions objectForKey:UIApplicationLaunchOptionsRemoteNotificationKey];
    
    BOOL result = YES;
    
	if ([self respondsToSelector:@selector(application:selectorDidFinishLaunchingWithOptions:)])
		result = (BOOL) [self application:application selectorDidFinishLaunchingWithOptions:launchOptions];
    else {
		[self applicationDidFinishLaunching:application];
		result = YES;
	}
    
	return result;
}

void didReceiveRemoteNotificationLocal(id self, SEL _cmd, id application, id userInfo) {
    processNotificationOpened(userInfo, [application applicationState] == UIApplicationStateActive);
}

void processNotificationOpened(NSDictionary* messageData, BOOL isActive) {
    [gameThrive notificationOpened:messageData];
    
    NSMutableDictionary* pushDict = [messageData mutableCopy];
    
    [pushDict setValue:[NSNumber numberWithBool:isActive] forKey:@"isActive"];
    
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:pushDict options:0 error:nil];
    NSString* jsonRequestData = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    const char* str = [jsonRequestData UTF8String];
    
    UnitySendMessage(unityListener, "onPushNotificationReceived", str);
}

@end