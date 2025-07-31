# 👤 **Profile Model**

Represents a public-facing profile. Profiles are linked to user accounts and contain social, visibility, and personal metadata.

---

```ts
Profile {
  id: int;                  // Unique identifier (auto-generated, primary key)
  userId: string;           // Unique identifier (user id)
  username: string;         // Unique username (used for login/profile URLs)
  displayName: string;      // Public display name
  avatarUrl: string;        // Optional profile picture URL
  createdAt: datetime;      // Account creation timestamp (UTC)
  friendsCount: int;        // Number of accepted friends
  followersCount: int;      // Number of followers
  followingCount: int;      // Number of accounts the profile is following
  diaryVisibility: string;  // One of: "public", "private"
}
```

---

### 🧠 Business Logic & Example SQL

- `username` must be unique and is used for login and profile URLs.
- `diaryVisibility` controls the default privacy for diaries.
- `friendsCount`, `followersCount`, and `followingCount` are updated when relationships change.
- Only the profile owner can update their own profile (except for public GET).

**Example SQL:**

```sql
-- Get a profile by profileId
SELECT * FROM Profiles WHERE profileId = 123;

-- Get a profile by username
SELECT * FROM Profiles WHERE username = 'someuser';

-- Update display name
UPDATE Profiles SET displayName = 'New Name' WHERE profileId = 123;
```