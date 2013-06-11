//
//  TTS.h
//  Please
//
//  Created by Jeff Schifano on 6/10/13.
//
//

#ifdef CORDOVA_FRAMEWORK
#import <Cordova/CDVPlugin.h>
#else
#import "CDVPlugin.h"
#endif

#import <Slt/Slt.h>
#import <OpenEars/OpenEarsEventsObserver.h>

@class PocketsphinxController;
@class FliteController;

@interface TTS : CDVPlugin <OpenEarsEventsObserverDelegate>;

- (void)speak:(CDVInvokedUrlCommand*)command;
- (void)interrupt:(CDVInvokedUrlCommand*)command;
- (void)stop:(CDVInvokedUrlCommand*)command;
- (void)silence:(CDVInvokedUrlCommand*)command;
- (void)speed:(CDVInvokedUrlCommand*)command;
- (void)pitch:(CDVInvokedUrlCommand*)command;
- (void)startup:(CDVInvokedUrlCommand*)command;
- (void)shutdown:(CDVInvokedUrlCommand*)command;
- (void)isLanguageAvailable:(CDVInvokedUrlCommand*)command;
- (void)setLanguage:(CDVInvokedUrlCommand*)command;

@property (nonatomic, strong) Slt *slt;
@property (nonatomic, strong) OpenEarsEventsObserver *openEarsEventsObserver;
@property (nonatomic, strong) PocketsphinxController *pocketsphinxController;
@property (nonatomic, strong) FliteController *fliteController;

@end
