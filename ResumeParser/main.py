import os
from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, Field, EmailStr, conint, confloat
from typing import List, Optional
import shutil
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain_community.document_loaders import PyPDFLoader
from dotenv import load_dotenv
from enum import Enum

load_dotenv()

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

skills_list = [
    "Python", "Java", "C++", "JavaScript", "TypeScript", "C#", "Go", "Rust", "SQL", "Ruby",
    "PHP", "Swift", "Kotlin", "R", "Scala", "Dart", "Haskell", "Perl", "Shell Scripting", "Bash",
    "Objective-C", "VBA", "React", "Angular", "Vue.js", "Node.js", "Express.js", "Next.js",
    "Gatsby", "Redux", "MobX", "Svelte", "jQuery", "Bootstrap", "Tailwind CSS", "SASS", "Less",
    "HTML5", "CSS3", "WebGL", "Django", "Flask", "Spring Boot", "ASP.NET", ".NET Core", "Laravel",
    "Ruby on Rails", "FastAPI", "Koa", "Gin", "REST API", "GraphQL", "SOAP", "Machine Learning",
    "Deep Learning", "Data Science", "Natural Language Processing", "Computer Vision",
    "Reinforcement Learning", "Pandas", "NumPy", "Matplotlib", "Scikit-learn", "TensorFlow",
    "PyTorch", "Keras", "OpenCV", "NLTK", "SpaCy", "Seaborn", "Statsmodels", "A/B Testing",
    "Time Series Analysis", "PostgreSQL", "MySQL", "MongoDB", "Cassandra", "Redis",
    "Elasticsearch", "SQLite", "Oracle", "SQL Server", "DynamoDB", "Neo4j", "AWS", "Azure",
    "Google Cloud", "GCP", "Docker", "Kubernetes", "Terraform", "Ansible", "Chef", "Puppet",
    "Jenkins", "GitLab CI", "GitHub Actions", "CI/CD", "Prometheus", "Grafana", "Splunk",
    "Linux", "Serverless", "Vagrant", "OpenStack", "RabbitMQ", "Kafka", "Message Queuing",
    "Git", "Jira", "Confluence", "Agile", "Scrum", "Kanban", "Project Management", "Figma",
    "Sketch", "UI/UX", "Quality Assurance", "Testing", "Microservices", "Blockchain",
    "Ethereum", "Solidity", "Unit Testing", "Integration Testing", "Flutter", "React Native",
    "Ionic", "Mobile App Development", "Cybersecurity", "Ethical Hacking",
    "Penetration Testing", "SIEM", "Apache Spark", "Apache Airflow", "Snowflake",
    "Data Engineering", "Large Language Models (LLMs)", "Generative AI", "LangChain",
    "Prompt Engineering", "Web3", "Smart Contracts", "Cypress", "Playwright", "Socket.io"
]

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


llm = ChatGoogleGenerativeAI(model="gemini-2.5-flash-lite")
structured_llm = llm.with_structured_output(Resume)

@app.post("/parse-resume")
async def parse_resume(file: UploadFile = File(...)):
    temp_path = f"temp_{file.filename}"
    try:
        content = await file.read()
        with open(temp_path, "wb") as f:
            f.write(content)

        loader = PyPDFLoader(temp_path)
        pages = loader.load()
        pdf_text = " ".join([p.page_content for p in pages])

        if not pdf_text.strip():
            raise HTTPException(status_code=400, detail="PDF is empty or unreadable")

        if not os.getenv("GOOGLE_API_KEY"):
            return {"error": "API Key not found in environment"}

        response = structured_llm.invoke(
            f"""
            Extract the resume into structured JSON.

            IMPORTANT:
            - The "skills" field MUST only contain values from this allowed list:
            {skills_list}

            Resume Text:
            {pdf_text}
            """
        )
        
        return response

    except Exception as e:
        print(f"ERROR OCCURRED: {e}")
        return {"error": str(e)}
    
    finally:
        if os.path.exists(temp_path):
            os.remove(temp_path)

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)