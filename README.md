# Binary Tech Hub — Full-Stack E-Commerce Platform

A 3-tier e-commerce web platform engineered for consumer electronics, computing hardware, and digital accessories. Built with a responsive frontend, an ASP.NET Core REST API, and a persistent MySQL relational database.

---

## 🏛️ System Architecture

- **Presentation Layer (Frontend)**: Semantic HTML5, CSS3 Grid/Flexbox, Vanilla JavaScript (ES6+), `localStorage` session state, EmailJS 2FA checkout.
- **Application Layer (Backend API)**: ASP.NET Core Web API (.NET 8/9), dependency injection, CORS middleware, transactional order management.
- **Data Persistence Layer (Database)**: MySQL Server 8.0, 1NF–3NF normalized relational schema (`categories`, `products`, `orders`, `order_items`), foreign key integrity, atomic stock management.

---

## ✨ Key Features

- **Dynamic Catalog**: Over 80 distinct products loaded dynamically via REST endpoints (`/api/products`).
- **Category Filtering**: Real-time filtering across 11 hardware and accessory categories.
- **Persistent Shopping Cart**: Live cart state preserved across page refreshes via `localStorage` with automated subtotal, 15% VAT, and total calculations.
- **Two-Factor Authentication (OTP Checkout)**: Customer checkout protected by real-time one-time PIN dispatch via EmailJS.
- **Atomic Transactions & Inventory Tracking**: Decrements inventory levels upon verified purchase, utilizing rollback mechanics on payment or database failure.

---

## 🚀 Getting Started

### 1. Database Configuration
1. Open **MySQL Workbench**.
2. Run the initialization script to establish the database:
   ```sql
   CREATE DATABASE binary_tech_hub;
   USE binary_tech_hub;