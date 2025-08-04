# ❤️ **DiaryReaction Model**

Tracks profile reactions to diary entries.

```ts
DiaryReaction {
  id: int;                // Unique identifier (auto-generated)
  diaryId: int;           // Diary being reacted to
  profileId: int;         // Profile who reacted (foreign key to Profiles)
  reactionType: string;   // Reaction type: "like", "love", "laugh", "sad", "angry", "surprised"
  createdAt: datetime;    // Reaction timestamp (UTC)
}
```

---

### 🧠 Business Logic & Example SQL (DiaryReaction)

- A profile can only have one active reaction per diary.
- Adding a reaction replaces any existing reaction for that profile and diary.
- Removing a reaction deletes the record for that profile and diary.

---

# 💬 **DiaryCommentReaction Model**

Tracks profile reactions to comments on diary entries.

```ts
DiaryCommentReaction {
  id: int;                // Unique identifier (auto-generated)
  diaryCommentId: int;    // Comment being reacted to
  profileId: int;         // Profile who reacted (foreign key to Profiles)
  reactionType: string;   // Reaction type
  createdAt: datetime;    // Reaction timestamp (UTC)
}
```

---

### 🧠 Business Logic & Example SQL (DiaryCommentReaction)

- A profile can only have one active reaction per comment (enforced by unique constraint in the DB or application logic).
- Adding a reaction replaces any existing reaction for that profile and comment.
- Removing a reaction deletes the record for that profile and comment.

---

### 🧠 Key Points

* Reaction entities are stored separately per resource type.

* API endpoints use **`react`** and **`unreact`** verbs for adding/removing reactions.

* Example endpoints:

  * `POST /profiles/{username}/diaries/{diaryId}/react` — Add reaction to diary
  * `POST /profiles/{username}/diaries/{diaryId}/unreact` — Remove reaction from diary
  * `POST /profiles/{username}/diaries/{diaryId}/comments/{commentId}/react` — Add reaction to comment
  * `POST /profiles/{username}/diaries/{diaryId}/comments/{commentId}/unreact` — Remove reaction from comment