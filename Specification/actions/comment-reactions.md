# 💬❤️ **Comment Reactions API**

## 1. Add/Update Comment Reaction

* **Method:** `POST`
* **Endpoint:** `/api/profiles/{username}/diaries/{diaryId}/comments/{commentId}/react`
* **Description:** Add a reaction to a comment.
* **Authentication:** Required (the authenticated user is the reactor)
* **Request body:**

```json
{
  "reactionType": "love" // Required, type of reaction: "like", "love", "laugh", "sad", "angry", "surprised"
}
```

* **Business Logic:**
  - A profile can only have one active reaction per comment.
  - Sending a new reaction will replace any existing reaction.
  - Duplicate requests with the same reaction type are idempotent.
  - Comment must exist and belong to the specified diary.
  - Diary must exist and belong to the specified profile.

**Example SQL:**
```sql
INSERT INTO CommentReaction (commentId, diaryId, profileId, reactionType)
VALUES (:commentId, :diaryId, :profileId, :reactionType)
ON CONFLICT (commentId, profileId) 
DO UPDATE SET reactionType = EXCLUDED.reactionType;
```

**Response Examples:**

✅ **Success (200 OK):**
```json
{
  "id": 123,
  "commentId": 456,
  "diaryId": 789,
  "profileId": 101,
  "reactionType": "love"
}
```

❌ **Validation Error (400 Bad Request):**
```json
{
  "message": "Invalid reaction type 'invalid'. Must be one of: like, love, laugh, sad, angry, surprised"
}
```

❌ **Comment Not Found (404 Not Found):**
```json
{
  "message": "Comment not found."
}
```

---

## 2. Remove Comment Reaction

* **Method:** `POST`
* **Endpoint:** `/api/profiles/{username}/diaries/{diaryId}/comments/{commentId}/unreact`
* **Description:** Remove the authenticated user's reaction from a comment.
* **Authentication:** Required (the authenticated user is the reactor)
* **Request body:** None

**Example SQL:**
```sql
DELETE FROM CommentReaction WHERE commentId = :commentId AND profileId = :profileId;
```

**Response Examples:**

✅ **Success (204 No Content):**
```
(Empty body)
```

❌ **No Reaction Found (404 Not Found):**
```json
{
  "message": "No reaction found to remove."
}
```

---

## 3. Get Comment Reactions

* **Method:** `GET`
* **Endpoint:** `/api/profiles/{username}/diaries/{diaryId}/comments/{commentId}/reactions`
* **Description:** Get all reactions for a specific comment.
* **Authentication:** Not required (public, but respects diary visibility)

**Example SQL:**
```sql
SELECT * FROM CommentReaction 
WHERE commentId = :commentId 
ORDER BY id DESC;
```

**Response Examples:**

✅ **Success (200 OK):**
```json
[
  {
    "id": 123,
    "commentId": 456,
    "diaryId": 789,
    "profileId": 101,
    "reactionType": "love"
  },
  {
    "id": 124,
    "commentId": 456,
    "diaryId": 789,
    "profileId": 102,
    "reactionType": "like"
  }
]
```

✅ **No Reactions (200 OK):**
```json
[]
```

---

## 🔐 **Access Control**

* All endpoints verify that the comment exists and belongs to the specified diary
* All endpoints verify that the diary exists and belongs to the specified profile
* Reactions respect the diary's visibility settings (public, friends-only, private)
* Only authenticated users can add/remove reactions
* Users can only manage their own reactions

---

## 📊 **Business Rules**

1. **One Reaction Per User**: Each profile can have at most one reaction per comment
2. **Reaction Replacement**: New reactions replace existing ones from the same user
3. **Idempotent Operations**: Duplicate reactions don't create errors
4. **Visibility Inheritance**: Comment reactions inherit diary visibility rules
5. **Valid Reaction Types**: Only predefined reaction types are accepted

---

## 🎭 **Supported Reaction Types**

| Type | Emoji | Label | Use Case |
|------|-------|-------|----------|
| `like` | 👍 | Me gusta | General approval |
| `love` | ❤️ | Me encanta | Strong positive feeling |
| `laugh` | 😄 | Divertido | Funny/humorous content |
| `sad` | 😢 | Triste | Sad/sympathetic response |
| `angry` | 😠 | Enojado | Disagreement/frustration |
| `surprised` | 😮 | Sorprendido | Unexpected/shocking content |
