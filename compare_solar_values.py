import math
from datetime import datetime
n=datetime(1981,2,8).timetuple().tm_yday
lat=30.19;lon=76.25;tz=5.3
decl=23.44*math.sin((360/365)*(n-81)*math.pi/180)
cosH=(math.cos(math.radians(90.833))-math.sin(math.radians(lat))*math.sin(math.radians(decl)))/(math.cos(math.radians(lat))*math.cos(math.radians(decl)))
h=math.degrees(math.acos(max(-1,min(1,cosH))))
eot=9.87*math.sin(2*math.pi*(n-81)/364)-7.53*math.cos(2*math.pi*(n-81)/364)-1.5*math.sin(2*math.pi*(n-81)/364)
noon=12-lon/15+tz-eot/60
print(noon-h/15,noon+h/15)
