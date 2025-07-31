## ❤️ Reactions (React / Unreact)

### `POST /profiles/{username}/diaries/{diaryId}/react`

* **Description:** Add a reaction to a diary.
* **Authentication:** Required (the authenticated user is the reactor)
* **Request body:**

```json
{
  "reactionType": "like" // Required, type of reaction (e.g., "like", "love", etc.)
}
```

* **Business Logic:**
  - A profile can only have one active reaction per diary.
  - Sending a new reaction will replace any existing reaction.
  - Duplicate requests with the same reaction type are idempotent.

**Example SQL:**

```sql
-- Add or update a reaction (upsert logic)
INSERT INTO DiaryReaction (diaryId, profile, reactionType)
VALUES (:diaryId, :profile, :reactionType)
ON CONFLICT(diaryId, profile) DO UPDATE SET reactionType = excluded.reactionType, createdAt = CURRENT_TIMESTAMP;
```

* **Response:** Success confirmation or error.

---

### `POST /profiles/{username}/diaries/{diaryId}/unreact`

* **Description:** Remove the profile’s reaction from a diary.
* **Authentication:** Required (the authenticated user is the reactor)
* **Request body:** None

* **Business Logic:**
  - Removes the profile’s existing reaction on the diary, regardless of type.
  - No effect if the profile has not reacted.

**Example SQL:**

```sql
-- Remove a reaction
DELETE FROM DiaryReaction WHERE diaryId = :diaryId AND profile = :profile;
```

* **Response:** Success confirmation or error.

---

### `POST /profiles/{username}/diaries/{diaryId}/comments/{commentId}/react`

* **Description:** Add a reaction to a comment on a diary.
* **Authentication:** Required (the authenticated user is the reactor)
* **Request body:**

```json
{
  "reactionType": "like" // Required, type of reaction
}
```

* **Business Logic:**
  - A profile can only have one active reaction per comment.
  - Sending a new reaction will replace any existing reaction.
  - Duplicate requests with the same reaction type are idempotent.

**Example SQL:**

```sql
-- Add or update a reaction (upsert logic)
INSERT INTO DiaryCommentReaction (diaryCommentId, profile, reactionType)
VALUES (:diaryCommentId, :profile, :reactionType)
ON CONFLICT(diaryCommentId, profile) DO UPDATE SET reactionType = excluded.reactionType, createdAt = CURRENT_TIMESTAMP;
```

* **Response:** Success confirmation or error.

---

### `POST /profiles/{username}/diaries/{diaryId}/comments/{commentId}/unreact`

* **Description:** Remove the profile’s reaction from a comment on a diary.
* **Authentication:** Required (the authenticated user is the reactor)
* **Request body:** None

* **Business Logic:**
  - Removes the profile’s existing reaction on the comment, regardless of type.
  - No effect if the profile has not reacted.

**Example SQL:**

```sql
-- Remove a reaction
DELETE FROM DiaryCommentReaction WHERE diaryCommentId = :diaryCommentId AND profile = :profile;
```

* **Response:** Success confirmation or error.