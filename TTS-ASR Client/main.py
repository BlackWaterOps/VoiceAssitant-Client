import kivy
import android
kivy.require('1.6.0')

from kivy.app import App
from kivy.properties import ObjectProperty
from kivy.uix.boxlayout import BoxLayout

from components.initialize import InitializePlatform
from components.ttsspeak import TtsSpeak


class TtsDemo(BoxLayout):
    saywhat_text = ObjectProperty(None)

    def say_something(self, text):
        TtsSpeak(text).speak()

    def clear(self):
        self.saywhat_text.text = ""
        self.saywhat_text.focus = True
    def hear_something(self):
        droid = android.Android()
        (id, result, error) = droid.recognizeSpeech("Say something")
        print result, error
        TtsSpeak(result).speak()

class TtsDemoApp(App):
    def build(self):
        InitializePlatform()
        return TtsDemo()
    

if __name__ == '__main__':
    TtsDemoApp().run()
