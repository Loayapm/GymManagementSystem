# 🏋️ Gym Management System

A desktop application built using **C# (Windows Forms)** and **SQL Server (LocalDB)** to manage gym operations such as clients, subscriptions, and payments.

---

## 📌 Overview

This project was developed as part of my learning journey in software development. It demonstrates my ability to build a complete working system that interacts with a database and handles real-world scenarios like authentication and data management.

---

## ✨ Features

* 🔐 **User Authentication**

  * Secure login system using password hashing (PBKDF2)
  * Password change functionality

* 👤 **Client Management**

  * Add, update, and search for clients
  * Store personal and medical information

* 💳 **Subscription Management**

  * Track subscriptions and expiration dates
  * Alerts for expired memberships

* 💰 **Payment Tracking**

  * Record payments and expenses
  * Monitor financial activity

* 📊 **Basic Reporting & Search**

  * Search for clients and payments
  * View stored records efficiently

---

## 🛠️ Technologies Used

* **C#**
* **Windows Forms (WinForms)**
* **SQL Server (LocalDB)**
* **ADO.NET (SqlConnection, SqlCommand)**
* **Password Hashing (PBKDF2)**

---

## 🧠 What I Learned

* Connecting C# applications to a SQL database
* Handling user authentication securely (hashing + salt)
* Working with forms and event-driven programming
* Managing application state and user input
* Structuring a medium-sized project with multiple forms

---

## ⚠️ Notes & Limitations

* This project was created during my early learning stage, so some parts of the code are not fully optimized.
* The application does not follow a layered architecture (e.g., Service/Repository pattern).
* Some logic is directly written inside UI forms.

👉 I plan to improve these aspects in future projects using better design patterns and architecture.

---

## 🚀 Future Improvements

* Refactor code using **OOP principles** and **clean architecture**
* Separate business logic from UI (Service Layer)
* Improve UI/UX design
* Add validation and error handling
* Migrate to **ASP.NET** for a web-based version

---

## 📸 Screenshots

*(To be added)*

---

## ⚙️ How to Run

1. Clone the repository:

   ```bash
   git clone https://github.com/Loayapm/GymManagementSystem.git
   ```

2. Open the solution file:

   ```
   GymManagementSystem.sln
   ```

3. Make sure SQL Server LocalDB is installed

4. Update the connection string if needed (in `App.config`)

5. Run the project from Visual Studio

---

## 📁 Database

This project uses a local SQL Server database.

👉 You may need to:

* Create the database manually
* Or run the provided SQL script (if available)

---

## 👨‍💻 Author

**Loay**

Computer Science student passionate about backend development and building real-world systems.

---

## 📌 Final Note

This project represents an important step in my journey as a developer. While it is not perfect, it shows my ability to build a complete application and continuously improve my skills.

---
