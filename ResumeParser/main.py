import os
from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, Field, EmailStr, conint, confloat
from typing import List, Optional
import shutil
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain_community.document_loaders import PyPDFLoader
from dotenv import load_dotenv

load_dotenv()

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

class Education(BaseModel):
    university_name: str
    degree: str
    gpa: Optional[float] = None

class Experience(BaseModel):
    company_name: Optional[str]
    n_years: Optional[int]
    project_name: Optional[str]
    project_description: Optional[str]
    tech_stack: Optional[str]

class Resume(BaseModel):
    name: str
    email: EmailStr
    phone_number: str
    experience: Optional[List[Experience]]
    education: Optional[List[Education]]
    skills: Optional[List[str]]


# Initialize Model with Structured Output
llm = ChatGoogleGenerativeAI(model="gemini-2.5-flash-lite") # Use flash for speed
structured_llm = llm.with_structured_output(Resume)

# Replace your @app.post section with this improved version
@app.post("/parse-resume")
async def parse_resume(file: UploadFile = File(...)):
    temp_path = f"temp_{file.filename}"
    try:
        # Save uploaded file
        content = await file.read()
        with open(temp_path, "wb") as f:
            f.write(content)

        # Extract Text
        loader = PyPDFLoader(temp_path)
        pages = loader.load()
        pdf_text = " ".join([p.page_content for p in pages])

        if not pdf_text.strip():
            raise HTTPException(status_code=400, detail="PDF is empty or unreadable")

        # Use a simpler prompt for structured output
        # Ensure your API Key is actually loaded
        if not os.getenv("GOOGLE_API_KEY"):
            return {"error": "API Key not found in environment"}

        # Invoke the structured LLM
        # IMPORTANT: Wrap the text in a HumanMessage format or a clear string
        response = structured_llm.invoke(
            f"Extract the following resume details into structured JSON: {pdf_text}"
        )
        
        return response

    except Exception as e:
        print(f"ERROR OCCURRED: {e}") # This prints to your VS Code terminal
        return {"error": str(e)}
    
    finally:
        if os.path.exists(temp_path):
            os.remove(temp_path)

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)