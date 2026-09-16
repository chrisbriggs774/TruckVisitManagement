# API and AWS Lambda Function Setup Guide

## Overview

This document explains how to configure and run the .NET 10 API and AWS Lambda function locally.

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Running the API](#running-the-api)
3. [Running the AWS Lambda Function Locally](#running-the-aws-lambda-function-locally)
4. [Configuration](#configuration)
5. [Common Commands](#common-commands)
6. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### 1. Install .NET 10 SDK

Both the API and Lambda function require .NET 10.

Verify the installation:

```bash
dotnet --version
```

The command should return a .NET 10.x version.

---

## Running the API

### Restore Dependencies

```bash
dotnet restore
```

### Build the API

```bash
dotnet build
```

### Run the API

```bash
dotnet run --project TruckVisitManagement.Api.csproj
```

Alternatively:

```bash
cd TruckVisitManagement.Api
dotnet run
```

### Verify the API

Once running, browse to:

```text
https://localhost:7080/swagger
```

or

```text
http://localhost:5080/swagger
```

if Swagger is enabled.

---

## Running the AWS Lambda Function Locally

### Install the AWS Lambda Mock Test Tool

Install the .NET 10 Lambda Test Tool globally:

```bash
dotnet tool install -g Amazon.Lambda.TestTool-10.0
```

Verify installation:

```bash
dotnet-lambda-test-tool-10.0
```

### Restore Dependencies

```bash
dotnet restore
```

### Build the Lambda Project

```bash
dotnet build
```

### Launch the Mock Lambda Test Tool

From the Lambda project directory:

```bash
dotnet lambda-test-tool-10.0
```

If installed as a global tool, use the executable made available by the tool installation.

### Invoke the Function

1. Start the Lambda Test Tool.
2. Select the function handler.
3. Supply a test event JSON payload.
4. Execute the function.
5. Review logs and output in the tool.

---

## Configuration

### Application Settings

Update the appropriate configuration files before running:

- `appsettings.json`
- `appsettings.Development.json`
- Environment variables
- AWS profile configuration (if required)

Example:

```json
{
  "AWS": {
    "Region": "eu-central-1"
  }
}
```

### AWS Credentials

If the Lambda accesses AWS resources locally, configure credentials:

```bash
aws configure
```

or set:

```bash
AWS_PROFILE=<profile-name>
AWS_REGION=<region>
```

---

## Common Commands

### API

```bash
dotnet restore
dotnet build
dotnet run
```

### Lambda

```bash
dotnet restore
dotnet build
dotnet tool install -g Amazon.Lambda.TestTool-10.0
```

---

## Troubleshooting

### .NET Not Found

Verify:

```bash
dotnet --info
```

### Lambda Test Tool Not Found

Reinstall:

```bash
dotnet tool install -g Amazon.Lambda.TestTool-10.0
```

### Package Restore Failures

Clear NuGet cache and restore:

```bash
dotnet nuget locals all --clear
dotnet restore
```
