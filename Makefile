FSC=fsc
FSI=fsi

FILES=improvements.fs constants.fs vector.fs body.fs physics.fs graphics.fs
OUTPUT=solar.dll

build:
	$(FSC) $(FILES) -a -o $(OUTPUT)

build-binary: build
	tail -n +3 main.fsx > main.fs
	fsc -r $(OUTPUT) main.fs

clean:
	rm *.dll *.exe main.fs

run:
	$(FSI) main.fsx
