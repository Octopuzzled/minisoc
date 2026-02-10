#!/bin/bash

# MiniSOC Server Startup Script
# Starts the MiniSOC server on http://localhost:5152

set -e  # Exit on error

echo "========================================"
echo "Starting MiniSOC Server..."
echo "========================================"
echo ""

# Check if we're in the repository root
if [ ! -d "server/MiniSOC.Server" ]; then
    echo "Error: server/MiniSOC.Server directory not found."
    echo "Please run this script from the repository root directory."
    exit 1
fi

# Navigate to server directory
cd server/MiniSOC.Server

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: dotnet command not found."
    echo "Please install .NET 8 SDK: https://dotnet.microsoft.com/download"
    exit 1
fi

echo "✓ Starting server..."
echo ""

# Run the server
dotnet run
