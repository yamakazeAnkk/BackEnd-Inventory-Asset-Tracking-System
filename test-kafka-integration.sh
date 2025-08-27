#!/bin/bash

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
cd UserService
dotnet run &
USER_SERVICE_PID=$!
cd ..

echo "⏳ Waiting for UserService to start..."
sleep 10

echo ""
echo "🔧 Starting AuthService (will publish to user-events topic)..."
cd AuthService
dotnet run &
AUTH_SERVICE_PID=$!
cd ..

echo "⏳ Waiting for AuthService to start..."
sleep 10

echo ""
echo "🧪 Testing Kafka Integration..."
echo "================================"

# Test 1: Check service status
echo "📡 Testing UserService status..."
curl -s http://localhost:5000/api/kafkatest/status | jq '.'

echo ""
echo "📡 Testing AuthService status..."
curl -s http://localhost:5001/api/kafkatest/status | jq '.'

# Test 2: Publish a test message from UserService
echo ""
echo "📤 Publishing test message from UserService..."
curl -s -X POST http://localhost:5000/api/kafkatest/publish-test-message \
  -H "Content-Type: application/json" \
  -d '"Hello from UserService!"' | jq '.'

# Test 3: Publish user events from AuthService
echo ""
echo "📤 Publishing user created event from AuthService..."
curl -s -X POST http://localhost:5001/api/kafkatest/publish-user-created \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "email": "test@example.com",
    "username": "testuser",
    "createdAt": "2025-01-27T14:00:00Z"
  }' | jq '.'

echo ""
echo "📤 Publishing user login event from AuthService..."
curl -s -X POST http://localhost:5001/api/kafkatest/publish-user-login \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "email": "test@example.com",
    "loginAt": "2025-01-27T14:00:00Z",
    "ipAddress": "192.168.1.100"
  }' | jq '.'

echo ""
echo "⏳ Waiting for messages to be processed..."
sleep 5

# Test 4: Check Kafka consumer groups
echo ""
echo "📊 Kafka Consumer Groups:"
kafka-consumer-groups --bootstrap-server localhost:9092 --list

echo ""
echo "📊 Consumer Group Details for user-service-group:"
kafka-consumer-groups --bootstrap-server localhost:9092 --describe --group user-service-group

echo ""
echo "📊 Consumer Group Details for auth-service-group:"
kafka-consumer-groups --bootstrap-server localhost:9092 --describe --group auth-service-group

echo ""
echo "🧹 Cleaning up..."
kill $USER_SERVICE_PID 2>/dev/null
kill $AUTH_SERVICE_PID 2>/dev/null

echo ""
echo "✅ Kafka Integration Test Complete!"
echo "=================================="
echo ""
echo "📋 Summary:"
echo "  - UserService: Consumes messages from 'user-events' topic"
echo "  - AuthService: Publishes user events to 'user-events' topic"
echo "  - Both services are configured to use localhost:9092"
echo "  - Messages are automatically serialized/deserialized as JSON"
echo ""
echo "🔗 Next Steps:"
echo "  1. Start both services manually:"
echo "     - cd UserService && dotnet run"
echo "     - cd AuthService && dotnet run"
echo "  2. Use the test endpoints to publish messages"
echo "  3. Watch the console logs to see message consumption"
echo "  4. Monitor Kafka topics: kafka-console-consumer --bootstrap-server localhost:9092 --topic user-events"
