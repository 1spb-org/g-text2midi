# g-text2midi

This software lets you play some text via MIDI. Almost all you need to construct the MIDI file is probably implemented, but not transposing, no loops, no linking or embedding. Here's an example:

```json
{
  "Tracks": [
    {
      "Start": 0,
      "Interval": 60,
      "Instrument": 3,
      "Commands": [
        "L50 *4 /C--3E--3G *3 /2G-A _ ",
        "_ /2G--3A /3G--4A L500 /4G /5G  L122  /A",
        "_ _ _ _ L77 *4 /C--3E--3G *3 /2G-A _",
        " _ /2G--3A /3G--4A L500 /4G /5G  L122  /A"
      ],
      "Octave": 3,
      "Channel": 0,
      "Velocity": 64
    },
    {
      "Start": 700,
      "Interval": 60,
      "Instrument": 41,
      "Commands": [
        "L50 *4 /C--3E--3G *3 /2G-A _ ",
        "_ /2G--3A /3G--4A L500 /4G /5G  L122  /A",
        "_ _ _ _ L77 *4 /C--3E--3G *3 /2G-A _",
        " _ /2G--3A /3G--4A L500 /4G /5G  L122  /A"
      ],
      "Octave": 3,
      "Channel": 1,
      "Velocity": 64
    }
  ],
  "Description": "An example of GM2T. See https://github.com/1spb-org/g-text2midi for help",
  "Name": "Example"
}

```

 "Commands" notation: 

```
<Commands> := <Command>[<Space><Commands>]
<Space> := <Space> | <Tab> | <CR> | <LF>

<Command> can contain the following:
L<Number> : Chord length
*<Number> : Current octave, 1 .. 7
/<Chord>:   Chord definition:
	<Chord> := <Note>[-<Chord>] | <Note> | <>
	<Note> := [<Number>]<NoteName>
Here 
	<Number> is note octave, optional;
	<NoteName> := C | C# | D | D# | E | F | F# | G | G# | A | A# | B, it's note's name
/_ or _ means pause with current interval
+<Number> - Increase interval (affects next note's start time)
-<Number> - Decrease interval (affects next note's start time)
```



Here's yet another example:

```json
{
  "Tracks": [
    {
      "Commands": [
        "L100 *4",
		"/C-E-G-3G /C-3A /D-3A +50 L150 /3B-D -100 L50 /C /D _ +50",
        "/E-C /E-C  /F-D       +50 L150 /E-C  -100 L50 /D /C _ +50",
        "/D-C-3G /C-3A /3B-D L250 /C-3G-3C",
      ],  
      "Interval": 100,
      "Instrument": 6,    
      "Channel": 0 
    }
  ],
  "Description": "God Save The King. Try to continue with https://www.youtube.com/watch?v=R9WpZFY-tTE",
  "Name": "Example"
}
```





 