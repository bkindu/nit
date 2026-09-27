import math
from datetime import datetime

d=datetime(1981,2,8,7,47)
y=d.year;m=d.month
if m<=2:y-=1;m+=12
a=y//100;b=2-a+a//4
jd=math.floor(365.25*(y+4716))+math.floor(30.6001*(m+1))+d.day+b-1524.5+(7+47/60)/24
x=(225.28676+3034.692024*jd/36525)/30+39
n=math.floor(((x%60)+60)%60)
print(jd,x,n)
