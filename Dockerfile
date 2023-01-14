FROM ubuntu:20.04

RUN apt update
RUN apt install -y openssh-server
RUN service ssh start

RUN apt install -y sudo

CMD ["tail", "-f", "/dev/null"]