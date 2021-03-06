# change to fsc on Windows
# change to fsharc on Linux, macOS
FSC=fsc

FILES=improvements.fs constants.fs vector.fs body.fs physics.fs graphics.fs xml.fs keymap.fs
OUTPUT=solar.dll

build:
	$(FSC) $(FILES) -a -o $(OUTPUT)
	$(FSC) -r $(OUTPUT) main.fs -o solar-bin.exe

build-standalone: build
	$(FSC) -r $(OUTPUT) main.fs -o solar-standalone.exe --standalone

clean:
	rm -f *.dll *.exe

fun:
	rm -rf *