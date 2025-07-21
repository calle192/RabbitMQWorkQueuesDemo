This project demonstrates a basic work queue pattern using RabbitMQ and .NET.
Messages are pushed into a queue, and a worker service consume them, simulating work based on the number of periods (.) in each message.
An automation script named entrypoint.sh is used by the producer to generate messages.
This readme only covers options that make use of the solutions docker-compose file.
Not covered by this readme is how to run each project with dotnet ex: dotnet Worker.dll.
## 🧱 Project Structure

- **NewTask**: A .NET console app that sends messages to RabbitMQ (`NewTask.dll`).
- **Worker**: A .NET console app that listens to the `task_queue` and processes incoming messages (`Worker.dll`).
- **entrypoint.sh**: A Bash script to send automated messages into the queue using `NewTask.dll`.

## 🚀 Getting Started

You can run this project using either:

### Option 1: 
Docker Compose (Runs 4 containers - one RabbitMQ broker, one producer, and two consumers - in a single console window)
  After cloning this repo, open powershell, navigate to the solutions folder, and run:
  
```bash
docker-compose build
docker-compose up
```

Health checks were put in place. Producer/consumer's containers wait until RabbitMQ's container has started.
If producer/consumer applicaitons complete before RabbitMQ is ready they will wait 5 seconds then attempt to reconnect.

### Option2:
Manualy launching each component in a seperate terminal.
After cloning this repo, open at least 4 powershell terminals, navigate to the solutions folder in each terminal, and complete:

1) Terminal One: (RabbitMQ)
   ```bash
   docker-compose build
   docker-compose up rabbitmq
   ```
2) Terminal Two: (Consumer/worker1)
   ```bash
   docker-compose up worker1
   ```
3) Terminal Three: (Consumer/worker2)
   ```bash
   docker-compose up worker2
   ```
4) Terminal Four: (Producer/newtask)
   ```bash
   docker-compose up newtask
   ```

### 🛠️ Key Concepts
   Durable Queues: Ensures messages aren't lost if RabbitMQ restarts (not when shutdown).
   Fair Dispatch: Worker gets one message at a time (BasicQos).
   Manual Acknowledgement: Worker manually sends BasicAck once it finishes processing.

### 🐇 Environment Variables
  The consumer and producer support the following environment variables (set-up is in docker-compose.yml)
    | Variable        | Default   |
    | --------------- | --------- |
    | `RABBITMQ_HOST` | localhost |
    | `RABBITMQ_PORT` | 5672      |
    | `RABBITMQ_USER` | guest     |
    | `RABBITMQ_PASS` | guest     |

### 🧪 Notes
  1) Ideal for learning RabbitMQ basics and integrating with .NET mircoservices.
  2) Tested in both Docker and local environments.
  3) Docker Compose can be used to run the entire solution in one shot, or you can use PowerShell for
     fine-grained control.
  4) Developed in Visual Studio: debug mode will only generate default hello world! message.

### 📁 Files Overview
  1) entrypoint.sh - Bash script for automated message sending.
  2) Worker/Program.cs - Asynchronous RabbitMQ consumer with retry logic.
  3) NewTask/Program.cs - Simple publisher with configurable message body.
     Contains retry logic.
  
