.PHONY: build install clean

build:
	go build -o dnsgrep

install: build
	mv dnsgrep /usr/local/bin/

clean:
	rm -f dnsgrep