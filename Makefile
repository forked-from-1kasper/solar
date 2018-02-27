# change to fsharc on Linux, macOS
FSC=fsc

FILES=improvements.fs constants.fs vector.fs body.fs physics.fs graphics.fs xml.fs
OUTPUT=solar.dll

build:
	$(FSC) $(FILES) -a -o $(OUTPUT)
	$(FSC) -r $(OUTPUT) main.fs

clean:
	rm *.dll *.exe main.fs
