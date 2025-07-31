# 👤 **Profile Lookup Model**

Represents a lookup entity for profile entries.

---

```ts
ProfileUsernameLookup {
  id: string;            // Unique identifier (username)
  profileId: string;     // Profile id associated with the unique id (username)
}
```

---

### 🧠 Business Logic & Example SQL

- Used to quickly map a username to a profileId for authentication and profile lookups.
- `id` must match the username in the Profiles table and be unique.
- Keep this table in sync with the Profiles table when usernames are changed (if allowed).

**Example SQL:**

```sql
-- Lookup profileId by username
SELECT profileId FROM ProfileUsernameLookup WHERE id = 'someuser';
```