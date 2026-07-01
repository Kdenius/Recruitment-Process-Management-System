# Recruitment Process Management System (RPMS)

A Recruitment Process Management System designed to manage the hiring lifecycle from resume intake to interview scheduling and document verification.

> 🚧 **Status:** Partial Implementation (Work in Progress)

---

## 🛠️ Technology Stack

| Category        | Technologies               |
| --------------- | -------------------------- |
| Frontend        | React (Vite), Tailwind CSS |
| Backend         | .NET Web API               |
| Resume Parser   | FastAPI (Python)           |
| Email Service   | SMTP / Mail Service        |
| Version Control | Git & GitHub               |

---

## ✅ Implemented Features

- 🔐 **Authentication:** User registration/login with JWT-based auth
- 📄 **Resume Processing:** Resume upload → FastAPI parsing → structured candidate profile creation
- 👤 **Candidate Management:** Auto profile creation and centralized storage from parsed resumes
- 📅 **Interview Scheduling:** Schedule interviews with panel assignment and email notifications to stakeholders
- 📁 **Document Verification:** HR defines required documents, candidates upload, HR verifies and tracks status

---

## 📚 Documentation

All system requirement documents and technical details are available in the repository.

👉 View documentation:

- 📄 [Requirements Document](./docs/RPMS_Report.pdf)

## 📌 Note

This repository contains a **partial implementation** of the RPMS system. Core modules like authentication, resume parsing, candidate creation, interview scheduling, and document verification are implemented, while remaining recruitment lifecycle features are in progress.
