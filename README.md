# g-text2midi

This software lets you play some text via MIDI. Almost all you need to construct the MIDI file is probably implemented, but not transposing, no loops, no linking or embedding. 

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
```

