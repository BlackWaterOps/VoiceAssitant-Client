//
//  TTS.m
//  Please
//
//  Created by Jeff Schifano on 6/10/13.
//
//

#import "TTS.h"
#import <OpenEars/PocketsphinxController.h>
#import <OpenEars/FliteController.h>
#import <OpenEars/LanguageModelGenerator.h>
#import <OpenEars/OpenEarsLogging.h>

@implementation TTS

@synthesize pocketsphinxController;
@synthesize fliteController;
@synthesize slt;
@synthesize openEarsEventsObserver;

- (void)speak:(CDVInvokedUrlCommand*)command {
    
    CDVPluginResult *pluginResult = nil;
    NSString speakText = [command.arguments objectAtIndex:0];
    
    if (fliteEngine != nil && speakText != nil && [speakText length] > 0) {
        [fliteEngine speakText:speakText];
        
        pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK];
    } else {
        // plugin failure
        pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_ERROR];
    }
    
    [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
}

- (void)interrupt:(CDVInvokedUrlCommand*)command {
    
}

- (void)stop:(CDVInvokedUrlCommand*)command {
    
}

- (void)silence:(CDVInvokedUrlCommand*)command {
    
}

- (void)speed:(CDVInvokedUrlCommand*)command {
    
}

- (void)pitch:(CDVInvokedUrlCommand*)command {
    
}

- (void)startup:(CDVInvokedUrlCommand*)command {
    // start up voice
    if (slt == nil) {
        slt = [[Slt alloc] init];
    }
    
    // startup speech synthesis
    if (fliteController == nil) {
        fliteController = [[FliteController alloc] init];
    }
    
    // startup speech recognizer
    if (pocketsphinxController == nil) {
        pocketsphinxController = [[PocketsphinxController alloc] init];
    }
    
    // startup observer
    if (openEarsEventsObserver == nil) {
        openEarsEventsObserver = [[OpenEarsEventsObserver alloc] init];
    }
}

- (void)shutdown:(CDVInvokedUrlCommand*)command {
    
}

- (void)isLanguageAvailable:(CDVInvokedUrlCommand*)command {
    
}

- (void)setLanguage:(CDVInvokedUrlCommand*)command {
    
}



@end
