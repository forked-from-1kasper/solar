# Solar

Simple Solar System simulator written in F#.

## Screenshot

![Screenshot](screenshot.png)

## Requirements

.NET (on Windows, I use 4.6.1) or mono (on Linux, I use Fedora 27, and macOS, where I did not test, and one person says that it does not work; I do not know why).

For build: F# (4.1), GNU Make (I use cygwin).

## Binaries

https://github.com/forked-from-1kasper/solar/releases

## Build

`make`.

## Run

`./main.exe --config solar_system.xml` or `mono main.exe --config solar_system.xml` on Linux and macOS.

Or just `./main.exe` (`solar_system.xml` is the default).

`./main.exe --help` for help.

## Controls

| Keys    |                                              |
|---------|----------------------------------------------|
| W A S D | move                                         |
| Q E     | axis move                                    |
| + -     | control time speed                           |
| , .     | control scale                                |
| 8       | move to selected planet                      |
| 9 0     | select planet                                |
| P       | change projection                            |
| O       | toggle information                           |
| I       | toggle orbits                                |
| Space   | toggle pause                                 |
