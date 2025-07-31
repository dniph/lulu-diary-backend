## 🤝 Friend Requests

### `POST /profiles/{profileId}/friend-requests`

* **Description:** Send a friend request **to** `{profileId}`.
* **Authentication:** Required (the authenticated user is the requester)
* **Request body:** None

* **Business Logic:**
  - Cannot send a friend request to self.
  - Fails if a pending or accepted friend request already exists between the profiles.

**Example SQL:**

```sql
-- Check for existing pending or accepted request
SELECT * FROM FriendRequests WHERE requesterProfileId = :currentUser AND requestedProfileId = :profileId AND status IN ('pending', 'accepted');
```

* **Response:** Success message or error details.

---

### `POST /profiles/{profileId}/friend-requests/{id}/accept`

* **Description:** Accept a friend request identified by `{id}`.
* **Authentication:** Required (only the requested profile can accept)
* **Request body:** None

* **Business Logic:**
  - Changes friend request status to "accepted".
  - Creates a friendship record.

**Example SQL:**

```sql
-- Accept a friend request
UPDATE FriendRequests SET status = 'accepted' WHERE id = :id AND requestedProfileId = :currentUser;
-- Insert into Friends
INSERT INTO Friends (profileA, profileB, createdAt) VALUES (:requesterProfileId, :requestedProfileId, CURRENT_TIMESTAMP);
```

* **Response:** Success confirmation or error.

---

### `POST /profiles/{profileId}/friend-requests/{id}/reject`

* **Description:** Reject or cancel a friend request identified by `{id}`.
* **Authentication:** Required (only requested profile can reject/cancel)
* **Request body:** None

* **Business Logic:**
  - Changes friend request status to "rejected".

**Example SQL:**

```sql
-- Reject a friend request
UPDATE FriendRequests SET status = 'rejected' WHERE id = :id AND requestedProfileId = :currentUser;
```

* **Response:** Success confirmation or error.