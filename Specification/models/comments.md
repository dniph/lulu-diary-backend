# 💬 **Diary Comments Model**

Represents comments left by profiles on diary entries.

```ts
Comments {
  id: int;               // Unique identifier (auto-generated)
  diaryId: int;          // Identifier of the diary this comment belongs to
  profileId: int;        // Identifier of the profile who made the comment (foreign key to Profiles)
  content: string;       // Text content of the comment
  createdAt: datetime;   // Timestamp when the comment was created (UTC)
  updatedAt: datetime;   // Timestamp when the comment was last updated (optional)
}
```

---

### 🧠 Business Logic & Example SQL

- Each comment is associated with exactly one diary entry (`diaryId`).
- Only the comment owner (`profileId`) or the diary owner can edit or delete the comment.
- Comments are ordered by `createdAt` by default.
- `content` must be non-empty and may have a length limit (e.g., 2000 characters).

**Example SQL:**

```sql
-- Get all comments for a diary, ordered by creation time
SELECT * FROM Comments WHERE diaryId = 123 ORDER BY createdAt ASC;
```
