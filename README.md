# API Resume Keyword Validation

A .NET Api that will receive file uploads and send these to a python FastAPI endpoint for processing of a keyword

## Problem Statement
"How do you upload files to a .NET webapi and then process this asynchronously in Python"

## Architecture

### How Logs Flow
- **ResumeApi** (.NET 8 minimal API): Contains endpoint for HTTP file upload and logic for forwarding to service-python
- **service-python** (FastAPI): Receives file upload and processes for keyword returning result to resumeApi

### Why Two Services?
I have chosen to use two API services to decouple logic. .NET will handle validation, authentication and file upload. Python will the keyword search.

## End Goal
?

## Installation & Run
Ensure to have the following installed:
- .NET SDK 8.0.416
- Python 3.12
- Docker Compose

Open root directory and run the following:
- docker compose build
- docker compose up

## Technologies Used
- Update here