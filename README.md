# BeatSaberSongExtractor
Modifies Beat Saber local custom level data.
This was made in an effort to alphabetize custom levels in-game because they are prefixed with a random set of characters.


Console application usage:

`BeatSaberSongExtractor <zippedLevelsDirectoryPath> <customLevelsDirectoryPath>`

e.g.

`BeatSaberSongExtractor "C:\Users\bab178\Downloads\ZippedSongs" "C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\Beat Saber_Data\CustomLevels"`


1. Gets custom songs `.zip` files from `zippedLevelsDirectoryPath`
```
zippedLevelsDirectoryPath
└───  4af (T Custom Song Name 1).zip
└─── 4e6f (F Custom Song Name 2).zip
└───   e4 (E Custom Song Name 3).zip
...
```

2. Extracts these custom levels to `customLevelsDirectoryPath` with reordered prefixes
```
customLevelsDirectoryPath
└─── 001 - e4 (E Custom Song Name 3)
     │   song.egg
     │   cover.jpg
     │   Easy.dat
     │   Expert.dat
     │   ExpertPlus.dat
     │   Hard.dat
     │   info.dat
     │   Normal.dat
└─── 002 - 4e6f (F Custom Song Name 2)
     │   song.egg
     │   cover.jpg
     │   ExpertPlus.dat
     │   info.dat
└─── 003 - 4af (T Custom Song Name 1)
     │   song.egg
     │   cover.jpg
     │   Easy.dat
     │   Expert.dat
     │   ExpertPlus.dat
     │   Hard.dat
     │   info.dat
     │   Normal.dat
...
```


## Before execution:

![Before execution](/images/before.jpg?raw=true)

## Reorganized after execution:

![Reorganized after execution](/images/after.jpg?raw=true)