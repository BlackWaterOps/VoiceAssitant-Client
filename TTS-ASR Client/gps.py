"""Read geolocation"""

__copyright__ = '(c) 2011 Emant Pte Ltd'
__license__ = 'Apache License, Version 2.0'

import android

droid = android.Android()

droid.startLocating()
print "reading GPS ..."
event = droid.eventWaitFor('location',10000).result
if event['name'] == "location":
  lat = str(event['data']['gps']['latitude'])
  lng = str(event['data']['gps']['longitude'])
  latlng = 'lat: ' + lat + ' lng: ' + lng
  print latlng

droid.stopLocating()
