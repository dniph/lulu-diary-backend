# 🤝 **Followers Model**

Represents follower relationships between profiles, where one profile follows another.

---

```ts
Followers {
  id: int;             // Unique identifier (auto-generated)
  profileId: int;      // The profile who is being followed (foreign key to Profiles)
  followerId: int;     // The profile who follows (foreign key to Profiles)
  createdAt: datetime; // Timestamp when the follow was created (UTC)
}
```

---

### 🧠 Business Logic & Example SQL

- Profiles cannot follow themselves (`profileId != followerId`).
- Each follower-followed pair must be unique (no duplicates).
- Only authenticated user can follow or unfollow others.

**Example SQL:**

```sql
-- Get all followers for a profile
SELECT * FROM Followers WHERE profileId = 123;

-- Get all profiles that X is following
SELECT * FROM Followers WHERE followerId = 123;
```