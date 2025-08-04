# 💬❤️ **CommentReaction Model**

## Database Schema

```ts
CommentReaction {
  id: int;                // Unique identifier (auto-generated)
  commentId: int;         // Comment being reacted to (foreign key to Comments)
  diaryId: int;           // Diary that contains the comment (foreign key to Diaries)
  profileId: int;         // Profile who reacted (foreign key to Profiles)
  reactionType: string;   // Reaction type: "like", "love", "laugh", "sad", "angry", "surprised"
}
```

---

### 🧠 Business Logic & Example SQL (CommentReaction)

* **One reaction per profile per comment**: A profile can only have one active reaction per comment.
* **Reactions replace each other**: If a profile reacts with "like" and then "love", the "like" is replaced.
* **Idempotent operations**: Multiple identical reactions do not create duplicates.

**Example SQL:**
```sql
-- Add or replace reaction
INSERT INTO CommentReaction (commentId, diaryId, profileId, reactionType)
VALUES (:commentId, :diaryId, :profileId, :reactionType)
ON CONFLICT (commentId, profileId) 
DO UPDATE SET reactionType = EXCLUDED.reactionType;

-- Remove reaction
DELETE FROM CommentReaction 
WHERE commentId = :commentId AND profileId = :profileId;

-- Get all reactions for a comment
SELECT * FROM CommentReaction 
WHERE commentId = :commentId 
ORDER BY id DESC;

-- Get all reactions for comments in a diary
SELECT * FROM CommentReaction 
WHERE diaryId = :diaryId 
ORDER BY id DESC;
```

---

### 🔗 **Relationships**

* **CommentReaction.commentId** → **Comment.id** (Many-to-One)
* **CommentReaction.diaryId** → **Diary.id** (Many-to-One)
* **CommentReaction.profileId** → **Profile.id** (Many-to-One)

---

### ✅ **Validation Rules**

* `reactionType` must be one of: "like", "love", "laugh", "sad", "angry", "surprised"
* `commentId` must reference an existing comment
* `profileId` must reference an existing profile
* Case-insensitive validation (stored as lowercase)

---

### 📋 **API Integration**

See [Comment Reactions API](../actions/comment-reactions.md) for endpoint details.

**Key Features:**
- ✅ One reaction per profile per comment
- ✅ Six reaction types with emoji support  
- ✅ Real-time reaction updates
- ✅ Respects comment and diary visibility rules
