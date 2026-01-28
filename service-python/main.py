from fastapi import FastAPI, File, UploadFile
from typing import Annotated
from dotenv import load_dotenv
import os
import pdfplumber
import io
app = FastAPI()

load_dotenv()

@app.get("/")
def read_root():
    return {"Hello": "World"}

@app.post("/process-resume")
async def process_resume(file: Annotated[UploadFile, File()]):
    content = await file.read()
    keywords = ["Python", "FastAPI", "Docker"]
    
    if file.filename and file.filename.endswith('.txt'):
        content_str = content.decode('utf-8', errors='ignore')
        keywords_found = search_keywords(content_str, keywords)
        return {"result": content, "file_type": "txt", "keywords_found": keywords_found}
    elif file.filename and file.filename.endswith('.pdf'):
        pdf_file = io.BytesIO(content)
        text_content = ""
        with pdfplumber.open(pdf_file) as pdf:
            for page in pdf.pages:
                text_content += page.extract_text() or ""
            keywords_found = search_keywords(text_content, keywords)
            return {"file_type": "pdf", "keywords_found": keywords_found}

        keywords_found = search_keywords(content_str, ["Python", "FastAPI", "Docker"])
        return {"result": content, "file_type": "pdf", "keywords_found": keywords_found}
    else:
        return {"error": "Could not determine file type"}

def search_keywords(resume_text, keywords):
    keywords_found = False
    found_keywords = [keyword for keyword in keywords if keyword.lower() in resume_text.lower()]
    if found_keywords:
        keywords_found = True
    return keywords_found
