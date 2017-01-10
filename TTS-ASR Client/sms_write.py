"""Send sms"""

__copyright__ = '(c) 2011 Emant Pte Ltd'
__license__ = 'Apache License, Version 2.0'

import android

droid = android.Android()
number = droid.dialogGetInput('Send SMS', 'Phone Number?').result 
message = 'Test SMS' 
result = droid.smsSend(number, message) 
