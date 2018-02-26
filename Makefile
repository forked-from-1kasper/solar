FSC=fsc
FSI=fsi

FILES=improvements.fs constants.fs vector.fs body.fs physics.fs graphics.fs xml.fs
OUTPUT=solar.dll

build:
	$(FSC) $(FILES) -a -o $(OUTPUT)
	fsc -r $(OUTPUT) main.fs

clean:
	rm *.dll *.exe main.fs

run:
	$(FSI) main.fsx
