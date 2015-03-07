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

static Class getClassWithProtocolInHierarchy(Class searchClass, Protocol* protocolToFind) {
    if (!class_conformsToProtocol(searchClass, protocolToFind)) {
        if ([searchClass superclass] == [NSObject class])
            return nil;
        
        Class foundClass = getClassWithProtocolInHierarchy([searchClass superclass], protocolToFind);
        if (foundClass)
            return foundClass;
        
        return searchClass;
    }
    
    return searchClass;
}

static void injectSelector(Class newClass, SEL newSel, Class addToClass, SEL makeLikeSel) {
    Method newMeth = class_getInstanceMethod(newClass, newSel);
    IMP imp = method_getImplementation(newMeth);
    const char* methodTypeEncoding = method_getTypeEncoding(newMeth);
    
    BOOL successful = class_addMethod(addToClass, makeLikeSel, imp, methodTypeEncoding);
    if (!successful) {
        class_addMethod(addToClass, newSel, imp, methodTypeEncoding);
        newMeth = class_getInstanceMethod(addToClass, newSel);
        
        Method orgMeth = class_getInstanceMethod(addToClass, makeLikeSel);
        
        method_exchangeImplementations(orgMeth, newMeth);
    }
}

const char* dictionaryToJsonChar(NSDictionary* dictionaryToConvert) {
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:dictionaryToConvert options:0 error:nil];
    NSString* jsonRequestData = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    
    return [jsonRequestData UTF8String];
}


+ (void)load {
    method_exchangeImplementations(class_getInstanceMethod(self, @selector(setDelegate:)), class_getInstanceMethod(self, @selector(setGameThriveUnityDelegate:)));
}

static Class delegateClass = nil;

- (void) setGameThriveUnityDelegate:(id<UIApplicationDelegate>)delegate {
    if(delegateClass != nil)
		return;
    
    delegateClass = getClassWithProtocolInHierarchy([delegate class], @protocol(UIApplicationDelegate));
    
    injectSelector(self.class, @selector(gameThriveApplication:didFinishLaunchingWithOptions:),
                   delegateClass, @selector(application:didFinishLaunchingWithOptions:));
    [self setGameThriveUnityDelegate:delegate];
}

- (BOOL)gameThriveApplication:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions {
    if ([launchOptions objectForKey:UIApplicationLaunchOptionsRemoteNotificationKey] != nil)
        initGameThriveObject(launchOptions, nil, true);
    
    if ([self respondsToSelector:@selector(gameThriveApplication:didFinishLaunchingWithOptions:)])
        return [self gameThriveApplication:application didFinishLaunchingWithOptions:launchOptions];
    
    return YES;
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

void _sendTags(const char* tags) {
    [gameThrive sendTagsWithJsonString:CreateNSString(tags)];
}

void _sendPurchase(double amount) {
    [gameThrive sendPurchase:[NSNumber numberWithDouble:amount]];
}

void _deleteTag(const char* key) {
    [gameThrive deleteTag:CreateNSString(key)];
}

void _deleteTags(const char* keys) {
    [gameThrive deleteTagsWithJsonString:CreateNSString(keys)];
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