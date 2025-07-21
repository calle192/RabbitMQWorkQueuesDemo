#!/bin/bash
echo "Sending automated messages to RabbitMQ..."

dotnet NewTast.dll "Hello World, .."
dotnet NewTask.dll "This is an automated message...."
dotnet NewTask.dll "Periods count as seconds!....."
dotnet NewTask.dll "This is a test message for RabbitMQ...."
dotnet NewTask.dll "This is a test message for RabbitMQ!....."
dotnet NewTask.dll "This is a test message for RabbitMQ!....."
dotnet NewTask.dll "This is the last message in the queue!....."

echo "All messages sent to RabbitMQ successfully!"