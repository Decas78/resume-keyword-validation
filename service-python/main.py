from fastapi import FastAPI
from pydantic import BaseModel
from dotenv import load_dotenv
import os
import json

app = FastAPI()

load_dotenv()

class APILog(BaseModel):
    method: str
    path: str
    queryString: str | None = None
    bodySizeBytes: int
    latencyMs: int
    responseStatusCode: int
    clientIp: str | None = None
    timestamp: str | None = None

@app.get("/process")
def process(value: int):
    result = value * 2
    return {"result": result} # Double the result before returning

@app.post("/logsimple")
def logsimple(entry: APILog):
    print("Simple log entry:", entry.json())
    return {"status": "log received"}
