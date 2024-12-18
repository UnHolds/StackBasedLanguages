FROM debian:12.5-slim

RUN apt-get update
RUN apt-get upgrade -y
RUN apt-get install wget tar xz-utils -y
RUN wget https://www.complang.tuwien.ac.at/forth/gforth/Snapshots/current/gforth.tar.xz
RUN unxz gforth.tar.xz
RUN mkdir gforth
RUN tar -xvf gforth.tar -C gforth --strip-components=1
WORKDIR /gforth
RUN ./install-deps.sh
RUN ./configure
RUN make
RUN make install
