"""Read sms"""

__copyright__ = '(c) 2011 Emant Pte Ltd'
__license__ = 'Apache License, Version 2.0'

import android

droid = android.Android()

SMSmsgs = droid.smsGetMessages(False, 'inbox').result 

for message in SMSmsgs:
    print 'From: '+message['address']+' > '+message['body']+'\n'
