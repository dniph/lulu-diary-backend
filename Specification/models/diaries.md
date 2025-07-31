# 📔 **Diary Model**

Represents a profile's personal diary entry. Each diary entry can include a title, content, and a visibility level determining who can access it.

---

```ts
Diary {
  id: int;                // Unique identifier (auto-generated)
  title: string;          // Required title of the diary
  content: string;        // Required diary body
  profileId: int;         // Owner of the diary (foreign key to Profiles)
  visibility: string;     // One of: "public", "friends-only", "private"
  createdAt: datetime;    // Creation timestamp (UTC), auto-generated
  updatedAt?: datetime;   // Last update timestamp (UTC, auto-generated if updated)
}
```

---

### 🧠 Business Logic & Example SQL

- Only the diary owner can create, update, or delete their diaries.
- Visibility rules must be enforced in all read endpoints.
- `createdAt` and `updatedAt` are managed by the database or application logic.
- `title` and `content` are required and should be validated for length and content.

**Example SQL:**

```sql
-- Get all diaries for a profile
SELECT * FROM Diary WHERE profileId = 123;
```