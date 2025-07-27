#!/bin/bash
set -e

echo "👉 Setting up Docker group permissions"

# Get the GID of the mounted Docker socket
DOCKER_GID=$(stat -c '%g' /var/run/docker.sock)
echo "📦 Detected Docker socket GID: $DOCKER_GID"

# Create docker group with host's GID if not already present
if ! getent group docker >/dev/null; then
  sudo groupadd -g "$DOCKER_GID" docker || true
else
  echo "🔁 Docker group already exists"
fi

# Add buildagent to the docker group
sudo usermod -aG docker buildagent
echo "👤 Added buildagent to docker group"

# Switch to 'buildagent' user with updated groups
echo "🚀 Switching to buildagent user"
exec sudo -E -u buildagent /run-agent.sh
