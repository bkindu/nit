import math
# julday returns days from 1900 epoch using the JS formula
day,month,year,hour=8,2,1981,2.4833333333
if month<3: year-=1; month+=12
leep=math.floor(year/100); leep=2-leep+math.floor(leep/4)
dj=math.floor(365.2501*year)+math.floor((month+1)*30.6001)-694025+leep+day+hour/24
v=(16.6981583332+15*2.4833333333+76.25+dj*0.985647335386+dj*dj/3446481700000)%360
print(dj,v/15)
