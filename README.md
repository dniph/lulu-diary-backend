## 🚀 ASP.NET Core API – Setup & Development Guide

This guide helps developers set up a local development environment and extend the database with new models using Entity Framework Core.

---

### 🛠️ Prerequisites

- [.NET SDK 9.0+](https://dotnet.microsoft.com/download/dotnet/9.0)
- PostgreSQL (any local setup or Docker-based)
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

```bash
dotnet tool install --global dotnet-ef
```

---

### ⚙️ Environment Setup

#### 1. **Clone the Repository**

```bash
git clone https://github.com/dniph/lulu-diary-backend.git
cd lulu-diary-backend
```

#### 2. **Create a `.env` File**

Copy the `.env.template` (if available) or create a new `.env`:

```
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=mydb;Username=myuser;Password=mypass
```

#### 3. **Trust the Developer Certificate (if needed)**

```bash
dotnet dev-certs https --trust
```

### 🧱 Adding a New Model to the Database

Let’s say you want to add a `Book` model.

#### 1. **Create a New Entity**

Create a C# class in `Models/Database/Book.cs`:

```csharp
public class Book
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
}
```

#### 2. **Update the `AppDbContext`**

In `Context/AppDbContext.cs`:

```csharp
public DbSet<Book> Books { get; set; }
```

#### 3. **Add a Migration**

```bash
dotnet ef migrations add CreateDiaryTable
```

#### 4. **Apply the Migration**

```bash
dotnet ef database update
```

> The database schema will be updated to include the new `Book` table.

---

### 📦 Deployment Notes

- Configure environment variables (e.g., in Azure App Service) instead of `.env` in production.
- Use `dotnet publish` for deployment-ready builds.