## 👥 Follow / Unfollow

### `POST /profiles/{username}/follow`

* **Description:** Follow the profile identified by `{username}`.
* **Authentication:** Required (the authenticated user becomes follower)
* **Request body:** None

* **Business Logic:**
  - Cannot follow self.
  - Cannot create duplicate follow relationship.

**Example SQL:**

```sql
-- Check for existing follow
SELECT * FROM Followers WHERE username = :username AND follower = :currentUser;
```

* **Response:** Success confirmation or error.

---

### `POST /profiles/{username}/unfollow`

* **Description:** Unfollow the profile identified by `{username}`.
* **Authentication:** Required (the authenticated user is the follower)
* **Request body:** None

* **Business Logic:**
  - Removes existing follow relationship.

**Example SQL:**

```sql
-- Remove follow relationship
DELETE FROM Followers WHERE username = :username AND follower = :currentUser;
```

* **Response:** Success confirmation or error.