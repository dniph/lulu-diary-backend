# 👥 **Friends Model**

Represents a confirmed friendship between two profiles.

```ts
Friends {
  id: int;             // Unique identifier (auto-generated)
  profileAId: int;     // One profile in the friendship (foreign key to Profiles)
  profileBId: int;     // The other profile in the friendship (foreign key to Profiles)
  createdAt: datetime; // Timestamp when friendship was established (UTC)
}
```

### 🧠 Business Logic & Example SQL (Friends)

- Friendships are bidirectional: a record for (A, B) means A and B are friends.
- Enforce `profileAId != profileBId` to prevent self-friendship.
- Only one friendship record per pair (unique constraint).

**Example SQL:**

```sql
-- Get all friends of profile X
SELECT * FROM Friends WHERE profileAId = 123 OR profileBId = 123;
```

---

# 📧 **Friend Requests Model**

Represents pending friend requests between profiles.

```ts
FriendRequests {
  id: int;                  // Unique identifier (auto-generated)
  requesterProfileId: int;  // Profile who sent the friend request (foreign key to Profiles)
  requestedProfileId: int;  // Profile who received the friend request (foreign key to Profiles)
  status: string;           // "pending", "accepted", or "rejected"
  createdAt: datetime;      // Timestamp when request was sent (UTC)
}
```

### 🧠 Business Logic & Example SQL (FriendRequests)

- Only one pending request can exist between two profiles at a time.
- Requests cannot be sent to oneself.
- When accepted: create a Friends record and update status to "accepted".
- When rejected or cancelled: update status to "rejected".
- Only the requested profile can accept or reject a request.

**Example SQL:**

```sql
-- Get incoming pending requests for profile X
SELECT * FROM FriendRequests WHERE requestedProfileId = 123 AND status = 'pending';
```