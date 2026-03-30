# 🏋️ Gym Management System

A desktop application built using **C# (Windows Forms)** and **SQL Server (LocalDB)** to manage gym operations such as clients, subscriptions, and payments.

---

## 📌 Overview

This project was developed as part of my learning journey in software development. It demonstrates my ability to build a complete working system that interacts with a database and handles real-world scenarios like authentication and data management.

---

## ✨ Features

### 🔐 Login
- **Login Screen**  
  ![Login](Screenshots/Login/login1.png)
- **Login Screen 2**  
  ![Login](Screenshots/Login/login2.png)

### 👥 Coaches
- **Add Coach**  
  ![Add Coach](Screenshots/Coaches/add.png)
- **Edit Coach**  
  ![Edit Coach](Screenshots/Coaches/edit.png)
- **Delete Coach**  
  ![Delete Coach](Screenshots/Coaches/delete.png)
- **Coaches Table**  
  ![Coaches Table](Screenshots/Coaches/table.png)

### 👤 Subscribers
- **Add Subscriber**  
  ![Add Subscriber](Screenshots/Subscribers/add.png)
- **Edit Subscriber**  
  ![Edit Subscriber](Screenshots/Subscribers/edit.png)
- **Delete Subscriber**  
  ![Delete Subscriber](Screenshots/Subscribers/delete.png)
- **Subscribers Table**  
  ![Subscribers Table](Screenshots/Subscribers/table.png)

### 📅 Attendance
- **Add Attendance**  
  ![Add Attendance](Screenshots/Attendance/add.png)
- **Delete Attendance**  
  ![Delete Attendance](Screenshots/Attendance/delete.png)
- **Attendance Table**  
  ![Attendance Table](Screenshots/Attendance/table.png)

### 💳 Subscriptions
- **Add Subscription**  
  ![Add Subscription](Screenshots/Subscriptions/add.png)
- **Edit Subscription**  
  ![Edit Subscription](Screenshots/Subscriptions/edit.png)
- **Delete Subscription**  
  ![Delete Subscription](Screenshots/Subscriptions/delete.png)
- **Move Subscription to New Coach**  
  ![Move to New Coach](Screenshots/Subscriptions/moveToNewCoach.png)
- **Pause Subscription**  
  ![Pause Subscription](Screenshots/Subscriptions/pause.png)
- **Pause Subscription 2**  
  ![Pause Subscription 2](Screenshots/Subscriptions/pause2.png)
- **Search Subscriptions**  
  ![Search Subscriptions](Screenshots/Subscriptions/search.png)

### 💰 Payments
- **Add Payment**  
  ![Add Payment](Screenshots/Payments/add.png)
- **Edit Payment**  
  ![Edit Payment](Screenshots/Payments/edit.png)
- **Delete Payment**  
  ![Delete Payment](Screenshots/Payments/delete.png)

### 💸 Payroll Advance
- **Add Payroll Advance**  
  ![Add Payroll Advance](Screenshots/Payroll Advance/add1.png)
- **Add Payroll Advance 2**  
  ![Add Payroll Advance 2](Screenshots/Payroll Advance/add2.png)
- **Delete Payroll Advance**  
  ![Delete Payroll Advance](Screenshots/Payroll Advance/delete.png)
- **Payroll Advance Table**  
  ![Payroll Advance Table](Screenshots/Payroll Advance/table.png)

### 🩺 Medical Records
- **Add Medical Record**  
  ![Add Medical Record](Screenshots/Medical Records/add.png)
- **Edit Medical Record**  
  ![Edit Medical Record](Screenshots/Medical Records/edit.png)
- **Delete Medical Record**  
  ![Delete Medical Record](Screenshots/Medical Records/delete.png)
- **Medical Records Table**  
  ![Medical Records Table](Screenshots/Medical Records/table.png)

### 🛒 Supplies
- **Add Supply**  
  ![Add Supply](Screenshots/Supplies/add.png)
- **Delete Supply**  
  ![Delete Supply](Screenshots/Supplies/delete.png)
- **Supplies Table**  
  ![Supplies Table](Screenshots/Supplies/table.png)

### 📋 Sections
- **Add Section**  
  ![Add Section](Screenshots/Sections/add.png)
- **Edit Section**  
  ![Edit Section](Screenshots/Sections/edit.png)
- **Delete Section**  
  ![Delete Section](Screenshots/Sections/delete.png)

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
