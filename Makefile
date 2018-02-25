FSC=fsc
FSI=fsi

FILES=improvements.fs constants.fs vector.fs body.fs physics.fs graphics.fs
OUTPUT=solar.dll

build:
	$(FSC) $(FILES) -a -o $(OUTPUT)

clear:
	rm $(OUTPUT)

run:
	$(FSI) main.fsx
