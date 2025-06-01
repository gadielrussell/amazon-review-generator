# 🛍️ Amazon Review Generator

**Amazon Review Generator** is a full-stack application that leverages a Large Language Model (LLM) to generate realistic Amazon-style product reviews. It processes a substantial dataset of Amazon reviews and provides a user-friendly interface for generating new reviews.

---

## 🚀 Features

- **LLM-Powered Review Generation**: Utilizes a fine-tuned LLM to produce human-like product reviews.
- **Extensive Dataset**: Ingests and processes several hundred thousand Amazon reviews for training and generation.
- **.NET Web API Backend**: Handles data processing and serves generated reviews through RESTful endpoints.
- **Angular Frontend**: Provides an interactive UI for users to generate and view reviews.

---

## 🧱 Tech Stack

- **Backend**: .NET Web API
- **Frontend**: Angular
- **Language Model**: Fine-tuned LLM (e.g., GPT-2)
- **Data Storage**: [Specify if using databases like PostgreSQL, MongoDB, etc.]
- **Deployment**: [Specify if deployed on platforms like Azure, AWS, etc.]

---

## 📁 Project Structure

```
amazon-review-generator/
├── AmazonReviewGenerator/        # .NET Web API backend
├── ARGApp/                       # Angular frontend application
├── data/                         # Dataset and preprocessing scripts
├── models/                       # Trained LLM models and configurations
├── .github/workflows/            # CI/CD workflows
└── README.md                     # Project documentation
```

---

## ⚙️ Setup Instructions

### Prerequisites

- **.NET SDK**: v6.0 or higher
- **Node.js & npm**: v14 or higher
- **Angular CLI**: v12 or higher
- **Python**: v3.8 or higher (for data preprocessing and model training)

### Backend (.NET Web API)

1. Navigate to the backend directory:
   ```bash
   cd AmazonReviewGenerator
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Run the application:
   ```bash
   dotnet run
   ```

### Frontend (Angular)

1. Navigate to the frontend directory:
   ```bash
   cd ARGApp
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Run the application:
   ```bash
   ng serve
   ```

### Data Preprocessing & Model Training

Is handled during App startup.

---

## 🧪 Usage

1. Start both backend and frontend applications as described above.
2. Open your browser and navigate to `http://localhost:4200`.
3. Use the interface to generate random Amazon-style product reviews.

---

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
