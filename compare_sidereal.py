import math
from datetime import datetime,timedelta
d=datetime(1981,2,8,7,47)-timedelta(hours=5.3)
y=d.year;m=d.month
if m<=2:y-=1;m+=12
a=y//100;b=2-a+a//4
jd=math.floor(365.25*(y+4716))+math.floor(30.6001*(m+1))+d.day+b-1524.5+(d.hour+d.minute/60)/24
v=(280.46061837+360.98564736629*(jd-2451545.0)+76.25)%360
print(v/15)
