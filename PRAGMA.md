# _PRAGMA Docs_
_PRAGMA Programming Language Documentation_

**Assignments:**

|| - next argument in function.
```
set X||25
text 50||50||Hello World||Red
```

&& - call a variable to function.
```
plus X||&&Y
text &&X||&&Y||&&Text||&&Color
```

**Variables, Conditions, Stop Code:**

var – create an any type (integer, string) variable.
var Name
```
var X||50
var X||"Hi!"
```

set – set a value to provided variable (text or integer).
set Variable Name || Value
```
set X||25
set X||"Hi!"
set X||&&y
```

if – starts an if-(else)-endcond construction (condition).
if   Variable Name   Operator(>;<;==)   Value
```
if X > 50
if Text == "Hello"
if Y < 50
```

if mouse cia - checks a click in typed area.
if mouse cia x||y||width||height
```
if mouse cia 0||0||100||100
```

else – starts an else construction (unless condition).
else
```
else
```

endcond – end of condition construction.
endcond
```
endcond
```

stop – stops program.
stop
```
stop
```

Example of variables and conditions
```
var X||50
var Y||"Hi"
if X > 0
(Some Code)
else
(Some Code)
endcond
if Y == Hi
(Some Code)
endcond
```

**Graphics functions:**

> You can take color name from HTML color names.

text – create text on screen.
text X||Y||Text||Color
```
text 50||50||Hello World||Red
```

rect – create a rectangle on screen.
rect X||Y||Width||Height||Color
```
rect 50||50||100||25||Blue
```

circle – create a circle on screen.
circle X||Y||Radius||Color
```
circle 50||300||25||White
```

pixel – create a pixel on screen.
pixel X||Y||Color
```
pixel 100||500||Purple
```

**Interact with PC:**

shutdown – shutdown a PC.
shutdown
```
shutdown
```

reboot – reboot a PC.
reboot
```
reboot
```

beep – make beep sound from PC speaker.
beep Frequency (in Hertz)||Duration (in milliseconds)
```
beep 1000||50
```

msg - Call a message on screen.
msg text1||text2||type
type can be w (warning) or i (info)
```
msg PRAGMA top!||Maybe...||i
msg Your PC eat a cake||PC will explode soon||w
```

**Math operations:**

plus – plus math operation.
plus Variable Name||Value (can be variable, use &&)
```
plus X||50
plus X||&&Y
```

minus – minus math operation.
minus Variable Name||Value (can be variable, use &&)
```
minus X||50
minus X||&&Y
```

multiply – minus math operation.
multiply Variable Name||Value (can be variable, use &&)
```
multiply X||50
multiply X||&&Y
```

divide – minus math operation.
divide Variable Name||Value (can be variable, use &&)
```
divide X||50
divide X||&&Y
```

power – make power of provided number.
power Variable Name||Value (can be variable, use &&)
```
divide X||2
divide X||&&Y
```

sqrt – make square root of provided number.
sqrt Variable Name
```
sqrt X
```
