import math
for dj in (29624.10347222222,29624.1,2444643.8243):
 v=(99.195+360*(dj-math.floor(dj))+76.25+dj*.98564735)%360
 print(dj,v/15)
