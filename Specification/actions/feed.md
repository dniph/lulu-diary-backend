## 📢 Feed

### `GET /feed`

* **Description:** Retrieve the public diary feed from all users or diaries the authenticated user has access to.
* **Authentication:** Supports authenticated and unauthenticated users
* **Response:** List of public diary entries, typically paginated.

---

### 🧠 Business Logic & Example SQL

- Unauthenticated users see only public diaries.
- Authenticated users see public, their own private, and friends-only diaries for users they follow.
- Pagination should be implemented for performance.
- Diaries should be ordered by `createdAt` (most recent first).