# Real-time ASP.NET Core SignalR

This project demonstrates a simple real-time communication setup using ASP.NET Core and SignalR.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

## Project Structure

- **SignalRServer**: ASP.NET Core Web API with SignalR Hub
- **SignalRClient**: Console app that connects to the SignalR Hub

## Setup and Run

### SignalR ServerSide

**_dotnet add package Microsoft.AspNetCore.SignalR_**

1. **Navigate to the project directory**:
   ```bash
   cd SignalRServer
   ```
2. **Restore dependencies and run:**
   dotnet restore
   dotnet run
   **_The server will be available at http://localhost:5281/chathub._**

### SignalR ClientSide

**_dotnet add package Microsoft.AspNetCore.SignalR.Client_**

cd ClientSide
dotnet restore
dotnet run
