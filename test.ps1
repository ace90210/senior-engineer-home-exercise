# Stop and remove the running TeamCity stack (including volumes)
Write-Host "`n🛑 Stopping and removing containers and volumes..."
docker compose down -v

# Find and remove the teamcity-agent-conf volume explicitly
Write-Host "`n🧹 Cleaning up old 'teamcity-agent-conf' volume..."
$volumes = docker volume ls --format "{{.Name}}"
$agentVolume = $volumes | Where-Object { $_ -like "*teamcity-agent-conf*" }

if ($agentVolume) {
    docker volume rm $agentVolume
    Write-Host "✅ Removed volume: $agentVolume"
} else {
    Write-Host "ℹ️ No lingering agent config volume found."
}

# Optional: ensure build cache doesn't interfere
Write-Host "`n🔨 Rebuilding Docker images..."
docker compose build --no-cache

# Start the services
Write-Host "`n🚀 Starting Docker Compose stack..."
docker compose up -d

# Show agent logs to confirm connection
Write-Host "`n📋 Tailing teamcity-agent logs..."
Start-Sleep -Seconds 5
docker compose logs -f teamcity-agent
