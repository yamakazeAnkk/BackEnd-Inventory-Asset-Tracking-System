#!/bin/bash

set -euo pipefail

USER_PORT=5000
AUTH_PORT=5001

echo "🚀 Testing Kafka Integration between UserService and AuthService"
echo "================================================================"

# Check if Kafka is running
echo "📋 Checking Kafka status..."
if brew services list | grep -q "kafka.*started"; then
    echo "✅ Kafka is running"
else
    echo "❌ Kafka is not running. Starting Kafka..."
    brew services start kafka
    sleep 5
fi

# List existing topics
echo "📋 Existing Kafka topics:"
kafka-topics --bootstrap-server localhost:9092 --list

echo ""
echo "🔧 Starting UserService (will consume from user-events topic)..."
(
  cd UserService
  ASPNETCORE_URLS=http://localhost:${USER_PORT} dotnet run > ../.userservice.log 2>&1 &
  echo $! > ../.userservice.pid
)
USER_SERVICE_PID=$(cat .userservice.pid)

echo "⏳ Waiting for UserService to start on :${USER_PORT}..."
for i in {1..30}; do
  if curl -s http://localhost:${USER_PORT}/api/kafkatest/status >/dev/null; then break; fi
  sleep 1
done

echo ""
echo "🔧 Starting AuthService (will publish to user-events topic)..."
(
  cd AuthService
  ASPNETCORE_URLS=http://localhost:${AUTH_PORT} dotnet run > ../.authservice.log 2>&1 &
  echo $! > ../.authservice.pid
)
AUTH_SERVICE_PID=$(cat .authservice.pid)

echo "⏳ Waiting for AuthService to start on :${AUTH_PORT}..."
for i in {1..30}; do
  if curl -s http://localhost:${AUTH_PORT}/api/kafkatest/status >/dev/null; then break; fi
  sleep 1
done

echo ""
echo "🧪 Testing Kafka Integration..."
echo "================================"

# Test 1: Check service status
echo "📡 Testing UserService status..."
curl -s http://localhost:${USER_PORT}/api/kafkatest/status | cat

echo ""
echo "📡 Testing AuthService status..."
curl -s http://localhost:${AUTH_PORT}/api/kafkatest/status | cat

# Test 2: Publish a test message from UserService
echo ""
echo "📤 Publishing test message from UserService..."
curl -s -X POST http://localhost:${USER_PORT}/api/kafkatest/publish-test-message \
  -H "Content-Type: application/json" \
  -d '"Hello from UserService!"' | cat

# Test 3: Publish user events from AuthService
echo ""
echo "📤 Publishing user created event from AuthService..."
curl -s -X POST http://localhost:${AUTH_PORT}/api/kafkatest/publish-user-created \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "email": "test@example.com",
    "username": "testuser"
  }' | cat

echo ""
echo "📤 Publishing user login event from AuthService..."
curl -s -X POST http://localhost:${AUTH_PORT}/api/kafkatest/publish-user-login \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "email": "test@example.com",
    "ipAddress": "192.168.1.100"
  }' | cat

echo ""
echo "⏳ Waiting for messages to be processed..."
sleep 3

# Test 4: Check Kafka consumer groups for the consumer service only (UserService)
echo ""
echo "📊 Kafka Consumer Groups:"
kafka-consumer-groups --bootstrap-server localhost:9092 --list | cat

echo ""
echo "📊 Consumer Group Details for user-service-group:"
kafka-consumer-groups --bootstrap-server localhost:9092 --describe --group user-service-group | cat

echo ""
echo "🧹 Cleaning up..."
kill $USER_SERVICE_PID 2>/dev/null || true
kill $AUTH_SERVICE_PID 2>/dev/null || true
rm -f .userservice.pid .authservice.pid
sleep 1

echo ""
echo "✅ Kafka Integration Test Complete!"
echo "=================================="
echo ""
echo "📋 Summary:"
echo "  - UserService: Consumes messages from 'user-events' topic"
echo "  - AuthService: Publishes user events to 'user-events' topic"
echo "  - Both services are configured to use localhost:9092"
echo "  - Messages are automatically serialized/deserialized as JSON"
