FROM gforth/base
RUN apt-get update
RUN apt-get install python3 python3-pip -y

# copy stuff
WORKDIR /flask
COPY flask/* .
