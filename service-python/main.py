from fastapi import FastAPI, File, UploadFile
from typing import Annotated
from dotenv import load_dotenv
import os
import json

app = FastAPI()

load_dotenv()

@app.get("/")
def read_root():
    return {"Hello": "World"}

@app.post("/process-resume")
def process_resume(file: Annotated[bytes, File()]):
    return {"result": file}