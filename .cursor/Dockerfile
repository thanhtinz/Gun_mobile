FROM ubuntu:22.04

ENV DEBIAN_FRONTEND=noninteractive
ENV UNITY_VERSION=6000.3.22f1
ENV UNITY_CHANGESET=1c726e1fb402

RUN apt-get update && apt-get install -y --no-install-recommends \
    curl ca-certificates gnupg git python3 python3-pip \
    libgtk-3-0 libnotify4 libnss3 libxss1 libxtst6 xdg-utils libatspi2.0-0 libuuid1 libsecret-1-0 \
    libgl1 libglu1-mesa libasound2 libpulse0 \
    && rm -rf /var/lib/apt/lists/*

# Unity Hub (official deb repo)
RUN install -d /etc/apt/keyrings \
    && curl -fsSL https://hub.unity3d.com/linux/keys/public | gpg --dearmor -o /etc/apt/keyrings/unityhub.gpg \
    && echo "deb [arch=amd64 signed-by=/etc/apt/keyrings/unityhub.gpg] https://hub.unity3d.com/linux/repos/deb stable main" > /etc/apt/sources.list.d/unityhub.list \
    && apt-get update \
    && apt-get install -y unityhub \
    && rm -rf /var/lib/apt/lists/*

USER ubuntu
