#!/bin/bash

# MiniSOC Agent Startup Script
# Collects events and sends them to the server

set -e  # Exit on error

echo "========================================"
echo "Starting MiniSOC Agent..."
echo "========================================"
echo ""

# Check if we're in the repository root
if [ ! -d "agent/MiniSOC.Agent" ]; then
    echo "Error: agent/MiniSOC.Agent directory not found."
    echo "Please run this script from the repository root directory."
    exit 1
fi

# Navigate to agent directory
cd agent/MiniSOC.Agent

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: dotnet command not found."
    echo "Please install .NET 8 SDK: https://dotnet.microsoft.com/download"
    exit 1
fi

# Warn if server might not be running
echo "⚠ Make sure the server is running first!"
echo "  Run: ./scripts/start-server.sh (in another terminal)"
echo ""

read -p "Press Enter to continue..."
echo ""

echo "✓ Starting agent..."
echo ""

# Run the agent
dotnet run
