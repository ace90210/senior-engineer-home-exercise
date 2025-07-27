#!/bin/bash
set -e

# Get the GID of the Docker socket on the host
DOCKER_GID=$(stat -c '%g' /var/run/docker.sock)

# Create the docker group inside the container if needed
if ! getent group docker >/dev/null; then
  sudo groupadd -g "$DOCKER_GID" docker || true
fi

# Add 'agent' to the docker group
sudo usermod -aG docker agent

# Run the TeamCity agent (default CMD/ENTRYPOINT from the base image)
exec /run-agent.sh
