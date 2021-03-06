# Solar

Simple Solar System simulator written in F#. Simulator can be configured through the xml configuration file.

## Screenshot

![Screenshot](screenshot.png)

## Requirements

.NET (on Windows, I use 4.7.1) or mono (on Linux, I use Fedora 27, and macOS; where I did not test, and one person says that it does not work; I do not know why).

For build: F# (4.1), MSBuild.

## Binaries

https://github.com/forked-from-1kasper/solar/releases

## Build

`MSBuild` or Visual Studio.

## Run

`./solar-bin.exe --config solar_system.xml` or `mono solar-bin.exe --config solar_system.xml` on Linux and macOS (or `solar-standalone.exe`).

Or just `./solar-bin.exe` (`solar_system.xml` is the default).

`./solar-bin.exe --help` for help.

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
