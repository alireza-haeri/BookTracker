# 📚 BookTracker API

BookTracker is a **practical backend project** built with **ASP.NET Core Minimal API**, **MongoDB**, and **JWT authentication**.  
It was designed both as a **portfolio project** and as a way to gain **hands-on experience** with MongoDB, authentication, and modern .NET practices.  


## 🚀 Features
- 🔐 **User Authentication** with JWT  
  - Register and login with phone number and password  
  - Secure token-based access to protected endpoints  
- 📖 **Book Management**  
  - Add, update, delete, and fetch books  
  - Retrieve a personal list of books belonging to a user  
- ✅ **Input Validation** using FluentValidation  
- 📑 **API Documentation** powered by Scalar  


## 🛠️ Tech Stack
- **ASP.NET Core Minimal API** (for lightweight endpoints)  
- **MongoDB** (document-oriented database)  
- **JWT Authentication** (secure user sessions)  
- **FluentValidation** (clean validation layer)  
- **Scalar** (modern API documentation)  


## 📌 Endpoints (v1)

### 👤 User
- `POST /user` → Register new user  
- `POST /user/login` → Login and receive JWT  

### 📚 Book
- `POST /book` → Add a new book  
- `GET /book/{id}` → Get book by ID  
- `PUT /book/{id}` → Update book by ID  
- `DELETE /book/{id}` → Delete book by ID  
- `GET /user/books` → Get all books for current user  


## ⚙️ Notes
- The focus of this project is **practical learning**, not applying a complex architecture.  
- The code is kept **modular and clean**, making it easy to extend later.  
- It demonstrates real-world usage of MongoDB, JWT authentication, and Minimal APIs.  


## ✨ Personal Takeaway
Building **BookTracker** was a valuable experience that helped me:  
- Understand MongoDB’s document-based design  
- Implement JWT authentication in practice  
- Explore the simplicity and flexibility of Minimal APIs  
- Create a complete, working project that can be showcased in my portfolio  


## 📫 Let's Connect!

Feel free to reach out if you want to collaborate on a project or just want to chat.

- 🔗 **LinkedIn:** [in/alireza-haeri-dev](https://www.linkedin.com/in/alireza-haeri-dev)  
- 🔗 **Telegram:** [@AlirezaHaeriDev](https://t.me/AlirezaHaeriDev)  
- 🔗 **Email:** alireza.haeri.dev@gmail.com  

